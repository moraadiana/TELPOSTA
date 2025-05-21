<%@ Page Title="Verify OTP" Language="C#" MasterPageFile="~/Layout/Site.Master" AutoEventWireup="true" CodeBehind="VerifyOTP.aspx.cs" Inherits="TELPOSTAStaff.pages.VerifyOTP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="limiter">
        <div class="container-login100">
            <div class="wrap-login100">
                <!-- Logo Section -->
                <div class="login100-form-title img-rounded text-center">
    <img src="/images/logo.png" alt="Logo" class="img-fluid" style="max-width: 200px;" />
    <hr class="custom-hr" />
</div>

                <!-- Page Title -->
                <div class="login100-form-title text-center">
                    <h5 class="portal-title">TELPOSTA STAFF PORTAL</h5>
                </div>

                <!-- Login Panel -->
                <asp:Panel DefaultButton="LbtnLogin" ID="LoginPanel" runat="server">
                    <asp:Label ID="lblError" runat="server" CssClass="error-label"></asp:Label>

                    <!-- OTP Input -->
                    <div class="wrap-input100 validate-input" data-validate="Enter OTP">
                        <asp:TextBox CssClass="input100" runat="server" ID="txtOTP" Placeholder="Enter OTP"></asp:TextBox>
                        <span class="focus-input100"></span>
                    </div>

                    <!-- Login Button -->
                    <div class="container-login100-form-btn">
                        <asp:LinkButton ID="lbtnLogin" runat="server" OnClick="lbtnLogin_Click" CssClass="login-btn"><strong>LOGIN</strong></asp:LinkButton>
                    </div>

                    <!-- Back Link -->
                    <div class="text-center p-t-15">
                        <asp:LinkButton ID="lbtnForgot" runat="server" OnClick="lbtnBack_Click" CssClass="forgot-link">Back <i class="fa fa-arrow-circle-left"></i></asp:LinkButton>
                    </div>

                    <!-- Resend OTP -->
                    <div class="mt-3 text-center">
                        <p class="text-muted small mb-1">Didn't receive the OTP?</p>
                        <asp:LinkButton ID="resend" runat="server" OnClick="lbtnResendOtp_Click" CssClass="resend-link">Resend OTP <i class="fa fa-sync-alt"></i></asp:LinkButton>
                    </div>
                </asp:Panel>

                <!-- Footer -->
                <div class="text-center p-t-25 footer-note">
                    <strong>All Rights Reserved TELPOSTA &copy; <%=DateTime.Now.Year %>. Powered by 
                        <a href="https://appkings.co.ke/" target="_blank">AppKings Solutions Ltd</a>.</strong>
                </div>
            </div>
        </div>
    </div>

    <style>
        .container-login100 {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            background: linear-gradient(to right, #eef2f7, #d9e1ef);
            padding: 20px;
        }

        .wrap-login100 {
            background: #ffffff;
            padding: 40px 30px;
            border-radius: 15px;
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
            max-width: 420px;
            width: 100%;
        }

        .logo-img {
            max-width: 180px;
            margin-bottom: 15px;
        }

        .portal-title {
            font-family: 'Segoe UI', sans-serif;
            color: #244767;
            text-transform: uppercase;
            font-weight: 600;
            margin-bottom: 15px;
        }

        .custom-hr {
            border: 2px solid #244767;
            border-radius: 2px;
            width: 60%;
            margin: 0 auto 20px auto;
        }

        .input100 {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid #ccc;
            border-radius: 6px;
            font-size: 15px;
            margin-bottom: 15px;
            transition: border 0.3s ease;
        }

        .input100:focus {
            border-color: #244767;
            outline: none;
        }

        .login-btn {
            background-color: #244767;
            color: white;
            padding: 12px;
            border-radius: 6px;
            width: 100%;
            text-align: center;
            font-size: 16px;
            font-weight: bold;
            text-decoration: none;
            transition: background-color 0.3s ease;
        }

        .login-btn:hover {
            background-color: #1a3655;
            text-decoration: none;
        }

        .forgot-link,
        .resend-link {
            color: #244767;
            font-size: 14px;
            text-decoration: none;
            transition: color 0.3s ease;
        }

        .forgot-link:hover,
        .resend-link:hover {
            color: #1a3655;
        }

        .error-label {
            color: red;
            font-size: 14px;
            text-align: center;
            display: block;
            margin-bottom: 10px;
        }

        .footer-note {
            font-size: 12px;
            margin-top: 30px;
            color: #777;
        }

        .footer-note a {
            color: #244767;
            text-decoration: underline;
        }

        .footer-note a:hover {
            text-decoration: none;
        }
    </style>
</asp:Content>
