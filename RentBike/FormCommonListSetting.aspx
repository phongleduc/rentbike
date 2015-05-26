<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormCommonListSetting.aspx.cs" Inherits="RentBike.FormCommonListSetting" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>CÀI ĐẶT DANH MỤC</h2>
    <table class="table table-striped table-hover ">
        <tbody>
            <tr>
                <td>
                    <asp:Button ID="btnRentTypeCreate" runat="server" Text="Tạo phân loại" CssClass="btn btn-primary" /></td>
            </tr>
        </tbody>
    </table>
    <div>
        <asp:Repeater ID="rptRentType" runat="server">
                <headertemplate>
                    <table id="tblRentType" class="table table-striped table-hover">
                      <thead>
                        <tr class="success">
                          <th>#</th>
                          <th>Tên phân loại</th>
                            <th>Trạng thái</th>
                        </tr>
                      </thead>
                      <tbody>
                </headertemplate>
                <itemtemplate>
                        <tr id='<%# Eval("ID") %>'>
                          <td><%# Container.ItemIndex + 1 %></td>
                          <td><%# Eval("NAME") %></td>
                            <td><%# Eval("ACTIVE") %></td>
                        </tr>                        
                </itemtemplate>
                <FooterTemplate>
                      </tbody>
                    </table>
                </FooterTemplate>
        </asp:Repeater>
        <asp:DropDownList ID="ddlPager" runat="server" CssClass="form-control dropdown-pager-width" OnSelectedIndexChanged="ddlPager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
    </div>
        <script>
        $(function () {
            //$.each($('#tblRentType tbody tr'), function () {
            //    $(this).attr('style', 'cursor:pointer');
            //    $(this).click(function () {
            //        location.href = "FormAccountUpdate.aspx?ID=" + $(this).attr('id');
            //    });
            //});
        });
    </script>
</asp:Content>
