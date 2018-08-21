using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Models
{
    public class UsersEditCommand
    {
        public IEnumerable<string> Emails { get; set; }
    }
}
