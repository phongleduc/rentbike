<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormContractHistoryManagement.aspx.cs" Inherits="RentBike.ContractHistoryManagement" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>HỢP ĐỒNG CŨ</h2>
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
    <asp:Repeater ID="rptContractHistory" runat="server">
        <HeaderTemplate>
            <table id="tblContractHistory" class="table table-striped table-hover">
                <thead>
                    <tr id='<%# Eval("ID") + "|" + Eval("STORE_ID") %>' class="success">
                        <th>#</th>
                        <th>Tên khách hàng</th>
                        <th>Loại hình</th>
                        <th class="text-right">Giá trị hàng cho thuê</th>
                        <th class="text-right">Phí/ngày</th>
                        <th class="text-center">Ngày thuê</th>
                        <th class="text-center">Ngày kết thúc</th>
                        <th class="text-center">Ngày thanh lý</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# Eval("CONTRACT_ID") + "|" + Eval("STORE_ID") %>' class="<%# Convert.ToBoolean(Eval("IS_BAD_CONTRACT")) == true ? "background-red" : "" %>">
                <td><%# Container.ItemIndex + 1 %></td>
                <td><strong><%# Eval("CUSTOMER_NAME") %></strong></td>
                <td><%# Eval("RENT_TYPE_NAME") %></td>
                <td class="text-right"><%#  string.Format("{0:0,0}", Eval("CONTRACT_AMOUNT")) %></td>
                <td class="text-right"><%#  string.Format("{0:0,0}", Eval("FEE_PER_DAY")) %></td>
                <td class="text-center"><%# String.Format("{0:dd/MM/yyyy}", Eval("RENT_DATE"))%></td>
                <td class="text-center"><%# String.Format("{0:dd/MM/yyyy}", Eval("END_DATE"))%></td>
                <td class="text-center"><%# String.Format("{0:dd/MM/yyyy}", Eval("CLOSE_CONTRACT_DATE"))%></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
           </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:DropDownList ID="ddlPager" runat="server" CssClass="form-control dropdown-pager-width" OnSelectedIndexChanged="ddlPager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
<%--    <b>Loại hợp đồng:</b>  <asp:DropDownList ID="drpRentType" runat="server" CssClass="form-control drp-renttype" OnSelectedIndexChanged="drpRentType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>--%>
    <script>
        $(function () {
            $('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                    return false;
                }
            });

            $.each($('#tblContractHistory tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormContractUpdate.aspx?ID=" + $(this).attr('id').split('|')[0] + "&sID=" + $(this).attr('id').split('|')[1] + "&copy=1";
                });
            });

            main.toolTip("#tblContractHistory tbody tr", "Sao chép hợp đồng", "top left", "bottom left", 15, 20);
        });
    </script>
</asp:Content>
