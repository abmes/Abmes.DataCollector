using Amazon.Lambda.Core;
using Abmes.DataCollector.Collector.ConsoleApp.Initialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Abmes.DataCollector.Collector.AmazonLambda;

public class Function
{
    [LambdaSerializer(typeof(global::Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
    public void FunctionHandler(CollectorParams collectorParams)
    {
        System.Console.WriteLine(collectorParams.CollectorMode);
        System.Console.WriteLine(collectorParams.ConfigSetName);
        System.Console.WriteLine(collectorParams.DataCollectionNames);
        System.Console.WriteLine(collectorParams.TimeFilter);

        Initializer
            .GetMainService()
            .MainAsync(
                CancellationToken.None,
                bootstrapper => bootstrapper.SetConfig(collectorParams.ConfigSetName, collectorParams.DataCollectionNames, collectorParams.CollectorMode, collectorParams.TimeFilter)
            )
            .Wait();
    }
}
