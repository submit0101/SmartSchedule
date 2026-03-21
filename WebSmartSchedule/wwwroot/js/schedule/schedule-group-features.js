// wwwroot/js/schedule/schedule-group-features.js
const groupComparisonCache = {};

/**
 * Загрузка списка всех групп для выпадающего списка.
 * Исключает текущую группу, используя надежное строковое сравнение.
 */
async function loadGroupsForComparison() {
    const select = document.getElementById('compareGroupSelect');
    if (!select) return;

    try {
        select.innerHTML = '<option value="" selected disabled>Загрузка...</option>';

        let groups = [];
        if (typeof ScheduleAPI.getAllGroups === 'function') {
            groups = await ScheduleAPI.getAllGroups();
        } else {
            console.warn("ScheduleAPI.getAllGroups не найден, используем fallback fetch");
            const res = await fetch('/api/Group'); 
            if (res.ok) groups = await res.json();
        }
        let currentGroupIdRaw = null;z
        if (window.appData && window.appData.groupId) {
            currentGroupIdRaw = window.appData.groupId;
        } else {
            const params = new URLSearchParams(window.location.search);
            currentGroupIdRaw = params.get('groupId');
        }
        const normalizeId = (id) => String(id).trim();
        const currentIdStr = currentGroupIdRaw ? normalizeId(currentGroupIdRaw) : null;
        select.innerHTML = '<option value="" selected>Выберите группу...</option>';
        groups
            .filter(g => {
                if (!currentIdStr) return true;
                return normalizeId(g.id) !== currentIdStr;
            })
            .sort((a, b) => a.name.localeCompare(b.name))
            .forEach(group => {
                const option = document.createElement('option');
                option.value = group.id;
                option.textContent = group.name;
                select.appendChild(option);
            });

    } catch (e) {
        console.error("Ошибка загрузки групп:", e);
        select.innerHTML = '<option value="" disabled>Ошибка загрузки</option>';
    }
}

/**
 * Основная функция: Сравнение расписания
 */
async function findCommonFreeTimeWithGroup() {
    // Проверка режима (если переменная currentScheduleType используется)
    if (typeof currentScheduleType !== 'undefined' && currentScheduleType !== 'group') {
        showAlert("Эта функция работает только в режиме группы", "warning");
        return;
    }

    const select = document.getElementById('compareGroupSelect');
    if (!select) return;

    const targetGroupId = select.value;
    if (!targetGroupId) {
        showAlert("Выберите группу для сравнения", "warning");
        return;
    }

    // Дополнительная защита от сравнения с самим собой
    if (window.appData && String(targetGroupId) === String(window.appData.groupId)) {
        showAlert("Нельзя сравнивать группу саму с собой", "warning");
        return;
    }

    const btn = document.getElementById('btnCompareGroups');
    if (btn) btn.disabled = true;

    try {
        const currentSchedule = ScheduleStore.data.schedule;

        // Получаем данные второй группы (кэш или запрос)
        let targetScheduleData;
        if (groupComparisonCache[targetGroupId]) {
            targetScheduleData = groupComparisonCache[targetGroupId];
        } else {
            const data = await ScheduleAPI.getGroupSchedule(targetGroupId);
            targetScheduleData = data.schedule;
            groupComparisonCache[targetGroupId] = targetScheduleData;
        }

        // Сброс старой подсветки
        document.querySelectorAll('.lesson-slot.common-free').forEach(el => el.classList.remove('common-free'));

        // Функция сбора занятых слотов
        const getBusySlots = (sched) => {
            const busy = new Set();
            for (const day in sched) {
                const slots = sched[day] || [];
                slots.forEach(slot => {
                    if (slot.lessons && slot.lessons.length > 0) {
                        const tid = slot.timeSlotId || (slot.lessons[0] ? slot.lessons[0].timeSlotId : null);
                        if (tid) busy.add(`${day}-${tid}`);
                    }
                });
            }
            return busy;
        };

        const currentBusy = getBusySlots(currentSchedule);
        const targetBusy = getBusySlots(targetScheduleData);

        const commonFreeSlots = [];
        const timeSlots = window.appData?.timeSlots || [];

        // Поиск пересечений
        timeSlots.forEach(ts => {
            for (let day = 1; day <= 6; day++) {
                const key = `${day}-${ts.id}`;

                if (!currentBusy.has(key) && !targetBusy.has(key)) {
                    commonFreeSlots.push({ day, timeSlotId: ts.id });
                }
            }
        });

        // Визуализация
        commonFreeSlots.forEach(({ day, timeSlotId }) => {
            const slot = document.querySelector(`.lesson-slot[data-day-of-week-id="${day}"][data-time-slot-id="${timeSlotId}"]`);
            if (slot) slot.classList.add('common-free');
        });

        // Итог
        if (commonFreeSlots.length > 0) {
            showAlert(`Найдено ${commonFreeSlots.length} общих окон.`, "success");
            const clearBtn = document.getElementById('btnClearGroupCompare');
            if (clearBtn) clearBtn.style.display = 'block';
        } else {
            showAlert("Нет общих свободных слотов.", "info");
        }

    } catch (e) {
        console.error(e);
        showAlert("Ошибка при сравнении: " + e.message, "danger");
    } finally {
        if (btn) btn.disabled = false;
    }
}

/**
 * Сброс режима сравнения
 */
function clearGroupComparison() {
    document.querySelectorAll('.lesson-slot.common-free').forEach(el => el.classList.remove('common-free'));

    const select = document.getElementById('compareGroupSelect');
    if (select) select.value = "";

    const clearBtn = document.getElementById('btnClearGroupCompare');
    if (clearBtn) clearBtn.style.display = 'none';

    showAlert("Сравнение сброшено", "info");
}

/**
 * Инициализация
 */
function initGroupFeatures() {
    const btnCompare = document.getElementById('btnCompareGroups');
    const btnClear = document.getElementById('btnClearGroupCompare');

    if (btnCompare) btnCompare.addEventListener('click', findCommonFreeTimeWithGroup);
    if (btnClear) btnClear.addEventListener('click', clearGroupComparison);

    loadGroupsForComparison();
}