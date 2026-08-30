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
        // Al copiar el valor a la consola de Render es fácil arrastrar espacios, un salto de
        // línea o las comillas con las que venía envuelto en un fichero de ejemplo.
        var raw = value.Trim().Trim('"', '\'').Trim();

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
            // Incluimos longitud y las claves que sí se reconocieron: sitúan el fallo sin
            // volcar la contraseña en los logs de Render.
            throw new InvalidOperationException(
                "La cadena de conexión 'Supabase' no tiene un formato válido. Usa clave=valor "
                + "(Host=...;Port=5432;Database=postgres;Username=...;Password=...;SSL Mode=Require) "
                + "o la URI postgresql://usuario:clave@host:puerto/base. Si la contraseña contiene "
                + "';' o '=', entrecomíllala con \" o usa la URI con la clave URL-encoded. "
                + $"Detalle: {ex.Message} (longitud del valor: {raw.Length}; "
                + $"claves antes del fallo: {DescribeKeys(raw)}).", ex);
        }

        return raw;
    }

    /// <summary>
    /// Lista los nombres de clave (nunca los valores) de los segmentos separados por ';'.
    /// </summary>
    private static string DescribeKeys(string raw)
    {
        var keys = raw.Split(';')
            .Where(segment => segment.Trim().Length > 0)
            .Select(segment =>
            {
                var parts = segment.Split('=', 2);
                var key = parts[0].Trim();

                // Un segmento sin '=' (o con un nombre que no parece una clave) suele ser un
                // trozo de la contraseña partido por un ';'. No lo reproducimos.
                return parts.Length == 2 && key.Length <= 30 && key.All(c => char.IsLetter(c) || c is ' ' or '_')
                    ? key
                    : "<segmento inválido>";
            });

        return string.Join(", ", keys);
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
