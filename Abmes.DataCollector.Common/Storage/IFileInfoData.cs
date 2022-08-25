﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Abmes.DataCollector.Common.Storage
{
    public interface IFileInfoData
    {
        string Name { get; }

        long? Size { get; }

        string MD5 { get; }

        string GroupId { get; }

        string StorageType { get; }
    }
}