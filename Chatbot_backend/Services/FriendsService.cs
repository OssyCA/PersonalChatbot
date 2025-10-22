using Chatbot_backend.Data;
using Chatbot_backend.DTO;
using Chatbot_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Chatbot_backend.Services
{
    public class FriendsService(MiniJwtDbContext context)
    {
        public async Task<FriendUserDto> FindUser(string name)
        {

            var user = await context.Users.SingleOrDefaultAsync(u => u.UserName == name);

            if (user is null)
            {
                throw new KeyNotFoundException(name);
            }

            var foundFriend = new FriendUserDto();

            foundFriend.Name = user.UserName;

            return foundFriend;
        }
        //public async Task<bool> SendFriendRequest()
        //{

        //}
    }
}
