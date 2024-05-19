using Guna.UI2.WinForms;
using System.Text.RegularExpressions;

namespace CanSat1.Utils
{
    public static class _Utils
    {
        
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false; // Empty or null string is not a valid email
            }

            try
            {
                // Using a regular expression to validate the email format
                var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)\.([a-zA-Z]{2,6})$");
                return regex.IsMatch(email);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static Task<bool> ValidatePasswordAsync(string enteredPassword, string storedPassword)
        {
            // Verify if the given password is Equal to stored password
            return Task.FromResult(storedPassword.Equals(enteredPassword.ToString()));
        }

        public static void updateSensorData(Guna2CircleProgressBar gunaCircle, string value,bool isTemperature = true)
        {
            int _value = int.Parse(value);
            gunaCircle.Value = _value;
            if (isTemperature)
            {
                gunaCircle.Text = $"{_value} ºC";
                gunaCircle.ProgressColor2 = _value >= 38 ? Color.DarkRed : Color.DarkGreen;
            }
            else{
                gunaCircle.ProgressColor2 = _value >= 50 ? Color.YellowGreen : Color.DarkGreen;
            }
        }


        public static void UpdateUIOnConnection(Guna2Button btn, bool isConnected, Guna2TabControl tabControl)
        {
            if (isConnected)
            {
                btn.Text = "Fechar Porta";
                btn.FillColor = Color.DarkRed;
                tabControl.SelectedIndex = 1;
            }
            else
            {
                btn.Text = "Abrir Porta";
                btn.FillColor = Color.DarkBlue;
                tabControl.SelectedIndex = 0;
            }
        }

        public static void ShowConnectionErrorMessage(string message)
        {
            MessageDialog.Show(message, MessageDialogStyle.Dark);
        }

        public static void HandleBtnOnLoading(Guna2Button btn, string text, bool enable)
        {
            btn.Text = text;
            btn.Enabled = enable;
        }
        public static void HandleBtnOnLoading(Guna2GradientButton btn, string text, bool enable)
        {
            btn.Text = text;
            btn.Enabled = enable;
        }

        public static void UpdateSinalizerPicture(Guna2CirclePictureBox pictureBox, bool detected) { 
            pictureBox.Image = detected ? CanSat1.Properties.Resources.led_off : CanSat1.Properties.Resources.led_off_black;
        }
    }
}