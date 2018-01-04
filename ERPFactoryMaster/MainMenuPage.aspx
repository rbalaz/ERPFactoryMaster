<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MainMenuPage.aspx.cs" Inherits="ERPFactoryMaster.MainMenuPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
    <link rel="stylesheet" href="/css/MainMenuPage.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
    <asp:Label ID="LogLabel" runat="server" Font-Bold="True" Font-Italic="True"></asp:Label>
    </p>
    <p>
    <span class="margin"><asp:Button ID="LogoutButton" runat="server" BackColor="#0066FF" ForeColor="White" Text="Log out" Width="125px" OnClick="LogoutButton_Click" /></span>
    <span class="margin"><asp:Button ID="NewOrderButton" runat="server" BackColor="#0066FF" ForeColor="White" Text="New order" Width="125px" OnClick="NewOrderButton_Click" /></span>
    <span class="margin"><asp:Button ID="InvoiceButton" runat="server" BackColor="#0066FF" ForeColor="White" Text="View orders" Width="125px" OnClick="InvoiceButton_Click" /></span>
    <span class="margin"><asp:Button ID="PayslipButton" runat="server" BackColor="#0066FF" ForeColor="White" Text="Show payslip" Width="125px" OnClick="PayslipButton_Click" /></span>
    </p>
<p>
    <asp:Label ID="OrderLabel" runat="server" Font-Italic="True"></asp:Label>
</p>
</asp:Content>
