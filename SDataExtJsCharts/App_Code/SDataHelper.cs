using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Net;
using System.IO;

/// <summary>
/// Summary description for SDataHelper
/// </summary>
public class SDataHelper
{
	public SDataHelper()
	{
		//
		// TODO: Add constructor logic here
		//
	}
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
    
    public System.Xml.XmlDocument _xmlCurrent = new XmlDocument();

    public void DownloadXML(string sUrl)
    {
        System.Net.WebClient client = new System.Net.WebClient();
        client.Encoding = System.Text.Encoding.UTF8;
        client.Headers.Add(System.Net.HttpRequestHeader.ContentType, "application/atom+xml");
        client.Credentials = new System.Net.NetworkCredential(_User, _Password);

        string result = client.DownloadString(sUrl);
        client.Dispose();
        client = null;
        _xmlCurrent.LoadXml(result.Substring(1));

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

    private string GetIDfromLink(string p)
    {
        return p.Substring(p.IndexOf("'") + 1, p.IndexOf("'", p.IndexOf("'") + 1) - p.IndexOf("'") - 1);
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
        return xnode.InnerText;
    }

    private string GetSingleAttributeFromxmldoc(XmlDocument xdoc, string sNodeName, string sattr)
    {
        var manager = new XmlNamespaceManager(xdoc.NameTable);
        XmlNamespaceManagerAddNamespacesforslx(ref manager);

        XmlNode xnode = xdoc.SelectSingleNode(sNodeName, manager);
        string retval = "";
        foreach (XmlAttribute xmlattr in xnode.Attributes)
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
        return GetSingleValueFromxmldoc(xdoc, sNodeName);
    }

    private string GetEntryFromFeed(string xinput, string entry)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xinput);

        System.Xml.XPath.XPathNavigator xmlNav = doc.DocumentElement.CreateNavigator();
        var manager = new XmlNamespaceManager(doc.NameTable);
        XmlNamespaceManagerAddNamespacesforslx(ref manager);
        //get the first entry node and put the feed node back on (now you have a feed entry)

        System.Xml.XPath.XPathNodeIterator nav = xmlNav.Select(entry, manager);
        nav.MoveNext();
        return nav.Current.InnerXml;

        nav = null;
        manager = null;
        doc = null;



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
        request = (System.Net.HttpWebRequest)WebRequest.Create(uri);
        request.Method = "PUT";
        request.ContentType = "application/atom+xml;type=entry";
        request.Headers.Add("if-match", GetSingleValueFromxmldoc(_xmlCurrent, "//http:etag"));

        System.Text.UTF8Encoding encoding = new UTF8Encoding();
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
            using (System.IO.Stream responseStream = response.GetResponseStream())
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
           // MessageBox.Show(response.StatusCode.ToString());
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
            //MessageBox.Show(response.StatusCode.ToString());
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
