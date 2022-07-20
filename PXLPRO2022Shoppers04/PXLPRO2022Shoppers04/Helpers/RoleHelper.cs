using System.Collections.Generic;

namespace PXLPRO2022Shoppers04.Helpers
{
    public class RoleHelper
    {
        public const string ClientRole = "Client";
        public const string AdminRole = "Admin";
        public static IEnumerable<string> Roles => GetRoles();

        private static List<string> GetRoles()
        {
            List<string> lst = new List<string>();
            lst.Add(ClientRole);
            lst.Add(AdminRole);
            return lst;
        }
    }
}