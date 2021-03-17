using System;

using CMS.Membership;

using Kentico.PageBuilder.Web.Mvc;

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