<%@ Import Namespace="RentBike.Common" %>

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormInOutAndPeriodUpdate.aspx.cs" Inherits="RentBike.FormInOutAndPeriodUpdate" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlTable" runat="server">
        <h2 class="text-center">Cập nhật trả phí</h2>
        <table class="table table-striped table-hover" style="width: 50%; margin-left: 25%;">
            <tbody>
                <tr>
                    <td colspan="2" class="text-center"><strong>Chi tiết khoản trả phí ngày <%= InOutDate.ToString("dd/MM/yyyy") %></strong></td>
                </tr>
                <tr>
                    <td colspan="2" class="text-center"><strong>
                        <asp:Label ID="lblMessage" runat="server" Text="" CssClass="text-center text-warning"></asp:Label></strong></td>
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
                        <asp:Button ID="btnSave" runat="server" Text="Lưu & thoát" CssClass="btn btn-primary" OnClientClick="return showModal();" OnClick="btnSave_Click" />
                        &nbsp;&nbsp;<asp:Button ID="btnCancel" runat="server" Text="Quay lại" CssClass="btn btn-primary" OnClick="btnCancel_Click" /></td>
                </tr>
            </tbody>
        </table>
        <!-- Modal HTML -->
        <div id="myModal" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title">Bạn cần nhập tài khoản cửa hàng trưởng để tiếp tục</h4>
                    </div>
                    <div class="modal-body">
                        <table class="table">
                            <tbody>
                                <tr>
                                    <td>
                                        <strong>Tài khoản</strong>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Mật khẩu</strong>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control input-sm"></asp:TextBox>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="modal-footer">
                        <asp:LinkButton runat="server" ID="lnkSave" CssClass="btn btn-primary" OnClick="btnSave_Click" Text="Lưu & thoát"></asp:LinkButton>
                        <button type="button" class="btn btn-primary" data-dismiss="modal">Đóng</button>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <script>
        $(document).ready(function () {
            $('#<%=txtIncome.ClientID %>').priceFormat({ prefix: '', suffix: '', centsLimit: 0 });

            $('input, textarea').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSave.ClientID %>').click();
                        return false;
                }
            });

        });

        var permission = '<%=(int)PERMISSION%>';
        function showModal() {
            if (permission === '3') {
                $("#myModal").modal('show');
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
