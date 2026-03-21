const LessonClipboard = {
    data: null,

    copy(lesson) {
        this.data = JSON.parse(JSON.stringify(lesson));
        document.body.classList.add('copy-mode');
        showAlert('Скопировано. Кликните по пустой ячейке для вставки (или ESC для отмены).', 'info');
        document.querySelectorAll('.lesson-action-tooltip').forEach(el => el.remove());
    },

    async pasteToSlot(slotElement, dayId, timeSlotId) {
        if (!this.data) return;

        const newLessonData = {
            ...this.data,
            id: 0,
            timeSlotId: parseInt(timeSlotId),
            dayOfWeekId: parseInt(dayId),
            groupId: currentScheduleType === 'group'
                ? parseInt(window.appData.groupId, 10)
                : this.data.groupId
        };

        try {
            const createdLessonResponse = await ScheduleAPI.createLesson(newLessonData);

            const lessonToStore = {
                ...createdLessonResponse,
                subjectTitle: createdLessonResponse.subjectTitle || this.data.subjectTitle,
                teacherFullName: createdLessonResponse.teacherFullName || this.data.teacherFullName,
                cabinetNumber: createdLessonResponse.cabinetNumber || this.data.cabinetNumber,
                buildingName: createdLessonResponse.buildingName || this.data.buildingName,
                groupName: createdLessonResponse.groupName || this.data.groupName,
                weekTypeId: createdLessonResponse.weekTypeId || this.data.weekTypeId
            };

            ScheduleStore.addLesson(lessonToStore, dayId, timeSlotId);

            const lessons = ScheduleStore.getLessonsInSlot(dayId, timeSlotId);
            if (typeof renderLessonsInSlot === 'function') {
                renderLessonsInSlot(slotElement, lessons, currentScheduleType);
            }

            showAlert('Занятие успешно вставлено!', 'success');
            this.clear();
        } catch (err) {
            console.error(err);
            showAlert(`Ошибка при вставке: ${err.message}`, 'danger');
        }
    },

    clear() {
        this.data = null;
        document.body.classList.remove('copy-mode');
        if (typeof pendingPasteSlot !== 'undefined' && pendingPasteSlot) {
            pendingPasteSlot.classList.remove('pending-paste');
            pendingPasteSlot = null;
        }
    }
};

document.addEventListener('keydown', (e) => {
    if (e.target.matches('input, textarea, [contenteditable]')) return;

    if ((e.ctrlKey || e.metaKey) && e.key === 'c') {
        if (window.currentSelectedLessonData) {
            e.preventDefault();
            LessonClipboard.copy(window.currentSelectedLessonData);
        }
    }

    if ((e.ctrlKey || e.metaKey) && e.key === 'v') {
        e.preventDefault();
        if (!LessonClipboard.data) {
            showAlert('Сначала скопируйте занятие (Ctrl+C)', 'warning');
            return;
        }
        if (pendingPasteSlot) {
            const timeId = parseInt(pendingPasteSlot.dataset.timeSlotId);
            const dayId = parseInt(pendingPasteSlot.dataset.dayOfWeekId);
            LessonClipboard.pasteToSlot(pendingPasteSlot, dayId, timeId);
        } else {
            showAlert('Кликните по ячейке, куда хотите вставить', 'info');
        }
    }

    if (e.key === 'Escape' && LessonClipboard.data) {
        LessonClipboard.clear();
        showAlert('Копирование отменено', 'info');
    }
});

document.addEventListener('click', (e) => {
    if (!LessonClipboard.data) return;
    if (e.target.closest('.copy-lesson-btn')) return;
    if (e.target.closest('.lesson-slot')) return;
    if (e.target.closest('.nav-link')) return;
    if (e.target.closest('.dropdown')) return;
    if (e.target.closest('button')) return;
    if (e.target.closest('select')) return;

    LessonClipboard.clear();
});
