using System;
using System.Threading.Tasks;

namespace ModernCode.Network.Facade
{
    public class Builder<T>
    {
        private Func<Task<T>> _func;

        public Builder(Func<Task<T>> func)
        {
            _func = func;
        }

        public Builder<T> Wrap(Func<Func<Task>, Task<T>> wrap)
        {
            var func = _func;
            _func = () => wrap(func);
            return this;
        }
    }
}