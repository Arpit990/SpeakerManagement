const sweetAlertConfigs = [
    {
        action: "create",
        title: "Created!",
        text: "Data saved successfully",
        icon: "success"
    },
    {
        action: "update",
        title: "Updated!",
        text: "Data updated successfully",
        icon: "success"
    },
    {
        action: "delete",
        title: "Deleted!",
        text: "Data deleted successfully",
        icon: "success"
    }
];


// Function to display SweetAlert based on action type
function showAlert(data) {
    if (data) {
        let msg = data.message;
        let icon = data.isSuccess ? "success" : "error";

        Swal.fire({
            title: msg,
            icon: icon
        });
    }
    else {
        console.error(`SweetAlert configuration not found for action`);
    }
}


function DeleteConformation({ confirmCallback }) {
    Swal.fire({
        title: "Are you sure want to delete this record?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            // User confirmed the delete action, execute the confirmCallback function
            if (typeof confirmCallback === 'function') {
                confirmCallback();
            }
        }
    });
}