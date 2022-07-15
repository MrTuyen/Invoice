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
    debugger;
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