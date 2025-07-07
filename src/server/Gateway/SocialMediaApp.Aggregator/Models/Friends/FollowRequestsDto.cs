using SocialMediaApp.Aggregator.Models.Users;
using System.Security.Principal;

namespace SocialMediaApp.Aggregator.Models.Friends
{
    public class FollowRequestsDto
    {
        public int Id { get; set; }
        public int RequestingUserId { get; set; }
        public UserListDto User { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
