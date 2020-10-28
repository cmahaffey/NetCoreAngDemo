namespace API.Entities
{
    public class UserLike
    {
        public AppUser LikingUser { get; set; }
        public AppUser LikedUser { get; set; }
        public int LikingUserId { get; set; }
        public int LikedUserId { get; set; }
    }
}