using AutoRef_API.Database;

using Microsoft.AspNetCore.Identity;

using System;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

public static class DbInitializer
{
    public static async Task SeedRolesAndAdmin(UserManager<Usuario> userManager, RoleManager<ApplicationRole> roleManager)
    {
        string adminRole = "Admin";
        string adminEmail = "adrian.estrada2001@gmail.com";
        string adminPassword = "Admin@123"; // Cambiar esto en producción.

        // 1. Crear el rol "Admin" si no existe
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = adminRole });
        }

        // 2. Crear el usuario administrador si no existe
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Usuario
            {
                UserName = adminEmail,
                Email = adminEmail,
                //NombreCompleto = "Super Administrador",
                EmailConfirmed = true, // Opcional: para evitar confirmaciones
                Nombre = "Adrián",
                PrimerApellido = "Estrada",
                SegundoApellido = "González",
                Nivel = "Nivel II + Hab. Nacional C Pista",
                ClubVinculado = "CLUB VOLEIBOL OVIEDO",
                Licencia = 16409,
                Latitud = 43.382436,
                Longitud = -5.558410,
                Ciudad = "Siero",
                Pais = "España",
                Direccion = "Reanes 65 C",
                Region = "Asturias",
                CodigoPostal = "33580",
                FechaNacimiento = new DateTime(2001, 11, 27)
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (!result.Succeeded)
            {
                Console.WriteLine("❌ Error al crear el usuario administrador:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"➡ {error.Code}: {error.Description}");
                }

                throw new Exception("Error al crear el usuario administrador.");
            }
            // 3. Asignar el rol "Admin" al usuario si aún no lo tiene
            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}
