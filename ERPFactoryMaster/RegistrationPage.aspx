<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistrationPage.aspx.cs" Inherits="ERPFactoryMaster.RegistrationPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
    <link rel="stylesheet" href="/Css/RegisterPage.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="First name:"></asp:Label>
        <span class="offset"><asp:TextBox ID="FirstNameBox" runat="server" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Last name:"></asp:Label>
        <span class="offset"><asp:TextBox ID="LastNameBox" runat="server" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Date of birth:"></asp:Label>
        <span class="offset"><asp:TextBox ID="BirthBox" runat="server" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Street:"></asp:Label>
        <span class="offset"><asp:TextBox ID="StreetBox" runat="server" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label5" runat="server" Font-Bold="True" Text="City:"></asp:Label>
        <span class="offset"><asp:TextBox ID="CityBox" runat="server" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label6" runat="server" Font-Bold="True" Text="Post code:"></asp:Label>
        <span class="offset"><asp:TextBox ID="PostCodeBox" runat="server" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label7" runat="server" Font-Bold="True" Text="Country:"></asp:Label>
        <span class="offset"><asp:TextBox ID="CountryBox" runat="server" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label8" runat="server" Font-Bold="True" Text="User name:"></asp:Label>
        <span class="offset"><asp:TextBox ID="UserBox" runat="server" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label9" runat="server" Font-Bold="True" Text="Password:"></asp:Label>
        <span class="offset"><asp:TextBox ID="PasswordBox" runat="server" TextMode="Password" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label10" runat="server" Font-Bold="True" Text="Confirm password:"></asp:Label>
        <span class="offset"><asp:TextBox ID="ConfirmPasswordBox" runat="server" TextMode="Password" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <span class="margin"><asp:Button ID="ConfirmButton" runat="server" BackColor="#0066FF" ForeColor="White" Height="38px" Text="Confirm" Width="90px" OnClick="ConfirmButton_Click" /></span>
        <span class="margin"><asp:Button ID="ReturnButton" runat="server" BackColor="#0066FF" ForeColor="White" Height="38px" Text="Back" Width="90px" OnClick="ReturnButton_Click" /></span>
    </p>
<p>
        <asp:Label ID="WarningLabel" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
    </p>
</asp:Content>
