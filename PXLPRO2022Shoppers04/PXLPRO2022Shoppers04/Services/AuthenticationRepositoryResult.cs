using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace PXLPRO2022Shoppers04.Services
{
    public class AuthenticationRepositoryResult
    {
        public IEnumerable<string> Errors => errors;
        public IdentityUser IdentityUser { get; set; }
        public SignInResult SignInResult { get; set; }
        public IdentityResult IdentityUserCreationResult { get; set; }
        public IdentityResult IdentityExternalLoginResult { get; set; }
        public bool Succeeded { get; set; }
        public void AddError(string error)
        {
            errors.Add(error);
        }
        List<string> errors = new List<string>();
    }
}