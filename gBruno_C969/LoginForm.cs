using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace gBruno_C969
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            SetLanguage();
        }


        // ============== test SPANISH ================
        private void SetLanguage() {
            string cultureCode = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if (cultureCode.StartsWith("es")) // SPANISSH
            {
                lblUsername.Text = "Nombre de usuario";
                lblPassword.Text = "Contrasena";   // Password
                btnLogin.Text = "Iniciar sesion";   // initiate session?
                btnCancel.Text = "Cancelar";        
            }
            else 
            { 
                lblUsername.Text= "Username";
                lblPassword.Text= "Password";
                btnLogin.Text = "Login";
                btnCancel.Text = "Cancel";
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(GetLocalizedString("Please enter both username and password."));
                return;
            }

                // user authentication - MySQL 
            if(ValidateUser(username, password))
            {
                //MessageBox.Show("Log in successful!");

                // LOG success
                LoginAttempt(username, true);

                this.Hide();
                MainForm mainForm = new MainForm();
                mainForm.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show(GetLocalizedString("Invalid username or passsword."));

                // LOG failed
                LoginAttempt(username, false);
            }
        }

        private string GetLocalizedString(string englishText)
        {
            //throw new NotImplementedException();

            string cultureCode = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            if (cultureCode == "es")
            {
                if (englishText == "Please  enter both username and password.") 
                
                    return "Por favor, ingrese su nombre de usuario y contrasena. ";
                if (englishText == "Invalid username or password.")
                    return "El nombre de usuarioa o la contrasena son incorrectos.";
                
                
            }
            return englishText;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private bool ValidateUser(string username, string password) 
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["client_schedule"].ToString();
            using (MySqlConnection conn = new MySqlConnection(connectionString)) 
            { 
                
                conn.Open();
                string query = "SELECT * FROM user WHERE userName = @username AND password = @password";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                MySqlDataReader reader = cmd.ExecuteReader();
                return reader.HasRows;
                //reader.Close();
            }
        }

        // LOG LOGIN Rubric
        private void LoginAttempt(string username, bool success)
        {
            string logPath = "Login_History.txt";
            string status = success ? "Success" : "Failed"; // ternary action
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";

            string logLine = $"{timestamp} - {status} login attempt for user '{username}'";

            try
            {
                System.IO.File.AppendAllText(logPath, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing to log file: {ex.Message}");
            }
        }





















    }
}
