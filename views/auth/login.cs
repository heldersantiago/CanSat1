using CanSat1.services;
using CanSat1.controllers;
using CanSat1.Utils;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanSat1.views.auth
{
    public partial class Login : Form
    {
        private LoginController _loginController;
        public Login()
        {
            InitializeComponent();
            var databaseService = new DatabaseService();
            var authService = new AuthService(databaseService);
            _loginController = new(authService);

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string email = tbLoginEmail.Text.Trim();
            string password = tbLoginPassword.Text.Trim();

            try
            {
                if (await _loginController.LoginAsync(email, password))
                {
                    main _main = new main();
                    this.Hide();
                    _main.Show();
                }
                else
                {
                    MessageDialog.Show("Password ou email incorrecto", MessageDialogStyle.Dark);
                }
            }
            catch (ArgumentException ex)
            {
                MessageDialog.Show(ex.Message, MessageDialogStyle.Dark);
            }
        }

        private void btnGotoRegister_Click(object sender, EventArgs e)
        {
            Register _register = new();
            this.Hide();
            _register.Show();
        }
    }
}
