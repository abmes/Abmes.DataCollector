using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.Configuration
{
    public interface IDataCollectionNameProvider
    {
        string GetDataCollectionName();
    }
}
