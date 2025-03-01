<%@ Page Title="Travel Requisition" Language="C#" MasterPageFile="~/Layout/Main.Master" AutoEventWireup="true" CodeBehind="TransportRequisition.aspx.cs" Inherits="TELPOSTAStaff.pages.TransportRequisition" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Main1" runat="Server">
    <!-- Content Wrapper. Contains page content -->
    <div class="content-wrapper">
        <!-- Content Header (Page header) -->
        <section class="content-header">
            <h1>Transport Requisition</h1>
            <ol class="breadcrumb">
                <li><a href="Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
                <li class="active">Transport Requisition</li>
            </ol>
        </section>

        <!-- Main content -->
        <section class="content">
            <div class="row">
                <div class="col-md-12">
                    <asp:MultiView ID="MultiView1" runat="server">
                        <asp:View ID="View1" runat="server">
                            <div class="box box-warning box-solid">
                                <div class="box-header with-border">
                                    <h3 class="box-title">New Transport Requisition</h3>
                                </div>
                                <div class="box-body">
                                    <div class="form-row">
                                     
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label>Staff No: </label>
                                                <asp:Label ID="lblStaffNo" runat="server" Text="" ForeColor="Blue"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label>Payee: </label>
                                                <asp:Label ID="lblPayee" runat="server" Text="" ForeColor="Blue"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label>Requester ID: </label>
                                                <asp:Label ID="lblRequester" runat="server" Text="" ForeColor="Blue"></asp:Label>
                                            </div>
                                        </div>       
                                        </div>
                                        <div class="form-row">
                                         <div class="col-md-4">
                                             <div class="form-group">
                                                 <label>Department: </label>
                                                 <asp:Label ID="lblDepartment" runat="server" Text="" ForeColor="Blue"></asp:Label>
                                             </div>
                                         </div>                                      

                                          <div class="col-md-4">
                                                 <div class="form-group">
                                                     <label>Travel Type</label>
                                                      <asp:DropDownList ID="ddlTravelType" runat="server" CssClass="form-control">
                                                          
                                                           <asp:ListItem Value="">Select an option</asp:ListItem>
                                                            <asp:ListItem Value="0">Local</asp:ListItem>
                                                            <asp:ListItem Value="1">International</asp:ListItem>

                                                        </asp:DropDownList>
                                                   
                                                 </div>
                                             </div>
                                             <div class="col-md-12">
                                                <div class="form-group">
                                                    <label>Purpose</label>
                                                    <asp:TextBox ID="txtPurpose" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                </div>
                                            </div>
                                            </div>
                                       
                                    
                                    <div class="form-row">
                                        <div class="col-md-12">
                                      
                                        <div class="box-footer clearfix">
                                            <asp:LinkButton ID="lbtnNext" runat="server" CssClass="btn btn-sm btn-success btn-flat pull-right" OnClick="lbtnNext_Click">
                                                <i class="fa fa-send-o"></i> Next
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lbtnBack" CssClass="btn btn-sm btn-warning btn-flat pull-left" runat="server" OnClick="lbtnBack_Click">
                                                <i class="fa fa-backward"></i> Back
                                            </asp:LinkButton>
                                        </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>
                        <asp:View ID="View2" runat="server">
                            <div class="panel panel-warning">
                                <div class="panel-heading">
                                    <h3 class="panel-title">Travel Request Lines</h3>
                                </div>
                                <div class="panel-body">
                                    <div id="newLines" runat="server" visible="false">
                                        <asp:LinkButton ID="lbnClose" ToolTip="Close Lines" CssClass="pull-right text-danger" runat="server" OnClick="lbnClose_Click">
                                            <i class="fa fa-minus-circle"></i> Close lines
                                        </asp:LinkButton>
                                        <table class="table table-hover">
                                            <thead>
                                                <tr>
                                                    <th>Employee/Trustee</th>
                                                    <th>Staff No.</th>
                                                  
                                                    <th>Destination Type</th>
                                                    <th>Destination</th>
                                                    <th>Include Accomodation</th>
                                                    <th>Start Date</th>
                                                    <th>No. of Days</th>
                                                    <th>End Date</th>
                                                  <%--  <th>Tax Code</th>--%>

                                                    
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        
                                                      <asp:DropDownList ID="ddlEmployeeTrustee" runat="server" CssClass="form-control"  AutoPostBack="true"  OnSelectedIndexChanged="ddlStaffNo_SelectedIndexChanged">
                                                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                                                        <asp:ListItem Value="1">Employee</asp:ListItem>
                                                        <asp:ListItem Value="2">Trustee</asp:ListItem>
                                                    </asp:DropDownList>
                          
                                                     </td>

                                                   
                                                     <td>

                                                           <asp:DropDownList ID="ddlStaffNo" CssClass="form-control" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="ddlDestination_SelectedIndexChanged" ></asp:DropDownList>
                                                     </td>       

                                                       <td>
                                                             <asp:DropDownList ID="ddlDestinationType" runat="server" CssClass="form-control">
                                                                 <asp:ListItem>-- Select -- </asp:ListItem>
                                                                   <asp:ListItem Value="1">Local</asp:ListItem>
                                                                   <asp:ListItem Value="0">International</asp:ListItem>
        
        

                                                               </asp:DropDownList>
                                                         </td>

                                                    <td>
                                                         <asp:DropDownList ID="ddlDestination" CssClass="form-control select2" runat="server"></asp:DropDownList>
                                                     
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlAccomodation" runat="server" CssClass="form-control">
                                                            <asp:ListItem>-- Select -- </asp:ListItem>
                                                              <asp:ListItem Value="0">Yes</asp:ListItem>
                                                              <asp:ListItem Value="1">No</asp:ListItem>
                                                           
        

                                                          </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDateOfTravel" runat="server" CssClass="form-control" TextMode="Date" required="true" onchange="calculateReturnDate()" onfocus="setMinDate()"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                         <asp:TextBox ID="txtNoOfDays" runat="server" CssClass="form-control" placeholder="Enter number of days" required="true" onchange="calculateReturnDate()"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Expected Return Date: "></asp:Label>
                                                         <asp:Label ID="lblReturnDate" runat="server" Font-Bold="True" ForeColor="#FF6600" CssClass="spLabel">&nbsp;</asp:Label>
                                                    </td>
                                                    
                                                    
                                                    <td>
                                                        <asp:Button ID="btnLine" CssClass="btn btn-primary pull-right" runat="server" Text="Add" OnClick="btnLine_Click" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                        <%--            <asp:LinkButton ID="lbnAddLine" ToolTip="Add Passenger" CssClass="pull-right text-info" runat="server" OnClick="lbnAddLine_Click">

                                        <i class="fa fa-plus-circle"></i> Add Line
                                    </asp:LinkButton>--%>
                                     <asp:LinkButton ID="lbtnAddLine" ToolTip="Add New Lines" class="pull-right text-info" runat="server" OnClick="lbtnAddLine_Click"><i class="fa fa-plus-circle"></i> Add Line</asp:LinkButton>
                                    <asp:GridView ID="gvLines" AutoGenerateColumns="false" DataKeyNames="StaffNo" CssClass="table table-responsive no-padding table-bordered table-hover" runat="server"
                                        AllowSorting="True" AllowPaging="true" ShowFooter="true" PageSize="15">
                                        <Columns>
                                            <asp:TemplateField HeaderText="#No">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Trustee/Employee" HeaderText="Trustee/Employee" />
                                            <asp:BoundField DataField="StaffNo" HeaderText="Staff No" />
                                            <asp:BoundField DataField="Payee" HeaderText="Payee" />
                                            <asp:BoundField DataField="DestinationType" HeaderText="Destination Type" />
                                            <asp:BoundField DataField="Destination" HeaderText="Destination " />
                                             <asp:BoundField DataField="IncludeAccomodation" HeaderText="Include Accomodation" />
                                             <asp:BoundField DataField="StartDate" HeaderText="Start Date " />
                                            <asp:BoundField DataField="NoofDays" HeaderText="No. of Days " />
                                            <asp:BoundField DataField="EndDate" HeaderText="End Date" />
                                            <asp:BoundField DataField="Description" HeaderText="Description" />
                                            <asp:BoundField DataField="TaxCode" HeaderText="Tax Code" />
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbtnCancel" CssClass="label label-danger" runat="server" ToolTip="Click to Remove line" OnClick="cancel" CommandArgument='<%# Eval("Line No_") %>'>
                                                        <i class="fa fa-remove"></i> Remove
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <span style="color: red">No Records</span>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                    <table class="table table-hover">
                                        <thead>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:LinkButton ID="LinkButton1" CssClass="btn btn-sm btn-warning btn-flat pull-left" runat="server" OnClick="lbtnBack_Click">
                                                        <i class="fa fa-backward"></i> Back
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lbtnSubmit" CssClass="btn btn-success btn-flat btn-sm pull-right" runat="server" OnClick="lbtnSubmit_Click">
                                                        <i class="fa fa-check-circle"></i> Submit Requisition
                                                    </asp:LinkButton>
                                                </td>
                                            </tr>
                                        </thead>
                                    </table>
                                </div>
                            </div>
                        </asp:View>
                    </asp:MultiView>
                </div>
            </div>
        </section>
    </div>

    <script>
        function calculateReturnDate() {
            var dateOfTravel = document.getElementById('<%= txtDateOfTravel.ClientID %>').value;
            var noOfDays = document.getElementById('<%= txtNoOfDays.ClientID %>').value;
            if (dateOfTravel && noOfDays) {
                var startDate = new Date(dateOfTravel);
                startDate.setDate(startDate.getDate() + parseInt(noOfDays));
                document.getElementById('<%= lblReturnDate.ClientID %>').textContent = startDate.toLocaleDateString();
            }
        }

        function setMinDate() {
            var dateOfTravel = document.getElementById('<%= txtDateOfTravel.ClientID %>');
            var today = new Date().toISOString().split('T')[0];
            dateOfTravel.setAttribute('min', today);
        }
        function fillPassengerDetails() {
            var ddl = document.getElementById('<%= ddlStaffNo.ClientID %>');
           var selectedValue = ddl.options[ddl.selectedIndex].text;

           // Split the selected value by " - "
           var details = selectedValue.split(" - ");

           
        }


    </script>
</asp:Content>

