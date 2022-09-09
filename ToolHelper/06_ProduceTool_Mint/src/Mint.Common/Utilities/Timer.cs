namespace Mint.Common.Utilities
{
    using System;
    using System.Diagnostics;

    public class Timer : IDisposable
    {
        private static Stopwatch? Watch;
        private Stopwatch watch;
        public static Timer TimeThis => new Timer();

        private Timer()
        {
            this.watch = Stopwatch.StartNew();
        }

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
                ConsoleLog.Debug($"\n------------------" +
                                 $"\nElapsed: {time:F2} sec");
                Timer.Watch = null;
            }
        }

        public void Dispose()
        {
            this.watch.Stop();
            double time = this.watch.ElapsedMilliseconds / 1000.0;
            ConsoleLog.Debug($"\n------------------" +
                             $"\nElapsed: {time:F2} sec");
        }
    }
}
