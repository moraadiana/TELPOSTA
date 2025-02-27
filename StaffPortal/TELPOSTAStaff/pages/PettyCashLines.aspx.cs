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
    public partial class PettyCashLines : System.Web.UI.Page
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
                LoadAccountNos();
                LoadAdvanceTypes();

                string query = Request.QueryString["query"];
                string approvalStatus = Request.QueryString["status"].Replace("%", " ");
                if (query == "new")
                {
                    MultiView1.SetActiveView(vwHeader);

                }
                else if (query == "old")
                {
                    string pettyCashNo = Request.QueryString["PettyCashNo"];
                    MultiView1.SetActiveView(vwLines);
                    lblLNo.Text = pettyCashNo;
                    lblPettyCashNo.Text = pettyCashNo;
                    LoadAdvanceTypes();
                    BindGridViewData(pettyCashNo);
                    BindAttachedDocuments(pettyCashNo);
                    // LoadAccountNos();
                }

                if (approvalStatus == "Open" || approvalStatus == "Pending")
                {
                    btnApproval.Visible = true;
                    btnCancellApproval.Visible = false;
                    attachments.Visible = true;
                }
                else if (approvalStatus == "Pending Approval")
                {
                    btnApproval.Visible = false;
                    btnCancellApproval.Visible = true;
                    lbtnAddLine.Visible = false;
                    lbtnClose.Visible = false;
                    attachments.Visible = false;
                }
                else
                {
                    btnApproval.Visible = false;
                    btnCancellApproval.Visible = false;
                    lbtnAddLine.Visible = false;
                    lbtnClose.Visible = false;
                    attachments.Visible = false;
                }
            }
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

                        lblDepartment.Text = responseArr[1];
                       // lblDirectorate.Text = responseArr[2];
                    }
                }

                lblStaffNo.Text = staffNo;
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

                //string grouping = "P-CASH";
                //string resCenters = webportals.GetResponsibilityCentres(grouping);
                //if (!string.IsNullOrEmpty(resCenters))
                //{
                //    string[] resCenterArr = resCenters.Split(new string[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);

                //    foreach (string rescenter in resCenterArr)
                //    {
                //        ddlResponsibilityCenter.Items.Add(new ListItem(rescenter));
                //    }
                //}
                //else
                //{
                //    ddlResponsibilityCenter.Items.Add(new ListItem("No responsibility centers available"));
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
               // ddlResponsibilityCenter.Items.Add(new ListItem("Error loading responsibility centers"));
            }
        }
        private void LoadAccountNos()
        {

            try
            {
                ddlAccountNo.Items.Clear();
                string AccountNo = webportals.GetAccountNo();
                if (!string.IsNullOrEmpty(AccountNo))
                {
                    string[] AccountNoArr = AccountNo.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string account in AccountNoArr)
                    {
                        string[] responseArr = account.Split(strLimiters, StringSplitOptions.None);
                        if (responseArr.Length >= 2)
                        {
                            //ListItem li = new ListItem(responseArr[1], responseArr[0]);
                            //ddlAccountNo.Items.Add(li);
                            string displayText = $"{responseArr[0]} - {responseArr[1]}"; // Display as "No - Name"
                            string value = responseArr[0]; // Only the account number is stored
                            ListItem li = new ListItem(displayText, value);
                            ddlAccountNo.Items.Add(li);
                        }

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
                ddlAdvancType.Items.Clear();
                string advancetypes = webportals.GetAdvancetype(6);
                if (!string.IsNullOrEmpty(advancetypes))
                {
                    string[] advancetypesArr = advancetypes.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string account in advancetypesArr)
                    {
                        string[] responseArr = account.Split(strLimiters, StringSplitOptions.None);
                        if (responseArr.Length >= 2)
                        {
                            
                            string displayText = $"{responseArr[0]} - {responseArr[1]}"; 
                            string value = responseArr[0];
                            ListItem li = new ListItem(displayText, value);
                            ddlAdvancType.Items.Add(li);
                        }

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
                    CommandText = "spLoadPettyCashAdvancedTypes",
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
                string pettyCashType = ddlPettyCashType.SelectedValue;
                //string ApplicantType = DdApplicantType.SelectedItem.Text;
                int MyPettyType = 0;
                if (pettyCashType == "Claim")
                {
                    MyPettyType = 1;
                }
                else if (pettyCashType == "Advance")
                {
                    MyPettyType = 2;
                }
                //string directorate = lblDirectorate.Text;
                //string responsibilityCenter = ddlResponsibilityCenter.SelectedValue;
                string purpose = txtPurpose.Text;


                if (string.IsNullOrEmpty(department))
                {
                    Message("Department cannot be null!");
                    return;
                }
                //if (string.IsNullOrEmpty(directorate))
                //{
                //    Message("Unit cannot be null!");
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

                string response = webportals.CreatePettyCashRequisitionHeader(username, department, purpose, MyPettyType);
                if (!string.IsNullOrEmpty(response))
                {
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        string pettyCashNo = responseArr[1];
                        Message($"Petty cash number {pettyCashNo} has been created successfully!");
                        Session["PettyCashNo"] = pettyCashNo;
                        BindGridViewData(pettyCashNo);
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
                string pettyCashNo = Session["PettyCashNo"].ToString();
                newLines.Visible = true;
                lbtnAddLine.Visible = false;
                lblLNo.Text = pettyCashNo;
                lblPettyCashNo.Text = pettyCashNo;
                LoadAdvanceTypes();
                BindGridViewData(pettyCashNo);
                BindAttachedDocuments(pettyCashNo);

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }
        private void BindAttachedDocuments(string documentNo)
        {
            try
            {
                // Call the AL web service method
                string docLines = webportals.GetDocumentlines(documentNo);

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
        private void BindAttachedDocuments1(string documentNo)
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
                    row.Cells[3].Text = row.Cells[3].Text.Split(' ')[0];
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }


        private void BindGridViewData(string pettyCashNo)
        {
            string pettyCashLines = webportals.GetPettyCashLines(pettyCashNo);

            if (!string.IsNullOrEmpty(pettyCashLines))
            {

                DataTable dt = new DataTable();
                dt.Columns.Add("Document No_");
                dt.Columns.Add("Advance Type");
                dt.Columns.Add("Account No_");
                dt.Columns.Add("Account Name");
                dt.Columns.Add("Amount");
                dt.Columns.Add("Line No_");


                string[] lines = pettyCashLines.Split(new[] { "[]" }, StringSplitOptions.RemoveEmptyEntries);


                foreach (string line in lines)
                {
                    string[] fields = line.Split(new[] { "::" }, StringSplitOptions.None);
                    if (fields.Length == 6)
                    {
                        DataRow row = dt.NewRow();
                        row["Document No_"] = fields[0];
                        row["Advance Type"] = fields[1];
                        row["Account No_"] = fields[2];
                        row["Account Name"] = fields[3];
                        row["Amount"] = fields[4];
                        row["Line No_"] = fields[5];

                        dt.Rows.Add(row);
                    }
                }

                gvLines.DataSource = dt;
                gvLines.DataBind();


            }

        }


        private void Message(string message)
        {
            string strScript = "<script>alert('" + message + "')</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }

        private void SuccessMessage(string message)
        {
            string page = "PettyCashListing.aspx";
            string strScript = "<script>alert('" + message + "');window.location='" + page + "'</script>";
            ClientScript.RegisterStartupScript(GetType(), "Client Script", strScript.ToString());
        }

        protected void btnLine_Click(object sender, EventArgs e)
        {
            try
            {
                string pettyCashNo = lblPettyCashNo.Text;
                string username = Session["username"].ToString();
                string advanceType = ddlAdvancType.SelectedValue;
                string amount = txtAmnt.Text;
                string accountNo = ddlAccountNo.SelectedValue;
                if (advanceType == "0")
                {
                    Message("Advance type cannot be null!");
                    return;
                }
                if (accountNo == "0")
                {
                    Message("Account Number cannot be null!");
                    return;
                }
                if (amount == "")
                {
                    Message("Amount cannot be empty!");
                    txtAmnt.Focus();
                    return;
                }

                string response = webportals.InsertPettyCashRequisitionLine(username, pettyCashNo, accountNo, advanceType, Convert.ToDecimal(amount));
                if (!string.IsNullOrEmpty(response))
                {
                    string[] strLimiters = new string[] { "::" };
                    string[] responseArr = response.Split(strLimiters, StringSplitOptions.None);
                    string returnMsg = responseArr[0];
                    if (returnMsg == "SUCCESS")
                    {
                        Message("Line added successfully!");
                        txtAmnt.Text = string.Empty;
                        BindGridViewData(pettyCashNo);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        protected void lbtnAddLine_Click(object sender, EventArgs e)
        {
            string pettyCashNo = string.Empty;
            if (Request.QueryString["PettyCashNo"] == null)
            {
                pettyCashNo = Session["PettyCashNo"].ToString();
            }
            else
            {
                pettyCashNo = Request.QueryString["PettyCashNo"].ToString();
            }
            lblLNo.Text = pettyCashNo;
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
                    string pettyCashNo = lblPettyCashNo.Text;
                    args = (sender as LinkButton).CommandArgument.ToString().Split(';');
                    string advanceType = args[0];
                    string response = webportals.RemoveImprestRequisitionLine(pettyCashNo, advanceType);
                    if (!string.IsNullOrEmpty(response))
                    {
                        if (response == "SUCCESS")
                        {
                            Message("Line deleted successfully!");
                            BindGridViewData(pettyCashNo);
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
                string pettyCashNo = lblPettyCashNo.Text;
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
                string msg = webportals.OnSendPettyCashRequisitionForApproval(pettyCashNo);
                if (msg == "SUCCESS")
                {
                    SuccessMessage($"Imprest number {pettyCashNo} has been sent for approval successfuly!");
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
                string pettyCashNo = lblPettyCashNo.Text;
                webportals.OnCancelImprestRequisition(pettyCashNo);
                SuccessMessage($"Petty cash number {pettyCashNo} has been cancelled successfuly!");
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
                    string DocumentNo = lblLNo.Text;
                    string username = Session["username"].ToString();
                    string filePath = fuClaimDocs.PostedFile.FileName.Replace(" ", "-");
                    string fileName = fuClaimDocs.FileName.Replace(" ", "-");
                    string fileExtension = Path.GetExtension(fileName).Split('.')[1].ToLower();
                    if (fileExtension == "pdf" || fileExtension == "jpg" || fileExtension == "png" || fileExtension == "jpeg")
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
                ex.Data.Clear();
            }
        }

        protected void lbtnRemoveAttach_Click(object sender, EventArgs e)
        {
            try
            {
                //string documentNo = Session["PettyCashNo"].ToString();
                string documentNo = Request.QueryString["PettyCashNo"];
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

    }
}