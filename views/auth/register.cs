using CanSat1.controllers;
using CanSat1.services;
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
    public partial class Register : Form
    {
        private readonly RegisterController registerController;
        public Register()
        {
            InitializeComponent();

            var  databaseService = new DatabaseService();
            var authService = new AuthService(databaseService);
            registerController = new(authService);
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string name = tbRegisterName.Text.Trim();
            string email = tbRegisterEmail.Text.Trim();
            string password1 = tbRegisterPassword1.Text.Trim();
            string password2 = tbRegisterPassword2.Text.Trim();

            try
            {
                if (await registerController.RegisterAsync(name,email, password1, password2))
                {
                    main _main = new main();
                    this.Hide();
                    _main.Show();
                }
                else
                {
                    MessageDialog.Show("Já existe um usuário com este email", MessageDialogStyle.Dark);
                }
            }
            catch (ArgumentException ex)
            {
                MessageDialog.Show(ex.Message, MessageDialogStyle.Dark);
            }
        }

        private void btnGotoLogin_Click(object sender, EventArgs e)
        {
            Login _login = new();
            this.Hide();
            _login.Show();
        }

    }
}
