namespace EcommerceApp.Observability
{
    public static class Profiling
    {
        /// <summary>
        /// Profiling menggunakan native CLR profiler.
        /// Tidak perlu inisialisasi di kode.
        /// </summary>
        public static void InitProfiling()
        {
            // Cek environment variable untuk Pyroscope
            string pyroscopeUrl = Env.GetEnv("PYROSCOPE_SERVER_ADDRESS", "http://172.193.209.242:4040");

            // Konfigurasi Pyroscope dengan endpoint langsung dari kode
            var pyroscopeConfig = new PyroscopeConfig
            {
                AppName = Env.ServiceName,
                ServerUrl = pyroscopeUrl // Endpoint Pyroscope diambil langsung dari konfigurasi
            };

            // Inisialisasi Pyroscope
            Pyroscope.Start(pyroscopeConfig);

            // Tambahkan log untuk verifikasi bahwa profiling sudah diaktifkan
            Console.WriteLine("🔍 Pyroscope profiler initialized with endpoint: " + pyroscopeUrl);
        }
    }
}
