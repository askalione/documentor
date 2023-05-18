using Documentor.Services.Dumps;

namespace Documentor.Controllers
{
    public class DumpController : AppController
    {
        private readonly IDumpProcessor _dumper;

        public DumpController(IDumpProcessor dumper)
        {
            if (dumper == null)
                throw new ArgumentNullException(nameof(dumper));

            _dumper = dumper;
        }

        public IActionResult Export()
        {
            byte[] dump = _dumper.ExportDump();
            return File(dump, "application/zip", $"Dump_{DateTime.Today.ToString("dd.MM.yyyy")}.zip");
        }
    }
}
