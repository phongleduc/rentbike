<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormReport.aspx.cs" Inherits="RentBike.FormReport" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>THÔNG BÁO</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td style="width: 50%;">
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
                <td class="text-left" style="font-size:28px;"><strong>Tổng phí chậm&nbsp;:&nbsp;&nbsp;<asp:Label ID="lblTotalAmountLeft" runat="server" CssClass="text-left"></asp:Label></strong></td>
            </tr>
        </tbody>
    </table>
    <asp:Repeater ID="rptReport" runat="server">
        <HeaderTemplate>
            <div class="text-right" style="margin-bottom: 5px">
                <a class="print" href="javascript:void(0);">
                    <i class="glyphicon glyphicon-print"></i>&nbsp;In</a>&nbsp;&nbsp;
            </div>
            <div id="areaToPrint">
                <table id="tblReport" class="table table-hover ">
                    <thead>
                        <tr class="success">
                            <th>#</th>
                            <th>Tên khách hàng</th>
                            <th>Loại hình thuê</th>
                            <th>Số ĐT khách hàng</th>
                            <th class="text-right">Giá trị HĐ/Phí</th>
                            <th class="text-right">Số lần đóng phí</th>
                            <th class="text-center">Thông báo</th>
                            <th class="text-center">Số ngày quá hạn</th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# Eval("ID") + "|" + Eval("STORE_ID") %>' class="<%# Eval("CSS_CLASS") %>">
                <td><%# Container.ItemIndex + 1 %></td>
                <td><strong><%# Eval("CUSTOMER_NAME") %></strong><br />
                    (<%# Convert.ToDateTime(Eval("BIRTH_DAY")).ToString("dd/MM/yyyy") %>)</td>
                <td><%# Eval("RENT_TYPE_NAME") %></td>
                <td><%# Eval("PHONE") %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", Convert.ToDecimal(Eval("FEE_PER_DAY"))) %></td>
                <td class="text-right"><%# Eval("PAYED_TIME") %> lần</td>
                <td class="text-right"><%# Eval("NOTE") %></td>
                <td class="text-center"><%# Eval("OVER_DATE") %> Ngày
                    <br />
                    <span style="color: red">(<%# Convert.ToDateTime(Eval("PAY_DATE")).ToString("dd/MM/yyyy") %>)</span></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
            </table>
                </div>
        </FooterTemplate>
    </asp:Repeater>

    <script>
        $(function () {
            $('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                    return false;
                }
            });

            $('a.print').click(function (e) {
                printDiv();
            });

            function printDiv() {
                var divToPrint = $('#areaToPrint').clone();
                divToPrint.prepend($("<h3 style='text-align:center;'>" + "Ngày <%= DateTime.Now.ToString("dd/MM/yyyy")%>" + "</h3>"));
                divToPrint.prepend($("<h1 style='text-align:center;'>Thông Báo</h1>"));
                divToPrint.find('table').css('width', '100%');
                divToPrint.find('table').css('border-collapse', 'collapse');
                divToPrint.find('table').find("tr").css('border', '1px solid black');
                divToPrint.find('table').find("td").css('border', '1px solid black');
                divToPrint.find('table').find("th").css('border', '1px solid black');
                newWin = window.open("");
                newWin.document.write(divToPrint.html());
                newWin.print();
                newWin.close();
            }

            $.each($('#tblReport tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormContractUpdate.aspx?ID=" + $(this).attr('id').split('|')[0] + "&sID=" + $(this).attr('id').split('|')[1];
                });
            });

            main.toolTip("#tblReport tbody tr", "Chi tiết hợp đồng", "top left", "bottom left", 15, 20);
        });
    </script>
</asp:Content>
