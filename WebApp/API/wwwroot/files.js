document.addEventListener("DOMContentLoaded", function () {
    loadFiles();
    loadUsers();
});

function loadFiles() {
    fetch("/api/files")
        .then(response => response.json())
        .then(files => {
            const fileList = document.getElementById("file-list");
            const fileSelect = document.getElementById("file");
            fileList.innerHTML = "";
            fileSelect.innerHTML = "";

            files.forEach(file => {
                const row = document.createElement("tr");
                row.innerHTML = `
                    <td>${file.name}</td>
                    <td>${file.size} KB</td>
                    <td>${new Date(file.uploadDate).toLocaleString()}</td>
                    <td>
                        <button onclick="downloadFile('${file.id}')">Скачать</button>
                        <button onclick="deleteFile('${file.id}')">Удалить</button>
                    </td>
                `;
                fileList.appendChild(row);

                // Добавляем файл в выпадающий список
                const option = document.createElement("option");
                option.value = file.id;
                option.textContent = file.name;
                fileSelect.appendChild(option);
            });
        });
}

function loadUsers() {
    fetch("/api/users")
        .then(response => response.json())
        .then(users => {
            const userSelect = document.getElementById("user");
            userSelect.innerHTML = "";

            users.forEach(user => {
                const option = document.createElement("option");
                option.value = user.id;
                option.textContent = user.email;
                userSelect.appendChild(option);
            });
        });
}

function downloadFile(fileId) {
    window.location.href = `/api/files/${fileId}/download`;
}

function deleteFile(fileId) {
    fetch(`/api/files/${fileId}`, { method: "DELETE" })
        .then(() => loadFiles());
}

// Назначение прав
document.getElementById("permissions-form").addEventListener("submit", function (e) {
    e.preventDefault();

    const userId = document.getElementById("user").value;
    const fileId = document.getElementById("file").value;
    const role = document.getElementById("role").value;

    fetch("/api/permissions", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ userId, fileId, role })
    }).then(() => alert("Права назначены!"));
});
