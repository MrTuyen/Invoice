app.controller('HomeController', ['$scope', '$timeout', 'CommonFactory', '$rootScope', function ($scope, $timeout, CommonFactory, $rootScope) {
    var url = '/Home/';
    //var chart;
    //var chart2;
    $scope.ActiveTab = 1;
    LoadingShow();
    angular.element(function () {
        LoadingHide();
    });
    //$("#dashboard-drag").sortable({
    //    revert: true
    //});

    $scope.DashboardType = {
        value: "1",    //Default value
        Options: [
            { value: "1", text: 'Dashboard' },
            { value: "2", text: 'Video' }
        ]
    };

    $scope.ChangeDashboard = function (value) {
        $scope.DashboardType.value = value;
    }

    $scope.CheckEnterpriseInfor = function () {
        if (
            ((!$rootScope.UserInfo.USERNAME === "demo" || !$rootScope.UserInfo.EMAIL.includes("demo")) && $rootScope.Enterprise.COMTAXCODE === "0106579683-999")
            || ($rootScope.Enterprise.ISFREETRIAL === true && (getCookie("NovaonFreeTrial") === false || getCookie("NovaonFreeTrial") === undefined))
        )
        {
            $(".modal-onboarding").modal('show');
            $scope.Enterprise.COMTAXCODE = "";
            $scope.Enterprise.COMNAME = "";
            $scope.Enterprise.COMADDRESS = "";
            $scope.Enterprise.COMPHONENUMBER = "";
            $scope.Enterprise.COMEMAIL = "";
            $scope.Enterprise.COMACCOUNTNUMBER = "";
            $scope.Enterprise.COMBANKNAME = "";
            $scope.Enterprise.COMLEGALNAME = "";
            $scope.Enterprise.TAXDEPARTEMENTCODE = "";
            $scope.Enterprise.TAXDEPARTEMENT = "";
        }
    }

    $scope.SetActiveTab = (step) => {
        if (true) {

        }
        $scope.ActiveTab = $scope.ActiveTab + step;
    }

    $scope.SaveEnterpriseInfo = function () {
        if (!$scope.Enterprise.COMTAXCODE) {
            alert('Vui lòng nhập mã số thuế doanh nghiệp.');
            return;
        }
        if (!$scope.Enterprise.COMNAME) {
            alert('Vui lòng nhập tên doanh nghiệp.');
            return;
        }
        if (!$scope.Enterprise.COMADDRESS) {
            alert('Vui lòng nhập địa chỉ doanh nghiệp.');
            return;
        }
        var action = url + "AddEnterpriseInfoFreeVersion";
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        var confirmContinue = function (result) {
            if (!result)
                return false;
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        alert('Cập nhật thông tin doanh nghiệp thành công!');
                        $rootScope.Enterprise.ISFREETRIAL = true;
                        setCookie("NovaonFreeTrial", true, 90)
                    } else {
                        alert('Doanh nghiệp với mã số thuế ' + '<b class="text-danger">' + $scope.Enterprise.COMTAXCODE +'</b> đã tồn tại.');
                    }
                } else {
                    alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveEnterpriseInfo");
                }
            });
        };
        confirm("Bạn có thực sự chắc chắn những thông tin trên đúng với doanh nghiệp của bạn? Bạn chỉ có thể cập nhật thông tin này <b class='text-danger'>1 lần duy nhất</b>.", "Thông báo", "Không", "Có", confirmContinue)
    };

    $scope.LoadInfoByTaxcode = function () {
        if ($rootScope.Enterprise.COMTAXCODE != null && $rootScope.Enterprise.COMTAXCODE != "") {
            var url = 'https://app.meinvoice.vn/Other/GetCompanyInfoByTaxCode?taxCode=' + $rootScope.Enterprise.COMTAXCODE;
            $("#loadTaxinfo").load(url, function (response, status, xhr) {
                var rawObj = JSON.parse(response);
                var obj = new Object();
                if (rawObj.Data != "") {
                    obj = JSON.parse(rawObj.Data);
                }
                $timeout(function () {
                    if (obj.companyName) {
                        $rootScope.Enterprise.COMADDRESS = obj.address;
                        $rootScope.Enterprise.COMNAME = obj.companyName.toUpperCase();
                    } else {
                        $rootScope.Enterprise.COMNAME = null;
                        $rootScope.Enterprise.COMADDRESS = null;
                    }
                });
            });
        } else {
            $rootScope.Enterprise.COMNAME = null;
            $rootScope.Enterprise.COMADDRESS = null;
        }
    }

    $scope.ShowDateTimePopup = function () {
        $('#dateTimePopUp').dialog({
            modal: true,
            resizable: false,
            width: 450,
            height: 200,
            open: function () {
                $(this).dialog("option", "title", "Lọc theo thời gian");
            },
            beforeClose: function (e, ui) {
                $scope.Timepickers.value = $scope.Timepickers.Options[0].value;
                $timeout(function () {
                    $("select.cb-select-time").selectpicker('refresh');
                }, 500)
            }
        });
    }

    $scope.LoadDashboard = function () {
        var searchTime = $scope.Timepickers.value;
        if ($scope.Timepickers.value == 5) {
            searchTime = moment($scope.FROMTIME).format('YYYY-MM-DD') + ';' + moment($scope.TOTIME).format('YYYY-MM-DD');
        }
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
            dateRange: searchTime
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.TotalMoneyBeforVAT = response.TotalMoneyBeforVAT;
                    $scope.TotalMoneyAfterVAT = response.TotalMoneyAfterVAT;
                    $scope.TotalMoneyVAT = response.TotalMoneyVAT;

                    $timeout(function () {
                        //load chart
                        var lstX = [];
                        var beforvatY = [];
                        var aftervatY = [];
                        var vatY = [];
                        //
                        var userY = [];
                        var cancelY = [];
                        var replaceY = [];

                        if (response.chartData != null) {
                            response.chartData.forEach(function (item) {
                                lstX.push(item.Date);
                                beforvatY.push(item.BeforVAT);
                                aftervatY.push(item.AfterVAT);
                                vatY.push(item.VAT);
                                //
                                userY.push(item.CountUse);
                                cancelY.push(item.CountCancel);
                                replaceY.push(item.CountReplace);
                            });

                            //Chart line
                            chart.updateOptions({
                                xaxis: {
                                    categories: lstX
                                }
                            });

                            chart.updateSeries([{
                                data: beforvatY
                            }, {
                                data: aftervatY
                            }, {
                                data: vatY
                            }], true);


                            //Chart column
                            chart2.updateOptions({
                                xaxis: {
                                    categories: lstX
                                }
                            });

                            chart2.updateSeries([{
                                data: userY
                            }, {
                                data: cancelY
                            }, {
                                data: replaceY
                            }], true);
                        }
                    }, 100)
                }
                $scope.CheckEnterpriseInfor();
            } 
        });
    };

    $scope.LoadData = function () {
        if ($scope.Timepickers.value == 5) {
            $scope.ShowDateTimePopup();
        }
        $scope.LoadDashboard();
    }

    //UI/UX
    $timeout(function () {
        $('select.cb-select-time').selectpicker();
    });


    //Init chart 
    var options = {
        chart: {
            height: 350,
            type: 'area',
            zoom: {
                enabled: false
            }
        },
        dataLabels: {
            enabled: true,
            formatter: function (val, opts) {
                return val.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
            }
        },
        stroke: {
            curve: 'smooth', //straight
            width: 2,
        },
        series: [{
            name: 'Doanh thu trước thuế',
            data: []
        }, {
            name: 'Doanh thu sau thuế',
            data: []
        }, {
            name: 'Tiền thuế',
            data: []
        }],
        colors: ['#2CA01C', '#3797D3', '#24BAB5'],
        markers: {
            size: 4
        },
        xaxis: {
            type: 'datetime',
            categories: [],
        },
        tooltip: {
            x: {
                format: 'dd/MM/yy'
            },
            y: {
                formatter: function (val) {
                    return val.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,') + " VND"
                }
            }
        },
        legend: {
            horizontalAlign: 'center',
            position: 'top'
        }
    }

    var chart = new ApexCharts(
        document.querySelector("#chart"),
        options
    );

    chart.render();

    var options2 = {
        chart: {
            height: 350,
            type: 'bar',
            stacked: true,
            zoom: {
                enabled: false
            }
        },
        series: [{
            name: 'Số hoá đơn sử dụng',
            data: []
        }, {
            name: 'Số hoá đơn huỷ',
            data: []
        }, {
            name: 'Số hoá đơn thay thế',
            data: []
        }],
        xaxis: {
            type: 'datetime',
            categories: [],
        },
        legend: {
            horizontalAlign: 'center',
            position: 'top'
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + " đơn"
                }
            }
        },
    }

    var chart2 = new ApexCharts(
        document.querySelector("#chart2"),
        options2
    );

    chart2.render();

    // custom datepicker for input text
    $scope.SetDatepikerFromTime = function () {
        $("#pk_fromtime_home").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_home"));
    }

    $scope.SetDatepikerToTime = function () {
        var fromTime = $scope.FROMTIME;
        $("#pk_totime_home").datepicker("option", 'minDate', fromTime);
        $("#pk_totime_home").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(fromTime)
        });
        SetVietNameInterface($("#pk_totime_home"));
    }
    //--------------------------------------
}]);
