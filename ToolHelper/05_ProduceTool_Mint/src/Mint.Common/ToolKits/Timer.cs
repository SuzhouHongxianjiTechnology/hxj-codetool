namespace Mint.Common
{
    using System;
    using System.Diagnostics;

    public static class Timer
    {
        private static Stopwatch Watch;

        public static void Start()
        {
            if (Timer.Watch == null)
            {
                Timer.Watch = Stopwatch.StartNew();
            }
        }

        public static void Stop()
        {
            if (Timer.Watch != null)
            {
                Timer.Watch.Stop();
                double time = Timer.Watch.ElapsedMilliseconds / 1000.0;
                ConsoleLog.Debug(Environment.NewLine + $"------------------" +
                                 Environment.NewLine + $"Elapsed: {time:F2} sec");
                Timer.Watch = null;
            }
        }
    }
}
