app.controller('StatisticController', ['$scope', '$timeout', 'CommonFactory', '$rootScope', function ($scope, $timeout, CommonFactory, $rootScope) {
    var url = '/Statistic/';

    angular.element(function () {
        LoadingHide();
    });

    var now = new Date();
    $scope.CurrentDay = now.getDate();
    $scope.CurrentMonth = now.getMonth() + 1;

    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getFullYear();

    $scope.GetQuarter = function (month) {
        if (month >= 1 && month <= 3) {
            $scope.ReportInvoice.FROMDATE = '01/01/' + $scope.ReportInvoice.YEAR;
            $scope.ReportInvoice.TODATE = '31/03/' + $scope.ReportInvoice.YEAR;
            return 1;
        }
        if (month >= 4 && month <= 6) {
            $scope.ReportInvoice.FROMDATE = '01/04/' + $scope.ReportInvoice.YEAR;
            $scope.ReportInvoice.TODATE = '30/06/' + $scope.ReportInvoice.YEAR;
            return 2;
        }
        if (month >= 7 && month <= 9) {
            $scope.ReportInvoice.FROMDATE = '01/07/' + $scope.ReportInvoice.YEAR;
            $scope.ReportInvoice.TODATE = '30/09/' + $scope.ReportInvoice.YEAR;
            return 3;
        }
        if (month >= 10 && month <= 12) {
            $scope.ReportInvoice.FROMDATE = '01/10/' + $scope.ReportInvoice.YEAR;
            $scope.ReportInvoice.TODATE = '31/12/' + $scope.ReportInvoice.YEAR;
            return 4;
        }
    }

    $scope.InitReportInvoice = function () {
        $scope.ReportInvoice = new Object();
        $scope.ReportInvoice.YEAR = $scope.CurrentYear;
        $scope.ReportInvoice.MONTHQUARTER = "Q" + $scope.GetQuarter($scope.CurrentMonth);
        //$scope.ReportInvoice.MONTHQUARTER = $scope.GetQuarter($scope.CurrentMonth);

        //var date = new Date();
        //var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
        //var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
        //if (ReportInvoice.MONTHQUARTER.includes('M')) {
        //    $scope.GetQuarter(ReportInvoice.MONTHQUARTER.substring(1, ReportInvoice.MONTHQUARTER.length - 1));
        //}
        //$scope.ReportInvoice.FROMDATE = ("0" + firstDay.getDate()).slice(-2) + "/" + ("0" + firstDay.getMonth()).slice(-2) + "/" + firstDay.getFullYear();
        //$scope.ReportInvoice.TODATE = ("0" + lastDay.getDate()).slice(-2) + "/" + ("0" + lastDay.getMonth()).slice(-2) + "/" + lastDay.getFullYear();
        //$scope.ReportInvoice.FIRSTDAY = ("0" + firstDay.getDate()).slice(-2) + "/" + ("0" + firstDay.getMonth()).slice(-2) + "/" + firstDay.getFullYear();
        //$scope.ReportInvoice.LASTDAY = ("0" + lastDay.getDate()).slice(-2) + "/" + ("0" + lastDay.getMonth()).slice(-2) + "/" + lastDay.getFullYear();
    }
    $scope.InitReportInvoice();

    $scope.GetOutputInvoice = function () {

        var action = url + 'GetOutputInvoice';
        var datasend = JSON.stringify({
            reportUsingInvoice: $scope.ReportInvoice
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    let data = response.result[0];
                    $scope.OutputInvoiceData = data;
                    $scope.TOTALREVENUEPRETAX = parseFloat(parseFloat(data.SUMREVENUENOTAX) + parseFloat(data.SUMREVENUEZEROPERCENTTAX) + parseFloat(data.SUMREVENUEFIVEPERCENTTAX) + parseFloat(data.SUMREVENUETENPERCENTTAX))
                    $scope.TOTALREVENUEUNDERTAX = parseFloat(parseFloat(data.SUMREVENUETENPERCENTTAX) + parseFloat(data.SUMREVENUEFIVEPERCENTTAX) + parseFloat(data.SUMREVENUEZEROPERCENTTAX))
                    $scope.TOTALTAXMONEY = parseFloat(parseFloat((0.1) * data.SUMREVENUETENPERCENTTAX) + parseFloat((0.05) * data.SUMREVENUEFIVEPERCENTTAX))
                    $scope.ReportTime = response.reportTime;
                    if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG) {
                        $scope.TOTALREVENUEUNDERTAX = parseFloat(parseFloat(data.SUMREVENUEFIVEPERCENTTAX) + parseFloat(data.SUMREVENUETHREEPERCENTTAX) + parseFloat(data.SUMREVENUETWOPERCENTTAX) + parseFloat(data.SUMREVENUEONEPERCENTTAX) + parseFloat(data.SUMREVENUEZEROPERCENTTAX));
                        $scope.TOTALTAXMONEY = parseFloat(parseFloat((0.01) * data.SUMREVENUEONEPERCENTTAX) + parseFloat((0.02) * data.SUMREVENUETWOPERCENTTAX) + parseFloat((0.03) * data.SUMREVENUETHREEPERCENTTAX)+ parseFloat((0.05) * data.SUMREVENUEFIVEPERCENTTAX))
                    }
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
    }


    $scope.GetUsingInvoiceStatus = function () {
        //$scope.ReportInvoice.YEAR = $scope.CurrentYear;
        //$scope.ReportInvoice.MONTHQUARTER = $scope.GetQuarter($scope.CurrentMonth);

        var action = url + 'GetUsingInvoice';
        var datasend = JSON.stringify({
            reportUsingInvoice: $scope.ReportInvoice
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.UsingInvoiceList = response.listUsingInvoiceThisTerm;
                    $scope.ReportTime = response.reportTime;
                } else {
                    $scope.ErrorMessage = response.msg;
                    $scope.UsingInvoiceList = response.listUsingInvoiceThisTerm;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
    }
    //
    $scope.GetEmailHistory = function (intpage) {
        $scope.FROMTIME = $('#pk_fromtime').val();
        $scope.TOTIME = $('#pk_totime').val();
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentRow = 10;
        $scope.currentpage = intpage;
        $scope.ListEmailHistory = new Array();
        var action = url + 'GetEmailHistoryByComtaxcode';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            itemPerPage: $scope.currentRow,
            currentPage: $scope.currentpage
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $timeout(function () {
                        $scope.ListEmailHistory = response.result;
                        $scope.TotalPages = response.TotalPages;
                        $scope.TotalRow = response.TotalRow;
                    }, 200)
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'MAILTO') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.MAILTO == $scope.Filter[prop];
                    });
                    o.value = f[0].MAILTO;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    o.value = f[0].TIME;
                }

                if (prop == 'STATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.STATUS == $scope.Filter[prop];
                    });
                    o.value = f[0].STATUS;
                }

                if (prop == 'MAILTYPE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.MAILTYPE == $scope.Filter[prop];
                    });
                    o.value = f[0].MAILTYPE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.RemoveFilter = function (f) {
        for (var prop in $scope.Filter) {
            if (prop == f.key) {
                $scope.Filter[prop] = null;
                break;
            }
        }

        //Refresh filter
        $scope.FilterInvoice();
    };
  
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

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
    var dayOfMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    lastDay = new Date(now.getFullYear(), now.getMonth(), dayOfMonth);
    var thisMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last month
    lastDay.setDate(firstDay.getDate() - 1);
    firstDay = new Date(lastDay.getFullYear(), lastDay.getMonth(), 1);
    var lastMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //
    $scope.Timepickers = {
        value: thisWeek,    //Default value
        Options: [
            { id: 1,value: thisWeek, text: 'Tuần này' },
            { id: 2,value: lastWeek, text: 'Tuần trước' },
            { id: 3,value: thisMonth, text: 'Tháng này' },
            { id: 4,value: lastMonth, text: 'Tháng trước' },
            { id: 5,value: '5', text: 'Tùy chọn' }
        ]
    }; 
    $scope.SetDatepikerFromTime = function () {
        $("#pk_fromtime").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime"));
        
        $("#pk_fromtime").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
    }

    $scope.SetDatepikerToTime = function () {
        var toTime = $scope.TOTIME;
        $("#pk_totime").datepicker("option", 'minDate', toTime);
        $("#pk_totime").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTime)
        });
        SetVietNameInterface($("#pk_totime"));

        var toTime = $scope.TOTIME;
        $("#pk_totime").datepicker("option", 'minDate', toTime);
        $("#pk_totime").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTime)
        });
        SetVietNameInterface($("#pk_totime"));
    }
    //End init time

    $scope.LISTMAILTYPE = [
        {
            value: 'Phat-hanh', name: "Thông báo phát hành"
        },
        {
            value: 'Huy', name: "Thông báo hủy"
        }
    ];

    $scope.LISTMAILSTATUS = [
        {
            value: 1, name: "Đã gửi"
        },
        {
            value: 2, name: "Đã xem"
        },
        {
            value: 3, name: "Gửi lỗi"
        }
    ];

}]);