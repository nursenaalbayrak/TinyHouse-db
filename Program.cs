using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Veritabanı_proje.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabanı klasörünü oluştur
var appDataPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
if (!Directory.Exists(appDataPath))
{
    Directory.CreateDirectory(appDataPath);
}

// SQLite bağlantısını yapılandır
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Session desteği ekle
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Sadece HTTP için port ayarı
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Veritabanını oluştur ve SQL scriptini çalıştır
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Veritabanı dosyası yoksa oluştur
        var dbFile = Path.Combine(appDataPath, "TinyHouse.db");
        if (!File.Exists(dbFile))
        {
            // SQL dosyasını oku
            var sql = File.ReadAllText("Database/init.sql");
            
            // SQL komutlarını ayrı ayrı çalıştır
            foreach (var command in sql.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(command))
                {
                    try
                    {
                        context.Database.ExecuteSqlRaw(command);
                        Console.WriteLine($"SQL komutu başarıyla çalıştırıldı");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"SQL komutu hatası: {ex.Message}");
                        Console.WriteLine($"Komut: {command}");
                    }
                }
            }
            
            Console.WriteLine("Veritabanı başarıyla oluşturuldu ve başlatıldı.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Veritabanı oluşturma hatası: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
