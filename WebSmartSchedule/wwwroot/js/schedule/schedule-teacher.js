// wwwroot/js/schedule/schedule-teacher.js

// ====================================================================
// ФУНКЦИИ ДЛЯ РАСПИСАНИЯ ПРЕПОДАВАТЕЛЯ
// ====================================================================

const comparisonCache = {};
let allTeachers = [];
let currentTeacherGroupFilter = 'all';

/**
 * Основная функция: Загрузка расписания преподавателя
 */
async function loadTeacherSchedule(teacherId) {
    if (isLoading) return;
    if (!window.appData) {
        showAlert("Данные приложения не загружены.", "danger");
        return;
    }

    try {
        isLoading = true;
        currentScheduleType = 'teacher';
        currentTeacherId = teacherId;
        currentTeacherGroupFilter = 'all';

        // === БЛОКИРУЕМ ФИЛЬТРЫ НА ВРЕМЯ ЗАГРУЗКИ ===
        const groupSelect = document.getElementById('teacherGroupsSelect');
        const filterBtn = document.getElementById('applyTeacherGroupFilterBtn');
        if (groupSelect) { groupSelect.disabled = true; groupSelect.value = 'all'; } // Сброс и блок
        if (filterBtn) filterBtn.disabled = true;
        // ============================================

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

        // 1. ЗАГРУЗКА
        const data = await ScheduleAPI.getTeacherSchedule(teacherId);

        // 2. STORE
        ScheduleStore.init(data);

        // 3. ЗАПОЛНЕНИЕ СПИСКА ГРУПП (Здесь же и разблокировка)
        populateTeacherGroupSelect();

        if (loadingEl) loadingEl.style.display = "none";

        // 4. СЕТКА
        const gridElement = document.createElement("div");
        gridElement.className = "schedule-grid";
        gridElement.id = "dynamic-schedule-grid";

        const timeHeader = document.createElement("div");
        timeHeader.className = "time-header";
        timeHeader.textContent = "Время";
        gridElement.appendChild(timeHeader);

        const days = ["Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота"];
        days.forEach(day => {
            const dayHeader = document.createElement("div");
            dayHeader.className = "day-header-grid";
            dayHeader.textContent = day;
            gridElement.appendChild(dayHeader);
        });

        const timeSlots = window.appData.timeSlots || [];
        timeSlots.forEach(timeSlot => {
            const timeCell = document.createElement("div");
            timeCell.className = "time-header";
            timeCell.innerHTML = `${formatTime(timeSlot.startTime)}<br>${formatTime(timeSlot.endTime)}`;
            gridElement.appendChild(timeCell);

            for (let day = 1; day <= 6; day++) {
                const slot = createLessonSlot(timeSlot, day, []);
                gridElement.appendChild(slot);
            }
        });

        newCard.innerHTML = '';
        newCard.appendChild(gridElement);
        setTimeout(() => newCard.classList.add('animate'), 10);

        updateTimeSlotFilterPanel();
        populateCompareTeacherSelect();

    } catch (err) {
        console.error("Ошибка загрузки преподавателя:", err);
        showAlert("Не удалось загрузить расписание преподавателя", "danger");
        // Если ошибка - оставляем фильтры заблокированными
    } finally {
        isLoading = false;
        const loadingEl = document.getElementById("loading");
        if (loadingEl) loadingEl.style.display = "none";
    }
}

/**
 * Заполнение списка групп и РАЗБЛОКИРОВКА
 */
function populateTeacherGroupSelect() {
    const select = document.getElementById('teacherGroupsSelect');
    const btn = document.getElementById('applyTeacherGroupFilterBtn');
    if (!select) return;

    select.innerHTML = '<option value="all">Все группы</option>';

    const uniqueGroups = new Map();
    const schedule = ScheduleStore.data.schedule || {};

    for (const day in schedule) {
        const slots = schedule[day] || [];
        slots.forEach(slot => {
            if (slot.lessons) {
                slot.lessons.forEach(lesson => {
                    if (lesson.groupId && lesson.groupName) {
                        uniqueGroups.set(lesson.groupId, lesson.groupName);
                    }
                });
            }
        });
    }

    const sortedGroups = Array.from(uniqueGroups.entries()).sort((a, b) => a[1].localeCompare(b[1]));

    sortedGroups.forEach(([id, name]) => {
        const option = document.createElement('option');
        option.value = id;
        option.textContent = name;
        select.appendChild(option);
    });

    // === РАЗБЛОКИРОВКА ===
    select.disabled = false;
    if (btn) btn.disabled = false;
}

function applyTeacherGroupFilter() {
    if (currentScheduleType !== 'teacher') return;

    const select = document.getElementById('teacherGroupsSelect');
    if (!select) return;

    currentTeacherGroupFilter = select.value;

    const slots = document.querySelectorAll('.lesson-slot');

    slots.forEach(slot => {
        const dayId = parseInt(slot.dataset.dayOfWeekId);
        const timeId = parseInt(slot.dataset.timeSlotId);
        const allLessons = ScheduleStore.getLessonsInSlot(dayId, timeId);

        let filteredLessons = allLessons;
        if (currentTeacherGroupFilter !== 'all') {
            const filterId = parseInt(currentTeacherGroupFilter);
            filteredLessons = allLessons.filter(l => l.groupId === filterId);
        }
        renderLessonsInSlot(slot, filteredLessons, 'teacher');
    });

    // Ненавязчивое уведомление, можно убрать если раздражает
    // showAlert("Фильтр применен", "success"); 
}

async function loadAllTeachers() {
    try {
        const response = await fetch('http://localhost:5062/api/Teacher/Short');
        if (!response.ok) throw new Error('Не удалось загрузить список преподавателей');
        allTeachers = await response.json();
    } catch (err) {
        console.error('Ошибка:', err);
    }
}

function populateCompareTeacherSelect() {
    if (!allTeachers || allTeachers.length === 0) return;
    const select = document.getElementById('compareWithTeacher');
    if (!select) return;

    select.innerHTML = '<option value="">Выберите преподавателя</option>';
    allTeachers.forEach(t => {
        if (t.id == currentTeacherId) return;
        const opt = document.createElement('option');
        opt.value = t.id;
        opt.textContent = t.fullName || t.name || t.Name || 'Без имени';
        select.appendChild(opt);
    });
}

async function getTeacherScheduleForComparison(teacherId) {
    if (currentScheduleType === 'teacher' && currentTeacherId === teacherId) {
        return ScheduleStore.data.schedule;
    }
    if (comparisonCache[teacherId]) {
        return comparisonCache[teacherId].schedule;
    }
    const data = await ScheduleAPI.getTeacherSchedule(teacherId);
    comparisonCache[teacherId] = data;
    return data.schedule;
}

async function findCommonFreeTime() {
    if (currentScheduleType !== 'teacher' || !currentTeacherId) {
        showAlert("Сначала загрузите расписание преподавателя", "warning");
        return;
    }
    const compareTeacherId = document.getElementById('compareWithTeacher').value;
    if (!compareTeacherId) {
        showAlert("Выберите преподавателя для сравнения", "warning");
        return;
    }
    try {
        const currentSchedule = ScheduleStore.data.schedule;
        const compareSchedule = await getTeacherScheduleForComparison(parseInt(compareTeacherId));
        document.querySelectorAll('.lesson-slot.common-free').forEach(el => el.classList.remove('common-free'));

        const getBusySlots = (sched) => {
            const busy = new Set();
            for (const day in sched) {
                const slots = sched[day] || [];
                slots.forEach(slot => {
                    if (slot.lessons && slot.lessons.length > 0) {
                        const tid = slot.timeSlotId || slot.lessons[0].timeSlotId;
                        if (tid) busy.add(`${day}-${tid}`);
                    }
                });
            }
            return busy;
        };
        const currentBusy = getBusySlots(currentSchedule);
        const compareBusy = getBusySlots(compareSchedule);
        const commonFreeSlots = [];
        const timeSlots = window.appData?.timeSlots || [];

        timeSlots.forEach(ts => {
            for (let day = 1; day <= 6; day++) {
                const key = `${day}-${ts.id}`;
                if (!currentBusy.has(key) && !compareBusy.has(key)) {
                    commonFreeSlots.push({ day, timeSlotId: ts.id });
                }
            }
        });

        commonFreeSlots.forEach(({ day, timeSlotId }) => {
            const slot = document.querySelector(`.lesson-slot[data-day-of-week-id="${day}"][data-time-slot-id="${timeSlotId}"]`);
            if (slot) slot.classList.add('common-free');
        });
        showAlert(`Найдено ${commonFreeSlots.length} общих свободных слотов.`, "success");
        document.getElementById('clearComparisonBtn').style.display = 'inline-block';
    } catch (err) {
        console.error("Ошибка при сравнении:", err);
        showAlert("Не удалось сравнить расписания", "danger");
    }
}

function highlightFreeTimeForTeacher() {
    if (!window.appData?.timeSlots || currentScheduleType !== 'teacher') {
        showAlert("Сначала загрузите расписание преподавателя.", "warning");
        return;
    }
    removeFreeTimeHighlight(true);
    const busySlots = new Set();
    const schedule = ScheduleStore.data.schedule;
    for (const day in schedule) {
        const slots = schedule[day] || [];
        slots.forEach(slot => {
            if (slot.lessons && slot.lessons.length > 0) {
                const tid = slot.timeSlotId || slot.lessons[0].timeSlotId;
                if (tid) busySlots.add(`${day}-${tid}`);
            }
        });
    }
    let highlightedCount = 0;
    window.appData.timeSlots.forEach(ts => {
        for (let day = 1; day <= 6; day++) {
            if (!busySlots.has(`${day}-${ts.id}`)) {
                const slot = document.querySelector(`.lesson-slot[data-day-of-week-id="${day}"][data-time-slot-id="${ts.id}"]`);
                if (slot) {
                    slot.classList.add('free-time');
                    highlightedCount++;
                }
            }
        }
    });
    if (highlightedCount > 0) {
        showAlert(`Найдено ${highlightedCount} окон.`, "info");
        document.getElementById('hideFreeTimeBtn').style.display = 'inline-block';
        document.getElementById('showFreeTimeBtn').style.display = 'none';
    } else {
        showAlert("Нет свободного времени.", "info");
    }
}

function removeFreeTimeHighlight(silent = false) {
    const slots = document.querySelectorAll('.lesson-slot.free-time');
    slots.forEach(s => s.classList.remove('free-time', 'reset-animation'));
    document.getElementById('hideFreeTimeBtn').style.display = 'none';
    document.getElementById('showFreeTimeBtn').style.display = 'inline-block';
    if (!silent && slots.length > 0) showAlert("Подсветка скрыта.", "info");
}

function initTeacherHandlers() {
    const showTeacherBtn = document.getElementById('showTeacherButton');
    const showFreeTimeBtn = document.getElementById('showFreeTimeBtn');
    const hideFreeTimeBtn = document.getElementById('hideFreeTimeBtn');
    const teacherSelect = document.getElementById('teacherSelect');

    // Кнопки для блокировки/разблокировки
    const applyGroupFilterBtn = document.getElementById('applyTeacherGroupFilterBtn');
    const groupSelect = document.getElementById('teacherGroupsSelect');

    document.getElementById('findCommonFreeTimeBtn')?.addEventListener('click', findCommonFreeTime);

    document.getElementById('clearComparisonBtn')?.addEventListener('click', function () {
        document.querySelectorAll('.lesson-slot.common-free').forEach(el => el.classList.remove('common-free'));
        document.getElementById('compareWithTeacher').value = '';
        this.style.display = 'none';
        showAlert("Сравнение сброшено", "info");
    });

    if (showTeacherBtn && teacherSelect) {
        showTeacherBtn.addEventListener('click', () => {
            const selectedId = teacherSelect.value;
            if (selectedId) {
                removeFreeTimeHighlight(true);
                loadTeacherSchedule(parseInt(selectedId));
            } else {
                showAlert("Выберите преподавателя.", "warning");
            }
        });

        
        teacherSelect.addEventListener('change', function () {
            if (!this.value) {
                removeFreeTimeHighlight(true);
                if (hideFreeTimeBtn) hideFreeTimeBtn.style.display = 'none';
                if (showFreeTimeBtn) showFreeTimeBtn.style.display = 'none';

                // Блокируем фильтры
                if (groupSelect) { groupSelect.disabled = true; groupSelect.value = 'all'; }
                if (applyGroupFilterBtn) applyGroupFilterBtn.disabled = true;
            }
        });
    }

    if (applyGroupFilterBtn) {
        applyGroupFilterBtn.addEventListener('click', applyTeacherGroupFilter);
    }

    if (showFreeTimeBtn) showFreeTimeBtn.addEventListener('click', highlightFreeTimeForTeacher);
    if (hideFreeTimeBtn) hideFreeTimeBtn.addEventListener('click', () => removeFreeTimeHighlight());
}

document.addEventListener('DOMContentLoaded', loadAllTeachers);