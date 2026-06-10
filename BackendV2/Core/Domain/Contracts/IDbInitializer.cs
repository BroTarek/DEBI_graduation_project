using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Domain.Contracts.InitializerDB
{
    public interface IDbInitializer
    {
        Task DataSeedAsync();
    }
}