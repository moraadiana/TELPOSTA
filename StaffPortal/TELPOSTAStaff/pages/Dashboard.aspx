<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Layout/Main.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="TELPOSTAStaff.pages.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Main1" runat="server">
    <div class="content-wrapper">
        <section class="content-header">
            <h1>Dashboard</h1>
            <ol class="breadcrumb">
                <li><a href="#"><i class="fa fa-dashboard"></i>Home</a></li>
                <li class="active">Dashboard</li>
            </ol>
        </section>
        
        <section class="content">

            
            <%--<div class="row">

                <div class="col-md-12" style="display: none">
                    <iframe src="10.10.1.140:8080/BC200/?company=ARA&page=22&showribbon=0&shownavigation=0&showuiparts=0" width="100%" height="500px" webserver=""></iframe>
                </div>
                <div class="col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box bg-green">
                        <span class="info-box-icon"><i class="ion ion-ios-pricetag-outline"></i></span>

                        <div class="info-box-content">
                            <span class="info-box-text">Leave Approvals</span>
                            
                            <span class="info-box-number">0</span>

                            <div class="progress">
                                <div class="progress-bar" style="width: 50%"></div>
                            </div>
                            <a href="#" class="small-box-footer white" style="color: white">View Requests <i class="fa fa-arrow-circle-right"></i></a>
                           
                        </div>
                        
                    </div>
                    
                </div>
                
                <div class="col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box bg-yellow">
                        <span class="info-box-icon"><i class="ion ion-android-list"></i></span>

                        <div class="info-box-content">
                            <span class="info-box-text">Imprest Approvals</span>
                            
                            <span class="info-box-number">0</span>

                            <div class="progress">
                                <div class="progress-bar" style="width: 20%"></div>
                            </div>
                            <a href="#" class="small-box-footer" style="color: white">View Requests <i class="fa fa-arrow-circle-right"></i></a>
                        </div>
                       
                    </div>
                    
                </div>
              

                <div class="clearfix visible-sm-block"></div>

                <div class="col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box bg-red">
                        <span class="info-box-icon"><i class="ion ion-jet"></i></span>

                        <div class="info-box-content">
                            <span class="info-box-text">Claim Approvals</span>
                            
                            <span class="info-box-number">0</span>

                            <div class="progress">
                                <div class="progress-bar" style="width: 70%"></div>
                            </div>
                            <a href="#" class="small-box-footer white" style="color: white">View Requests <i class="fa fa-arrow-circle-right"></i></a>
                        </div>
                       
                    </div>
                   
                </div>
               
                <div class="col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box bg-aqua">
                        <span class="info-box-icon"><i class="ion ion-ios-email-outline"></i></span>

                        <div class="info-box-content">
                            <span class="info-box-text">Stores Approvals</span>
                           
                            <span class="info-box-number">0</span>

                            <div class="progress">
                                <div class="progress-bar" style="width: 40%"></div>
                            </div>
                            <a href="#" class="small-box-footer white" style="color: white">View Requests <i class="fa fa-arrow-circle-right"></i></a>
                        </div>
                        
                    </div>
                   
                </div>
           
            </div> --%>
            

            <div class="row">
                <div class="col-md-12">
                    <div class="box">
                        <div class="box-header with-border">
                            <h3 class="box-title">User Profile</h3>

                            <div class="box-tools pull-right">
                                <button type="button" class="btn btn-box-tool" data-widget="collapse">
                                    <i class="fa fa-minus"></i>
                                </button>
                                <div class="btn-group">

                                    <ul class="dropdown-menu" role="menu">
                                    </ul>
                                </div>
                                <button type="button" class="btn btn-box-tool" data-widget="remove"><i class="fa fa-times"></i></button>
                            </div>
                        </div>
                        <!-- /.box-header -->
                        <div class="box-body">
                            <div class="row">
                                <!-- /.col -->
                                <div class="col-md-4">
                                    <p class="text-center">
                                        <strong>Profile Photo </strong>
                                    </p>
                                    <div class="box-body box-profile">
                                        <asp:Image ID="ImgProfileDefault" class="profile-user-img img-responsive img-circle" runat="server" Height="250px" Width="200px" Visible="false" alt="User profile picture" />
                                        <asp:Image ID="ImgProfilePic" class="profile-user-img img-responsive img-circle" runat="server" Height="250px" Width="200px" alt="User profile picture" />

                                        <h3 class="profile-username text-center">
                                            <asp:Label ID="lblTitle" runat="server" Text="" Visible="false"></asp:Label>
                                            <asp:Label ID="lblStaffName" runat="server" Text=""></asp:Label></h3>
                                        <p class="text-muted text-center">
                                            <asp:Label ID="lblDesignation" runat="server" Text=""></asp:Label>
                                        </p>
                                        
                                    </div>
                                    <div class="progress-group">
                                        <span class="progress-text"></span>
                                        <span class="progress-number"><b></b></span>

                                        <div class="progress sm">
                                            <div class="progress-bar progress-bar-success" style="width: 100%"></div>
                                        </div>
                                       <div style="text-align: center; margin-top: 15px;">
                                          <button
                                            type="button"
                                            class="btn btn-primary"
                                            data-toggle="modal"
                                            data-target="#uploadModal">
                                            Change Profile Photo
                                          </button>
                                        </div>

                                    </div>
                                </div>
                                
                                <!-- Modal -->
                                <div
                                  class="modal fade"
                                  id="uploadModal"
                                  tabindex="-1"
                                  role="dialog"
                                  aria-labelledby="uploadModalLabel"
                                  aria-hidden="true">
                                  <div class="modal-dialog" role="document">
                                    <asp:Panel runat="server" CssClass="modal-content">
                                      <div class="modal-header">
                                        <h5 class="modal-title" id="uploadModalLabel">Change Profile Photo</h5>
                                        <button type="button" class="close"data-dismiss="modal" aria-label="Close">
                                          <span aria-hidden="true">&times;</span>
                                        </button>
                                      </div>
                                  
                                        <div class="modal-body">
                                            <asp:FileUpload ID="fuProfilePic" runat="server" CssClass="form-control" onchange="previewImage(event)" />
                                            <br />
                                            <img id="imgPreview" src="#" alt="Image Preview" style="display:none; width: 200px; height: 200px;" class="img-circle img-responsive center-block" />
                                        </div>
                                      <div class="modal-footer">
                                        <asp:Button ID="btnUpload" runat="server" CssClass="btn btn-primary"  Text="Upload"  OnClick="btnUpload_Click" />
                                        <button type="button"class="btn btn-secondary" data-dismiss="modal">
                                          Close
                                        </button>
                                      </div>
                                    </asp:Panel>
                                  </div>
                                </div>


                                <!-- /.col -->
                                <div class="col-md-8">
                                    <p class="text-center">
                                        <strong>Personal Information</strong>
                                    </p>

                                    <ul class="list-group list-group-unbordered">
                                        <li class="list-group-item">
                                            <b>Employee No:</b> <a class="pull-right">
                                                <asp:Label ID="lblEmployeeNo" runat="server" Text=""></asp:Label></a>
                                        </li>
                                        <li class="list-group-item">
                                            <b>Gender:</b> <a class="pull-right">
                                                <asp:Label ID="lblGender" runat="server" Text=""></asp:Label></a>
                                        </li>
                                        <li class="list-group-item">
                                            <b>ID Number:</b> <a class="pull-right">
                                                <asp:Label ID="lblIDNo" runat="server" Text=""></asp:Label></a>
                                        </li>
                                        <li class="list-group-item">
                                            <b>Email Address:</b> <a class="pull-right">
                                                <asp:Label ID="lblEmail" runat="server" Text=""></asp:Label></a>
                                        </li>
                                        <li class="list-group-item">
                                            <b>Company E-Mail:</b> <a class="pull-right">
                                                <asp:Label ID="lblEmailCompany" runat="server" Text=""></asp:Label></a>
                                        </li>
                                        <li class="list-group-item">
                                            <b>Phone Number:</b> <a class="pull-right">
                                                <asp:Label ID="lblPhoneNo" runat="server" Text=""></asp:Label></a>
                                        </li>
                                        <li class="list-group-item">
                                            <b>Citizenship:</b> <a class="pull-right">
                                                <asp:Label ID="lblCitizenship" runat="server" Text=""></asp:Label></a>
                                        </li>
                                        <li class="list-group-item">
                                            <b>Postal Address:</b> <a class="pull-right">
                                                <asp:Label ID="lblPostalCode" runat="server" Text=""></asp:Label>
                                                <asp:Label ID="lblPostalAddress" runat="server" Text=""></asp:Label></a>
                                        </li>
                                        <li class="list-group-item">
                                                <b>Days To Retire:</b> <a class="pull-right">
                                                <asp:Label ID="lblDaysToRtr" runat="server" Text=""></asp:Label></a>
                                                
                                        </li>
   
                                    </ul>
                                    <div class="box-footer clearfix">
                                    </div>
                                </div>
                            </div>
                            <!-- /.row -->
                        </div>
                    </div>
                    <!-- /.box -->
                </div>
                <!-- /.col -->
            </div>
        </section>
        <!-- /.content -->
    </div>
    <script type="text/javascript">
        function previewImage(event) {
            var input = event.target;
            var preview = document.getElementById('imgPreview');

            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    preview.src = e.target.result;
                    preview.style.display = "block";
                };
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>

</asp:Content>

