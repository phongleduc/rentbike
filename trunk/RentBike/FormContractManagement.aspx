<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormContractManagement.aspx.cs" Inherits="RentBike.ContractManagement" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>QUẢN LÝ HỢP ĐỒNG</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" /></td>
                <td>
                    <asp:Button ID="btnNew" runat="server" Text="Làm hợp đồng" CssClass="btn btn-primary" OnClick="btnNew_Click" /></td>
            </tr>
        </tbody>
    </table>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td class="text-right"><strong>Hợp đồng cho thuê xe</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblRentBikeNo" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng giá trị hợp đồng thuê xe</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalBikeContractAmount" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng số hợp đồng chưa thanh lý</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblNotFinishedContract" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td class="text-right"><strong>Hợp đồng cho thuê TBVP</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblRentOfficeEquip" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng giá trị hợp đồng thuê TBVP</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalEquipContractAmount" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng phí hợp đồng/ngày</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalFeeContract" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td class="text-right"><strong>Hợp đồng khác</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblRentOther" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng giá trị hợp đồng thuê khác</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalOtherContractAmount" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng số tiền hợp đồng chưa thanh lý</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalMoneyOfNotFinishContract" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
        </tbody>
    </table>
    <asp:Repeater ID="rptContract" runat="server">
        <HeaderTemplate>
            <table id="tblContract" class="table table-striped table-hover ">
                <thead>
                    <tr class="success">
                        <th>#</th>
                        <th>Tên khách hàng</th>
                        <th>Địa chỉ</th>
                        <th class="text-right">Số tiền</th>
                        <th class="text-right">Phí/ngày</th>
                        <th class="text-center">Ngày thuê</th>
                        <th class="text-center">Ngày hết hạn</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# Eval("ID") + "|" + Eval("STORE_ID") %>'>
                <td><%# Container.ItemIndex + 1 %></td>
                <td><strong><%# Eval("CUSTOMER_NAME") %></strong></td>
                <td><%# Eval("CURRENT_RESIDENCE") == null? (Eval("PERMANENT_RESIDENCE") == null? Eval("ADDRESS") : Eval("PERMANENT_RESIDENCE")) : Eval("CURRENT_RESIDENCE") %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", Eval("CONTRACT_AMOUNT")) %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", Eval("FEE_PER_DAY")) %></td>
                <td class="text-center"><%# String.Format("{0:dd/MM/yyyy}", Eval("RENT_DATE"))%></td>
                <td class="text-center"><%# String.Format("{0:dd/MM/yyyy}", Eval("END_DATE"))%></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
           </table>
        </FooterTemplate>
    </asp:Repeater>
    <%--    <asp:DropDownList ID="ddlPager" runat="server" CssClass="form-control dropdown-pager-width" OnSelectedIndexChanged="ddlPager_SelectedIndexChanged" AutoPostBack="true" Visible="false"></asp:DropDownList>--%>
    <b>Loại hợp đồng:</b>
    <asp:DropDownList ID="drpRentType" runat="server" CssClass="form-control drp-renttype" OnSelectedIndexChanged="drpRentType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    <script>
        $(function () {
            $('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                    return false;
                }
            });
            $.each($('#tblContract tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormContractUpdate.aspx?ID=" + $(this).attr('id').split('|')[0] + "&sID=" + $(this).attr('id').split('|')[1];
                });
            });

            main.toolTip("#tblContract tbody tr", "Chi tiết hợp đồng", "top left", "bottom left", 15, 20);
        });
    </script>
</asp:Content>
