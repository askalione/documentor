using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Models
{
    public class Notification
    {
        public NotificationType Type { get; set; }
        public string Message { get; set; }
    }
}
