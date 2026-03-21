document.addEventListener('DOMContentLoaded', function () {
    function setupFormValidation(form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
                form.classList.add('was-validated');
                const modalContent = form.closest('.modal-content');
                if (modalContent) {
                    modalContent.style.animation = 'none';
                    void modalContent.offsetWidth;
                    modalContent.style.animation = 'flash 0.6s ease-in-out';
                }
                const firstInvalid = form.querySelector(':invalid');
                if (firstInvalid) firstInvalid.focus();
            }
        });
    }
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(setupFormValidation);
});