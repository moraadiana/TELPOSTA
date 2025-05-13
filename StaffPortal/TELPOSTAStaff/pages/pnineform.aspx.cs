using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff.pages
{
    public partial class pnineform : System.Web.UI.Page
    {
        readonly Staffportall webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        string[] strLimiters2 = new string[] { "[]" };

        protected void Page_Load(object sender, EventArgs e)
          {
              if (!IsPostBack)
              {
                  if (Session["username"] == null)
                  {
                      Response.Redirect("~/Default.aspx");
                  }
                  LoadYears();
                  //LoadP9();
              }
          }
        private void LoadYears()
        {
            try
            {
                ddlYear.Items.Clear();
                ddlYear.Items.Add(" -- Select Year --");


                string payslipYears = webportals.GetPayslipYears();
                if (!string.IsNullOrEmpty(payslipYears))
                {
                    string[] yearsArr = payslipYears.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string year in yearsArr.Distinct())
                    {
                        ddlYear.Items.Add(new ListItem(year.Trim()));
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadYears: {ex.Message}");
            }
        }

        protected void LoadP9()
          {
              try
              {
                  var filename = Session["username"].ToString().Replace(@"/", @"");
                  var employee = Session["username"].ToString();

                if (!int.TryParse(ddlYear.SelectedValue, out int period))
                {
                    Console.WriteLine("Please select a valid Year.");
                    return;
                }
                try
                  {
                      string returnstring = "";
                      Components.ObjNav.Generatep9Report(period, employee, String.Format("p9Form{0}.pdf", filename), ref returnstring);
                      myPDF.Attributes.Add("src", ResolveUrl("~/Downloads/" + String.Format("p9Form{0}.pdf", filename)));
                      
                      byte[] bytes = Convert.FromBase64String(returnstring);
                      string path = HostingEnvironment.MapPath("~/Downloads/" + $"p9Form{filename}.pdf");
                      if (System.IO.File.Exists(path))
                      {
                          System.IO.File.Delete(path);
                      }
                      FileStream stream = new FileStream(path, FileMode.CreateNew);
                      BinaryWriter writer = new BinaryWriter(stream);
                      writer.Write(bytes, 0, bytes.Length);
                      writer.Close();
                      myPDF.Attributes.Add("src", ResolveUrl("~/Downloads/" + String.Format("p9Form{0}.pdf", filename)));
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
          protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
          {
              LoadP9();
          }
    }
}