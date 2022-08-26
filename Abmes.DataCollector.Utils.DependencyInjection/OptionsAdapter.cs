using Microsoft.Extensions.Options;

namespace Abmes.DataCollector.Utils.DependencyInjection;

static class OptionsAdapter
{
    public static T Adapt<T>(IOptions<T> o) where T : class, new()
    {
        return o.Value;
    }
}
