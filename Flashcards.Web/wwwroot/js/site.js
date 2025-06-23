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

            if (url.includes("UpdateAvatar")) {
                setupClearableInputs(document.getElementById("form-modal"));
            }
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                toastr.warning(window.localization.loggedOutMessage);

                // Ждём 2 секунды, чтобы тостер успел показаться
                setTimeout(function () {
                    window.location.href = "/Account/Account/Login";
                }, 2000);
            }
        }
    })
}

popupPost = (form) => {
    $.ajax({
        url: form.attr("action"),
        type: form.attr("method"),
        data: form.serialize(),
        success: function (res) {
            if (res.success) {
                $("#form-modal").modal('hide');
                location.reload();
            } else {
                $("#form-modal .modal-body").html(res);
                $.validator.unobtrusive.parse(document);
            }
        }
    });
}

//Clear input button
function setupClearableInputs(container) {
    container.querySelectorAll('.clearable-input').forEach(function (input) {
        const clearBtn = input.closest('.position-relative')?.querySelector('.btn-clear');
        if (!clearBtn) return;

        function toggleClearButton() {
            clearBtn.style.display = input.value.trim() !== '' ? 'block' : 'none';
        }

        toggleClearButton();

        input.addEventListener('input', toggleClearButton);
        clearBtn.addEventListener('click', function () {
            input.value = '';
            toggleClearButton();
            input.focus();
        });
    });
}