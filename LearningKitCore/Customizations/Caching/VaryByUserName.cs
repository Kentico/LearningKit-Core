using CMS.Membership;
using Kentico.PageBuilder.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningKitCore.Customizations.Caching
{
    public class VaryByUserName : ICacheVaryByOption
    {
        public string GetKey()
        {
            string userFirstName = MembershipContext.AuthenticatedUser?.FirstName ?? String.Empty;

            return $"UserFirstName={userFirstName}";
        }
    }
}
