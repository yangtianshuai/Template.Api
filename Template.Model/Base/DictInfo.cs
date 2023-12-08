using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Model
{
    /// <summary>
    /// 字典型
    /// </summary>
    public class DictInfo: BaseInfo
    {
        /// <summary>
        /// 编码序号
        /// </summary>          
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }
    }
}