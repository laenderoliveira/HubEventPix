using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerPix
{
    public static class Extensions
    {
        public static T BindTo<T>(this IConfiguration configuration, string key, T target)
        {
            configuration.GetSection(key).Bind(target);
            return target;
        }
    }
}
