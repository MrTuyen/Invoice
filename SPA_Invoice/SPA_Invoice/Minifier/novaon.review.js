(function ($) {
    'use strict';

    //Customize alert
    var ALERT_TITLE = "Thông báo";
    var ALERT_BUTTON_TEXT = "Ok";
    var CONFIRM_OK_BUTTON_TEXT = "Đồng ý";
    var CONFIRM_CC_BUTTON_TEXT = "Bỏ qua";
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

        txt = txt.replace(/(?:\\n)/g, '<br>');

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

    $('#input_select_file').change(function (e) {
        if (this.files && this.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                var path = $('#input_select_file').val().split('\\');
                $('[name="file-xml"]').val(path[path.length - 1]);

                //Tiến hành tra cứu theo file
                searchByFile();
            };

            reader.readAsDataURL(this.files[0]);
        }
    });

    $('#cmd_search').click(function () {
        var issearch_by_code = ($('[name="lb_search_invoice"]:checked').attr('id') === "lb_search_by_symbol") ? true : false;
        if (issearch_by_code) {
            var keyword = $('[name="keyword"]').val();
            if (keyword == null || keyword.trim() == '') {
                alert("Mã tra cứu không được để trống. \nVui lòng nhập mã tra cứu.");
                return false;
            }

            //Tiến hành tra cứu theo mã
            var isLoading = $('.toggle-show');
            isLoading.toggleClass('d-none');

            var data = JSON.stringify({
                id: keyword
            });
            $.ajax({
                url: "/tracuu/search_by_code",
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
                    if (response && response.rs) {
                        var wh = window.innerHeight - 100;
                        $('#popupViewInvoice').dialog({
                            modal: true,
                            resizable: false,
                            
                            open: function (data, e, f) {
                                //$('#frmResult').attr('src', url);
                                //if ($('.type .checked').attr('value') == "code") {
                                //    var transidParam = $('#txtCode').val().trim();
                                //    einvoiceSearch.GetInvoice(transidParam);
                                //}
                                //else if ($('.type .checked').attr('value') == "customner") {
                                //    var arrtemp = url.split("&Code=");
                                //    var transidParam = arrtemp[arrtemp.length - 1];
                                //    einvoiceSearch.GetInvoice(transidParam);
                                //}
                                //else {
                                //    $('.totalAmount').attr('data', $('.totalAmount').text().trim());
                                //    $('.totalAmount').html(parseFloat($('.totalAmount').text().trim()).toLocaleString());
                                //    var transidParam = $('#txtCode').val().trim();
                                //    einvoiceSearch.GetInvoice(transidParam);
                                //}

                                //$('#infomation-panel').css('height', h - 56);
                                //$('#history-panel').css('height', h - 47);
                                
                            }
                        });
                    } else {
                        alert("Mã tra cứu không đúng. Vui lòng xem lại.\n\nMã tra cứu được tạo ra tự động bởi hệ thống OnFinance.asia của NOVAON và được ghi dưới mỗi hóa đơn điện tử.");
                    }

                    //Hide loading
                    isLoading.toggleClass('d-none');
                },

                error: function (error) {
                    isLoading.toggleClass('d-none');
                    console.log(error);
                }
            });
        } else {
            searchByFile();
        }
    });

    //Tìm theo file
    var searchByFile = function () {
        var file = $('#fFile')[0].files[0];
        if (file) {
            if (file.name != null && file.name != "") {
                var arrFileName = file.name.split('.');
                var extension = arrFileName[arrFileName.length - 1];
                if (extension == "xml") {
                    var strInvData = "";
                    var fileReafer = new FileReader();
                    fileReafer.onload = function (event) {
                        strInvData = event.target.result;
                        //Validate cho truong hop mo file xml = notepad bi chen them ki tu ko nhin thay vao dau
                        if (strInvData != null && strInvData.length > 0 && strInvData[0] != "<") {
                            strInvData = strInvData.substring(1);
                        }
                        var captchaValue = "";
                        //if ($("#captchaContainer").css("display") == "block") {
                        //    if ($("#captcha_captchaTextbox").val() == "") {
                        //        $("#captcha_captchaTextbox").focus()
                        //        einvoiceSearch.ShowErrorMessage("Mã xác nhận không được để trống.");
                        //        error = "Mã xác nhận không được để trống.";
                        //    }
                        //    else {
                        //        captchaValue = $("#captcha_captchaTextbox").val();
                        //    }
                        //}
                        //var param, uri = "tra-cuu/GetXMLData";
                        //if (captchaValue != "") {
                        //    param = { invData: strInvData, captchaValue: captchaValue };
                        //}
                        //else {
                        //    param = { invData: strInvData };
                        //}
                        //MISA.mask.show();
                        //CommonFunction.CallApi(CommonFunction.GetFullUrl(uri), param, function (stringResult) {
                        //    MISA.mask.hide();
                        //    if (CommonFunction.CheckJsonFormat(stringResult)) {
                        //        result = JSON.parse(stringResult);
                        //        if (result.success == true && result.data != null && result.data != "") {
                        //            var data = JSON.parse(result.data);
                        //            $('#txtCode').val(data.txtCodeValue)
                        //            $('#InvNoSuccess').html(data.invNoSuccessText);
                        //            $('#InvNo').html(data.invNoText);
                        //            $('#companyCode').html(data.companyCodeInnerText);
                        //            if (result.errorCode != undefined && result.errorCode != "") {
                        //                einvoiceSearch.ShowErrorMessage("Mã số tra cứu không đúng. Vui lòng nhập lại mã số.");
                        //                if (result.errorCode == "InvalidTransactionID" && result.customData != null && result.customData > 2) {
                        //                    isShowCaptcha = true;
                        //                }

                        //            }
                        //            else {
                        //                //Ẩn captcha nếu tra cứu thành công
                        //                $('#captchaContainer').css("display", "none");
                        //                $("#captcha_captchaTextbox").val("");

                        //                var urlSearch = "tra-cuu/DownloadHandler.ashx?Type=pdf&Viewer=1&Code=" + data.transactionID + "&ext=" + result.customData;
                        //                //lấy lịch sử hóa đơn điện tử
                        //                einvoiceSearch.GetInvoiceHistory(data.transactionID);
                        //                einvoiceSearch.ShowSearchResultPopup(urlSearch);
                        //            }
                        //        }
                        //    }
                        //    else {
                        //        einvoiceSearch.ShowErrorMessage(einvoiceSearch.exceptionError);
                        //    }
                        //})
                    }
                    fileReafer.readAsText(file);
                }
                else {
                    einvoiceSearch.ShowErrorMessage("File không đúng chuẩn định dạng của hóa đơn. Vui lòng chọn lại.");
                }
            }
        }
        else {
            einvoiceSearch.ShowErrorMessage("File không được để trống. Vui lòng chọn file xml để tra cứu.");
        }
    }
})(jQuery);