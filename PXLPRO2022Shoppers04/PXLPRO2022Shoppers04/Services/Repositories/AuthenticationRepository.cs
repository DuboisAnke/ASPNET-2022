using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PXLPRO2022Shoppers04.Data;
using PXLPRO2022Shoppers04.Helpers;
using PXLPRO2022Shoppers04.Models;
using PXLPRO2022Shoppers04.Services.Interfaces;
using PXLPRO2022Shoppers04.ViewModels;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Services.Repositories
{
    public class AuthenticationRepository : IAuthentication
    {
        UserManager<IdentityUser> _userManager;
        SignInManager<IdentityUser> _signinManager;
        RoleManager<IdentityRole> _roleManager;
        AppDbContext _context;
        public AuthenticationRepository(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signinManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<AuthenticationRepositoryResult> RegisterAsync(
            RegisterViewModel registerData)
        {
            var result = new AuthenticationRepositoryResult();
            var identityUser = new IdentityUser
            {
                UserName = registerData.UserName,
                Email = registerData.Email
            };
            var identityResult = await _userManager.CreateAsync(identityUser,
                registerData.Password);
            if (identityResult.Succeeded)
            {
                var roleClient = await _roleManager.FindByNameAsync(RoleHelper.ClientRole);
                await _userManager.AddToRoleAsync(identityUser, roleClient.Name);
                _context.Clients.AddRange(
                    new Client
                    {
                        FirstName = registerData.Firstname,
                        Name = registerData.Lastname,
                        User = identityUser
                    });
                await _context.SaveChangesAsync();
                result.Succeeded = true;
            }
            else
            {
                foreach (var error in identityResult.Errors)
                {
                    result.AddError(error.Description);
                }
            }

            return result;
        }

        private async Task<AuthenticationRepositoryResult> GetIdentityUserAsync(string userName, string email,
            string password)
        {
            var result = new AuthenticationRepositoryResult();
            IdentityUser user = null;
            if (userName != null)
            {
                user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    result.AddError("Geen gebruiker gevonden met deze username!");
            }
            else
            {
                if (email != null)
                {
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                        result.AddError("Geen gebruiker gevonden met dit email adres!");
                }
            }

            if (user != null)
            {
                result.IdentityUser = user;
                var identityResult = await _signinManager.PasswordSignInAsync(user, password, false, false);
                if (identityResult.Succeeded)
                    result.Succeeded = true;
            }

            return result;
        }

        private async Task<AuthenticationRepositoryResult> LoginAsync(IdentityUser user,
            string password)
        {
            var result = new AuthenticationRepositoryResult();
            var identityResult = await _signinManager.PasswordSignInAsync(user, password,
                false, false);
            if (identityResult.Succeeded)
                result.Succeeded = true;
            else
                result.AddError("Problemen met inloggen!");
            return result;
        }

        public async Task<AuthenticationRepositoryResult> LoginAsync(string username, string email,
            string password)
        {
            var result = await GetIdentityUserAsync(username, email, password);
            if (result.Succeeded)
                result = await LoginAsync(result.IdentityUser, password);
            return result;
        }

        public async Task LogoutAsync()
        {
            await _signinManager.SignOutAsync();
        }

        async Task<AuthenticationRepositoryResult> IAuthentication.GetIdentityUserAsync(string userName, string email, string password)
        {
            var result = new AuthenticationRepositoryResult();
            IdentityUser user = null;
            if (userName != null)
            {
                user = await _userManager.FindByNameAsync(userName);
                if (user == null) result.AddError("Geen gebruiker gevonden met deze username!");
            }
            else
            {
                if (email != null)
                {
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null) result.AddError("Geen gebruiker gevonden met dit email adres!");
                }
            }
            if (user != null)
            {
                result.IdentityUser = user;
                var identityResult = await _signinManager.PasswordSignInAsync(user, password, false, false);
                if (identityResult.Succeeded) result.Succeeded = true;
            }
            return result;
        }

        public AuthenticationProperties GetExtAuthProperties(string provider, string redirectUrl)
        {
            return _signinManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return await _signinManager.GetExternalLoginInfoAsync();
        }

        public async Task<AuthenticationRepositoryResult> ExternalLoginAsync(ExternalLoginInfo externalLoginInfo, UserInfoViewModel userInfoViewModel)
        {
            var result = new AuthenticationRepositoryResult();
            result.SignInResult = await _signinManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, false);
            if (!result.SignInResult.Succeeded)
            {
                IdentityUser user = new IdentityUser(userInfoViewModel.UserName)
                {
                    Email = userInfoViewModel.Email,
                };
                result.IdentityUserCreationResult = await _userManager.CreateAsync(user);
                if (result.IdentityUserCreationResult.Succeeded)
                {
                    var roleClient = await _roleManager.FindByNameAsync(RoleHelper.ClientRole);
                    await _userManager.AddToRoleAsync(user, roleClient.Name);


                    _context.Clients.AddRange(
                        new Client
                        {
                            IdentityUserId = user.Id,
                            FirstName = userInfoViewModel.FirstName,
                            Name = userInfoViewModel.LastName
                        });
                    await _context.SaveChangesAsync();

                    result.IdentityExternalLoginResult = await _userManager.AddLoginAsync(user, externalLoginInfo);
                    if (result.IdentityExternalLoginResult.Succeeded)
                    {
                        await _signinManager.SignInAsync(user, false);
                    }
                }
            }
            return result;
        }
    }
}