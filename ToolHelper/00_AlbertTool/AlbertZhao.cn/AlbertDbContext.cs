using AlbertZhao.cn.Models;
using Microsoft.EntityFrameworkCore;

namespace AlbertZhao.cn.DbContextExtension
{
    public class AlbertDbContext: DbContext
    {
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //先用直接赋值的形式来测试
            string connStr = "Server = .; Database = AlbertWebDb; Trusted_Connection = True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            //引用Zack.EFCore.Batch.MSSQL Package.支持批量操作SQL Server数据库
            optionsBuilder.UseBatchEF_MSSQL();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //从当前程序集加载所有的IEntityTypeConfiguration<T>
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
