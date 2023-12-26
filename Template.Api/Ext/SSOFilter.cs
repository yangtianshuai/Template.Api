using Api.Config;
using Api.Config.Sso;
using Microsoft.AspNetCore.Http;
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
            var session = await _session.GetSessionAsync<Session>(cookie.ID);
            //加入Session
            if (session == null)
            {
                session = new Session
                {
                    Token = cookie.ID
                };
                // await OpenApi.Get("服务ID").GetAsync();

                await _session.SetSessionAsync(session);
            }
            else
            {
                if (!_session.ContainToken(session))
                {
                    _session.SetToken(session);
                }
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

        public override string GetCookieID(HttpContext context)
        {
            return context.GetToken();
        }
    }
}