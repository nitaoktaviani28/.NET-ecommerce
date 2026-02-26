/**
 * Observability/Env.cs
 * 
 * Equivalent to: observability/env.go
 * 
 * Environment variable helper.
 */

namespace EcommerceApp.Observability;

public static class Env
{
    /// <summary>
    /// Get environment variable or return default value.
    /// Equivalent to getEnv() in Go.
    /// </summary>
    public static string GetEnv(string key, string defaultValue = "")
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }
}
