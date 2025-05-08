﻿<%@ Page Title="Imprest Accounting Lines" Language="C#" MasterPageFile="~/Layout/Main.Master" AutoEventWireup="true" CodeBehind="ImprestAccountingLines.aspx.cs" Inherits="TELPOSTAStaff.pages.ImprestAccountingLines" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Main1" runat="server">
    <div class="content-wrapper">
        <section class="content-header">
            <h1>Imprest Accounting Lines
            </h1>
            <ol class="breadcrumb">
                <li><a href="Dashboard.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
                <li class="active">Imprest Accounting Lines</li>
            </ol>
        </section>

        <section class="content">
            <div class="row">
                <div class="col-md-12">
                    <div class="box box-warning box-solid">
                        <div class="box-header with-border">
                            <h3 class="box-title"><i class="fa fa-diamond"></i>&nbsp;Imprest Accounting Lines</h3>
                            <div class="box-tools pull-right">
                                <button type="button" class="btn btn-box-tool" data-widget="collapse">
                                    <i class="fa fa-minus"></i>
                                </button>
                                <button type="button" class="btn btn-box-tool" data-widget="remove"><i class="fa fa-times"></i></button>
                            </div>
                        </div>
                        <div class="box-body">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Posted Imprests</label>
                                        <asp:DropDownList ID="ddlPostedImprest" CssClass="form-control select2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPostedImprest_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Responsibility Center</label>
                                        <asp:DropDownList ID="ddlResponsibilityCenter" CssClass="form-control select2" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                             
                            </div>
                            <%--<div class="row">
                                <div class="col-md-12">
                                    <h3>Upload supporting documents</h3>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Upload document</label>
                                        <asp:FileUpload ID="fuImprestDocs" CssClass="form-control" runat="server" />
                                        <br />
                                        <asp:LinkButton ID="lbtnUpload" runat="server" CssClass="btn btn-primary" OnClick="lbtnUpload_Click"><i class="fa fa-upload"></i>&nbsp;Upload</asp:LinkButton>
                                    </div>
                                </div>
                            </div>--%>
                            <div class="row">
                                <div class="col-md-12">
                                    <h3>Imprest Surrender Details</h3>
                                </div>
                                <div class="col-md-12">
                                    <asp:GridView ID="gvLines" AutoGenerateColumns="false" DataKeyNames="Advance Type" class="table table-responsive no-padding table-bordered table-hover" runat="server"
                                        AllowSorting="True" AllowPaging="true" ShowFooter="true" PageSize="5">
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="#No" SortExpression="">
                                                <HeaderStyle Width="30px" />
                                                <ItemTemplate>
                                                    <%# string.Format("{0}",Container.DataItemIndex + 1 +".") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                           <%-- <asp:BoundField DataField="Advance Type" HeaderText="Advance Type" />--%>
                                            <asp:BoundField DataField="Account No" HeaderText="Account No" />
                                            <asp:BoundField DataField="Account Name" HeaderText="Account Name" />
                                            <asp:BoundField DataField="Amount" HeaderText="Amount" />
                                            <asp:TemplateField HeaderText="Receipt No">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="ddlReceipts" CssClass="form-control select2" OnSelectedIndexChanged="ddlReceipts_SelectedIndexChanged" AutoPostBack="true" runat="server">
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField> 
                                             <%--<asp:TemplateField HeaderText="Actual Amount Spent" SortExpression="">
                                             <ItemTemplate>
                                                 <asp:TextBox ID="txtActualAmount" runat="server" Text="" BorderColor="Blue" BorderStyle="Ridge" BorderWidth="2px" ForeColor="Blue" Width="100px" />
                                             </ItemTemplate>
                                             <ItemStyle Font-Bold="True" ForeColor="Green"></ItemStyle>
                                         </asp:TemplateField>
                                         <asp:TemplateField HeaderText="Cash Amount Returned">
                                             <ItemTemplate>
                                                 <asp:TextBox ID="txtAmountReturned" runat="server" Text="" BorderColor="Blue" BorderStyle="Ridge" BorderWidth="2px" ForeColor="Blue" Width="100px" />
                                             </ItemTemplate>
                                             <ItemStyle Font-Bold="True" ForeColor="Green"></ItemStyle>
                                         </asp:TemplateField>--%>

                                           <asp:TemplateField HeaderText="Actual Amount Spent">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtActualAmount" runat="server" 
                                                        CssClass="actualAmount form-control"
                                                        onkeyup="calculateCashReturned(this)"
                                                        BorderColor="Blue" BorderStyle="Ridge" BorderWidth="2px" ForeColor="Blue" Width="100px" />
                                                </ItemTemplate>
                                                <ItemStyle Font-Bold="True" ForeColor="Green"></ItemStyle>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Cash Amount Returned">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtAmountReturned" runat="server"
                                                        CssClass="cashReturned form-control"
                                                        ReadOnly="false"
                                                        BorderColor="Blue" BorderStyle="Ridge" BorderWidth="2px" ForeColor="Blue" Width="100px" />
                                                </ItemTemplate>
                                                <ItemStyle Font-Bold="True" ForeColor="Green"></ItemStyle>
                                            </asp:TemplateField>


                                         </Columns>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <EmptyDataTemplate>
                                            <span style="color: red">No Recods</span>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </div>
                            </div>

                           <%-- <div class="row">
                                <div class="col-md-12">
                                    <h3>Document Attachments</h3>
                                </div>
                                <div class="col-md-12">
                                    <asp:GridView ID="gvAttachments" AutoGenerateColumns="false" DataKeyNames="No_" class="table table-responsive no-padding table-bordered table-hover" runat="server"
                                        AllowSorting="True" AllowPaging="true" ShowFooter="true" PageSize="5">
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="#No" SortExpression="">
                                                <HeaderStyle Width="30px" />
                                                <ItemTemplate>
                                                    <%# string.Format("{0}",Container.DataItemIndex + 1 +".") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="No_" HeaderText="Document No" />
                                            <asp:BoundField DataField="File Name" HeaderText="File Name" />
                                            <asp:BoundField DataField="$systemCreatedAt" HeaderText="Date Uploaded" />
                                            <asp:TemplateField HeaderText="Action" SortExpression="" HeaderStyle-HorizontalAlign="Left">
                                                <ItemStyle Width="110px" HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lbtnRemoveAttach" CssClass="label label-danger" runat="server" ToolTip="Click to Remove line" OnClick="lbtnRemoveAttach_Click" OnClientClick="return confirm('Are you sure you want to delete this line?')" CommandArgument='<%# Eval("$systemId") %>'><i class="fa fa-remove"></i> Remove</asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <EmptyDataTemplate>
                                            <span style="color: red">No Recods</span>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </div>
                            </div>--%>
                            <div class="row">
                                <div class="col-md-12">
                                    <a href="ImprestAccountingListing.aspx" class="btn btn-warning pull-left"><i class="fa fa-backward"></i>&nbsp;Back</a>
                                    <asp:LinkButton ID="lbtnSubmit" runat="server" CssClass="btn btn-success pull-right" OnClick="lbtnSubmit_Click"><i class="fa fa-paper-plane"></i>&nbsp;Submit</asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>


   <script>
       function calculateCashReturned(actualInput) {
           var row = actualInput.closest('tr');

           // Get the 'Amount' value and ensure it’s parsed correctly
           var amountCell = row.querySelectorAll('td')[3]; // Adjust index if necessary
           var amount = parseFloat(amountCell.textContent.replace(/[^0-9.-]+/g, "")) || 0;
           var actual = parseFloat(actualInput.value) || 0;

           // Calculate returned amount, which can be negative
           var returned = amount - actual;

           // Set the 'Cash Amount Returned' field
           var returnedBox = row.querySelector('.cashReturned');
           returnedBox.value = returned.toFixed(2); // This will display negative values correctly
           __doPostBack('<%= gvLines.ClientID %>', '');
       }


   </script>




</asp:Content>


