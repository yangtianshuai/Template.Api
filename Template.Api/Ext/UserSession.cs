using Api.Config;

namespace Template.Api
{
    public class UserSession: Session
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int user_id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string user_name { get; set; }
    }
}