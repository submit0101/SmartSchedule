
document.addEventListener('DOMContentLoaded', function () {
    function setupFormValidation(form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();

                // Убираем старые tooltip'ы
                const existingTooltips = document.querySelectorAll('.invalid-tooltip');
                existingTooltips.forEach(el => el.remove());

                // Проставляем title и стилизуем поля
                const invalidFields = form.querySelectorAll(':invalid');
                invalidFields.forEach(field => {
                    const message = field.validationMessage || 'Это поле обязательно';
                    field.setAttribute('title', message);
                    field.classList.add('is-invalid');

                    // Активируем Bootstrap Tooltip
                    new bootstrap.Tooltip(field, {
                        placement: 'top',
                        trigger: 'manual'
                    });
                    field.tooltip = new bootstrap.Tooltip(field);
                    field.tooltip.show();
                });

                // Анимация модального окна
                const modalContent = form.closest('.modal-content');
                if (modalContent) {
                    modalContent.style.animation = 'none';
                    void modalContent.offsetWidth;
                    modalContent.style.animation = 'flash 0.6s ease-in-out';
                }

                // Фокус на первое невалидное поле
                const firstInvalid = invalidFields[0];
                if (firstInvalid) firstInvalid.focus();

                return false;
            }
        });

        // Скрываем tooltip при вводе
        form.addEventListener('input', function (e) {
            const field = e.target;
            if (field.matches(':valid')) {
                field.classList.remove('is-invalid');
                if (field.tooltip) {
                    field.tooltip.dispose();
                    field.removeAttribute('title');
                }
            }
        });
    }

    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(setupFormValidation);
});
