/*--------------------------------------------------------------
            SAWN
--------------------------------------------------------------*/
function showError(msg) {
    swal({
        title: "Error",
        text: msg,
        confirmButtonColor: "#2196F3",
        type: "error"
    });
}
function showSuccess(msg) {
    swal({
        title: "Success",
        text: msg,
        confirmButtonColor: "#66BB6A",
        type: "success"
    });
}
function showInfo(msg) {
    swal({
        title: "Information",
        text: msg,
        confirmButtonColor: "#66BB6A",
        type: "info"
    });
}