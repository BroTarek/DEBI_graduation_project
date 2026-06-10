using YouTubeClone.Domain.EnumsHelper.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeClone.Shared.Common.Params
{
    public class QueryParams : BaseQueryParams  
    {
        public SortingOptionsEnum? Sort { get; set; }
    }
}