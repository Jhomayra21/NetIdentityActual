using Microsoft.AspNetCore.Identity;
using NetIdentity.Models;

namespace NetIdentity.Data
{
    public static class SeedData
    {
        private const string CLAIM_GENERO = "Genero";
        private const string CLAIM_FECHA_NACIMIENTO = "FechaNacimiento";
        private const string GENERO_FEMENINO = "Femenino";
        private const string GENERO_MASCULINO = "Masculino";

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureDeleted();  
            context.Database.EnsureCreated();  

            string[] roleNames = { "Admin", "Usuario" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            if (await userManager.FindByEmailAsync("admin@test.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    FechaNacimiento = DateTime.Now.AddYears(-30),
                    NombreCompleto = "Administrador Sistema",
                    EmailConfirmed = true,
                    genero = GENERO_MASCULINO
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddClaimAsync(adminUser,
                        new System.Security.Claims.Claim(CLAIM_FECHA_NACIMIENTO, adminUser.FechaNacimiento.ToString("yyyy-MM-dd")));
                    await userManager.AddClaimAsync(adminUser,
                        new System.Security.Claims.Claim(CLAIM_GENERO, adminUser.genero));
                }
            }
            else
            {
                var adminUser = await userManager.FindByEmailAsync("admin@test.com");
                if (string.IsNullOrEmpty(adminUser.genero))
                {
                    adminUser.genero = GENERO_MASCULINO;
                    await userManager.UpdateAsync(adminUser);
                    
                    var claims = await userManager.GetClaimsAsync(adminUser);
                    if (!claims.Any(c => c.Type == CLAIM_GENERO))
                    {
                        await userManager.AddClaimAsync(adminUser,
                            new System.Security.Claims.Claim(CLAIM_GENERO, adminUser.genero));
                    }
                }
            }

            if (await userManager.FindByEmailAsync("menor@test.com") == null)
            {
                var userMenor = new ApplicationUser
                {
                    UserName = "menor@test.com",
                    Email = "menor@test.com",
                    FechaNacimiento = DateTime.Now.AddYears(-15),
                    NombreCompleto = "Juan Menor",
                    EmailConfirmed = true,
                    genero = GENERO_MASCULINO
                };

                var result = await userManager.CreateAsync(userMenor, "Menor123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(userMenor, "Usuario");
                    await userManager.AddClaimAsync(userMenor,
                        new System.Security.Claims.Claim(CLAIM_FECHA_NACIMIENTO, userMenor.FechaNacimiento.ToString("yyyy-MM-dd")));
                    await userManager.AddClaimAsync(userMenor,
                        new System.Security.Claims.Claim(CLAIM_GENERO, userMenor.genero));
                }
            }
            else
            {
                var userMenor = await userManager.FindByEmailAsync("menor@test.com");
                if (string.IsNullOrEmpty(userMenor.genero))
                {
                    userMenor.genero = GENERO_MASCULINO;
                    await userManager.UpdateAsync(userMenor);
                    
                    var claims = await userManager.GetClaimsAsync(userMenor);
                    if (!claims.Any(c => c.Type == CLAIM_GENERO))
                    {
                        await userManager.AddClaimAsync(userMenor,
                            new System.Security.Claims.Claim(CLAIM_GENERO, userMenor.genero));
                    }
                }
            }

            if (await userManager.FindByEmailAsync("mayor@test.com") == null)
            {
                var userMayor = new ApplicationUser
                {
                    UserName = "mayor@test.com",
                    Email = "mayor@test.com",
                    FechaNacimiento = DateTime.Now.AddYears(-25),
                    NombreCompleto = "María Mayor",
                    EmailConfirmed = true,
                    genero = GENERO_FEMENINO
                };

                var result = await userManager.CreateAsync(userMayor, "Mayor123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(userMayor, "Usuario");
                    await userManager.AddClaimAsync(userMayor,
                        new System.Security.Claims.Claim(CLAIM_FECHA_NACIMIENTO, userMayor.FechaNacimiento.ToString("yyyy-MM-dd")));
                    await userManager.AddClaimAsync(userMayor,
                        new System.Security.Claims.Claim(CLAIM_GENERO, userMayor.genero));
                }
            }
            else
            {
                var userMayor = await userManager.FindByEmailAsync("mayor@test.com");
                if (string.IsNullOrEmpty(userMayor.genero))
                {
                    userMayor.genero = GENERO_FEMENINO;
                    await userManager.UpdateAsync(userMayor);
                    
                    var claims = await userManager.GetClaimsAsync(userMayor);
                    if (!claims.Any(c => c.Type == CLAIM_GENERO))
                    {
                        await userManager.AddClaimAsync(userMayor,
                            new System.Security.Claims.Claim(CLAIM_GENERO, userMayor.genero));
                    }
                }
            }
        }
    }


}
