function DisplayErrorsOnForm(formSelector, ErrorsArray) {

    if (ErrorsArray != null && ErrorsArray != undefined) {
        $.each(ErrorsArray, function (key, value) {
            // console.log(key, value.Field);
            key = key.toCapitalize();
            AddErrorToField(formSelector, value.Field, value.Message);
        });
    }
}

function AddErrorToField(formSelector, fieldName, errorMessage) {
    $(`${formSelector} [name="${fieldName}"]`).addClass('is-invalid');

    // error message icon + tooltip show
    $(`${formSelector} [name="${fieldName}"]`).parent().parent().find('label').find('.error-tooltip-popup').remove();
    var errorIconHTML = ' <span class="error-tooltip-popup" data-bs-toggle="tooltip" data-bs-placement="top" title="' + errorMessage + '"><span class="iconify alert-icon text-danger fw-bold font-size-16 cursor-pointer" data-icon="ri:alert-line" data-inline="false"></span></span>';
    if ($(`${formSelector} [name="${fieldName}"]`).attr("type") != "hidden")
        $(`${formSelector} [name="${fieldName}"]`).parent().parent().find('label').append(errorIconHTML);
}