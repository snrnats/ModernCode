using System;
using System.Threading.Tasks;

namespace ModernCode.Utils.Extensions
{
    public static class FuncExAsync
    {
        public static Func<Task<T>> Wrap<T>(this Func<Task<T>> func, Func<Func<Task<T>>, Task<T>> wrap)
        {
            return FuncEx.Wrap(func, wrap);
        }
    }
}