using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetIdentity.Data;
using NetIdentity.Models;

var builder = WebApplication.CreateBuilder(args);

// Constantes para políticas y claims
const string CLAIM_GENERO = "Genero";
const string CLAIM_FECHA_NACIMIENTO = "FechaNacimiento";
const string GENERO_FEMENINO = "Femenino";
const string GENERO_MASCULINO = "Masculino";

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("menoresEdad", policy =>
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var birthDateClaim = user.FindFirst(CLAIM_FECHA_NACIMIENTO);
                if (birthDateClaim != null && DateTime.TryParse(birthDateClaim.Value, out DateTime birthDate))
                {
                    var edad = DateTime.Today.Year - birthDate.Year;
                    if (birthDate.Date > DateTime.Today.AddYears(-edad)) edad--;
                    return edad < 18;
                }
            }
            return false;
        }));

    options.AddPolicy("soloFemenino", policy =>
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var generoClaim = user.FindFirst(CLAIM_GENERO);
                return generoClaim != null && generoClaim.Value == GENERO_FEMENINO;
            }
            return false;
        }));

    options.AddPolicy("soloMasculino", policy =>
        policy.RequireAssertion(context =>
        {
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var generoClaim = user.FindFirst(CLAIM_GENERO);
                return generoClaim != null && generoClaim.Value == GENERO_MASCULINO;
            }
            return false;
        }));

    options.AddPolicy("SoloAdmin", policy => policy.RequireRole("Admin"));

    options.AddPolicy("AdminOUsuario", policy =>
        policy.RequireRole("Admin", "Usuario"));

});


builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

app.Run();
