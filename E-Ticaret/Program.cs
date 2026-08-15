using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using ETicaret.Data.Context;
using ETicaret.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Repository Pattern Kaydı.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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
    options.SignIn.RequireConfirmedAccount = false;
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
#region Veri Tohumlama.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // ApplicationUser sınıfını kullanma.
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Admin Rolü Yoksa Oluştur.
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Müşteri sınıfı yoksa oluştur. Bu satıra gerçekten ihtiyaç var mı emin değilim. Daha sonra döneceğim.
    if (!await roleManager.RoleExistsAsync("Customer"))
    {
        await roleManager.CreateAsync(new IdentityRole("Customer"));
    }

    // Varsayılan Admin Kullanıcısı Yoksa Oluştur.
    var adminEmail = "admin@eticaret.com";
    var adminPassword = "Password12!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Sistem",
            LastName = "Yöneticisi",
            EmailConfirmed = true // Giriş yapabilmesi için maili onaylanmış sayıyoruz.
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);

        // Kullanıcı başarıyla oluştuysa ona "Admin" rolünü ata.
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!dbContext.Categories.Any())
    {
        dbContext.Categories.AddRange(
            new Category { Name = "Elektronik" },
            new Category { Name = "Giyim & Moda" },
            new Category { Name = "Ev & Yaşam" },
            new Category { Name = "Kozmetik & Kişisel Bakım" },
            new Category { Name = "Spor & Outdoor" }
        );

        dbContext.SaveChanges();
    }
}
#endregion
app.Run();
