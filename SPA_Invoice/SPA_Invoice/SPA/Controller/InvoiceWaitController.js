app.controller('InvoiceWaitController', ['$scope', '$rootScope', '$timeout', '$sce', 'CommonFactory', '$filter', '$location', function ($scope, $rootScope, $timeout, $sce, CommonFactory, $filter, $location) {
    var url = '/Invoice/';

    angular.element(function () {
        //$rootScope.GetPaymentStatus();
        //$rootScope.GetInvoiceStatus();
        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
    });

    $scope.TabWaiting = {
        TabWaitingNumber: true,
        TabInvoice: false
    };

    $scope.CurrentNumberObject = new Object();
    $rootScope.TabClick = function (xxx, item) {
        if (item) {
            $scope.NumberWaitingId = item.ID;
            $scope.CurrentNumberObject = item;
        }
        var listNumber = [];
        var listUsedNumber = [];
        var xx = [];
        if ($scope.CurrentNumberObject.USEDNUMBER != null) {
            xx = $scope.CurrentNumberObject.USEDNUMBER.split(',');
            for (let i = 0; i < xx.length; i++) {
                listUsedNumber.push(parseInt(xx[i]));
            }
        }
        for (let i = $scope.CurrentNumberObject.FROMNUMBER; i <= $scope.CurrentNumberObject.TONUMBER; i++) {
            listNumber.push(i);
        }
        var filteredArray = listNumber.filter(function (x) {
            return listUsedNumber.indexOf(x) < 0;
        });
        $scope.LISTUSEDNUMBER = listUsedNumber.sort(function (a, b) { return a - b; });
        $scope.LISTNUMBER = filteredArray.sort(function (a, b) { return a - b; });

        if (listUsedNumber.length == listNumber.length) {

        }

        $scope.TabWaiting.TabWaitingNumber = false;
        $scope.TabWaiting.TabInvoice = false;
        $scope.Filter = new Object();
        $scope.ListInvoice = [];
        switch (xxx) {
            case 1:
                $scope.TabWaiting.TabWaitingNumber = true;
                $scope.GetInvoiceWaiting(1);
                break;
            case 2:
                $scope.TabWaiting.TabInvoice = true;
                $scope.Filter = new Object();
                $scope.Filter.INVOICESTATUS = 1;
                $scope.Filter.FORMCODE = item.FORMCODE;
                $scope.Filter.SYMBOLCODE = item.SYMBOLCODE;
                $scope.GetInvoice(1);
                break;
            default:
                $scope.TabWaiting.TabWaitingNumber = true;
                break;
        }
    }

    $rootScope.ReloadWaitingInvoice = function (formcode, symbolcode) {
        if ($location.path().toString().includes('/dai-hoa-don-cho')) {
            $scope.Filter = new Object();
            $scope.ListInvoice = [];
            $scope.Filter.INVOICESTATUS = 1;
            $scope.Filter.FORMCODE = formcode;
            $scope.Filter.SYMBOLCODE = symbolcode;
            $scope.GetInvoice(1);
        }
    }


    $scope.SignWaiting = function (invoiceId, signTime, waitingNumber) {
        var dateReg = /^\d{2}[./-]\d{2}[./-]\d{4}$/

        if (!waitingNumber) {
            alert('Vui lòng nhập vào số muốn ký.');
            return false;
        }
        if (parseInt(waitingNumber) <= 0) {
            alert('Vui lòng nhập số lớn hơn 0.');
            return false;
        }
        if (parseInt(waitingNumber) < parseInt($scope.CurrentNumberObject.FROMNUMBER)) {
            alert('Số hóa đơn không nhỏ hơn số bắt đầu.');
            return false;
        }
        if (parseInt(waitingNumber) > parseInt($scope.CurrentNumberObject.TONUMBER)) {
            alert('Số hóa đơn không lớn hơn số đến.');
            return false;
        }
        if (!signTime) {
            alert('Vui lòng nhập vào ngày muốn ký.');
            return false;
        }
        if (!signTime.match(dateReg)) {
            alert('Vui lòng nhập đúng định dạng ngày dd/mm/yyyy.');
            return false;
        }
        if ($scope.CurrentNumberObject.USEDNUMBER !== null) {
            var find = $scope.CurrentNumberObject.USEDNUMBER.split(',').filter(function (x) {
                return x == waitingNumber;
            })
            if (find.length > 0) {
                alert('Số hóa đơn ' + waitingNumber + ' đã được sử dụng.');
                return false;
            }
        }

        LoadingShow();
        SignWaiting(invoiceId, signTime, $scope.NumberWaitingId, waitingNumber);
    }

    $scope.GetNumber = function (intpage) {
        $scope.FormNumber = new Object();

        $scope.FormNumber.FORMCODE = $scope.Number.FORMCODE;
        $scope.FormNumber.SYMBOLCODE = $scope.Number.SYMBOLCODE;
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    let result = response.data[0];
                    $scope.Number.CURRENTNUMBER = result.CURRENTNUMBER;
                    $scope.Number.FROMNUMBER = result.CURRENTNUMBER;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }

    $scope.SetDatepiker = function (index) {
        let ctr = $('#pk_' + index);
        ctr.datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface(ctr);
    }

    $scope.CheckDateOfPreviousInvoice = function (numberInvoice, item) {
        var ctr = $('#pk_' + item.ID);
        //var dateNow = new Date();
        var dateNow = null;
        var action = url + 'CheckDateOfPreviousInvoice';
        var min_date = "";
        var max_date = "";
        var datasend = JSON.stringify({
            form: item,
            number: parseInt(numberInvoice),
        });
        if (numberInvoice != null && numberInvoice != "") {
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        var minInvoice = response.result.filter((item) => {
                            return item.NUMBER < numberInvoice;
                        });
                        var maxInvoice = response.result.filter((item) => {
                            return item.NUMBER > numberInvoice;
                        });
                        min_date = (minInvoice.length === 0) ? null : new Date(minInvoice[0].SIGNEDTIME.match(/\d+/)[0] * 1);
                        ctr.datepicker("option", 'minDate', min_date);
                        max_date = (maxInvoice.length === 0) ? dateNow : new Date(maxInvoice[0].SIGNEDTIME.match(/\d+/)[0] * 1);
                        ctr.datepicker("option", 'maxDate', max_date);

                        ctr.datepicker({
                            dateFormat: 'dd/mm/yy',
                            mindate: min_date,
                            maxdate: max_date
                        });
                    } else {
                        ctr.datepicker("option", 'maxDate', dateNow);
                        ctr.datepicker({
                            dateFormat: 'dd/mm/yy',
                            maxDate: dateNow
                        });
                        $scope.ErrorMessage = response.msg;
                    }
                    ctr.datepicker("option", 'minDate', min_date);
                    ctr.datepicker("option", 'maxDate', max_date);
                    SetVietNameInterface(ctr);
                    ctr.datepicker('show');
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - CheckDateOfPreviousInvoice');
                }
            });
        }
    }

    $scope.GetInvoice = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }

        $scope.currentpage = intpage;
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: 10
        });
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    alert(response.msg);
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }

    $rootScope.GetInvoiceWaiting = function (intpage) {
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentRow = 10;
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceNumerWaiting';
        var datasend = JSON.stringify({
            itemPerPage: $scope.currentRow,
            currentPage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    response.result.forEach(function (numberItem) {
                        numberItem.NUMBERSTATUS = $scope.ENUM_WAITINGNUMBERSTATUS.CON_SO;
                        let listNumber = [];
                        let listUsedNumber = [];
                        let listAvailableNumber = [];
                        let tempArray = [];
                        if (numberItem.USEDNUMBER != null) {
                            tempArray = numberItem.USEDNUMBER.split(',');
                            for (let i = 0; i < tempArray.length; i++) {
                                listUsedNumber.push(parseInt(tempArray[i]));
                            }
                            numberItem.USEDNUMBER = listUsedNumber.sort(function (a, b) { return a - b; }).join(', ');
                        }
                        for (let i = numberItem.FROMNUMBER; i <= numberItem.TONUMBER; i++) {
                            listNumber.push(i);
                        }

                        if (listUsedNumber.length == listNumber.length) {
                            numberItem.NUMBERSTATUS = $scope.ENUM_WAITINGNUMBERSTATUS.HET_SO;
                        }

                        listAvailableNumber = listNumber.filter(function (e) {
                            return !listUsedNumber.includes(e);
                        })
                        numberItem.AVAILABLENUMBER = listAvailableNumber.join(', ');
                    });

                    $scope.ListInvoiceWaiting = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    alert(response.msg);
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
    }

    //Init time
    var now = new Date();
    var firstDay = new Date();
    var lastDay = new Date();
    var currentDay = now.getDay();

    // Sunday - Saturday : 0 - 6
    //This week
    firstDay.setDate(now.getDate() - currentDay);
    lastDay.setDate(firstDay.getDate() + 6);
    var thisWeek = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last week
    firstDay.setDate(firstDay.getDate() - 7);
    lastDay.setDate(lastDay.getDate() - 7);
    var lastWeek = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //This month
    var dayOfMonth = new Date(now.getFullYear(), now.getMonth(), 0).getDate();
    firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    lastDay = new Date(now.getFullYear(), now.getMonth(), (dayOfMonth - 1));
    var thisMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last month
    lastDay.setDate(firstDay.getDate() - 1);
    firstDay = new Date(lastDay.getFullYear(), lastDay.getMonth(), 1);
    var lastMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();


    $scope.Timepickers = {
        value: thisWeek,    //Default value
        Options: [
            { value: thisWeek, text: 'Tuần này' },
            { value: lastWeek, text: 'Tuần trước' },
            { value: thisMonth, text: 'Tháng này' },
            { value: lastMonth, text: 'Tháng trước' }
        ]
    };
    //End init time

    $scope.ENUM_WAITINGNUMBERSTATUS = {
        CON_SO: 'Còn số',
        HET_SO: 'Hết số'
    };

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });    

    $(document).on("keyup", ".number-waitting", function (e) {
        if (e.keyCode == 69) {
            $(".number-waitting").val("");
        }
    })

}]);