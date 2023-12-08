using Microsoft.EntityFrameworkCore;
using Template.Model;

namespace Template.Data.DB
{
    public partial class SampleContext : DbContext
    {
        public SampleContext(DbContextOptions<SampleContext> options)
           : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //设置表多主键           
            base.OnModelCreating(builder);
        }

        public DbSet<UserInfo> userInfos { get; set; }
    }
}