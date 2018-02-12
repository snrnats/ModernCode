using System;
using System.Threading.Tasks;
using ModernCode.Network.Exceptions;
using ModernCode.Utils.Extensions;

namespace ModernCode.Network.Facade
{
    public class Builder<T>
    {
        private Func<Task<T>> _func;

        public Builder(Func<Task<T>> func)
        {
            _func = func;
        }

        public Builder<T> Handle<TError>(Action<ServerException, TError> handler)
        {
            return Wrap(func => CommonFacades.Handle(func, handler));
        }

        public Builder<T> Try<TException>(int attempts) where TException:Exception
        {
            return Wrap(func => CommonFacades.Retry<T, TException>(func, attempts));
        }

        public Builder<T> Wrap(Func<Func<Task<T>>, Task<T>> wrap)
        {
            _func = _func.Wrap(wrap);
            return this;
        }

        public Task<T> Do()
        {
            return _func();
        }
    }
}