using api;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using Microsoft.Extensions.Logging;
using plugin.services;
using shared;

namespace plugin;
public class CS2Cookies : BasePlugin, IPluginConfig<CS2CookiesConfig>, ICS2Cookies
{
    public override string ModuleName => "CS2Cookies";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "Skle";
    public override string ModuleDescription => "A plugin that stores cookies for CS2 servers.";
    
    public CS2CookiesConfig? Config { get; set; }

    private ICS2CookiesShared? cookieService;

    public static PluginCapability<ICS2CookiesShared> CookieCapability { get; } = new("CS2Cookies:cookie_service");
    public override void Load(bool hotReload)
    {
        InitCookiesService();

        Capabilities.RegisterPluginCapability(CookieCapability, () => this.getCookieService()!);
    }

    private void InitCookiesService()
    {
        if (Config == null)
        {
            Logger.LogError("CS2Cookies: Config is null, cannot initialize service.");
            return;
        }
        cookieService = new CookieService(this);
    }

    public void OnConfigParsed(CS2CookiesConfig config)
    {
        Config = config;
    }

    public BasePlugin getBase()
    {
        return this;
    }

    public ICS2CookiesShared getCookieService()
    {
        return cookieService;
    }
}
