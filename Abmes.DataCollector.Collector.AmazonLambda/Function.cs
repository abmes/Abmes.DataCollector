using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Abmes.DataCollector.Collector.ConsoleApp.Initialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Abmes.DataCollector.Collector.AmazonLambda
{
    public class Function
    {
        [LambdaSerializer(typeof(global::Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public void FunctionHandler(CollectorParams collectorParams)
        {
            System.Console.WriteLine(collectorParams.ConfigSetName);
            System.Console.WriteLine(collectorParams.DataCollectionNames);

            //Initializer.GetMainService().MainAsync(CancellationToken.None).Wait();

            System.Console.WriteLine("Exitting after 5 seconds ...");
            Task.Delay(5000);
        }
    }
}
