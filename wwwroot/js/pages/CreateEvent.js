$(document).ready(function () {
    EventFormValidation();
    initFlatpickrForNewInputs();
    toggleDeleteButtonVisibility();

    flatpickr("#EventDate", {
        dateFormat: "d/m/Y",
        minDate: "today"
    });

    $("#cloneTask").on("click", function () {
        cloneTaskForm();
    });

    $(document).on('click', '.removeTask', function () {
        $(this).closest('.taskSection').remove();
        toggleDeleteButtonVisibility();
    });

    /*$("#btnSaveEvent").on("click", function () {
        SubmitFormData();
    });*/
});

function cloneTaskForm() {
    var taskForm = $("#EventForm .taskSection").first().clone();
    taskForm.find("input[name='Deadline']").val("");
    $("#EventForm .taskSection").last().after(taskForm);
    initFlatpickrForNewInputs();
    toggleDeleteButtonVisibility();
}

function initFlatpickrForNewInputs() {
    flatpickr("input[name='Deadline']", {
        dateFormat: "d/m/Y",
        minDate: "today"
    });
}

function toggleDeleteButtonVisibility() {
    if ($('.taskSection').length === 1) {
        $('.removeTask').hide(); // Hide the delete button
    } else {
        $('.removeTask').show(); // Show the delete button
    }
}

function SubmitFormData() {
    let url = $("#EventForm").attr('action');
    let formData = new FormData($("#EventForm")[0]);

    // Serialize tasks
    $('.taskSection').each(function (index) {
        formData.append(`Tasks[${index}].TaskId`, $(this).find('select[name="TaskId"]').val());
        formData.append(`Tasks[${index}].Deadline`, $(this).find('input[name="Deadline"]').val());
    });

    GlobalAjax({
        url: url,
        type: "POST",
        data: formData, 
        isMultipartFormData: true,
        successCallback: function (response) {
            ResetForm("#EventForm");
            showAlert(response.responseJSON);
        },
        errorCallback: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function EventFormValidation() {
    $("#EventForm").validate({
        rules: {
            EventName: {
                required: true,
            },
            EventLogo: {
                required: true
            },
            EventDate: {
                required: true
            },
            TaskId: {
                required: true
            },
            Deadline: {
                required: true
            }
        },
        messages: {
            EventName: "Please Enter Event Name",
            EventLogo: "Please Select Event Logo",
            EventDate: "Please Select Event Date",
            TaskId: "Please Select Task",
            Deadline: "Please Select Task Deadline"
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