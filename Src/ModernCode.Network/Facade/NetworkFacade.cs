using System;
using System.Threading.Tasks;

namespace ModernCode.Network.Facade
{
    public static class NetworkFacade
    {
        public static Func<Task> Facade(this HttpService httpService, Func<Task> requestFunc)
        {
            return requestFunc;
        }
    }
}