using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Model
{
    /// <summary>
    /// 用户表
    /// </summary>
    [Serializable]
    [Table("t_user")]
    public class UserInfo : DictInfo
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string cellpone { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime register_time { get; set; }
        /// <summary>
        /// 状态：1-正常；2-禁用；3-注销
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 是否认证，1表示已认证
        /// </summary>
        public int auth_flag { get; set; }
    }
}