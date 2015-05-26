<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormSearch.aspx.cs" Inherits="RentBike.FormSearch" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>KẾT QUẢ TÌM KIẾM</h2>
    <h3><asp:Literal ID="litSearchResult" runat="server"></asp:Literal></h3>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control input-md"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" /></td>
            </tr>
        </tbody>
    </table>
    <asp:Repeater ID="rptSearch" runat="server">
        <HeaderTemplate>
            <table id="tblSearch" class="table table-hover">
                <thead>
                    <tr class="success">
                        <th>#</th>
                        <th>Tên khách hàng</th>
                        <th>Số CMT/GPLX</th>
                        <th>Cửa hàng</th>
                        <th>Địa chỉ (HKTT)</th>
                        <th>Trạng thái</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# Eval("ID") + "|" + Eval("STORE_ID") %>' class="<%# (Convert.ToBoolean(Eval("IS_BAD_CONTRACT")) == true || IsBadContract(Convert.ToInt32(Eval("ID"))) == true) ? "background-red" : "" %>">
                <td><%# Container.ItemIndex + 1 %></td>
                <td>'<%# Eval("CUSTOMER_NAME") %>'</td>
                <td><%# Eval("LICENSE_NO") %></td>
                <td><%# Eval("STORE_NAME") %></td>
                <td><%# Eval("PERMANENT_RESIDENCE") %></td>
                <td><%# Convert.ToBoolean(Eval("CONTRACT_STATUS")) ? "Chưa thanh lý" : "Đã thanh lý" %></td>
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

            $.each($('#tblSearch tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormContractUpdate.aspx?ID=" + $(this).attr('id').split('|')[0] + "&sID=" + $(this).attr('id').split('|')[1];
                });
            });

            main.toolTip("#tblSearch tbody tr", "Chi tiết hợp đồng", "top left", "bottom left", 15, 20);
        });
    </script>
</asp:Content>
