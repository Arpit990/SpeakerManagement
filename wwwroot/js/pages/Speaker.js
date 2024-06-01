$(document).ready(function () {
    $(".markAsComplete").on("change", function () {
        if ($(this).is(':checked')) {
            var formData;
            var url;

            // Find the closest form element
            var form = $(this).closest('form');
            var inputType = form.find('input[name="InputType"]').val();

            if (inputType != "File") {
                url = "/Speaker/SaveData";
                formData = new FormData();
                var data = $('textarea[name="data"]').val();;
                formData.append('data', data);
            }
            else {
                url = "/Speaker/SaveFile";
                formData = new FormData(form[0]);
            }
            // Get SpeakerTaskId value from the hidden input field
            var speakerTaskId = form.find('input[name="SpeakerTaskId"]').val(); 

            // Append SpeakerTaskId to the FormData object
            formData.append('speakerTaskId', speakerTaskId);

            SubmitFormData(formData, url);
        }  
    });

    setDeadlineDateColor();
});

function SubmitFormData(formData, url) {
    
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

function setDeadlineDateColor() {
    $('.deadlineDate').each(function (index, element) {
        var taskDeadline = $(this).text().trim();

        // Parse the custom date string into a Moment object
        var customDate = moment(taskDeadline, "DD MMMM YYYY");

        // Get today's date as a Moment object
        var today = moment();

        // Calculate the difference in days
        var differenceInDays = customDate.diff(today, 'days');

        if (differenceInDays <= 1) {
            $(this).addClass("text-danger");
        }
        else if (differenceInDays <= 3) {
            $(this).addClass("text-warning");
        }
        else if (differenceInDays <= 5) {
            $(this).addClass("text-info");
        }
    });
}