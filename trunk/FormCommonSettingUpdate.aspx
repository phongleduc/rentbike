<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormCommonSettingUpdate.aspx.cs" Inherits="RentBike.FormCommonSettingUpdate" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Thông tin danh mục</h2>
    <table class="table table-striped table-hover" style="width: 60%; margin-left: 20%;">
        <tbody>
            <tr>
                <td colspan="2" class="text-center">
                    <strong>
                        <asp:Label ID="lblMessage" runat="server" Text="" CssClass="text-center text-warning"></asp:Label></strong>
                </td>
            </tr>
            <tr>
                <td class="text-right">Tên danh mục</td>
                <td>
                    <asp:TextBox ID="txtItem" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="text-right">Kích hoạt</td>
                <td>
                    <asp:RadioButton ID="rdbActive" runat="server" Text="Sử dụng" GroupName="activegroup" Checked="True" />
                    -
                    <asp:RadioButton ID="rdbDeActive" runat="server" Text="Ngừng sử dụng" GroupName="activegroup" /></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Button ID="btnSave" runat="server" Text="Lưu & thoát" CssClass="btn btn-primary" OnClick="btnSave_Click" />&nbsp;<asp:Button ID="btnCancel" runat="server" Text="Thoát" CssClass="btn btn-primary" OnClick="btnCancel_Click" /></td>
            </tr>
        </tbody>
    </table>
</asp:Content>
