<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="ERPFactoryMaster.MainPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
    <link rel="stylesheet" href="/Css/MainPage.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="margin">
        <asp:Label ID="LogLabel" runat="server" Font-Bold="True" Font-Italic="True"></asp:Label>
    </div>
    <p>
        <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Login:"></asp:Label>
        <span class="offset"><asp:TextBox ID="LoginBox" runat="server" ToolTip="Enter your user name." CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Password:"></asp:Label>
        <span class="offset"><asp:TextBox ID="PasswordBox" runat="server" ToolTip="Enter your password." TextMode="Password" CssClass="offset"></asp:TextBox></span>
    </p>
    <p>
        <span class="margin"><asp:Button ID="ConfirmButton" runat="server" BackColor="#0066FF" ForeColor="White" Height="38px" Text="Confirm" Width="90px" OnClick="ConfirmButton_Click" /></span>
        <span class="margin"><asp:Button ID="RegisterButton" runat="server" BackColor="#0066FF" ForeColor="White" Height="38px" Text="Register" Width="90px" OnClick="RegisterButton_Click" /></span>
    </p>
<p>
        <asp:Button ID="CreateButton" runat="server" OnClick="CreateButton_Click" Text="Create tables" BackColor="#0066FF" ForeColor="White" Height="38px" CssClass="margin" />
        <asp:Button ID="FillButton" runat="server" OnClick="FillButton_Click" Text="Fill tables" BackColor="#0066FF" ForeColor="White" Height="38px" CssClass="margin" />
        <asp:Button ID="SimulateButton" runat="server" OnClick="SimulationButton_Click" Text="Simulate orders" BackColor="#0066FF" ForeColor="White" Height="38px" CssClass="margin" />
    </p>
</asp:Content>
