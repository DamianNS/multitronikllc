
namespace multitronikllcAPIMock
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<Services.UsuariosService>();
            builder.Services.AddControllers();            
            builder.Services.AddOpenApi();
            var app = builder.Build();
            app.MapOpenApi();
            app.MapControllers();
            app.Run();
        }
    }
}
