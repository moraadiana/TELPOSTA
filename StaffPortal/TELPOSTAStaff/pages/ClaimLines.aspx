﻿<%@ Page Title="Claim Lines" Language="C#" MasterPageFile="~/Layout/Main.Master" AutoEventWireup="true" CodeBehind="ClaimLines.aspx.cs" Inherits="TELPOSTAStaff.pages.ClaimLines" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Main1" runat="server">
    <div class="content-wrapper">
        <section class="content-header">
            <h1>Claim Requisition</h1>
            <ol class="breadcrumb">
                <li><a href="Dashboard.aspx"><i class="fa fa-dashboard"></i>Home</a></li>
                <li class="active">Claim Requisition</li>
            </ol>
        </section>

        <section class="content">
            <div class="row">
                <div class="col-md-12">
                    <asp:MultiView ID="MultiView1" runat="server">
                        <asp:View ID="vwHeader" runat="server">
                            <div class="box box-info box-solid">
                                <div class="box-header with-border">
                                    <h3 class="box-title">New Claim Requisition</h3>
                                </div>
                                <div class="box-body">
                                    <div class="row">
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
                                      <!--  <div class="col-md-4">
                                            <div class="form-group">
                                                <label>Directorate: </label>
                                                <asp:Label ID="lblDirectorate" runat="server" Text="" ForeColor="Blue"></asp:Label>
                                            </div>
                                        </div>-->  
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label>Department: </label>
                                                <asp:Label ID="lblDepartment" runat="server" Text="" ForeColor="Blue"></asp:Label>
                                            </div>
                                        </div>                                      
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label>Responsibility Center</label>
                                                <asp:DropDownList ID="ddlResponsibilityCenter" CssClass="form-control select2" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <%-- <div class="col-md-4">
                                             <div class="form-group">
                                                 <label>Responsibility Center: </label>
                                                 <asp:Label ID="lblResCenter" runat="server" Text="" ForeColor="Blue" Font-Bold="true"></asp:Label>
                                             </div>
                                        </div>--%>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <label>Purpose</label>
                                                <asp:TextBox ID="txtPurpose" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <asp:LinkButton ID="lbtnSubmit" runat="server" CssClass="btn btn-primary pull-right" OnClick="lbtnSubmit_Click"><i class="fa fa-paper-plane"></i>&nbsp;Submit</asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>

                        <asp:View ID="vwLines" runat="server">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="box box-info box-solid">
                                        <div class="box-header">
                                            <h3 class="box-title">Claim Lines &mdash;
                                                <asp:Label ID="lblClaimNo" runat="server" Text=""></asp:Label></h3>
                                        </div>
                                        <div class="box-body">
                                            <div id="newLines" runat="server" visible="false">
                                                <asp:LinkButton ID="lbtnClose" ToolTip="Close Lines" class="pull-right" runat="server" OnClick="lbtnClose_Click"><i class="fa fa-minus-circle"></i> Close lines</asp:LinkButton>
                                                <table class="table table-hover">
                                                    <thead>
                                                        <tr>
                                                            <th>No:</th>
                                                            <th>Advance Type</th>
                                                            <th>Amount:</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <th>
                                                                <asp:Label ID="lblLNo" runat="server" Text="Label"></asp:Label></th>
                                                            <td>
                                                                <asp:DropDownList ID="ddlAdvancType" runat="server" CssClass="form-control select2"></asp:DropDownList></td>
                                                                 <%--<asp:Label ID="lblAdvanceType" runat="server" Text="Label"></asp:Label></td>--%>
                                                            <td>
                                                                <asp:TextBox ID="txtAmnt" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <th></th>
                                                            <td></td>
                                                            <td>
                                                                <asp:Button ID="btnLine" class="btn btn-primary pull-right" runat="server" Text="Add" OnClick="btnLine_Click" /></td>
                                                            <td></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>

                                            <asp:LinkButton ID="lbtnAddLine" ToolTip="Add New Lines" class="pull-right text-info" runat="server" OnClick="lbtnAddLine_Click"><i class="fa fa-plus-circle"></i> Add Line</asp:LinkButton>

                                            <div id="attachments" runat="server" visible="false">
                                                <div class="row">
                                                    <div class="col-md-4">
                                                        <label>Attach supporting documents</label>
                                                        <asp:FileUpload ID="fuClaimDocs" CssClass="form-control" ToolTip="Attach documents with .pdf, .png, .jpg and .jpeg extensions only" runat="server" />
                                                        <br />
                                                        <asp:LinkButton ID="lbtnAttach" runat="server" CssClass="btn btn-primary" OnClick="lbtnAttach_Click"><i class="fa fa-upload"></i>&nbsp;Upload</asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />

                                            <h4>Claim Lines</h4>
                                            <asp:GridView ID="gvLines" AutoGenerateColumns="false" DataKeyNames="No" class="table table-responsive no-padding table-bordered table-hover" runat="server"
                                                AllowSorting="True" AllowPaging="true" ShowFooter="true" PageSize="5">
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="#No" SortExpression="">
                                                        <HeaderStyle Width="30px" />
                                                        <ItemTemplate>
                                                            <%# string.Format("{0}",Container.DataItemIndex + 1 +".") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="No" HeaderText="Number" />
                                                    <asp:BoundField DataField="Advance Type" HeaderText="Advance Type" />
                                                    <asp:BoundField DataField="varAmount" HeaderText="Amount" />
                                                    <asp:BoundField DataField="Due Date" HeaderText="Due Date" />
                                                    <asp:TemplateField HeaderText="Action" SortExpression="" HeaderStyle-HorizontalAlign="Left">
                                                        <ItemStyle Width="110px" HorizontalAlign="Left" />
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lbtnRemove" CssClass="label label-danger" runat="server" ToolTip="Click to Remove line" OnClick="lbtnRemove_Click" OnClientClick="return confirm('Are you sure you want to delete this line?')" CommandArgument='<%# Eval("Line No_") %>'><i class="fa fa-remove"></i> Remove</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <FooterStyle HorizontalAlign="Center" />
                                                <EmptyDataTemplate>
                                                    <span style="color: red">No Recods</span>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                            <br />
                                             <div class="row">
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
                                            </div>
                                           
                                            <table class="table table-hover">
                                                <thead>
                                                    <tr>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                        <th>&nbsp;</th>
                                                        <th>
                                                            <asp:Button ID="btnApproval" CssClass="btn btn-success pull-right" runat="server" Text="Send Approval" OnClick="btnApproval_Click" />&nbsp;
                                        <asp:Button ID="btnCancellApproval" runat="server" CssClass="btn btn-danger pull-right" OnClick="btnCancellApproval_Click" Text="Cancel Approval Request" />
                                                        </th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:View>
                    </asp:MultiView>
                </div>
            </div>
        </section>
    </div>
</asp:Content>
