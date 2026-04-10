class HistoryManager {
    constructor(api) {
        this.stack = [];
        this.api = api;
    }

    // Записываем шаг (принимает ID и старый объект Lesson)
    recordMove(lessonId, oldLessonData) {
        this.stack.push({
            lessonId: lessonId,
            oldLessonData: oldLessonData
        });

        if (this.stack.length > 20) this.stack.shift();
        console.log(`Записано в историю. Доступно отмен: ${this.stack.length}`);
    }

    async undo() {
        if (this.stack.length === 0) return;

        const lastAction = this.stack.pop();

        // Текущее состояние пары (чтобы знать, откуда она улетает)
        const currentLesson = ScheduleStore.findLesson(lastAction.lessonId);
        if (!currentLesson) return;

        const currentDayId = currentLesson.dayOfWeekId;
        const currentTimeId = currentLesson.timeSlotId;
        const oldDayId = lastAction.oldLessonData.dayOfWeekId;
        const oldTimeId = lastAction.oldLessonData.timeSlotId;

        // Собираем данные для сервера (как они были раньше)
        const undoData = {
            Id: lastAction.lessonId,
            GroupId: lastAction.oldLessonData.groupId,
            SubjectId: lastAction.oldLessonData.subjectId,
            TeacherId: lastAction.oldLessonData.teacherId,
            CabinetId: lastAction.oldLessonData.cabinetId,
            TimeSlotId: oldTimeId,
            DayOfWeekId: oldDayId,
            WeekTypeId: lastAction.oldLessonData.weekTypeId,
            Subgroup: lastAction.oldLessonData.subgroup
        };

        try {
            // 1. Отправляем старые данные на сервер
            if (this.api.moveLesson) {
                await this.api.moveLesson(lastAction.lessonId, undoData);
            } else {
                await this.api.updateLesson(lastAction.lessonId, undoData);
            }

            // 2. Обновляем твой локальный стор
            ScheduleStore.removeLesson(lastAction.lessonId);
            ScheduleStore.addLesson(lastAction.oldLessonData, oldDayId, oldTimeId);

            // 3. Вызываем твои функции перерисовки ячеек!
            refreshSingleSlot(currentDayId, currentTimeId); // Очищаем ячейку, откуда ушла пара
            refreshSingleSlot(oldDayId, oldTimeId);         // Рисуем пару там, куда она вернулась

            // Красивая подсветка (необязательно)
            setTimeout(() => {
                const card = document.getElementById(lastAction.lessonId);
                if (card) {
                    card.classList.add('filtered-highlight');
                    setTimeout(() => card.classList.remove('filtered-highlight'), 600);
                }
            }, 100);

        } catch (error) {
            console.error("Ошибка при отмене:", error);
            this.stack.push(lastAction); // Возвращаем в стек, если сервер упал
            showAlert("Не удалось отменить действие", "danger");
        }
    }
}

// Инициализация
const appHistory = new HistoryManager(ScheduleAPI);

// Перехват Ctrl+Z
document.addEventListener('keydown', function (event) {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'z') {
        if (event.target.tagName === 'INPUT' || event.target.tagName === 'TEXTAREA') return;
        event.preventDefault();
        appHistory.undo();
    }
});