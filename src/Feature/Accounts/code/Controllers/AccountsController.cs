﻿using System;
using System.Web.Mvc;
using System.Web.Security;
using Sitecore.Demo.Shared.Feature.Accounts.Attributes;
using Sitecore.Demo.Shared.Feature.Accounts.Models;
using Sitecore.Demo.Shared.Feature.Accounts.Repositories;
using Sitecore.Demo.Shared.Feature.Accounts.Services;
using Sitecore.Demo.Shared.Foundation.Accounts.Services;
using Sitecore.Demo.Shared.Foundation.Alerts.Extensions;
using Sitecore.Demo.Shared.Foundation.Alerts.Models;
using Sitecore.Demo.Shared.Foundation.Dictionary.Repositories;
using Sitecore.Demo.Shared.Foundation.SitecoreExtensions.Attributes;
using Sitecore.Demo.Shared.Foundation.SitecoreExtensions.Extensions;
using Sitecore.Diagnostics;

namespace Sitecore.Demo.Shared.Feature.Accounts.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IFedAuthLoginButtonRepository _fedAuthLoginRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;
        private readonly IAccountsSettingsService _accountsSettingsService;
        private readonly IGetRedirectUrlService _getRedirectUrlService;
        private readonly IUserProfileService _userProfileService;
        private readonly IExportFileService _exportFileService;

        public AccountsController(IAccountRepository accountRepository, INotificationService notificationService, IAccountsSettingsService accountsSettingsService, IGetRedirectUrlService getRedirectUrlService, IUserProfileService userProfileService, IFedAuthLoginButtonRepository fedAuthLoginRepository, IExportFileService exportFileService)
        {
            _fedAuthLoginRepository = fedAuthLoginRepository;
            _accountRepository = accountRepository;
            _notificationService = notificationService;
            _accountsSettingsService = accountsSettingsService;
            _getRedirectUrlService = getRedirectUrlService;
            _userProfileService = userProfileService;
            _exportFileService = exportFileService;
        }

        public static string UserAlreadyExistsError => DictionaryPhraseRepository.Current.Get("/Accounts/Register/User Already Exists", "A user with specified e-mail address already exists");

        private static string ForgotPasswordEmailNotConfigured => DictionaryPhraseRepository.Current.Get("/Accounts/Forgot Password/Email Not Configured", "The Forgot Password E-mail has not been configured");

        private static string UserDoesNotExistError => DictionaryPhraseRepository.Current.Get("/Accounts/Forgot Password/User Does Not Exist", "User with specified e-mail address does not exist");

        [RedirectAuthenticated]
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult AccountsMenu()
        {
            var isLoggedIn = Context.IsLoggedIn && Context.PageMode.IsNormal;
            var accountsMenuInfo = new AccountsMenuInfo
            {
                IsLoggedIn = isLoggedIn,
                LoginInfo = !isLoggedIn ? CreateLoginInfo() : null,
                UserFullName = isLoggedIn ? Context.User.Profile.FullName : null,
                UserEmail = isLoggedIn ? Context.User.Profile.Email : null,
                AccountsDetailsPageUrl = _accountsSettingsService.GetPageLinkOrDefault(Context.Item, Templates.AccountsSettings.Fields.AccountsDetailsPage)
            };
            return View(accountsMenuInfo);
        }

        private LoginInfo CreateLoginInfo(string returnUrl = null)
        {
            string defaultRedirectUrl = _getRedirectUrlService.GetRedirectUrl(AuthenticationStatus.Authenticated);
            
            return new LoginInfo
            {
                ReturnUrl = returnUrl ?? defaultRedirectUrl
            };
        }

        [HttpPost]
        [ValidateModel]
        [RedirectAuthenticated]
        [ValidateRenderingId]
        public ActionResult Register(RegistrationInfo registrationInfo)
        {
            if (_accountRepository.Exists(registrationInfo.Email))
            {
                ModelState.AddModelError(nameof(registrationInfo.Email), UserAlreadyExistsError);
            }

            try
            {
                _accountRepository.RegisterUser(registrationInfo, _userProfileService.GetUserDefaultProfileId());

                string link = _getRedirectUrlService.GetRedirectUrl(AuthenticationStatus.Authenticated);
                Response.Redirect(link);
            }
            catch (MembershipCreateUserException ex)
            {
                Log.Error($"Can't create user with {registrationInfo.Email}", ex, this);
                ModelState.AddModelError(nameof(registrationInfo.Email), ex.Message);
            }

            return View(registrationInfo);
        }

        [RedirectAuthenticated]
        public ActionResult Login(string returnUrl = null)
        {
            return View(CreateLoginInfo(returnUrl));
        }

        public ActionResult LoginTeaser()
        {
            return View();
        }

        [HttpPost]
        [ValidateRenderingId]
        public ActionResult Login(LoginInfo loginInfo)
        {
            if (ModelState.IsValid)
            {
                return LoginUser(loginInfo);
            }

            return View(CreateLoginInfo());
        }

        protected virtual ActionResult LoginUser(LoginInfo loginInfo)
        {
            LoginInfo model = CreateLoginInfo();
            model.Email = loginInfo.Email;
            model.Password = loginInfo.Password;

            var user = _accountRepository.Login(loginInfo.Email, loginInfo.Password);
            if (user == null)
            {
                ModelState.AddModelError("invalidCredentials", DictionaryPhraseRepository.Current.Get("/Accounts/Login/User Not Found", "Username or password is not valid."));
            }
            else
            {
                var redirectUrl = model.ReturnUrl;
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    redirectUrl = _getRedirectUrlService.GetRedirectUrl(AuthenticationStatus.Authenticated);
                }

                Response.Redirect(redirectUrl);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateModel]
        public ActionResult _Login(LoginInfo loginInfo)
        {
            return LoginUser(loginInfo);
        }

        [HttpPost]
        [ValidateModel]
        public ActionResult LoginTeaser(LoginInfo loginInfo)
        {
            return LoginUser(loginInfo);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            _accountRepository.Logout();

            return Redirect(Context.Site.GetRootItem().Url());
        }

        [RedirectAuthenticated]
        public ActionResult ForgotPassword()
        {
            try
            {
                _accountsSettingsService.GetForgotPasswordMailTemplate();
            }
            catch (Exception)
            {
                return this.InfoMessage(InfoMessage.Error(ForgotPasswordEmailNotConfigured));
            }

            return View();
        }

        [HttpPost]
        [ValidateModel]
        [RedirectAuthenticated]
        public ActionResult ForgotPassword(PasswordResetInfo model)
        {
            if (!_accountRepository.Exists(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), UserDoesNotExistError);

                return View(model);
            }

            try
            {
                var newPassword = _accountRepository.RestorePassword(model.Email);
                _notificationService.SendPassword(model.Email, newPassword);
                return this.InfoMessage(InfoMessage.Success(DictionaryPhraseRepository.Current.Get("/Accounts/Forgot Password/Reset Password Success", "Your password has been reset.")));
            }
            catch (Exception ex)
            {
                Log.Error($"Can't reset password for user {model.Email}", ex, this);
                ModelState.AddModelError(nameof(model.Email), ex.Message);

                return View(model);
            }
        }

        [RedirectUnauthenticated]
        public ActionResult BasicInformation()
        {
            if (!Context.PageMode.IsNormal)
            {
                return View(_userProfileService.GetEmptyProfile());
            }

            var profile = _userProfileService.GetProfile(Context.User);

            return View(profile);
        }

        [RedirectUnauthenticated]
        public ActionResult EditProfile()
        {
            if (!Context.PageMode.IsNormal)
            {
                return View(_userProfileService.GetEmptyProfile());
            }

            var profile = _userProfileService.GetProfile(Context.User);
            return View(profile);
        }

        [HttpPost]
        [ValidateModel]
        [RedirectUnauthenticated]
        public virtual ActionResult EditProfile(EditProfile profile)
        {
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            if (profile.Email != null)
            {
                _userProfileService.SaveProfile(Context.User.Profile, profile);
                Session["EditProfileMessage"] = new InfoMessage(DictionaryPhraseRepository.Current.Get("/Accounts/Edit Profile/Edit Profile Success", "Profile was successfully updated"));
            }
            return Redirect(Request.RawUrl);
        }

        [RedirectUnauthenticated]
        public ActionResult ChangePassword()
        {
            ChangePasswordInfo passwordInfo = new ChangePasswordInfo();
            return View(passwordInfo);
        }

        [HttpPost]
        [RedirectUnauthenticated]
        public ActionResult ChangePassword(ChangePasswordInfo changePasswordInfo)
        {
            if (string.IsNullOrWhiteSpace(changePasswordInfo.Password))
            {
                ModelState.AddModelError(nameof(changePasswordInfo.Password), DictionaryPhraseRepository.Current.Get("/Accounts/Edit Profile/Old Password", "Please insert your old password."));
                return View(changePasswordInfo);
            }

            try
            {
                if (_accountRepository.ChangePassword(Context.User.Profile.Email, changePasswordInfo.Password, changePasswordInfo.NewPassword))
                {
                    Session["ChangePasswordMessage"] = new InfoMessage(DictionaryPhraseRepository.Current.Get("/Accounts/Forgot Password/Change Password Success", "Your password has been changed."));

                    return Redirect(Request.RawUrl);
                }
                else
                {
                    Log.Error($"Can't change password for user {Context.User.Profile.Email}", this);
                    ModelState.AddModelError(nameof(changePasswordInfo.Password), "Failed to change password for user");
                    return View(changePasswordInfo);
                }
            }
            catch (MembershipPasswordException ex)
            {
                Log.Error($"Can't change password for user {Context.User.Profile.Email}", ex, this);
                ModelState.AddModelError(nameof(changePasswordInfo.Password), ex.Message);

                return View(changePasswordInfo);
            }
        }

        public ActionResult FedAuth()
        {
            var model = new FedAuthLoginInfo
            {
                LoginButtons = _fedAuthLoginRepository.GetAll()
            };

            return View(model);
        }

        [RedirectUnauthenticated]
        public ActionResult ExportData()
        {
            return View(new ExportAccount() { AccountToBeExported = true });
        }

        [HttpPost]
        [RedirectUnauthenticated]
        public ActionResult ExportData(ExportAccount exportAccount)
        {
            if (Context.User.Profile.Email != null)
            {
                var fileNameWithExportedData = _userProfileService.ExportData(Context.User.Profile);

                if (!string.IsNullOrEmpty(fileNameWithExportedData))
                {
                    return RedirectToAction("ExportedDataDownload", new { fileName = fileNameWithExportedData });
                }
            }

            return View(new ExportAccount());
        }

        [HttpGet]
        public ActionResult ExportedDataDownload(string fileName)
        {
            var exportedData = _exportFileService.ReadExportedDataFromFile(fileName);

            return File(exportedData, "application/json", "ExportedContactData.json");
        }

        [RedirectUnauthenticated]
        public ActionResult DeleteAccount()
        {
            return View(new DeleteAccount() { AccountToBeDeleted = true });
        }

        [HttpPost]
        [RedirectUnauthenticated]
        public ActionResult DeleteAccount(DeleteAccount deleteAccount)
        {
            if (deleteAccount.AccountToBeDeleted)
            {

                if (Context.User.Profile.Email != null)
                {
                    _userProfileService.DeleteProfile(Context.User.Profile);
                    _accountRepository.Logout();
                }

                return Redirect(Context.Site.GetRootItem().Url());
            }

            return View(new DeleteAccount() { AccountToBeDeleted = true });
        }
    }
}