<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormBadContractReport.aspx.cs" Inherits="RentBike.FormBadContractReport" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>HỢP ĐỒNG TREO</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td style="width:50%;">
                    <div class="col-lg-15">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control input-md" placeholder="Tìm kiếm"></asp:TextBox>
                    </div>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" /></td>
            </tr>
        </tbody>
    </table>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td class="text-right"><strong>Hợp đồng cho thuê xe:</strong></td>
                <td class="text-right">Số lượng:&nbsp;<asp:Label ID="lblRentBikeCount" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng giá trị:&nbsp;</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalFeeBikeContract" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td class="text-right"><strong>Hợp đồng thuê thiết bị:</strong></td>
                <td class="text-right">Số lượng:&nbsp;<asp:Label ID="lblRentEquipCount" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng giá trị:&nbsp;</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalFeeEquiqContract" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td class="text-right"><strong>Hợp đồng thuê khác:</strong></td>
                <td class="text-right">Số lượng:&nbsp;<asp:Label ID="lblRentOtherCount" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng giá trị:&nbsp;</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalFeeOtherContract" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td class="text-right"><strong>Tổng hợp đồng khách hàng xấu:</strong></td>
                <td class="text-right">Số lượng:&nbsp;<asp:Label ID="lblNumberOfBadContract" runat="server" CssClass="text-right"></asp:Label></td>
                <td class="text-right"><strong>Tổng giá trị HĐ (<asp:Label ID="lblPercentBadContract" runat="server" CssClass="text-right"></asp:Label>)</strong></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalBadContract" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
        </tbody>
    </table>
    <asp:Repeater ID="rptBadContract" runat="server">
        <HeaderTemplate>
            <table id="tblBadContract" class="table table-hover ">
                <thead>
                    <tr class="success">
                        <th>#</th>
                        <th>Tên khách hàng</th>
                        <th>Loại hình</th>
                        <th class="text-right">Giá trị HĐ</th>
                        <th class="text-right">Phí/ngày</th>
                        <th class="text-center">Đã trả phí hết ngày</th>
                        <th class="text-center">Số ngày chậm</th>
                        <th class="text-center">Tổng tiền phí</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# Eval("ID") %>' class="background-red">
                <td><%# Container.ItemIndex + 1 %></td>
                <td><strong><%# Eval("CUSTOMER_NAME") %></strong></td>
                <td><%# Eval("RENT_TYPE_NAME") %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", Eval("CONTRACT_AMOUNT")) %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", Eval("FEE_PER_DAY")) %> VNĐ</td>
                <td class="text-center"><%# String.Format("{0:dd/MM/yyyy}", Eval("PAY_DATE"))%></td>
                <td class="text-center red"><%# Eval("OVER_DATE")%> Ngày</td>
                <td class="text-center"><%# string.Format("{0:0,0}", Convert.ToDecimal(Eval("FEE_PER_DAY")) * Convert.ToDecimal(Eval("OVER_DATE"))) %> VNĐ</td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
           </table>
        </FooterTemplate>
    </asp:Repeater>
    <script>
        $(function () {
            $.each($('#tblBadContract tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormContractUpdate.aspx?ID=" + $(this).attr('id');
                });
            });
            main.toolTip("#tblBadContract tbody tr", "Chi tiết hợp đồng", "top left", "bottom left", 15, 20);
        });
    </script>
</asp:Content>
