app.directive("onlyNumber", function () {
    return {
        restrict: "A",
        link: function (scope, element, attr) {
            element.bind('input', function () {
                var position = this.selectionStart - 1;

                //remove all but number and .
                var fixed = this.value.replace(/[^0-9\.]/g, '');
                if (fixed.charAt(0) === '.')                  //can't start with .
                    fixed = fixed.slice(1);

                var pos = fixed.indexOf(".") + 1;
                if (pos >= 0)               //avoid more than one .
                    fixed = fixed.substr(0, pos) + fixed.slice(pos).replace('.', '');

                if (this.value != fixed) {
                    this.value = fixed;
                    this.selectionStart = position;
                    this.selectionEnd = position;
                }
            });
        }
    };
});

app.directive('ngMin', function ($rootScope) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attr, ctrl) {
            scope.$watch(attr.ngMin, function () {
                ctrl.$setViewValue(ctrl.$viewValue);
            });
            var minValidator = function (value) {
                var min = scope.$eval(attr.ngMin) || 0;
                if (!$rootScope.isNullOrEmpty(value) && value < min) {
                    ctrl.$setValidity('ngMin', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('ngMin', true);
                    return value;
                }
            };

            ctrl.$parsers.push(minValidator);
            ctrl.$formatters.push(minValidator);
        }
    };
});
app.directive('ngMax', function ($rootScope) {
    return {
        restrict: 'A',
        link: function (scope, elem, attr, ctrl) {
            elem.bind('input', function () {
                var val = this.value.replace(/[^0-9\.]/g, '');
                var max = parseFloat(scope.$eval(attr.ngMax)) || Infinity;

                var intVal = parseFloat(val);

                if (val != null && val != '' && intVal > max) {
                    while (intVal > max) {
                        val = val.slice(0, -1);
                        intVal = parseFloat(val);
                    }
                    this.value = val;
                }
            });
        }
    };
});

app.directive('focusOn', ['$timeout',
    function ($timeout) {
        var checkDirectivePrerequisites = function (attrs) {
            if (!attrs.focusOn && attrs.focusOn != "") {
                throw "FocusOnCondition missing attribute to evaluate";
            }
        }

        return {
            restrict: "A",
            link: function (scope, element, attrs, ctrls) {
                checkDirectivePrerequisites(attrs);

                scope.$watch(attrs.focusOn, function (currentValue, lastValue) {
                    if (currentValue == true) {
                        $timeout(function () {
                            element.focus();
                        });
                    }
                });
            }
        };
    }
]);

app.directive('selectOnClick', ['$window', function ($window) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.on('click', function () {
                if (!$window.getSelection().toString()) {
                    // Required for mobile Safari
                    this.setSelectionRange(0, this.value.length)
                }
            });
        }
    };
}]);

app.directive('convertToNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return val != null ? parseInt(val, 10) : null;
            });
            ngModel.$formatters.push(function (val) {
                return val != null ? '' + val : null;
            });
        }
    };
});

app.directive('customOnChange', ['$timeout',
    function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $timeout(function () {
                    var onChangeFunc = scope.$eval(attrs.customOnChange);
                    element.bind('change', onChangeFunc);
                }, 100);
            }
        };
    }]
);

app.filter('numberFormat', function () {
    return function (n, decPlaces, thouSeparator, decSeparator, trimZero, emptyWhenZero) {
        try {
            if (isNaN(n)) {
                return 0;
            }

            if (n == 0) {
                if (emptyWhenZero == "true") {
                    return "";
                } else if (trimZero == "true") {
                    return 0;
                }
            }

            var decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
                decSeparator = decSeparator == undefined ? "." : decSeparator,
                thouSeparator = thouSeparator == undefined ? "," : thouSeparator,
                sign = n < 0 ? "-" : "",
                i = parseInt(n = Math.abs(+n || 0).toFixed(decPlaces)) + "",
                j = (j = i.length) > 3 ? j % 3 : 0;
            return sign + (j ? i.substr(0, j) + thouSeparator : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thouSeparator) + (decPlaces ? decSeparator + Math.abs(n - i).toFixed(decPlaces).slice(2) : "");
        } catch (e) {
            return n;
        }
    }
});

app.filter('dateTimeFormat', function ($filter) {
    function getCurrentDate() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = "0" + dd;
        }

        if (mm < 10) {
            mm = "0" + mm;
        }

        today = dd + "/" + mm + "/" + yyyy;
        return today;
    };

    return function (input) {
        if (!input) { return ""; }
        var temp = input.replace(/\//g, "").replace("(", "").replace(")", "").replace("Date", "").replace("+0700", "").replace("-0000", "");

        var date;
        var resultDate;

        if (input.indexOf("Date") > -1) {

            resultDate = new Date(+temp);
            date = $filter("date")(resultDate, "dd/MM/yyyy");


            var utc = resultDate.getTime() + (resultDate.getTimezoneOffset() * 60000);

            // create new Date object for different city
            // using supplied offset
            resultDate = new Date(utc + (3600000 * 7));

            if (getCurrentDate() === date) {
                return $filter("date")(resultDate, "HH:mm") + " Hôm nay";
            } else {
                return $filter("date")(resultDate, "HH:mm ") + " " + $filter("date")(resultDate, "dd/MM/yyyy");
            }

        } else {

            date = $filter("date")(new Date(temp), "dd/MM/yyyy");

            if (getCurrentDate() === date) {
                return "Hôm nay";
            } else {
                var utc = date.getTime() + (date.getTimezoneOffset() * 60000);

                // create new Date object for different city
                // using supplied offset
                resultDate = new Date(utc + (3600000 * 7));
                return $filter("date")(resultDate, "dd/MM/yyyy");
            }
        }
    };
});


app.filter('dateFormat', function ($filter) {
    return function (input, format) {
        if (!input) { return ""; }
        var temp = input.replace(/\//g, "").replace("(", "").replace(")", "").replace("Date", "").replace("+0700", "").replace("-0000", "");

        var resultDate = new Date(+temp);
        return $filter("date")(resultDate, format);
    };
});

app.filter('sumTotal', function () {
    return function (input, property) {
        var i = input instanceof Array ? input.length : 0;
        if (typeof property === 'undefined' || i === 0) {
            return i;
        } else if (isNaN(input[0][property])) {
            throw 'filter total can count only numeric values';
        } else {
            var total = 0;
            while (i--)
                total += parseFloat(input[i][property]);
            return total;
        }
    };
});

app.filter('totalSumPriceQty', function () {
    return function (data, key1, key2) {
        if (angular.isUndefined(data) || angular.isUndefined(key1) || angular.isUndefined(key2))
            return 0;
        var sum = 0;
        angular.forEach(data, function (value) {
            sum = sum + (parseFloat(value[key1], 10) * parseFloat(value[key2], 10));
        });
        return sum;
    }
});

app.filter('totalWithOperation', function () {
    return function (data, key1, key2, operation) {
        if (angular.isUndefined(data) || angular.isUndefined(key1) || angular.isUndefined(key2) || angular.isUndefined(operation))
            return 0;
        var sum = 0;
        angular.forEach(data, function (value) {
            sum += parseFloat(eval(value[key1] + operation + value[key2]));
        });
        return sum;
    }
});

app.filter('Contains', function () {
    return function (string, substring) {
        return (string.indexOf(substring) != -1);
    };
});

app.filter('IndexOf', function () {
    return function (string, substring) {
        if (!string)
            return false;
        else
            return (string.indexOf(substring) != -1);
    };
});
