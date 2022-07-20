using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Helpers;
using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.Services.Interfaces;
using PXLPRO2022Shoppers04.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace PXLPRO2022Shoppers04.Controllers
{
    public class AccountController : Controller
    {
        UserManager<IdentityUser> _userManager;
        SignInManager<IdentityUser> _signInManager;
        AppDbContext _context;
        RoleManager<IdentityRole> _roleManager;
        IAuthentication _repo;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, AppDbContext context, IAuthentication authenticationRepository,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _repo = authenticationRepository;
            _roleManager = roleManager;
        }


        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await _repo.LogoutAsync();
            return RedirectToAction("Login");
        }

        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerData)
        {
            if (ModelState.IsValid)
            {
                var result = await _repo.RegisterAsync(registerData);
                if (result.Succeeded)
                    return View("Login", new LoginViewModel());
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                    return View(registerData);
                }
            }

            return View(registerData);
        }

        #endregion

        #region LoginWithUsername & LoginWithEmail

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(login.Username);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(login.Username))
                    {
                        var result = await _repo.LoginAsync(null, login.Username, login.Password);
                        if (result.Succeeded) return RedirectToAction("Index", "Home");
                    }
                }

                var user2 = await _userManager.FindByNameAsync(login.Username);
                if (user2 != null)
                {
                    if (!string.IsNullOrEmpty(login.Username))
                    {
                        var result = await _repo.LoginAsync(login.Username, null, login.Password);
                        if (result.Succeeded) return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "No account found with this username/email or the password you filled in is incorrect.");
            }

            return View(login);
        }

        #endregion

        #region Twitter

        public async Task<IActionResult> TwitterLogin()
        {
            string redirectUrl = Url.Action("TwitterResponse", "Account");
            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties("Twitter", redirectUrl);
            return new ChallengeResult("Twitter", properties);
        }

        public async Task<IActionResult> TwitterResponse()
        {
            //retrieve information that was send in the http request (by twitter)
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                //user did not login properly with facebook -> redirect to login page
                return RedirectToAction(nameof(Login));
            }

            //Put info provided by twitter (claims) into a model
            string userName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Name).Value;
            userName =
                $"{userName}_{externalLoginInfo.LoginProvider}_{externalLoginInfo.ProviderKey}"; //make sure username is unique
            Guid userId = Guid.NewGuid();
            UserInfoViewModel userInfoViewModel = new UserInfoViewModel
            {
                UserName = userName,
                Email = userName + "_" + userId +
                        "@generatedemail.com" //creating unique email adress as Twitter doesn't provide
            };

            //try to sign in with twitter user id (ProviderKey)
            SignInResult result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            //Sign in failed -> user does not exist yet in our database -> create one

            IdentityUser user = new IdentityUser(userInfoViewModel.UserName)
            {
                Email = userInfoViewModel.Email
            };
            IdentityResult identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                //link the created user to the facebook login info
                identityResult = await _userManager.AddLoginAsync(user, externalLoginInfo);
                if (identityResult.Succeeded)
                {
                    var roleClient = await _roleManager.FindByNameAsync(RoleHelper.ClientRole);
                    if (roleClient != null)
                    {
                        var r = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) |(?<=[^A-Z])(?=[A-Z]) |(?<=[A-Za-z])(?=[^A-Za-z])",
                            RegexOptions.IgnorePatternWhitespace);
                        string s = externalLoginInfo.Principal.FindFirst(ClaimTypes.Name).Value;
                        var extracted = $"{r.Replace(s, " ")}";
                        var splittedName = extracted.Split(" ");

                        await _userManager.AddToRoleAsync(user, roleClient.Name);
                        _context.Clients.AddRange(new Client
                        {
                            User = user,
                            Name = splittedName[0],
                            FirstName = splittedName[1],
                        });
                        await _context.SaveChangesAsync();
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            if (_userManager.Users.Any(x => x.Email == userInfoViewModel.Email))
            {
                await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                    externalLoginInfo.ProviderKey, false);
            }

            return View("AccessDenied");
        }

        #endregion

        #region Google

        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("ExternalLoginResponse", "Account");
            var properties = _repo.GetExtAuthProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }

        #endregion

        #region Facebook

        public IActionResult FacebookLogin()
        {
            string redirectUrl = Url.Action("ExternalLoginResponse", "Account");
            var properties = _repo.GetExtAuthProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }


        public async Task<IActionResult> ExternalLoginResponse()
        {
            ExternalLoginInfo externalLoginInfo = await _repo.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null) return RedirectToAction(nameof(Login));
            var userInfo = GetUserInfoModel(externalLoginInfo);
            var result = await _repo.ExternalLoginAsync(externalLoginInfo, userInfo);
            if (!result.SignInResult.Succeeded)
            {
                if (!result.IdentityUserCreationResult.Succeeded)
                {
                    return View("Login");
                }
                else
                {
                    if (!result.IdentityExternalLoginResult.Succeeded)
                    {
                        return View("Login");
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        UserInfoViewModel GetUserInfoModel(ExternalLoginInfo externalLoginInfo)
        {
            //Put info provided by IdentityServer, google or facebook (claims) into a model
            //identityserver
            var externalUser = externalLoginInfo.Principal;
            string userName;
            string firstName;
            string lastName;
            if (externalLoginInfo.Principal.FindFirst(ClaimTypes.Name) != null)
            {
                string fullName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Name).Value;
                string[] names = fullName.Split(" ");
                firstName = names[0];
                lastName = names[1];
                userName = fullName.Replace(" ", "");
            }

            else
            {
                string fullName = externalUser.FindFirst("name").Value;
                string[] names = fullName.Split(" ");
                firstName = names[0];
                lastName = names[1];
                userName = fullName.Replace(" ", "");
            }


            string email;
            if (externalLoginInfo.Principal.FindFirst(ClaimTypes.Email) != null)
                email = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value;
            else
                email = externalLoginInfo.Principal.FindFirst("email").Value;

            //make sure username is unique
            UserInfoViewModel userInfoModel = new UserInfoViewModel
            {
                UserName = userName,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            return userInfoModel;

        }

        #endregion

        #region Discord

        public IActionResult DiscordLogin()
        {
            string redirectUrl = Url.Action("DiscordResponse", "Account");
            var properties = _repo.GetExtAuthProperties("Discord", redirectUrl);
            return new ChallengeResult("Discord", properties);
        }

        public async Task<IActionResult> DiscordResponse()
        {
            //retrieve information that was send in the http request (by facebook)
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                //user did not login properly with discord -> redirect to login page
                return RedirectToAction(nameof(Login));
            }

            //Put info provided by discord (claims) into a model
            string userName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Name).Value;
            userName =
                $"{userName}_{externalLoginInfo.LoginProvider}_{externalLoginInfo.ProviderKey}"; //make sure username is unique
            Guid userId = Guid.NewGuid();
            UserInfoViewModel userInfoViewModel = new UserInfoViewModel
            {
                UserName = userName,
                Email = userName + "_" + userId +
                        "@generatedemail.com" //creating unique email adress as Discord doesn't provide
            };
            //try to sign in with discord user id (ProviderKey)
            SignInResult result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            //Sign in failed -> user does not exist yet in our database -> create one

            IdentityUser user = new IdentityUser(userInfoViewModel.UserName)
            {
                Email = userInfoViewModel.Email
            };
            IdentityResult identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                //link the created user to the discord login info
                identityResult = await _userManager.AddLoginAsync(user, externalLoginInfo);
                if (identityResult.Succeeded)
                {
                    var roleClient = await _roleManager.FindByNameAsync(RoleHelper.ClientRole);
                    if (roleClient != null)
                    {
                        _context.Clients.AddRange(new Client
                        {
                            User = user
                        });
                        await _context.SaveChangesAsync();
                        await _userManager.AddToRoleAsync(user, roleClient.Name);
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            if (_userManager.Users.Any(x => x.Email == userInfoViewModel.Email))
            {
                await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                    externalLoginInfo.ProviderKey, false);
            }

            return View("AccessDenied");
        }

        #endregion

        #region external login - IdentityServer
        public IActionResult IdentityServerLogin()
        {
            string redirectUrl = Url.Action("ExternalLoginResponse", "Account");
            string authScheme = OpenIdConnectDefaults.AuthenticationScheme;
            var properties = _repo.GetExtAuthProperties(authScheme, redirectUrl);
            return new ChallengeResult(authScheme, (Microsoft.AspNetCore.Authentication.AuthenticationProperties)properties);
        }
        #endregion
    }
}