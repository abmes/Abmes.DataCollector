using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Common.Collecting
{
    public interface ISimpleContentProvider
    {
        Task<byte[]?> GetContentAsync(string uri, CancellationToken cancellationToken);
    }
}
