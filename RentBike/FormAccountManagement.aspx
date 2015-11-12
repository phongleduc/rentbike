<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormAccountManagement.aspx.cs" Inherits="RentBike.FormAccountManagement" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>QUẢN LÝ TÀI KHOẢN</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" /></td>
                <td>
                    <asp:Button ID="btnNew" runat="server" Text="Tạo tài khoản" CssClass="btn btn-primary" OnClick="btnNew_Click" /></td>
            </tr>
        </tbody>
    </table>
    <div>
        <asp:Repeater ID="rptAccount" runat="server">
            <HeaderTemplate>
                <table id="tblAccount" class="table table-striped table-hover">
                    <thead>
                        <tr class="success">
                            <th>#</th>
                            <th>Tên người dùng*</th>
                            <th>Cửa hàng*</th>
                            <th>Chức vụ</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr id='<%# Eval("ID") %>'>
                    <td><%# Container.ItemIndex + 1 %></td>
                    <td><%# Eval("NAME") %></td>
                    <td><%# GetStoreName(Convert.ToInt32(Eval("STORE_ID"))) %></td>
                    <td><%# GetPermissionName(Convert.ToInt32(Eval("PERMISSION_ID"))) %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                    </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:DropDownList ID="ddlPager" runat="server" CssClass="form-control dropdown-pager-width" OnSelectedIndexChanged="ddlPager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    </div>
    <script>
        $(function () {
            $('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                return false;
                }
            });
            $.each($('#tblAccount tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormAccountUpdate.aspx?ID=" + $(this).attr('id');
                });
            });

            main.toolTip("#tblAccount tbody tr", "Chi tiết tài khoản", "top left", "bottom left", 15, 20);
        });
    </script>
</asp:Content>
