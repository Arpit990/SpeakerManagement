$(document).ready(function () {
    $("#LoginForm").validate({
        rules: {
            email: {
                required: true,
                email: true
            },
            password: {
                required: true
            }
        },
        messages: {
            email: {
                required: "Please enter email",
                email: "Please enter a valid email"
            },
            password: "Please enter password"
        }
    });
})