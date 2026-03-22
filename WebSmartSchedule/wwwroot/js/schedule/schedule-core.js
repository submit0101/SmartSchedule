// wwwroot/js/schedule/schedule-core.js

function allowDrop(e) {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
}

async function loadSchedule(groupId) {
    if (isLoading || !window.appData) return;
    try {
        isLoading = true;
        currentScheduleType = 'group';
        currentTeacherId = null;

        const desktopContainer = document.getElementById("schedule-desktop");
        const mobileContainer = document.getElementById("schedule-mobile");
        const isMobile = window.innerWidth <= 768;
        const container = isMobile ? mobileContainer : desktopContainer;
        if (!container) return;

        const loadingEl = document.getElementById("loading");
        if (loadingEl) loadingEl.style.display = "block";

        const currentCard = container.querySelector('.schedule-card');
        if (currentCard) {
            currentCard.classList.add('schedule-fade-out');
            await new Promise(r => setTimeout(r, 300));
            currentCard.remove();
        }

        const newCard = document.createElement('div');
        newCard.className = 'schedule-card schedule-fade-in';
        container.appendChild(newCard);
        const data = await ScheduleAPI.getGroupSchedule(groupId);
        ScheduleStore.init(data);

        if (loadingEl) loadingEl.style.display = "none";

        const schedule = data.schedule;
        const days = ["Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота"];
        const timeSlots = window.appData.timeSlots || [];

        const gridElement = document.createElement("div");
        gridElement.className = "schedule-grid";
        gridElement.id = "dynamic-schedule-grid";

        const timeHeader = document.createElement("div");
        timeHeader.className = "time-header";
        timeHeader.textContent = "Время";
        gridElement.appendChild(timeHeader);

        days.forEach(day => {
            const dayHeader = document.createElement("div");
            dayHeader.className = "day-header-grid";
            dayHeader.textContent = day;
            gridElement.appendChild(dayHeader);
        });

        timeSlots.forEach(timeSlot => {
            const timeCell = document.createElement("div");
            timeCell.className = "time-header";
            timeCell.innerHTML = `${formatTime(timeSlot.startTime)}<br>${formatTime(timeSlot.endTime)}`;
            gridElement.appendChild(timeCell);

            for (let day = 1; day <= 6; day++) {
                const dayData = schedule[day] || [];
                const slot = createLessonSlot(timeSlot, day, dayData);
                gridElement.appendChild(slot);
            }
        });

        newCard.innerHTML = '';
        newCard.appendChild(gridElement);
        setTimeout(() => newCard.classList.add('animate'), 10);
        updateTimeSlotFilterPanel();

    } catch (err) {
        console.error("Ошибка загрузки расписания:", err);
        showAlert("Не удалось загрузить расписание", "danger");
    } finally {
        isLoading = false;
        const loadingEl = document.getElementById("loading");
        if (loadingEl) loadingEl.style.display = "none";
    }
}

function createLessonSlot(timeSlot, day, daySchedule) {
    const slot = document.createElement("div");
    slot.className = "lesson-slot";
    slot.dataset.timeSlotId = timeSlot.id;
    slot.dataset.dayOfWeekId = day;


    const initialLessons = ScheduleStore.getLessonsInSlot(day, timeSlot.id); //

    slot.addEventListener('dragover', handleDragOver);
    slot.addEventListener('dragenter', handleDragEnter);
    slot.addEventListener('dragleave', handleDragLeave);
    slot.addEventListener('drop', handleDrop);

    slot.addEventListener('click', (event) => {
       
        if (LessonClipboard.data) {
            event.stopPropagation();
            LessonClipboard.pasteToSlot(slot, day, timeSlot.id); 
            return;
        }

        if (event.target.closest('.lesson-item')) return;
        const currentLessons = ScheduleStore.getLessonsInSlot(day, timeSlot.id); 
        const state = analyzeSlotState(currentLessons); 

        if (state.isFull) {
            showAlert('Ячейка занята', 'warning');
            return;
        }

        if (typeof pendingPasteSlot !== 'undefined' && pendingPasteSlot) pendingPasteSlot.classList.remove('pending-paste');
        pendingPasteSlot = slot;
        slot.classList.add('pending-paste');

       
        openCreateLessonModalForSlot(timeSlot.id, day, currentLessons); 
    });

    
    renderLessonsInSlot(slot, initialLessons, currentScheduleType); 
    return slot;
}

function renderLessonsInSlot(slot, lessons, scheduleType = 'group') {
    const filtered = selectedWeekType === "all" ?
        lessons : lessons.filter(l => l.weekTypeId == selectedWeekType);

    slot.innerHTML = '';
    if (filtered.length === 0) {
        slot.classList.add("empty");
        slot.textContent = "Нет занятия";
        return;
    }

    slot.classList.remove("empty");

    // Сортировка: Числитель (1), Знаменатель (2), Полная (3)
    const sortedLessons = [...filtered].sort((a, b) => {
        const order = { [WeekType.NUMERATOR]: 1, [WeekType.DENOMINATOR]: 2, [WeekType.FULL]: 3 };
        return (order[a.weekTypeId] || 99) - (order[b.weekTypeId] || 99);
    });

    // === НОВАЯ ЛОГИКА ГРУППИРОВКИ ПОДГРУПП ===
    const groupedLessons = {};

    sortedLessons.forEach(lesson => {
        let subject = lesson.subjectTitle || 'Без названия';

        if (!groupedLessons[subject]) {
            groupedLessons[subject] = {
                baseLesson: lesson, // Сохраняем первый урок для цвета бейджа и ID
                ids: [],
                teachers: [],
                cabinets: []
            };
        }

        groupedLessons[subject].ids.push(lesson.id);

        // Собираем учителей
        let teacher = lesson.teacherFullName;
        if (teacher && teacher !== 'Не назначен' && !groupedLessons[subject].teachers.includes(teacher)) {
            groupedLessons[subject].teachers.push(teacher);
        }

        // Собираем кабинеты (вместе со зданием)
        let cabText = lesson.cabinetNumber ? `Каб. ${lesson.cabinetNumber}` : '';
        if (lesson.buildingName) cabText += ` (${lesson.buildingName})`;
        if (cabText && !groupedLessons[subject].cabinets.includes(cabText)) {
            groupedLessons[subject].cabinets.push(cabText);
        }
    });

    // === ОТРИСОВКА СГРУППИРОВАННЫХ КАРТОЧЕК ===
    for (const subject in groupedLessons) {
        const group = groupedLessons[subject];
        // Передаем теперь всю группу в функцию создания элемента
        const item = createLessonCardElement(group, subject, scheduleType, slot);
        slot.appendChild(item);
    }
}

function createLessonCardElement(group, subjectTitle, scheduleType, parentSlot) {
    const item = document.createElement("div");
    const lesson = group.baseLesson; // Берем свойства от первой подгруппы (например, тип недели)

    item.className = `lesson-item weektype-${lesson.weekTypeId}`;
    item.draggable = true;

    // Сохраняем все id для истории и первый id для совместимости с drag-and-drop
    item.dataset.lessonIds = group.ids.join(',');
    item.dataset.lessonId = group.ids[0];

    // Логика цветов бейджей (осталась ваша)
    let badgeType = '?';
    let badgeColor = 'secondary';
    if (lesson.weekTypeId == WeekType.NUMERATOR) {
        badgeType = 'Ч'; badgeColor = 'success';
    } else if (lesson.weekTypeId == WeekType.DENOMINATOR) {
        badgeType = 'З'; badgeColor = 'primary';
    } else if (lesson.weekTypeId == WeekType.FULL) {
        badgeType = 'Ц'; badgeColor = 'danger';
    }

    const badgeClass = `lesson-slot-badge bg-${badgeColor}`;

    // Склеиваем массивы через запятую
    const teachersStr = group.teachers.join(', ') || 'Не назначен';
    const cabinetsStr = group.cabinets.join(', ') || 'Каб. ?';

    let itemContent = `
        <span class="${badgeClass}">${badgeType}</span>
        <strong>${subjectTitle}</strong>
    `;

    if (scheduleType === 'teacher') {
        itemContent += `
            <small>Гр: ${lesson.groupName || '?'}</small><br/>
            <small>${cabinetsStr}</small>
        `;
    } else {
        itemContent += `
            <small>${teachersStr}</small><br/>
            <small>${cabinetsStr}</small>
        `;
    }

    item.innerHTML = itemContent;

    // Подключаем ваши события Drag & Drop
    item.addEventListener('dragstart', handleDragStart);
    item.addEventListener('dragend', handleDragEnd);

    // Подключаем клик для модального окна/всплывающих кнопок
    item.onclick = (e) => {
        e.stopPropagation();
        if (LessonClipboard.data) return;
        // Передаем базовый урок (первую подгруппу), чтобы открывалось модальное окно
        showLessonActionTooltip(e, item, lesson, parentSlot);
    };

    return item;
}

function showLessonActionTooltip(e, itemElement, lessonData, parentSlot) {
    document.querySelectorAll('.lesson-action-tooltip').forEach(el => el.remove());

    const tooltip = document.createElement('div');
    tooltip.className = 'lesson-action-tooltip';

    const rect = itemElement.getBoundingClientRect();
    tooltip.style.left = (rect.right + window.scrollX - 100) + 'px';
    tooltip.style.top = (rect.top + window.scrollY - 40) + 'px';

    tooltip.innerHTML = `
        <button title="Копировать" class="copy-lesson-btn"><i class="bi bi-clipboard"></i></button>
        <button title="Изменить" class="edit-lesson-btn"><i class="bi bi-pencil"></i></button>
        <button title="Удалить" class="delete-lesson-btn"><i class="bi bi-trash"></i></button>
    `;
    document.body.appendChild(tooltip);

    tooltip.querySelector('.copy-lesson-btn').onclick = (ev) => {
        ev.stopPropagation();
        LessonClipboard.copy(lessonData);
        tooltip.remove();
    };

    tooltip.querySelector('.edit-lesson-btn').onclick = (ev) => {
        ev.stopPropagation();
        tooltip.remove();
        window.currentSelectedLessonData = lessonData;
        openEditLessonModal(lessonData);
    };

    tooltip.querySelector('.delete-lesson-btn').onclick = (ev) => {
        ev.stopPropagation();
        tooltip.remove();
        deleteLessonUniversal(lessonData.id, parentSlot);
    };

    setTimeout(() => { if (document.body.contains(tooltip)) tooltip.remove(); }, 3000);
}

// Универсальное удаление
async function deleteLessonUniversal(lessonId, slotElement) {
    if (!confirm('Удалить занятие?')) return;
    try {
        await ScheduleAPI.deleteLesson(lessonId);
        ScheduleStore.removeLesson(lessonId);
        showAlert('Занятие успешно удалено!', 'success');

        if (slotElement) {
            const dayId = slotElement.dataset.dayOfWeekId;
            const timeId = slotElement.dataset.timeSlotId;
            const remainingLessons = ScheduleStore.getLessonsInSlot(dayId, timeId);
            renderLessonsInSlot(slotElement, remainingLessons, currentScheduleType);
        } else {
            loadSchedule(window.appData.groupId);
        }
    } catch (err) {
        showAlert(err.message, 'danger');
    }
}

// Фильтры и поиск
function applyWeekTypeFilter() {
    if (!window.scheduleData) return;
    document.querySelectorAll('.lesson-slot').forEach(slot => {
        const dayId = parseInt(slot.dataset.dayOfWeekId);
        const timeId = parseInt(slot.dataset.timeSlotId);
        const lessons = ScheduleStore.getLessonsInSlot(dayId, timeId);
        renderLessonsInSlot(slot, lessons, currentScheduleType);
    });
    applyTimeSlotFilter();
}

function applyTimeSlotFilter() {
    const grid = document.getElementById('dynamic-schedule-grid');
    if (!grid || !window.appData?.timeSlots) return;
    for (let i = 0; i < grid.children.length; i++) {
        const el = grid.children[i];
        if (i < 7) {
            el.style.display = '';
        } else {
            const slotIndex = Math.floor((i - 7) / 7);
            const timeSlotId = window.appData.timeSlots[slotIndex]?.id;
            const show = selectedTimeSlotFilter === null || timeSlotId === selectedTimeSlotFilter;
            el.style.display = show ? '' : 'none';
        }
    }
}

function performSearch() {
    const query = document.getElementById('scheduleSearch')?.value.trim().toLowerCase() || '';
    const slots = document.querySelectorAll('.lesson-slot');
    const items = document.querySelectorAll('.lesson-item');
    slots.forEach(slot => slot.classList.remove('search-match', 'search-focus'));
    items.forEach(item => item.classList.remove('search-highlight'));
    if (query === '') return;
    let firstMatch = null;
    items.forEach(item => {
        if (item.textContent.toLowerCase().includes(query)) {
            item.classList.add('search-highlight');
            const slot = item.closest('.lesson-slot');
            if (slot) {
                slot.classList.add('search-match');
                if (!firstMatch) firstMatch = slot;
            }
        }
    });
    if (firstMatch) {
        const audio = document.getElementById('spotSound');
        if (audio) {
            audio.currentTime = 0;
            audio.play().catch(e => console.warn("Звук заблокирован:", e));
        }
        firstMatch.scrollIntoView({ behavior: 'smooth', block: 'center' });
        firstMatch.classList.add('search-focus');
        setTimeout(() => firstMatch.classList.remove('search-focus'), 1200);
    } else {
        showAlert("Ничего не найдено", "warning");
    }
}

function updateTimeSlotFilterPanel() {
    const container = document.getElementById('timeSlotFilterContent');
    if (!container || !window.scheduleData || !window.appData?.timeSlots) return;
    const timeToIdMap = {};
    window.appData.timeSlots.forEach(ts => {
        const key = `${ts.startTime.slice(0, 5)}-${ts.endTime.slice(0, 5)}`;
        timeToIdMap[key] = ts.id;
    });
    function normalizeTimeDisplay(display) {
        if (!display) return null;
        const parts = display.split(' - ');
        if (parts.length !== 2) return null;
        return `${parts[0].padStart(5, '0')}-${parts[1].padStart(5, '0')}`;
    }
    const usedTimeSlotIds = new Set();
    const schedule = window.scheduleData.schedule || {};
    for (let day = 1; day <= 6; day++) {
        const daySlots = schedule[day] || [];
        daySlots.forEach(slot => {
            if (slot.lessons?.length > 0) {
                const key = normalizeTimeDisplay(slot.timeSlotDisplay);
                const timeSlotId = timeToIdMap[key];
                if (timeSlotId) usedTimeSlotIds.add(timeSlotId);
            }
        });
    }
    const timeSlots = window.appData.timeSlots
        .filter(ts => usedTimeSlotIds.has(ts.id))
        .sort((a, b) => a.id - b.id);
    container.innerHTML = '';
    if (timeSlots.length === 0) {
        const noSlots = document.createElement('div');
        noSlots.style.textAlign = 'center';
        noSlots.style.padding = '8px';
        noSlots.style.color = '#94a3b8';
        noSlots.style.fontSize = '0.75rem';
        noSlots.innerHTML = '<i class="bi bi-calendar-x me-1"></i> Нет занятий';
        container.appendChild(noSlots);
        return;
    }
    const allBtn = document.createElement('button');
    allBtn.className = selectedTimeSlotFilter === null ? 'pair-btn active' : 'pair-btn';
    allBtn.textContent = 'Все';
    allBtn.onclick = () => {
        selectedTimeSlotFilter = null;
        updateFilterButtons();
        applyTimeSlotFilter();
    };
    container.appendChild(allBtn);
    timeSlots.forEach(ts => {
        const btn = document.createElement('button');
        btn.dataset.timeId = ts.id;
        btn.textContent = `${ts.startTime.slice(0, 5)}–${ts.endTime.slice(0, 5)}`;
        btn.className = selectedTimeSlotFilter === ts.id ? 'pair-btn active' : 'pair-btn';
        btn.onclick = () => {
            selectedTimeSlotFilter = ts.id;
            updateFilterButtons();
            applyTimeSlotFilter();
        };
        container.appendChild(btn);
    });
}

function updateFilterButtons() {
    const container = document.getElementById('timeSlotFilterContent');
    if (!container) return;
    container.querySelectorAll('.pair-btn').forEach(btn => {
        const isAll = btn.textContent === 'Все';
        const timeId = btn.dataset.timeId ? parseInt(btn.dataset.timeId) : null;
        btn.classList.toggle('active',
            (isAll && selectedTimeSlotFilter === null) || timeId === selectedTimeSlotFilter
        );
    });
}