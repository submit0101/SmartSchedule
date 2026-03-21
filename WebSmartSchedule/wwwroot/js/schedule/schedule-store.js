const ScheduleStore = {
    
    get data() { return window.scheduleData; },
    set data(val) { window.scheduleData = val; },

    init(data) {
        this.data = data;
    },
    findLesson(lessonId) {
        if (!this.data?.schedule) return null;
        const id = Number(lessonId);

        for (const day in this.data.schedule) {
            const daySlots = this.data.schedule[day] || [];
            for (const slot of daySlots) {
                if (slot.lessons) {
                    const lesson = slot.lessons.find(l => l.id === id);
                    if (lesson) return lesson;
                }
            }
        }
        return null;
    },


    getLessonsInSlot(dayId, timeId) {
        if (!this.data?.schedule) return [];
        const tId = Number(timeId);
        const daySchedule = this.data.schedule[dayId] || [];
        const slotData = daySchedule.find(s => s.timeSlotId == tId || s.lessons?.[0]?.timeSlotId == tId);
        return slotData?.lessons || [];
    },


    removeLesson(lessonId) {
        const id = Number(lessonId);
        if (!this.data?.schedule) return;

        for (const day in this.data.schedule) {
            const slots = this.data.schedule[day] || [];
            let slotIndexToRemove = -1;

            slots.forEach((slot, index) => {
                if (slot.lessons) {
                    const initialLength = slot.lessons.length;
                    slot.lessons = slot.lessons.filter(l => l.id !== id);
                    if (slot.lessons.length === 0 && initialLength > 0) {
                        slotIndexToRemove = index;
                    }
                }
            });
            if (slotIndexToRemove !== -1) {
                slots.splice(slotIndexToRemove, 1);
            }
        }
    },

    
    addLesson(lesson, dayId, timeId) {
        if (!this.data.schedule) this.data.schedule = {};
        if (!this.data.schedule[dayId]) {
            this.data.schedule[dayId] = [];
        }
        const tId = Number(timeId);
        let slot = this.data.schedule[dayId].find(s =>
            s.timeSlotId == tId ||
            (s.lessons && s.lessons.length > 0 && s.lessons[0].timeSlotId == tId)
        );
        if (!slot) {
            slot = { timeSlotId: tId, lessons: [] };
            this.data.schedule[dayId].push(slot);
        }
        lesson.timeSlotId = tId;
        lesson.dayOfWeekId = Number(dayId);
        slot.lessons.push(lesson);
    },

    updateLessonData(updatedLesson) {
        const existing = this.findLesson(updatedLesson.id);
        if (existing) {
            Object.assign(existing, updatedLesson);
        }
    }
};