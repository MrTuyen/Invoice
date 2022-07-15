app.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    $routeProvider
        .when('/tong-quan', {
            templateUrl: '/Home/Home'
        })
        .when('/quan-ly-hoa-don', {
            templateUrl: '/Invoice/Index',
        })
        .when('/quan-ly-khach-hang', {
            templateUrl: '/Customer/Index'
        })
        .when('/chi-tiet-khach-hang', {
            templateUrl: '/Customer/Detail',
            controller: 'CustomerController'
        })
        .when('/quan-ly-san-pham', {
            templateUrl: '/Product/Index',
            controller: 'ProductController'
        })
        .when('/quan-ly-hang-hoa-dich-vu', {
            templateUrl: '/Category/Index',
            controller: 'CategoryController'
        })
        .when('/cai-dat', {
            templateUrl: '/Settings/Index',
        })
        .when('/thong-tin-tai-khoan', {
            templateUrl: '/Settings/UserSettings',
        })
        .when('/dai-hoa-don-cho', {
            templateUrl: '/Invoice/InvoiceWait',
        })
        .when('/cai-dat-tai-khoan', {
            templateUrl: '/Settings/AccountSetting',
        })
        .when('/thong-bao-phat-hanh', {
            templateUrl: '/Settings/Announcement'
        })
        .when('/quan-ly-nguoi-dung', {
            templateUrl: '/User/Index'
        })
        .when('/cong-cu', {
            templateUrl: '/Settings/Tool',
            controller: 'SettingsController'
        })
        .when('/danh-muc', {
            templateUrl: '/Settings/AttachementFile',
            controller: 'SettingsController'
        })
        .when('/bao-cao-tinh-hinh-su-dung-hoa-don', {
            templateUrl: '/Statistic/UsingInvoice',
            controller: 'StatisticController'
        })
        .when('/bang-ke-hoa-don-dau-ra', {
            templateUrl: '/Statistic/OutputInvoice',
            controller: 'StatisticController'
        })
        .when('/lich-su-gui-mail', {
            templateUrl: '/Statistic/MailHistory',
            controller: 'StatisticController'
        })
        .when('/bao-cao-lich-su-hoat-dong', {
            templateUrl: '/Statistic/ActionHistory',
            controller: 'StatisticController'
        })
        .when('/mau-hoa-don/:id', {
            templateUrl: '/TempInvoice/Index',
            controller: 'TempInvoiceController'
        })
        .when('/settings-template/:id', {
            templateUrl: '/SettingTemplate/Index',
            controller: 'SettingTemplateController'
        })
        .when('/mau-hoa-don', {
            templateUrl: '/TempInvoice/Index',
            controller: 'TempInvoiceController'
        })
        .when('/bien-lai-thu-phi-le-phi', {
            templateUrl: '/Receipt/Index',
            controller: 'ReceiptController'
        })
        .when('/quan-ly-cong-to', {
            templateUrl: '/Meter/Index',
            controller: 'MeterController'
        })
        .when('/nhat-ky-truy-cap', {
            templateUrl: '/Log/Index',
            controller: 'LogController'
        })
        .when('/tien-thanh-toan', {
            templateUrl: '/CurrencyUnit/Index',
            controller: 'CurrencyUnitController'
        })
        .when('/hinh-thuc-thanh-toan', {
            templateUrl: '/PaymentMethod/Index',
            controller: 'PaymentMethodController'
        })
        .when('/don-vi-tinh', {
            templateUrl: '/QuantityUnit/Index',
            controller: 'QuantityUnitController'
        })
        .when('/cai-dat-he-thong', {
            templateUrl: '/Settings/Parameters',
            controller:'SettingsController'
        })
        .otherwise({
            redirectTo: '/tong-quan'
        });
    $locationProvider
        .html5Mode(false)
        .hashPrefix('');
}]);
