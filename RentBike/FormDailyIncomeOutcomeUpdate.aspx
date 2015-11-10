<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormDailyIncomeOutcomeUpdate.aspx.cs" Inherits="RentBike.FormDailyIncomeOutcomeUpdate" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlTable" runat="server">
        <table class="table table-striped table-hover" style="width: 50%; margin-left: 25%;">
            <tbody>
                <tr>
                    <td colspan="2" class="text-center"><strong>Chi tiết khoản thu/chi</strong></td>
                </tr>
                <tr>
                    <td colspan="2" class="text-center"><strong>
                        <asp:Label ID="lblMessage" runat="server" Text="" CssClass="text-center text-warning"></asp:Label></strong></td>
                </tr>
                <tr>
                    <td>Chi phí ngày</td>
                    <td>
                        <asp:TextBox ClientIDMode="Static" ID="txtFeeDate" runat="server" CssClass="form-control input-sm text-right"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Loại chi phí <span class="red">*</span></td>
                    <td>
                        <asp:DropDownList ID="ddlInOutFee" runat="server" CssClass="form-control"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>Cửa hàng</td>
                    <td>
                        <asp:DropDownList ID="ddlStore" runat="server" CssClass="form-control"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>Số tiền</td>
                    <td>
                        <asp:TextBox ID="txtFeeAmount" runat="server" CssClass="form-control input-sm text-right"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Thông tin thêm</td>
                    <td>
                        <asp:TextBox ID="txtMoreInfo" runat="server" TextMode="MultiLine" CssClass="form-control input-sm" /></td>
                </tr>
                <%--<tr>
                <td>Thông tin hợp đồng</td>
                <td>Theo hợp đồng số HDCT8552. Khách hàng: Nguyễn Quang Hùng - 1984-03/05/2014</td>
            </tr>--%>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnSave" runat="server" Text="Lưu & thoát" CssClass="btn btn-primary" OnClick="btnSave_Click" />&nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" Text="Quay lại" CssClass="btn btn-primary" OnClick="btnCancel_Click" /></td>
                </tr>
            </tbody>
        </table>
        <script>
            $(document).ready(function () {
                $("#txtFeeDate").datepicker();
                $('#<%=txtFeeAmount.ClientID %>').priceFormat({ prefix: '', suffix: '', centsLimit: 0 });
            });
        </script>
    </asp:Panel>
</asp:Content>
