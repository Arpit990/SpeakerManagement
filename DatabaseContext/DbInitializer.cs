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
                string adminUserEmail = "admin@seventhharmonymarketing.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FirstName = "Arpit",
                        LastName = "Santoki",
                        UserName = "arpit.santoki",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "V8ZgFp0XXk+IBg0VNd5qrw==");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.SuperAdmin);
                }


                var adminUsers = UsersList();

                foreach (var user in adminUsers)
                {
                    var appUser = await userManager.FindByEmailAsync(user.Email);
                    if (appUser == null)
                    {
                        var newAppUser = new ApplicationUser()
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            UserName = user.UserName,
                            Email = user.Email,
                            EmailConfirmed = true,
                            PhoneNumber = user.Phone,
                            PhoneNumberConfirmed = true,
                            OrganizationId = user.OrganizationId
                        };
                        await userManager.CreateAsync(newAppUser, user.Password);
                        await userManager.AddToRoleAsync(newAppUser, user.Role);
                    }
                }
            }
        }

        public static List<ApplicationUsers> UsersList()
        {
            // Create a list to store ApplicationUsers objects
            List<ApplicationUsers> userList = new List<ApplicationUsers>();

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Oleta",
                LastName = "Abbott",
                Email = "dpettegre6@columbia.edu",
                Phone = "+62 640 802 7111",
                UserName = "dpettegre6",
                Password = "$07&YVmhktgYVS",
                OrganizationId = 1,
                Role = UserRoles.Admin
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Marcel",
                LastName = "Jones",
                Email = "acharlota@liveinternet.ru",
                Phone = "+967 253 210 0344",
                UserName = "acharlota",
                Password = "$07&M9lbMdydMN",
                OrganizationId = 2,
                Role = UserRoles.Admin
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Trace",
                LastName = "Douglas",
                Email = "lgribbinc@posterous.com",
                Phone = "+1 609 937 3468",
                UserName = "lgribbinc",
                Password = "$07&ftGj8LZTtv9g",
                OrganizationId = 3,
                Role = UserRoles.Admin
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Maurine",
                LastName = "Stracke",
                Email = "kdulyt@umich.edu",
                Phone = "+48 143 590 6847",
                UserName = "kdulyt",
                Password = "$07&5t6q4KC7O",
                OrganizationId = 4,
                Role = UserRoles.Admin
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Macy",
                LastName = "Greenfelder",
                Email = "jissetts@hostgator.com",
                Phone = "+81 915 649 2384",
                UserName = "jissetts",
                Password = "$07&ePawWgrnZR8L",
                OrganizationId = 5,
                Role = UserRoles.Admin
            });

            // Adding ApplicationUsers objects for each user
            userList.Add(new ApplicationUsers()
            {
                FirstName = "Piper",
                LastName = "Schowalter",
                Email = "fokillq@amazon.co.jp",
                Phone = "+60 785 960 7918",
                UserName = "fokillq",
                Password = "$07&xZnWSWnqH",
                OrganizationId = 1,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Griffin",
                LastName = "Braun",
                Email = "lgronaverp@cornell.edu",
                Phone = "+62 511 790 0161",
                UserName = "lgronaverp",
                Password = "$07&4a1dAKDv9KB9",
                OrganizationId = 2,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Felicity",
                LastName = "O'Reilly",
                Email = "beykelhofm@wikispaces.com",
                Phone = "+63 919 564 1690",
                UserName = "beykelhofm",
                Password = "$07&zQwaHTHbuZyr",
                OrganizationId = 3,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Tressa",
                LastName = "Weber",
                Email = "froachel@howstuffworks.com",
                Phone = "+34 517 104 6248",
                UserName = "froachel",
                Password = "$07&rfVSKImC",
                OrganizationId = 4,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Doyle",
                LastName = "Ernser",
                Email = "ckensleyk@pen.io",
                Phone = "+86 634 419 6839",
                UserName = "ckensleyk",
                Password = "$07&tq7kPXyf",
                OrganizationId = 5,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Miles",
                LastName = "Cummerata",
                Email = "yraigatt3@nature.com",
                Phone = "+86 461 145 4186",
                UserName = "yraigatt3",
                Password = "$07&sRQxjPfdS",
                OrganizationId = 1,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Luciano",
                LastName = "Sauer",
                Email = "smargiottau@altervista.org",
                Phone = "+420 491 212 0935",
                UserName = "smargiottau",
                Password = "$07&pSGvhgS2A",
                OrganizationId = 2,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Kaya",
                LastName = "Emard",
                Email = "lskeelv@webeden.co.uk",
                Phone = "+1 813 801 4535",
                UserName = "lskeelv",
                Password = "$07&Eolj9Svg28",
                OrganizationId = 3,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Darien",
                LastName = "Witting",
                Email = "aaughtonx@businessweek.com",
                Phone = "+33 888 700 6632",
                UserName = "aaughtonx",
                Password = "$07&Vzwc72RhNGu",
                OrganizationId = 4,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Thaddeus",
                LastName = "McCullough",
                Email = "igannan11@microsoft.com",
                Phone = "+62 358 347 4052",
                UserName = "igannan11",
                Password = "$07&MB63jkg3W",
                OrganizationId = 5,
                Role = UserRoles.Speaker
            });

            userList.Add(new ApplicationUsers()
            {
                FirstName = "Jasmin",
                LastName = "Hermiston",
                Email = "lgutridge13@sun.com",
                Phone = "+81 649 401 1274",
                UserName = "lgutridge13",
                Password = "$07&SqR03CE",
                OrganizationId = 1,
                Role = UserRoles.Speaker
            });

            return userList;
        }
    }

    public class ApplicationUsers
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int OrganizationId { get; set; }
        public string Role { get; set; }
    }
}
