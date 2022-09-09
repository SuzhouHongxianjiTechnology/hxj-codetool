namespace Mint.Common.Utilities
{
    public static class UserInputParser
    {
        public static void Parse(string[] args, out string command, out string argument1, out string argument2)
        {
            command = argument1 = argument2 = string.Empty;
            if (args == null) return;
            if (args.Length > 0) command = args[0];
            if (args.Length > 1) argument1 = args[1];
            if (args.Length > 2) argument2 = args[2];
        }
    }
}
