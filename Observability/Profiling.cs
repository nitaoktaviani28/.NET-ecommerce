namespace EcommerceApp.Observability;

public static class Profiling
{
    /// <summary>
    /// Profiling menggunakan native CLR profiler.
    /// Tidak perlu inisialisasi di kode.
    /// </summary>
    public static void InitProfiling()
    {
        // Native profiler diaktifkan melalui Dockerfile (CORECLR env).
        // Tidak perlu kode tambahan di sini.
    }
}
