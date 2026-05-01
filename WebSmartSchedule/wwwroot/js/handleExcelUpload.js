/**
 * Универсальная функция загрузки и предпросмотра Excel
 * @param {Event} event - Событие input type="file"
 * @param {string} endpoint - Относительный путь для отправки (напр. '/api/Teacher/import')
 * @param {string} entityName - Название для заголовка (напр. 'Преподаватели')
 */
async function handleExcelUpload(event, endpoint, entityName = "данных") {
    const file = event.target.files[0];
    if (!file) return;

    event.target.value = '';

    try {
        // Меняем заголовок модалки
        document.getElementById('importPreviewTitle').innerHTML = `<i class="bi bi-file-earmark-excel me-2"></i>Предпросмотр: ${entityName}`;

        // Читаем файл
        const reader = new FileReader();
        reader.readAsArrayBuffer(file);

        reader.onload = async (e) => {
            const buffer = e.target.result;
            const workbook = new ExcelJS.Workbook();
            await workbook.xlsx.load(buffer);

            const worksheet = workbook.worksheets[0];
            if (!worksheet || worksheet.rowCount === 0) {
                alert("Файл пуст или лист не найден.");
                return;
            }

            const thead = document.getElementById('previewThead');
            const tbody = document.getElementById('previewTbody');
            thead.innerHTML = '';
            tbody.innerHTML = '';

            const headerRow = worksheet.getRow(1);
            const colCount = worksheet.actualColumnCount || headerRow.cellCount;

            let trHead = document.createElement('tr');
            for (let i = 1; i <= colCount; i++) {
                let cellValue = headerRow.getCell(i).value || `Колонка ${i}`;
                trHead.innerHTML += `<th class="fw-bold text-uppercase text-muted" style="background: #f8fafc;">${cellValue}</th>`;
            }
            thead.appendChild(trHead);

            const maxRowsPreview = Math.min(worksheet.rowCount, 11);
            for (let r = 2; r <= maxRowsPreview; r++) {
                const row = worksheet.getRow(r);
                let trBody = document.createElement('tr');

                for (let c = 1; c <= colCount; c++) {
                    let cellVal = row.getCell(c).value;
                    if (cellVal && typeof cellVal === 'object') {
                        cellVal = cellVal.result || cellVal.text || cellVal;
                    }
                    trBody.innerHTML += `<td>${cellVal || '<span class="text-muted opacity-50">—</span>'}</td>`;
                }
                tbody.appendChild(trBody);
            }

            const confirmBtn = document.getElementById('confirmImportBtn');

            const newBtn = confirmBtn.cloneNode(true);
            confirmBtn.parentNode.replaceChild(newBtn, confirmBtn);

            newBtn.onclick = () => executeUpload(file, endpoint, newBtn);

            openModal('importPreviewModal');
        };

    } catch (error) {
        console.error('Ошибка чтения файла:', error);
        alert('Не удалось прочитать Excel файл. Проверьте формат.');
    }
}

/**
 * Функция отправки файла на сервер
 */
async function executeUpload(file, endpoint, btnElement) {
    const originalText = btnElement.innerHTML;
    btnElement.disabled = true;
    btnElement.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Загрузка...';

    const formData = new FormData();
    formData.append('file', file);

    try {
        const baseUrl = window.AppConfig?.apiBaseUrl || '';
        const fullUrl = `${baseUrl}${endpoint}`;

        const response = await fetch(fullUrl, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const result = await response.json();
            alert(result.message || 'Импорт успешно завершен!');
            location.reload();
        } else {
            const errorText = await response.text();
            alert('Ошибка импорта на сервере: ' + errorText);
            closeModal('importPreviewModal');
        }
    } catch (error) {
        console.error('Ошибка связи:', error);
        alert('Не удалось отправить файл на сервер');
    } finally {
        btnElement.disabled = false;
        btnElement.innerHTML = originalText;
    }
}