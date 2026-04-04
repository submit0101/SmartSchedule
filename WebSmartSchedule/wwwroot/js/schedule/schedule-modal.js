async function autoLoadFreeCabinets() {
    const dayId = document.getElementById('gridDayOfWeekId').value;
    const timeId = document.getElementById('gridTimeSlotId').value;
    const weekId = document.getElementById('gridWeekTypeId').value;

    const cabinetSelects = [
        document.getElementById('gridCabinetId'),
        document.getElementById('gridCabinetId_1'),
        document.getElementById('gridCabinetId_2')
    ];

    if (!dayId || !timeId || !weekId) {
        cabinetSelects.forEach(select => {
            if (select) select.innerHTML = '<option value="">-- Заполните время --</option>';
        });
        return;
    }

    cabinetSelects.forEach(select => { if (select) select.disabled = true; });

    try {
        const freeCabinets = await ScheduleAPI.getFreeCabinets(dayId, timeId, weekId);

        cabinetSelects.forEach(select => {
            if (!select) return;
            select.innerHTML = '<option value="">-- Выберите кабинет --</option>';

            if (freeCabinets.length === 0) {
                const opt = document.createElement('option');
                opt.disabled = true;
                opt.textContent = "Нет свободных кабинетов";
                select.appendChild(opt);
            } else {
                freeCabinets.forEach(cab => {
                    const option = document.createElement('option');
                    option.value = cab.id;
                    let bName = cab.buildingName;
                    if (!bName && window.appData && window.appData.cabinets) {
                        const knownCab = window.appData.cabinets.find(c => c.id == cab.id);
                        if (knownCab) bName = knownCab.buildingName;
                    }
                    const building = bName ? ` (${bName})` : '';
                    option.textContent = `${cab.number}${building}`;
                    select.appendChild(option);
                });
            }
        });
    } catch (err) {
        console.error(err);
        cabinetSelects.forEach(select => {
            if (select) select.innerHTML = '<option value="">Ошибка сервера</option>';
        });
    } finally {
        cabinetSelects.forEach(select => { if (select) select.disabled = false; });
    }
}

function openCreateLessonModalForSlot(timeSlotId, dayOfWeekId, existingLessonsInSlot, pastedLessonData = null) {
    const modal = document.getElementById('createLessonGridModal');
    const form = document.getElementById('createLessonGridForm');
    if (!modal || !form) return;

    if (existingLessonsInSlot && existingLessonsInSlot.length > 0) {
        const hasFullWeekWholeGroup = existingLessonsInSlot.some(l => l.weekTypeId == WeekType.FULL && !l.subgroup);
        if (hasFullWeekWholeGroup && !pastedLessonData) {
            showAlert('Эта ячейка полностью занята целой парой.', 'warning');
            return;
        }
    }

    form.reset();
    form.classList.remove('was-validated');

    const formatSelect = document.getElementById('gridLessonFormat');
    if (formatSelect) {
        formatSelect.value = "1";
        if (typeof toggleLessonFormat === 'function') toggleLessonFormat();
    }

    document.getElementById('gridTimeSlotId').value = timeSlotId;
    document.getElementById('gridDayOfWeekId').value = dayOfWeekId;

    populateGridSelect('gridSubjectId', window.appData.subjects, 'title');
    populateGridSelect('gridSubjectId_1', window.appData.subjects, 'title');
    populateGridSelect('gridSubjectId_2', window.appData.subjects, 'title');

    populateGridSelect('gridGroupId', window.appData.groups, 'name');

    populateGridSelect('gridTeacherId', window.appData.teachers, 'fullName');
    populateGridSelect('gridTeacherId_1', window.appData.teachers, 'fullName');
    populateGridSelect('gridTeacherId_2', window.appData.teachers, 'fullName');

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
    const name = select.getAttribute('name') || selectId;
    select.innerHTML = `<option value="">-- Выберите --</option>`;
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

function createLessonUIObject(data, serverResponse, subgroupOverride = null) {
    const subjectObj = window.appData.subjects.find(s => s.id == data.SubjectId);
    const teacherObj = window.appData.teachers.find(t => t.id == data.TeacherId);
    const cabinetObj = window.appData.cabinets.find(c => c.id == data.CabinetId);
    const groupObj = window.appData.groups.find(g => g.id == data.GroupId);
    const teacherShortName = teacherObj ? formatShortName(teacherObj.fullName) : 'Неизвестно';

    return {
        ...serverResponse,
        subjectTitle: subjectObj ? subjectObj.title : 'Неизвестно',
        teacherFullName: teacherShortName,
        cabinetNumber: cabinetObj ? cabinetObj.number : '',
        buildingName: cabinetObj ? cabinetObj.buildingName : '',
        groupName: groupObj ? groupObj.name : (window.appData.groupName || 'Группа'),
        weekTypeId: parseInt(data.WeekTypeId),
        dayOfWeekId: parseInt(data.DayOfWeekId),
        timeSlotId: parseInt(data.TimeSlotId),
        subgroup: subgroupOverride !== null ? subgroupOverride : (data.Subgroup ? parseInt(data.Subgroup) : null)
    };
}

document.getElementById('createLessonGridForm')?.addEventListener('submit', async function (e) {
    e.preventDefault();

    const format = document.getElementById('gridLessonFormat').value;
    const groupEl = document.getElementById('gridGroupId');
    const groupWasDisabled = groupEl.disabled;
    groupEl.disabled = false;

    const baseData = {
        GroupId: document.getElementById('gridGroupId').value,
        WeekTypeId: document.getElementById('gridWeekTypeId').value,
        DayOfWeekId: document.getElementById('gridDayOfWeekId').value,
        TimeSlotId: document.getElementById('gridTimeSlotId').value
    };

    if (!baseData.GroupId || !baseData.WeekTypeId) {
        showAlert('Выберите группу и тип недели!', 'warning');
        groupEl.disabled = groupWasDisabled;
        return;
    }

    try {
        if (format === '1') {
            if (!this.checkValidity()) {
                this.classList.add('was-validated');
                groupEl.disabled = groupWasDisabled;
                return;
            }

            const formData = new FormData(this);
            const data = Object.fromEntries(formData);

            const serverResponse = await ScheduleAPI.createLesson(data);
            const lessonForUI = createLessonUIObject(data, serverResponse);

            ScheduleStore.addLesson(lessonForUI, lessonForUI.dayOfWeekId, lessonForUI.timeSlotId);
            refreshSingleSlot(lessonForUI.dayOfWeekId, lessonForUI.timeSlotId);
            showAlert('Занятие создано!', 'success');

        } else {
            const sub1 = document.getElementById('gridSubjectId_1').value;
            const tch1 = document.getElementById('gridTeacherId_1').value;
            const cab1 = document.getElementById('gridCabinetId_1').value;

            const sub2 = document.getElementById('gridSubjectId_2').value;
            const tch2 = document.getElementById('gridTeacherId_2').value;
            const cab2 = document.getElementById('gridCabinetId_2').value;

            if (!sub1 || !tch1 || !cab1 || !sub2 || !tch2 || !cab2) {
                showAlert('Заполните предмет, преподавателя и кабинет для ОБЕИХ подгрупп!', 'warning');
                groupEl.disabled = groupWasDisabled;
                return;
            }

            const data1 = { ...baseData, SubjectId: sub1, TeacherId: tch1, CabinetId: cab1, Subgroup: 1 };
            const data2 = { ...baseData, SubjectId: sub2, TeacherId: tch2, CabinetId: cab2, Subgroup: 2 };

            const [resp1, resp2] = await Promise.all([
                ScheduleAPI.createLesson(data1),
                ScheduleAPI.createLesson(data2)
            ]);

            const lesson1UI = createLessonUIObject(data1, resp1, 1);
            const lesson2UI = createLessonUIObject(data2, resp2, 2);

            ScheduleStore.addLesson(lesson1UI, lesson1UI.dayOfWeekId, lesson1UI.timeSlotId);
            ScheduleStore.addLesson(lesson2UI, lesson2UI.dayOfWeekId, lesson2UI.timeSlotId);

            refreshSingleSlot(baseData.DayOfWeekId, baseData.TimeSlotId);
            showAlert('Обе подгруппы успешно добавлены!', 'success');
        }

        closeModal("createLessonGridModal");

        if (typeof pendingPasteSlot !== 'undefined' && pendingPasteSlot) {
            pendingPasteSlot.classList.remove('pending-paste');
            pendingPasteSlot = null;
        }

    } catch (e) {
        console.error(e);
        showAlert(`Ошибка: ${e.message}`, 'danger');
    } finally {
        groupEl.disabled = groupWasDisabled;
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

        refreshSingleSlot(lessonData.dayOfWeekId, lessonData.timeSlotId);
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
    const isMoved = oldLesson.dayOfWeekId !== newLesson.dayOfWeekId || oldLesson.timeSlotId !== newLesson.timeSlotId;
    if (isMoved) {
        refreshSingleSlot(newLesson.dayOfWeekId, newLesson.timeSlotId);
    }
}

function openSubgroupSelectModal(lessonIds) {
    const container = document.getElementById('subgroupActionButtons');
    if (!container) return;
    container.innerHTML = '';

    lessonIds.forEach(id => {
        const lesson = ScheduleStore.findLesson(id);
        if (!lesson) return;

        const btn = document.createElement('button');
        // Используем стили list-group-item для создания красивых карточек
        btn.className = 'list-group-item list-group-item-action d-flex justify-content-between align-items-center rounded border';

        btn.innerHTML = `
            <div class="ms-2 me-auto text-start">
                <div class="fw-bold text-dark mb-1" style="font-size: 0.95rem;">
                    ${lesson.subgroup ? lesson.subgroup + ' подгруппа' : 'Вся группа'}
                </div>
                <div class="text-muted" style="font-size: 0.8rem; line-height: 1.2;">
                    ${lesson.subjectTitle}<br>
                    ${lesson.teacherFullName}
                </div>
            </div>
            <span class="text-primary fs-5">&rsaquo;</span>
        `;

        btn.onclick = () => {
            closeModal('selectSubgroupModal');
            if (typeof openEditLessonModal === 'function') {
                openEditLessonModal(lesson);
            }
        };

        container.appendChild(btn);
    });

    openModal('selectSubgroupModal');
}

function refreshSingleSlot(dayId, timeId) {
    const slotEl = document.querySelector(`.lesson-slot[data-day-of-week-id="${dayId}"][data-time-slot-id="${timeId}"]`);
    if (slotEl) {
        const lessons = ScheduleStore.getLessonsInSlot(dayId, timeId);
        if (typeof renderLessonsInSlot === 'function') {
            renderLessonsInSlot(slotEl, lessons, typeof currentScheduleType !== 'undefined' ? currentScheduleType : 'group');
        }
    }
}
// Добавьте это в конец schedule-modal.js
document.addEventListener('change', function(e) {
    // Проверяем, что изменен первый предмет и выбрано деление на подгруппы
    if (e.target && e.target.id === 'gridSubjectId_1') {
        const format = document.getElementById('gridLessonFormat').value;
        const subject2 = document.getElementById('gridSubjectId_2');
        
        if (format === '2' && subject2) {
            subject2.value = e.target.value; // Копируем значение
            subject2.classList.add('bg-light'); // Визуально выделяем, что поле "автоматическое"
        }
    }
});

// Также добавим синхронизацию при переключении самого формата
document.getElementById('gridLessonFormat')?.addEventListener('change', function() {
    if (this.value === '2') {
        const sub1 = document.getElementById('gridSubjectId_1').value;
        const sub2 = document.getElementById('gridSubjectId_2');
        if (sub1 && sub2) {
            sub2.value = sub1;
        }
    }
});