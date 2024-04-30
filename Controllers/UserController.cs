using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.Infrastructure;
using SpeakerManagement.Repository;
using SpeakerManagement.ViewModels.Account;

namespace SpeakerManagement.Controllers
{
    [Authorize(Roles = UserRoles.SuperAdmin+","+UserRoles.Admin)]
    public class UserController : Controller
    {
        #region Private
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion

        #region Constructor
        public UserController(
            IUserRepository userRepository,
            IOrganizationRepository organizationRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager
        )
        {
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Public
        public async Task<IActionResult> Index()
        {
            ViewBag.OrganizationList = await _organizationRepository.GetOrganizationDropDown();
            return View();
        }

        public async Task<IActionResult> GetUserList(GridSearch gridSearch)
        {
            var user = await _signInManager.UserManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            var usersList = new List<UserInfo>();
            if (roles.Contains(UserRoles.SuperAdmin))
            {
                usersList = await _userRepository.GetUsersList(UserRoles.Admin);
            }
            else if (roles.Contains(UserRoles.Admin))
            {
                usersList = await _userRepository.GetUsersList(UserRoles.Speaker);
            }

            return Json(usersList.SearchGrid(gridSearch));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var createdUserRole = string.Empty;
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (User.IsInRole(UserRoles.SuperAdmin))
                    createdUserRole = UserRoles.Admin;
                else if(User.IsInRole(UserRoles.Admin))
                    createdUserRole = UserRoles.Speaker;

                var result = await _userRepository.CreateUser(model, createdUserRole, user.OrganizationId, HttpContext.Request);
                
                if (result)
                    return Json(FormResult.Success());
            }
            return Json(FormResult.Error(ModelState));
        }

        public async Task<IActionResult> Get(string id)
        {
            var org = await _userRepository.Get(x => x.Id == id);
            if (org != null)
                return Json(org);
            else
                return Json(FormResult.Success("Record Not Found"));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.UpdateUserDetail(model);

                if (result)
                    return Json(FormResult.Success());
            }
            return Json(FormResult.Error(ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userRepository.DeleteUser(id);

            if (result)
                return Json(FormResult.Success());
            else
                return Json(FormResult.Success("Record Not Found"));
        }
        #endregion
    }
}
