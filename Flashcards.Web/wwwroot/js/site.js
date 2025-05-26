// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

showInPopup = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal .modal-body").html(res);
            $("#form-modal .modal-title").html(title);
            $("#form-modal").modal('show');
        }
    })
}

addToFav = (url) => {
    $.ajax({
        type: "POST",
        url: url,
        success: function (res) {
            if (res.isFavorite) {
                $("#BtnFav").on("click", function () {
                    $(this).css("background-color", "yellow");
                });
            }
            else {
                $("#BtnFav").on("click", function () {
                    $(this).css("background-color", "transparent");
                });
            }
        }
    })
}