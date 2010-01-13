using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SdataTickets
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        
        frmMain myparent;

        public Login(frmMain parent)
        {
            InitializeComponent();
            myparent = parent;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            myparent.User = txtUserName.Text;
            myparent.Password = txtPassword.Text;
            myparent.UpdateStatus();
            this.Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            txtUserName.Text = myparent.User;
            txtPassword.Text = myparent.Password;
        }
    }
}
