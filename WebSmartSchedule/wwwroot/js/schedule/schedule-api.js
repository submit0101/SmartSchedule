const ScheduleAPI = {
    baseUrl: window.AppConfig.apiBaseUrl,

    async getGroupSchedule(groupId) {
        const res = await fetch(`${this.baseUrl}/lesson/group/${groupId}/structured`);
        if (!res.ok) throw new Error('Ошибка загрузки расписания группы');
        return await res.json();
    },

    async getTeacherSchedule(teacherId) {
        const res = await fetch(`${this.baseUrl}/lesson/teacher/${teacherId}/structured`);
        if (!res.ok) throw new Error('Ошибка загрузки расписания преподавателя');
        return await res.json();
    },

    async checkConflict(lessonId, dayId, timeId) {
        try {
            const res = await fetch(`${this.baseUrl}/Lesson/check-conflict?lessonId=${lessonId}&targetDayId=${dayId}&targetTimeId=${timeId}`);
            if (!res.ok) return { isWeekConflict: true, isTeacherBusy: true, isCabinetBusy: true };
            return await res.json();
        } catch (e) {
            console.error(e);
            return { isWeekConflict: true, isTeacherBusy: true, isCabinetBusy: true };
        }
    },

    async getFreeCabinets(dayId, timeId, weekId) {
     
        const params = new URLSearchParams({
            day: dayId,          
            timeslot: timeId,    
            weektype: weekId    
        });

        const res = await fetch(`${this.baseUrl}/Lesson/available?${params}`);
        if (!res.ok) throw new Error('Не удалось загрузить свободные кабинеты');
        return await res.json();
    },

    async moveLesson(lessonId, data) {
        const res = await fetch(`${this.baseUrl}/lesson/${lessonId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (!res.ok) throw new Error(await res.text());
        return true;
    },
    async getAllGroups() {
      
        const res = await fetch(`${this.baseUrl}/Group/all`);
        if (!res.ok) throw new Error('Ошибка загрузки списка групп');
        return await res.json();
    },
    async createLesson(data) {
        const res = await fetch(`${this.baseUrl}/lesson`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (!res.ok) throw new Error(await res.text());
        return await res.json(); 
    },

    async updateLesson(lessonId, data) {
        const res = await fetch(`${this.baseUrl}/lesson/${lessonId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (!res.ok) throw new Error(await res.text());
        return true;
    },

    async deleteLesson(lessonId) {
        const res = await fetch(`${this.baseUrl}/lesson/${lessonId}`, { method: 'DELETE' });
        if (!res.ok) throw new Error(await res.text());
        return true;
    },
    async updateLessonsBatch(lessonsArray) {
        const res = await fetch(`${this.baseUrl}/lesson/batch`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(lessonsArray)
        });
        if (!res.ok) throw new Error(await res.text());
        return true;
    }
};