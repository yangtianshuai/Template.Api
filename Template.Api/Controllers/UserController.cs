using Api.Config;
using Api.Config.Cache;
using Api.Config.Filter;
using Api.Config.Open;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Template.Data.Interface;

namespace Template.Api.Controllers
{
    [Route("user")]
    public class UserController : ApiCorsController
    {
        private readonly ServerSession _serverSession;
       
        private readonly IUserRepository _userReposiotry;     

        public UserController(ServerSession serverSession, IUserRepository userReposiotry)
        {
            _serverSession = serverSession;
            _userReposiotry = userReposiotry; 
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var result = new ResponseResult();
            return result.ToJson();
        }

        [HttpGet("test2")]
        public async Task<IActionResult> Test2()
        {
            var result = new ResponseResult();
            var http = await OpenApi.Get("id").GetAsync("");
            return result.ToJson();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user_name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        [HttpPost("login")]
        [NoAuthorization]
        public async Task<IActionResult> Login(string user_name, string pwd)
        {
            var result = new ResponseResult();

            var user = await _userReposiotry.FirstOrDefaultAsync(
                t => t.cellpone == user_name && t.password == pwd && t.status == 1);
            if (user != null)
            {
                await _serverSession.SetSessionAsync(new Session
                {
                    Token = Guid.NewGuid().ToString(),
                    Create_Time = DateTime.Now
                });
                result.Data = user.id;
            }
            else
            {
                result.Message = "用户名或者密码不正确";
            }
            return result.ToJson();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync()
        {
            var result = new ResponseResult();

            
            return result.ToJson();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="user_name">用户名</param>
        /// <returns></returns>
        [HttpPost("GetUser")]       
        public async Task<IActionResult> GetUser(string user_name)
        {
            var result = new ResponseResult();           
             
             var user = await _userReposiotry.FirstOrDefaultAsync(t => t.cellpone == user_name);
            if (user != null)
            {
                result.Data = new
                {
                    user.id,
                    user.register_time
                };
            }
            else
            {
                result.Message = "用户名不存在";
            }
            return result.ToJson();
        }
    }
}