using System;
using System.Collections.Generic;
using System.Text;

namespace ModernCode.Utils.Extensions
{
    public static class FuncEx
    {
        public static Func<T> Wrap<T>(this Func<T> func, Func<Func<T>, T> wrap)
        {
            return () => wrap(func);
        }
    }
}
