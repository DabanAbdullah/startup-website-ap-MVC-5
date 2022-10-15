using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using lawzand.Models;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Helpers;
using lawzand.Utility;

namespace lawzand.Controllers
{

    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpPost]
        public JsonResult sudo(string pass)
        {
            string result = "no";

            try
            { lawzandEntities1 dbb = new lawzandEntities1();

                var rec = dbb.sudoes.FirstOrDefault();
                if (rec != null)
                {
                    if (rec.pass == pass) result = "ok";
                }
                else result = "no";


               
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }


            return Json(result);

        }




        [HttpPost]
        public async Task<JsonResult> lockout(string email)
        {
            string result = "";

            try
            {



                var user = await UserManager.FindByNameAsync(email);
                user.LockoutEnabled = true;
                user.LockoutEndDateUtc = DateTime.Now.AddYears(100);

                await UserManager.UpdateSecurityStampAsync(user.Id);
                var logins = user.Logins;
                if (logins != null)
                {
                    foreach (var login in logins)
                    {
                        await UserManager.RemoveLoginAsync(login.UserId,
                            new UserLoginInfo(
                                login.LoginProvider, login.ProviderKey));
                    }
                }
               
                await UserManager.UpdateAsync(user);

                result = "ok";
            }
            catch(Exception ex)
            {
                result = ex.Message;
            }


            return Json(result);

        }



        [HttpPost]
        public async Task<JsonResult> disablelockout(string email)
        {
            string result = "";

            try
            {

                var user = await UserManager.FindByNameAsync(email);
                user.LockoutEnabled = true;
                user.LockoutEndDateUtc = DateTime.Now.AddYears(-1) ;
                await UserManager.UpdateAsync(user);
                result = "ok";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }


            return Json(result);

        }




        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckUsername(string username)
        {
            lawzandEntities1 db = new lawzandEntities1();
            string photo = "";
            var rec = db.AspNetUsers.Where(x => x.Email == username).FirstOrDefault();
            if (rec != null)
            {
             //   photo = "../../"+rec.profilephoto;
                photo = photo.Replace("~", "");
            }
            else
            {
                photo = "../../Content/login/img-01.png";
               
            }
            return Json(photo);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
      
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    ModelState.AddModelError("", "You must have a confirmed email to log on.");
                    return View();
                }
            }
            // This doesn't count login failures towards account lockout  
            // To enable password failures to trigger account lockout, change to shouldLockout: true  
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, true, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "This Account Has been Locked Out,too much inavlid attempts Please try again later.");
                    return View(model);
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = true });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
      
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, FormCollection form, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            { //Use Namespace called :  System.IO  
                string FileName = "users.png";
                //To Get File Extension  
           

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, fullname=model.fullname,EmailConfirmed=true };
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                if (!roleManager.RoleExists("Admin"))
                {
                    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                    role.Name = "Admin";
                    roleManager.Create(role);
                }
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                   

                    await UserManager.AddToRoleAsync(user.Id, "Admin");

                  
                    await UserManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Adminpanel");

                    //  await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    //  string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //  var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");  
                    //  string body = string.Empty;
                    //  using (StreamReader reader = new StreamReader(Server.MapPath("~/Template/confirmation.html")))
                    //  {
                    //    body = reader.ReadToEnd();
                    // }
                    //  body = body.Replace("{ConfirmationLink}", callbackUrl);
                    // body = body.Replace("{UserName}", model.Email);
                    //  body = body.Replace("{title}", "Account Confimation");
                    // bool IsSendEmail =SendEmail.EmailSend(model.Email, "Confirm your account", body, true);
                    //if (IsSendEmail)
                    //    return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
            }
            // If we got this far, something failed, redisplay form  
            return View(model);
            // return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }




        [HttpPost]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public async Task<JsonResult> forgotpassword(string username)
        {

            string result = "";
            if (username.Length > 0)
            {

                result = "ok";
                var user = await UserManager.FindByNameAsync(username);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    result = ("User Does not Exist");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var callbackUrl = Url.Action("ResetPassword", "Account",
     new { UserId = user.Id, code = code }, protocol: Request.Url.Scheme);  // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");  
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Template/confirmation.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{ConfirmationLink}", callbackUrl);
                body = body.Replace("{UserName}", username);
                body = body.Replace("{title}", "Password Reset");
                bool IsSendEmail = SendEmail.EmailSend(username, "Reset Your Password", body, true);
                if (IsSendEmail)
                    result = ("Email has been sent to reset your password");
            }


            return Json(result);
        }






        //
        // POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}