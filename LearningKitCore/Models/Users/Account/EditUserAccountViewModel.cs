using Kentico.Membership;

namespace LearningKitCore.Models.Users.Account
{
    public class EditUserAccountViewModel
    {
        public ApplicationUser User { get; set; }

        public bool AvatarUpdateFailed { get; set; }
    }
}
