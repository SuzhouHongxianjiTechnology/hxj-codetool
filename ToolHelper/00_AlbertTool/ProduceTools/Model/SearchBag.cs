using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Albert.Model
{
    public class SearchBag
    {
        /// <summary>
        /// 包数据
        /// </summary>
        public List<SearchBagData> data { get; set; }

        /// <summary>
        /// 包总数
        /// </summary>
        public int totalHits { get; set; }
    }

    public class SearchBagData
    {
        /// <summary>
        ///作者
        /// </summary>
        public List<string> authors { get; set; }

        /// <summary>
        /// 包id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 包拥有的版本
        /// </summary>
        public List<Version> versions { get; set; }
    }

    public class Version
    {
        public string version { get; set; }
    }
}
