let dragOverCache = {};
let lastDragOverCheck = 0;
const DRAG_OVER_DEBOUNCE = 200;

function handleDragStart(e) {
    if (window.dragSrcElement) window.dragSrcElement.classList.remove('dragging');
    const lessonItem = this;
    if (!lessonItem) {
        e.preventDefault();
        return;
    }
    window.dragSrcElement = lessonItem;

    const lessonIds = lessonItem.dataset.lessonIds || lessonItem.dataset.lessonId;
    if (!lessonIds) {
        e.preventDefault();
        return;
    }

    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/plain', lessonIds.toString());
    lessonItem.classList.add('dragging');
}

function handleDragEnd(e) {
    if (this.classList?.contains('dragging')) this.classList.remove('dragging');

    document.querySelectorAll('.lesson-slot').forEach(slot =>
        slot.classList.remove('drag-over', 'drag-over-valid', 'drag-over-invalid', 'conflict-week', 'conflict-teacher', 'conflict-cabinet', 'drag-over-loading')
    );

    window.dragSrcElement = null;
    dragOverCache = {};
}

function handleDragEnter(e) {
    e.preventDefault();
    const targetSlot = this;
    targetSlot.classList.remove('drag-over-valid', 'drag-over-invalid', 'conflict-week', 'conflict-teacher', 'conflict-cabinet');
    targetSlot.classList.add('drag-over');
}

async function handleDragOver(e) {
    e.preventDefault();
    const now = Date.now();
    if (now - lastDragOverCheck < DRAG_OVER_DEBOUNCE) return;
    lastDragOverCheck = now;

    const targetSlot = this;

    let lessonIdsStr = '';
    if (window.dragSrcElement) {
        lessonIdsStr = window.dragSrcElement.dataset.lessonIds || window.dragSrcElement.dataset.lessonId;
    } else {
        lessonIdsStr = e.dataTransfer.getData('text/plain');
    }

    if (!lessonIdsStr) return;

    const lessonId = parseInt(lessonIdsStr.split(',')[0], 10);
    const lesson = ScheduleStore.findLesson(lessonId);

    if (!lesson) {
        targetSlot.classList.add('drag-over-invalid');
        return;
    }

    if (currentScheduleType === 'group' && window.appData?.groupId && lesson.groupId !== parseInt(window.appData.groupId, 10)) {
        targetSlot.classList.add('drag-over-invalid');
        return;
    }

    if (currentScheduleType === 'teacher' && currentTeacherId && lesson.teacherId !== currentTeacherId) {
        targetSlot.classList.add('drag-over-invalid');
        return;
    }

    const targetTimeId = parseInt(targetSlot.dataset.timeSlotId, 10);
    const targetDayId = parseInt(targetSlot.dataset.dayOfWeekId, 10);

    if (lesson.timeSlotId === targetTimeId && lesson.dayOfWeekId === targetDayId) {
        targetSlot.classList.add('drag-over-valid');
        return;
    }

    const cacheKey = `${targetDayId}-${targetTimeId}`;
    if (dragOverCache[cacheKey]) {
        applyConflictClasses(targetSlot, dragOverCache[cacheKey]);
        return;
    }

    targetSlot.classList.add('drag-over-loading');
    try {
        const conflictResult = await ScheduleAPI.checkConflict(lessonId, targetDayId, targetTimeId);
        if (!targetSlot.classList.contains('drag-over-loading')) {
            return;
        }

        targetSlot.classList.remove('drag-over-loading');
        dragOverCache[cacheKey] = conflictResult;
        applyConflictClasses(targetSlot, conflictResult);
    } catch (err) {
        console.error(err);
        targetSlot.classList.remove('drag-over-loading');
    }
}

function applyConflictClasses(slot, conflict) {
    if (conflict.isWeekConflict || conflict.isTeacherBusy || conflict.isCabinetBusy) {
        slot.classList.remove('drag-over-valid');
        slot.classList.add('drag-over', 'drag-over-invalid');
        if (conflict.isWeekConflict) slot.classList.add('conflict-week');
        if (conflict.isTeacherBusy) slot.classList.add('conflict-teacher');
        if (conflict.isCabinetBusy) slot.classList.add('conflict-cabinet');
    } else {
        slot.classList.remove('drag-over-invalid', 'conflict-week', 'conflict-teacher', 'conflict-cabinet');
        slot.classList.add('drag-over', 'drag-over-valid');
    }
}

function handleDragLeave(e) {
    this.classList.remove('drag-over', 'drag-over-valid', 'drag-over-invalid', 'conflict-week', 'conflict-teacher', 'conflict-cabinet', 'drag-over-loading');
}

async function handleDrop(e) {
    e.preventDefault();
    const targetSlot = this;
    targetSlot.classList.remove('drag-over', 'drag-over-valid', 'drag-over-invalid', 'conflict-week', 'conflict-teacher', 'conflict-cabinet', 'drag-over-loading');

    let lessonIdsStr = '';
    if (window.dragSrcElement) {
        lessonIdsStr = window.dragSrcElement.dataset.lessonIds || window.dragSrcElement.dataset.lessonId;
    } else {
        lessonIdsStr = e.dataTransfer.getData('text/plain');
    }

    if (!lessonIdsStr) return;

    const lessonIds = lessonIdsStr.split(',').map(id => parseInt(id, 10));
    const targetTimeId = parseInt(targetSlot.dataset.timeSlotId, 10);
    const targetDayId = parseInt(targetSlot.dataset.dayOfWeekId, 10);

    const firstLesson = ScheduleStore.findLesson(lessonIds[0]);
    if (!firstLesson) return;

    const oldDayId = firstLesson.dayOfWeekId;
    const oldTimeId = firstLesson.timeSlotId;

    if (oldTimeId === targetTimeId && oldDayId === targetDayId) return;

    const conflictResult = await ScheduleAPI.checkConflict(lessonIds[0], targetDayId, targetTimeId);
    if (conflictResult.isWeekConflict || conflictResult.isTeacherBusy || conflictResult.isCabinetBusy) {
        showAlert('Невозможно переместить занятие из-за конфликта.', 'danger');
        return;
    }

    try {
        const updatePromises = lessonIds.map(async (id) => {
            const lesson = ScheduleStore.findLesson(id);
            if (!lesson) return;

            const updateData = {
                Id: lesson.id,
                GroupId: lesson.groupId,
                SubjectId: lesson.subjectId,
                TeacherId: lesson.teacherId,
                CabinetId: lesson.cabinetId,
                TimeSlotId: targetTimeId,
                DayOfWeekId: targetDayId,
                WeekTypeId: lesson.weekTypeId,
                Subgroup: lesson.subgroup
            };

            if (ScheduleAPI.moveLesson) {
                await ScheduleAPI.moveLesson(id, updateData);
            } else {
                await ScheduleAPI.updateLesson(id, updateData);
            }

            ScheduleStore.removeLesson(id);
            lesson.timeSlotId = targetTimeId;
            lesson.dayOfWeekId = targetDayId;
            ScheduleStore.addLesson(lesson, targetDayId, targetTimeId);
        });

        await Promise.all(updatePromises);

        const oldSlotElement = document.querySelector(`.lesson-slot[data-day-of-week-id="${oldDayId}"][data-time-slot-id="${oldTimeId}"]`);
        if (oldSlotElement) {
            const oldLessons = ScheduleStore.getLessonsInSlot(oldDayId, oldTimeId);
            if (typeof renderLessonsInSlot === 'function') {
                renderLessonsInSlot(oldSlotElement, oldLessons, typeof currentScheduleType !== 'undefined' ? currentScheduleType : 'group');
            }
        }

        const newLessons = ScheduleStore.getLessonsInSlot(targetDayId, targetTimeId);
        if (typeof renderLessonsInSlot === 'function') {
            renderLessonsInSlot(targetSlot, newLessons, typeof currentScheduleType !== 'undefined' ? currentScheduleType : 'group');
        }

        delete dragOverCache[`${oldDayId}-${oldTimeId}`];
        delete dragOverCache[`${targetDayId}-${targetTimeId}`];
        showAlert('Занятие успешно перемещено!', 'success');

    } catch (error) {
        console.error(error);
        showAlert(`Ошибка: ${error.message || 'Неизвестная ошибка'}`, 'danger');
    } finally {
        window.dragSrcElement = null;
    }
}