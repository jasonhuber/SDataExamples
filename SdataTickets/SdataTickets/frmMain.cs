using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Net;
using System.IO;

namespace SdataTickets
{
    public partial class frmMain : Form
    {
        XmlDocument _xmlCurrent = new XmlDocument();
        
        #region "Username and Password" 
        //username and password can be reset using the dilogues
        string _User = "Lee";

        public string User
        {
            get { return _User; }
            set { _User = value; }
        }
        string _Password = "";

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        #endregion 
        
        #region "toolstrip Updates" "This is where I keep the toolstrip updated and such"
        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            Login lgn = new Login(this);
            lgn.ShowDialog();
        }
        public void UpdateStatus()
        {
            toolStripStatusLabel1.Text = "Logged in as " + _User + "(click to change)";
            toolStripStatusLabel1.BackColor = Color.LightGray;
        }
        public void UpdateStatus(string status)
        {
            toolStripStatusLabel1.Text = status;
        }
        public void UpdateStatus(string status, System.Drawing.Color scolor)
        {
            toolStripStatusLabel1.Text = status;
            toolStripStatusLabel1.BackColor = scolor;
        }

        private void linkLabel1_MouseHover(object sender, EventArgs e)
        {
            UpdateStatus("Click to build your search!");
        }

        private void linkLabel1_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus();
        }
        #endregion "toolstrip Updates"

        #region "Other UI Stuff"
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            UpdateStatus();
        }

 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            QueryBuilder lgn = new QueryBuilder(this);
            lgn.ShowDialog();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }
        #endregion "other Ui stuff"

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            string results = GetTickets(txtSearch.Text);
            XmlDocument xmlTicket = new XmlDocument();
            if (int.Parse(GetSingleValueFromxmldoc(results, "//opensearch:totalResults")) > 1)
            {
                UpdateStatus("Your search returned more than one row!, using only the first result", Color.Red);
                timer1.Interval= 3000;
                timer1.Start();
            }
            xmlTicket.LoadXml("<entry>" + GetEntryFromFeed(results, "//atom:entry") + "</entry>");

            LoadUIFromResult(xmlTicket);
            //save the xml for when I decide to update or delete.
            _xmlCurrent = xmlTicket;
        }

        private void LoadUIFromResult(XmlDocument xmldoc)
        {
            txtTicketId.Text = GetSingleValueFromxmldoc(xmldoc, "//slx:TicketNumber");
            txtTicketSubject.Text = GetSingleValueFromxmldoc(xmldoc, "//slx:Subject"); 
            GetContactInformationAndLoadUI(xmldoc); //todo: do this.
        }

        private void UpdateSingleValue(string nodename, string nodevalue, ref XmlDocument doc)
        {
            var xmlNav = doc.DocumentElement.CreateNavigator();
            var manager = new XmlNamespaceManager(doc.NameTable);
            XmlNamespaceManagerAddNamespacesforslx(ref manager);

            var nav = xmlNav.SelectSingleNode(nodename, manager);
            if (nav.Value != nodevalue)
            {
                var nil = nav.SelectSingleNode("@xsi:nil", manager);
                if (nodevalue == null)
                {
                    if (nil == null)
                    {
                        var attributes = nav.CreateAttributes();
                        attributes.WriteAttributeString("xsi:nil", "true");
                        attributes.Close();
                    }
                }
                else
                {
                    if (nil != null)
                    {
                        nil.DeleteSelf();
                    }
                }

                nav.SetValue(nodevalue);
            }
            else
            {
                nav.DeleteSelf();
            }
        }
 
        private void UpdateSingleAttribute(string nodename, string attrvalue, ref XmlDocument doc, string attrname)
        {
            var xmlNav = doc.DocumentElement.CreateNavigator();
            var manager = new XmlNamespaceManager(doc.NameTable);
            XmlNamespaceManagerAddNamespacesforslx(ref manager);

            var nav = xmlNav.SelectSingleNode(nodename, manager);
            var nil = nav.SelectSingleNode("@" + attrname, manager);

            if (nil == null)
            {
                var attributes = nav.CreateAttributes();
                attributes.WriteAttributeString(attrname, attrvalue);
                attributes.Close();
            }
            else 
            {
                nil.SetValue(attrvalue);
            }
        }
        
        private void GetContactInformationAndLoadUI(XmlDocument TicketsxmlDoc)
        {
            XmlDocument xmlcontact = GetContactFromLink(GetSingleAttributeFromxmldoc(TicketsxmlDoc, "//atom:link[@title='Contact']", "href"));
            txtContactName.Text = GetSingleValueFromxmldoc(xmlcontact,"//slx:Name");
            txtContactPhone.Text = GetSingleValueFromxmldoc(xmlcontact, "//slx:WorkPhone");
            txtContactEmail.Text = GetSingleValueFromxmldoc(xmlcontact, "//slx:Email");
            XmlDocument xmlContact = new XmlDocument();
            xmlcontact.LoadXml("<entry>" + GetEntryFromFeed(DownloadXML(GetSingleAttributeFromxmldoc(TicketsxmlDoc, "//atom:link[@title='Contact']", "href")),"//atom:entry")+ "</entry>");

            txtContactId.Text = GetIDfromLink(GetSingleAttributeFromxmldoc(xmlcontact, "//atom:link[@title='Self']", "href"));

        }

        private string GetIDfromLink(string p)
        {
            return p.Substring(p.IndexOf("'")+1, p.IndexOf("'", p.IndexOf("'") + 1) - p.IndexOf("'")-1);
        }

        private XmlDocument GetContactFromLink(string Contacturl)
        { 
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(DownloadXML(Contacturl));
            return doc;
        }
        
        private void XmlNamespaceManagerAddNamespacesforslx(ref XmlNamespaceManager xnsm)
        {
            xnsm.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xnsm.AddNamespace("slx", "http://schemas.sage.com/dynamic/2007");
            xnsm.AddNamespace("sdata", "http://schemas.sage.com/sdata/2008/1");
            xnsm.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            xnsm.AddNamespace("opensearch", "http://a9.com/-/spec/opensearch/1.1/");
            xnsm.AddNamespace("http", "http://schemas.sage.com/sdata/http/2008/1");
        }

        private string GetSingleValueFromxmldoc(XmlDocument xdoc, string sNodeName)
        {
            var manager = new XmlNamespaceManager(xdoc.NameTable);
            XmlNamespaceManagerAddNamespacesforslx(ref manager);

            XmlNode xnode = xdoc.SelectSingleNode(sNodeName, manager);
            return  xnode.InnerText;
        }

        private string GetSingleAttributeFromxmldoc(XmlDocument xdoc, string sNodeName, string sattr)
        {
            var manager = new XmlNamespaceManager(xdoc.NameTable);
            XmlNamespaceManagerAddNamespacesforslx(ref manager);

            XmlNode xnode = xdoc.SelectSingleNode(sNodeName, manager);
            string retval = "";
            foreach(XmlAttribute xmlattr in xnode.Attributes)
            {
                if (xmlattr.Name == sattr)
                {
                    retval = xmlattr.Value;
                    return retval;
                }
            }
            return retval;
            
            
            }

        private string GetSingleValueFromxmldoc(string sxml, string sNodeName)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(sxml);
            return GetSingleValueFromxmldoc(xdoc,sNodeName);
        }

        private string GetEntryFromFeed(string xinput, string entry)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xinput);

            System.Xml.XPath.XPathNavigator xmlNav = doc.DocumentElement.CreateNavigator();
            var manager = new XmlNamespaceManager(doc.NameTable);
            XmlNamespaceManagerAddNamespacesforslx(ref manager);
            //get the first entry node and put the feed node back on (now you have a feed entry)
            
            XPathNodeIterator nav = xmlNav.Select(entry, manager);
            nav.MoveNext();
            return nav.Current.InnerXml;

            nav = null;
            manager = null;
            doc = null;

            
            
        }

        private string GetTickets(string where)
        {
            return DownloadXML("http://localhost:3333/sdata/slx/dynamic/-/Tickets" + where);
        }

        private string DownloadXML(string sUrl)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/atom+xml");
            client.Credentials = new System.Net.NetworkCredential(_User, _Password);

            string result = client.DownloadString(sUrl);
            client.Dispose();
            client = null;
            return result.Substring(1);
            
        }

        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UpdateSingleValue("//slx:Subject", txtTicketSubject.Text, ref _xmlCurrent);
            SendUpdatedEntryViaPUT(_xmlCurrent, GetSingleAttributeFromxmldoc(_xmlCurrent, "//atom:link[@title='Edit']", "href"));
        }

        private bool SendUpdatedEntryViaPUT(XmlDocument _xmlCurrent, string url)
        {
            //CredentialCache cache = new CredentialCache();
            //cache.Add(new Uri(url), "Digest", new NetworkCredential(_User, _Password));
            //cache.Add(new Uri(url), "Basic", new NetworkCredential(_User, _Password));

            //// set up connection
            //WebClient _client;
            //_client = new WebClient();
            //_client.Encoding = Encoding.UTF8;
            //_client.Credentials = cache;
            ////_client.Headers.Add("if-match", GetSingleValueFromxmldoc(_xmlCurrent, "//http:etag"));
            //_client.Headers.Add(HttpRequestHeader.ContentType, "application/atom+xml;type=entry");
            //_client.Headers.Add(HttpRequestHeader.IfMatch, GetSingleValueFromxmldoc(_xmlCurrent, "//http:etag"));
            //string result = _client.UploadString(url, "PUT", _xmlCurrent.CreateNavigator().OuterXml);
            Uri uri = new Uri(url);
            System.Net.HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "PUT";
            request.ContentType = "application/atom+xml;type=entry";
            request.Headers.Add("if-match", GetSingleValueFromxmldoc(_xmlCurrent, "//http:etag"));

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(_xmlCurrent.OuterXml);
            request.ContentLength = postBytes.Length;

            System.IO.Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(_User, _Password);
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                        //MessageBox.Show(result);
                    }
                }
            }
            return true;
        }

        private bool SendDELETEEntryViaDELETE(XmlDocument _xmlCurrent, string url)
        {
            Uri uri = new Uri(url);
            System.Net.HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "DELETE";
            request.ContentType = "application/atom+xml;type=entry";
            request.Headers.Add("if-match", GetSingleValueFromxmldoc(_xmlCurrent, "//http:etag"));

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(_xmlCurrent.InnerXml);
            request.ContentLength = postBytes.Length;

            System.IO.Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(_User, _Password);
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                MessageBox.Show(response.StatusCode.ToString());
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                        //MessageBox.Show(result);
                    }
                }
            }
            return true;
        }
    
        //thanks to : http://www.vbforums.com/showthread.php?t=287324 for the following snippet
        public string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

        private void lnkDelete_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SendDELETEEntryViaDELETE(_xmlCurrent, GetSingleAttributeFromxmldoc(_xmlCurrent, "//atom:link[@title='Edit']", "href"));

        }

        private void lnkInsert_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Get template
            XmlDocument xmltemplate = new XmlDocument();
 
            xmltemplate.LoadXml("<entry>"+ GetEntryFromFeed(DownloadXML("http://localhost:3333/sdata/slx/dynamic/-/Tickets/$template?include=contact,owner,account"),"//entry") + "</entry>");

            //update template
            UpdateSingleValue("//slx:Subject", txtTicketSubject.Text, ref xmltemplate);
            UpdateSingleAttribute("//slx:Contact", txtContactId.Text, ref xmltemplate, "sdata:key");
            XmlDocument xmlAccount = new XmlDocument();
            xmlAccount.LoadXml("<entry>" + GetEntryFromFeed(DownloadXML("http://localhost:3333/sdata/slx/dynamic/-/Contacts('" + txtContactId.Text + "')/Account"), "//atom:entry") + "</entry>");

            UpdateSingleAttribute("//slx:Account", GetIDfromLink(GetSingleAttributeFromxmldoc(xmlAccount, "//atom:link[@title='Self']", "href")), ref xmltemplate, "sdata:key");

            //post template
            SendPOSTEntryViaPOST(xmltemplate, "http://localhost:3333/sdata/slx/dynamic/-/Tickets");
        }

        private bool SendPOSTEntryViaPOST(XmlDocument _xmlCurrent, string url)
        {
            Uri uri = new Uri(url);
            System.Net.HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/atom+xml;type=entry";

            //request.Headers.Add("if-match", GetSingleValueFromxmldoc(_xmlCurrent, "//http:etag"));

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] postBytes = encoding.GetBytes(_xmlCurrent.InnerXml);
            request.ContentLength = postBytes.Length;

            System.IO.Stream postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(_User, _Password);
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                MessageBox.Show(response.StatusCode.ToString());
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                        //MessageBox.Show(result);
                    }
                }
            }
            return true;
        }

       
    }
}
