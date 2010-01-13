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
    public partial class QueryBuilder : Form
    {
        public QueryBuilder()
        {
            InitializeComponent();
        }
        frmMain myparent;
        public QueryBuilder(frmMain parent)
        {
            InitializeComponent();
            myparent = parent;
        }
        private void btnUpdateSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string ssrch = "";

                if (txtTicketId.Text.Length > 0)
                {
                    ssrch += "TicketNumber eq '" + txtTicketId.Text + "'";
                }
                if (ssrch.Length > 0)
                {
                    myparent.txtSearch.Text = "?where=" + ssrch;
                }
                else
                {
                    myparent.txtSearch.Text = "";
                }
            }
            finally { this.Close(); }


        }
    }
}
