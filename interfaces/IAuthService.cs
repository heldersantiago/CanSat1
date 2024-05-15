using CanSat1.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanSat1.interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(User user);
        Task DestroySessionAsync();
        Task<User> GetUserFromSessionAsync();
    }
}
