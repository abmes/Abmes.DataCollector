using Abmes.DataCollector.Common.Storage;

namespace Abmes.DataCollector.Collector.Common.Collecting;

public record CollectItem(FileInfoData? CollectFileInfo, string CollectUrl);
