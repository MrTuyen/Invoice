var login = angular.module('novaonlogin', ["ngRoute"]);
login.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/', {
            templateUrl: '/Account/Login',
            controller: 'accountController'
        })
        .when('/dang-nhap', {
            templateUrl: '/Account/Login',
            controller: 'accountController'
        })
        .when('/dang-ky', {
            templateUrl: '/Account/Register',
            controller: 'accountController'
        })
        .otherwise({
            redirectTo: '/'
        });    
}]);