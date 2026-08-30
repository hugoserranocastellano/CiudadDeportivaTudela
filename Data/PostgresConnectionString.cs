using Npgsql;

namespace CiudadDeportivaTudela.Data;

/// <summary>
/// Supabase muestra la cadena de conexión en formato URI (postgresql://usuario:clave@host:puerto/base),
/// pero Npgsql sólo entiende el formato clave=valor. Si se pega la URI tal cual, ADO.NET revienta con
/// "Format of the initialization string does not conform to specification starting at index N".
/// </summary>
public static class PostgresConnectionString
{
    public static string Normalize(string value)
    {
        // Al copiar el valor a la consola de Render es fácil arrastrar espacios o un salto de línea.
        var raw = value.Trim();

        if (raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
            || raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return FromUri(raw);
        }

        // Validamos aquí para fallar con un mensaje que explique el formato esperado
        // en vez del "initialization string" opaco de ADO.NET.
        try
        {
            _ = new NpgsqlConnectionStringBuilder(raw);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException(
                "La cadena de conexión 'Supabase' no tiene un formato válido. Usa clave=valor "
                + "(Host=...;Port=5432;Database=postgres;Username=...;Password=...;SSL Mode=Require) "
                + "o la URI postgresql://usuario:clave@host:puerto/base. Si la contraseña contiene "
                + "';' o '=', entrecomíllala con \" o usa la URI con la clave URL-encoded.", ex);
        }

        return raw;
    }

    private static string FromUri(string raw)
    {
        var uri = new Uri(raw);
        var userInfo = uri.UserInfo.Split(':', 2);
        var database = uri.AbsolutePath.Trim('/');

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = string.IsNullOrEmpty(database) ? "postgres" : database,
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
            // Supabase exige TLS y su certificado no está en el almacén del contenedor.
            SslMode = SslMode.Require,
        };

        return builder.ConnectionString;
    }
}
