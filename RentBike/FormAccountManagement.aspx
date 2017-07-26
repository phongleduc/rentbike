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
        <%
            if (ViewState["DeleteSuccess"] != null)
            {
        %>
        <div class="alert alert-success">
            <strong><%= ViewState["DeleteSuccess"] %></strong>
        </div>
        <%
            }
        %>
        <%
            if (ViewState["DeleteFail"] != null)
            {
        %>
        <div class="alert alert-danger">
            <strong><%= ViewState["DeleteFail"] %></strong>
        </div>
        <%
            }
        %>
        <asp:Repeater ID="rptAccount" runat="server" OnItemCommand="rptAccount_ItemCommand">
            <HeaderTemplate>
                <table id="tblAccount" class="table table-striped table-hover">
                    <thead>
                        <tr class="success">
                            <th>#</th>
                            <th>Tên người dùng*</th>
                            <th>Cửa hàng*</th>
                            <th>Chức vụ</th>
                            <th>Trạng thái</th>
                            <th>Xóa</th>
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
                    <td><%# GetStatus(Convert.ToBoolean(Eval("ACTIVE"))) %></td>
                    <td>
                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-primary" CommandName="Delete" CommandArgument='<%#Eval("ID")%>' OnClientClick='javascript:if(!confirm("Bạn có chắc chắn muốn xóa tài khoản?"))return false;' Text="Xóa"></asp:LinkButton>
                    </td>
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
