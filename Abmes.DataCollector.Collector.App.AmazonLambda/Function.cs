using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Abmes.DataCollector.Collector.App.AmazonLambda;

public class Function
{
    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
    public async Task FunctionHandlerAsync(CollectorParams collectorParams)
    {
        System.Console.WriteLine(collectorParams.CollectorMode);
        System.Console.WriteLine(collectorParams.ConfigSetName);
        System.Console.WriteLine(collectorParams.DataCollectionNames);
        System.Console.WriteLine(collectorParams.TimeFilter);

        await Abmes.DataCollector.Collector.App.Library.Initialization.Initializer
            .GetMainService()
            .MainAsync(
                bootstrapper => bootstrapper.SetConfig(
                    collectorParams.ConfigSetName,
                    collectorParams.DataCollectionNames,
                    collectorParams.CollectorMode,
                    collectorParams.TimeFilter),
                0,
                default
            );
    }
}
