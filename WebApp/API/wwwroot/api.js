const API_URL = "http://localhost:5000/api";

const api = {
    async saveFile(title, content) {
        const token = localStorage.getItem("token");
        const response = await fetch(`${API_URL}/files`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({ title, content })
        });

        return response.json();
    },

    async getFiles() {
        const token = localStorage.getItem("token");
        const response = await fetch(`${API_URL}/files`, {
            headers: { "Authorization": `Bearer ${token}` }
        });

        return response.json();
    },

    async getFile(id) {
        const token = localStorage.getItem("token");
        const response = await fetch(`${API_URL}/files/${id}`, {
            headers: { "Authorization": `Bearer ${token}` }
        });

        return response.json();
    },

    async downloadFile(id) {
        const token = localStorage.getItem("token");
        const response = await fetch(`${API_URL}/files/${id}`, {
            headers: { "Authorization": `Bearer ${token}` }
        });

        const fileData = await response.json();
        const blob = new Blob([fileData.content], { type: "text/markdown" });
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = `${fileData.title}.md`;
        link.click();
    },

    async setPermission(fileId, userId, permissionType) {
        const token = localStorage.getItem("token");
        const response = await fetch(`${API_URL}/files/${fileId}/permissions`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({ userId, permissionType })
        });

        return response.json();
    }
};
