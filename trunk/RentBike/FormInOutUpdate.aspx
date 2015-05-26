﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormInOutUpdate.aspx.cs" Inherits="RentBike.FormInOutUpdate" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4 class="text-center">Cập nhật trả phí</h4>
    <asp:Repeater ID="rptContractInOut" runat="server">
        <HeaderTemplate>
            <table id="tblContractInOut" class="table table-striped table-hover ">
                <thead>
                    <tr class="success">
                        <th>#</th>
                        <th class="text-right">Số thu</th>
                        <th class="text-right">Số chi</th>
                        <th class="text-center">Ngày thu/chi</th>
                        <th class="text-center">Kỳ</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# Eval("ID") + "|" + Eval("PERIOD_ID")%>'>
                <td class="text-center"><%# Container.ItemIndex + 1 %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", (Eval("IN_AMOUNT").ToString() == "0" ? string.Empty : Eval("IN_AMOUNT"))) %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", (Eval("OUT_AMOUNT").ToString() == "0" ? string.Empty : Eval("OUT_AMOUNT"))) %></td>
                <td class="text-center"><%# string.Format("{0:dd/MM/yyyy}", Eval("INOUT_DATE") == null ? Eval("PERIOD_DATE") : Eval("INOUT_DATE")) %></td>
                <td class="text-center"><%# string.Format("{0:dd/MM/yyyy}", Eval("PERIOD_DATE")) %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr class="danger">
                <td>Mức phí:</td>
                <td>
                    <asp:Label ID="lblAmountPerDay" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr class="info">
                <td>Tổng số đã thanh toán:</td>
                <td>
                    <asp:Label ID="lblTotalPaid" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr class="warning">
                <td>Số tiền dư lần trước:</td>
                <td>
                    <asp:Label ID="lblAmountRemain" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr class="danger">
                <td>Số tiền còn thiếu:</td>
                <td>
                    <asp:Label ID="lblAmountLeft" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            <tr class="info">
                <td>Tổng số phí còn thiếu:</td>
                <td>
                    <asp:Label ID="lblTotalAmoutLeft" runat="server" CssClass="text-right"></asp:Label></td>
            </tr>
            </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <br />
    <table class="table table-striped table-hover" style="width: 50%; margin-left: 25%;">
        <tbody>
            <tr>
                <td colspan="2" class="text-center"><strong>Chi tiết khoản trả phí</strong></td>
            </tr>
            <tr>
                <td class="text-right">Loại chi phí</td>
                <td>
                    <asp:DropDownList ID="ddInOutType" Enabled="False" runat="server" CssClass="form-control"></asp:DropDownList></td>
            </tr>
            <tr>
                <td class="text-right">Cửa hàng</td>
                <td>
                    <asp:TextBox ID="txtStore" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="text-right">Số tiền</td>
                <td>
                    <asp:TextBox ID="txtIncome" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="text-right">Thông tin thêm</td>
                <td>
                    <asp:TextBox ID="txtMoreInfo" runat="server" TextMode="MultiLine" CssClass="form-control input-sm"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="text-right">Thông tin hợp đồng</td>
                <td>
                    <asp:HyperLink ID="hplContract" runat="server"></asp:HyperLink></td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnSave" runat="server" Text="Lưu & thoát" CssClass="btn btn-primary" OnClick="btnSave_Click" />&nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" Text="Quay lại" CssClass="btn btn-primary" OnClick="btnCancel_Click" /></td>
            </tr>
        </tbody>
    </table>
    <script>
        $(document).ready(function () {
            $('#<%=txtIncome.ClientID %>').priceFormat({ prefix: '', suffix: '', centsLimit: 0 });

            $('input, textarea').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSave.ClientID %>').click();
                    return false;
                }
            });

            $.each($('#tblContractInOut tbody tr'), function () {
                $(this).attr('style', 'cursor:pointer');
                $(this).click(function () {
                    location.href = "FormInOutAndPeriodUpdate.aspx?ID=" + $(this).attr('id').split('|')[0] + "&pid=" + $(this).attr('id').split('|')[1];
                });
            });
        });
    </script>
</asp:Content>
