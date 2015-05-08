<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormDailyIncomeOutcome.aspx.cs" Inherits="RentBike.FormDailyIncomeOutcome" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>CHI TIẾT HÀNG NGÀY</h2>
    <asp:Repeater ID="rptInOut" runat="server" OnItemDataBound="rptInOut_ItemDataBound">
        <HeaderTemplate>
            <table class="table table-striped table-hover ">
                <thead>
                    <tr class="success">
                        <th>Ngày/Tháng</th>
                        <th class="text-right">Dư đầu kỳ</th>
                        <th class="text-right">Tổng thu</th>
                        <th class="text-right">Tổng chi</th>
                        <th class="text-right">Dư cuối kỳ</th>
                        <th class="text-right">Chi tiết</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Convert.ToDateTime(Eval("InOutDate")).ToString("dd/MM/yyyy") %></td>
                <td class="text-right"><%# Eval("BeginAmount").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("BeginAmount")) %></td>
                <td class="text-right"><%# Eval("TotalIn").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("TotalIn")) %></td>
                <td class="text-right"><%# Eval("TotalOut").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("TotalOut")) %></td>
                <td class="text-right"><%# Eval("EndAmount").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("EndAmount")) %></td>
                <td class="text-right">
                    <a id='<%# String.Format("a-{0}-{1}", Eval("StoreId"), Convert.ToDateTime(Eval("InOutDate")).ToString("dd/MM/yyyy").Replace("/", "-")) %>' href='<%# String.Format("#detail-{0}-{1}", Eval("StoreId"), Convert.ToDateTime(Eval("InOutDate")).ToString("dd/MM/yyyy").Replace("/", "-")) %>' class="fancybox">Chi tiết...</a>
                    <div id='<%# String.Format("detail-{0}-{1}", Eval("StoreId"), Convert.ToDateTime(Eval("InOutDate")).ToString("dd/MM/yyyy").Replace("/", "-")) %>' style="display: none;">
                        <asp:Repeater ID="rptInOutDayDetail" runat="server" OnItemDataBound="rptInOutDayDetail_ItemDataBound">
                            <HeaderTemplate>
                                <table class="table table-striped border_table" border="1">
                                    <tr class="success">
                                        <th colspan="14" style="text-align:center">Chi tiết ngày:
                                            <asp:Literal ID="litInoutDate" runat="server"></asp:Literal></th>
                                    </tr>
                                    <tr>
                                        <th colspan="2">&nbsp;</th>
                                        <th colspan="4" style="text-align:center">THIẾT BỊ VĂN PHÒNG</th>
                                        <th colspan="4" style="text-align:center">GIẤY TỜ XE & KHÁC</th>
                                        <th colspan="4">&nbsp;</th>
                                    </tr>
                                    <tr>
                                        <th>STT</th>
                                        <th>Nội Dung</th>
                                        <th>Cho Thuê</th>
                                        <th>Thu Phí</th>
                                        <th>Thanh Lý</th>
                                        <th>Thừa Phí</th>
                                        <th>Cho Thuê</th>
                                        <th>Thu Phí</th>
                                        <th>Thanh Lý</th>
                                        <th>Thừa Phí</th>
                                        <th>Chi Khác</th>
                                        <th>Thu Khác</th>
                                        <th>Nhập Vốn</th>
                                        <th>Xuất Vốn</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Literal ID="litNo" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litCustomerName" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litContractFeeEquip" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litRentFeeEquip" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litClosedFeeEquip" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litRedundantFeeEquip" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litContractFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litRentFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litClosedFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litRedundantFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litInOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litOutOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litInCapital" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litOutCapital" runat="server"></asp:Literal></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                <tr>
                                    <td colspan="2">&nbsp;</td>
                                    <td><asp:Literal ID="litTotalContractFeeEquip" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalRentFeeEquip" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalCloseFeeEquip" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalRedundantFeeEquip" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalContractFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalRentFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalCloseFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalRedundantFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalInOther" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalOutOther" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalInCapital" runat="server"></asp:Literal></td>
                                    <td><asp:Literal ID="litTotalOutCapital" runat="server"></asp:Literal></td>
                                </tr>
                                <tr>
                                    <td colspan="2" class="red success result">Tổng</td>
                                    <td colspan="12" class="red success result"><asp:Literal ID="litTotal" runat="server"></asp:Literal></td>
                                </tr>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
<%--                        <div class="text-right">
                            <a class="print" href="javascript:void(0);">
                                <img src="App_Themes/Theme1/image/printer-blue.png" /></a>
                        </div>--%>
                    </div>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td>Tổng hợp cuối kỳ</td>
                <td class="text-right">
                    <asp:Label ID="lblTotalBegin" runat="server" Text=""></asp:Label>
                </td>
                <td class="text-right">
                    <asp:Label ID="lblTotalIn" runat="server" Text=""></asp:Label></td>
                <td class="text-right">
                    <asp:Label ID="lblTotalOut" runat="server" Text=""></asp:Label></td>
                <td class="text-right text-result">
                    <asp:Label ID="lblTotalEnd" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            </tbody>
           </table>
        </FooterTemplate>
    </asp:Repeater>
    <script>
        $(function () {
            //$("#txtViewDate").datepicker();
            $('.fancybox').fancybox({
                onComplete: function () {
                    alert("Tesst");
                    $.fancybox.showLoading();
                    $('#fancybox-frame').load(function () {
                        $.fancybox.hideLoading();
                    });
                }
            });

            var options = {};
            $('a.print').click(function (e) {
                $(this).parent().parent().printArea(options);
            });

            <%--            $('#<%=txtViewDate.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                            return false;
                        }
            });--%>
        });
    </script>
</asp:Content>
