document.addEventListener("DOMContentLoaded", async function () {
    const markdownInput = document.getElementById("markdownInput");
    const htmlOutput = document.getElementById("htmlOutput");
    const saveButton = document.getElementById("saveFile");
    const downloadButton = document.getElementById("downloadFile");
    const shareButton = document.getElementById("shareFile");
    const fileListContainer = document.getElementById("fileList");

    markdownInput.addEventListener("input", () => {
        htmlOutput.innerHTML = marked(markdownInput.value);
    });

    saveButton.addEventListener("click", async () => {
        const content = markdownInput.value;
        const title = prompt("Введите название файла:");

        if (!title) return;

        await api.saveFile(title, content);
        alert("Файл сохранён!");
        loadFiles();
    });

    downloadButton.addEventListener("click", async () => {
        const fileId = prompt("Введите ID файла для скачивания:");
        if (fileId) await api.downloadFile(fileId);
    });

    shareButton.addEventListener("click", async () => {
        const fileId = prompt("Введите ID файла:");
        const userId = prompt("Введите ID пользователя:");
        const permissionType = prompt("Выберите роль: Editor (редактирование) или Viewer (просмотр)");

        if (fileId && userId && permissionType) {
            await api.setPermission(fileId, userId, permissionType);
            alert("Доступ назначен!");
        }
    });

    async function loadFiles() {
        const files = await api.getFiles();
        fileListContainer.innerHTML = "";

        files.forEach(file => {
            const fileElement = document.createElement("div");
            fileElement.innerHTML = `
                <p><strong>${file.title}</strong> (Владелец: ${file.Owner})</p>
                <button onclick="loadFile(${file.id})">📂 Открыть</button>
                <button onclick="api.downloadFile(${file.id})">⬇ Скачать</button>
            `;
            fileListContainer.appendChild(fileElement);
        });
    }

    async function loadFile(id) {
        const file = await api.getFile(id);
        markdownInput.value = file.content;
        htmlOutput.innerHTML = marked(file.content);
    }

    loadFiles();
});
