using Microsoft.AspNetCore.Identity;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;

namespace SpeakerManagement.DatabaseContext
{
    public class DbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DataContext>();

                context.Database.EnsureCreated();

                //Organization
                if (!context.Organizations.Any())
                {
                    context.Organizations.AddRange(new List<Organization>()
                    {
                        new Organization()
                        {
                            OrganizationName = "Dave Cross, LLC",
                        },
                        new Organization()
                        {
                            OrganizationName = "Seventh Harmony",
                        },
                        new Organization()
                        {
                            OrganizationName = "Crossfire Ventures",
                        },
                        new Organization()
                        {
                            OrganizationName = "Harmonic Ventures, LLC",
                        },
                        new Organization()
                        {
                            OrganizationName = "Elysian Echoes, LLC",
                        }
                    });
                    context.SaveChanges();
                }

                //Task
                if (!context.Tasks.Any())
                {
                    context.Tasks.AddRange(new List<Tasks>()
                    {
                        new Tasks()
                        {
                            TaskName = "Submit Bio",
                            InputType = "Textarea",
                        },
                        new Tasks()
                        {
                            TaskName = "Submit Headshot",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Submit W9",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 1 Video",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 2 Video",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 3 Video",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 1 Notes",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 2 Notes",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 3 Notes",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 1 Bonus",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 2 Bonus",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Class 3 Bonus",
                            InputType = "File",
                        },
                        new Tasks()
                        {
                            TaskName = "Swag Bag Ad",
                            InputType = "File",
                        }
                    });
                    context.SaveChanges();
                }
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.SuperAdmin));

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await roleManager.RoleExistsAsync(UserRoles.Speaker))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Speaker));

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string adminUserEmail = "admin@shmarketing.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FirstName = "Arpit",
                        LastName = "Santoki",
                        UserName = "arpit_santoki",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.SuperAdmin);
                }

                /*
                string appUserEmail = "user@shmarketing.com";

                var appUser = await userManager.FindByEmailAsync(appUserEmail);
                if (appUser == null)
                {
                    var newAppUser = new ApplicationUser()
                    {
                        FirstName = "Arpit",
                        LastName = "Santoki",
                        UserName = "app-user",
                        Email = appUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAppUser, "Coding@1234?");
                    await userManager.AddToRoleAsync(newAppUser, UserRoles.Speaker);
                }
                */
            }
        }
    }
}
