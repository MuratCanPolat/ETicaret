using ETicaret.Core.Entities;
using ETicaret.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabanı ve DbContext Ayarları.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // appsettings.json'dan gelen bağlantı adresi.
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // OpenIddict.
    options.UseOpenIddict();
});

// ASP.NET Core Identity Ayarları.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Şifre kuralları
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// OpenIddict Ayarları.
builder.Services.AddOpenIddict()
    // Veritabanı olarak EF Core ve bu DbContext'i kullan.
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AppDbContext>();
    })
// Token üretecek merkez.
    .AddServer(options =>
    {
        // Yetkilendirme ve Token uç noktalarını belirleme.
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetTokenEndpointUris("connect/token")
               .SetEndSessionEndpointUris("connect/logout");

        // İzin verilen OAuth akışları.
        options.AllowAuthorizationCodeFlow()
               .AllowClientCredentialsFlow();

        // Geliştirme ortamı için geçici şifreleme anahtarları.
        options.AddEphemeralEncryptionKey()
               .AddEphemeralSigningKey();

        // ASP.NET Core altyapısını kullan ve HTTP isteklerini OpenIddict'e yönlendir emri.
        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough()
               .EnableEndSessionEndpointPassthrough();
    })
    // Gelen token'ları denetleyecek kısım.
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
