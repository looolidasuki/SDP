using System;
using System.Drawing;
using System.Windows.Forms;
using Sales_user.Controllers;
using Sales_user.Models;

namespace FurnitureERP
{
    public class LoginForm : Form
    {
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblError;
        public Staff LoggedInStaff { get; private set; }

        public LoginForm()
        {
            InitUI();
        }

        private void InitUI()
        {
            Text = "Premium Living Furniture ERP – Login";
            Size = new Size(480, 380);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 247, 250);

            Panel header = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.FromArgb(8, 35, 74) };
            Label title = new Label
            {
                Text = "PREMIUM LIVING FURNITURE",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(title);
            Controls.Add(header);

            Panel body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(50, 20, 50, 20) };

            Label lblEmail = new Label { Text = "Email Address", Location = new Point(50, 30), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtEmail = new TextBox { Location = new Point(50, 52), Width = 340, Height = 28, Font = new Font("Segoe UI", 10) };

            Label lblPass = new Label { Text = "Password", Location = new Point(50, 95), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtPassword = new TextBox { Location = new Point(50, 117), Width = 340, Height = 28, Font = new Font("Segoe UI", 10), PasswordChar = '●' };

            btnLogin = new Button
            {
                Text = "Sign In",
                Location = new Point(50, 165),
                Width = 340,
                Height = 40,
                BackColor = Color.FromArgb(8, 35, 74),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            lblError = new Label { Location = new Point(50, 215), Width = 340, ForeColor = Color.Red, AutoSize = false, Height = 24, Font = new Font("Segoe UI", 9) };

            body.Controls.Add(lblEmail);
            body.Controls.Add(txtEmail);
            body.Controls.Add(lblPass);
            body.Controls.Add(txtPassword);
            body.Controls.Add(btnLogin);
            body.Controls.Add(lblError);
            Controls.Add(body);

            AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Please enter email and password.";
                return;
            }
            try
            {
                var controller = new StaffController();
                Staff staff = controller.Login(email, password);
                if (staff != null)
                {
                    LoggedInStaff = staff;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    lblError.Text = "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Connection error: " + ex.Message;
            }
        }
    }
}
