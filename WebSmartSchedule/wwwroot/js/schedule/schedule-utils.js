// wwwroot/js/schedule/schedule-utils.js

const WeekType = {
    NUMERATOR: 1,   // Числитель (ID 1)
    DENOMINATOR: 2, // Знаменатель (ID 2)
    FULL: 3         // Целая (ID 3)
};
let selectedWeekType = "all";
let currentLessonId = null;
let isLoading = false;
let resizeTimer = null;
let pendingPasteSlot = null;    
let selectedTimeSlotFilter = null;
let currentScheduleType = 'group';
let currentTeacherId = null;

window.appData = window.appData || {};
window.currentSelectedLessonData = null;


function analyzeSlotState(lessons) {
    if (!lessons || lessons.length === 0) {
        return { isFull: false, hasNum: false, hasDen: false, isWhole: false };
    }
    const isWhole = lessons.some(l => l.weekTypeId == WeekType.FULL);
    const hasNum = lessons.some(l => l.weekTypeId == WeekType.NUMERATOR);
    const hasDen = lessons.some(l => l.weekTypeId == WeekType.DENOMINATOR);
    const isFull = isWhole || (hasNum && hasDen);

    return { isFull, hasNum, hasDen, isWhole };
}

function formatTime(time) {
    return time ? time.slice(0, 5) : "—";
}

function showAlert(message, type = 'info') {
    const alertContainer = document.getElementById('alert-container');
    if (!alertContainer) return;

    const alert = document.createElement('div');
    alert.className = `alert alert-${type} alert-dismissible fade show`;
    alert.role = 'alert';
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    alertContainer.appendChild(alert);
    setTimeout(() => {
        if (alert.parentNode) {
            alert.classList.remove('show');
            setTimeout(() => alert.remove(), 150);
        }
    }, 5000);
}

function openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = 'block';
        setTimeout(() => modal.classList.add("show"), 10);
        document.body.classList.add('modal-open');
        let backdrop = document.querySelector('.modal-backdrop');
        if (!backdrop) {
            backdrop = document.createElement('div');
            backdrop.className = 'modal-backdrop fade show';
            document.body.appendChild(backdrop);
        }
    }
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (!modal) return;

    const dialog = modal.querySelector('.modal-dialog');
    const form = modal.querySelector('form');
    if (form) {
        form.reset();
        form.classList.remove('was-validated');
    }

    modal.classList.remove("show");
    if (typeof pendingPasteSlot !== 'undefined' && pendingPasteSlot) {
        const slotToClear = pendingPasteSlot;
        setTimeout(() => {
            if (slotToClear) {
                slotToClear.classList.remove('pending-paste');
            }
        }, 300);
        pendingPasteSlot = null;
    }
    

    const backdrop = document.querySelector('.modal-backdrop');
    if (backdrop) backdrop.remove();
    document.body.classList.remove('modal-open');

    setTimeout(() => {
        modal.style.display = 'none';
        if (dialog) dialog.style.animation = '';
    }, 300);
}

function showScheduleControls() {
    const filterButtons = document.getElementById('filterButtons');
    const groupSelector = document.getElementById('groupSelector');
    if (filterButtons) filterButtons.classList.remove('d-none');
    if (groupSelector) groupSelector.classList.remove('d-none');
}

function hideScheduleControls() {
    const filterButtons = document.getElementById('filterButtons');
    const groupSelector = document.getElementById('groupSelector');
    if (filterButtons) filterButtons.classList.add('d-none');
    if (groupSelector) groupSelector.classList.add('d-none');
}

function toggleCustomFilters() {
    const wrapper = document.getElementById('customFilterWrapper');
    const isOpen = wrapper.classList.toggle('open');
    wrapper.style.maxHeight = isOpen ? wrapper.scrollHeight + "px" : null;
    document.querySelectorAll('.filter-btn-text').forEach(el => el.textContent = isOpen ? "Скрыть фильтры" : "Показать фильтры");
    document.querySelectorAll('.transition-icon').forEach(el => el.style.transform = isOpen ? "rotate(180deg)" : "rotate(0deg)");
}