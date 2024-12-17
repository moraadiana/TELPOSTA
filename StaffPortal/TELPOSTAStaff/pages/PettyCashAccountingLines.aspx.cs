﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TELPOSTAStaff.pages
{
    public partial class PettyCashAccountingLines : System.Web.UI.Page
    {
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader reader;
        SqlDataAdapter adapter;
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

                LoadResponsibilityCenter();
                string query = Request.QueryString["query"].ToString();
                string documentNo = string.Empty;
                if (query == "new")
                {
                    documentNo = webportals.GetNextPettyCashNo();
                    LoadPostedPettyCash();
                }
                else
                {
                    documentNo = Request.QueryString["SurrenderNo"].ToString();
                    string pettyCashNo = Request.QueryString["PettyCashNo"];
                    string response = webportals.GetPettyCashDetails(pettyCashNo);
                    if (!string.IsNullOrEmpty(response))
                    {
                        ddlPostedPettyCash.Items.Clear();
                        string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                        string pettyCashNumber = responseArr[0];
                        string pettyCashDescription = responseArr[1];
                        string responsibilityCenter = responseArr[2];
                        ListItem li = new ListItem(pettyCashNumber + " => " + pettyCashDescription, pettyCashNumber);
                        ddlPostedPettyCash.Items.Add(li);
                        ddlResponsibilityCenter.SelectedValue = responsibilityCenter;
                    }
                }

                Session["DocumentNo"] = documentNo;
                BindAttachedDocuments(documentNo);
                BindGridViewData();
            }
            string approvalStatus = Request.QueryString["status"].Replace("%", " ");
            if (approvalStatus == "Open" || approvalStatus == "Pending")
            {
                lbtnSubmit.Visible = true;

            }
            else if (approvalStatus == "Pending Approval")
            {

                lbtnSubmit.Visible = false;
            }
        }

        private void LoadPostedPettyCash()
        {
            try
            {
                string username = Session["username"].ToString();
                connection = Components.GetconnToNAV();
                command = new SqlCommand()
                {
                    CommandText = "spPettyCashToSurrender",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                command.Parameters.AddWithValue("@Company_Name", Components.Company_Name);
                command.Parameters.AddWithValue("@userID", "'" + username + "'");
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListItem li = new ListItem(reader["No_"].ToString() + " => " + reader["Purpose"].ToString(), reader["No_"].ToString());
                        ddlPostedPettyCash.Items.Add(li);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void LoadResponsibilityCenter()
        {
            try
            {
                ddlResponsibilityCenter.Items.Clear();

                string grouping = "SURP-CASH";
                string resCenters = webportals.GetResponsibilityCentres(grouping);
                if (!string.IsNullOrEmpty(resCenters))
                {
                    string[] resCenterArr = resCenters.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string rescenter in resCenterArr)
                    {
                        ddlResponsibilityCenter.Items.Add(new ListItem(rescenter));
                    }
                }
                else
                {
                    ddlResponsibilityCenter.Items.Add(new ListItem("No responsibility centers available"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                ddlResponsibilityCenter.Items.Add(new ListItem("Error loading responsibility centers"));
            }
        }
        private void BindGridViewData()
        {
            string pettyCashNo = ddlPostedPettyCash.SelectedValue.ToString();
            string pettyCashLines = webportals.GetPettyCashLines(pettyCashNo);

            if (!string.IsNullOrEmpty(pettyCashLines))
            {

                DataTable dt = new DataTable();
                // dt.Columns.Add("Document No_");
                dt.Columns.Add("Advance Type");
                dt.Columns.Add("Account No_");
                dt.Columns.Add("Account Name");
                dt.Columns.Add("Amount");
                //  dt.Columns.Add("Line No_");


                string[] lines = pettyCashLines.Split(new[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);


                foreach (string line in lines)
                {
                    string[] fields = line.Split(new[] { "::" }, StringSplitOptions.None);
                    if (fields.Length == 6)
                    {
                        DataRow row = dt.NewRow();
                        // row["Document No_"] = fields[0];
                        row["Advance Type"] = fields[1];
                        row["Account No_"] = fields[2];
                        row["Account Name"] = fields[3];
                        row["Amount"] = fields[4];
                        // row["Line No_"] = fields[5];

                        dt.Rows.Add(row);
                    }
                }

                gvLines.DataSource = dt;
                gvLines.DataBind();


            }
            foreach (GridViewRow row in gvLines.Rows)
            {
                string account = row.Cells[2].Text;
                string surrenderNo = Session["DocumentNo"].ToString();
                TextBox txtActualAmount = row.FindControl("txtActualAmount") as TextBox;
                TextBox txtAmountReturned = row.FindControl("txtAmountReturned") as TextBox;
                //if (txtActualAmount.Text == "") txtActualAmount.Text = "0";
                //if (txtAmountReturned.Text == "") txtAmountReturned.Text = "0";

                string response = webportals.LoadPettyCashSurrenderSurrenderLineDetails(surrenderNo, account);
                if (!string.IsNullOrEmpty(response))
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        decimal actualAmount = Convert.ToDecimal(responseArr[1]);
                        decimal cashReturned = Convert.ToDecimal(responseArr[2]);
                        txtActualAmount.Text = actualAmount.ToString();
                        txtAmountReturned.Text = cashReturned.ToString();
                    }
                    else
                    {
                        txtActualAmount.Text = "0";
                        txtAmountReturned.Text = "0";
                    }
                }
            }

        }



        private void BindAttachedDocuments(string documentNo)
        {
            try
            {
                connection = Components.GetconnToNAV();
                command = new SqlCommand()
                {
                    CommandText = "spDocumentLines",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                command.Parameters.AddWithValue("@Company_Name", Components.Company_Name);
                command.Parameters.AddWithValue("@DocNo", "'" + documentNo + "'");
                adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                gvAttachments.DataSource = dt;
                gvAttachments.DataBind();
                connection.Close();

                foreach (GridViewRow row in gvAttachments.Rows)
                {
                    row.Cells[3].Text = row.Cells[5].Text.Split(' ')[0];
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void ddlPostedPettyCash_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGridViewData();
        }

        protected void lbtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string pettyCashNo = ddlPostedPettyCash.SelectedValue.ToString();
                string responsibilityCenter = ddlResponsibilityCenter.SelectedValue.ToString();
                string username = Session["username"].ToString();
                string documentNo = string.Empty;

                if (pettyCashNo == "")
                {
                    Message("Posted imprest no cannot be empty!");
                    return;
                }
                if (responsibilityCenter == "")
                {
                    Message("Responsibility center cannot be empty!");
                    return;
                }

                if (gvAttachments.Rows.Count < 1)
                {
                    Message("Please upload supporting documents.");
                    return;
                }

                string pettyCashSurrenderNo = Session["DocumentNo"].ToString();

                string response = webportals.CreatePettyCashSurrenderHeader(pettyCashSurrenderNo, pettyCashNo, responsibilityCenter);
                if (response != null)
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        documentNo = responseArr[1];
                    }
                    else
                    {
                        Message("An error occured while surendering petty cash. Please try again later");
                        return;
                    }
                }

                foreach (GridViewRow row in gvLines.Rows)
                {
                    TextBox txtActualAmount = row.FindControl("txtActualAmount") as TextBox;
                    TextBox txtAmountReturned = row.FindControl("txtAmountReturned") as TextBox;

                    if (!Components.IsNumeric(txtActualAmount.Text))
                    {
                        Message("Invalid Actual Amount Spent!");
                        return;
                    }
                    if (!Components.IsNumeric(txtAmountReturned.Text))
                    {
                        Message("Invalid Cash Amount Returned!");
                        return;
                    }

                    decimal AmountSpent = Convert.ToDecimal(txtActualAmount.Text);
                    decimal cashReturned = Convert.ToDecimal(txtAmountReturned.Text);
                    decimal totalAmount = AmountSpent + cashReturned;
                    decimal Amount = Convert.ToDecimal(row.Cells[4].Text);
                    string accountNo = row.Cells[2].Text;

                    if (totalAmount != Amount)
                    {
                        Message("The sum of Actual Amount Spent and Cash Amount Returned must ne equal to the amount.");
                        return;
                    }
                    string response1 = webportals.InsertPettyCashSurrenderLines(documentNo, pettyCashNo, Amount, AmountSpent, cashReturned, accountNo);
                    if (response1 != null)
                    {
                        string[] responseArr = response1.Split(strLimiters, StringSplitOptions.None);
                        string returnMsg = responseArr[0];
                        if (returnMsg == "SUCCESS")
                        {
                            documentNo = responseArr[1];
                        }
                        else
                        {
                            Message("An error occured while surendering petty cash. Please try again later");
                            return;
                        }
                    }
                }

                string approvalResponse = webportals.OnSendPettyCashSurrenderForApproval(documentNo);
                if (approvalResponse == "SUCCESS")
                {
                    SuccessMessage("Petty Cash surrender has been submitted successfully!");
                }
                else
                {
                    Message(approvalResponse);
                    return;
                }
            }
            catch (Exception ex)
            {
                Message("ERROR: " + ex.Message);
                ex.Data.Clear();
            }
        }

        private void Message(string message)
        {
            string strScript = "<script>alert('" + message + "');</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }
        private void SuccessMessage(string message)
        {
            string page = "PettyCashAccounting.aspx";
            string strScript = "<script>alert('" + message + "');window.location='" + page + "';</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }

        protected void lbtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuImprestDocs.PostedFile != null)
                {
                    string username = Session["username"].ToString();
                    string filePath = fuImprestDocs.PostedFile.FileName.Replace(" ", "-");
                    string fileName = fuImprestDocs.FileName.Replace(" ", "-");
                    string fileExtension = Path.GetExtension(fileName).Split('.')[1].ToLower();
                    string DocumentNo = Session["DocumentNo"].ToString();
                    if (fileExtension == "pdf" || fileExtension == "jpg" || fileExtension == "png" || fileExtension == "jpeg" || fileExtension == "docx" || fileExtension == "doc")
                    {
                        string strPath = Server.MapPath("~/Uploads");
                        if (!Directory.Exists(strPath))
                        {
                            Directory.CreateDirectory(strPath);
                        }

                        string pathToUpload = Path.Combine(strPath, DocumentNo + fileName.ToUpper());
                        if (File.Exists(pathToUpload))
                        {
                            File.Delete(pathToUpload);
                        }
                        fuImprestDocs.SaveAs(pathToUpload);
                        Components.ObjNav.SaveMemoAttchmnts(DocumentNo, pathToUpload, fileName.ToUpper(), username);
                        Stream fs = fuImprestDocs.PostedFile.InputStream;
                        BinaryReader br = new BinaryReader(fs);
                        byte[] bytes = br.ReadBytes((int)fs.Length);
                        string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                        Components.ObjNav.RegFileUploadAtt(DocumentNo, fileName.ToUpper(), base64String, 52178705, "Petty Cash Surrender");
                        BindAttachedDocuments(DocumentNo);
                        Message("Document uploaded successfully");
                    }
                    else
                    {
                        Message("Please upload files with .pdf, .png, .jpg and .jpeg extensions only!");
                    }
                }
                else
                {
                    Message("Please upload a file!");
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        protected void lbtnRemoveAttach_Click(object sender, EventArgs e)
        {
            try
            {
                string documentNo = Session["DocumentNo"].ToString();
                //string documentNo = Request.QueryString["SurrenderNo"];
                // string documentNo = lblLNo.Text;
                string[] args = new string[2];
                args = (sender as LinkButton).CommandArgument.ToString().Split(';');
                string systemId = args[0];
                if (Components.ObjNav.DeleteDocumentAttachments(systemId))
                {
                    Message("Document deleted successfully!");
                    BindAttachedDocuments(documentNo);

                }
                else
                {
                    Message("An error occured while deleting document. Please try again later!");
                    return;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        protected void lbtnRemoveAttach_Click1(object sender, EventArgs e)
        {
            try
            {
                string[] args = new string[2];
                args = (sender as LinkButton).CommandArgument.ToString().Split(';');
                string systemId = args[0];
                string documentNo = Session["DocumentNo"].ToString();
                string fileName = string.Empty;
                string documentDetails = webportals.GetAttachmentDetails(systemId);
                if (documentDetails != null)
                {
                    string[] documentsDetailsArr = documentDetails.Split(strLimiters, StringSplitOptions.None);
                    fileName = documentsDetailsArr[1].Split('.')[0];
                }

                string response = webportals.DeleteDocumentAttachment(systemId, fileName, documentNo);
                if (response != null)
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg1 = responseArr[0];
                    string returnMsg2 = responseArr[1];
                    if (returnMsg1 == "SUCCESS" && returnMsg2 == "SUCCESS")
                    {
                        Message("Document deleted successfully.");
                        BindAttachedDocuments(documentNo);
                    }
                    else
                    {
                        Message("An error has occured. Please try again later.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void ddlReceipts_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetPostedReceipts();
        }

        private void GetPostedReceipts()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
    }
}