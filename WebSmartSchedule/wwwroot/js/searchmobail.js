function toggleSearchForm() {
    const searchForm = document.getElementById('searchForm');
    if (!searchForm) return;

    if (searchForm.classList.contains('collapsed')) {
        // Открытие формы
        searchForm.style.display = 'block';
        requestAnimationFrame(() => {
            searchForm.classList.remove('collapsed');
            searchForm.style.maxHeight = `${searchForm.scrollHeight}px`;
        });
    } else {
        // Закрытие формы
        searchForm.style.maxHeight = '0';
        searchForm.classList.add('collapsed');

        // После анимации убираем из потока документа
        const handleTransitionEnd = () => {
            searchForm.style.display = 'none';
            searchForm.removeEventListener('transitionend', handleTransitionEnd);
        };

        searchForm.addEventListener('transitionend', handleTransitionEnd);
    }
}

// При загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    const isMobile = window.innerWidth <= 768;
    const searchForm = document.getElementById('searchForm');

    if (isMobile && searchForm) {
        // Полное скрытие формы с самого начала
        searchForm.classList.add('collapsed');
        searchForm.style.maxHeight = '0';

        // Убираем из потока сразу, чтобы не было "следов"
        searchForm.style.display = 'none';
    }
});

// При изменении размера экрана
window.addEventListener('resize', function () {
    const isDesktop = window.innerWidth > 768;
    const searchForm = document.getElementById('searchForm');

    if (isDesktop && searchForm) {
        // На десктопе всегда показываем форму нормально
        searchForm.classList.remove('collapsed');
        searchForm.style.display = 'block';
        searchForm.style.maxHeight = null;
    } else if (!isDesktop && searchForm) {
        if (!searchForm.classList.contains('collapsed')) {
            // Если форма открыта, обновляем высоту
            searchForm.style.display = 'block';
            searchForm.style.maxHeight = `${searchForm.scrollHeight}px`;
        } else {
            // Скрываем форму
            searchForm.style.maxHeight = '0';
            searchForm.style.display = 'none';
        }
    }
});