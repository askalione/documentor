using System.Reflection;

namespace Documentor.Framework.Versioning
{
    public class AppVersionProvider : IAppVersionProvider
    {
        private string? _version = null;

        public string GetVersion()
        {
            if (_version == null)
            {
                _version = Assembly.GetEntryAssembly()!
                   .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                   .InformationalVersion;
            }

            return _version;
        }
    }
}
