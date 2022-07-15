"use strict";

var dyn_functions = [];
var callbackExec = function (str) { };

function CreatePagerSubmit(currPage, totalPage, $ctr, frmSearch) {
    var pager = GenHtmlPager(currPage, totalPage);
    $ctr.html(pager);
    //$ctr.html(pager);
    $ctr.find('span').click(function () {
        if ($(this).text() == "...") {
            return;
        }
        if (frmSearch == null)
            frmSearch = "frmSearch";
        var pIndex = parseInt($(this).text());

        $("#" + frmSearch + " #hdPageIndex").val(pIndex);
        $("#" + frmSearch).submit();

        $ctr.css('opactity', '0.5');
        $ctr.css('cursor', 'progress');
    });
}

function CreatePagerAjax(currPage, totalPage, $ctr, functionName) {
    var pager = GenHtmlPager(currPage, totalPage);
    $ctr.html(pager);
    $ctr.find('span').click(function () {
        if ($(this).text() == "...") {
            return;
        }
        //CreatePagerAjax(parseInt($(this).text()), totalPage, $ctr, functionName);
        var pIndex = parseInt($(this).text());
        dyn_functions[functionName](pIndex);

        $ctr.css('opactity', '0.5');
        $ctr.css('cursor', 'progress');
    });
}

function CreateDivPagerAjax(currPage, totalPage, $ctr, model, action, view, functionName, inputName) {
    var pager = GenHtmlPager(currPage, totalPage);
    $ctr.html(pager);
    $ctr.find('span').click(function () {
        if ($(this).text() == "...") {
            return;
        }
        var pIndex = parseInt($(this).text());
        dyn_functions[functionName](model, action, view, pIndex, inputName);

        $ctr.css('opactity', '0.5');
        $ctr.css('cursor', 'progress');
    });
}

function GenHtmlPager(currPage, totalPage) {
    var pager = '';
    var numButton = 8;
    if (totalPage < 1 || currPage < 1) {
        return;
    }
    if (totalPage < numButton) {
        for (var i = 1; i <= totalPage; i++) {
            if (i != currPage) {
                pager += '<span class="btn pagerbtn">';
                pager += i;
                pager += '</span>'
            }
            else {
                pager += '<span class="btn active pagerbtn">';
                pager += i;
                pager += '</span>'
            }
        }
    }
    else {
        var center = Math.floor((numButton - 2) / 2);
        var left = currPage - center;
        if (left < 1)
            left = 1;
        var right = currPage + center;
        if (right > totalPage)
            right = totalPage;
        if (right - left <= 2 * center) {
            right = left + 2 * center;
        }
        if (currPage - center > 1) {
            pager += '<span class="btn pagerbtn">';
            pager += 1;
            pager += '</span>';
            if (currPage - center > 2) {
                pager += '<span class="btn pagerbtn">...</span>';
            }
        }
        else {
            right++;
        }

        if (right >= totalPage) {
            right = totalPage;
            left = right - (2 * center);
        }
        var temp = '';
        if (currPage + center < totalPage) {
            if (currPage + center < totalPage - 1) {
                temp += '<span class="btn pagerbtn">...</span>';
            }
            temp += '<span class="btn pagerbtn">';
            temp += totalPage;
            temp += '</span>';
        }
        else {
            left--;
        }
        for (var i = left; i <= right; i++) {
            if (i != currPage) {
                pager += '<span class="btn pagerbtn" page="' + i + '">';
                pager += i;
                pager += '</span>'
            }
            else {
                pager += '<span class="btn active pagerbtn" page="' + i + '">';
                pager += i;
                pager += '</span>'
            }
        }
        pager += temp;
    }
    return pager;
}

function printDiv(divName) {
    var printContents = document.getElementById(divName).innerHTML;
    var originalContents = document.body.innerHTML;

    document.body.innerHTML = printContents;

    window.print();

    document.body.innerHTML = originalContents;
}

function CheckAll(chkAll, chkSelect) {
    if ($(chkAll).attr('checked')) {
        $('input:checkbox[id=' + chkSelect + ']').attr('checked', true);
    }
    else {
        $('input:checkbox[id=' + chkSelect + ']').attr('checked', false);
    }
}

function ShowLoading(value) {
    document.getElementById('loading-page').style.display = value ? 'block' : 'none';
}


//Cookie's owned
function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = document.cookie;
    try {
        decodedCookie = decodeURIComponent(document.cookie);
    } catch (error) {
        console.log(error);
    }

    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
}
function checkCookie(cookie_name) {

}

var validation = {
    isEmailAddress: function (str) {
        var pattern = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return pattern.test(str);  // returns a boolean
    },
    isNotEmpty: function (str) {
        var pattern = /\S+/;
        return pattern.test(str);  // returns a boolean
    },
    isNumber: function (str) {
        var pattern = /^\d+$/;
        return pattern.test(str);  // returns a boolean
    },
    isSame: function (str1, str2) {
        return str1 === str2;
    }
};
