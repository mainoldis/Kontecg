using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Kontecg.Updates
{
    public interface IUpdateChecker
    {
        Task<JObject> CheckAsync(IEnumerable<IUpdateSource> sources, Action<int> progress = null);
    }
}
