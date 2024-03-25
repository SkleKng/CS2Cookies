
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using api;
using shared;

namespace models;

public class Cookie : ICookie
{
    private readonly string Key;
    private readonly ICS2Cookies CS2Cookies;

    public Cookie(string key, ICS2Cookies CS2Cookies)
    {
        Key = key;
        this.CS2Cookies = CS2Cookies;
    }

    public async Task<string?> Get(CCSPlayerController player) {
        if(player == null) throw new ArgumentException("Attempted to read cookies on console.");
        if(player.AuthorizedSteamID == null) throw new ArgumentException("Attempted to get cookie for an unauthorized player.");

        string steamid = player.AuthorizedSteamID.SteamId64.ToString();

        var cmd = CS2Cookies.getCookieService().GetConnection().CreateCommand();
        cmd.CommandText = "SELECT value FROM cs2_cookies_cache WHERE steamid = @steamid AND cookieid = (SELECT id FROM cs2_cookies WHERE name = @key)";
        cmd.Parameters.AddWithValue("@steamid", steamid);
        cmd.Parameters.AddWithValue("@key", Key);

        var reader = await cmd.ExecuteReaderAsync();

        if(reader.Read()) {
            CS2Cookies.getBase().Logger.LogInformation($"Got cookie {Key} for {steamid} as {reader.GetString(0)}");
            return reader.GetString(0);
        }

        CS2Cookies.getBase().Logger.LogInformation($"No cookie {Key} for {steamid}");
        return null;

    }

    public async void Set(CCSPlayerController player, string value) {
        if(player == null) throw new ArgumentException("Attempted to set cookies on console.");
        if(player.AuthorizedSteamID == null) throw new ArgumentException("Attempted to set cookie for an unauthorized player.");

        string steamid = player.AuthorizedSteamID.SteamId64.ToString();

        var cmd = CS2Cookies.getCookieService().GetConnection().CreateCommand();
        cmd.CommandText = "INSERT INTO cs2_cookies_cache (steamid, cookieid, value) VALUES (@steamid, (SELECT id FROM cs2_cookies WHERE name = @key), @value)";
        cmd.Parameters.AddWithValue("@steamid", steamid);
        cmd.Parameters.AddWithValue("@key", Key);
        cmd.Parameters.AddWithValue("@value", value);

        await cmd.ExecuteNonQueryAsync();

        CS2Cookies.getBase().Logger.LogInformation($"Set cookie {Key} for {steamid} to {value}");
    }
}