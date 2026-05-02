using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Contracts.InitializerDB
{
    public interface IDbInitializer
    {
        Task DataSeedAsync();
    }
}