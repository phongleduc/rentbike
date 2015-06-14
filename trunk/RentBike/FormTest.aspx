<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormTest.aspx.cs" Inherits="RentBike.FormTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button ID="btnTest" runat="server" Text="Test" OnClick="btnTest_Click"/>
    <asp:Label ID="lblTest" runat="server"></asp:Label>
</asp:Content>
