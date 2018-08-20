using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Documentor.Services;
using Microsoft.AspNetCore.Mvc;

namespace Documentor.Controllers
{
    public class DumpController : BaseController
    {
        private readonly IDumper _dumper;

        public DumpController(IDumper dumper)
        {
            if (dumper == null)
                throw new ArgumentNullException(nameof(dumper));

            _dumper = dumper;
        }

        public IActionResult Index()
        {
            byte[] dump = _dumper.Export();
            return File(dump, "application/zip, application/octet-stream", $"Dump_{DateTime.Today.ToString("dd.mm.yyyy")}.zip");
        }
    }
}