using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SupermarketManagementSystem.Helpers;

namespace SupermarketManagementSystem.Forms
{
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblStatus;

        public LoginForm()
        {
            Text = "Supermarket Management System - Login";
            Size = new Size(380, 300);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var lblTitle = new Label
            {
                Text = "Supermarket Management System",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 20),
                Size = new Size(340, 40)
            };

            var lblUser = new Label { Text = "Username:", Location = new Point(40, 90), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(140, 87), Size = new Size(180, 24), Text = "admin" };

            var lblPass = new Label { Text = "Password:", Location = new Point(40, 130), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(140, 127), Size = new Size(180, 24), UseSystemPasswordChar = true };

            btnLogin = new Button { Text = "Login", Location = new Point(140, 170), Size = new Size(100, 32) };
            btnLogin.Click += BtnLogin_Click;

            lblStatus = new Label
            {
                ForeColor = Color.Red,
                Location = new Point(20, 215),
                Size = new Size(320, 40),
                TextAlign = ContentAlignment.TopCenter
            };

            Controls.Add(lblTitle);
            Controls.Add(lblUser);
            Controls.Add(txtUsername);
            Controls.Add(lblPass);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(lblStatus);

            AcceptButton = btnLogin;
        }

        // ថ្មី: function សម្រាប់ hash password ដោយប្រើ SHA256
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblStatus.Text = "Please enter username and password.";
                return;
            }

            try
            {
                // ថ្មី: hash password មុននឹងយកទៅប្រៀបធៀបជាមួយ database
                string hashedPass = HashPassword(pass);

                var table = DBHelper.ExecuteQuery(
                    "SELECT UserID, FullName, Role FROM Users WHERE Username=@u AND PasswordHash=@p AND IsActive=1",
                    new SqlParameter("@u", user),
                    new SqlParameter("@p", hashedPass));

                if (table.Rows.Count == 0)
                {
                    lblStatus.Text = "Invalid username or password.";
                    return;
                }

                var row = table.Rows[0];
                CurrentSession.UserID = Convert.ToInt32(row["UserID"]);
                CurrentSession.FullName = row["FullName"].ToString();
                CurrentSession.Role = row["Role"].ToString();

                Hide();
                var main = new MainForm();
                main.FormClosed += (s2, e2) => Close();
                main.Show();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Database connection failed:\n" + ex.Message;
            }
        }
    }

    /// <summary>Holds the logged-in user for the rest of the app.</summary>
    public static class CurrentSession
    {
        public static int UserID { get; set; }
        public static string FullName { get; set; } = "";
        public static string Role { get; set; } = "";
    }
}