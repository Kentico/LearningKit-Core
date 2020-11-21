using CMS.Base;
using CMS.Membership;

using Kentico.Forms.Web.Mvc;


namespace LearningKitCore.Components.FormBuilder.VisibilityConditions
{
    // Visibility condition that evaluates whether the current user
    // in the administration interface has the 'Administrator' privilege level
    public class AdministratorPropertyCondition : VisibilityCondition
    {
        // Determines whether the property is visible
        public override bool IsVisible()
        {
            // True if the current user's privilege level is 'Administrator' or higher
            // In effect, the condition hides properties for users with the 'Editor' level
            return MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
        }
    }
}
