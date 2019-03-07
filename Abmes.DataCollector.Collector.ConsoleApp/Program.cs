﻿using Abmes.DataCollector.Collector.ConsoleApp.Initialization;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace Abmes.DataCollector.Collector.ConsoleApp
{
    class Program
    {
        static int DelayedExitCode(int exitCode, int delaySeconds = 0)
        {
            if (delaySeconds > 0)
            {
                WriteLine($"Exitting after {delaySeconds} seconds ...");
                Task.Delay(TimeSpan.FromSeconds(delaySeconds)).Wait();
            }

            return exitCode;
        }

        static async Task<int> Main(string[] args)
        {
            int exitCode;

            try
            {
                await 
                    Initializer
                    .GetMainService()
                    .MainAsync(CancellationToken.None);

                exitCode = DelayedExitCode(0, 5);
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
                exitCode = DelayedExitCode(1, 5);
            }

            #if DEBUG
            Task.Delay(500).Wait();
            WriteLine("Press any key to quit...");
            ReadKey();
            #endif

            return exitCode;
        }
    }
}
