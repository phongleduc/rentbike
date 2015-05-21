<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormSummaryPayFeeDaily.aspx.cs" Inherits="RentBike.FormSummaryPayFeeDaily" %>
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
                <table class="table">
                    <tr class="success">
                        <th>#</th>
                        <th>Tên khách hàng</th>
                        <th>Loại hình thuê</th>
                        <th>Số ĐT khách hàng</th>
                        <th class="text-right">Giá trị HĐ/Phí</th>
                        <th class="text-center">Ghi chú</th>
                        <th class="text-right">Số lần đóng phí</th>
                        <th class="text-center">Thông báo</th>
                        <th class="text-center">Xử lý HĐ</th>
                    </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# string.Format("HtmlTableRow{0}", Container.ItemIndex) %>'>
                <td><%# Container.ItemIndex + 1 %></td>
                <td><strong><%# Eval("CUSTOMER_NAME") %></strong></td>
                <td><%# Eval("RENT_TYPE_NAME") %></td>
                <td><%# Eval("PHONE") %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", Convert.ToDecimal(Eval("PAY_FEE"))) %></td>
                <td class="text-right"><%# Eval("NOTE") %></td>
                <td class="text-right"><%# Eval("PAY_TIME") %> lần</td>
                <td class="text-center"><%# Eval("PAY_MESSAGE") %></td>
                <td>
                    <asp:HyperLink ID="hplUpdateContract" CssClass="text-center" runat="server" Text='<%# Eval("CONTRACT_NO")%>' NavigateUrl='<%# Eval("ID","FormContractUpdate.aspx?ID={0}") %>'></asp:HyperLink></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
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

        });
    </script>
</asp:Content>
