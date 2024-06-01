using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpeakerManagement.Data;
using SpeakerManagement.DatabaseContext;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.ViewModels.Account;
using System.Net;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<GridResult> GetUsersList(GridSearch gridSearch);
        Task<List<SelectListItem>> GetUsersForDropDown();
        Task<MethodResponse<string>> CreateUser(ApplicationUser user, string role, int organizationId, HttpRequest request);
        Task<MethodResponse<string>> ConfirmEmail(string userId, string token);
        Task<MethodResponse<string>> UpdateUserDetail(ApplicationUser user);
        Task<MethodResponse<string>> DeleteUser(string userId);
    }
    #endregion

    #region class
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        #region Private
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region Constructor
        public UserRepository(
            DataContext dataContext,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor
        ) : base(dataContext, httpContextAccessor)
        {
            _userManager = userManager;
        }
        #endregion

        #region Public
        public async Task<GridResult> GetUsersList(GridSearch gridSearch)
        {
            int orgId = 0;
            string role = string.Empty;
            var loginUserDetail = await GetLoggedInUserDetail();
            var loginUserRole = await GetLoggedInUserRoles();

            if(loginUserRole.IsSuccess) 
            {
                if(loginUserRole.Data.Contains(UserRoles.Admin) && loginUserDetail.Data is ApplicationUser appUser)
                {
                    orgId = appUser.OrganizationId;
                    role = UserRoles.Speaker;
                }
            }

            var query = from x_user in _context.Users
                        join x_user_role in _context.UserRoles on x_user.Id equals x_user_role.UserId
                        join x_role in _context.Roles on x_user_role.RoleId equals x_role.Id
                        join x_org in _context.Organizations on x_user.OrganizationId equals x_org.Id
                        select new UserInfo
                        {
                            UserId = x_user.Id,
                            UserName = x_user.UserName,
                            FirstName = x_user.FirstName,
                            LastName = x_user.LastName,
                            Email = x_user.Email,
                            PhoneNumber = x_user.PhoneNumber,
                            Website = x_user.Website,
                            OrganizationName = x_org.OrganizationName,
                            Role = x_role.Name,
                            OrganizationId = x_org.Id,
                            IsActive = x_user.EmailConfirmed
                        };

            if(!string.IsNullOrEmpty(role))
                query = query.Where(x => x.Role == role);

            if(orgId != 0)
                query = query.Where(x => x.OrganizationId == orgId);

            var result = PredicateSearchExt(gridSearch, query);

            return result;
        }
        
        public async Task<List<SelectListItem>> GetUsersForDropDown()
        {
            int orgId = 0;
            var loginUserDetail = await GetLoggedInUserDetail();

            if (loginUserDetail.IsSuccess)
            {
                if (loginUserDetail.Data is ApplicationUser appUser)
                {
                    orgId = appUser.OrganizationId;
                }
            }

            return await (from user in _context.Users
                          join userrole in _context.UserRoles on user.Id equals userrole.UserId
                          join role in _context.Roles on userrole.RoleId equals role.Id
                          where role.Name == UserRoles.Speaker && user.OrganizationId == orgId
                          select new SelectListItem
                          {
                              Value = user.Id.ToString(),
                              Text = user.FirstName
                          }).OrderBy(x => x.Text).ToListAsync();
        }

        public async Task<MethodResponse<string>> CreateUser(ApplicationUser user, string role, int organizationId, HttpRequest request)
        {
            var adminUser = await _userManager.FindByEmailAsync(user.Email);
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = $"{user.FirstName}.{user.LastName}",
                    PhoneNumber = user.PhoneNumber,
                    OrganizationId = organizationId,
                    Email = user.Email,
                    Website = user.Website
                };

                var autoGeneratedPassword = Common.GeneratePassword();

                // Create the user in the database
                var result = await _userManager.CreateAsync(newAdminUser, autoGeneratedPassword);
                if (result.Succeeded)
                {
                    // Generate email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newAdminUser);

                    // Add user to Admin role
                    await _userManager.AddToRoleAsync(newAdminUser, role);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Construct the confirmation link manually
                    var confirmationLink = $"{request.Scheme}://{request.Host}/Account/ConfirmEmail?userId={newAdminUser.Id}&token={WebUtility.UrlEncode(token)}";

                    SendInvitation(organizationId, user.FirstName, user.Email, confirmationLink, autoGeneratedPassword);

                    return MethodResponse<string>.Success(string.Empty, "Invitation Sent Successfully");
                }
                return MethodResponse<string>.Fail("Error in user creation");
            }
            return MethodResponse<string>.Fail($"{user.Email} already exist!");
        }

        public async Task<MethodResponse<string>> ConfirmEmail(string userId, string token)
        {
            if (userId != null && token != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (result.Succeeded)
                    {
                        return MethodResponse<string>.Success(string.Empty, $"Email: {user.Email} verify successfully");
                    }
                }
                return MethodResponse<string>.Fail("User not found");
            }
            return MethodResponse<string>.Fail("Invalid UserId or Token");
        }

        public async Task<MethodResponse<string>> UpdateUserDetail(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                return MethodResponse<string>.Fail("User not found with given UserId");
            }

            // Update the user details
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.UserName = $"{user.FirstName}_{user.LastName}";
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Website = user.Website;
            existingUser.OrganizationId = user.OrganizationId;

            var result = await _userManager.UpdateAsync(existingUser);
            if (result.Succeeded)
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                return MethodResponse<string>.Success(string.Empty, "User details updated successfully");
            }

            return MethodResponse<string>.Fail("Error while updating user details");
        }

        public async Task<MethodResponse<string>> DeleteUser(string userId)
        {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Delete the user using UserManager
                _ = await _userManager.DeleteAsync(user);
                return MethodResponse<string>.Success(string.Empty, "User deleted successfully");
            }
            return MethodResponse<string>.Fail("User not found");
        }

        public async Task<MethodResponse<object>> GetLoggedInUserDetail()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user != null)
            {
                return MethodResponse<object>.Success(user);
            }
            return MethodResponse<object>.Fail("User Not Found");
        }

        public async Task<MethodResponse<IList<string>>> GetLoggedInUserRoles()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return MethodResponse<IList<string>>.Success(roles);
            }
            return MethodResponse<IList<string>>.Fail("User Not Found");
        }

        #endregion

        #region private
        private MethodResponse<string> SendInvitation(int orgId, string name, string userEmail, string confirmationLink, string password)
        {
            var org = _context.Organizations.Where(x => x.Id == orgId).AsNoTracking().FirstOrDefault();

            if (org == null)
                return MethodResponse<string>.Fail("Organization not found with given id");

            var subject = $"{org.OrganizationName} has invited you to be a Speaker!";
            var message = @$"
                            <html>
                                <head>
                                    <title>Invitation Email</title>
                                </head>
                                <body>
                                    <p>Hello {name}, you have been invited by {org.OrganizationName} to be a Speaker for a future event.</p>
                                    <p>Complete your account registration by clicking this link or copy & pasting it into your preferred browser:</p>
                                    <p><a href='{confirmationLink}'>{confirmationLink}</a></p>
                                    <p>After confirm login you can login using Password: {password}</p>
                                </body>
                            </html>";

            MailSender.SendEmailConfirmationMail(userEmail, subject, message, "");

            return MethodResponse<string>.Success("Mail Sent Successfully");
        }
        #endregion
    }
    #endregion
}
