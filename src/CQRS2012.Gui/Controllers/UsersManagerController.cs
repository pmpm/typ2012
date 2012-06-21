using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Services;
using CQRS2012.Gui.ViewModels;

namespace CQRS2012.Gui.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersManagerController : Controller
    {
        private const string DefaultPassword = "123456";
        private IRankingService _rankingService;
        private readonly IBetStrategyService _betStrategyService;

        public UsersManagerController(IRankingService rankingService, IBetStrategyService betStrategyService)
        {
            this._rankingService = rankingService;
            this._betStrategyService = betStrategyService;
        }

        //
        // GET: /UsersManager/

        public ActionResult Index()
        {
            var membershipUsers = Membership.GetAllUsers();
            var users = new List<UserModel>();
            foreach (MembershipUser user in membershipUsers)
            {
                users.Add(new UserModel
                {
                    UserName = user.UserName,
                    IsApproved = user.IsApproved,
                    Email = user.Email,
                    IsOnline = user.IsOnline
                });
            }

            return View(users);
        }

        //
        // GET: /UsersManager/BlockUnblock

        [Authorize]
        public ActionResult BlockUnblock(string userName)
        {
            var user = Membership.GetUser(userName);

            return View(new UserModel
            {
                UserName = user.UserName,
                IsApproved = user.IsApproved,
                Email = user.Email,
                IsOnline = user.IsOnline
            });
        }

        //
        // POST: /UsersManager/BlockUnblock

        [Authorize]
        [HttpPost]
        public ActionResult BlockUnblock(UserModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user = Membership.GetUser(model.UserName);
                user.IsApproved = !user.IsApproved;
                Membership.UpdateUser(user);
                this._rankingService.UpdateRanking();
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /UsersManager/ResetPassword

        [Authorize]
        public ActionResult ResetPassword(string userName)
        {
            var user = Membership.GetUser(userName);
            ViewBag.DefaultPassword = DefaultPassword;
            return View(new UserModel
            {
                UserName = user.UserName,
                IsApproved = user.IsApproved,
                Email = user.Email,
                IsOnline = user.IsOnline
            });
        }

        //
        // POST: /UsersManager/ResetPassword

        [Authorize]
        [HttpPost]
        public ActionResult ResetPassword(UserModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user = Membership.GetUser(model.UserName);
                if (user.IsLockedOut)
                {
                    user.UnlockUser();
                }
                var generatedPass = user.ResetPassword();
                user.ChangePassword(generatedPass, DefaultPassword);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /UsersManager/CreateUser

        public ActionResult CreateUser()
        {
            var roles = new List<string> {""};
            roles.AddRange(Roles.GetAllRoles().ToList());
            ViewBag.Role = roles.Select(r => new SelectListItem {Text = r, Value = r});
         
            return View();
        }

        //
        // POST: /UsersManager/CreateUser

        [HttpPost]
        public ActionResult CreateUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    this._rankingService.SetupUserRanking(model.UserName);
                    this.TryAddUserToRole(model.UserName,model.Role);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult CreateRabbitUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRabbitUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //TODO: przy większej ilości strategi mozna dodac comboboxa w widoku do wyboru strategi i przekazywac nazwe strategi tutaj
                    var strategyName = "Randomer";
                   
                    this._betStrategyService.BindUserWithStrategy(model.UserName, strategyName);
                    this._betStrategyService.ApplyStrategyForUserToExistedGames(model.UserName, strategyName);
                    this._rankingService.SetupUserRanking(model.UserName);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            return View(model);
        }

        private void TryAddUserToRole(string userName, string role)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(role))
            {
                Roles.AddUserToRole(userName, role);
            }
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

    }
}
