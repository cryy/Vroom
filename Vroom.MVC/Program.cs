using Microsoft.EntityFrameworkCore;
using Vroom.Mappers;
using Vroom.Service.Database;
using Vroom.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<VroomContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddAutoMapper(typeof(VehicleMappingProfile));
builder.Services.AddSingleton<IVehicleService, VehicleService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VroomContext>();
    await db.Database.EnsureCreatedAsync();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
