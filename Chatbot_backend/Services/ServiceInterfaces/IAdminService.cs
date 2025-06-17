using Chatbot_backend.DTO;
using Chatbot_backend.Models;
using System.ComponentModel;

namespace Chatbot_backend.Services.ServiceInterfaces
{
    public interface IAdminService
    {
        Task<List<ShowUserDto>> GetUsers();
    }
}
