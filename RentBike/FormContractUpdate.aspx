﻿<%@ Import Namespace ="RentBike.Common" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="FormContractUpdate.aspx.cs" Inherits="RentBike.FormContractUpdate" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4 class="text-center">Chi tiết hợp đồng</h4>
    <%--<asp:Panel ID="pnlCustomerCheck" runat="server">--%>
    <asp:Panel ID="pnlTable" runat="server">
        <table class="table table-striped table-hover ">
            <tbody>
                <tr>
                    <td>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="btn btn-primary" OnClick="btnSearch_Click" /></td>
                    <td>
                        <asp:Button ID="btnNew" runat="server" Text="Làm hợp đồng" CssClass="btn btn-primary" OnClick="btnNew_Click" /></td>
                </tr>
            </tbody>
        </table>
        <table class="table table-striped table-hover" style="width: 70%; margin-left: 10%;">
            <tbody>
                <tr>
                    <td colspan="2" class="text-center"><strong>
                        <asp:Label ID="lblMessage" runat="server" Text="" CssClass="text-center text-warning"></asp:Label></strong></td>
                </tr>
                <tr class="info">
                    <td colspan="2" class="text-center"><strong>Thông tin khách hàng</strong></td>
                </tr>
                <tr>
                    <td class="text-right">Tên khách hàng</td>
                    <td>
                        <asp:HiddenField ID="hdfCus" runat="server" />
                        <asp:TextBox ID="txtCustomerName" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="text-right">Ngày sinh</td>
                    <td>
                        <asp:TextBox ID="txtBirthDay" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">Số CMND/Số GPLX</td>
                    <td>
                        <asp:TextBox ID="txtLicenseNumber" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">Ngày cấp</td>
                    <td>
                        <asp:TextBox ID="txtRangeDate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">Nơi cấp</td>
                    <td>
                        <asp:TextBox ID="txtPlaceDate" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">Số điện thoại</td>
                    <td>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="text-right">Hộ khẩu thường trú</td>
                    <td>
                        <asp:TextBox ID="txtPermanentResidence" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="text-right">Hiện trú tại</td>
                    <td>
                        <asp:TextBox ID="txtCurrentResidence" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr class="success">
                    <td colspan="2" class="text-center"><strong>Thông tin hợp đồng</strong></td>
                </tr>
                <%if (!IsNewContract)
                    {%>
                <tr>
                    <td class="text-right">Số hợp đồng</td>
                    <td>
                        <asp:TextBox ID="txtContractNo" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <%} %>
                <tr>
                    <td class="text-right">Loại hình</td>
                    <td>
                        <asp:DropDownList ID="ddlRentType" runat="server" CssClass="form-control rent-type-extra" onchange="onRentTypeChange();"></asp:DropDownList>
                        <asp:Button ID="btnLowRecoverability" runat="server" Text="Khả năng thu hồi thấp" CssClass="btn btn-primary low-recoverability" OnClick="btnLowRecoverability_Click" CommandArgument="LowRecoverability" />
                    </td>
                </tr>
                <tr>
                    <td class="text-right">Cửa hàng</td>
                    <td>
                        <asp:DropDownList ID="ddlStore" runat="server" CssClass="form-control"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="text-right">Giá trị tài sản</td>
                    <td>
                        <asp:TextBox ID="txtAmount" ClientIDMode="Static" runat="server" CssClass="form-control input-sm text-right"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="text-right">Phí thuê/1 ngày</td>
                    <td>
                        <asp:HiddenField ID="hdfFeeRate" ClientIDMode="Static" runat="server" />
                        <asp:TextBox ID="txtFeePerDay" ClientIDMode="Static" runat="server" CssClass="form-control input-sm text-right"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="text-right">Ngày thuê</td>
                    <td>
                        <asp:TextBox ClientIDMode="Static" ID="txtRentDate" runat="server" CssClass="form-control input-sm text-right"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">Ngày hết hạn</td>
                    <td>
                        <asp:TextBox ClientIDMode="Static" ID="txtEndDate" runat="server" CssClass="form-control input-sm text-right"></asp:TextBox>
                    </td>
                </tr>
                <tr class="select-row">
                    <td class="text-right">Lịch trả phí</td>
                    <td>
                        <asp:Repeater ID="rptPayFeeSchedule" runat="server">
                            <ItemTemplate>
                                <div style="display: inline-block">
                                    <a href="FormInOutUpdate.aspx?ID=<%# Eval("ID")%>">Trả phí ngày <%# String.Format("{0:dd/MM/yyyy}", Eval("PAY_DATE"))%></a>
                                    <asp:Label ID="lblBlue" runat="server" Visible='<%# ShowBlueImage(Convert.ToDecimal(Eval("AMOUNT_PER_PERIOD")), Convert.ToDecimal(Eval("ACTUAL_PAY"))) %>'>
                                            &nbsp;&nbsp;&nbsp;<img src="App_Themes/Theme1/image/tick-blue.png" />
                                            &nbsp;Đã trả phí
                                    </asp:Label>
                                    <asp:Label ID="lblOrange" runat="server" Visible='<%# ShowOrangeImage(Convert.ToDecimal(Eval("AMOUNT_PER_PERIOD")), Convert.ToDecimal(Eval("ACTUAL_PAY"))) %>'>
                                        &nbsp;&nbsp;&nbsp;<img src="App_Themes/Theme1/image/tick-orange.png" />&nbsp;
                                    Còn thiếu&nbsp;<%# RentBike.Common.Helper.FormatedAsCurrency(Convert.ToDecimal(Eval("AMOUNT_PER_PERIOD")) -  Convert.ToDecimal(Eval("ACTUAL_PAY")))%> VNĐ
                                    </asp:Label>
                                </div>
                                <br />
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">Ghi chú</td>
                    <td>
                        <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr class="warning">
                    <td colspan="2" class="text-center"><strong>Thông tin thêm</strong></td>
                </tr>
                <tr>
                    <td class="text-right">Người xác minh</td>
                    <td>
                        <asp:TextBox ID="txtReferencePerson" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trItemName">
                    <td class="text-right">Loại xe</td>
                    <td>
                        <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trLicenseNo">
                    <td class="text-right">Biển kiểm soát</td>
                    <td>
                        <asp:TextBox ID="txtItemLicenseNo" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trSerial1">
                    <td class="text-right">Số khung</td>
                    <td>
                        <asp:TextBox ID="txtSerial1" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trSerial2">
                    <td class="text-right">Số máy</td>
                    <td>
                        <asp:TextBox ID="txtSerial2" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trImplementer">
                    <td class="text-right">Người làm</td>
                    <td>
                        <asp:TextBox ID="txtImplementer" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trDocuments">
                    <td class="text-right">Giấy tờ để lại</td>
                    <td>
                        <asp:TextBox ID="txtBackDocument" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trReferencePhone">
                    <td class="text-right">Số điện thoại gia đình</td>
                    <td>
                        <asp:TextBox ID="txtReferencePhone" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trSchool">
                    <td class="text-right">Trường</td>
                    <td>
                        <asp:TextBox ID="txtSchool" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trClass">
                    <td class="text-right">Lớp</td>
                    <td>
                        <asp:TextBox ID="txtClass" runat="server" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr id="trPhoto">
                    <td class="text-right">Ảnh lưu trữ</td>
                    <td>
                        <asp:FileUpload ID="fileUploadPhoto" runat="server" AllowMultiple="true" CssClass="form-control input-sm"></asp:FileUpload>
                        <asp:Literal ID="litPhoto" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="text-right">Chi tiết </td>
                    <td>
                        <asp:TextBox ID="txtItemDetail" runat="server" TextMode="MultiLine" CssClass="form-control input-sm"></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnSave" runat="server" Text="Lưu & thoát" CssClass="btn btn-primary" OnClientClick="return showModal();" OnClick="btnSave_Click" />
                        &nbsp;<asp:Button ID="btnFinishContract" runat="server" Text="Thanh lý HĐ" CssClass="btn btn-primary" OnClick="btnFinishContract_Click" />
                        &nbsp;<asp:Button ID="btnCancel" runat="server" Text="Quay lại" CssClass="btn btn-primary" OnClick="btnCancel_Click" />
                    </td>
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
    <!-- Add Button helper (this is optional) -->
    <link rel="stylesheet" type="text/css" href="script/fancybox/helpers/jquery.fancybox-buttons.css?v=1.0.5" />
    <script type="text/javascript" src="script/fancybox/helpers/jquery.fancybox-buttons.js?v=1.0.5"></script>
    <script>
        $(document).ready(function () {
            $('#<%=txtAmount.ClientID %>').keyup(function () {
                calculateContractFee($(this).val());
            });

            $('#<%=ddlRentType.ClientID %>').change(function () {
                var value = $('#<%=txtAmount.ClientID %>').val();
                calculateContractFee(value);
            });

            $('#<%=txtRentDate.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                onSelect: function (dateStr) {
                    var date = $(this).datepicker('getDate');
                    if (date) {
                        date.setDate(date.getDate() + 29);
                        $('#<%=txtEndDate.ClientID %>').val($.datepicker.formatDate('dd/mm/yy', date));
                    }
                }
            });
            $('#<%=txtEndDate.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                onSelect: function (dateStr) {
                    var date = $(this).datepicker('getDate');
                    if (date) {
                        date.setDate(date.getDate() - 29);
                        $('#<%=txtRentDate.ClientID %>').val($.datepicker.formatDate('dd/mm/yy', date));
                    }
                }
            });
            $('#<%=txtRangeDate.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "1920:" + new Date().getFullYear()
            });

            $('#<%=txtBirthDay.ClientID %>').datepicker({
                dateFormat: 'dd/mm/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: "1920:" + new Date().getFullYear()
            });

            $('#<%=txtAmount.ClientID %>').priceFormat({ prefix: '', suffix: '', centsLimit: 0 });
            $('#<%=txtFeePerDay.ClientID %>').priceFormat({ prefix: '', suffix: '', centsLimit: 0 });

            $('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSearch.ClientID %>').click();
                    return false;
                }
            });

            var rentTypeId = '<%= RentTypeID%>';
            hideToRentType(rentTypeId);

            $('input, textarea').not('#<%=txtSearch.ClientID %>').keypress(function (e) {
                if (e.which == 13) {
                    $('#<%=btnSave.ClientID %>').click();
                    return false;
                }
            });

            /*
             *  Button helper. Disable animations, hide close button, change title type and content
             */

            $('.fancybox-buttons').fancybox({
                openEffect: 'none',
                closeEffect: 'none',

                prevEffect: 'none',
                nextEffect: 'none',

                closeBtn: false,

                helpers: {
                    title: {
                        type: 'inside'
                    },
                    buttons: {}
                },

                afterLoad: function () {
                    this.title = 'Image ' + (this.index + 1) + ' of ' + this.group.length + (this.title ? ' - ' + this.title : '');
                }
            });
        });

        function hideToRentType(rentTypeId) {
            if (parseInt(rentTypeId) == 3) {
                $('#trReferencePhone, #trSchool, #trClass').hide();
                $('#trItemName, #trLicenseNo, #trSerial1, #trSerial2, #trImplementer, #trDocuments').hide();
            } else if (parseInt(rentTypeId) == 2) {
                $('#trItemName, #trLicenseNo, #trSerial1, #trSerial2, #trImplementer, #trDocuments').hide();
                $('#trReferencePhone, #trSchool, #trClass').show();
                $('#trReferencePhone td').first().text('Số điện thoại gia đình');
            } else {
                $('#trItemName, #trLicenseNo, #trSerial1, #trSerial2, #trImplementer, #trDocuments').show();
                $('#trReferencePhone, #trSchool, #trClass').hide();
                $('#trReferencePhone td').first().text('Số điện thoại');
            }
        }

        function onRentTypeChange() {
            var rentTypeId = $("[id='<%=ddlRentType.ClientID %>'] option:selected").val();
            hideToRentType(rentTypeId);

        }

        var RentCarFeePerDay = parseInt('<%=RentCarFeePerDay%>');
        var RentEquipFeePerDay = parseInt('<%=RentEquipFeePerDay%>');
        var RentOtherFeePerDay = parseInt('<%=RentOtherFeePerDay%>');

        function calculateContractFee(value) {
            if (value != undefined && value != '') {
                var rentTypeId = $('#<%= ddlRentType.ClientID%>').val();
                var amount = value.replace(/\,/g, '');
                var multipleFee = Math.floor(parseInt(amount) / 100000);

                if (multipleFee > 0) {
                    switch (rentTypeId) {
                        case '1':
                            $('#<%=txtFeePerDay.ClientID %>').val(Math.round(multipleFee * RentCarFeePerDay)).priceFormat({ prefix: '', suffix: '', centsLimit: 0 });
                            break;
                        case '2':
                            $('#<%=txtFeePerDay.ClientID %>').val(Math.round(multipleFee * RentEquipFeePerDay)).priceFormat({ prefix: '', suffix: '', centsLimit: 0 });
                            break;
                        default:
                            $('#<%=txtFeePerDay.ClientID %>').val(Math.round(multipleFee * RentOtherFeePerDay)).priceFormat({ prefix: '', suffix: '', centsLimit: 0 });
                            break;
                    }
                }
                else
                    $('#<%=txtFeePerDay.ClientID %>').val(Math.round(multipleFee * RentOtherFeePerDay)).priceFormat({ prefix: '', suffix: '', centsLimit: 0 });
            }
        }

        var permission = '<%=(int)PERMISSION%>';
        var isNewContract = '<%=IsNewContract%>';
        function showModal() {
            if (permission === '3') {
                if (isNewContract === 'False') {
                    $("#myModal").modal('show');
                    return false;
                }
            }
            return true;
        }
    </script>
</asp:Content>
