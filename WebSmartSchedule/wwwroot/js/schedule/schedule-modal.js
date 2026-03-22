async function autoLoadFreeCabinets() {
    const dayId = document.getElementById('gridDayOfWeekId').value;
    const timeId = document.getElementById('gridTimeSlotId').value;
    const weekId = document.getElementById('gridWeekTypeId').value;
    const cabinetSelect = document.getElementById('gridCabinetId');

    if (!dayId || !timeId || !weekId) {
        cabinetSelect.innerHTML = '<option value="">-- Заполните данные (время, тип недели) --</option>';
        return;
    }

    cabinetSelect.disabled = true;

    try {
        const freeCabinets = await ScheduleAPI.getFreeCabinets(dayId, timeId, weekId);
        cabinetSelect.innerHTML = '<option value="">-- Выберите кабинет --</option>';

        if (freeCabinets.length === 0) {
            const opt = document.createElement('option');
            opt.disabled = true;
            opt.textContent = "Нет свободных кабинетов";
            cabinetSelect.appendChild(opt);
        } else {
            freeCabinets.forEach(cab => {
                const option = document.createElement('option');
                option.value = cab.id;
                let bName = cab.buildingName;
                if (!bName && window.appData && window.appData.cabinets) {
                    const knownCab = window.appData.cabinets.find(c => c.id == cab.id);
                    if (knownCab) {
                        bName = knownCab.buildingName;
                    }
                }
                const building = bName ? ` (${bName})` : '';
                option.textContent = `${cab.number}${building}`;

                cabinetSelect.appendChild(option);
            });
        }
    } catch (err) {
        console.error("Ошибка загрузки кабинетов:", err);
        cabinetSelect.innerHTML = '<option value="">Ошибка связи с сервером</option>';
    } finally {
        cabinetSelect.disabled = false;
    }
}

function openCreateLessonModalForSlot(timeSlotId, dayOfWeekId, existingLessonsInSlot, pastedLessonData = null) {
    const modal = document.getElementById('createLessonGridModal');
    const form = document.getElementById('createLessonGridForm');
    if (!modal || !form) return;

    form.reset();
    form.classList.remove('was-validated');

    const gridSubgroupEl = document.getElementById('gridSubgroup');
    if (gridSubgroupEl) gridSubgroupEl.value = "";

    document.getElementById('gridTimeSlotId').value = timeSlotId;
    document.getElementById('gridDayOfWeekId').value = dayOfWeekId;

    populateGridSelect('gridSubjectId', window.appData.subjects, 'title');
    populateGridSelect('gridGroupId', window.appData.groups, 'name');
    populateGridSelect('gridTeacherId', window.appData.teachers, 'fullName');
    populateGridSelect('gridWeekTypeId', window.appData.weekTypes, 'name');

    const groupSelect = document.getElementById('gridGroupId');
    const teacherSelect = document.getElementById('gridTeacherId');

    if (currentScheduleType === 'teacher') {
        if (teacherSelect) {
            teacherSelect.value = currentTeacherId;
            teacherSelect.disabled = true;
        }
        if (groupSelect) {
            groupSelect.value = "";
            groupSelect.disabled = false;
        }
    } else {
        if (groupSelect) {
            groupSelect.value = window.appData.groupId;
            groupSelect.disabled = true;
        }
        if (teacherSelect) {
            teacherSelect.value = "";
            teacherSelect.disabled = false;
        }
    }

    let suggestedWeekTypeId = '';
    const state = analyzeSlotState(existingLessonsInSlot);

    if (pastedLessonData) {
        if (state.isFull) { showAlert('Ячейка занята', 'warning'); return; }
    } else {
        if (!state.isWhole) {
            if (state.hasNum && !state.hasDen) suggestedWeekTypeId = WeekType.DENOMINATOR;
            else if (state.hasDen && !state.hasNum) suggestedWeekTypeId = WeekType.NUMERATOR;
        }
    }

    if (suggestedWeekTypeId) {
        document.getElementById('gridWeekTypeId').value = suggestedWeekTypeId;
    }

    if (pastedLessonData) {
        document.getElementById('gridSubjectId').value = pastedLessonData.subjectId || '';
        if (currentScheduleType === 'teacher') {
            if (groupSelect) groupSelect.value = pastedLessonData.groupId || '';
        } else {
            if (teacherSelect) teacherSelect.value = pastedLessonData.teacherId || '';
        }
    }

    autoLoadFreeCabinets();
    openModal('createLessonGridModal');
}

document.addEventListener('DOMContentLoaded', () => {
    ['gridWeekTypeId', 'gridDayOfWeekId', 'gridTimeSlotId'].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.addEventListener('change', autoLoadFreeCabinets);
    });
});

function populateGridSelect(selectId, items, displayProp, secondaryProp = null) {
    const select = document.getElementById(selectId);
    if (!select) return;
    const name = select.getAttribute('name');
    const defaults = {
        'WeekTypeId': '-- Выберите тип недели --',
        'SubjectId': '-- Выберите предмет --',
        'TeacherId': '-- Выберите преподавателя --',
        'CabinetId': '-- Выберите кабинет --',
        'GroupId': '-- Выберите группу --'
    };
    select.innerHTML = `<option value="">${defaults[name] || '-- Выберите --'}</option>`;
    items.forEach(item => {
        const option = document.createElement("option");
        option.value = item.id;
        let text = item[displayProp] || "Неизвестно";
        if (secondaryProp && item[secondaryProp] && displayProp === 'number') {
            text = `${text} (${item[secondaryProp]})`;
        }
        option.textContent = text;
        select.appendChild(option);
    });
}

function openEditLessonModal(lesson) {
    window.currentSelectedLessonData = lesson;
    document.getElementById("editLessonId").value = lesson.id;

    const editSubgroupEl = document.getElementById("editSubgroup");
    if (editSubgroupEl) editSubgroupEl.value = lesson.subgroup || "";

    fillSelect("editSubjectId", window.appData.subjects, lesson.subjectId);
    fillSelect("editTeacherId", window.appData.teachers, lesson.teacherId);
    fillSelect("editCabinetId", window.appData.cabinets, lesson.cabinetId);
    fillSelect("editTimeSlotId", window.appData.timeSlots, lesson.timeSlotId);
    fillSelect("editWeekTypeId", window.appData.weekTypes, lesson.weekTypeId);
    fillSelect("editGroupId", window.appData.groups, lesson.groupId);
    document.getElementById("editDayOfWeekId").value = lesson.dayOfWeekId;

    const groupSelect = document.getElementById('editGroupId');
    const teacherSelect = document.getElementById('editTeacherId');

    if (currentScheduleType === 'teacher') {
        if (teacherSelect) teacherSelect.disabled = true;
        if (groupSelect) groupSelect.disabled = false;
    } else {
        if (groupSelect) groupSelect.disabled = true;
        if (teacherSelect) teacherSelect.disabled = false;
    }

    openModal("editLessonModal");
}

function fillSelect(id, items, selectedValue) {
    const select = document.getElementById(id);
    if (!select) return;
    select.innerHTML = "";
    items.forEach(item => {
        const option = document.createElement("option");
        option.value = item.id;
        let text = item.title || item.fullName || item.name || (item.number ? `${item.number} ${item.buildingName ? `(${item.buildingName})` : ''}` : `Slot ${item.slotNumber}`);
        option.textContent = text;
        if (item.id === selectedValue) option.selected = true;
        select.appendChild(option);
    });
}

function formatShortName(fullName) {
    if (!fullName) return 'Неизвестно';
    const parts = fullName.trim().split(/\s+/);
    if (parts.length === 1) return parts[0];
    if (parts.length === 2) return `${parts[0]} ${parts[1][0]}.`;
    return `${parts[0]} ${parts[1][0]}.${parts[2][0]}.`;
}

document.getElementById('createLessonGridForm')?.addEventListener('submit', async function (e) {
    e.preventDefault();

    if (!this.checkValidity()) {
        this.classList.add('was-validated');
        return;
    }

    const groupEl = document.getElementById('gridGroupId');
    const teacherEl = document.getElementById('gridTeacherId');

    const groupWasDisabled = groupEl.disabled;
    const teacherWasDisabled = teacherEl.disabled;

    groupEl.disabled = false;
    teacherEl.disabled = false;

    const formData = new FormData(this);
    const data = Object.fromEntries(formData);

    groupEl.disabled = groupWasDisabled;
    teacherEl.disabled = teacherWasDisabled;

    try {
        const serverResponse = await ScheduleAPI.createLesson(data);
        showAlert('Создано!', 'success');
        closeModal("createLessonGridModal");

        const subjectObj = window.appData.subjects.find(s => s.id == data.SubjectId);
        const teacherObj = window.appData.teachers.find(t => t.id == data.TeacherId);
        const cabinetObj = window.appData.cabinets.find(c => c.id == data.CabinetId);
        const groupObj = window.appData.groups.find(g => g.id == data.GroupId);
        const teacherShortName = teacherObj ? formatShortName(teacherObj.fullName) : 'Неизвестно';

        const lessonForUI = {
            ...serverResponse,
            subjectTitle: subjectObj ? subjectObj.title : 'Неизвестно',
            teacherFullName: teacherShortName,
            cabinetNumber: cabinetObj ? cabinetObj.number : '',
            buildingName: cabinetObj ? cabinetObj.buildingName : '',
            groupName: groupObj ? groupObj.name : (window.appData.groupName || 'Группа'),
            weekTypeId: parseInt(data.WeekTypeId),
            dayOfWeekId: parseInt(data.DayOfWeekId),
            timeSlotId: parseInt(data.TimeSlotId),
            subgroup: data.Subgroup ? parseInt(data.Subgroup) : null
        };

        ScheduleStore.addLesson(lessonForUI, lessonForUI.dayOfWeekId, lessonForUI.timeSlotId);

        const slot = document.querySelector(`.lesson-slot[data-day-of-week-id="${lessonForUI.dayOfWeekId}"][data-time-slot-id="${lessonForUI.timeSlotId}"]`);
        if (slot) {
            const lessons = ScheduleStore.getLessonsInSlot(lessonForUI.dayOfWeekId, lessonForUI.timeSlotId);
            renderLessonsInSlot(slot, lessons, currentScheduleType);
        }

        if (typeof pendingPasteSlot !== 'undefined' && pendingPasteSlot) {
            pendingPasteSlot.classList.remove('pending-paste');
            pendingPasteSlot = null;
        }
    } catch (e) {
        console.error(e);
        showAlert(`Ошибка: ${e.message}`, 'danger');
    }
});

document.getElementById("editLessonForm")?.addEventListener('submit', async function (e) {
    e.preventDefault();

    const groupEl = document.getElementById('editGroupId');
    const teacherEl = document.getElementById('editTeacherId');

    const groupWasDisabled = groupEl.disabled;
    const teacherWasDisabled = teacherEl.disabled;

    groupEl.disabled = false;
    teacherEl.disabled = false;

    const formData = new FormData(this);
    const data = Object.fromEntries(formData);

    groupEl.disabled = groupWasDisabled;
    teacherEl.disabled = teacherWasDisabled;

    try {
        await ScheduleAPI.updateLesson(data.Id, data);
        updateLessonInUI(data);
        showAlert('Обновлено!', 'success');
        closeModal("editLessonModal");
    } catch (e) {
        console.error(e);
        showAlert(`Ошибка: ${e.message}`, 'danger');
    }
});

function deleteLesson() {
    closeModal("editLessonModal");
    openModal("deleteConfirmModal");
}

async function confirmDelete() {
    const lessonData = window.currentSelectedLessonData;
    if (!lessonData) {
        closeModal("deleteConfirmModal");
        return;
    }
    try {
        await ScheduleAPI.deleteLesson(lessonData.id);
        showAlert('Занятие успешно удалено!', 'success');
        closeModal("deleteConfirmModal");
        ScheduleStore.removeLesson(lessonData.id);

        const slotElement = document.querySelector(`.lesson-slot[data-day-of-week-id="${lessonData.dayOfWeekId}"][data-time-slot-id="${lessonData.timeSlotId}"]`);
        if (slotElement) {
            const lessons = ScheduleStore.getLessonsInSlot(lessonData.dayOfWeekId, lessonData.timeSlotId);
            renderLessonsInSlot(slotElement, lessons, currentScheduleType);
        } else {
            loadSchedule(window.appData.groupId);
        }
        window.currentSelectedLessonData = null;
    } catch (error) {
        showAlert(`Ошибка: ${error.message}`, 'danger');
    }
}

function updateLessonInUI(formDataData) {
    const lessonId = parseInt(formDataData.Id);
    const oldLesson = ScheduleStore.findLesson(lessonId);
    if (!oldLesson) {
        loadSchedule(window.appData.groupId);
        return;
    }
    const newLesson = mergeFormDataToLesson(oldLesson, formDataData);
    ScheduleStore.removeLesson(lessonId);
    ScheduleStore.addLesson(newLesson, newLesson.dayOfWeekId, newLesson.timeSlotId);
    refreshScheduleSlots(oldLesson, newLesson);
}

function mergeFormDataToLesson(oldLesson, formData) {
    const findName = (arr, id, prop) => {
        const item = arr.find(x => x.id == id);
        return item ? item[prop] : '???';
    };

    const tObj = window.appData.teachers.find(x => x.id == formData.TeacherId);
    const teacherFullName = tObj ? (typeof formatShortName === 'function' ? formatShortName(tObj.fullName) : tObj.fullName) : 'Не назначен';
    const cabinet = window.appData.cabinets.find(c => c.id == formData.CabinetId);

    return {
        ...oldLesson,
        subjectId: parseInt(formData.SubjectId),
        teacherId: parseInt(formData.TeacherId),
        cabinetId: parseInt(formData.CabinetId),
        weekTypeId: parseInt(formData.WeekTypeId),
        groupId: parseInt(formData.GroupId),
        dayOfWeekId: parseInt(formData.DayOfWeekId),
        timeSlotId: parseInt(formData.TimeSlotId),
        subgroup: formData.Subgroup ? parseInt(formData.Subgroup) : null,
        subjectTitle: findName(window.appData.subjects, formData.SubjectId, 'title'),
        groupName: findName(window.appData.groups, formData.GroupId, 'name'),
        teacherFullName: teacherFullName,
        cabinetNumber: cabinet ? cabinet.number : '?',
        buildingName: cabinet ? cabinet.buildingName : '',
    };
}

function refreshScheduleSlots(oldLesson, newLesson) {
    refreshSingleSlot(oldLesson.dayOfWeekId, oldLesson.timeSlotId);
    const isMoved = oldLesson.dayOfWeekId !== newLesson.dayOfWeekId ||
        oldLesson.timeSlotId !== newLesson.timeSlotId;
    if (isMoved) {
        refreshSingleSlot(newLesson.dayOfWeekId, newLesson.timeSlotId);
    }
}

function refreshSingleSlot(dayId, timeId) {
    const slotEl = document.querySelector(`.lesson-slot[data-day-of-week-id="${dayId}"][data-time-slot-id="${timeId}"]`);
    if (slotEl) {
        const lessons = ScheduleStore.getLessonsInSlot(dayId, timeId);
        renderLessonsInSlot(slotEl, lessons, currentScheduleType);
    }
}