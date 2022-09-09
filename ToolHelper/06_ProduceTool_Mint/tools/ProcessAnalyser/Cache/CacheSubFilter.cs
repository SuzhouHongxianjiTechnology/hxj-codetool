#nullable disable

namespace ProcessAnalyser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Mint.Common.Utilities;

    public class CacheSubFilter
    {
        public string Command { get; set; }

        public string Description { get; set; }

        public string Conditions { get; set; }

        public static Dictionary<string, CacheSubFilter> LoadCacheSubFilters()
        {
            string json = FileUtils.ReadJson(Files.custom_command);
            var filterList = JsonSerializer.Deserialize<List<CacheSubFilter>>(json);
            var dict = new Dictionary<string, CacheSubFilter>();
            foreach (var filter in filterList)
            {
                dict[filter.Command] = filter;
            }
            return dict;
        }

        private static readonly Dictionary<string, string> ConditionPropertyMap = new Dictionary<string, string>
        {
            ["IsProduced"] = "IsProduced",
            ["HasBlockedSub"] = "BlockedSub",
            ["HasBlockedNonSub"] = "BlockedNonSub",
            ["HasIncompatibleAPIs"] = "IncompatibleAPIs",
            ["HasFilteredAPIs"] = "FilteredAPIs",
        };

        private static bool IsMatch(SubDllData data, string condition)
        {
            bool flag = true;
            if (condition.StartsWith('!'))
            {
                condition = condition.Substring(1);
                flag = false;
            }

            string propName = null;
            if (ConditionPropertyMap.TryGetValue(condition, out propName))
            {
                var property = data.GetProperty(propName);
                switch (propName)
                {
                    case "IsProduced":
                        return flag == (bool) property;
                    default:
                        return flag == ((List<string>) property).Any();
                }
            }
            throw new ArgumentException($"Unknown condition: {condition}");
        }

        public static bool MatchAll(SubDllData data, in List<string> RPN)
        {
            var stack = new Stack<bool>();
            foreach (string part in RPN)
            {
                switch (part)
                {
                    case "&":
                        stack.Push(stack.Pop() & stack.Pop());
                        break;
                    case "|":
                        stack.Push(stack.Pop() | stack.Pop());
                        break;
                    default:
                        stack.Push(IsMatch(data, part));
                        break;
                }
            }
            return stack.Peek();
        }

        public static List<string> ToReversePolishNotation(string conditions)
        {
            var stack = new Stack<string>();
            var list = new List<string>();

            foreach (string part in conditions.Split(' '))
            {
                switch (part)
                {
                    case "&":
                    case "|":
                        while (stack.Any() && (stack.Peek() == "&" || stack.Peek() == "|"))
                            list.Add(stack.Pop());
                        stack.Push(part);
                        break;
                    case "(":
                        stack.Push(part);
                        break;
                    case ")":
                        while (stack.Peek() != "(")
                            list.Add(stack.Pop());
                        stack.Pop();
                        break;
                    default:
                        list.Add(part);
                        break;
                }
            }

            while (stack.Any())
            {
                list.Add(stack.Pop());
            }

            return list;
        }
    }
}
