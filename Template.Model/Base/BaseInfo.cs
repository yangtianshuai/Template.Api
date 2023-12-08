using System;

namespace Template.Model
{
    public class BaseInfo: ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone(); 
        }

        public BaseInfo()
        {
            create_time = DateTime.Now;
        }       

        /// <summary>
        /// 新增人员
        /// </summary>     
        public string create_by { get; set; }
        /// <summary>
        /// 新增时间
        /// </summary>      
        public DateTime create_time { get; set; }
        /// <summary>
        /// 修改人员
        /// </summary>     
        public string update_by { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>      
        public DateTime? update_time { get; set; }
        /// <summary>
        /// 删除标志 *表示删除
        /// </summary>       
        public int delete_flag { get; set; }

        public void Add(string userId)
        {
            create_by = userId;
            create_time = DateTime.Now;
        }
        public void Modify(string userId)
        {
            update_by = userId;
            update_time = DateTime.Now;
        }

        public void Delete(string userId)
        {
            Modify(userId);
            delete_flag = 0;
        }
    }
}