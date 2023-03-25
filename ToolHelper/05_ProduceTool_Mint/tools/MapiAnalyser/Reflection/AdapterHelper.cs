namespace MapiAnalyser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MapiAnalyser.Cache;
    using Mint.Common;

    public static class AdapterHelper
    {
        public static void ReflectionAdapter()
        {
            Assembly assembly = typeof(System.Web.HttpContext).Assembly;

            foreach (Type t in assembly.GetExportedTypes())
            {
                bool hasAny = false;
                foreach (MemberInfo mi in t.GetMembers())
                {
                    if (mi.MemberType == MemberTypes.Method)
                    {
                        Console.Write($"{t.Namespace}.{t.Name}.{mi.Name}");
                        var parameters = ((MethodInfo)mi).GetParameters();
                        if (parameters.Any())
                        {
                            Console.Write("(");
                            Console.Write(string.Join(",", parameters.Select(p => p.ParameterType)));
                            Console.Write(")");
                        }
                        Console.WriteLine();
                        hasAny = true;
                    }

                    else if (mi.MemberType == MemberTypes.Constructor)
                    {
                        string ctor = mi.Name.Replace(".", "#");
                        Console.Write($"{t.Namespace}.{t.Name}.{ctor}");
                        var parameters = ((ConstructorInfo)mi).GetParameters();
                        if (parameters.Any())
                        {
                            Console.Write("(");
                            Console.Write(string.Join(",", parameters.Select(p => p.ParameterType)));
                            Console.Write(")");
                        }
                        Console.WriteLine();
                        hasAny = true;
                    }
                }
                if (hasAny)
                {
                    Console.WriteLine(t.FullName);
                }
            }
        }


        public static void RemainingAPIs()
        {
            var incAPIs = FileUtils.ReadLines("systemweb");
            var adaAPIs = FileUtils.ReadLines("adapter");

            var remaining = incAPIs.Except(adaAPIs);
            foreach (var api in remaining)
            {
                ConsoleLog.Warning(api);
            }
        }


        public static void FindWrongFilters()
        {
            var incompatible = FileUtils.ReadLines("systemweb");
            var implemented = FileUtils.ReadLines("adapter");

            var result = new Dictionary<string, List<string>>();

            foreach (var data in MapiCache.ReadCache())
            {
                string name = data.AssemblyName;
                var filtered = data.FilteredAPIs;
                foreach (var f in filtered.Intersect(incompatible))
                {
                    if (!implemented.Contains(f))
                    {
                        if (result.ContainsKey(name))
                        {
                            result[name].Add(f);
                        }
                        else
                        {
                            result.Add(name, new List<string> { f });
                        }
                    }
                }
            }

            foreach (var a in result)
            {
                ConsoleLog.Title(a.Key);
                foreach (var v in a.Value)
                {
                    ConsoleLog.Error($"    {v}");
                }
            }
        }
    }
}
