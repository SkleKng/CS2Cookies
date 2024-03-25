using models;
using MySqlConnector;
using shared;
using api;

namespace plugin.services;

public class CookieService : ICS2CookiesShared
{
    private readonly ICS2Cookies CS2Cookies;
    private readonly MySqlConnection conn;

    public CookieService(ICS2Cookies CS2Cookies)
    {
        this.CS2Cookies = CS2Cookies;

        this.conn = new MySqlConnection(CS2Cookies.Config?.DBConnectionString);

        createTables();
    }

    private async void createTables() {
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS cs2_cookies (
                id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(255)
            )
            """;
        await cmd.ExecuteNonQueryAsync();

        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS cs2_cookies_cache (
                steamid BIGINT NOT NULL,
                cookieid INT NOT NULL,
                value VARCHAR(255),
                PRIMARY KEY (steamid, cookieid)
            )
            """;

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<ICookie?> FindClientCookie(string key)
    {
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT EXISTS(SELECT 1 FROM cs2_cookies WHERE name = @key)";
        cmd.Parameters.AddWithValue("@key", key);
        var reader = await cmd.ExecuteReaderAsync();

        if (reader.Read())
        {
            if (reader.GetBoolean(0))
            {
                return new Cookie(key, CS2Cookies);
            }
        }

        return null;
    }

    public async Task<ICookie> RegClientCookie(string key)
    {
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO cs2_cookies (name) VALUES (@key)";
        cmd.Parameters.AddWithValue("@key", key);
        await cmd.ExecuteNonQueryAsync();

        return new Cookie(key, CS2Cookies);
    }

    public MySqlConnection GetConnection()
    {
        return conn;
    }
}