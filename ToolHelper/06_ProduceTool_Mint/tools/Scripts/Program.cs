using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Mint.Common;
using Mint.Database;
using Mint.Database.APIs;
using Mint.Database.Enums;
using Mint.Substrate;

namespace Scripts
{
    class Program
    {
        static void Main(string[] args)
        {
            // IncompatibleNugets.Execute();

            XElement parent = new XElement("parent");
            XElement e1 = new XElement("data", "test43");
            e1.AddAnnotation("Hello");

            XElement e2 = new XElement("data2", "test2");

            parent.Add(e1);
            parent.Add(e2);

            foreach (var e in parent.Elements().Where(e => e.Annotations<string>().Any()))
            {
                ConsoleLog.Debug(e);
            }

        }
    }
}
