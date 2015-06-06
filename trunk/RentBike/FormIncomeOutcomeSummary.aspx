<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormIncomeOutcomeSummary.aspx.cs" Inherits="RentBike.FormIncomeOutcomeSummary" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>TỔNG HỢP THU CHI</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td style="width: 50%;">
                    <div class="col-lg-6">
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control input-md" placeholder="Ngày bắt đầu"></asp:TextBox>
                    </div>
                    <div class="col-lg-6">
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control input-md" placeholder="Ngày kết thúc"></asp:TextBox>
                    </div>
                </td>

                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" OnClientClick="return validateSearch();" /></td>
            </tr>
        </tbody>
    </table>
    <asp:Repeater ID="rptInOut" runat="server">
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
                        <div class="detail-header">Tổng hợp thu chi</div>
                        <table class="table border_table" border="1">
                            <tbody>
                                <tr class="success">
                                    <td colspan="2">Chi tiết ngày: <%# Convert.ToDateTime(Eval("InOutDate")).ToString("dd/MM/yyyy") %></td>
                                </tr>
                                <tr>
                                    <td>Dư đầu ngày:</td>
                                    <td><%# Eval("BeginAmount").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("BeginAmount")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Cho thuê xe:</td>
                                    <td><%# Eval("ContractFeeCar").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("ContractFeeCar")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Phí thuê xe:</td>
                                    <td><%# Eval("RentFeeCar").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("RentFeeCar")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Thanh lý thuê xe:</td>
                                    <td><%# Eval("CloseFeeCar").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("CloseFeeCar")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Thừa phí thuê xe:</td>
                                    <td><%# Eval("RedundantFeeCar").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("RedundantFeeCar")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Cho thuê thiết bị:</td>
                                    <td><%# Eval("ContractFeeEquip").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("ContractFeeEquip")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Phí thuê thiết bị:</td>
                                    <td><%# Eval("RentFeeEquip").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("RentFeeEquip")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Thanh lý thuê thiết bị:</td>
                                    <td><%# Eval("CloseFeeEquip").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("CloseFeeEquip")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Thừa phí thuê thiết bị:</td>
                                    <td><%# Eval("RedundantFeeEquip").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("RedundantFeeEquip")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Cho thuê khác...:</td>
                                    <td><%# Eval("ContractFeeOther").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("ContractFeeOther")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Phí thuê khác...:</td>
                                    <td><%# Eval("RentFeeOther").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("RentFeeOther")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Thanh lý thuê khác:</td>
                                    <td><%# Eval("CloseFeeOther").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("CloseFeeOther")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Thừa phí thuê khác:</td>
                                    <td><%# Eval("RedundantFeeOther").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("RedundantFeeOther")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Nhập vốn:</td>
                                    <td><%# Eval("InCapital").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("InCapital")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Xuất vốn:</td>
                                    <td><%# Eval("OutCapital").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("OutCapital")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Thu khác:</td>
                                    <td><%# Eval("InOther").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("InOther")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Chi khác:</td>
                                    <td><%# Eval("OutOther").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("OutOther")) %> VNĐ</td>
                                </tr>
                                <tr>
                                    <td>Dư cuối ngày:</td>
                                    <td><%# Eval("EndAmount").ToString() == "0"? "0": string.Format("{0:0,0}", Eval("EndAmount")) %> VNĐ</td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="text-right"><a class="print" href="javascript:void(0);">
                            <i class="glyphicon glyphicon-print"></i>&nbsp;In</a>&nbsp;&nbsp;</a></div>
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

    <table class="table table-striped table-hover" style="width: 50%; margin-left: 25%;">
        <tbody>
            <tr class="success">
                <td colspan="2" class="text-center"><strong>Tổng kết kinh doanh</strong></td>
            </tr>
            <tr>
                <td>Tổng giá trị hợp đồng thuê xe</td>
                <td class="text-right">
                    <asp:Label ID="lblRentBikeAmount" runat="server" Text="" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td>Tổng giá trị hợp đồng cho thuê thiết bị</td>
                <td class="text-right">
                    <asp:Label ID="lblRentEquipAmount" runat="server" Text="" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td>Tổng giá trị hợp đồng cho thuê khác</td>
                <td class="text-right">
                    <asp:Label ID="lblRentOtherAmount" runat="server" Text="" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td>Tổng giá trị tất cả hợp đồng cho thuê</td>
                <td class="text-right">
                    <asp:Label ID="lblRentAll" runat="server" Text="" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td>Tổng phí</td>
                <td class="text-right">
                    <asp:Label ID="lblSumAllIn" runat="server" Text="" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td>Tổng chi phí</td>
                <td class="text-right">
                    <asp:Label ID="lblSumAllOut" runat="server" Text="" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr>
                <td>Tổng đầu tư</td>
                <td class="text-right">
                    <asp:Label ID="lblTotalInvest" runat="server" Text="" CssClass="text-right"></asp:Label></td>
            </tr>
        </tbody>
    </table>
    <script>
        $(function () {
            $("#txtViewDate").datepicker();
            $('#<%=txtStartDate.ClientID %>').datepicker();
            $('#<%=txtEndDate.ClientID %>').datepicker();

            $('#tblInOut tbody tr').not('.border_table tr').attr('style', 'cursor:pointer');
            $('#tblInOut tbody tr').not('.border_table tr').click(function () {
                $.fancybox({
                    href: $(this).find('a.fancybox').attr('href'),
                    openEffect: 'elastic',
                    closeEffect: 'elastic'
                });
            });

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

            main.toolTip("#tblInOut tbody tr", "Chi tiết thu chi", "top left", "bottom left", 15, 20, ".border_table tr");
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
