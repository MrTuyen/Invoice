

function SetVietNameInterface(ctr) {
    ctr.datepicker("option", "monthNames", ['Tháng một', 'Tháng hai', 'Tháng ba', 'Tháng bốn', 'Tháng năm', 'Tháng sáu', 'Tháng bảy', 'Tháng tám', 'Tháng chín', 'Tháng mười', 'Tháng mười một', 'Tháng mười hai']);
    ctr.datepicker("option", "monthNamesShort", ['Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6', 'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12']);
    ctr.datepicker("option", "dayNamesShort", ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']);
    ctr.datepicker("option", "dayNamesMin", ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']);
    ctr.datepicker("option", "dayNames", ['Chủ nhật', 'Thứ hai', 'Thứ ba', 'Thứ tư', 'Thứ năm', 'Thứ sáu', 'Thứ bảy']);
}

function SetDateTimePicker(ctr) {
    ctr.datetimepicker({ maxDate: null, dateFormat: "dd/mm/yy" })
    SetVietNameInterface(ctr);
}

function SetDatePicker(ctr) {
    ctr.datepicker({ maxDate: null, dateFormat: "dd/mm/yy" })
    SetVietNameInterface(ctr);
}

function fdelFromAreaPrice(div) {
    if (!confirm('Xác nhận xóa?')) return false;

    var boxPrice = $("#" + $(div).attr("rel"));
    var idPrice = $(div).attr("data-remove");
    if (idPrice == "") {
        boxPrice.hide("fast", function () {
            boxPrice.remove();
        });
        return;
    }
    var randomUnique = "del" + Math.round(new Date().getTime() + (Math.random() * 100));
    $.ajax({
        url: "/Products/Deletespecial/" + randomUnique,
        type: "POST",
        data: { salepriceid: idPrice },
        success: function (result) {
            boxPrice.hide("fast", function () {
                boxPrice.remove();
            });
        },
        error: function (result) {
            alert("Lỗi???\n" + result.responseText);
        }
    });

    return true;
}

function onsubmitform(div) {
    var $form = $(div);
    var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
    var action = $form.attr("action") + "/" + randomUnique;
    $.ajax({
        url: action,
        type: "POST",
        data: $form.serializeArray(),
        success: function (result) {
            $form.find(".cmddel_special_price").attr("data-remove", result.responseId);
            $form.find('input[name="hSalepriceID"]').val(result.responseId);
            alert(result.responseText);
        },
        error: function (result) {
            alert("Lỗi truyền dữ liệu, thông tin chưa lưu.\n" + result.responseText);
        }
    });
    return false;
}

function autoSalePrice(input) {
    var $input = $(input);
    var percent = $input.val();
    var costprice = $input.parent().parent().find('input[name="COSTPRICE"]').val();
    var saleprice = (percent / 100) * costprice + parseInt(costprice);
    $input.parent().parent().find('input[name="SALEPRICE"]').val(Math.floor(saleprice / 100) * 100);
}

function autoPercentage(input) {
    var $input = $(input);
    var saleprice = parseInt($input.val());
    var costprice = parseInt($input.parent().parent().find('input[name="COSTPRICE"]').val());

    var oPercent = $input.parent().parent().find('input[name="PROFITPERCENTAGE"]');
    if ((saleprice - costprice) <= 0) {
        oPercent.val(0);
    } else {
        oPercent.val(((saleprice - costprice) / costprice) * 100);
    }
}

function autoSalePriceByCost(input) {
    var $input = $(input);
    var costprice = parseInt($input.val());
    var oPercent = $input.parent().parent().find('input[name="PROFITPERCENTAGE"]');
    var oSalePrice = $input.parent().parent().find('input[name="SALEPRICE"]');
    var percent = numeral(oPercent.val()).format('0,0.00');
    if (oPercent.val() == '') {
        oPercent.val(0);
    }
    var saleprice = (percent / 100) * costprice + costprice;
    oSalePrice.val(Math.floor(saleprice / 100) * 100);
}

function uniqueCheckRadio(radio) {
    var $this = $(radio);
    var oPrice = $this.parent().find('input[name="pricespecial"]');
    var valPrice = oPrice.val();
    $this.parent().parent().find('input[name="pricespecial"]').val(0);
    oPrice.val(valPrice);
}

function loadCustomer(input) {
    var cName = $("#CustomerName");
    var cAddr = $("#CustomerAddress");
    var cId = $("#CustomerId");
    if (cName.text() == "" && input.val() != "") {
        var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
        $.post("/SearchInputVoucher/GetCustomer", { custax: input.val(), randomUnique: randomUnique })
            .done(function (result) {
                if (result.responseName != "") {
                    cName.text(result.responseName);
                    cAddr.text(result.responseAddress);
                    cId.val(result.responseId);
                } else {
                    alert("Không thể tìm thấy nhà cung cấp có mã số thuế: " + input.val());
                    input.focus();
                }
            })
            .fail(function (result) {
                alert("Lỗi không thể tìm nhà cung cấp");
            });
    }
    return false;
}

function delTableRow(td) {
    //var productid = td.parent().find("input[name='ProductID']").val();
    if (!confirm("Bạn chắc chắn muốn xóa dòng này?"))
        return false;

    //td.parent().hide();
    td.parent().hide("fast", function () {
        td.parent().remove();
        if ($("#listRowDetail tr:not(:hidden)").length <= 0) {
            $("select[name='VATInput']").val(0);
            $("#VoucherDiscount").val(0);
            $("#totalVoucherMoney").val(0);
            $("#totalVATVoucher").val(0);
            $("#TotalAmountVoucher").val(0);
        }
        updateTotalMoney();
        updateTotalDiscount();
    });
}

var timerId;

function popupSelectOneProduct(jsobj, callback) {
    var obj = JSON.parse(jsobj);
    if (obj.length == 1) {
        callback(obj[0]);
    } else if (obj.length > 1) {
        var div = $('<div class="popupSelectOneProduct">');
        var i = 0;
        obj.forEach(function (o) {
            var guid = "id" + o.ProductID;
            var input = $('<input type="radio"/>').attr('id', guid).attr('name', 'UserSelectedProductID').val(i)
                .keyup(function (e) {
                    if (e.keyCode == 13) {
                        var selectedIndex = $(this).val();
                        callback(obj[selectedIndex]);
                        $('.popupSelectOneProduct').dialog("close");
                    }
                });
            var label = $('<label/>').attr('for', guid).text(" " + o.ProductName + "(" + o.UnitName + ")").css('color', '#188338').prepend(input);
            var p = $('<p/>').append(label);
            div.append(p);
            i++;
        });

        div.dialog({
            title: 'Chọn sản phẩm',
            modal: true,
            resizable: false,
            width: 600,
            height: 250,
            close: function () {
                $('.popupSelectOneProduct').remove();
            },
            buttons: [{
                text: "Ok",
                icons: {
                    primary: "ui-icon-heart"
                },
                click: function () {
                    var selectedIndex = $('input[name="UserSelectedProductID"]:checked').val();
                    callback(obj[selectedIndex]);
                    $(this).dialog("close");
                }
            }]
        });
    }
}

function loadProduct(input) {
    var _tr = input.parent().parent();
    var ProductID = input;
    var ProductName = _tr.find('span[data-field="ProductName"]');
    var UnitID = _tr.find('input[name="QUANTITYUNITID"]');
    var UnitName = _tr.find('span[data-field="UnitName"]');
    var Quantity = _tr.find('input[name="Quantity"]');
    var Price = _tr.find('span[data-field="Price"]');
    var ToMoney = _tr.find('input[name="ToMoney"]');
    var Discount = _tr.find('input[name="Discount"]');

    if (ProductName.text() == "" && input.val() != "") {
        var f = 0;
        $("#listRowDetail tr:not(:hidden) input[name='ProductID']").each(function (index) {
            if ($(this).val() == input.val()) f++;
        });
        if (f > 2) {
            alert("Sản phẩm đã tồn tại, vui lòng kiểm tra lại");
            input.val("");
            input.focus();
        } else {
            var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
            $.post("/SearchInputVoucher/GetProduct", { productid: input.val(), InputTypeInput: $("#InputTypeInput").val(), randomUnique: randomUnique })
                .done(function (result) {
                    if (result.success) {
                        var callback = function (o) {
                            input.val(o.ProductID.trim());
                            ProductName.text(o.ProductName);
                            UnitName.text(o.UnitName);
                            Quantity.val(1);
                            Price.val(0);
                            ToMoney.val(0);
                            Discount.val(0);
                            UnitID.val(o.UnitID);
                        };

                        var o = popupSelectOneProduct(result.jsobj, callback);

                        //$("td.button-new-rowdetail").trigger("click");

                    } else {
                        alert("Không tìm thấy sản phẩm: " + input.val());
                        input.focus();
                    }
                })
                .fail(function (result) {
                    alert("Lỗi không thể tìm sản phẩm");
                });
        }
    }
    return false;
}

function updateTotalMoney(input) {
    if (input != null) {
        var parent = input.parent().parent();
        var newQuantity = parseFloat(parent.find('input[name="Quantity"]').autoNumeric('get'));
        //var quantityunit = parent.find('span[data-field="UnitName"]').text();
        //if (quantityunit == 'Kg') {
        //    var oldQuantity = parseFloat(parent.find('input[name="oldQuantity"]').val());
        //    if (Math.abs(oldQuantity / newQuantity) > 0.5) {
        //        alert("");
        //    }
        //}
        var price = parseInt(parent.find('input[name="ToMoney"]').autoNumeric('get')) / newQuantity;
        parent.find("span[data-field='Price']").text($.number(price, 2));
    }
    var total = 0;
    $("#listRowDetail tr:not(:hidden) input[name='ToMoney']").each(function () {
        if ($(this).val() != '')
            total += parseInt($(this).autoNumeric('get'));
    });
    $("#sumToTalMoneyRow").val(total);
    updateSumMoneyVoucher();
}

function updateSumMoneyVoucher() {



    var sumVATVoucher = 0;
    var sumToTalMoneyRow = parseInt($("#sumToTalMoneyRow").val());
    var sumToTalDiscountRow = parseInt($("#sumToTalDiscountRow").val());
    var voucherDiscount = parseInt($("#VoucherDiscount").autoNumeric('get'));
    var VAT = parseInt($("select[name='VATInput']").val());

    var totalVoucherMoney = $("#totalVoucherMoney");
    var totalVATVoucher = $("#totalVATVoucher");
    var totalMoney = sumToTalMoneyRow - sumToTalDiscountRow - voucherDiscount;
    var totalMoneyProduct = sumToTalMoneyRow - sumToTalDiscountRow;
    $("#listRowDetail tr:not(:hidden)").each(function () {
        var ttsp = parseInt($(this).find("input[name='ToMoney']").autoNumeric('get'));
        var cksp = parseInt($(this).find("input[name='Discount']").autoNumeric('get'));
        var vatsp = parseInt($(this).find("select[name='VAT']").val());
        sumVATVoucher += ((vatsp / 100) * (ttsp - cksp - (voucherDiscount / sumToTalMoneyRow * ttsp)));

        //(1 - voucherDiscount / totalMoneyProduct)
    });
    //console.log(totalMoney);
    totalVoucherMoney.val(totalMoney);

    //var sumVATVoucher = (VAT / 100) * totalMoney;

    totalVATVoucher.val(sumVATVoucher);
    $("#TotalAmountVoucher").val(totalMoney + sumVATVoucher);
}

function updateTotalDiscount() {
    var discount = 0;
    $("tr:not(:hidden) input[name='Discount']").each(function () {
        if ($(this).val() != '') {

            discount += parseInt($(this).autoNumeric('get'));
        }
    });
    $("#sumToTalDiscountRow").val(discount);
    updateSumMoneyVoucher();
}

function checkVoucherDiscount(obj) {
    var vDiscount = parseInt($(obj).autoNumeric('get'));
    var vTotal = parseInt($("#sumToTalMoneyRow").val());
    if (vDiscount > vTotal) {
        alert("Chiết khấu (theo hóa đơn) phải nhỏ hơn hoặc bằng tổng thành tiền: ");
        setTimeout(function () { obj.focus() }, 10);
    }
}

function hiddenRow(btn) {
    if (confirm("Bạn chắc chắn muốn xóa dòng này?")) {
        btn.parent().hide(300, function () {
            $(this).remove();
        });
    }
    return false;
}

function getDecimalSeparator() {
    var n = 3 / 2;
    n = n.toLocaleString().substring(1, 2);
    return n;
}

//var cHamxuly = function () { };
function openPopupSelectProduct(_this, callback, storeid) {

    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    storeid = storeid == null ? 0 : storeid;

    $.post("/Common/SelectProduct", { storeid: storeid })
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn sản phẩm',
                modal: true,
                resizable: false,
                width: 1000,
                height: 650,
                close: function () {
                    btn.removeAttr("disabled");
                    $('.popupselectproduct').remove();
                    setTimeout(function () {
                        callbackExec(null);
                    }, 500);
                }
            });
            modal.html(result);
            //$("select#ddlpSubGroup").multiselect('refresh');
        });
}

var callbackExec = function (str) { };

function openPopupSelectItems(_this, callback) {

    var btn = $(_this);
    btn.attr("disabled", "disabled");
    callbackExec = callback;

    $.post("/Common/SelectItems")
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn Item',
                modal: true,
                resizable: false,
                width: 800,
                height: 670,
                close: function () {
                    btn.removeAttr("disabled");
                    $('.popupselectproduct').remove();
                    //setTimeout(function () {
                    //    callback(null);
                    //}, 500);
                }
            });
            modal.html(result);
        });
}

function openPopupSelectMarket(_this, params, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectMarket", params)
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn chợ',
                modal: true,
                resizable: false,
                width: 1000,
                height: 600,
                close: function () {
                    btn.removeAttr("disabled");

                    $('.popupselectproduct').remove();
                }
            });
            modal.html(result);
        });
}

function openPopupSelectCombo(_this, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectCombo")
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn Combo',
                modal: true,
                resizable: false,
                width: 1000,
                height: 600,
                close: function () {
                    btn.removeAttr("disabled");

                    $('.popupselectproduct').remove();
                }
            });
            modal.html(result);
        });
}

function openPopupSelectStore(_this, params, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectStore", params)
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn siêu thị',
                modal: true,
                resizable: false,
                width: 1000,
                height: 600,
                close: function () {
                    btn.removeAttr("disabled");

                    $('.popupselectproduct').remove();
                }
            });
            modal.html(result);
        });
}

var $POSloading = {};

function RefreshSession() {
    var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
    var action = "/Account/RefreshSession/" + randomUnique;
    $.post(action, {})
        .done(function (response) {
            if (response) {
                console.log(response.msg);
                //Nếu session null
                if (!response.rs) {
                    if (window.location.href.indexOf('Account/Login') == -1) {
                        window.location.reload();
                    }
                }
            } else {
                console.log("Lỗi không nhận được phản hồi - RefreshSession (js) ");
            }
        })
        .fail(function (result) {
            console.log(response.msg);
        });
}

$(function () {
    //Giữ session
    if (window.location.href.indexOf('Account/Login') == -1) {
        var stopRequest = setInterval(RefreshSession, 300000);
    }

    $('#modelLoading').modal({
        show: false,
        keyboard: false
    });
    $POSloading.show = function () {
        $('#modelLoading').modal('show');
    };
    $POSloading.hide = function () {
        $('#modelLoading').modal('hide');
    };

    //var lH = $("#leftContent").height();
    //$('#mainContent').resize(function () {
    //    var mH = $(this).height();
    //    if (lH < mH) {//$("#leftContent").height(mH);
    //        $("#leftContent").animate({
    //            height: mH
    //        }, 100);
    //    }
    //});
    //$('#mainContent').resize();

    /* DataTables */
    if ($('.dynamicTable').length > 0) {
        $('.dynamicTable').dataTable({
            "sPaginationType": "bootstrap",
            "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
            "oLanguage": {
                "sLengthMenu": "_MENU_ số dòng mỗi trang"
            }
        });
    }

    $("input[name*='typeprice_']").click(function () {
        uniqueCheckRadio(this);
    });

    $('form#frmSearchDoPrice').submit(function () {
        var input = $(this).find(".seachInput");
        if (input.val() == "") {
            alert("Vui lòng nhập mã sản phẩm hoặc tên sản phẩm cần tiềm kiếm");
            input.focus();
            return false;
        }
        return true;
    });

    $('#btnSubmitNewPermission').click(function () {
        var form = $("#frmAddPermission");
        if ($("#idPermission").val() == '') {
            alert("Chưa nhập hoặc nhập sai mã quyền");
            $("#idPermission").focus();
            return false;
        }
        if ($("#namePermission").val() == '') {
            alert("Chưa nhập tên quyền");
            $("#namePermission").focus();
            return false;
        }
        $.post("/Permission/GetPermissionById", form.serializeArray())
            .done(function (result) {
                if (result.perId != "") {
                    alert("Mã quyền đã tồn tại, vui lòng chọn mã quyền khác");
                    $("#idPermission").focus();
                    return false;
                } else
                    form.submit();
            }).fail(function (result) {
                alert("Lỗi không thể lưu thông tin");
                return false;
            });
        return false;
    });

    $(".cmddel_special_price").click(function () {
        return fdelFromAreaPrice(this);
    });

    $("a.cmdadnew_special_price").click(function () {
        var formhtml = $("#templateFrom").html();
        var hProductID = $(this).attr("data-product");
        var hPriceAreaID = $(this).attr("data-pricearea");
        var hrBeforarea = $("#" + $(this).attr("rel"));
        var formID = "frm_" + Math.round(new Date().getTime() + (Math.random() * 100));
        var $form = $("<form action='/Products/Special' method='post'></form>").addClass("frmajaxload").attr({ id: formID, onSubmit: "return onsubmitform(this)" }).append(formhtml);

        hrBeforarea.after($form);

        $("#" + formID + " a.cmddel_special_price").attr({ rel: formID, onclick: "fdelFromAreaPrice(this)" });
        $("#" + formID + " .row2 input[type='radio']").attr("name", "typeprice_" + formID);

        $("#" + formID + " input[data-id='tempradio1']").attr("id", "a_" + formID).click(function () { uniqueCheckRadio(this); });
        $("#" + formID + " label[data-id='tempradio1']").attr("for", "a_" + formID);

        $("#" + formID + " input[data-id='tempradio2']").attr("id", "b_" + formID).click(function () { uniqueCheckRadio(this); });
        $("#" + formID + " label[data-id='tempradio2']").attr("for", "b_" + formID);

        $("#" + formID + " input[data-id='tempradio3']").attr("id", "c_" + formID).click(function () { uniqueCheckRadio(this); });
        $("#" + formID + " label[data-id='tempradio3']").attr("for", "c_" + formID);

        //$("#" + formID + " input.datepicker_load").datepicker({ dateFormat: 'dd/mm/yy' });
        $("#" + formID + " input.timepicker_load").timepicker({ stepMinute: 5 });

        $("#" + formID + " input.pricespecial_load").number(true, 0);

        $("#" + formID + " input[name='htypeprice']").val("typeprice_" + formID);
        $("#" + formID + " input[name='hProductID']").val(hProductID);
        $("#" + formID + " input[name='hAreaID']").val(hPriceAreaID);


        SetDatePicker($("#" + formID + " input.datepicker_load"));
    });

    $('form.frmajaxload').submit(function () {
        return onsubmitform(this);
    });

    SetDatePicker($(".datepicker"));

    $('.timepicker').timepicker({
        timeFormat: 'HH:mm',
        stepMinute: 5
    });

    $('input[name="PROFITPERCENTAGE"]').number(true, 2);
    $('input.priceinput').number(true, 0);
    //$('input.checkpriceinput').number(true, 0);

    $("#btn_EditInputVoucher").click(function () {
        $("#btn_AcceptInputVoucher").hide();
        $("#btn_SaveInputVoucher").show();
        $("#btn_CacelEditInputVoucher").show();
        $(".hidden-col").show();
        $(".stt-col").hide();
        $(this).remove();

        $(".text_editable").removeAttr('disabled');
        $("#CustomerTax").keyup(function (e) {
            var cName = $("#CustomerName");
            var cAddr = $("#CustomerAddress");
            var cId = $("#CustomerId");
            cName.text("");
            cAddr.text("");
            cId.val("");
            if (e.keyCode == 13) {
                $(this).blur();
            }
        }).focusout(function () {
            loadCustomer($(this));
        });

        return false;
    });
    $("#btn_EditInputAcceptVoucher").click(function () {
        $("#btn_AcceptInputVoucher").hide();
        $("#btn_AcceptStockTransfer").hide();
        $("#btn_SaveInputAcceptVoucher").show();
        $("#btn_CacelEditInputAcceptVoucher").show();
        //$(".hidden-col").show();
        //$(".stt-col").hide();
        $(this).remove();
        $(".text_editaccept").removeAttr('disabled');


        return false;
    });

    $("td.button-new-rowdetail").click(function () {
        var template = $("table tr#rowTemplate:first").clone();
        template.removeAttr("id");
        template.find("td.button-delete-rowdetail").click(function () { delTableRow($(this)); });
        template.find('input[name="ProductID"]').keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).parent().parent().find('input[name="Quantity"]').focus();
            } else {
                $(this).parent().parent().find('span[data-field="ProductName"]').text("");
                //$(this).parent().parent().find('input[name="VAT"]').val(0);
                $(this).parent().parent().find('span[data-field="UnitName"]').text("");
                $(this).parent().parent().find('input[name="Quantity"]').val(0);
                $(this).parent().parent().find('span[data-field="Price"]').text("");
                $(this).parent().parent().find('input[name="ToMoney"]').val(0);
                $(this).parent().parent().find('input[name="Discount"]').val(0);
            }
        }).focusout(function () {
            loadProduct($(this));
        });
        template.find('select[name="VAT"]').change(function (e) {
            $('input[name="VAT"]').val($(this).val());
            updateSumMoneyVoucher();
        })
            .val($('select[name="VATInput"]').val());

        template.find('input[name="Quantity"], input[name="InQuantity"], input[name="OutQuantity"]').keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).parent().parent().find('input[name="ToMoney"]').focus().select();;
            }
        }).focusout(function () {
            updateTotalMoney($(this));
        }).autoNumeric('init', { mDec: 3, vMin: 0, vMax: 999999 });

        template.find('input[name="ToMoney"]').focusout(function () {
            updateTotalMoney($(this));
        }).keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).parent().parent().find('input[name="Discount"]').focus().select();;
            }
        }).autoNumeric('init', { mDec: 0, vMin: 0, vMax: 9999999999 });
        template.find('input[name="Discount"]').focusout(function () {
            if (parseInt($(this).autoNumeric('get')) > parseInt($(this).parent().parent().find('input[name="ToMoney"]').autoNumeric('get'))) {
                alert("Tiền chiết khấu theo sản phẩm phải nhỏ hơn hoặc bằng thành tiền sản phẩm.");
                $(this).val(0);
                setTimeout(function () { $(this).focus() }, 10);
            }
            updateTotalDiscount();
        }).keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).blur();
            }
        }).autoNumeric('init', { mDec: 0, vMin: 0 });

        $("#listRowDetail").append(template);
    });

    $(".button-delete-rowdetail").click(function () {
        delTableRow($(this));
    });

    var keyold = '';
    $('input[name="ProductID"]').keydown(function (e) {
        keyold = $(this).val();
    });
    $('input[name="ProductID"]').keyup(function (e) {
        var parent = $(this).parent().parent();
        if (e.keyCode == 13) {
            parent.find('input[name="Quantity"]').focus();
        } else if (keyold != $(this).val()) {
            parent.find('span[data-field="ProductName"]').text("");
            //parent.find('input[name="VAT"]').val(0);
            parent.find('span[data-field="UnitName"]').text("");
            parent.find('input[name="Quantity"]').val(0);
            parent.find('span[data-field="Price"]').text("");
            parent.find('input[name="ToMoney"]').val(0);
            parent.find('input[name="Discount"]').val(0);
            parent.find('input[name="action"]').val(2);

            updateTotalMoney();
            updateTotalDiscount();
        }
    }).focusout(function () {
        loadProduct($(this));
    });

    $('input[name="Quantity"]').keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).parent().parent().find('input[name="ToMoney"]').focus().select();
        }
    }).focusout(function () {
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalMoney($(this));
    });

    $('input[name="ToMoney"]').focusout(function () {
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalMoney($(this));
    }).keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).parent().parent().find('input[name="Discount"]').focus().select();
        }
    });

    $('input[name="Discount"]').focusout(function () {
        if (parseInt($(this).autoNumeric('get')) > parseInt($(this).parent().parent().find('input[name="ToMoney"]').autoNumeric('get'))) {
            alert("Tiền chiết khấu theo sản phẩm phải nhỏ hơn hoặc bằng thành tiền sản phẩm.");
            $(this).val(0);
            setTimeout(function () { $(this).focus() }, 10);
        }
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalDiscount();
    }).keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    });

    $("#VoucherDiscount").keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    }).focusout(function () {
        updateSumMoneyVoucher();
    });

    $("select[name='VATInput']").change(function (e) {
        $('input[name="VAT"]').val($(this).val());
        updateSumMoneyVoucher();
    });

    $("select[name='VAT']").change(function (e) {
        $('input[name="VAT"]').val($(this).val());
        updateSumMoneyVoucher();
    });


});

$(function () {
    //Customize alert
    var ALERT_TITLE = 'Thông báo';
    var ALERT_BUTTON_TEXT = 'Đồng ý';
    var CONFIRM_OK_BUTTON_TEXT = 'Đồng ý';
    var CONFIRM_CC_BUTTON_TEXT = 'Bỏ qua';
    var removeCustomAlert = function () {
        document.getElementsByTagName('body')[0].removeChild(document.getElementById('modalContainer'));
    }

    if (document.getElementById) {
        window.alert = function (txt) {
            createCustomAlert(txt, null);
        }
        window.confirm = function (txt, title, button1, button2, callback) {
            createCustomAlert(txt, title, button1, button2, callback);
        }
    }

    function createCustomAlert(txt, title, button1, button2, callback) {
        if (!title)
            title = ALERT_TITLE;
        if (!button1)
            button1 = CONFIRM_CC_BUTTON_TEXT;
        if (!button2)
            button2 = CONFIRM_OK_BUTTON_TEXT;
        var d = document;

        if (d.getElementById('modalContainer'))
            return;

        var mObj = d.getElementsByTagName('body')[0].appendChild(d.createElement('div'));
        mObj.id = 'modalContainer';
        mObj.style.height = d.documentElement.scrollHeight + 'px';

        var alertObj = mObj.appendChild(d.createElement('div'));
        alertObj.id = 'alertBox';

        if (d.all && !window.opera) alertObj.style.top = document.documentElement.scrollTop + 'px';
        alertObj.style.left = (d.documentElement.scrollWidth - alertObj.offsetWidth) / 2 + 'px';
        alertObj.style.visiblity = 'visible';

        var h1 = alertObj.appendChild(d.createElement('h1'));
        h1.appendChild(d.createTextNode(title));

        var msg = alertObj.appendChild(d.createElement('p'));

        msg.innerHTML = txt;

        if (callback) {
            var div = alertObj.appendChild(d.createElement('div'));
            div.className = 'btnGroup';

            var btn2 = div.appendChild(d.createElement('button'));
            btn2.id = 'cancelBtnAlert';
            btn2.className = 'btn btn-light';
            btn2.appendChild(d.createTextNode(button1));
            btn2.href = '#';
            btn2.onclick = function () {
                removeCustomAlert(); callback(false);
                return false;
            }

            var btn = div.appendChild(d.createElement('button'));
            //btn.id = 'closeBtnAlert';
            btn.className = 'btn btn-success';
            btn.appendChild(d.createTextNode(button2));
            btn.href = '#';
            btn.onclick = function () {
                removeCustomAlert(); callback(true);
                return false;
            }
        } else {
            var btn = alertObj.appendChild(d.createElement('button'));
            btn.id = 'closeBtnAlert';
            btn.className = 'btn btn-success';
            btn.appendChild(d.createTextNode(ALERT_BUTTON_TEXT));
            btn.href = '#';
            btn.onclick = function () {
                removeCustomAlert();
                return false;
            }
        }

        alertObj.style.display = 'block';
    }
});
