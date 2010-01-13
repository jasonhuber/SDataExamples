using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //Load a data store with data
        //Loop through that store creating JS string
        //Register that string like this:

        /*
         var store = new Ext.data.JsonStore({
        fields:['name', 'visits'],
        data: [
            {name:'Jul 07', visits: 245000},
            {name:'Aug 07', visits: 240000},
            {name:'Sep 07', visits: 355000},
            {name:'Oct 07', visits: 375000},
            {name:'Nov 07', visits: 490000},
            {name:'Dec 07', visits: 495000},
            {name:'Jan 08', visits: 520000},
            {name:'Feb 08', visits: 620000}
        ]
    });
         */

        System.Text.StringBuilder js = new System.Text.StringBuilder();


        SDataHelper sdh = new SDataHelper();
        sdh.Password = "";
        sdh.User = "Lee";
        sdh.DownloadXML("http://localhost:3333/sdata/slx/dynamic/-/Products");

        DataSet myDS = new DataSet();

        myDS.ReadXml(new XmlNodeReader(sdh._xmlCurrent)); // I really shouldn't be doing this.
        int i;
        js.Append("var store = new Ext.data.JsonStore({");
        js.Append("fields:['name', 'price'],");
        js.Append("data: [");

        for (i = 0; i < myDS.Tables[8].Rows.Count - 1; i++)
        {
            js.Append("{name:'" + myDS.Tables[8].Rows[i].ItemArray[1].ToString() + 
                "', price: " + myDS.Tables[8].Rows[i].ItemArray[12].ToString() + "}");
            if (i+1<myDS.Tables[8].Rows.Count - 1)
            {
                //need to add a comma, but not on the last row.
                js.Append(",");
            }
        }
//template:        js.Append("{name:'Jul 07', price: 245000},");
        js.Append("]");
        js.Append("});");

        ClientScript.RegisterClientScriptBlock(this.GetType(),"Chart JS",js.ToString(),true);
        
    }
}
