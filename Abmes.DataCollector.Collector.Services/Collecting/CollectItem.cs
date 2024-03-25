using Abmes.DataCollector.Common.Data.Storage;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public record CollectItem(FileInfoData? CollectFileInfo, string CollectUrl);
