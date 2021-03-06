﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormDailyIncomeOutcome.aspx.cs" Inherits="RentBike.FormDailyIncomeOutcome" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>CHI TIẾT HÀNG NGÀY</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td>
                    <div class="col-lg-6">
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control input-md" placeholder="Ngày bắt đầu"></asp:TextBox>
                    </div>
                    <div class="col-lg-6">
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control input-md" placeholder="Ngày kết thúc"></asp:TextBox>
                    </div>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" OnClientClick="return validateSearch();" /></td>
                <td>
                    <asp:Button ID="btnNew" runat="server" Text="Thêm mới" CssClass="btn btn-primary" OnClick="btnNew_Click" /></td>
            </tr>
        </tbody>
    </table>
    <asp:Repeater ID="rptInOut" runat="server" OnItemDataBound="rptInOut_ItemDataBound">
        <HeaderTemplate>
            <table id="tblInOut" class="table table-striped table-hover ">
                <thead>
                    <tr class="success">
                        <th>Ngày/Tháng</th>
                        <th class="text-right">Dư đầu kỳ</th>
                        <th class="text-right">Tổng thu</th>
                        <th class="text-right">Tổng chi</th>
                        <th class="text-right">Dư cuối kỳ</th>
                        <th class="text-right" style="display: none;">Chi tiết</th>
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
                <td class="text-right" style="display: none;">
                    <a id='<%# String.Format("a-{0}-{1}", Eval("StoreId"), Convert.ToDateTime(Eval("InOutDate")).ToString("dd/MM/yyyy").Replace("/", "-")) %>' href='<%# String.Format("#detail-{0}-{1}", Eval("StoreId"), Convert.ToDateTime(Eval("InOutDate")).ToString("dd/MM/yyyy").Replace("/", "-")) %>' class="fancybox">Chi tiết...</a>
                    <div id='<%# String.Format("detail-{0}-{1}", Eval("StoreId"), Convert.ToDateTime(Eval("InOutDate")).ToString("dd/MM/yyyy").Replace("/", "-")) %>'>
                        <asp:Repeater ID="rptInOutDayDetail" runat="server" OnItemDataBound="rptInOutDayDetail_ItemDataBound">
                            <HeaderTemplate>
                                <table class="table border_table" border="1">
                                    <tr class="success">
                                        <th colspan="16" style="text-align: center">Chi tiết ngày:
                                            <asp:Literal ID="litInoutDate" runat="server"></asp:Literal></th>
                                    </tr>
                                    <tr>
                                        <th colspan="3">&nbsp;</th>
                                        <th colspan="4" style="text-align: center">THIẾT BỊ VĂN PHÒNG</th>
                                        <th colspan="4" style="text-align: center">GIẤY TỜ XE & KHÁC</th>
                                        <th colspan="5">&nbsp;</th>
                                    </tr>
                                    <tr>
                                        <th>STT</th>
                                        <th>Nội Dung</th>
                                        <th>Kỳ Thu</th>
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
                                        <th>Tiền Xuất</th>
                                        <th>Tiền Nhập</th>
                                        <th>Ghi Chú</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr id="trItem" class="inout-detail" runat="server">
                                    <td>
                                        <asp:Literal ID="litNo" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litCustomerName" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litPeriod" runat="server"></asp:Literal></td>
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
                                        <asp:Literal ID="litOutOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litInOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litOutCapital" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litInCapital" runat="server"></asp:Literal></td>
                                    <td></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                <tr class="background-green-yellow">
                                    <td colspan="2">&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:Literal ID="litTotalContractFeeEquip" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalRentFeeEquip" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalCloseFeeEquip" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalRedundantFeeEquip" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalContractFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalRentFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalCloseFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalRedundantFeeCarAndOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalOutOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalInOther" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalOutCapital" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Literal ID="litTotalInCapital" runat="server"></asp:Literal></td>
                                    <td class="text-result">
                                        <asp:Literal ID="litTotal" runat="server"></asp:Literal></td>
                                </tr>
                                </table>
                                <%--                                <div class="text-right">
                                    <asp:Image ID="ExcelIcon" runat="server" ImageUrl="~/App_Themes/Theme1/image/excel-icon.png" />
                                    <asp:LinkButton ID="lnkExportExcel" runat="server" OnClick="lnkExportExcel_Click" Text="Xuất ra Excel"></asp:LinkButton>
                                </div>--%>
                            </FooterTemplate>
                        </asp:Repeater>
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
    <asp:DropDownList ID="ddlPager" runat="server" CssClass="form-control dropdown-pager-width" OnSelectedIndexChanged="ddlPager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    <script>
        $(function () {
            //$("#txtViewDate").datepicker();
            //$(".fancybox").fancybox({
            //    openEffect: 'elastic',
            //    closeEffect: 'elastic'
            //});

            $('#tblInOut tbody tr').not(".border_table tr").attr('style', 'cursor:pointer');
            $('#tblInOut tbody tr').not(".border_table tr").click(function () {
                $.fancybox({
                    href: $(this).find('a.fancybox').attr('href'),
                    openEffect: 'elastic',
                    closeEffect: 'elastic'
                });
            });

            $('#<%=txtStartDate.ClientID %>').datepicker();
            $('#<%=txtEndDate.ClientID %>').datepicker();

            var options = {};
            $('a.print').click(function (e) {
                $(this).parent().parent().printArea(options);
            });

            $('#<%=txtStartDate.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    if (validateSearch()) {
                        $('#<%=btnSearch.ClientID %>').click();
                        return false;
                    }
                }
            });

            $('#<%=txtEndDate.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    if (validateSearch()) {
                        $('#<%=btnSearch.ClientID %>').click();
                        return false;
                    }
                }
            });

            main.toolTip("#tblInOut tbody tr", "Chi tiết hàng ngày", "top left", "bottom left", 15, 20, ".border_table tr");
            main.toolTip(".border_table tr td a", "Chỉnh sửa", "top left", "bottom left", 15, 20);

            <%--            $('#<%=txtViewDate.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                            return false;
                        }
            });--%>
        });
        function validateSearch() {
            if (new Date($('#<%=txtEndDate.ClientID %>').val()) < new Date($('#<%=txtStartDate.ClientID %>').val())) {
                alert("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
