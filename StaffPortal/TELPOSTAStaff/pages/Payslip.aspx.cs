﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff.pages
{
    public partial class Payslip : System.Web.UI.Page
    {
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader reader;
        SqlDataAdapter adapter;
        string[] strLimiters2 = new string[] { "[]" };
        Staffportall webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }
                LoadYears();
                LoadMonths();
                ViewPayslip();
            }
        }


        private void LoadYears1()
        {
            try
            {
                connection = Components.GetconnToNAV();
                command = new SqlCommand()
                {
                    CommandText = "spGetPayslipYears",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                command.Parameters.AddWithValue("@Company_Name", Components.Company_Name);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    ddlYear.DataSource = reader;
                    ddlYear.DataTextField = "Period Year";
                    ddlYear.DataValueField = "Period Year";
                    ddlYear.DataBind();
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        //GetPayslipYears
        private void LoadYears()
        {
            try
            {
                ddlYear.Items.Clear();

                string payslipYears = webportals.GetPayslipYears();
                if (!string.IsNullOrEmpty(payslipYears))
                {
                    string[] yearsArr = payslipYears.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);

                    // Directly add all unique years to the dropdown
                    foreach (string year in yearsArr.Distinct()) // Remove duplicates if any
                    {
                        ddlYear.Items.Add(new ListItem(year.Trim()));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error instead of clearing it
                Console.WriteLine($"Error in LoadYears: {ex.Message}");
            }
        }

        private void LoadMonths()
        {
            try
            {
                ddlMonth.Items.Clear();
                 string Year = ddlYear.SelectedValue;
                int currentYear = Convert.ToInt32(Year);

                string payslipMonths = webportals.GetPayslipMonths(currentYear);
                if (!string.IsNullOrEmpty(payslipMonths))
                {
                    string[] monthsArr = payslipMonths.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string months in monthsArr)
                    {

                        string[] responseArr = months.Split(strLimiters, StringSplitOptions.None);
                        if (responseArr.Length == 2)
                        {
                            string monthNumber = responseArr[0];
                            string monthName = responseArr[1];


                            ListItem li = new ListItem(monthName, monthNumber);
                            ddlMonth.Items.Add(li);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void ViewPayslip()
        {
            try
            {
                string username = Session["username"].ToString();
                var filename = Session["username"].ToString().Replace(@"/", @"");
                string pdfFileName = String.Format(@"PAYSLIP-{0}.pdf", filename);
                var month = ddlMonth.SelectedValue;

                if (month == "12")
                {
                    month = "12";

                }
                else if (month == "11")
                {
                    month = "11";
                }
                else if (month == "10")
                {
                    month = "10";
                }
                else if (month == "")
                {
                    month = "01";
                }
                else
                {
                    month = "0" + month;
                }
                var myDate = month + "/01/" + ddlYear.SelectedValue;
                var period = DateTime.ParseExact(myDate, "M/dd/yyyy", CultureInfo.InvariantCulture);



                var filePath = Server.MapPath("~/Downloads/") + String.Format("PAYSLIP-{0}.pdf", filename);

                // Check if directory exists, if not create it
                if (!Directory.Exists(Server.MapPath("~/Downloads/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Downloads/"));
                }
               webportals.GeneratePaySlipReport1(username, period, String.Format(@"PAYSLIP-{0}.pdf", filename));

                
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine("Payslip generated successfully.");
                    myPDF.Attributes.Add("src", ResolveUrl("~/Downloads/" + String.Format("PAYSLIP-{0}.pdf", filename)));
                }
                else
                {
                    throw new FileNotFoundException("Payslip PDF was not found after generation.");
                }


            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadMonths();
                ViewPayslip();
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ViewPayslip();
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        private void Message(string message)
        {
            string strScript = "<script>alert('" + message + "');</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }
    }
}