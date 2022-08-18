/*
 源码己托管:https://gitee.com/kuiyu/dotnetcodes
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNet.Utilities
{
    /// <summary>
    /// 经典的红包分配算法
    /// </summary>
    public class RedPacket

    { 

        /// <summary>
        /// 红包算法
        /// </summary>
        /// <param name="remainMoney">要分配的总额度</param>
        /// <param name="remainCount">要分配的份数</param>
        /// <returns></returns>
        public static List<double> GetMoneys(double remainMoney,int remainCount)
        {
            //人均最小金额  
            double min = 0.1;
            if (remainMoney < remainCount * min)
                return null;

            int num = remainCount;
            List<double> array = new List<double>();
            Random r = new Random();
            for (int i = 0; i < num; i++)
            {
                if (remainCount == 1)
                {
                    remainCount--;
                    array.Add(Convert.ToDouble(remainMoney.ToString("0.00")));
                   // Console.WriteLine(string.Format("第{0}个红包：{1}元", i + 1, Convert.ToDouble(remainMoney.ToString("0.00"))));
                }
                else
                {
                    //(remainMoney - (remainCount - 1) * min)：保存剩余金额可以足够的去分配剩余的红包数量  
                    double max = (remainMoney - (remainCount - 1) * min) / remainCount * 2;
                    double money = r.NextDouble() * max;
                    money = Convert.ToDouble((money <= min ? min : money).ToString("0.00"));
                    remainCount--;
                    remainMoney -= money;
                    array.Add(money);
                   // Console.WriteLine(string.Format("第{0}个红包：{1}元", i + 1, money));
                }
            }
            //再次随机  
            return array.OrderBy(o => Guid.NewGuid()).ToList();
        }

    }

  
}
