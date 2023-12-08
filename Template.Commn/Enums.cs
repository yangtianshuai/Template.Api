using System.ComponentModel;

namespace Template.Commn
{
    public static class Enums
    {
        /// <summary>
        /// 性别
        /// </summary>
        public enum Gender
        {
            /// <summary>
            /// 未知
            /// </summary>
            [Description("未知")]
            None = 0,
            /// <summary>
            /// 男性
            /// </summary>
            [Description("男")]
            Male = 1,
            /// <summary>
            /// 女性
            /// </summary>
            [Description("女")]
            Female = 2
        }

        /// <summary>
        /// 用户状态
        /// </summary>
        public enum UserStatus
        {
            /// <summary>
            /// 正常
            /// </summary>
            [Description("正常")]
            Normal = 1,
            /// <summary>
            /// 禁用
            /// </summary>
            [Description("禁用")]
            Forbidden = 2,
            /// <summary>
            /// 注销
            /// </summary>
            [Description("注销")]
            Cancel = 3
        }
    }
}