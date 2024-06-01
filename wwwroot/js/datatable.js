function BindDataTable({
    tableSelector,
    ajaxUrl,
    columnDefs,
    singleRowSelectable = false,
    additionalData,
    initComplete,
    drawComplete,
    rowSelection
}) {

    // Determine select style based on singleRowSelectable
    var selectStyle = singleRowSelectable ? 'single' : 'multi'; 

    var dataTable = $(tableSelector).DataTable({
        "processing": true,
        "serverSide": true,
        "select": {
            style: selectStyle
        },
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

    // Event listener for row selection
    dataTable.on('select', function (e, dt, type, indexes) {
        var selectedData = dataTable.rows({ selected: true }).data().toArray();
        if (typeof rowSelection === 'function') {
            rowSelection(selectedData);
        }

        // Check the corresponding checkboxes for selected rows
        indexes.forEach(function (index) {
            $(tableSelector + ' tbody tr:eq(' + index + ') .row-select-checkbox').prop('checked', true);
        });
    });

    // Event listener for row deselection
    dataTable.on('deselect', function (e, dt, type, indexes) {
        var selectedData = dataTable.rows({ selected: true }).data().toArray();
        if (typeof rowSelection === 'function') {
            rowSelection(selectedData);
        }

        // Uncheck the corresponding checkboxes for deselected rows
        indexes.forEach(function (index) {
            $(tableSelector + ' tbody tr:eq(' + index + ') .row-select-checkbox').prop('checked', false);
        });
    });

    /*
    // Event listener for row selection using checkboxes
    $(tableSelector + ' tbody').on('click', 'tr', function () {
        var $checkbox = $(this);
        var $row = $checkbox.closest('tr');
        var row = dataTable.row($row);

        if ($checkbox.prop('checked')) {
            row.select(); // Select row if checkbox is checked
        } else {
            row.deselect(); // Deselect row if checkbox is unchecked
        }
    });

    // Array to store selected row data
    var selectedRows = []; 

    // Handle row selection change using DataTables events
    $(tableSelector + ' tbody').on('change', '.row-select-checkbox', function (e) {
        e.stopPropagation(); // Prevent event bubbling

        var checkbox = this;
        var $row = $(checkbox).closest('tr');
        var row = dataTable.row($row);
        var rowData = row.data();

        if (row) {
            if (checkbox.checked) {
                // Checkbox checked: Add row data to selectedRows
                if (!selectedRows.some(r => r.id === rowData.id)) {
                    selectedRows.push(rowData);
                    $row.addClass('selected');
                }
            } else {
                // Checkbox unchecked: Remove row data from selectedRows
                selectedRows = selectedRows.filter(r => r.id !== rowData.id);
                $row.removeClass('selected');
            }

            // Invoke rowSelection callback with updated selectedRows
            if (typeof rowSelection === 'function') {
                rowSelection(selectedRows);
            }
        }
    });

    // Handle header checkbox for selecting/deselecting all rows
    $(tableSelector + ' thead').on('change', '.select-all-checkbox', function () {
        var checkboxes = $(tableSelector + ' tbody .row-select-checkbox');
        var checked = $(this).prop('checked');

        checkboxes.prop('checked', checked).trigger('change');
    });

    // Handle row click to toggle checkbox and row selection
    $(tableSelector + ' tbody').on('click', 'tr', function (e) {
        var $checkbox = $(this).find('.row-select-checkbox');
        $checkbox.prop('checked', !$checkbox.prop('checked')).trigger('change');
    });
    */
    

    // Return the DataTable instance
    return dataTable;
}