using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.ViewModels.Account;

namespace SpeakerManagement.Repository
{
    #region interface
    public interface IAccountRepository
    {
        Task<string> UserSignIn(Login model);
        Task<bool> UserSignOut();
    }
    #endregion

    #region class
    public class AccountRepository : IAccountRepository
    {
        #region Private
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public AccountRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Public
        public async Task<string> UserSignIn(Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("UserId", user.Id);

                        // Get the role of the user
                        var role = await _userManager.GetRolesAsync(user);

                        // Determine the dashboard route based on the user's role
                        if (role.Contains(UserRoles.SuperAdmin))
                        {
                            return "/Home/SuperAdminDashboard";
                        }
                        else if (role.Contains(UserRoles.Admin))
                        {
                            return "/Home/AdminDashboard";
                        }
                        else if (role.Contains(UserRoles.Speaker))
                        {
                            return "/Home/SpeakerDashboard";
                        }
                        else
                        {
                            return "/Home/DefaultDashboard";
                        }
                    }
                }
            }
            // Return null or default route if login fails or user not found
            return "/Account/Login"; // Redirect to login page if sign-in fails
        }

        public async Task<bool> UserSignOut()
        {
            await _signInManager.SignOutAsync();
            return true;
        }
        #endregion
    }
    #endregion
}
