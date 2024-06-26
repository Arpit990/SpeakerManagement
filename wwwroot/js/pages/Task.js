var tblTasks;

$(document).ready(function () {
    FormValidation();
    BindTaskTable();

    $("#btnAddTask").on("click", function () {
        $("#addEditTaskModalLabel").html("Add Task");
        $("#addEditTaskModalForm").attr("action", "/Task/Create");
        $("#addEditTaskModal").modal("show");
    });

    $(".btnClose").on("click", function () {
        OnModalHide("#addEditTaskModal", function () {
            ResetForm("#addEditTaskModalForm");
        });
    });
});

function BindTaskTable() {
    var colDef = [
        {
            "title": "Ser No",
            "name": null,
            "orderable": false,
            "targets": 0,
            "render": function (data, type, row, meta) {
                // Generate auto-incremented serial number
                return meta.row + 1;
            }
        },
        { "title": "Task Name", "name": "taskName", "orderable": false, "targets": 1 },
        { "title": "Input Type", "name": "inputType", "orderable": false, "targets": 2 },
        { "title": "Validation", "name": "validation", "orderable": false, "targets": 3 },
        { "title": "Instruction", "name": "instructions", "orderable": false, "targets": 4 },
        {
            "title": "Is Optional",
            "name": "isOptional",
            "orderable": false,
            "targets": 5,
            "render": function (data, type, row) {
                if (row.isOptional) {
                    return "Yes";
                }
                return "No";
            }
        },
        {
            "title": "Action",
            "name": null,
            "orderable": false,
            "targets": 6,
            "render": function (data, type, row) {
                return `
                    <a href="javascript: void(0)" data-id="${row.id}" class="text-primary fs-5 text-decoration-none btnEditTask">
                        <svg xmlns="http://www.w3.org/2000/svg" width="1em" height="1em" viewBox="0 0 24 24"><g fill="none"><path stroke="currentColor" d="m5.93 19.283l.021-.006l2.633-.658l.045-.01c.223-.056.42-.105.599-.207c.179-.101.322-.245.484-.407l.033-.033l7.194-7.194l.024-.024c.313-.313.583-.583.77-.828c.2-.263.353-.556.353-.916s-.152-.653-.353-.916c-.187-.245-.457-.515-.77-.828l-.024-.024l-.353.354l.353-.354l-.171-.171l-.024-.024c-.313-.313-.583-.583-.828-.77c-.263-.2-.556-.353-.916-.353s-.653.152-.916.353c-.245.187-.515.457-.828.77l-.024.024l-7.194 7.194a7.24 7.24 0 0 1-.033.032c-.162.163-.306.306-.407.485c-.102.18-.15.376-.206.6l-.011.044l-.664 2.654a12.99 12.99 0 0 0-.007.027a3.72 3.72 0 0 0-.095.464c-.015.155-.011.416.198.626c.21.21.47.213.625.197a3.43 3.43 0 0 0 .492-.101Z" /><path fill="currentColor" d="m12.5 7.5l3-2l3 3l-2 3z" /></g></svg>
                    </a>
                    <a href="javascript: void(0)" data-id="${row.id}" class="text-danger fs-5 text-decoration-none btnDeleteTask">
                        <svg xmlns="http://www.w3.org/2000/svg" width="1em" height="1em" viewBox="0 0 28 28"><path fill="currentColor" d="M11.5 6h5a2.5 2.5 0 0 0-5 0M10 6a4 4 0 0 1 8 0h6.25a.75.75 0 0 1 0 1.5h-1.31l-1.217 14.603A4.25 4.25 0 0 1 17.488 26h-6.976a4.25 4.25 0 0 1-4.235-3.897L5.06 7.5H3.75a.75.75 0 0 1 0-1.5zM7.772 21.978a2.75 2.75 0 0 0 2.74 2.522h6.976a2.75 2.75 0 0 0 2.74-2.522L21.436 7.5H6.565zM11.75 11a.75.75 0 0 1 .75.75v8.5a.75.75 0 0 1-1.5 0v-8.5a.75.75 0 0 1 .75-.75m5.25.75a.75.75 0 0 0-1.5 0v8.5a.75.75 0 0 0 1.5 0z" /></svg>
                    </a>`;
            }
        }
    ]

    tblTasks = BindDataTable({
        tableSelector: "#tblTasks",
        ajaxUrl: "/Task/GetTaskList",
        columnDefs: colDef,
        drawComplete: function () {
            $(".btnEditTask").off().on("click", function () {
                $("#addEditTaskModalLabel").html("Edit Task");
                $("#addEditTaskModalForm").attr("action", "/Task/Edit");
                $("#addEditTaskModal").modal("show");

                let id = $(this).attr("data-id");
                editTaskDetail(id);
            });

            $(".btnDeleteTask").off().on("click", function () {
                let id = $(this).attr("data-id");
                DeleteConformation({
                    confirmCallback: function () {
                        GlobalAjax({
                            url: "/Task/Delete",
                            type: "POST",
                            data: { id: id },
                            successCallback: function (response) {
                                showAlert(response.responseJSON);
                                tblTasks.draw();
                            },
                            errorCallback: function (xhr, status, error) {
                                console.error(error);
                            }
                        })
                    }
                });
            });
        }
    })
}

function SubmitFormData() {
    let url = $("#addEditTaskModalForm").attr('action');
    let data = SerializeFormData("#addEditTaskModalForm");

    GlobalAjax({
        url: url,
        type: "POST",
        data: data,
        successCallback: function (response) {
            $("#addEditTaskModal").modal("hide");
            ResetForm("#addEditTaskModalForm");
            showAlert(response.responseJSON);
            tblTasks.draw();
        },
        errorCallback: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function editTaskDetail(id) {
    GlobalAjax({
        url: "/Task/Get",
        type: "GET",
        data: { id: id },
        successCallback: function (response) {
            if (response.responseJSON.isSuccess)
                BindFormData(response.responseJSON.responseData, "#addEditTaskModalForm");
            else
                showAlert(response.responseJSON);
        },
        errorCallback: function (xhr, status, error) {
            console.error(error);
        }
    });
}


function FormValidation() {
    $("#addEditTaskModalForm").validate({
        rules: {
            TaskName: {
                required: true,
            },
            InputType: {
                required: true
            },
            Validation: {
                required: true
            }
        },
        messages: {
            TaskName: "Please Enter Task Name",
            InputType: "Please Select Input Type",
            Validation: "Please Select Validation Type",
        },
        normalizer: function (value) {
            return $.trim(value);
        },
        highlight: function (element) {
            $(element).addClass('is-invalid').removeClass('is-valid');
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid').addClass('is-valid');
        },
        submitHandler: function () {
            SubmitFormData();
        }
    });
}