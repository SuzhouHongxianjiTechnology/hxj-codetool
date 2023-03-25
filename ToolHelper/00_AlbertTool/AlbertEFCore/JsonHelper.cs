using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbertEFCore
{
    public class JsonHelper
    {
        private string JsonFilePath { get; set; }
        public JsonHelper(string JsonFilePath)
        {
            this.JsonFilePath = AppDomain.CurrentDomain.BaseDirectory+JsonFilePath;   
        }

        /// <summary>
        /// 返回一个JsonObject的字典对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public JObject ReadJsonSub()
        {
            try
            {
                using (StreamReader file = File.OpenText(JsonFilePath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        var jsonObject = (JObject)JToken.ReadFrom(reader);
                        file.Close();
                        return jsonObject;                               
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;                
            }
             
        }
    }
}
