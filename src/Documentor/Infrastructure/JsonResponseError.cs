using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Infrastructure
{
    public class JsonResponseError
    {
        public string Key { get; }
        public string Message { get; }

        public JsonResponseError(string message)
        {
            Message = message;
        }

        public JsonResponseError(string key, string message)
        {
            Key = key;
            Message = message;
        }
    }
}
