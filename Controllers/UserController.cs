using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakerManagement.Data;
using SpeakerManagement.Entities;
using SpeakerManagement.Helper;
using SpeakerManagement.Repository;

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
            ViewBag.UserRoleList = Common.UserRoleList();
            return View();
        }

        public async Task<IActionResult> GetUserList(GridSearch gridSearch, string UserRole)
        {
            var result = await _userRepository.GetUsersList(gridSearch);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser model, string UserRole = UserRoles.Speaker)
        {
            if (ModelState.IsValid)
            {
                int orgId = 0;
                ApplicationUser user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(UserRoles.Admin))
                        orgId = user.OrganizationId;
                    else if (roles.Contains(UserRoles.SuperAdmin))
                        orgId = model.OrganizationId;
                }

                var result = await _userRepository.CreateUser(model, UserRole, orgId, HttpContext.Request);

                if (result.IsSuccess)
                    return Json(FormResult.Success(true, result.Message));
                else
                    return Json(FormResult.Success(false, Params.SaveErr));
            }
            return Json(FormResult.Error(ModelState));
        }

        public async Task<IActionResult> Get(string id)
        {
            var user = await _userRepository.Get(x => x.Id == id);

            if (user != null)
                return Json(FormResult.Success(true, Params.Get, user));
            else
                return Json(FormResult.Success(false, Params.NoRecord));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.UpdateUserDetail(model);

                if (result.IsSuccess)
                    return Json(FormResult.Success(true, Params.Update));
                else
                    return Json(FormResult.Success(false, Params.UpdateErr));
            }
            return Json(FormResult.Error(ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userRepository.DeleteUser(id);

            if (result.IsSuccess)
                return Json(FormResult.Success(true, Params.Delete));
            else
                return Json(FormResult.Success(false, Params.NoRecord));
        }
        #endregion
    }
}
