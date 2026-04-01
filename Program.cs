using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Змінено на UseNpgsql та вказано правильну назву змінної для PostgreSQL
builder.Services.AddDbContext<MyDatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AZURE_POSTGRESQL_CONNECTIONSTRING")));

// 2. Виправлено отримання рядка підключення для Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("AZURE_REDIS_CONNECTIONSTRING");
    options.InstanceName = "SampleInstance";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add App Service logging
builder.Logging.AddAzureWebAppDiagnostics();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MyDatabaseContext>();
    context.Database.Migrate(); // Це застосує міграції до PostgreSQL
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();