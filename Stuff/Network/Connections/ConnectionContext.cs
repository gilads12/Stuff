using Microsoft.AspNetCore.Http.Features;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stuff.Network.Connections
{
    public class ConnectionContext
    {
        public IFeatureCollection Features { get; } = new FeatureCollection();
        public IDictionary<object, object> Items { get; } = new ConcurrentDictionary<object, object>()
        {
            ["Buffer"] = new List<byte>()
        };
    }
}
