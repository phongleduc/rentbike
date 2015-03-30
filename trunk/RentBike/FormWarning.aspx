﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormWarning.aspx.cs" Inherits="RentBike.FormWarning" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>THÔNG BÁO</h2>
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
    <asp:Repeater ID="rptWarning" runat="server" OnItemDataBound="rptWarning_ItemDataBound">
        <HeaderTemplate>
            <table class="table table-striped table-hover">
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
                        <th class="text-center">Xử lý HĐ</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr id='<%# string.Format("HtmlTableRow{0}", Container.ItemIndex) %>'>
                <td><%# Container.ItemIndex + 1 %></td>
                <td><%# Eval("CUSTOMER_NAME") %></td>
                <td><%# Eval("RENT_TYPE_NAME") %></td>
                <td><%# Eval("PHONE") %></td>
                <td class="text-right"><%# string.Format("{0:0,0}", Convert.ToDecimal(Eval("FEE_PER_DAY")) * 10) %></td>
                <td class="text-right"><%# Eval("PAYED_TIME") %> lần</td>
                <td class="text-right"><%# Eval("NOTE") %></td>
                <td class="text-center <%# ShowClass(Convert.ToDateTime(Eval("PAY_DATE"))) %>"><%# Convert.ToDateTime(Eval("PAY_DATE")).ToString("dd/MM/yyyy") %> <br />(<%# Eval("OVER_DATE") %>Ngày)
                    <asp:HiddenField ID ="hdfOverDay" Value='<%# Eval("OVER_DATE") %>' runat="server" />
                </td>
                <td>
                    <asp:HyperLink ID="hplUpdateContract" CssClass="text-center" runat="server" Text='<%# Eval("CONTRACT_NO")%>' NavigateUrl='<%# Eval("ID","FormContractUpdate.aspx?ID={0}") %>'></asp:HyperLink></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
           </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:DropDownList ID="ddlPager" runat="server" CssClass="form-control dropdown-pager-width" OnSelectedIndexChanged="ddlPager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    <asp:HiddenField ID="hfPager" runat="server" />
    <script>
        $(function () {
            $('#<%=txtDate.ClientID %>').datepicker();

            $('#<%=ddlPager.ClientID %>').change(function () {
                $('#<%=hfPager.ClientID %>').val($(this).val());
            });

            $('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                    return false;
                }
            });
        });
    </script>
</asp:Content>
