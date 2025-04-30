using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.UI;
using System.Web.UI.WebControls;
using TELPOSTAStaff.NAVWS;

namespace TELPOSTAStaff.pages
{
    public partial class TransportRequisitionListing : System.Web.UI.Page
    {
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
                }
            }

        }
        protected string Jobs()
        {
            var htmlStr = string.Empty;
            try
            {
                string username = Session["username"].ToString();
                string ClaimList = webportals.GetMyClaims(username);
                if (!string.IsNullOrEmpty(ClaimList))
                {
                    string[] ClaimListArr = ClaimList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < ClaimListArr.Length; i++)
                    {
                        string[] responseArr = ClaimListArr[i].Split(strLimiters, StringSplitOptions.None);
                        if (responseArr.Length >= 5)
                        {
                            // Extract fields
                            string No = responseArr[0];
                            string Date = responseArr[1];
                            string Payee = responseArr[2];
                            string Purpose = responseArr[3];
                            string status = responseArr[4];
                            string accommodationProvided = responseArr[5];

                            // Determine status class
                            var statusCls = "secondary";
                            switch (status)
                            {
                                case "Open":
                                    statusCls = "warning"; break;
                                case "Pending":
                                    statusCls = "warning"; break;
                                case "1st Approval":
                                    statusCls = "default"; break;
                                case "2nd Approval":
                                    statusCls = "primary"; break;
                                case "Cheque Printing":
                                    statusCls = "success"; break;
                                case "Posted":
                                    statusCls = "success"; break;
                                case "Cancelled":
                                    statusCls = "danger"; break;
                                case "Checking":
                                    statusCls = "info"; break;
                                case "VoteBook":
                                    statusCls = "info"; break;
                                case "Pending Approval":
                                    statusCls = "primary"; break;
                                case "Approved":
                                    statusCls = "success"; break;
                            }

                            // Generate table row
                            htmlStr += "<tr class='text-primary small'>";
                            htmlStr += $"<td>{i + 1}</td>";
                            htmlStr += $"<td>{No}</td>";
                            htmlStr += $"<td>{Date}</td>";
                            htmlStr += $"<td>{Payee}</td>";
                            htmlStr += $"<td>{Purpose}</td>";

                            // 
                            htmlStr += $"<td><span class='label label-{statusCls}'>{status}</span></td>";



                            // Dropdown actions
                            htmlStr += "<td>";
                            htmlStr += "  <div class='dropdown'>";
                            htmlStr += "    <button class='btn btn-sm btn-primary dropdown-toggle' type='button' data-toggle='dropdown'>Actions <span class='caret'></span></button>";
                            htmlStr += "    <ul class='dropdown-menu'>";
                            htmlStr += $"      <li><a class='dropdown-item' href='transportrequisition.aspx?requestNo={No}&query=old&status={status}&accomodation={accommodationProvided}'><i class='fa fa-info-circle text-success'></i> Details</a></li>";
                            //htmlStr += $"      <li><a class='dropdown-item' href='transportreq2.aspx?requestNo={No}&query=old&status={status}&accomodation={accommodationProvided}'><i class='fa fa-info-circle text-success'></i> Details</a></li>";
                            htmlStr += $"      <li><a class='dropdown-item' href='ApprovalTracking.aspx?DocNum={No}'><i class='fa fa-check-circle text-info'></i> Approval Tracking</a></li>";
                            htmlStr += "    </ul>";
                            htmlStr += "  </div>";
                            htmlStr += "</td>";

                            htmlStr += "</tr>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return htmlStr;
        }

        protected string Jobs1()
        {
            var htmlStr = string.Empty;
            try
            {
                string username = Session["username"].ToString();
                string ClaimList = webportals.GetMyClaims(username);
                if (!string.IsNullOrEmpty(ClaimList))
                {
                    int counter = 0;
                    string[] ClaimListArr = ClaimList.Split(strLimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string claimlist in ClaimListArr)
                    {
                        counter++;
                        string[] responseArr = claimlist.Split(strLimiters, StringSplitOptions.None);
                        if (responseArr.Length == 5)
                        {
                            var statusCls = "default";
                            string accommodationProvided = responseArr[5];
                            string status = responseArr[4];
                            switch (status)
                            {
                                case "Pending":
                                    statusCls = "warning"; break;
                                case "1st Approval":
                                    statusCls = "default"; break;
                                case "2nd Approval":
                                    statusCls = "primary"; break;
                                case "Cheque Printing":
                                    statusCls = "success"; break;
                                case "Posted":
                                    statusCls = "success"; break;
                                case "Cancelled":
                                    statusCls = "danger"; break;
                                case "Checking":
                                    statusCls = "info"; break;
                                case "VoteBook":
                                    statusCls = "info"; break;
                                case "Pending Approval":
                                    statusCls = "primary"; break;
                                case "Approved":
                                    statusCls = "success"; break;
                            }

                            htmlStr += String.Format(@"
                    <tr class='text-primary small'>
                        <td>{0}</td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                        <td>{4}</td>
                        <td><span class='label label-{5}'>{6}</span></td>
                        <td>
                            <div class='options btn-group'>
                                <a class='label label-success dropdown-toggle btn-success' data-toggle='dropdown' href='#' style='padding:4px;margin-top:3px'>
                                    <i class='fa fa-gears'></i> Options
                                </a>
                                <ul class='dropdown-menu'>
                                    <li><a href=$'transportrequisition.aspx?requestNo={1}&query=old&status={6} & accomodation = {accommodationProvided}'>
                                        <i class='fa fa-plus-circle text-success'></i><span class='text-success'>Details</span></a></li>
                                    <li><a href='ApprovalTracking.aspx?DocNum={1}'>
                                        <i class='fa fa-plus-circle text-success'></i><span class='text-success'>Approval Tracking</span></a></li>
                                </ul>    
                            </div>
                        </td>
                    </tr>
                    ",
                                counter,
                                responseArr[0],
                                responseArr[1],
                                responseArr[2],
                                responseArr[3],
                                statusCls,
                                responseArr[4]
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return htmlStr;
        }

    }
}
