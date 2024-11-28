using Abmes.DataCollector.Common;

namespace Abmes.DataCollector.Collector.Services.Ports.Collecting;

public record CollectItem(FileInfoData? CollectFileInfo, string CollectUrl);
