/**
 * Универсальная функция загрузки Excel
 * @param {Event} event - Событие input type="file"
 * @param {string} endpoint - Относительный путь (например, '/Teacher/import')
 */
async function handleExcelUpload(event, endpoint) {
    const file = event.target.files[0];
    if (!file) return;

    event.target.value = '';

    const formData = new FormData();
    formData.append('file', file);

    try {
        const fullUrl = `${window.AppConfig.apiBaseUrl}${endpoint}`;

        const response = await fetch(fullUrl, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const result = await response.json();
            alert(result.message);
            location.reload();
        } else {
            const errorText = await response.text();
            alert('Ошибка импорта: ' + errorText);
        }
    } catch (error) {
        console.error('Ошибка:', error);
        alert('Не удалось связаться с сервером');
    }
}