
function GlobalAjax({
    data,
    url,
    type,
    isMultipartFormData = false,
    successCallback,
    errorCallback
}) {

    // Make an AJAX POST request
    $.ajax({
        url: url,
        type: type,
        data: data,
        contentType: isMultipartFormData ? false : undefined,
        processData: isMultipartFormData ? false : undefined,
        success: function (response) {
            if (response.responseJSON.responseCode == "Success") {

                //remove error treatments
                /*$(formSelector).find('input, select, textarea').each(function () {
                    $(this).removeClass('is-invalid');
                    // error message in tooltip over the icon
                    $(this).parent().parent().find('label').find('.error-tooltip-popup').remove();
                });*/

                // Invoke success callback if provided
                if (successCallback && typeof successCallback === 'function') {
                    successCallback(response);
                }

            }
        },
        error: function (xhr, status, error) {
            if (response.responseJSON.responseCode == "Error") {

                DisplayErrorsOnForm(formSelector, response.responseJSON.Errors);

                var infoText = `<div class="alert alert-danger alert-dismissible fade show" role="alert"> <span class="iconify alert-icon" data-icon="ri:alert-line" data-inline="false"></span> ${response.ResponseJSON.Message}.<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button> </div>`;
                $('#LERETAGlobalFormSaveSlideUpBar .slideupbar-notification').prepend(infoText);

                // Invoke error callback if provided
                if (errorCallback && typeof errorCallback === 'function') {
                    errorCallback(xhr, status, error);
                }

            }
        }
    });
}


function BindFormData(data, formElement) {
    // Iterate over each property in the data object
    $.each(data, function (key, value) {
        // Find the form field with the corresponding name attribute
        var fieldName = key.toCapitalize();
        var field = $(formElement).find('[name="' + fieldName + '"]');

        // Check if the field exists
        if (field.length > 0) {
            // Determine the field type (input, select, textarea, etc.) and set the value accordingly
            var fieldType = field.prop('type');

            if (field.is('input[type="checkbox"]') || field.is('input[type="radio"]')) {
                // For checkbox and radio inputs, set checked state based on value
                field.prop('checked', value);
            } else if (fieldType === 'select-multiple') {
                // For multi-select dropdowns, set selected options based on array of values
                if ($.isArray(value)) {
                    field.val(value);
                }
            } else {
                // For other input types (text, textarea, select, etc.), set the value directly
                field.val(value);
            }
        }
    });
}


function SerializeFormData(formElement) {
    // Serialize form data into a JS object
    /*var formData = $(formElement).serializeArray().reduce(function (obj, item) {
        obj[item.name] = item.value;
        return obj;
    }, {});

    return formData;*/

    var dataString = 'submit=submit';

    // Serialize non-file input fields
    if ($(formElement).find('input[type!="file"]').length > 0) {
        dataString += '&' + $(formElement).find('input[type!="file"][type!="checkbox"]').serialize();
    }

    // Serialize file input field
    var fileInput = $(formElement).find('input[type="file"]')[0];
    if (fileInput && fileInput.files.length > 0) {
        var file = fileInput.files[0];
        var elementName = $(fileInput).attr('name');
        dataString += '&' + elementName + '=' + encodeURIComponent(file.name);
    }

    if ($(formElement).find('input[type!="checkbox"]').length > 0) {
        dataString += '&' + $(formElement).find('input[type!="checkbox"]').serialize();
    }

    //for all checkbox inputs
    $(formElement).find('input[type="checkbox"]').each(function () {
        var elementName = $(this).attr('name');
        if ($(this).is(':checked')) {
            dataString += '&' + elementName + '=true';
        }
        else {
            dataString += '&' + elementName + '=false';
        }
    });

    dataString += '&' + $(formElement).find('select').serialize();
    console.log(dataString);
    dataString += '&' + $(formElement).find('textarea').serialize();
    
    dataString += '&' + $(formElement).find('input[type="hidden"]').serialize();

    dataString = removeDuplicatesFromDataString(dataString);

    console.log(dataString);
    return dataString;
}

function removeDuplicatesFromDataString(dataString) {

    //split the concatenated values into a string array
    var arrayPossDups = dataString.split("&");
    var arrayUnique = [];

    //loop through, only adding unique ones into the replacement array
    $.each(arrayPossDups, function (i, el) {
        if ($.inArray(el, arrayUnique) === -1) arrayUnique.push(el);
    });

    return arrayUnique.join("&");
}

function OnModalHide(modalSelector, callback) {
    // Ensure the modalSelector is a valid jQuery selector
    if ($(modalSelector).length === 0) {
        console.error('Invalid modal selector:', modalSelector);
        return;
    }

    // Reset the form validation
    $(modalSelector).validate().resetForm();

    // Remove any added classes for highlighting
    $(modalSelector + " :input").removeClass('is-invalid is-valid');
    $(modalSelector + " textarea").removeClass('is-invalid is-valid');
    $(modalSelector + " select").removeClass('is-invalid is-valid');

    // Attach the callback function to the 'hide.bs.modal' event of the modal
    $(modalSelector).on('hide.bs.modal', function () {
        // Execute the callback function when the modal is about to be hidden
        if (typeof callback === 'function') {
            callback();
        }
    });
}

function OnModalShow(modalSelector, callback) {
    // Ensure the modalSelector is a valid jQuery selector
    if ($(modalSelector).length === 0) {
        console.error('Invalid modal selector:', modalSelector);
        return;
    }

    // Attach the callback function to the 'hide.bs.modal' event of the modal
    $(modalSelector).on('show.bs.modal', function () {
        // Execute the callback function when the modal is about to be hidden
        if (typeof callback === 'function') {
            callback();
        }
    });
}



function ResetForm(formSelector) {
    // Use jQuery to select the form element based on the provided selector
    var $form = $(formSelector);

    // Reset all input fields (including hidden fields)
    $form.find('input, textarea, select').each(function () {
        var $input = $(this);
        var type = $input.attr('type');
        var name = $input.attr('name');

        if (type === 'hidden') {
            if (name === 'Id') {
                // Reset value of hidden input with name 'Id' to '0'
                $input.val('0');
            } else {
                // Reset value of other hidden inputs to an empty string
                $input.val('');
            }
        } else {
            // Reset value based on input type
            switch (type) {
                case 'text':
                case 'textarea':
                case 'password':
                case 'number':
                case 'email':
                case 'tel':
                case 'url':
                    $input.val('');
                    break;
                case 'checkbox':
                case 'radio':
                    $input.prop('checked', false);
                    break;
                case 'select-multiple':
                    $input.find('option').prop('selected', false);
                    break;
                default:
                    // For unsupported input types, reset value to empty string
                    $input.val('');
                    break;
            }
        }
    });

    // Reset all form data and clear any form validation state
    $form.trigger('reset');
}




// Utility
String.prototype.toCapitalize = function () {
    // Check if the string is not empty
    if (this.length === 0) {
        return this; // Return the string unchanged if it's empty
    }

    // Capitalize the first character and concatenate the rest of the string
    return this.charAt(0).toUpperCase() + this.slice(1);
};