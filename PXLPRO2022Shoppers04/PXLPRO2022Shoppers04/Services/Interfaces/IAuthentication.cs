using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using PXLPRO2022Shoppers04.ViewModels;
using System.Threading.Tasks;

namespace PXLPRO2022Shoppers04.Services.Interfaces
{
    public interface IAuthentication
    {
        Task<AuthenticationRepositoryResult> LoginAsync(string userName, string email,
            string password);
        Task<AuthenticationRepositoryResult> ExternalLoginAsync(ExternalLoginInfo externalLoginInfo, UserInfoViewModel userInfoViewModel);
        Task<AuthenticationRepositoryResult> RegisterAsync(RegisterViewModel registerData);
        Task LogoutAsync();
        Task<AuthenticationRepositoryResult> GetIdentityUserAsync(string userName, string email, string password);
        AuthenticationProperties GetExtAuthProperties(string provider, string redirectUrl);
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();


    }
}