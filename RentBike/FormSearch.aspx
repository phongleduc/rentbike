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
    <asp:Repeater ID="rptCustomer" runat="server">
        <HeaderTemplate>
            <table class="table table-striped table-hover">
                <thead>
                    <tr class="success">
                        <th>#</th>
                        <th>Tên khách hàng</th>
                        <th>Số CMT/GPLX</th>
                        <th>Cửa hàng</th>
                        <th>Địa chỉ (HKTT)</th>
                        <th>Số hợp đồng</th>
                        <th>Trạng thái</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Container.ItemIndex + 1 %><asp:HiddenField ID="hdfCustomerID" Value='<%# Eval("CUSTOMER_ID") %>' runat="server" />
                </td>
                <td><%# Eval("CUSTOMER_NAME") %></td>
                <td><%# Eval("LICENSE_NO") %></td>
                <td><%# Eval("STORE_NAME") %></td>
                <td><%# Eval("PERMANENT_RESIDENCE") %></td>
                <td>
                    <asp:HyperLink ID="hplContractInfo" runat="server" Text='<%# Eval("CONTRACT_NO") %>' NavigateUrl='<%# GetURL(Convert.ToString(Eval("ID")), Convert.ToString(Eval("STORE_ID"))) %>'></asp:HyperLink>
                </td>
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
        });
    </script>
</asp:Content>
