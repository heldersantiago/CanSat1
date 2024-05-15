using CanSat1.services;
using System;
using CanSat1.models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CanSat1.interfaces;
using CanSat1.Utils;

namespace CanSat1.controllers
{
    public class RegisterController
    {
        private readonly IAuthService _authService;
        public RegisterController(IAuthService authService) {
            _authService = authService;
        }
        public async Task<bool> RegisterAsync(string name, string email, string password, string password2) {

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Nome é um campo obrigatório");
            }

            if (!password.Equals(password2))
            {
                throw new ArgumentException("As senhas são diferentes");
            }

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

            return await _authService.RegisterAsync(new User(name,email,password));
        }
    }
}
