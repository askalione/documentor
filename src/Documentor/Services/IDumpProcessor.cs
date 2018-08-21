using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Services
{
    public interface IDumpProcessor
    {
        byte[] ExportDump();
    }
}
