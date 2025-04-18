using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Models;
using System.ComponentModel;

namespace JwtMinimalAPI.Services.ServiceInterfaces
{
    public interface IAdminService
    {
        Task<List<ShowUserDto>> GetUsers();
    }
}
