/*Скрипт, который запрещает модалкам или bootbox добавлять padding под scrollbar, если scrollbar и так уже активен.Это создавало лишнний отступ*/
(function forceRemoveModalPadding() {
    const resetPadding = () => {
        const modal = document.querySelector('#form-modal');
        document.body.style.paddingRight = '';
        if (modal) modal.style.paddingRight = '';
    };

    // Создаем наблюдатель за изменением стилей body
    const observer = new MutationObserver(() => {
        const padding = document.body.style.paddingRight;
        if (padding && padding !== '0px') {
            resetPadding();
        }
    });

    observer.observe(document.body, { attributes: true, attributeFilter: ['style'] });

    // Также сбрасываем при показе модального окна
    $(document).on('shown.bs.modal', function () {
        setTimeout(resetPadding, 0);
    });
})();