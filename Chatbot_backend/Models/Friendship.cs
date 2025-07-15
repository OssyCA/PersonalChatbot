namespace Chatbot_backend.Models
{
    public class Friendship
    {
        public Guid UserAId { get; set; }
        public User UserA { get; set; }

        public Guid UserBId { get; set; }
        public User UserB { get; set; }

        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }


    
}
