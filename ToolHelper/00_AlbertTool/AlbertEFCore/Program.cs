using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbertEFCore
{
    //Migration迁移问题：包版本要统一
    //https://stackoverflow.com/questions/62763225/i-get-an-error-when-i-add-migration-using-entity-framework-core
    internal class Program
    {
        /// <summary>
        /// <see cref="RemoveAllLine(AlbertDbContext)"/>
        /// <see cref="InitDataBase(DbContext)"/>
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>If you use migration for the first time,
        /// you must run command like:add-migration notes and update-database.
        /// </remarks>
        /// <returns></returns>
        static async Task Main(string[] args)
        {          
            using (var ctx = new AlbertDbContext())
            {
                await RemoveAllLine(ctx);
                await InitDataBase(ctx);
            }        
        }

        /// <summary>
        /// 移除配置信息数据库下的ProduceToolConfig表中的所有内容
        /// </summary>
        /// <param name="dbCtx"></param>
        /// <returns></returns>
        static async Task RemoveAllLine(AlbertDbContext dbCtx)
        {
            var produceToolEntities = dbCtx.ProduceToolEntity;
            dbCtx.RemoveRange(produceToolEntities);
            //foreach (var item in produceToolEntities)
            //{
            //    dbCtx.Remove(item);
            //}
            await dbCtx.SaveChangesAsync();
        }

        /// <summary>
        /// 根据Json回读的数据，将Json的Key和Value写入数据库ProduceToolConfig表中
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        static async Task InitDataBase(DbContext ctx)
        {
            var jsonHelper = new JsonHelper("Configs\\ProduceTool.json");
            //此处为第一种方式，也可以进行批量的操作，将produceToolEntity实例放入到list中进行批量增加
            //foreach (var item in jsonHelper.ReadJsonSub())
            //{
            //    var produceToolEntity = new ProduceToolEntity()
            //    {
            //        Name = item.Key,
            //        Value = item.Value.ToString(),
            //    };
            //    ctx.Add(produceToolEntity);
            //}
            //此处为方式二,批量增加
            var listProduceToolEntities = new List<ProduceToolEntity>();
            foreach (var item in jsonHelper.ReadJsonSub())
            {
                var produceToolEntity = new ProduceToolEntity()
                {
                    Name = item.Key,
                    Value = item.Value.ToString(),
                };
                listProduceToolEntities.Add(produceToolEntity);
            }
            await ctx.AddRangeAsync(listProduceToolEntities);
            await ctx.SaveChangesAsync();
        }
    }
}
