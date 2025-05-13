using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TELPOSTAStaff.pages
{
    public partial class LeaveStatement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }

                GenerateLeaveStatement();
            }
        }
      
        protected void GenerateLeaveStatement()
        {
            string username = Session["username"].ToString();
            string filename = username.ToString().Replace(@"/", @"");
            try
            {
                try
                {
                    
                    string returnstring = "";
                    Components.ObjNav.GenerateLeaveStatement(Session["username"].ToString(), String.Format("LvSttmnts{0}.pdf", filename), ref returnstring);
                    
                    myPDF.Attributes.Add("src", ResolveUrl("~/Downloads/" + String.Format("LvSttmnts{0}.pdf", filename)));

                    byte[] bytes = Convert.FromBase64String(returnstring);
                    string path = HostingEnvironment.MapPath("~/Downloads/" + $"LvSttmnts{filename}.pdf");
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    FileStream stream = new FileStream(path, FileMode.CreateNew);
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(bytes, 0, bytes.Length);
                    writer.Close();
                    myPDF.Attributes.Add("src", ResolveUrl("~/Downloads/" + String.Format("LvSttmnts{0}.pdf", filename)));
                }
                catch (Exception exception)
                {
                    exception.Data.Clear();
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

    }
}