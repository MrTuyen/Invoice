function DateFormat(dateString) {
    var currentTime = new Date(parseInt(dateString.substr(6)));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    var date = day + "/" + month + "/" + year;
    return date;
}
function validateDate(dateStr) {
    const regExp = /^(\d\d?)\/(\d\d?)\/(\d{4})$/;
    let matches = dateStr.match(regExp);
    let isValid = matches;
    let maxDate = [0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    if (matches) {
        const date = parseInt(matches[1]);
        const month = parseInt(matches[2]);
        const year = parseInt(matches[3]);

        isValid = month <= 12 && month > 0;
        isValid &= date <= maxDate[month] && date > 0;

        const leapYear = (year % 400 == 0)
            || (year % 4 == 0 && year % 100 != 0);
        isValid &= month != 2 || leapYear || date <= 28;
    }

    return isValid
}
//Load Data function  
function loadData() {
    // https://tracuuhddt.onfinance.asia/?comtaxcode=0336692463&mkh=true&sdt=true : query string example
    var cusPhoneNumber = $('[name="cusphonenumber"]').val();
    var customerCode = $('[name="customercode"]').val();
    var fromdate = $('[name="fromdate"]').val();
    var todate = $('[name="todate"]').val();
    var queryStr = getUrlVars(window.location.href);

    if (fromdate == '' || todate == '')
    {
        alert("Vui lòng chọn từ ngày đến ngày");
        return;
    }
    else if (!(validateDate(fromdate)) || !(validateDate(todate))) {
        alert("Ngày bạn nhập không đúng định dạng");
        return;
    }
    else if (queryStr[1] === "true" && customerCode == '') {
        alert("Vui lòng nhập mã khách hàng");
        return;
    }
    else if (queryStr[2] === "true" && cusPhoneNumber == '') {
        alert("Vui lòng nhập số điện thoại");
        return;
    }
    else
    {
        $('#my-img').css('display', 'block');
        $('#content-table').css('display', 'none');
        $.ajax({
            url: "/Search/GetSearchCustomerID",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify({ cusPhoneNumber: cusPhoneNumber, customerCode: customerCode, currentPage: 12, itemPerPage: 1, fromdate: fromdate, todate: todate, comtaxcode: queryStr[0] }),
            dataType: "json",
            success: function (result) {
                var html = '';
                if (result.result.length ==0) {
                    html += '<tr><td colspan="6"  style="text-align:center;">Dữ liệu trống!</td></tr>';
                } else {
                    for (var i = 0; i < result.result.length; i++) {
                        let c = result.result[i].REFERENCECODE.toString();
                        html += "<tr>";
                        if (result.result[i].CUSTOMERCODE != null) {
                            html += "<td>" + result.result[i].CUSTOMERCODE + "</td>";
                        } else {
                            html += "<td>" + result.result[i].CUSTAXCODE + "</td>";
                        }
                        html += "<td style='text-align: left;'>" + result.result[i].CUSNAME + "</td>";
                        html += "<td>" + DateFormat(result.result[i].SIGNEDTIME) + "</td>";
                        html += "<td>" + result.result[i].REFERENCECODE + "</td>";
                        html += "<td>" + formatNumber(result.result[i].TOTALPAYMENT, '.', ',') + "</td>";
                        html += "<td style='text-align:center;'><span class='fas fa-eye' style='color:#34d3e3;cursor: pointer;' title='Xem file hóa đơn' onclick='showPopupFileView(\"" + c + "\")'></span></td>";
                        html += "</tr>";
                    }
                }
                $('#my-img').css('display', 'none');
                $('#content-table').css('display', 'block');
                $('.tbody').html(html);
            },
            error: function (errormessage) {
            }
        });
    }
}
var showPopupFileView = function (keyword) {
        var data = JSON.stringify({
            id: keyword
        });
        $.ajax({
            url: "/tracuu/search_by_code",
            type: "POST",
            cache: true,
            crossDomain: true,
            contentType: "application/json; charset=utf-8;",
            dataType: "json",
            data: data,
            processData: true,
            async: true,
            success: function (response) {
                if (response && response.rs && response.msg) {
                    var obj = response.msg;

                    if (obj.SIGNLINK) {
                        showPopupFile(obj);
                    } else {
                        alert("Không tìm thấy hóa đơn bạn cần xem. <br>Vui lòng hỏi lại bên bán để xác nhận hóa đơn đã được ký hay chưa.");
                    }
                } else {
                    alert("Hóa đơn không tồn tại. Vui lòng xem lại!. Hóa đơn được tạo ra tự động bởi hệ thống OnFinance.asia của NOVAON");
                }
            },

            error: function (error) {
            }
        });
}
var showPopupFile = function (obj) {
    console.log(obj);
    if (obj.SIGNLINK) {
        //Lưu mã hóa đơn tìm được
        $('#invoice_id').val(obj.ID);
        $('#popupViewInvoice').dialog({
            width: 730, height:'auto',
            modal: true,
            resizable: false,
            
            open: function (data, e, f) {
                $('#frViewFileInvoice').attr('src', 'https://e.onfinance.asia/NOVAON_FOLDER' + obj.SIGNLINK);
                $(this).dialog("option", "title", "Hóa đơn của: " + obj.CUSNAME);
            }
        });
    } else {
        alert("Không tìm thấy hóa đơn bạn cần xem. <br>Vui lòng hỏi lại bên bán để xác nhận hóa đơn đã được ký hay chưa.");
    }
}
function formatNumber(nStr, decSeperate, groupSeperate) {
    nStr += '';
    x = nStr.split(decSeperate);
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + groupSeperate + '$2');
    }
    return x1 + x2;
}
var getUrlVars = function (url) {
    var vars = [], hash;
    var hashes = url.slice(url.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[1]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}