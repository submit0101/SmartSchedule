document.addEventListener("DOMContentLoaded", function () {
    // 1. Создаем независимый кастомный компонент (Lightbox) для картинок
    if (!document.getElementById('customHelpLightbox')) {
        const lightboxHtml = `
            <div id="customHelpLightbox" style="display: none; position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background-color: rgba(0, 0, 0, 0.85); z-index: 10000; justify-content: center; align-items: center; opacity: 0; transition: opacity 0.3s ease;">
                <button id="customHelpLightboxClose" style="position: absolute; top: 20px; right: 30px; background: transparent; border: none; color: white; font-size: 40px; cursor: pointer; line-height: 1; padding: 10px;">&times;</button>
                <img id="customHelpLightboxImg" src="" style="max-width: 90vw; max-height: 90vh; object-fit: contain; border-radius: 8px; box-shadow: 0 4px 25px rgba(0,0,0,0.5);">
            </div>
        `;
        document.body.insertAdjacentHTML('beforeend', lightboxHtml);

        const lightbox = document.getElementById('customHelpLightbox');
        const closeBtn = document.getElementById('customHelpLightboxClose');

        const closeLightbox = () => {
            lightbox.style.opacity = '0';
            setTimeout(() => { lightbox.style.display = 'none'; }, 300);
        };

        closeBtn.addEventListener('click', closeLightbox);
        lightbox.addEventListener('click', function (e) {
            if (e.target === lightbox) closeLightbox();
        });
        document.addEventListener('keydown', function (e) {
            if (e.key === "Escape" && lightbox.style.display === 'flex') closeLightbox();
        });
    }

    // 2. Словарь справки со всеми скриншотами
    const helpDictionary = {
        "main": {
            title: "Главная страница",
            content: `
            <div class="alert alert-info mb-3 shadow-sm">
                <i class="bi bi-info-circle me-2"></i>
                Добро пожаловать в АИС «Аудиториум»!
            </div>
            
            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig1.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Главная страница" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 1 — Интерфейс главной страницы
                </figcaption>
            </figure>

            <h6><i class="bi bi-list-ul text-primary me-2"></i>Основные элементы:</h6>
            <ul class="mb-4">
                <li class="mb-1"><strong>Боковая панель навигации</strong> — для перехода к справочникам и расписанию</li>
                <li class="mb-1"><strong>Цифровые часы</strong> — показывают текущее время и учебную неделю</li>
                <li><strong>Интерактивные карточки</strong> — для быстрого доступа к разделам</li>
            </ul>

            <h6><i class="bi bi-gear text-secondary me-2"></i>Технические требования:</h6>
            <div class="card bg-light border-0 shadow-sm">
                <div class="card-body py-3">
                    <p class="mb-0 small text-muted">
                        <i class="bi bi-browser-chrome me-2 text-primary"></i>Современный браузер (Chrome, Яндекс.Браузер, Firefox)<br>
                        <i class="bi bi-phone me-2 text-secondary mt-2"></i>Для мобильных: Android 6.0+ (доступно приложение)
                    </p>
                </div>
            </div>
        `
        },

        "dictionaries": {
            title: "Справочные данные",
            content: `
            <p class="lead mb-3" style="font-size: 1rem;">
                Для корректного составления расписания оператору необходимо заполнить справочники.
            </p>

            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig2.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Список справочников" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 2 — Интерфейс управления справочными данными
                </figcaption>
            </figure>

            <div class="card border-primary mb-4 shadow-sm">
                <div class="card-header bg-primary text-white py-2">
                    <i class="bi bi-plus-circle me-2"></i>Как добавить запись
                </div>
                <div class="card-body">
                    <ol class="mb-0">
                        <li class="mb-2">Для добавления новой записи нажмите кнопку <strong class="text-success">«Добавить»</strong>.</li>
                        <li class="mb-2">Откроется модальное окно. Заполните необходимые поля...</li>
                        <li>...и нажмите <strong>«Сохранить»</strong>.</li>
                    </ol>
                </div>
            </div>

            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig3.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Форма добавления" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 3 — Форма добавления нового кабинета
                </figcaption>
            </figure>

            <div class="card border-warning mb-4 shadow-sm">
                <div class="card-header bg-warning py-2">
                    <i class="bi bi-pencil-square me-2"></i>Редактирование и удаление
                </div>
                <div class="card-body">
                    <p class="mb-3 small">В таблице предусмотрены кнопки для редактирования и удаления существующих записей.</p>
                    
                    <figure class="figure w-100 mb-3">
                        <img src="/images/help/fig3d.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Подтверждение удаления" title="Нажмите для увеличения">
                        <figcaption class="figure-text small text-muted mt-2">
                            <i class="bi bi-image me-1"></i>Рис. 4 — Окно подтверждения удаления
                        </figcaption>
                    </figure>
                </div>
            </div>

            <hr class="my-4">

            <h6><i class="bi bi-bell text-info me-2"></i>Сообщения системы:</h6>
            
            <div class="alert alert-success py-2 mb-3 shadow-sm">
                <i class="bi bi-check-circle me-2"></i><strong>Предмет успешно добавлен!</strong><br>
                Информирует о том, что данные успешно прошли все проверки и сохранены в базе данных.
            </div>

            <div class="alert alert-danger py-2 mb-0 shadow-sm">
                <i class="bi bi-x-circle me-2"></i><strong>Объект с именем [...] уже существует</strong><br>
                Появляется при попытке добавить в справочник запись, которая уже была создана ранее.
            </div>
        `
        },

        "schedule": {
            title: "Формирование сетки расписания",
            content: `
            <p>Экран разделен на панель фильтрации (сверху) и интерактивную сетку (снизу).</p>

            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig4.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Рабочее пространство диспетчера" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 4 — Рабочее пространство диспетчера с панелью фильтрации
                </figcaption>
            </figure>

            <div class="card border-info mb-4 shadow-sm">
                <div class="card-header bg-info text-white py-2">
                    <i class="bi bi-calendar-plus me-2"></i>Добавление занятия
                </div>
                <div class="card-body">
                    <ol class="mb-0">
                        <li class="mb-2">Выберите нужные параметры в фильтрах, чтобы отобразить текущую сетку занятий.</li>
                        <li class="mb-2">Для добавления нового занятия кликните на свободную ячейку в сетке расписания.</li>
                    </ol>
                </div>
            </div>

            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig5.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Ввод параметров нового занятия" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 5 — Ввод параметров нового занятия
                </figcaption>
            </figure>

            <p>Система автоматически проверит данные, разместит занятие в сетке и сообщит в уведомлении.</p>

            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig6.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Отображение добавленного занятия" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 6 — Отображение добавленного занятия в расписании
                </figcaption>
            </figure>

            <hr class="my-4">

            <h6><i class="bi bi-pencil-square text-success me-2"></i>Управление занятием:</h6>
            <p>Для <strong>редактирования</strong> или <strong>удаления</strong> конкретной пары наведите на нее курсор, нажмите левую кнопку мыши (ЛКМ) и выберите соответствующий пункт в появившемся контекстном меню.</p>
            
            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig-context-menu.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Контекстное меню занятия" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 6.1 — Контекстное меню занятия
                </figcaption>
            </figure>

            <h6><i class="bi bi-arrows-move text-primary me-2"></i>Перенос занятий (Drag-and-Drop):</h6>
            <p>Вы можете переносить пары простым перетаскиванием. Зажмите карточку занятия мышью и наведите на другую ячейку. При наведении система подсветит ячейку цветом, указывая на ее статус:</p>
            
            <ul class="small mb-3">
                <li><span class="badge bg-success">Зеленый</span> — ячейка свободна, перенос возможен.</li>
                <li><span class="badge bg-danger">Красный</span> — конфликт: занят кабинет.</li>
                <li><span class="badge bg-warning text-dark">Оранжевый</span> — конфликт: занят преподаватель.</li>
                <li><span class="badge" style="background-color: #6f42c1;">Фиолетовый</span> — конфликт: занята группа.</li>
            </ul>

            <div id="dragDropCarousel" class="carousel slide mb-2 border rounded shadow-sm overflow-hidden" data-bs-ride="false">
                <div class="carousel-indicators">
                    <button type="button" data-bs-target="#dragDropCarousel" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1"></button>
                    <button type="button" data-bs-target="#dragDropCarousel" data-bs-slide-to="1" aria-label="Slide 2"></button>
                    <button type="button" data-bs-target="#dragDropCarousel" data-bs-slide-to="2" aria-label="Slide 3"></button>
                    <button type="button" data-bs-target="#dragDropCarousel" data-bs-slide-to="3" aria-label="Slide 4"></button>
                </div>
                <div class="carousel-inner bg-light">
                    <div class="carousel-item active">
                        <img src="/images/help/fig-dnd-green.png" class="d-block w-100 help-zoom-img" alt="Зеленая ячейка" title="Нажмите для увеличения">
                        <div class="carousel-caption d-none d-md-block bg-dark bg-opacity-75 rounded p-2 mb-2">
                            <p class="m-0 small">Зеленый — Свободно</p>
                        </div>
                    </div>
                    <div class="carousel-item">
                        <img src="/images/help/fig-dnd-red.png" class="d-block w-100 help-zoom-img" alt="Красная ячейка" title="Нажмите для увеличения">
                        <div class="carousel-caption d-none d-md-block bg-dark bg-opacity-75 rounded p-2 mb-2">
                            <p class="m-0 small">Красный — Занят кабинет</p>
                        </div>
                    </div>
                    <div class="carousel-item">
                        <img src="/images/help/fig-dnd-orange.png" class="d-block w-100 help-zoom-img" alt="Оранжевая ячейка" title="Нажмите для увеличения">
                        <div class="carousel-caption d-none d-md-block bg-dark bg-opacity-75 rounded p-2 mb-2">
                            <p class="m-0 small">Оранжевый — Занят учитель</p>
                        </div>
                    </div>
                    <div class="carousel-item">
                        <img src="/images/help/fig-dnd-purple.png" class="d-block w-100 help-zoom-img" alt="Фиолетовая ячейка" title="Нажмите для увеличения">
                        <div class="carousel-caption d-none d-md-block bg-dark bg-opacity-75 rounded p-2 mb-2">
                            <p class="m-0 small">Фиолетовый — Занята группа</p>
                        </div>
                    </div>
                </div>
                <button class="carousel-control-prev" type="button" data-bs-target="#dragDropCarousel" data-bs-slide="prev">
                    <span class="carousel-control-prev-icon bg-dark rounded-circle p-2" aria-hidden="true"></span>
                    <span class="visually-hidden">Предыдущий</span>
                </button>
                <button class="carousel-control-next" type="button" data-bs-target="#dragDropCarousel" data-bs-slide="next">
                    <span class="carousel-control-next-icon bg-dark rounded-circle p-2" aria-hidden="true"></span>
                    <span class="visually-hidden">Следующий</span>
                </button>
            </div>
            <figcaption class="figure-text text-center small text-muted mb-4">
                <i class="bi bi-image me-1"></i>Рис. 6.2 — Цветовая индикация (Листайте слайдер)
            </figcaption>
            <hr class="my-4">

            <h6><i class="bi bi-graph-up text-primary me-2"></i>Аналитика и загруженность:</h6>
            <p>Для анализа использования кабинетов перейдите во вкладку «Загруженность». Система сформирует таблицу с процентом загрузки каждой аудитории и выведет график ТОП-5 самых загруженных аудиторий.</p>

            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig7.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Интерфейс статистики загруженности" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 7 — Интерфейс статистики загруженности
                </figcaption>
            </figure>

            <p>Для детального просмотра расписания конкретного кабинета перейдите на вкладку «Занятость (Тепловая карта)». Красные ячейки обозначают занятое время, зеленые — свободное.</p>

            <figure class="figure w-100 mb-4">
                <img src="/images/help/fig8.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Тепловая карта" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 8 — Тепловая карта занятости кабинета
                </figcaption>
            </figure>

            <hr class="my-4">

            <h6><i class="bi bi-exclamation-triangle text-danger me-2"></i>Сообщения оператору:</h6>
            
            <div class="alert alert-danger mb-3 shadow-sm">
                <i class="bi bi-shield-exclamation me-2"></i>
                <strong>Отмена переноса: Занята группа, преподаватель, кабинет.</strong><br>
                Возникает при попытке диспетчера поставить пару в ячейку, где выбранный ресурс (аудитория или преподаватель) уже занят другим занятием.
            </div>

            <figure class="figure w-100 mb-3">
                <img src="/images/help/fig11.png" class="figure-img img-fluid border rounded shadow-sm help-zoom-img" alt="Сообщение о конфликте" title="Нажмите для увеличения">
                <figcaption class="figure-text small text-muted mt-2">
                    <i class="bi bi-image me-1"></i>Рис. 9 — Сообщение о конфликте в расписании
                </figcaption>
            </figure>

            <p class="small text-muted mb-0"><strong>Действия оператора:</strong> Закрыть сообщение, проверить занятость преподавателя или кабинета через вкладки аналитики и выбрать для занятия другой свободный временной слот или другой свободный кабинет.</p>
        `
        }
    };

    // Функция определения текущего раздела по URL
    function getHelpData() {
        const path = window.location.pathname.toLowerCase();
        if (path === "/" || path === "/index" || path === "") return helpDictionary["main"];
        if (path.includes("/lessons")) return helpDictionary["schedule"];
        return helpDictionary["dictionaries"];
    }

    // Подключение к Offcanvas
    const helpOffcanvasElement = document.getElementById('contextHelpOffcanvas');
    if (helpOffcanvasElement) {
        helpOffcanvasElement.addEventListener('show.bs.offcanvas', function () {
            const data = getHelpData();
            document.getElementById('helpSectionTitle').innerText = data.title;
            const bodyContent = document.getElementById('helpSectionBody');
            bodyContent.innerHTML = data.content;

            // Логика кастомного Lightbox для ВСЕХ картинок (включая те, что в слайдере)
            const images = bodyContent.querySelectorAll('.help-zoom-img');
            const lightbox = document.getElementById('customHelpLightbox');
            const lightboxImg = document.getElementById('customHelpLightboxImg');

            images.forEach(img => {
                img.style.cursor = 'zoom-in';
                img.addEventListener('click', function () {
                    lightboxImg.src = this.src;
                    lightbox.style.display = 'flex';
                    setTimeout(() => { lightbox.style.opacity = '1'; }, 10);
                });
            });

            // Инициализация слайдера Bootstrap, если он есть на странице
            const carouselElement = document.getElementById('dragDropCarousel');
            if (carouselElement && typeof bootstrap !== 'undefined') {
                new bootstrap.Carousel(carouselElement, {
                    interval: false // отключаем автоперелистывание, чтобы пользователь мог читать спокойно
                });
            }
        });
    }
});