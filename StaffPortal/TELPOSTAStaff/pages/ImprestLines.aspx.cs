﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff.pages
{
    public partial class ImprestLines : System.Web.UI.Page
    {
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader reader;
        SqlDataAdapter adapter;
        Staffportall webportals = Components.ObjNav;
        string[] strLimiters = new string[] { "::" };
        string[] strLimiters2 = new string[] { "[]" };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }

                LoadStaffDetails();
                LoadResponsibilityCenter();
                GetMemoNo();

                string query = Request.QueryString["query"];
                string approvalStatus = Request.QueryString["status"].Replace("%", " ");
                if (query == "new")
                {
                    MultiView1.SetActiveView(vwHeader);
                }
                else if (query == "old")
                {
                    string imprestNo = Request.QueryString["ImprestNo"];
                    MultiView1.SetActiveView(vwLines);
                    lblLNo.Text = imprestNo;
                    lblImprestNo.Text = imprestNo;
                    LoadAdvanceTypes();
                    BindGridViewData(imprestNo);
                    BindAttachedDocuments(imprestNo);
                }

                if (approvalStatus == "Open" || approvalStatus == "Pending")
                {
                    btnApproval.Visible = true;
                   // btnCancellApproval.Visible = false;
                    attachments.Visible = true;
                }
                else if (approvalStatus == "Pending Approval")
                {
                    btnApproval.Visible = false;
                    //btnCancellApproval.Visible = true;
                    lbtnAddLine.Visible = false;
                    lbtnClose.Visible = false;
                    attachments.Visible = false;
                }
                else
                {
                    btnApproval.Visible = false;
                   // btnCancellApproval.Visible = false;
                    lbtnAddLine.Visible = false;
                    lbtnClose.Visible = false;
                    attachments.Visible = false;
                }
            }
        }

        private string[] GetMemoNo()
        {
            string[] memo = new string[3];

            try
            {
                connection = Components.GetconnToNAV();
                command = new SqlCommand()
                {
                    CommandText = "spGetMemoNo",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                command.Parameters.AddWithValue("@Company_Name", Components.Company_Name);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        memo[0] = reader["No_"].ToString();
                        memo[1] = reader["Responsibility centre"].ToString().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return memo;

        }
        private void LoadStaffDetails()
        {
            try
            {
                string staffNo = Session["username"].ToString();
                string staffName = Session["StaffName"].ToString();
                string response = webportals.GetStaffDepartmentDetails(staffNo);
                if (!string.IsNullOrEmpty(response))
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                       // lblDirectorate.Text = responseArr[1];
                        lblDepartment.Text = responseArr[1];
                    }
                }

                lblStaffNo.Text = staffNo;
                lblRequester.Text = staffNo;
                lblPayee.Text = staffName;
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
                //ddlResponsibilityCenter.Items.Clear();
                connection = Components.GetconnToNAV();
                command = new SqlCommand()
                {
                    CommandText = "spGetResponsilityCenter",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                command.Parameters.AddWithValue("@Company_Name", Components.Company_Name);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListItem li = new ListItem(reader["Name"].ToString().ToUpper(), reader["Code"].ToString());
                        //ddlResponsibilityCenter.Items.Add(li);

                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }

        }
        private void LoadAdvanceTypes()
        {
            try
            {
                string advancetype = webportals.GetAdvancetype(3);
                if (!string.IsNullOrEmpty(advancetype))
                {
                    string[] resCenterArr = advancetype.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);

                   
                    foreach (string rescenter in resCenterArr)
                    {
                        string[] responseArr = rescenter.Split(strLimiters, StringSplitOptions.None);
                        ListItem li = new ListItem(responseArr[0]);
                        ddlAdvancType.Items.Add(li);
                    }
                }


            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void LoadAdvanceTypes1()
        {
            try
            {
                ddlAdvancType.Items.Clear();
                connection = Components.GetconnToNAV();
                command = new SqlCommand()
                {
                    CommandText = "spLoadImprestAdvancedTypes",
                    CommandType = CommandType.StoredProcedure,
                    Connection = connection
                };
                command.Parameters.AddWithValue("@Company_Name", Components.Company_Name);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ListItem li = new ListItem(reader["Description"].ToString().ToUpper(), reader["Code"].ToString());
                        ddlAdvancType.Items.Add(li);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void lbtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string username = Session["username"].ToString();
                string department = lblDepartment.Text;
                //string directorate = lblDirectorate.Text;
                //string responsibilityCenter = ddlResponsibilityCenter.SelectedValue;
                string purpose = txtPurpose.Text;

                //if (string.IsNullOrEmpty(department))
                //{
                //    Message("Department cannot be null!");
                //    return;
                //}
                //if (string.IsNullOrEmpty(directorate))
                //{
                //    Message("Division cannot be null!");
                //    return;
                //}
                //if (string.IsNullOrEmpty(responsibilityCenter))
                //{
                //    Message("Responsibility center cannot be null!");
                //    return;
                //}
                if (purpose == "")
                {
                    Message("Purpose cannot be null!");
                    txtPurpose.Focus();
                    return;
                }
                if (purpose.Length > 200)
                {
                    Message("Purpose should be 200 characters and below!");
                    return;
                }

                string response = webportals.CreateImprestRequisitionHeader(username,  purpose);
                if (!string.IsNullOrEmpty(response))
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        string imprestNo = responseArr[1];
                        Message($"Imprest number {imprestNo} has been created successfully!");
                        Session["ImprestNo"] = imprestNo;
                        BindGridViewData(imprestNo);
                        NewView();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void NewView()
        {
            try
            {
                MultiView1.SetActiveView(vwLines);
                string imprestNo = Session["ImprestNo"].ToString();
                newLines.Visible = true;
                lbtnAddLine.Visible = false;
                lblLNo.Text = imprestNo;
                lblImprestNo.Text = imprestNo;
                LoadAdvanceTypes();
                BindGridViewData(imprestNo);
                BindAttachedDocuments(imprestNo);

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void BindAttachedDocuments1(string claimNo)
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
                command.Parameters.AddWithValue("@ReqNo", "'" + claimNo + "'");
                adapter = new SqlDataAdapter();
                adapter.SelectCommand = command;
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                gvAttachments.DataSource = dt;
                gvAttachments.DataBind();
                connection.Close();

                foreach (GridViewRow row in gvAttachments.Rows)
                {
                    row.Cells[3].Text = row.Cells[3].Text.Split(' ')[0];
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        private void BindAttachedDocuments(string imprestNo)
        {
            try
            {
                // Call the AL web service method
                string docLines = webportals.GetDocumentlines(imprestNo);

                // Check if the response is not empty or null
                if (!string.IsNullOrEmpty(docLines) && docLines != "No document lines")
                {
                    // Split the response by '[]' to separate each line
                    string[] lineItems = docLines.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);

                    // Create a DataTable to hold the parsed data for binding
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Document No");
                    dt.Columns.Add("Description");
                    dt.Columns.Add("$systemCreatedAt");
                    dt.Columns.Add("SystemId");
                    


                    // Loop through each line item
                    foreach (string item in lineItems)
                    {
                        // Split each line by '::' to get individual fields
                        string[] fields = item.Split(strLimiters, StringSplitOptions.None);

                        // Check if we have the correct number of fields to avoid errors
                        if (fields.Length == 4)
                        {
                            DataRow row = dt.NewRow();
                            row["Document No"] = fields[0];
                            row["Description"] = fields[1];
                            row["$systemCreatedAt"] = fields[2];
                            row["SystemId"] = fields[3];
                            dt.Rows.Add(row);
                        }
                    }

                    // Bind the DataTable to the GridView
                    gvAttachments.DataSource = dt;
                    gvAttachments.DataBind();
                }
                else
                {
                    // Handle the case where there are no imprest lines
                    gvAttachments.DataSource = null;
                    gvAttachments.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or show an error message as needed)
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        private void BindGridViewData(string imprestNo)
        {
            try
            {
                // Call the AL web service method
                string imprestLines = webportals.GetImprestLines(imprestNo);

                // Check if the response is not empty or null
                if (!string.IsNullOrEmpty(imprestLines) && imprestLines != "No Imprests lines")
                {
                    // Split the response by '[]' to separate each line
                    string[] lineItems = imprestLines.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);

                    // Create a DataTable to hold the parsed data for binding
                    DataTable dt = new DataTable();
                    dt.Columns.Add("No");
                    dt.Columns.Add("Advance Type");
                    dt.Columns.Add("Account No");
                    dt.Columns.Add("Account Name");
                    dt.Columns.Add("Amount");

                    // Loop through each line item
                    foreach (string item in lineItems)
                    {
                        // Split each line by '::' to get individual fields
                        string[] fields = item.Split(strLimiters, StringSplitOptions.None);

                        // Check if we have the correct number of fields to avoid errors
                        if (fields.Length == 5)
                        {
                            DataRow row = dt.NewRow();
                            row["No"] = fields[0];
                            row["Advance Type"] = fields[1];
                            row["Account No"] = fields[2];
                            row["Account Name"] = fields[3];
                            row["Amount"] = fields[4];
                            dt.Rows.Add(row);
                        }
                    }

                    // Bind the DataTable to the GridView
                    gvLines.DataSource = dt;
                    gvLines.DataBind();
                }
                else
                {
                    gvLines.DataSource = null;
                    gvLines.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (log or show an error message as needed)
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void Message(string message)
        {
            string strScript = "<script>alert('" + message + "')</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }

        private void SuccessMessage(string message)
        {
            string page = "ImprestListing.aspx";
            string strScript = "<script>alert('" + message + "');window.location='" + page + "'</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }

        protected void btnLine_Click(object sender, EventArgs e)
        {
            try
            {
                string imprestNo = lblImprestNo.Text;
                string employeeNo = Session["username"].ToString();
                string advanceType = ddlAdvancType.SelectedValue;
                string amount = txtAmnt.Text;
                if (advanceType == "0")
                {
                    Message("Advance type cannot be null!");
                    return;
                }
                if (amount == "")
                {
                    Message("Amount cannot be empty!");
                    txtAmnt.Focus();
                    return;
                }

                string response = webportals.InsertImprestRequisitonLines(imprestNo, advanceType, Convert.ToDecimal(amount));
                if (!string.IsNullOrEmpty(response))
                {
                    string[] strLimiters = new string[] { "::" };
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        Message("Line added successfully!");
                        txtAmnt.Text = string.Empty;
                        
                    }
                    BindGridViewData(imprestNo);
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void lbtnAddLine_Click(object sender, EventArgs e)
        {
            string imprestNo = string.Empty;
            if (Request.QueryString["ImprestNo"] == null)
            {
                imprestNo = Session["ImprestNo"].ToString();
            }
            else
            {
                imprestNo = Request.QueryString["ImprestNo"].ToString();
            }
            lblLNo.Text = imprestNo;
            LoadAdvanceTypes();
            newLines.Visible = true;
            lbtnAddLine.Visible = false;
        }

        protected void lbtnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                string status = Request.QueryString["status"].ToString().Replace("%", " ");
                if (status == "Open" || status == "Pending")
                {
                    string[] args = new string[2];
                    string imprestNo = lblImprestNo.Text;
                    args = (sender as LinkButton).CommandArgument.ToString().Split(';');
                    string advanceType = args[0];
                    string response = webportals.RemoveImprestRequisitionLine(imprestNo, advanceType);
                    if (!string.IsNullOrEmpty(response))
                    {
                        if (response == "SUCCESS")
                        {
                            Message("Line deleted successfully!");
                            BindGridViewData(imprestNo);
                        }
                        else
                        {
                            Message("An error occured while removing line. Please try again later");
                            return;
                        }
                    }
                }
                else
                {
                    Message("You can only edit an open document!");
                    return;
                }

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void btnApproval_Click(object sender, EventArgs e)
        {
            try
            {
                string imprestNo = lblImprestNo.Text;
                if (gvLines.Rows.Count < 1)
                {
                    Message("Please add lines before sending for approval!");
                    return;
                }
                if (gvAttachments.Rows.Count < 1)
                {
                    Message("Please attach documents before sending for approval!");
                    return;
                }
                string msg = webportals.OnSendImprestRequisitionForApproval(imprestNo);
                if (msg == "SUCCESS")
                {
                    SuccessMessage($"Imprest number {imprestNo} has been sent for approval successfuly!");
                }
                else
                {
                    Message("ERROR: " + msg);
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void btnCancellApproval_Click(object sender, EventArgs e)
        {
            try
            {
                string imprestNo = lblImprestNo.Text;
                webportals.OnCancelImprestRequisition(imprestNo);
                SuccessMessage($"Imprest number {imprestNo} has been cancelled successfuly!");
            }
            catch (Exception ex)
            {
                Message("ERROR: " + ex.Message);
                ex.Data.Clear();
            }
        }

        protected void lbtnClose_Click(object sender, EventArgs e)
        {
            newLines.Visible = false;
            lbtnAddLine.Visible = true;
        }

        protected void lbtnAttach_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuClaimDocs.PostedFile != null)
                {
                    string DocumentNo = lblLNo.Text; // Replace slashes with dashes
                    string username = Session["username"].ToString();
                    string filePath = fuClaimDocs.PostedFile.FileName.Replace(" ", "-");
                    string fileName = fuClaimDocs.FileName.Replace(" ", "-");
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    if (fileExtension == ".pdf" || fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg")
                    {
                        string strPath = Server.MapPath("~/Uploads");
                        if (!Directory.Exists(strPath))
                        {
                            Directory.CreateDirectory(strPath);
                        }


                        string pathToUpload = Path.Combine(strPath, DocumentNo.Replace("/", "-") + fileName.ToUpper());

                        if (File.Exists(pathToUpload))
                        {
                            File.Delete(pathToUpload);
                        }
                        fuClaimDocs.SaveAs(pathToUpload);
                        webportals.SaveMemoAttchmnts(DocumentNo, pathToUpload, fileName.ToUpper(), username);

                        Stream fs = fuClaimDocs.PostedFile.InputStream;
                        BinaryReader br = new BinaryReader(fs);
                        byte[] bytes = br.ReadBytes((int)fs.Length);
                        string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                        webportals.RegFileUploadAtt(DocumentNo, fileName.ToUpper(), base64String, 52178708, "Imprest Requisition");
                        BindAttachedDocuments(DocumentNo);
                        Message("Document uploaded successfully!");
                    }
                    else
                    {
                        Message("Please upload files with .pdf, .png, .jpg and .jpeg extensions only!");
                        return;
                    }
                }
                else
                {
                    Message("Please upload a file!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Message("An error occurred: " + ex.Message);
            }
        }


        protected void lbtnRemoveAttach_Click(object sender, EventArgs e)
        {
            string status = Request.QueryString["status"].ToString().Replace("%", " ");
            if (status == "Open" || status == "Pending")
            {
                string[] args = new string[2];
                args = (sender as LinkButton).CommandArgument.ToString().Split(';');
                string systemId = args[0];
                string documentNo = lblLNo.Text;
                string fileName = string.Empty;
                string documentDetails = webportals.GetAttachmentDetails(systemId);
                if (documentDetails != null)
                {
                    string[] documentsDetailsArr = documentDetails.Split(strLimiters, StringSplitOptions.None);
                    fileName = documentsDetailsArr[1];
                    documentNo = documentsDetailsArr[0];
                }

                string response = webportals.DeleteDocumentAttachment(systemId, fileName, documentNo);
                if (response != null)
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg1 = responseArr[0];
                    string returnMsg2 = responseArr[1];
                    if (returnMsg2 == "SUCCESS")//&& returnMsg2 == "SUCCESS"
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
            else
            {
                Message("You can only edit an open document!");
                return;
            }
        }
    }
}