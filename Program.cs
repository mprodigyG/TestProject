namespace TestProject {
    public class Program {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
             {
                 // prevent it from changing JSON object property to camel case
                 options.JsonSerializerOptions.PropertyNamingPolicy = null;
             });

            // Register the ServerSettings class and bind it to the "ServerSettings" section
            builder.Services.Configure<ServerSettings>(
                builder.Configuration.GetSection("ServerSettings"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.MapControllers();


            app.Run();
        }
    }
}