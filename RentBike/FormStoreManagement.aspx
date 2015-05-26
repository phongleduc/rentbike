<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormStoreManagement.aspx.cs" Inherits="RentBike.FormShopManagement" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>QUẢN LÝ CỬA HÀNG</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" /></td>
                <td>
                    <asp:Button ID="btnNew" runat="server" Text="Tạo cửa hàng" CssClass="btn btn-primary" OnClick="btnNew_Click" /></td>
            </tr>
        </tbody>
    </table>
    <asp:Repeater ID="rptStore" runat="server" OnItemCommand="rptStore_ItemCommand">
        <HeaderTemplate>
            <table id="tblStore" class="table table-striped table-hover ">
                <thead>
                    <tr class="success">
                        <th>#</th>
                        <th class="text-center">Tên cửa hàng*</th>
                        <th class="text-center">Thành phố</th>
                        <th class="text-center">Số điện thoại*</th>
                        <th class="text-center">Ghi chú</th>
                        <th class="text-center">Trạng thái</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# Eval("ID") %>'>
                <td><%# Container.ItemIndex + 1 %></td>
                <td><%# Eval("NAME") %></td>
                <td><%# Eval("CITY") %></td>
                <td><%# Eval("PHONE") %></td>
                <td><%# Eval("NOTE") %></td>
                <td class="text-center"><%# Convert.ToBoolean(Eval("ACTIVE")) ? "Đang hoạt động" : "Ngừng hoạt động" %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
           </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:DropDownList ID="ddlPager" runat="server" CssClass="form-control dropdown-pager-width" OnSelectedIndexChanged="ddlPager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    <script>
        $(function () {
            $('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                    return false;
                }
            });
            $.each($('#tblStore tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormStoreDetail.aspx?ID=" + $(this).attr('id');
                });
            });
            main.toolTip("#tblStore tbody tr", "Chi tiết cửa hàng", "top left", "bottom left", 15, 20);
        });
    </script>
</asp:Content>
