﻿using Abmes.DataCollector.Vault.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Vault.FileSystem.Storage
{
    public delegate IFileSystemStorage IFileSystemStorageFactory(StorageConfig storageConfig);
}
