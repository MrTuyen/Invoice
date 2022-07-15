app.factory('CommonFactory', ['$timeout', function ($timeout) {
    var service = {};

    service.PostDataAjax = function (url, data, callBack, timeout) {
        if (!timeout)
            timeout = 60000;
        var tokenString = "";
        var uuid = "";
        var storeID = 0;

        $.ajax({
            url: url,
            type: "POST",
            headers: {
                "X-Requested-With": "Novaon-DS",
                "Token-String": tokenString,
                "Device-UUID": uuid,
            },

            timeout: timeout,
            cache: true,
            crossDomain: true,
            contentType: "application/json; charset=utf-8;",
            dataType: "json",
            data: data,
            processData: true,
            beforeSend: function () { },
            async: true,
            tryCount: 0,
            retryLimit: 3,

            success: function (response) {
                if (response) {
                    $timeout(function () {
                        callBack(response);
                    }, 10);
                } else {
                    $timeout(callBack, 10);
                }
            },

            error: function (error) {
                $timeout(callBack, 10);
            }
        });
    };


    return service;
}]);

app.factory('rootVar', function () {
    var mytemp = null;//the object to hold our data
    return {
        getTemp: function () {
            return mytemp;
        },
        setTemp: function (value) {
            mytemp = value;
        }
    }

});

app.factory('persistObject', function () {

    var persistObject = [];

    function set(objectName, data) {
        persistObject[objectName] = data;
    }
    function get(objectName) {
        return persistObject[objectName];
    }
    return {
        set: set,
        get: get
    }
});

app.factory('permissions', function ($rootScope) {
    var permissionList;
    return {
        setPermissions: function (permissions) {
            permissionList = permissions;
            $rootScope.$broadcast('permissionsChanged');
        },
        hasPermission: function (permissions) {
            var hasPermission = false;
            if (!permissionList) {
                if (localStorage.roles && typeof (localStorage.roles).toLowerCase() == 'string')
                    permissionList = [localStorage.roles];
                else
                    permissionList = localStorage.roles;
            }
            if (!permissionList) return false;
            angular.forEach(permissions, function (permission, index) {
                if (!hasPermission) {
                    permission = permission.trim();
                    return angular.forEach(permissionList, function (item) {
                        if (item) {
                            hasPermission = hasPermission || item.trim() === permission;
                        }
                    });
                }
            });
            return hasPermission;
        }
    };
});

//app.directive('modal', function () {
//    return {
//        template: '<div class="modal fade">' +
//            '<div class="modal-dialog">' +
//            '<div class="modal-content">' +
//            '<div class="modal-header">' +
//            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
//            '<h4 class="modal-title">{{ title }}</h4>' +
//            '</div>' +
//            '<div class="modal-body" ng-transclude></div>' +
//            '</div>' +
//            '</div>' +
//            '</div>',
//        restrict: 'E',
//        transclude: true,
//        replace: true,
//        scope: true,
//        link: function postLink(scope, element, attrs) {
//            scope.title = attrs.title;

//            scope.$watch(attrs.visible, function (value) {
//                if (value == true)
//                    $(element).modal('show');
//                else
//                    $(element).modal('hide');
//            });

//            $(element).on('shown.bs.modal', function () {
//                scope.$apply(function () {
//                    scope.$parent[attrs.visible] = true;
//                });
//            });

//            $(element).on('hidden.bs.modal', function () {
//                scope.$apply(function () {
//                    scope.$parent[attrs.visible] = false;
//                });
//            });
//        }
//    };
//});

app.directive('goClick', function ($location) {
    return function (scope, element, attrs) {
        var path;

        attrs.$observe('goClick', function (val) {
            path = val;
        });

        element.bind('click', function () {
            scope.$apply(function () {
                $location.path(path);
            });
        });
    };
});

app.directive('autoComplete', function ($timeout) {
    return function (scope, iElement, iAttrs) {
        iElement.autocomplete({
            source: scope[iAttrs.uiItems],
            select: function () {
                $timeout(function () {
                    iElement.trigger('input');
                }, 0);
            }
        });
    };
});

//app.directive('draggable', function () {
//    return {
//        // A = attribute, E = Element, C = Class and M = HTML Comment
//        restrict: 'A',
//        link: function (scope, element, attrs) {
//        }
//    }
//});

//app.directive('droppable', function ($compile) {
//    return {
//        restrict: 'A',
//        link: function (scope, element, attrs) {
//            //element.droppable({
//            //    accept: ".dashboard-review-box",
//            //    hoverClass: "drop-hover",
//            //    drop: function (event, ui) { }
//            //})
//        }
//    }
//});

app.directive('sortable', function () {
    return {
        restrict: 'A',
        link: function (scope, elt, attrs) {
            return elt.sortable({
                connectToSortable: $("#iframe_templateViewInvoice").contents().find("#sortable").sortable({
                    //items: "> div",
                    revert: true,
                    iframeFix: true
                }),
                revert: true,
                iframeFix: true
                //revert: true,
                //stop: function (evt, ui) {
                //    return scope.apply(function () {
                //        /* code goes here */
                //    });
                //}
            });
        }
    };
});
