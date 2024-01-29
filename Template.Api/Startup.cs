using Api.Config;
using Api.Config.Cache;
using Api.Config.DI;
using Api.Config.Net;
using Api.Config.Proxy;
using Api.Config.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using NLog.Web;
using SSO.Client;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using Template.Data.DB;

namespace Template.Api
{
    public class Startup
    {       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }       
        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            //var env = Environment.GetEnvironmentVariable("Environment");
            //var app_id = Environment.GetEnvironmentVariable("AppId");

            var env = Configuration.GetSection("Environment").Value;
           
            services.AddSetting(Configuration, env);

            var app_id = AppSetting.GetSetting("AppID");

            //services.AddSetting(Configuration).Remote(option =>
            //{                
            //    option.PullTicks = 60;
            //    option.DowLoad = (context) =>
            //    {
            //        //app_id、secret、env、code
            //        //从配置中心加载配置                   
            //        return "";
            //    };
            //});

            //配置数据库连接
            services.AddDbContext<SampleContext>(options =>
            {
                //options.EnableSensitiveDataLogging(true); 开启全部SQL日志
                options.UseSqlServer(AppSetting.GetSetting("DatabaseConnections:Connection"));
            });

            //设置DI
            services.SetDI(options =>
            {
                options.Add(
                    typeof(DotNetCore.Repository.IRepository),//仓储层
                    typeof(IocService)//服务层
                );
            });

            //内存缓存
            //services.AddCache();

            //Redis缓存
            services.AddDistributedRedisCache(option =>
            {
                //redis 连接字符串
                option.Configuration = AppSetting.GetSetting("RedisConnections:Connection");
                //redis 实例名
                option.InstanceName = $"Template.Api-{env}-";
            }).AddRedisCache();

            //开启Session缓存
            //services.AddMemorySession();

            services.AddMemorySession(options =>
            {
                options.TokenKey = "Template_Token";
                //开启Redis缓存Session
                options.Service = typeof(RedisSessionService);
            });

            //添加SSO
            services.AddSSO(options =>
            {
                //Cas验证基本URL
                options.BaseURL = AppSetting.GetSetting("Sso:BaseURL");
                //采用自定义登录页面，需要设置此项
                //options.LoginURL = AppSetting.GetSetting("Sso:LoginURL");              
                options.Mode = SsoMode.Proxy;//前后端分离，后端接入使用代理模式
                options.AppID = app_id;//当前应用ID
                //用于IP映射规则转换（特别是针对内外网问题）
                options.AddIpMappings(AppSetting.GetSetting<IPMapping[]>("Sso:Mappings"));
            }).UseOAuth2();

            //设置HTTP请求
            services.SetHttp(options =>
            {
                options.TimeOut = 30;//请求超时时间
                options.SuccessOnly = false;//默认true只有响应200，可以收到信息
            });

            services.AddMvc(options =>
            {
                //添加过滤器                         
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ActionFilter>();
                //开启SSO
                options.Filters.Add<SSOFilter>();       
                //开启安全验证
                options.Filters.Add<AuthorizationAttribute>();
                options.UseCentralRoutePrefix(new RouteAttribute("api"));

            }).AddJsonOptions(options =>
            {
                //设置时间格式
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Json 全局设置
            var setting = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                //日期类型默认格式化处理
                setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                //空值处理
                setting.NullValueHandling = NullValueHandling.Ignore;
                return setting;
            });

            services.AddStorage(option =>
            {
                option.AppId = app_id;
                option.SetBaseUrl(AppSetting.GetSetting("Storage:BaseUrl")).UserHttp();
            });

            //添加跨域解决方案
            services.AddCors2();

            services.AddProxy(option =>
            {
                option.Host(AppSetting.GetSetting<List<ProxyHost>>("Proxy:Hosts"));
            });

            services.AddOpen(options =>
            {
                //设置app_id
                options.SetAppID(app_id);

                options.DownLoad = async (_options) => {                                              
                    var url = string.Format(AppSetting.GetSetting("Open:Authorize")
                        , OpenOptions.AppID
                        , HttpUtility.UrlEncode(AppSetting.GetSetting("Open:RedirectUri")));
                    //http://ip:port/open/authorize?app_id={0}&redirect_uri={1}&type=uri
                    await HttpHelper.GetAsync(url);
                };

                //本地设置
                //options.Add("/api/user/GetUser", "access_token", new List<string>() { "bdab37c28480474ea93ecef6d644e265" });
            });

            //添加Swagger (提供WebAPI接口)
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1.0",
                    Title = "Template API文档",
                    Description = "API文档"
                });

                // 配置生成的 xml 注释文档路径
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, MethodBase.GetCurrentMethod().DeclaringType.Namespace + ".xml");
                c.IncludeXmlComments(xmlPath);
            });
        }
       
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddNLog();
            env.ConfigureNLog("nlog.config");
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //自定义路由拦截
                routes.Routes.Add(new MvcRouter());
            });

            app.UseCors("any");

            app.UseMiddleware<ProxyMiddleware>();            

            //添加Swagger及默认UI
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                var virtualPath = Configuration.GetSection("Virtual").Value;
                string path = "/swagger/v1/swagger.json";              
                c.DocumentTitle = "Template API Doc";
                if (virtualPath != null)
                {
                    path = virtualPath + path;
                }
                c.SwaggerEndpoint(path, "Template API V1");
            });
        }
    }
}