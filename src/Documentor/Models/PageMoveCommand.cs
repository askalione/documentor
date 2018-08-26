using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Models
{
    public class PageMoveCommand
    {
        public string VirtualPath { get; set; }
        public string NewParentVirtualPath { get; set; }
        public int NewSequenceNumber { get; set; }
    }
}
