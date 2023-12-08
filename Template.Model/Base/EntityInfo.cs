using System;
using System.ComponentModel.DataAnnotations;

namespace Template.Model
{
    /// <summary>
    /// 业务实体数据
    /// </summary>
    public class EntityInfo : BaseInfo
    {
        /// <summary>
        /// 流水号（GUID-36位）
        /// </summary>
        [Key]
        public string id { get; set; }

        public string GetId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}