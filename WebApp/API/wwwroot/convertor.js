// Утилита для получения параметра из URL
function getQueryParam(param) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(param);
}

// Утилита для выполнения HTTP-запросов с обработкой ошибок
async function fetchData(url, options = {}) {
    try {
        const response = await fetch(url, options);

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Ошибка ${response.status}: ${errorText}`);
        }

        return await response.json();
    } catch (error) {
        throw new Error(`Ошибка при выполнении запроса: ${error.message}`);
    }
}

// Загрузка документа при открытии страницы
async function loadDocument() {
    const inputField = document.getElementById('input_text_field');
    const documentId = getQueryParam('documentId');

    if (documentId) {
        try {
            const data = await fetchData(`http://localhost:5163/Document/Get/${documentId}`);
            inputField.value = data.text || '';
        } catch (error) {
            alert(`Произошла ошибка при загрузке документа: ${error.message}`);
        }
    }
}

// Логика конвертации Markdown в HTML
async function convertMarkdown() {
    const inputField = document.getElementById('input_text_field');
    const outputField = document.getElementById('output_text_field');
    const mdText = inputField.value;

    if (!mdText.trim()) {
        alert('Поле ввода пустое. Введите Markdown текст.');
        return;
    }

    try {
        const data = await fetchData('http://localhost:5163/Processor/Convert', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ mdText }),
        });

        outputField.innerHTML = data.html || '';
    } catch (error) {
        alert(`Произошла ошибка при конвертации: ${error.message}`);
    }
}

// Логика сохранения документа
async function saveDocument() {
    const inputField = document.getElementById('input_text_field');
    const documentId = getQueryParam('documentId');
    const text = inputField.value;

    if (!documentId) {
        alert('Не найден documentId в параметрах URL.');
        return;
    }

    if (!text.trim()) {
        alert('Поле ввода пустое. Невозможно сохранить пустой документ.');
        return;
    }

    try {
        await fetchData(`http://localhost:5163/Document/Update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ documentId, text }),
        });

        alert('Документ успешно сохранён!');
    } catch (error) {
        alert(`Произошла ошибка при сохранении: ${error.message}`);
    }
}

// Инициализация страницы
window.addEventListener('DOMContentLoaded', () => {
    loadDocument();

    document.getElementById('convert_button').addEventListener('click', convertMarkdown);
    document.getElementById('save_button').addEventListener('click', saveDocument);
});