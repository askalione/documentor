using Microsoft.Extensions.Options;

namespace Documentor.Framework.Options
{
    public interface IOptionsModifier<out T> : IOptionsSnapshot<T> where T : class, new()
    {
        void Update(Action<T> applyChanges);
    }
}
