namespace MapiAnalyser.Execution
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using MapiAnalyser.Cache;
    using Mint.Common;
    using Mint.Database.APIs;
    using Mint.Database.Enums;
    using Mint.Substrate.Construction;

    public static class OneTime
    {

        public static void Run()
        {
            // AdapterHelper.ReflectionAdapter();
            AdapterHelper.RemainingAPIs();

            // AdapterHelper.FindWrongFilters();
        }
    }
}
