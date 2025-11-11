using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NistXGH.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SgsiDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("SgsiDbContext"))
);

builder.Services.AddScoped<IFormatacaoService, FormatacaoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API SGSI",
        Version = "v1",
        Description = "Documentação gerada com Swagger"
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API SGSI v1");
        c.RoutePrefix = "swagger" ; // Swagger abre na raiz: http://localhost:5263
    });
}





app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
