document.addEventListener("DOMContentLoaded", function () {
    console.log("функция активирована");
    const toggleButton = document.getElementById("toggleFiltersBtn");
    const filterForms = document.querySelectorAll(".filter-form");

    if (!toggleButton || filterForms.length === 0) return;

    toggleButton.addEventListener("click", function () {
        let isVisible = false;

       
        filterForms.forEach(form => {
            if (window.getComputedStyle(form).display !== "none") {
                isVisible = true;
            }
        });

        // Переключаем видимость
        filterForms.forEach(form => {
            form.style.display = isVisible ? "none" : "block";
        });

        // Меняем текст кнопки
        toggleButton.innerHTML = isVisible ? '🔍 Показать фильтры' : '❌ Скрыть фильтры';
    });
});