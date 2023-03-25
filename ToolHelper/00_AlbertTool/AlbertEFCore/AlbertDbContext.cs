using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbertEFCore
{
    public class AlbertDbContext:DbContext
    {
        public DbSet<ProduceToolEntity> ProduceToolEntity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //先用直接赋值的形式来测试
            string connStr = "Server = .; Database = AlbertConfigDb; Trusted_Connection = True;MultipleActiveResultSets=true";
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
