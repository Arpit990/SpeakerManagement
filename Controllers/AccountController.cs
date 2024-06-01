using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Repository;
using SpeakerManagement.ViewModels.Account;

namespace SpeakerManagement.Controllers
{
    public class AccountController : Controller
    {
        #region Private
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructor
        public AccountController(
            IUserRepository userRepository, 
            IAccountRepository accountRepository
        )
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }
        #endregion

        #region Public
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login() => View(new Login());

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var dashboardRoute = await _accountRepository.UserSignIn(login);

                if (dashboardRoute != null)
                {
                    // Redirect to the appropriate dashboard route based on the user's role
                    return Redirect(dashboardRoute);
                }
                else
                {
                    TempData["Error"] = "Wrong credentials. Please, try again!";
                }
            }
            return View(login);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.UserSignOut();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ResetPassword() => View();

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId != null && token != null)
            {
                var result = await _userRepository.ConfirmEmail(userId, token);
                if (result.IsSuccess)
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            return RedirectToAction("Error", "Home");
        }
        #endregion
    }
}
