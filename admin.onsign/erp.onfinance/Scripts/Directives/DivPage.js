
// Control
app.directive('divpage', function ($timeout, $parse) {
    return {
        restrict: 'E',
        scope: {
            pageclick: '&',
            pagemodel: '=',
            totalpage: '@',
            currentpage: '@'
        },
        templateUrl: '/Directives/divPage.html',
        link: function (scope, element, attr) {
            scope.range = new Array(100);

            scope.listPage = [];

            scope.$watch('currentpage', function (newValue, oldValue) {
                rebuildPage(parseInt(newValue), parseInt(scope.totalpage));
            });

            scope.$watch('totalpage', function (newValue, oldValue) {
                rebuildPage(parseInt(scope.currentpage), parseInt(newValue));
            });

            scope.pageOnClick = function (page) {
                scope.pagemodel = page;
                $timeout(scope.pageclick, 50);
            }

            var rebuildPage = function (currentPage, totalPage) {
                var listPage = [];
                var startPage = 1, endPage = totalPage;

                //Hiển thị page 1
                if (currentPage > 3) {
                    startPage = currentPage - 2;
                    var p = {};
                    p.page = 1;
                    p.text = 1;
                    listPage.push(p);
                }

                //Có hiển thị 3 chấm ở đầu hay không?
                if (currentPage > 4) {
                    var p = {};
                    p.page = currentPage - 3;
                    p.text = '...';
                    listPage.push(p);
                }

                //các nút ở giữa
                endPage = Math.min(currentPage + 2, totalPage);
                for (var i = startPage; i <= endPage; i++) {
                    var p = {};
                    p.page = i;
                    p.text = i;
                    listPage.push(p);
                }

                //có hiển thị 3 chấm gần cuối hay không
                if (currentPage + 3 < totalPage) {
                    var p = {};
                    p.page = currentPage + 3;
                    p.text = '...';
                    listPage.push(p);
                }

                //có hiển thị Page cuối hay không?
                if (currentPage + 2 < totalPage) {
                    var p = {};
                    p.page = totalPage;
                    p.text = totalPage;
                    listPage.push(p);
                }

                scope.listPage = listPage;
            }

            var initialise = function () {
            }

            initialise();
        }
    };
});