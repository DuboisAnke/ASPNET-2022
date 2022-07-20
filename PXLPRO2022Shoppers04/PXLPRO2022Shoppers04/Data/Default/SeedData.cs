using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PXLPRO2022Shoppers04.Helpers;
using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.Models.Categories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Data.Default
{
    public class SeedData
    {
        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            AppDbContext context = app.ApplicationServices.CreateScope()
                .ServiceProvider
                .GetRequiredService<AppDbContext>();

            RoleManager<IdentityRole> roleManager =
                app.ApplicationServices.CreateScope()
                    .ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();

            UserManager<IdentityUser> userManager =
                app.ApplicationServices.CreateScope()
                    .ServiceProvider
                    .GetRequiredService<UserManager<IdentityUser>>();

            if (!context.Roles.Any())
            {
                foreach (var roleString in RoleHelper.Roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleString));
                }

                var userAdmin = new IdentityUser();
                userAdmin.Email = "admin@pxl.be";
                userAdmin.UserName = userAdmin.Email;
                var resultAdmin = await userManager.CreateAsync(userAdmin, "Adm!n007");
                if (resultAdmin.Succeeded)
                {
                    var roleAdmin = await roleManager.FindByNameAsync(RoleHelper.AdminRole);
                    if (roleAdmin != null)
                    {
                        await userManager.AddToRoleAsync(userAdmin, roleAdmin.Name);
                        var client = new Client
                        {
                            FirstName = "Kristof",
                            Name = "Palmaers",
                            IdentityUserId = userAdmin.Id,
                        };
                        context.Clients.Add(client);
                        await context.SaveChangesAsync();
                    }

                    var userClient = new IdentityUser();
                    userClient.Email = "client@pxl.be";
                    userClient.UserName = userClient.Email;
                    var resultClient = await userManager.CreateAsync(userClient, "Cli3nt001!");
                    if (resultClient.Succeeded)
                    {
                        var roleClient = await roleManager.FindByNameAsync(RoleHelper.ClientRole);
                        if (roleClient != null)
                        {
                            await userManager.AddToRoleAsync(userClient, roleClient.Name);
                            var client = new Client
                            {
                                FirstName = "Kristof",
                                Name = "Palmaers",
                                IdentityUserId = userClient.Id,
                            };
                            context.Clients.Add(client);
                            await context.SaveChangesAsync();

                            if (!context.Addresses.Any())
                            {
                                context.Addresses.AddRange(
                                    new Address
                                    {
                                        ClientId = client.ClientId,
                                        StreetName = "Pxl-laan",
                                        HouseNumber = "5",
                                        City = "Hasselt",
                                        ZIP = "3500",
                                        Country = "Belgium"
                                    });
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }

                if (!context.ProductCategories.Any())
                {
                    context.ProductCategories.AddRange(
                        new ProductCategory
                        {
                            Name = "Keyboard"
                        },
                        new ProductCategory
                        {
                            Name = "Mouse"
                        },
                        new ProductCategory
                        {
                            Name = "MousePad"
                        }
                    );
                    await context.SaveChangesAsync();
                    if (!context.Products.Any())
                    {
                        string assetsPath = @"\assets\";
                        context.Products.AddRange(

                            new Keyboard
                            {
                                Name = "Logitech G810",
                                Price = 149.99f,
                                Brand = "Logitech",
                                Color = "Black",
                                Description = "A Gaming keyboard for pros",
                                ImageLink = @$"{assetsPath}logitech810.png",
                                Layout = "QWERTY",
                                Switch = "Romer-G",
                                ProductCategory = context.ProductCategories.FirstOrDefault(x => x.Name == "Keyboard"),
                                SSN = await APIHelper.CreateSSN(Guid.NewGuid(), 5),
                                Size = "Medium",
                                HasRGB = true
                            },
                            new Keyboard
                            {
                                Name = "Logitech G213",
                                Price = 99.99f,
                                Brand = "Logitech",
                                Color = "Black",
                                Description = "A Gaming keyboard for pros",
                                ImageLink = @$"{assetsPath}logitechg213.png",
                                Layout = "QWERTY",
                                Switch = "Yellow",
                                ProductCategory = context.ProductCategories.FirstOrDefault(x => x.Name == "Keyboard"),
                                SSN = await APIHelper.CreateSSN(Guid.NewGuid(), 4),
                                Size = "Medium",
                                HasRGB = true
                            },
                            new Mouse
                            {
                                Name = "Logitech G502",
                                Price = 99.99f,
                                Brand = "Logitech",
                                Color = "Black",
                                Description = "A Gaming mouse for pros",
                                ImageLink = @$"{assetsPath}logitechg502.png",
                                DPI = 3400,
                                ProductCategory = context.ProductCategories.FirstOrDefault(x => x.Name == "Mouse"),
                                SSN = await APIHelper.CreateSSN(Guid.NewGuid(), 8),
                                RightHanded = true,
                                HasRGB = true
                            }, new MousePad
                            {
                                Name = "Logitech G840XXL",
                                Price = 99.99f,
                                Brand = "Logitech",
                                Color = "Black",
                                Description = "A Gaming mousepad for pros",
                                ImageLink = @$"{assetsPath}logitechg840xxl.png",
                                WidthInMM = 900,
                                DepthInMM = 400,
                                ProductCategory = context.ProductCategories.FirstOrDefault(x => x.Name == "MousePad"),
                                SSN = await APIHelper.CreateSSN(Guid.NewGuid(), 87),
                                Surface = "Soft",
                                Material = "Plastic",
                                HasRGB = false
                            });
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}