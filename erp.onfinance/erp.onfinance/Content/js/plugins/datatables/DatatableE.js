var datatableE = {
    Init: function (querySelector, url, param, columns, columnDefs, createdRowFunction, drawCallbackFunction, initCompleteFunction) {
        var table = $(querySelector).DataTable({
            "serverSide": true,
            "searching": true,
            "proccesing": true,
            "stateSave": true,
            "ajax": {
                "url": url,
                "method": "post",
                "data": param,
                "dataSrc": "data"
            },
            initComplete: function (settings, json) {
                $(querySelector).wrap('<div class="DatatableWrapDiv"></div>');
                initCompleteFunction(settings, json);
                $(".DatatableWrapDiv").css("overflow-x", "auto");
                $(".DatatableWrapDiv").css("width", "100%");
                //$(".DatatableWrapDiv table").css("max-width", "100%");
            },
            "drawCallback": function (settings) {
                drawCallbackFunction(settings, settings.json.data);
            },
            "language": {
                "lengthMenu": "Hiển thị _MENU_ dòng trên trang",
                "zeroRecords": "Không có dữ liệu",
                //"info": "Đang hiển thị trang _PAGE_ trong tổng _PAGES_ trang",
                "info": "",
                "infoEmpty": "Không có dòng dữ liệu nào",
                "infoFiltered": "(Lọc từ _MAX_ dòng)",
                "search": "Tìm kiếm",
                "paginate": {
                    "previous": "Trước",
                    "next": "Tiếp"
                }
            },
            "columns": columns,
            "columnDefs": columnDefs,
            "createdRow": function (row,data,dataIndex) {
                createdRowFunction(row, data, dataIndex);
            }
        });
        return table;
    }
};