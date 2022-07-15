
app.directive('suggestproduct', function ($timeout, $rootScope) {
    return {
        restrict: "E",
        //require: "?ngModel",
        scope: {
            textboxplaceholder: "=?",
            pagesize: "=?",
            selectsearchresult: "&",
            keypress: "&",
            //selectresultmodel: "=?",//Trả về ProductModel
            selectresultid: "=?",//Trả về ProductID
            selectbarcoderesult: "&",
            autoclear: "=?",
            isbasequantityunit: "=?", //Khi bắn barcode sẽ luôn luôn trả ra mã sản phẩm có đơn vị tính cơ sở
            delayresearch: "=?",
            textboxmaxlength: "=?",
            textview: "=?",
            disabled: "=?",
            userkeycode: "=?",
            fucusme: "=?"
        },
        transclude: true,
        templateUrl: "/Directives/SuggestProduct.html?v=12",
        link: function (scope, element, attrs) {
            //if (!ngModel) return;

            var submissionHandler = null;
            //#region Variable
            //Cờ show list product
            scope.isShowlist = true;
            scope.isMousOverFocus = false;
            scope.delayresearch = scope.delayresearch || 500;
            scope.textboxmaxlength = scope.textboxmaxlength || 1000;
            scope.keyword = scope.textview || '';
            scope.fucusme = scope.fucusme || true;
            //#endregion

            scope.clearTextSearch = function () {
                scope.lstObject = null;
                scope.keyword = null;
            }

            scope.KeyDown = function (event) {
                if (event.keyCode == 40 || event.keyCode == 38) {  //Down UP arrow
                    if (scope.lstObject != null && scope.lstObject.length > 0) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //index =-1 có nghĩa là chưa có item được chọn, index > -1 có nghĩa là đang được chọn
                        //Tăng lên 1 để chọn cái tiếp theo
                        //Giảm đi 1 để chọn cái bên trên
                        index += (event.keyCode == 40 ? 1 : -1);

                        //Nếu tăng quá mức thì quay về đầu tiên index = 0
                        if (index >= scope.lstObject.length)
                            index = 0;
                        else if (index < 0)
                            index = scope.lstObject.length - 1;

                        //Clear hết trước khi chọn lại
                        scope.lstObject.forEach(function (obj) {
                            obj.isSelected = false;
                        });
                        //Chọn cái mới
                        scope.lstObject[index].isSelected = true;
                    } else {
                        //Gọi hàm thêm dòng, nếu được khai báo
                        //ngModel.$setViewValue(event.keyCode);
                        scope.userkeycode = event.keyCode;
                        scope.keypress();
                    }
                } else if (event.keyCode == 13) {
                    //Enter
                    if (scope.lstObject != null && scope.lstObject.length > 0 && scope.isShowlist) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //Nếu chưa có cái nào được chọn thì chọn cái đầu tiên
                        index = index == -1 ? 0 : index;
                        scope.redirect(scope.lstObject[index]);
                    }

                    //Gọi hàm thêm dòng, nếu được khai báo
                    //ngModel.$setViewValue(event.keyCode);
                    scope.userkeycode = event.keyCode;
                    scope.keypress();

                } else if (scope.keyword && scope.keyword.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8)
                    event.preventDefault();
            }

            scope.LossFocus = function () {
                if (!scope.isMousOverFocus) {
                    scope.isShowlist = false;
                }
            }

            scope.MousOverFocus = function (isOver) {
                scope.isMousOverFocus = isOver;
                if (isOver) {
                    //Clear selection
                    for (var i = 0; i < scope.lstObject.length; i++) {
                        if (scope.lstObject[i].isSelected) {
                            scope.lstObject[i].isSelected = false;
                            break;
                        }
                    }
                }
            }

            scope.search = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                submissionHandler = $timeout(function () {
                    if (scope.keyword.length >= 2) {
                        scope.isSearching = true;
                        scope.errorMessage = null;

                        $.ajax({
                            url: "/Product/SuggestByObject",
                            type: "POST",
                            dataType: 'json',
                            contentType: 'application/json;charset=utf-8;',
                            async: false,
                            data: JSON.stringify({ strKeyword: scope.keyword, intPageSize: scope.pagesize }),
                            success: function (result) {
                                $timeout(function () {
                                    if (result) {
                                        if (result.rs) {
                                            //Thêm điều kiện keyword.length > 0 để tránh trường hợp người dùng chọn rồi mà server chưa phản hồi danh sách
                                            if (scope.keyword.length > 0) {
                                                scope.lstObject = result.listResult;
                                            }
                                        } else {
                                            alert(result.msg);
                                        }
                                    }
                                    //Set biến không phải quét từ mã vạch
                                    scope.isRedirect = false;
                                    scope.isSearching = false;
                                });
                            }
                        });
                    } else {
                        //Set biến không phải quét từ mã vạch
                        scope.isRedirect = false;
                        scope.lstObject = null;
                    }

                    scope.textview = scope.keyword;
                }, scope.delayresearch);
            }

            scope.redirect = function (obj) {
                $timeout.cancel(submissionHandler);
                scope.isSearching = false;
                scope.lstObject = null;
                //Gán giá trị đã chọn cho biến toàn cục
                $rootScope.selectedResultobj = obj;
                //Gọi hàm xử lý
                $timeout(scope.selectsearchresult, 50);
                //Gán lại vào input
                scope.keyword = obj.PRODUCTNAME;
            }
        }
    };
});