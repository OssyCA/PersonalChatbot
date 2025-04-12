using JwtMinimalAPI.DTO;
using JwtMinimalAPI.Models;
using System.ComponentModel;

namespace JwtMinimalAPI.Services
{
    public interface IAdminService
    {
        Task<List<ShowUserDto>> GetUsers();
    }
}
