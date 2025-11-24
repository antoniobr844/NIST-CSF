using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NistXGH.Services;

// Este arquivo deve ficar no projeto NistXGH, não no projeto de testes
public partial class Program
{
    // Método necessário para o WebApplicationFactory
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TestStartup>();
            });
}

public class TestStartup
{
    public IConfiguration Configuration { get; }

    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllersWithViews();

        // Usar InMemory para testes
        services.AddDbContext<SgsiDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase")
        );

        services.AddScoped<IFormatacaoService, FormatacaoService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "API SGSI - Test",
                    Version = "v1",
                    Description = "Documentação para testes",
                }
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API SGSI v1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
        });
    }
}
