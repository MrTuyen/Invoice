function Sign() {
    try {
        // dữ liệu được trả lại hàm chooseFileCallbak, plugin tự động gọi lại hàm này
        Plugin.checkPlugin(checkPluginCallbak);
    }
    catch (e) {
        console.log(e);
    }
}

////////////////////////////////////////////
// Check plugin
///////////////////////////////////////////
function checkPluginCallbak(result) {
    // result là kết quả check plugin
    //  1: là thành công
    // -1: là không thành công
    //alert(result);
    if (result == "1") {
        $.ajax({
            url: "/Home/GetFile",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                signData(data);
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
    else {
        alert('Plugin chưa được chạy');
    }
}

function signData(data) {
    try {
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
        // Tọa độ góc dưới, bên trái
        dataPdf.llX = 357.718;
        dataPdf.llY = 279.796;
        // Tọa độ góc trên bên phải
        dataPdf.urX = 543.431;
        dataPdf.urY = 364.355;
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
        console.log(e);
    }
}

function signCallback(result) {
    var dataSignedArr = JSON.parse(result);
    // Lấy ra chuỗi json 
    var dataPdfSigned = JSON.parse(dataSignedArr[0]);
    var dataXmlSigned = JSON.parse(dataSignedArr[1]);
    if (dataPdfSigned.code == 0 && dataXmlSigned.code == 0) {
        var obj = {
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
                window.open("/Outputs/signed.pdf");
                window.open("/Outputs/signed.xml");
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    } else {
        alert("Lỗi ký số: " + dataPdfSigned.error);
    }
}

