<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormWarning.aspx.cs" Inherits="RentBike.FormWarning" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>DANH SÁCH GỌI PHÍ</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td>
                    <div class="col-lg-4">
                        <asp:TextBox ID="txtDate" runat="server" CssClass="form-control input-md" placeholder="Hợp đồng đến hạn trong ngày"></asp:TextBox>
                    </div>
                    <div class="col-lg-7">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control input-md" placeholder="Tìm kiếm"></asp:TextBox>
                    </div>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" /></td>
            </tr>
        </tbody>
    </table>
    <asp:Repeater ID="rptWarning" runat="server">
        <HeaderTemplate>
            <div class="text-right" style="margin-bottom: 5px">
                <asp:Image ID="ExcelIcon" runat="server" ImageUrl="~/App_Themes/Theme1/image/excel-icon.png" />
                <asp:LinkButton ID="lnkExportExcel" runat="server" OnClick="lnkExportExcel_Click" Text="Xuất ra Excel"></asp:LinkButton>
            </div>
            <div id="areaToPrint">
                <table id="tblWarning" class="table table-hover">
                    <thead>
                        <tr class="success">
                            <th>#</th>
                            <th>Tên khách hàng</th>
                            <th>Loại hình thuê</th>
                            <th>Số ĐT khách hàng</th>
                            <th class="text-right">Giá trị HĐ/Phí</th>
                            <th class="text-center">Ghi chú</th>
                            <th class="text-right">Số lần đóng phí</th>
                            <th class="text-center">Thông báo</th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# Eval("ID") %>' class="<%# Eval("CSS_CLASS") %>">
                <td><%# Container.ItemIndex + 1 %></td>
                <td><strong><%# Eval("CUSTOMER_NAME") %></strong></td>
                <td><%# Eval("RENT_TYPE_NAME") %></td>
                <td><%# Eval("PHONE") %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", Convert.ToDecimal(Eval("FEE_PER_DAY"))) %></td>
                <td class="text-right"><%# Eval("NOTE") %></td>
                <td class="text-right"><%# Eval("PAYED_TIME") %> lần</td>
                <td class="text-center"><%# Eval("PERIOD_MESSAGE") %></td>
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
            $('#<%=txtDate.ClientID %>').datepicker();

            $('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                    return false;
                }
            });
            $.each($('#tblWarning tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormContractUpdate.aspx?ID=" + $(this).attr('id');
                });
            });
            main.toolTip("#tblWarning tbody tr", "Chi tiết hợp đồng", "top left", "bottom left", 15, 20);
        });
    </script>
</asp:Content>
