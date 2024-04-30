using Microsoft.AspNetCore.Identity;
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
        #endregion

        #region Constructor
        public AccountRepository(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                        // Get the role of the user
                        var role = await _userManager.GetRolesAsync(user);

                        // Determine the dashboard route based on the user's role
                        if (role.Contains("Super Admin"))
                        {
                            return "/Home/SuperAdminDashboard";
                        }
                        else if (role.Contains("Admin"))
                        {
                            return "/Home/AdminDashboard";
                        }
                        else if (role.Contains("Speaker"))
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
