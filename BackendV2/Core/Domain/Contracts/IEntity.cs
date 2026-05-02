using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Contracts
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}