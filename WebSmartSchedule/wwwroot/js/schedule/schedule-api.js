const ScheduleAPI = {
    baseUrl: 'http://localhost:5062/api',

    getHeaders() {
        const headers = { 'Content-Type': 'application/json' };

        
        if (window.appData && window.appData.token) {
            console.log("Токен найден, прикрепляем к запросу...");
            headers['Authorization'] = 'Bearer ' + window.appData.token;
        } else {
            console.warn("ВНИМАНИЕ: Токен не найден в window.appData.token!");
        }

        return headers;
    },

    async getGroupSchedule(groupId) {
        const res = await fetch(`${this.baseUrl}/lesson/group/${groupId}/structured`, {
            headers: this.getHeaders() 
        });
        if (!res.ok) throw new Error('Ошибка загрузки расписания группы');
        return await res.json();
    },

    async getTeacherSchedule(teacherId) {
        const res = await fetch(`${this.baseUrl}/lesson/teacher/${teacherId}/structured`, {
            headers: this.getHeaders() 
        });
        if (!res.ok) throw new Error('Ошибка загрузки расписания преподавателя');
        return await res.json();
    },

    async checkConflict(lessonId, dayId, timeId) {
        try {
            const res = await fetch(`${this.baseUrl}/Lesson/check-conflict?lessonId=${lessonId}&targetDayId=${dayId}&targetTimeId=${timeId}`, {
                headers: this.getHeaders() 
            });
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

        const res = await fetch(`${this.baseUrl}/Lesson/available?${params}`, {
            headers: this.getHeaders() 
        });
        if (!res.ok) throw new Error('Не удалось загрузить свободные кабинеты');
        return await res.json();
    },

    async moveLesson(lessonId, data) {
        const res = await fetch(`${this.baseUrl}/lesson/${lessonId}`, {
            method: 'PUT',
            headers: this.getHeaders(), 
            body: JSON.stringify(data)
        });
        if (!res.ok) throw new Error(await res.text());
        return true;
    },

    async getAllGroups() {
        const res = await fetch(`${this.baseUrl}/Group/all`, {
            headers: this.getHeaders() 
        });
        if (!res.ok) throw new Error('Ошибка загрузки списка групп');
        return await res.json();
    },

    async createLesson(data) {
        const res = await fetch(`${this.baseUrl}/lesson`, {
            method: 'POST',
            headers: this.getHeaders(), 
            body: JSON.stringify(data)
        });
        if (!res.ok) throw new Error(await res.text());
        return await res.json();
    },

    async updateLesson(lessonId, data) {
        const res = await fetch(`${this.baseUrl}/lesson/${lessonId}`, {
            method: 'PUT',
            headers: this.getHeaders(), 
            body: JSON.stringify(data)
        });
        if (!res.ok) throw new Error(await res.text());
        return true;
    },

    async deleteLesson(lessonId) {
        const res = await fetch(`${this.baseUrl}/lesson/${lessonId}`, {
            method: 'DELETE',
            headers: this.getHeaders() 
        });
        if (!res.ok) throw new Error(await res.text());
        return true;
    },
    async updateLessonsBatch(lessonsArray) {
        const res = await fetch(`${this.baseUrl}/lesson/batch`, {
            method: 'PUT',
            headers: this.getHeaders(),
            body: JSON.stringify(lessonsArray)
        });
        if (!res.ok) throw new Error(await res.text());
        return true;
    }
};