// === Модальное окно ===
"use strict";
function openModal(id) {
    const modal = document.getElementById(id);
    if (!modal) {
        console.error(`Модалка с id="${id}" не найдена`);
        return;
    }
    modal.style.display = 'flex';
    document.body.style.overflow = 'hidden';
}

function closeModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;
    modal.style.display = 'none';
    document.body.style.overflow = '';
}


