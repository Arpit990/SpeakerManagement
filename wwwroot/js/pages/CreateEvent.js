$(document).ready(function () {
    flatpickr("#EventDate", {
        dateFormat: "d/m/Y",
        minDate: "today"
    });

    $('#addForm').on("click", function () {
        var newForm = $('#EventForm .taskSection').first().clone();
        $('.taskSection').append(newForm);
    });
});