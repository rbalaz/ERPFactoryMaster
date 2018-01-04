<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewInvoicePage.aspx.cs" Inherits="ERPFactoryMaster.NewInvoicePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
    <link rel="stylesheet" href="/css/NewInvoicePage.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
    <asp:Label ID="LogLabel" runat="server" Font-Bold="True" Font-Italic="True"></asp:Label>
    </p>
    <p>
    <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Create new product order:"></asp:Label>
    </p>
    <p>
    <asp:Label ID="Label2" runat="server" Text="Choose a product:"></asp:Label>
    <asp:DropDownList ID="ProductList" runat="server">
        <asp:ListItem>Farebný papier žltý</asp:ListItem>
        <asp:ListItem>Klasický papier</asp:ListItem>
        <asp:ListItem>Kartón</asp:ListItem>
        <asp:ListItem>Farebný papier červený</asp:ListItem>
        <asp:ListItem>Farebný papier zelený</asp:ListItem>
        <asp:ListItem>Farebný papier modrý</asp:ListItem>
        <asp:ListItem>Farebný papier čierny</asp:ListItem>
    </asp:DropDownList>
    </p>
    <p>
    <asp:Label ID="Label3" runat="server" Text="Amount:"></asp:Label>
    <asp:TextBox ID="AmountBox" runat="server" Width="47px">0</asp:TextBox>
    </p>
    <p>
    <span class="margin"><asp:Button ID="ConfirmButton" runat="server" BackColor="#0066FF" ForeColor="White" Height="38px" Text="Confirm" Width="90px" OnClick="ConfirmButton_Click" /></span>
    <span class="margin"><asp:Button ID="ReturnButton" runat="server" BackColor="#0066FF" ForeColor="White" Height="38px" Text="Back" Width="90px" OnClick="ReturnButton_Click" /></span>
    </p>
</asp:Content>
