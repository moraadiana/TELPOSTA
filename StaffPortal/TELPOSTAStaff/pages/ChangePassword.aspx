<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Layout/Main.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="TELPOSTAStaff.pages.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Main1" runat="Server">
    <div class="app-login">
        
        <div class="panel panel-success text-center box shadow-5 animated fadeInLeft b-r-4 p-a-20">
            <div class="panel-body">
                <%--<h4>Sign in to start your session</h4>--%>
                <asp:Label ID="Label1" runat="server" CssClass="label label-danger" ></asp:Label>
                <div class="text-left" role="form" action="#">
                    <asp:Label ID="LblError" runat="server" CssClass="label label-danger"></asp:Label>
                     <div class="form-group has-feedback">
                          <label for="inputEmail3" class="col-sm-6 control-label">Current Password:</label>
                        <asp:TextBox ID="txtOldPass" type="password" class="form-control" placeholder="Current Password" runat="server"></asp:TextBox>
                        <span class="form-control-feedback">
                            <i class="fa fa-fw fa-envelope"></i>
                        </span>
                    </div>
                    <div class="form-group has-feedback">
                         <label for="inputEmail3" class="col-sm-6 control-label">New Password:</label>
                        <asp:TextBox ID="TxtNewPass" type="password" class="form-control" placeholder="New Password" runat="server"></asp:TextBox>
                        <span class="form-control-feedback">
                            <i class="fa fa-fw fa-envelope"></i>
                        </span>
                    </div>
                      <div class="form-group has-feedback">
                           <label for="inputEmail3" class="col-sm-6 control-label">Confirm password:</label>
                        <asp:TextBox ID="TxtConfirmNewPass" type="password" class="form-control" placeholder="Confirm password" runat="server"></asp:TextBox>
                        <span class="form-control-feedback">
                            <i class="fa fa-fw fa-envelope"></i>
                        </span>
                    </div>
                    <asp:Button ID="BtnResetPass" class="btn btn-danger btn-block btn-flat" runat="server" Text="Reset Password" OnClick="BtnResetPass_Click" />
                 
                </div>
            </div>
        </div>
    </div>
    </asp:Content>
