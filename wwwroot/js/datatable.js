function BindDataTable({
    tableSelector,
    ajaxUrl,
    columnDefs,
    additionalData,
    initComplete,
    drawComplete,
    rowSelection
}) {
    var dataTable = $(tableSelector).DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": ajaxUrl,
            "type": "GET",
            "data": function (d) {
                var requestData = {
                    start: d.start,
                    length: d.length,
                    draw: d.draw,
                    search: d.search.value,
                    order: d.columns[d.order[0].column].data,
                    orderDir: d.order[0].dir
                };

                // Merge additionalData into the request data
                if (additionalData) {
                    $.extend(requestData, additionalData);
                }

                return requestData;
            }
        },
        lengthMenu: [
            [10, 20, 50, 100],
            [10, 20, 50, 100]
        ],
        "columnDefs": columnDefs,
        "columns": columnDefs.map(obj => ({ title: obj.title || obj.name, data: obj.name })),
        "initComplete": function () {
            if (typeof initComplete === 'function') {
                // Pass the DataTable instance to the callback
                initComplete(dataTable); 
            }
        },
        drawCallback: function () {
            if (typeof drawComplete === 'function') {
                // Pass the DataTable instance to the callback
                drawComplete(dataTable);
            }
        }
    });


    /*[
        {
            "data": null,
            "orderable": false,
            "className": "dt-body-center",
            "render": function (data, type, row, meta) {
                return '<input type="checkbox" class="row-select-checkbox">';
            }
        }
    ]*/

    // Array to store selected row data
    var selectedRows = []; 

    // Handle row selection change using DataTables events
    $(tableSelector + ' tbody').on('change', '.row-select-checkbox', function () {
        var checkbox = this;
        var row = dataTable.row($(checkbox).closest('tr'));
        var rowData = row.data();

        if (row) {
            if (checkbox.checked) {
                // Checkbox checked: Add row data to selectedRows
                if (!selectedRows.some(r => r.id === rowData.id)) {
                    selectedRows.push(rowData);
                }
            } else {
                // Checkbox unchecked: Remove row data from selectedRows
                selectedRows = selectedRows.filter(r => r.id !== rowData.id);
            }

            // Invoke rowSelection callback with updated selectedRows
            if (typeof rowSelection === 'function') {
                rowSelection(selectedRows);
            }
        }
    });

    // Return the DataTable instance
    return dataTable;
}