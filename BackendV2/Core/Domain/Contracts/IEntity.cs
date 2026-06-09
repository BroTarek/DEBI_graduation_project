using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Domain.Contracts
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}