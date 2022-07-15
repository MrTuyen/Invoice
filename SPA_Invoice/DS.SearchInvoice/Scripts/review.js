(function ($) {
    'use strict';


    //Customize alert
    var ALERT_TITLE = "Thông báo";
    var ALERT_BUTTON_TEXT = "Ok";
    var CONFIRM_OK_BUTTON_TEXT = "Đồng ý";
    var CONFIRM_CC_BUTTON_TEXT = "Bỏ qua";
    var CURRENT_URL = window.location.href;

    var searchInvoice = function () {
        var issearch_by_code = ($('[name="lb_search_invoice"]:checked').attr('id') === "lb_search_by_symbol") ? true : false;

        //Tra cứu theo mã
        if (issearch_by_code) {
            var keyword = $('[name="keyword"]').val();
            if (keyword == null || keyword.trim() == '') {
                alert("Mã tra cứu không được để trống. <br>Vui lòng nhập mã tra cứu.");
                return false;
            }

            //Tiến hành tra cứu theo mã
            var isLoading = $('.toggle-show');
            isLoading.toggleClass('d-none');

            var data = JSON.stringify({
                id: keyword
            });
            $.ajax({
                url: appUri + "/tracuu/search_by_code",
                type: "POST",
                headers: {
                    "X-Requested-With": "Novaon-DS",
                    "Token-String": "",
                    "Device-UUID": "",
                },

                timeout: 60000,
                cache: true,
                crossDomain: true,
                contentType: "application/json; charset=utf-8;",
                dataType: "json",
                data: data,
                processData: true,
                async: true,
                tryCount: 0,
                retryLimit: 3,

                success: function (response) {
                    if (response && response.rs && response.msg) {
                        var obj = response.msg;

                        //Show lên cho xem
                        showPopupFileView(obj);
                    } else {
                        alert("Mã tra cứu không đúng. Vui lòng xem lại. <br><br>Mã tra cứu được tạo ra tự động bởi hệ thống OnFinance.asia của NOVAON và được ghi dưới mỗi hóa đơn điện tử.");
                    }

                    //Hide loading
                    isLoading.toggleClass('d-none');
                },

                error: function (error) {
                    isLoading.toggleClass('d-none');
                }
            });
        } else {
            //Tra cứu theo file
            searchByFile();
        }
    }

    // Hàm hỗ trợ tra cứu theo QR code
    var getUrlVars = function (url) {
        var vars = [], hash;
        var hashes = url.slice(url.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    };
    var code = getUrlVars(CURRENT_URL);
    if (code.referencecode != null) {
        $('[name="keyword"]').val(code.referencecode);
        searchInvoice();
    }
    // --------------------------------

    var removeCustomAlert = function () {
        document.getElementsByTagName("body")[0].removeChild(document.getElementById("modalContainer"));
    }

    if (document.getElementById) {
        window.alert = function (txt) {
            createCustomAlert(txt, null);
        }
        window.confirm = function (txt, callback) {
            createCustomAlert(txt, callback);
        }
    }

    function createCustomAlert(txt, callback) {
        var d = document;

        if (d.getElementById("modalContainer")) return;

        var mObj = d.getElementsByTagName("body")[0].appendChild(d.createElement("div"));
        mObj.id = "modalContainer";
        mObj.style.height = d.documentElement.scrollHeight + "px";

        var alertObj = mObj.appendChild(d.createElement("div"));
        alertObj.id = "alertBox";
        if (d.all && !window.opera) alertObj.style.top = document.documentElement.scrollTop + "px";
        alertObj.style.left = (d.documentElement.scrollWidth - alertObj.offsetWidth) / 2 + "px";
        alertObj.style.visiblity = "visible";

        var h1 = alertObj.appendChild(d.createElement("h1"));
        h1.appendChild(d.createTextNode(ALERT_TITLE));

        var msg = alertObj.appendChild(d.createElement("p"));
        //msg.appendChild(d.createTextNode(txt));
        var find = '\n';
        var re = new RegExp(find, 'g');

        //txt = txt.replace(/""/g, '<br>');

        msg.innerHTML = txt;



        if (callback) {
            var div = alertObj.appendChild(d.createElement("div"));
            div.className = "btnGroup";

            var btn2 = div.appendChild(d.createElement("button"));
            btn2.id = "cancelBtnAlert";
            btn2.className = "btn btn-default";
            btn2.appendChild(d.createTextNode(CONFIRM_CC_BUTTON_TEXT));
            btn2.href = "#";
            btn2.onclick = function () { removeCustomAlert(); callback(false); return false; }

            var btn = div.appendChild(d.createElement("button"));
            //btn.id = "closeBtnAlert";
            btn.className = "btn btn-success";
            btn.appendChild(d.createTextNode(CONFIRM_OK_BUTTON_TEXT));
            btn.href = "#";
            btn.onclick = function () { removeCustomAlert(); callback(true); return false; }
        } else {
            var btn = alertObj.appendChild(d.createElement("button"));
            btn.id = "closeBtnAlert";
            btn.className = "btn btn-success";
            btn.appendChild(d.createTextNode(ALERT_BUTTON_TEXT));
            btn.href = "#";
            btn.onclick = function () { removeCustomAlert(); return false; }
        }


        alertObj.style.display = "block";

    }

    $('.navbar-xbootstrap, .cover-bg').click(function () {
        $('.nav-xbootstrap').toggleClass('visible');
        $('.cover-bg').toggle();
    });

    $('[name="lb_search_invoice"]').change(function () {
        var frm_text = $('.frm-search-text');
        var frm_file = $('.frm-search-file');
        $('[name="keyword"], [name="file-xml"]').val('');

        if ($('[name="lb_search_invoice"]:checked').attr('id') === "lb_search_by_symbol") {
            frm_text.show();
            frm_file.hide();
        } else {
            frm_text.hide();
            frm_file.show();
        }
    });

    $('.frm-search-file').click(function () {
        $('#input_select_file').trigger('click');
    });

    var globalInvoiceObj = null;
    var showPopupFileView = function (obj) {
        console.log(obj);
        globalInvoiceObj = obj;
        if (obj.SIGNEDLINK) {
            //Lưu mã hóa đơn tìm được
            $('#invoice_id').val(obj.ID);

            $('#popupViewInvoice').dialog({
                modal: true,
                resizable: false,

                open: function (data, e, f) {
                    $('#frViewFileInvoice').attr('src', obj.SIGNEDLINK);
                    $(this).dialog("option", "title", "Hóa đơn của: " + obj.CUSNAME);

                    // kiểm tra hóa đơn đã phát hành chưa (Nếu chưa thì ẩn nút chuyển hóa đơn giấy đi)
                    if (obj.INVOICESTATUS === 2 && (obj.INVOICETYPE != 4 && obj.INVOICETYPE != 3)) {
                        // kiểm tra tồn tại nút chuyển đổi chưa
                        var btnConvert = $('#btnConvertInvoice');
                        if (btnConvert == undefined || btnConvert.length == 0) {
                            //thêm nút thực hiện chức năng chuyển đổi hóa đơn điện tử sang hóa đơn giấy
                            var btnConvertInvoice = $('<button id ="btnConvertInvoice" class="btn btn-warning convert-invoice-paper" role="button" aria-haspopup="true" aria-expanded="false">Chuyển thành hóa đơn giấy</button>');
                            btnConvertInvoice.appendTo($('[aria-describedby="popupViewInvoice"] .ui-dialog-titlebar'));

                            $('#btnConvertInvoice').click(function (e) {
                                showContentInvoicePaper();
                                stopEvent(e);
                            });
                        }
                    }
                    else {
                        $('#btnConvertInvoice').remove();
                    }

                    //Xóa nút download trước đó mỗi khi tìm kiếm lại
                    $('#downloadFileInvoice').remove();
                    var dropGroup = $('<div class="dropdown" id="downloadFileInvoice" />').appendTo($('[aria-describedby="popupViewInvoice"] .ui-dialog-titlebar'));
                    var dropButton = $('<button class="btn btn-success dropdown-toggle" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Tải file</button>').appendTo(dropGroup);
                    var menuDown = $('<div class="dropdown-menu" aria-labelledby="dropdownMenuLink" />').appendTo(dropGroup);

                    //Link tải file pdf
                    var downPdf = $('<a class="dropdown-item download-link-template-pdf">Tải file PDF</a>').attr({
                        target: '_blank',
                        href: appUri + "/tracuu/downloadfile?link=" + obj.PREVIEWLINK
                    }).appendTo(menuDown);

                    //Link tải file xml
                    var downXml = $('<a class="dropdown-item">Tải file XML</a>').attr({
                        target: '_blank',
                        href: appUri + "/tracuu/downloadfile?link=" + obj.SIGNEDXML
                    }).appendTo(menuDown);

                    //Link tải file chứng chỉ chữ ký số
                    var downCer = $('<a class="dropdown-item">Tải chứng chỉ</a>').attr({
                        target: '_blank',
                        href: appUri + "/tracuu/downloadfile?link=" + obj.CERTIFICATELINK
                    }).appendTo(menuDown);
                }
            });
        } else {
            alert("Không tìm thấy hóa đơn bạn cần xem. <br>Vui lòng hỏi lại bên bán để xác nhận hóa đơn đã được ký hay chưa.");
        }
    }

    /*
     * Dừng dom event
     */
    var stopEvent = function (e) {
        if (e.stopPropagation) {
            e.stopPropagation();
            e.preventDefault();
        }
    }

    /*
     * show popup nhập thông tin người thực hiện chuyển đổi hóa đơn 
     */
    var showContentInvoicePaper = function () {
        $('#txtConverter').val('');
        $('#txtConverter').focus();
        $('#btnContine').show();
        $('#btnCancel').show();
        $('#popupContentInvoicePaper').dialog({
            modal: true,
            resizable: false,
            width: 380,
            height: 184,
            open: function () {
                $(this).dialog("option", "title", "CHUYỂN THÀNH HÓA ĐƠN GIẤY");
            }
        });
    }

    // Xử lý nút Tiếp tục, Hủy bỏ trên Popup Chuyển thành hóa đơn giấy
    $('#btnCancel').click(function (e) {
        $('#popupContentInvoicePaper').dialog('close');
        $('#txtConverter').removeClass('required');
        $('.text-required').hide();
    });

    // Nhấn tiếp tục chuyển thành hóa đơn giấy
    $('#btnContine').click(function (e) {
        var converter = $('#txtConverter').val();
        if (converter == '') {
            $('#popupContentInvoicePaper').css('height', '196px');
            $('#txtConverter').addClass('required');
            $('.text-required').show();
            $('#txtConverter').focus();
        }
        else {
            convertInvoicePaper();
        }
    });

    // xử lý khi người dùng nhấn nút enter
    $('#popupContentInvoicePaper').bind('keypress', function (e) {
        if (e.keyCode == 13) {
            var converter = $('#txtConverter').val();
            if (converter == '') {
                $('#txtConverter').addClass('required');
                $('.text-required').show();
                $('#txtConverter').focus();
            }
            else {
                convertInvoicePaper();
            }
        }
    });

    // show thông tin chuyển đổi hóa đơn điện tử thành hóa đơn giấy
    var convertInvoicePaper = function () {
        $('#btnContine').hide();
        $('#btnCancel').hide();
        //Tiến hành gọi cuống Server
        var isLoading = $('.toggle-show');
        isLoading.toggleClass('d-none');

        var data = JSON.stringify({
            id: $('#invoice_id').val(),
            fullName: $('#txtConverter').val(),
            invoice: globalInvoiceObj
        });

        $.ajax({
            url: appUri + "/tracuu/convert_invoice",
            type: "POST",
            headers: {
                "X-Requested-With": "Novaon-DS",
                "Token-String": "",
                "Device-UUID": "",
            },

            timeout: 60000,
            cache: true,
            crossDomain: true,
            contentType: "application/json; charset=utf-8;",
            dataType: "json",
            data: data,
            processData: true,
            async: true,
            tryCount: 0,
            retryLimit: 3,
            success: function (response) {
                if (response) {
                    if (response.rs) {
                        //Chuyển thành công thì remove button chuyển để tránh chuyển lần nữa
                        $('#btnConvertInvoice').remove();
                        //Gán địa chỉ download cho button Tải file
                        $('.download-link-template-pdf').attr("href", appUri + "/tracuu/downloadfile?link=" + response.linkDown);
                        //Reload lại iframe để show file sau chuyển đổi
                        $('#frViewFileInvoice').attr('src', response.linkView);

                        alert("Chuyển đổi thành công!");
                    } else {
                        alert(response.msg);
                        $('#btnContine').show();
                        $('#btnCancel').show();
                    }
                } else {
                    alert("Không thể chuyển đồi sang hóa đơn giấy, Vui lòng thử lại sau.");
                    $('#btnContine').show();
                    $('#btnCancel').show();
                }

                //Hide loading
                isLoading.toggleClass('d-none');
                $('#popupContentInvoicePaper').dialog('close');
            },

            error: function (error) {
                alert("Không thể chuyển đồi sang hóa đơn giấy, Vui lòng thử lại sau.");
                isLoading.toggleClass('d-none');
                $('#btnContine').show();
                $('#btnCancel').show();
            }
        });
    }

    // Xử lý Validate Textbox Tên người chuyển đổi
    $('#txtConverter').change(function (e) {
        var converter = $('#txtConverter').val();
        if (converter == '') {
            $('#txtConverter').addClass('required');
            $('.text-required').show();
        }
        else {
            $('#txtConverter').removeClass('required');
            $('.text-required').hide();
        }
    });

    $('#txtConverter').on("keypress", function (e) {
        $('#txtConverter').removeClass('required');
        $('.text-required').hide();
    });

    //var reloadIframe = function (obj) {
    //    var iframe = $('#popupViewInvoice iframe').attr('src');
    //    //thực show tab thông tin hóa đơn khi thực hiện chức năng chuyển thành hóa đơn giấy
    //    $('#popupViewInvoice iframe').attr('src', obj);
    //    // tải hóa đơn ra hóa đơn chuyển đổi đúng phải tải hóa đơn gốc
    //    $('#showPopupInvoicePaper').hide();
    //}

    $('#cmd_search').click(function () {
        searchInvoice();
    });


    //Tìm theo file
    $('#input_select_file').fileupload({
        autoUpload: false,
        add: function (e, data) {
            if (!data.files[0].type.match('text/xml')) {
                return;
            }

            //Hiển thị tên file data.files[0]
            $('[name="file-xml"]').val(data.files[0].name);

            //Tiến hành tra cứu theo mã
            var isLoading = $('.toggle-show');
            isLoading.toggleClass('d-none');

            var fileData = new FormData();
            fileData.append("file0", data.files[0]);
            $.ajax({
                type: "POST",
                url: appUri + "/tracuu/search_by_file",
                contentType: false,
                processData: false,
                data: fileData,
                success: function (result) {
                    if (result && result.rs && result.msg) {
                        var obj = result.msg;
                        showPopupFileView(obj);
                    } else {
                        alert(result.msg);
                    }

                    //Hide loading
                    isLoading.toggleClass('d-none');
                },
                error: function (xhr, status, p3, p4) {
                    isLoading.toggleClass('d-none');
                    alert("Không thể đọc file xml.");
                }
            });
        }
    });

})(jQuery);