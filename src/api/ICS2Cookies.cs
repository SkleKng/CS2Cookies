using CounterStrikeSharp.API.Core;
using plugin;
using shared;

namespace api
{
    public interface ICS2Cookies : IPluginConfig<CS2CookiesConfig>
    {
        public ICS2CookiesShared getCookieService();
        public BasePlugin getBase();
    }
}