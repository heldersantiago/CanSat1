using CanSat1.interfaces;
using CanSat1.services;
using CanSat1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanSat1.controllers
{
    public class LoginController
    {
        private readonly IAuthService _authService;
        public LoginController(IAuthService authService) {
            _authService = authService;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email é um campo obrigatório");
            }

            if (!_Utils.IsValidEmail(email))
            {
                throw new ArgumentException("Email inválido");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password é um campo obrigatório");
            }

            return await _authService.LoginAsync(email, password);
        }

        public async void logout()
        {
            await _authService.DestroySessionAsync();
        }
    }
}
