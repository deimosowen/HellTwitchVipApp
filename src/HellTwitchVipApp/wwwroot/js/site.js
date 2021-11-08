$(function () {
    const url = new URL(window.location.href);
    const status = url.searchParams.get('status');
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toastr-top-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "4500",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    if (status === 'Success') {
        toastr.success("Изменения сохранены", "Готово!");
    }
    if (status === 'Error') {
        toastr.error("Произошла ошибка. Повторите попытку позже", "Ошибка!");
    }
    url.searchParams.delete('status');
    history.pushState(null, document.title, url);
});
var createModal = function (data) {
    if (document.getElementById(data.id) || !data.content)
        return false;
    var divModal = document.createElement("div");
    divModal.setAttribute("id", data.id);
    divModal.setAttribute("class", "modal fade");
    divModal.setAttribute("style", "display: none");
    divModal.setAttribute("data-bs-keyboard", "false");
    divModal.setAttribute("tabindex", "-1");
    divModal.innerHTML = data.content;
    if (data.backdrop) divModal.setAttribute("data-bs-backdrop", "static");
    $("body").append(divModal);
    $(divModal).modal("show");
    $(divModal).on("hidden.bs.modal", function () {
        divModal.remove();
    });
    return divModal;
};
$(document).on('show.bs.modal', '.modal', function () {
    var zIndex = 1040 + (20 * $('.modal:visible').length);
    $(this).css('z-index', zIndex);
    setTimeout(function () {
        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    }, 0);
});
$(document).on('click', '[data-toggle="modal"]',
    function (e) {
        e.preventDefault();
        e.stopPropagation();
        $.ajax({
            url: $(this).attr('href'),
            success: (data) => {
                createModal({
                    content: data,
                    id: $(this).data('modal-id') || 'modal1',
                    backdrop: $(this).data('modal-backdrop') || false
                });
            },
            error: (data, textStatus) => {
            }
        });
    });
$(document).on("click", "#kt_gift_information",
    function (e) {
        e.preventDefault();
        e.stopPropagation();
        Swal.fire({
            html: "Для доступа в VipLand необходимо подарить <strong>15 сабок</strong></br><div class='d-flex align-items-center flex-column mt-2'>" +
                "<li class='d-flex align-items-center py-1'><span class='bullet bullet-dot bg-info me-1'></span> Tier1 - 1 сабка</li>" +
                "<li class='d-flex align-items-center py-1'><span class='bullet bullet-dot bg-info me-1'></span> Tier2 - 2 сабки</li>" +
                "<li class='d-flex align-items-center py-1'><span class='bullet bullet-dot bg-info me-1'></span> Tier3 - 5 сабок</li></div>",
            icon: "info",
            buttonsStyling: false,
            confirmButtonText: "Все понятно",
            customClass: {
                confirmButton: "btn btn-info"
            }
        });
    });