using Pyroscope;

namespace EcommerceApp.Observability
{
    public static class Profiling
    {
        public static void Init(IServiceCollection services)
        {
            // Mengonfigurasi Pyroscope untuk mengirim data profiling
            services.AddPyroscope(options =>
            {
                options.ApplicationName = "ecommerce-app";  // Ganti dengan nama aplikasi kamu
                options.ServerAddress = "http://172.193.209.242:4040";  // Ganti dengan alamat server Pyroscope di AKS
            });
        }
    }
}
