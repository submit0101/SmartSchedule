
let dragOverCache = {};

function handleDragStart(e) {
    if (window.dragSrcElement) window.dragSrcElement.classList.remove('dragging');
    const lessonItem = this;
    if (!lessonItem) {
        e.preventDefault();
        return;
    }
    window.dragSrcElement = lessonItem;
    const lessonId = lessonItem.dataset.lessonId;
    if (!lessonId) {
        e.preventDefault();
        return;
    }
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/plain', lessonId);
    lessonItem.classList.add('dragging');
}

function handleDragEnd(e) {
    if (this.classList?.contains('dragging')) this.classList.remove('dragging');
    // ЖЕСТКАЯ ОЧИСТКА ВСЕХ СТИЛЕЙ
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

let lastDragOverCheck = 0;
const DRAG_OVER_DEBOUNCE = 200; 

async function handleDragOver(e) {
    e.preventDefault();
    const now = Date.now();
    if (now - lastDragOverCheck < DRAG_OVER_DEBOUNCE) return;
    lastDragOverCheck = now;

    const targetSlot = this;

    let lessonId = null;
    if (window.dragSrcElement) {
        lessonId = parseInt(window.dragSrcElement.dataset.lessonId, 10);
    } else {
        lessonId = parseInt(e.dataTransfer.getData('text/plain'), 10);
    }

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
    // При выходе курсора убираем ВСЕ классы, включая индикатор загрузки.
    // Это сигнал для handleDragOver, что результат проверки больше не нужен.
    this.classList.remove('drag-over', 'drag-over-valid', 'drag-over-invalid', 'conflict-week', 'conflict-teacher', 'conflict-cabinet', 'drag-over-loading');
}

async function handleDrop(e) {
    e.preventDefault();
    const targetSlot = this;

    // Сразу чистим стили
    targetSlot.classList.remove('drag-over', 'drag-over-valid', 'drag-over-invalid', 'conflict-week', 'conflict-teacher', 'conflict-cabinet', 'drag-over-loading');

    let lessonId = null;
    if (window.dragSrcElement) {
        lessonId = parseInt(window.dragSrcElement.dataset.lessonId, 10);
    } else {
        lessonId = parseInt(e.dataTransfer.getData('text/plain'), 10);
    }

    const lesson = ScheduleStore.findLesson(lessonId);
    if (!lesson) return;

    // ОБЯЗАТЕЛЬНО: Приводим ID целевой ячейки к числам
    const targetTimeId = parseInt(targetSlot.dataset.timeSlotId, 10);
    const targetDayId = parseInt(targetSlot.dataset.dayOfWeekId, 10);

    const oldDayId = lesson.dayOfWeekId;
    const oldTimeId = lesson.timeSlotId;

    if (oldTimeId === targetTimeId && oldDayId === targetDayId) return;

    // Повторная проверка перед дропом (на всякий случай)
    const conflictResult = await ScheduleAPI.checkConflict(lessonId, targetDayId, targetTimeId);
    if (conflictResult.isWeekConflict || conflictResult.isTeacherBusy || conflictResult.isCabinetBusy) {
        showAlert('Невозможно переместить занятие из-за конфликта.', 'danger');
        return;
    }

    try {
        await ScheduleAPI.moveLesson(lessonId, {
            Id: lesson.id,
            GroupId: lesson.groupId,
            SubjectId: lesson.subjectId,
            TeacherId: lesson.teacherId,
            CabinetId: lesson.cabinetId,
            TimeSlotId: targetTimeId,
            DayOfWeekId: targetDayId,
            WeekTypeId: lesson.weekTypeId
        });

        // 1. Удаляем из старого места в Store
        ScheduleStore.removeLesson(lessonId);

        // 2. Обновляем объект урока
        lesson.timeSlotId = targetTimeId;
        lesson.dayOfWeekId = targetDayId;

        // 3. Добавляем в новое место в Store (Здесь сработает обновленный addLesson)
        ScheduleStore.addLesson(lesson, targetDayId, targetTimeId);

        // 4. Перерисовка

        // Перерисовываем СТАРУЮ ячейку
        // Ищем её заново через DOM, так как ссылка oldSlot могла устареть
        const oldSlotElement = document.querySelector(`.lesson-slot[data-day-of-week-id="${oldDayId}"][data-time-slot-id="${oldTimeId}"]`);
        if (oldSlotElement) {
            const oldLessons = ScheduleStore.getLessonsInSlot(oldDayId, oldTimeId);
            renderLessonsInSlot(oldSlotElement, oldLessons, currentScheduleType);
        }

        // Перерисовываем НОВУЮ ячейку (targetSlot - это элемент DOM, на который мы бросили)
        // Получаем ОБНОВЛЕННЫЙ список уроков (теперь там должно быть 2 урока)
        const newLessons = ScheduleStore.getLessonsInSlot(targetDayId, targetTimeId);
        renderLessonsInSlot(targetSlot, newLessons, currentScheduleType);

        // Сброс кэша
        delete dragOverCache[`${oldDayId}-${oldTimeId}`];
        delete dragOverCache[`${targetDayId}-${targetTimeId}`];

        showAlert('Урок успешно перемещён', 'success');

    } catch (error) {
        console.error(error);
        showAlert(`Ошибка: ${error.message || 'Неизвестная ошибка'}`, 'danger');
    } finally {
        window.dragSrcElement = null;
    }
}