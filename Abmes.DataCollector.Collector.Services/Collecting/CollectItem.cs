using Abmes.DataCollector.Common;

namespace Abmes.DataCollector.Collector.Services.Collecting;

public record CollectItem(FileInfoData? CollectFileInfo, string CollectUrl);
