document.addEventListener("DOMContentLoaded", async function () {
    const markdownInput = document.getElementById("markdownInput");
    const htmlOutput = document.getElementById("htmlOutput");
    const saveButton = document.getElementById("saveFile");
    const downloadButton = document.getElementById("downloadFile");
    const shareButton = document.getElementById("shareFile");
    const fileListContainer = document.getElementById("fileList");

    let currentFileId = null; // Для отслеживания текущего открытого файла

    markdownInput.addEventListener("input", () => {
        htmlOutput.innerHTML = marked(markdownInput.value);
    });

    saveButton.addEventListener("click", async () => {
        const content = markdownInput.value;

        if (currentFileId) {
            // Если редактируем существующий файл
            await api.updateFile(currentFileId, content);
            alert("Файл обновлён!");
        } else {
            const title = prompt("Введите название файла:");
            if (!title) return;
            await api.saveFile(title, content);
            alert("Файл сохранён!");
        }

        loadFiles();
    });

    downloadButton.addEventListener("click", async () => {
        if (!currentFileId) {
            alert("Сначала откройте файл!");
            return;
        }
        await api.downloadFile(currentFileId);
    });

    shareButton.addEventListener("click", async () => {
        if (!currentFileId) {
            alert("Сначала откройте файл!");
            return;
        }
        const userId = prompt("Введите ID пользователя:");
        const permissionType = prompt("Выберите роль: Editor (редактирование) или Viewer (просмотр)");

        if (userId && permissionType) {
            await api.setPermission(currentFileId, userId, permissionType);
            alert("Доступ назначен!");
        }
    });

    async function loadFiles() {
        const files = await api.getFiles();
        fileListContainer.innerHTML = "";

        files.forEach(file => {
            const fileElement = document.createElement("div");
            fileElement.classList.add("file-item");

            fileElement.innerHTML = `
                <p><strong>${file.title}</strong> (Владелец: ${file.Owner})</p>
                <button onclick="loadFile(${file.id})">📂 Открыть</button>
                <button onclick="api.downloadFile(${file.id})">⬇ Скачать</button>
                <button onclick="deleteFile(${file.id})">❌ Удалить</button>
            `;

            fileListContainer.appendChild(fileElement);
        });
    }

    async function loadFile(id) {
        const file = await api.getFile(id);
        currentFileId = file.id;
        markdownInput.value = file.content;
        htmlOutput.innerHTML = marked(file.content);
    }

    async function deleteFile(id) {
        if (confirm("Вы уверены, что хотите удалить этот файл?")) {
            await api.deleteFile(id);
            alert("Файл удалён!");
            loadFiles();
        }
    }

    loadFiles(); // Загружаем файлы при загрузке страницы
});
