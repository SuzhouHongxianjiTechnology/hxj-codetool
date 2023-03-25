namespace Albert.Commons.Helpers
{
    public static class ConsoleLogHelper
    {
        public static void InLine(Object msg)
        {
            Display(msg, ConsoleColor.Gray, true);
        }

        public static void InLine(Object msg, ConsoleColor color)
        {
            Display(msg, color, true);
        }

        public static void Message(Object msg, bool inLine = false)
        {
            Display(msg, ConsoleColor.Gray, inLine);
        }

        public static void Highlight(Object msg, bool inLine = false)
        {
            Display(msg, ConsoleColor.White, inLine);
        }

        public static void Warning(Object msg, bool inLine = false)
        {
            Display(msg, ConsoleColor.Yellow, inLine);
        }

        public static void Error(Object msg, bool inLine = false)
        {
            Display(msg, ConsoleColor.Red, inLine);
        }

        public static void Success(Object msg, bool inLine = false)
        {
            Display(msg, ConsoleColor.Green, inLine);
        }

        public static void Title(Object msg, bool inLine = false)
        {
            Display(msg, ConsoleColor.Cyan, inLine);
        }

        public static void Path(Object msg, bool inLine = false)
        {
            Display(msg, ConsoleColor.DarkCyan, inLine);
        }

        public static void Ignore(Object msg, bool inLine = false)
        {
            Display(msg, ConsoleColor.DarkGray, inLine);
        }

        public static void Debug(Object msg)
        {
            Display(msg, ConsoleColor.Magenta);
        }

        public static void LogAction(object msg, Action action)
        {
            ConsoleLogHelper.Message(msg, inLine: true);
            try
            {
                action.Invoke();
                ConsoleLogHelper.Success("[DONE]");
            }
            catch (Exception e)
            {
                ConsoleLogHelper.Error("[FAILED]");
                ConsoleLogHelper.Error(e);
            }
        }

        private static void Display(Object msg, ConsoleColor color = ConsoleColor.Gray, bool inLine = false)
        {
            Console.ForegroundColor = color;
            if (inLine)
            {
                Console.Write(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
            Console.ResetColor();
        }
    }
}
