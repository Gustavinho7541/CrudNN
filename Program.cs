using CrudNovoNN.Data;
using CrudNovoNN.Infrastructure;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
});

// CONEXÃO FORÇADA
var connStr = "Host=localhost;Port=5433;Database=CRUDNN;Username=postgres;Password=123456;Include Error Detail=true";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connStr));

// Cultura pt-BR
var supportedCultures = new[] { new CultureInfo("pt-BR") };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// CRIA O BANCO/TABELAS AUTOMATICAMENTE
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("Banco conectado com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erro ao conectar no banco:");
        Console.WriteLine(ex.Message);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();