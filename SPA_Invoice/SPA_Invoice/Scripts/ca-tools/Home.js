/*
 * truongnv 20200226
 * load thông tin kiểm tra xem người dùng có sử dụng chứ ký số HSM hay không
 */
$(document).ready(function () {
    /*var timeDelay = 5000;*/           // MILLISECONDS (5 SECONDS).
    var timeDelay = 0;
    setTimeout(sendRequest, timeDelay);  // MAKE THE AJAX CALL AFTER A FEW SECONDS DELAY.

    $('#txtMoney').change(function () {
        UpdateFormat(this);
    });
    $('#txtQ').change(function () {
        UpdateFormat(this);
    });
    function sendRequest() {
        $.ajax({
            type: "POST",
            url: "/AjaxMethod/AjaxMethod",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.rs)
                    localStorage.setItem('novaon_kyso_hsm', response.msg);
                else
                    localStorage.setItem('novaon_kyso_hsm', response.msg);
            },
            failure: function (response) {
                localStorage.setItem('novaon_kyso_hsm', response.msg);
            },
            error: function (response) {
                localStorage.setItem('novaon_kyso_hsm', response.msg);
            }
        });
    }
});

/////////////////////////////////////////////////////////////////////////////////////////
// KÝ DẢI SỐ CHỜ
/////////////////////////////////////////////////////////////////////////////////////////

var invoiceIdObj = 0;
var signTimeObj = "";
var waitingObj = 0;
var waitingNumberObj = 0;

function SignWaiting(idInvoice, signTime, numberWaitingId, waitingNumber) {
    //LoadingShow();
    try {

        var dateReg = /^\d{2}[./-]\d{2}[./-]\d{4}$/
        if (!signTime.match(dateReg)) {
            alert('Vui lòng nhập đúng định dạng ngày dd/mm/yyyy.');
            return false;
        }

        invoiceIdObj = idInvoice;
        signTimeObj = signTime;
        waitingObj = numberWaitingId;
        waitingNumberObj = waitingNumber;

        // Kiểm tra xem khách hàng có sử dụng chữ ký số hsm hay không
        if (localStorage.getItem("novaon_kyso_hsm") === "OK") {
            signWaitingHSMApi(invoiceIdObj, signTimeObj, numberWaitingId, waitingNumber);
        }
        else {
            // dữ liệu được trả lại hàm chooseFileCallbak, plugin tự động gọi lại hàm này
            Plugin.checkPlugin(checkPluginCallbakWaiting);
        }
    }
    catch (e) {
        LoadingHide();
    }
}

/**
 * truongnv 20200226
 * Ký file xml bằng công cụ HSM
 * @param {any} invoiceId:
 * @param {any} signTime:
 * @param {any} numberWaitingId: 
 * @param {any} waitingNumber: 
 */
function signWaitingHSMApi(invoiceId, signTime, numberWaitingId, waitingNumber) {
    var data = { invoiceId: invoiceId, numberId: numberWaitingId, tempNumberWaiting: waitingNumber, signTime: signTime }
    $.ajax({
        url: "/Home/SignWaitingHSMApi",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        beforeSend: function () {
            LoadingShow();
        },
        success: function (data) {
            if (!data.rs)
                alert(data.msg);
            else {
                alert(data.msg + " Hóa đơn số: " + data.info.NUMBER + ", mẫu số: " + data.info.FORMCODE + ", ký hiệu: " + data.info.SYMBOLCODE);
            }
            LoadingHide();
        },
        error: function (errormessage) {
            alert("Hệ thống xảy ra lỗi trong quá trình ký hóa đơn.");
            LoadingHide();
        }
    });
}

////////////////////////////////////////////
// Check plugin
///////////////////////////////////////////
function checkPluginCallbakWaiting(result) {
    LoadingShow();
    // result là kết quả check plugin
    //  1: là thành công
    // -1: là không thành công
    //alert(result);
    var data = { invoiceId: invoiceIdObj, signTime: signTimeObj, numberWaitingId: waitingObj, waitingNumber: waitingNumberObj }
    if (result == "1") {
        $.ajax({
            url: "/Home/GetFileWaiting",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            data: data,
            success: function (response) {
                if (response.rs)
                {
                    signDataWaiting(response.data);
                }
                else {
                    alert(response.msg);
                }
                LoadingHide();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
                LoadingHide();
            }
        });
    }
    else {
        alert('Để ký điện tử bạn cần cài đặt phần mềm ký điện tử của ONFINANCE <a href="http://support.onfinance.asia/uploads/onfinance_invoice_ca.exe">tại đây</a> hoặc truy cập <a href="http://support.onfinance.asia/tools" target="_blank">http://support.onfinance.asia/tools</a>');
        LoadingHide();
    }
}

function signDataWaiting(data) {
    try {
        LoadingShow();
        var dataArray = [];
        // xml
        var dataXml = {};
        dataXml.type = 'xml';
        dataXml.input = data.Base64Xml;;
        dataXml.index = 0;
        dataXml.uri = '#data';
        dataArray.push(dataXml);
        Plugin.signXML(dataArray, "" ,signCallbackWaiting);
    }
    catch (e) {
        LoadingHide();
    }
}

function signCallbackWaiting(result) {
    var dataSignedArr = JSON.parse(result);
    // Lấy ra chuỗi json 
    var dataXmlSigned = JSON.parse(dataSignedArr[0]);
    if (dataXmlSigned.code == 0) {
        LoadingShow();
        var obj = {
            invoiceId: invoiceIdObj,
            signTime: signTimeObj,
            numberWaitingId: waitingObj,
            base64Xml: dataXmlSigned.data,
            subject: dataXmlSigned.subject
        };
        $.ajax({
            url: "/Home/SaveFileWaiting",
            data: JSON.stringify(obj),
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                if (result.rs === true) {
                    window.ReloadInvoice();
                    alert(result.msg);
                    //alert(result.msg + " Hóa đơn số: " + result.info.NUMBER + ", mẫu số: " + result.info.FORMCODE + ", ký hiệu: " + result.info.SYMBOLCODE);
                }
                else {
                    alert(result.msg);
                }
                LoadingHide();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
                LoadingHide();
            }
        });
    } else {
        if (dataXmlSigned.error === "Not found certificate" || dataXmlSigned.error === "Private key not exists") {
            dataXmlSigned.error = "Bạn chưa cắm USB Token. Thử rút ra cắm lại hoặc reset lại máy tính.";
        }
        alert("Ký không thành công. " + dataXmlSigned.error);
        LoadingHide();
    }
}

/////////////////////////////////////////////////////////////////////////////////////////
/////////     KÝ DẢI SỐ THƯỜNG
/////////////////////////////////////////////////////////////////////////////////////////
var test = 0;
function Sign(idInvoice) {
    //LoadingShow();
    try {
        test = idInvoice;
        // Kiểm tra xem khách hàng có sử dụng chữ ký số hsm hay không
        if (localStorage.getItem("novaon_kyso_hsm") === "OK") {
            signXmlHSMApi(idInvoice);
        }
        else {
            // dữ liệu được trả lại hàm chooseFileCallbak, plugin tự động gọi lại hàm này
            signXmlUSBMultiple(idInvoice);
        }
        return new Promise(function (resovle, reject) {
            resovle();
        });
    }
    catch (e) {
        LoadingHide();
    }
}

function SignMultiple(idInvoices, isUSBToken) {
    try {
        if (isUSBToken) {
            signXmlUSBMultiple(idInvoices);
        }
        else {
            signXmlHSMApiMultiple(idInvoices);
        }
    }
    catch (e) {
        LoadingHide();
    }
}

/**
 * truongnv 20200226
 * Ký file xml bằng công cụ HSM
 * @param {any} invoiceId: Số hóa đơn
 */
function signXmlHSMApi(invoiceId) {
    var data = { idInvoice: invoiceId }
    $.ajax({
        url: "/Home/SignXmlHSMApi",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        beforeSend: function () {
            LoadingShow();
        },
        success: function (data) {
            if (!data.rs) {
                alert(data.msg);
                location.reload(true);
            }
            else {
                alert(data.msg + " Hóa đơn số: " + data.info.NUMBER + ", mẫu số: " + data.info.FORMCODE + ", ký hiệu: " + data.info.SYMBOLCODE);
                window.ReloadInvoice();
            }
            LoadingHide();
        },
        error: function (errormessage) {
            alert("Hệ thống xảy ra lỗi trong quá trình ký hóa đơn. Vui lòng thử lại.");
            LoadingHide();
        }
    });
}


/**
 * truongnv 20200226
 * Ký file xml bằng công cụ HSM
 * @param {any} invoiceId: Số hóa đơn
 */
function signXmlHSMApiMultiple(idInvoices) {
    var data = { idInvoices: idInvoices }
    $.ajax({
        url: "/Home/signXmlHSMApiMultiple",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        data: JSON.stringify(data),
        beforeSend: function () {
            LoadingShow();
        },
        success: function (data) {
            if (!data.rs)
                toastr.warning(data.msg);
            else {
                if (!data.isNoti);
                else
                    toastr.success(data.msg);
                //location.reload(true);
            }
            LoadingHide();
        },
        error: function (errormessage) {
            alert("Hệ thống xảy ra lỗi trong quá trình ký hóa đơn.");
            LoadingHide();
        }
    });
}

function sendInvoiceEmail() {
    $.ajax({
        url: "/Home/SendMutiEmail",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        data: {},
        success: function (data) {
            console.log("send is mail success.");
        },
        error: function (errormessage) {
            console.log("error." + errormessage);
        }
    });
}

////////////////////////////////////////////
// Check plugin
///////////////////////////////////////////
function checkPluginCallbak(result) {
    LoadingShow();
    // result là kết quả check plugin
    //  1: là thành công
    // -1: là không thành công
    //alert(result);
    var data = { idInvoice: test }
    if (result == "1") {
        $.ajax({
            url: "/Home/GetFile",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            data: data,
            success: function (data) {
                if (data != "Vui lòng xem lại thông báo phát hành dải hóa đơn!"
                    && data != "Chưa lấy được file hóa đơn để ký. Vui lòng xem lại!"
                    && data != "Dải hóa đơn này đã hết số."
                    && data != "Lấy mẫu hóa đơn không thành công."
                ) {
                    signData(data);
                }
                else {
                    alert(data);
                }
                LoadingHide();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
                LoadingHide();
            }
        });
    }
    else {
        alert('Để ký điện tử bạn cần cài đặt phần mềm ký điện tử của ONFINANCE <a href="http://support.onfinance.asia/uploads/onfinance_invoice_ca.exe">tại đây</a> hoặc truy cập <a href="http://support.onfinance.asia/tools" target="_blank" >http://support.onfinance.asia/tools</a>');
        LoadingHide();
    }
}

function signData(data) {
    try {
        LoadingShow();
        var dataArray = [];

        // pdf
        var dataPdf = {};
        dataPdf.type = 'pdf';  // loại file
        dataPdf.input = data.Base64Pdf;
        dataPdf.index = 0;
        dataPdf.visibleMode = 3;
        dataPdf.rendermode = 0;
        dataPdf.pageNo = 1;
        dataPdf.img = '';
        //Tọa độ góc dưới, bên trái
        //Tọa độ góc dưới, bên trái
        dataPdf.llX = 381.91;
        dataPdf.llY = 61.537;
        // Tọa độ góc trên bên phải
        dataPdf.urX = 544.729;
        dataPdf.urY = 136.581;
        // Kích thước ảnh
        dataPdf.imageWidth = 1000;
        dataPdf.imageHeight = 292;
        dataArray.push(dataPdf);

        // xml
        var dataXml = {};
        dataXml.type = 'xml';
        dataXml.input = data.Base64Xml;;
        dataXml.index = 0;
        dataXml.uri = '#data';
        dataArray.push(dataXml);
        Plugin.signPdfAndXml(dataArray, signCallback);
    }
    catch (e) {
        LoadingHide();
    }
}

function signCallback(result) {
    var dataSignedArr = JSON.parse(result);
    // Lấy ra chuỗi json 
    var dataPdfSigned = JSON.parse(dataSignedArr[0]);
    var dataXmlSigned = JSON.parse(dataSignedArr[1]);
    if (dataPdfSigned.code == 0 && dataXmlSigned.code == 0) {
        LoadingShow();
        var obj = {
            idInvoice: test,
            base64Pdf: dataPdfSigned.data,
            base64Xml: dataXmlSigned.data,
            subject: dataPdfSigned.subject
        };
        $.ajax({
            url: "/Home/SaveFile",
            data: JSON.stringify(obj),
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                if (result.rs === true) {
                    window.ReloadInvoice();
                    alert(result.msg + " Hóa đơn số: " + result.info.NUMBER + ", mẫu số: " + result.info.FORMCODE + ", ký hiệu: " + result.info.SYMBOLCODE);
                }
                else {
                    alert(result.msg);
                }
                LoadingHide();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
                LoadingHide();
            }
        });
    } else {
        if (dataPdfSigned.error === "Not found certificate" || dataPdfSigned.error === "Private key not exists") {
            dataPdfSigned.error = "Bạn chưa cắm USB Token. Thử rút ra cắm lại hoặc reset lại máy tính.";
        }
        alert("Ký không thành công. " + dataPdfSigned.error);
        LoadingHide();
    }
}

////////////////////// Ký USB hàng loạt //////////////////////
/////////////////////////////////////////////////////////////
var listInvoicesId = "";
var listNextNumbers = "";
function signXmlUSBMultiple(idInvoices) {
    listInvoicesId = idInvoices;
    Plugin.checkPlugin(checkPluginCallbakUSB);
}
/**
 * tuyennv 202000518
 * Ký file xml bằng USB Token
 * @param {any} invoiceId: Số hóa đơn
 */
function checkPluginCallbakUSB(result) {
    LoadingShow();
    // result là kết quả check plugin
    //  1: là thành công
    // -1: là không thành công
    //alert(result);
    if (result == "1") {
        var data = { idInvoices: listInvoicesId }
        $.ajax({
            url: "/Home/SignXmlUSBApiMultiple",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            beforeSend: function () {
                LoadingShow();
            },
            success: function (response) {
                if (response.rs) {
                    listInvoicesId = response.data2;
                    listNextNumbers = response.data3;
                    signUSBData(response.data);
                }
                else {
                    alert(response.msg);
                    LoadingHide();
                }
            },
            error: function (errormessage) {
                alert("Hệ thống xảy ra lỗi trong quá trình ký hóa đơn.");
                LoadingHide();
            }
        });
    }
    else {
        alert('Để ký điện tử bạn cần cài đặt phần mềm ký điện tử của ONFINANCE <a href="http://support.onfinance.asia/uploads/onfinance_invoice_ca.exe">tại đây</a> hoặc truy cập <a href="http://support.onfinance.asia/tools" target="_blank" >http://support.onfinance.asia/tools</a>');
        LoadingHide();
    }
}

function signUSBData(data) {
    try {
        var dataArray = [];
        // xml
        for (item of data) {
            var dataXml = {};
            dataXml.type = 'xml';
            dataXml.input = item.Base64Xml;;
            dataXml.index = 0;
            dataXml.uri = '#data';
            dataArray.push(dataXml);
        }
        Plugin.signXML(dataArray, "", signUSBCallback);
    }
    catch (e) {
        LoadingHide();
    }
}

function signUSBCallback(result) {
    var listBase64Xml = [];
    var dataSignedArr = JSON.parse(result);
    for (item of dataSignedArr) {
        listBase64Xml.push(JSON.parse(item).data);
    }
    // Lấy ra chuỗi json 
    var dataXmlSigned = JSON.parse(dataSignedArr[0]);
    var obj = {
        listInvoiceId: listInvoicesId,
        base64Xml: listBase64Xml,
        cerFileInfo: dataXmlSigned.subject,
        listNextNumber: listNextNumbers
    };
    if (dataXmlSigned.code == 0) {
        $.ajax({
            url: "/Home/SaveFileUSB",
            data: JSON.stringify(obj),
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                if (result.rs === true) {
                    alert(result.msg);
                    window.ReloadInvoice();
                }
                else {
                    alert(result.msg);
                }
                LoadingHide();
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
                LoadingHide();
            }
        });
    } else {
        if (dataXmlSigned.error === "Not found certificate" || dataXmlSigned.error === "Private key not exists") {
            dataXmlSigned.error = "Bạn chưa cắm USB Token. Thử rút ra cắm lại hoặc reset lại máy tính.";
        }
        // Cập nhật trạng thái chưa ký cho dải hóa đơn ISSINGING = false
        //$.ajax({
        //    url: "/Home/UpdateSigningStatus",
        //    data: JSON.stringify(obj),
        //    type: "POST",
        //    contentType: "application/json;charset=UTF-8",
        //    dataType: "json",
        //    success: function () {
        //        console.log("OK");
        //    },
        //    error: function (errormessage) {
        //        console.log(errormessage);
        //    }
        //});
        alert("Ký không thành công. " + dataXmlSigned.error);
        LoadingHide();
    }
}

function LoadingShow() {
    $('.full-overlay').css({ 'z-index': 1000000, 'opacity': .5 });
    $('#mainLoadingSVG').show();
}

function LoadingHide() {
    $('.full-overlay').css({ 'z-index': -1, 'opacity': 0 });
    $('#mainLoadingSVG').hide();
}