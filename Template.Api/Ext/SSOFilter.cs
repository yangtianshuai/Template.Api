using Api.Config;
using Api.Config.Open;
using Api.Config.Sso;
using SSO.Client;
using System.Threading.Tasks;

namespace Template.Api
{
    public class SSOFilter : SsoFilter
    {       
        private readonly ServerSession _session;

        public SSOFilter(ISsoHandler casHandler, ServerSession session) :base(casHandler)
        {
            _session = session;
        }
        public override async Task ValidateComplate(SsoCookie cookie)
        {           
            //加入Session
            if (!await _session.ContainAsync(cookie.ID))
            {
                var session = new Session
                {
                    Token = cookie.ID
                };

                //await OpenApi.Get("服务ID").GetAsync();
                await _session.SetSessionAsync(session);
            }
        }
        public override async Task LogoutComplate(SsoCookie cookie)
        {            
            //取消Session
            if (await _session.ContainAsync(cookie.ID))
            {                
                await _session.ClearSessionAsync(cookie.ID);
            }
        }
    }
}