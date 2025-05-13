<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/Main.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="TELPOSTAStaff.pages.MyProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Main1" runat="server">

     <div class="content-wrapper">
     <!-- Content Header (Page header) -->
     <section class="content-header">
         <h1>My Profile</h1>
         <ol class="breadcrumb">
             <li><a href="Dashboard.aspx"><i class="fa fa-dashboard"></i> Home</a></li>
             <li class="active">My Profile</li>
         </ol>
     </section>

     <!-- Main content -->
     <section class="content">
         <div class="row">
             <div class="col-md-12">
                 <!-- Profile Information -->
                 <div class="box box-info box-solid">
                     <div class="box-header with-border">
                         <h3 class="box-title"><i class="fa fa-user"></i> Employee Profile</h3>
                         
                     </div>
                         <div class="box-tools pull-right">
                             <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i></button>
                             <button type="button" class="btn btn-box-tool" data-widget="remove"><i class="fa fa-remove"></i></button>
                            
                            <%-- <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-primary" OnClick="btnEdit_Click"/>--%>
                                
                         </div>
                     </div>
                     <!-- /.box-header -->
  
                     <div class="box-body">

                         <div class="row"> 
                              <div class="col-md-4">
                                  <div class="form-group">
                                      <label>Employee Number: </label>
                                       <asp:Label ID="lblEmpno" runat="server" Text="" ForeColor="Blue" Font-Bold="true"></asp:Label>
                                  </div>
                              </div>
                              <div class="col-md-4">
                                  <div class="form-group">
                                      <label>ID Number: </label>
                                       <asp:Label ID="lblID" runat="server" Text="" ForeColor="Blue" Font-Bold="true"></asp:Label>
                                  </div>
                              </div>
                             <!-- Date of Birth -->
                             <div class="col-md-4">
                                 <div class="form-group">
                                     <label>Date of Birth</label>
                                     <%--<asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control"></asp:TextBox>--%>
                                     <asp:Label ID="lblDoB" runat="server" Text="" ForeColor="Blue" Font-Bold="true"></asp:Label>
                                 </div>
                             </div>
                         </div>
                         <div class="row">
                               <div class="col-md-4">
                                   <div class="form-group">
                                       <label>Title: </label>
                                      <%-- "MR.","MRS.","MISS.","MS.","DR.","  ",CC,"ASSCOC.PROF","PROF.",PROF--%>

                                           <asp:DropDownList ID="ddlTitle" runat="server" CssClass="form-control" >
                                               <asp:ListItem Text=" " Value=" "></asp:ListItem>
                                               <asp:ListItem Text="MR" Value="0 "></asp:ListItem>
                                               <asp:ListItem Text="MRS" Value="1"></asp:ListItem>
                                               <asp:ListItem Text="MISS" Value="2"></asp:ListItem>
                                               <asp:ListItem Text="MS" Value="3"></asp:ListItem>
                                               <asp:ListItem Text="DR" Value="4"></asp:ListItem>
                                               <asp:ListItem Text=" " Value="5"></asp:ListItem>
                                               <asp:ListItem Text="CC" Value="6"></asp:ListItem>
                                               <asp:ListItem Text="ASSCOC.PROF" Value="7"></asp:ListItem>
                                               <asp:ListItem Text="PROF." Value="4"></asp:ListItem>
                                               
                                           </asp:DropDownList>
                                   </div>
                               </div>

                             <!-- First Name -->
                             <div class="col-md-4">
                                 <div class="form-group">
                                     <label>First Name</label>
                                     <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                 </div>
                             </div>
                             <!-- Middle Name -->
                             <div class="col-md-4">
                                 <div class="form-group">
                                     <label>Middle Name</label>
                                     <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                 </div>
                             </div>
                            
                         </div>
                         <div class="row">
                              <!-- Last Name -->
                              <div class="col-md-4">
                                  <div class="form-group">
                                      <label>Last Name</label>
                                      <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" ></asp:TextBox>
                                  </div>
                              </div>
                             <!-- Gender -->
                             <div class="col-md-4">
                                 <div class="form-group">
                                     <label>Gender</label>
                                     <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control" >
                                         <asp:ListItem Text=" " Value="0 "></asp:ListItem>
                                         <asp:ListItem Text="Male" Value="1"></asp:ListItem>
                                         <asp:ListItem Text="Female" Value="2"></asp:ListItem>
                                     </asp:DropDownList>
                                 </div>
                             </div>
                             
                             <!-- Marital Status -->
                             <div class="col-md-4">
                                 <div class="form-group">
                                     <label>Marital Status</label>
                                     <asp:DropDownList ID="ddlMaritalStatus" runat="server" CssClass="form-control" >
                                         <asp:ListItem Text=" " Value="0 "></asp:ListItem>
                                         <asp:ListItem Text="Single" Value="1"></asp:ListItem>
                                         <asp:ListItem Text="Married" Value="2"></asp:ListItem>
                                     </asp:DropDownList>
                                 </div>
                             </div>
                                 
                         </div>
                         <div class="row">
                             <div class="col-md-4">
                                 <div class="form-group">
                                     <label>Phone Number</label>
                                     <asp:TextBox ID="txtPhoneNo" runat="server" CssClass="form-control" ></asp:TextBox>
                                 </div>
                             </div>
                             <!-- Religion -->
                             <div class="col-md-4">
                                 <div class="form-group">
                                     <label>Religion</label>
                                     <asp:TextBox ID="txtReligion" runat="server" CssClass="form-control" ></asp:TextBox>
                                 </div>
                             </div>
                             <div class="col-md-4">
                                 <div class="form-group">
                                     <label>Tribe</label>
                                     <asp:TextBox ID="txtTribe" runat="server" CssClass="form-control" ></asp:TextBox>
                                 </div>
                             </div>
                            
                         </div>
                          <div class="row">
                          <div class="col-md-4">
                              <div class="form-group">
                                  <label>Email</label>
                                  <asp:TextBox ID="txtEmail" type="email" runat="server" CssClass="form-control" ></asp:TextBox>
                              </div>
                          </div>
                             
                          <div class="col-md-4">
                              <div class="form-group">
                                  <label>County</label>
                                  <asp:TextBox ID="txtCounty" runat="server" CssClass="form-control"></asp:TextBox>
                              </div>
                          </div>
                          <div class="col-md-4">
                              <div class="form-group">
                                  <label>Postal Address</label>
                                  <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" ></asp:TextBox>
                              </div>
                          </div>
                          </div>
                     </div>
                 <div>
                     <asp:Button ID="btnSave" runat="server" Text="Save"  class="btn btn-sm btn-success btn-flat pull-right" OnClick="btnSave_Click"/>

                 </div>
                 </div>
             </section>
     </div>


</asp:Content>
