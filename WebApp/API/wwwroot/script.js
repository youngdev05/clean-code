const DOMUtils = {
    createElement: (tag, className, textContent) => {
        const element = document.createElement(tag);
        if (className) element.className = className;
        if (textContent) element.textContent = textContent;
        return element;
    },
    createInput: (type, name, placeholder) => {
        const input = DOMUtils.createElement('input');
        input.type = type;
        input.name = name;
        input.placeholder = placeholder;
        return input;
    },
    createButton: (text, onClick, className) => {
        const button = DOMUtils.createElement('button', className, text);
        button.type = 'button';
        if (onClick) button.addEventListener('click', onClick);
        return button;
    },
    createForm: (className, onSubmit) => {
        const form = DOMUtils.createElement('form', className);
        if (onSubmit) form.addEventListener('submit', onSubmit);
        return form;
    },
};

// Утилиты для работы с сервером
const ApiUtils = {
    fetchData: async (url, options = {}) => {
        try {
            const response = await fetch(url, {
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                ...options,
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText);
            }

            return await response.json();
        } catch (error) {
            throw new Error(`Ошибка при выполнении запроса: ${error.message}`);
        }
    },
};

// Управление документами
const DocumentManager = {
    userDocs: [],
    sharedDocs: [],

    // Загрузка документов
    loadDocuments: async () => {
        try {
            DocumentManager.userDocs = await ApiUtils.fetchData('http://localhost:5250/Document/My');
            DocumentManager.sharedDocs = await ApiUtils.fetchData('http://localhost:5250/Document/Shared');
            DocumentManager.updateDocumentLists();
        } catch (error) {
            console.error('Ошибка при загрузке документов:', error);
            alert('Не удалось загрузить документы.');
        }
    },

    // Обновление списков документов
    updateDocumentLists: () => {
        const personalContainer = document.getElementById('personal-documents');
        const availableContainer = document.getElementById('available-documents');

        personalContainer.innerHTML = '';
        availableContainer.innerHTML = '';

        DocumentManager.userDocs.forEach(doc => {
            personalContainer.appendChild(DocumentManager.createDocumentCard(doc, 'personal'));
        });

        DocumentManager.sharedDocs.forEach(doc => {
            availableContainer.appendChild(DocumentManager.createDocumentCard(doc, 'shared'));
        });
    },

    // Создание карточки документа
    createDocumentCard: (doc, type) => {
        const card = DOMUtils.createElement('div', 'document-card');
        card.id = `${type}_${doc.id}`;

        const info = DOMUtils.createElement('div', 'document-info');
        info.appendChild(DOMUtils.createElement('span', 'document-name', doc.name));
        info.appendChild(DOMUtils.createElement('span', 'document-author', `Автор: ${doc.authorName}`));

        const actions = DOMUtils.createElement('div', 'document-actions');
        actions.appendChild(DOMUtils.createButton('Доступ', () => DocumentManager.showAccessForm(doc.id), 'access-button'));
        actions.appendChild(DOMUtils.createButton('Открыть', () => window.location.href = `/Page/Convertor?documentId=${doc.id}`, 'open-button'));
        actions.appendChild(DOMUtils.createButton('Удалить', () => DocumentManager.deleteDocument(doc.id), 'delete-button'));
        actions.appendChild(DOMUtils.createButton('Переименовать', () => DocumentManager.showRenameForm(doc.id), 'rename-button'));

        card.appendChild(info);
        card.appendChild(actions);
        return card;
    },

    // Удаление документа
    deleteDocument: async (documentId) => {
        try {
            await ApiUtils.fetchData('http://localhost:5250/Document/Delete', {
                method: 'DELETE',
                body: JSON.stringify({ documentId }),
            });

            document.getElementById(`personal_${documentId}`)?.remove();
            document.getElementById(`shared_${documentId}`)?.remove();
            alert('Документ успешно удалён!');
        } catch (error) {
            alert(`Ошибка при удалении документа: ${error.message}`);
        }
    },

    // Переименование документа
    showRenameForm: (documentId) => {
        FormManager.showForm({
            title: 'Переименовать документ',
            fields: [{ type: 'text', name: 'newName', placeholder: 'Новое имя документа' }],
            onSubmit: async (data) => {
                try {
                    await ApiUtils.fetchData('/Document/Rename', {
                        method: 'PATCH',
                        body: JSON.stringify({ documentId, text: data.newName }),
                    });

                    alert('Документ успешно переименован!');
                    DocumentManager.updateDocumentName(documentId, data.newName);
                } catch (error) {
                    alert(`Ошибка: ${error.message}`);
                }
            },
        });
    },

    // Обновление имени документа в интерфейсе
    updateDocumentName: (documentId, newName) => {
        const card = document.getElementById(`personal_${documentId}`) || document.getElementById(`shared_${documentId}`);
        if (card) {
            const nameElement = card.querySelector('.document-name');
            if (nameElement) nameElement.textContent = newName;
        }
    },

    // Управление доступом
    showAccessForm: (documentId) => {
        FormManager.showForm({
            title: 'Управление доступом',
            fields: [
                { type: 'text', name: 'userLogin', placeholder: 'Логин пользователя' },
                {
                    type: 'select',
                    name: 'accessOptions',
                    options: [
                        { value: 'grant', text: 'Разрешить доступ' },
                        { value: 'deny', text: 'Запретить доступ' },
                    ],
                },
            ],
            onSubmit: async (data) => {
                try {
                    const url = data.accessOptions === 'grant' ? '/Access/Give' : '/Access/Remove';
                    await ApiUtils.fetchData(url, {
                        method: 'POST',
                        body: JSON.stringify({ userLogin: data.userLogin, documentId }),
                    });

                    alert('Действие выполнено успешно!');
                } catch (error) {
                    alert(`Ошибка: ${error.message}`);
                }
            },
        });
    },
};

// Управление формами
const FormManager = {
    showForm: (config) => {
        const overlay = DOMUtils.createElement('div', 'overlay');
        const popup = DOMUtils.createElement('div', 'popup');

        const closeButton = DOMUtils.createButton('×', () => document.body.removeChild(overlay), 'close-button');
        popup.appendChild(closeButton);

        const form = DOMUtils.createForm(config.title.toLowerCase().replace(/ /g, '-') + '-form', (event) => {
            event.preventDefault();
            const formData = new FormData(event.target);
            const data = Object.fromEntries(formData.entries());
            config.onSubmit(data);
            document.body.removeChild(overlay);
        });

        config.fields.forEach(field => {
            if (field.type === 'select') {
                const select = DOMUtils.createElement('select');
                select.name = field.name;
                field.options.forEach(option => {
                    const optionElement = DOMUtils.createElement('option');
                    optionElement.value = option.value;
                    optionElement.textContent = option.text;
                    select.appendChild(optionElement);
                });
                form.appendChild(select);
            } else {
                form.appendChild(DOMUtils.createInput(field.type, field.name, field.placeholder));
            }
        });

        form.appendChild(DOMUtils.createButton(config.title.split(' ')[0], null, 'submit-button'));
        popup.appendChild(form);
        overlay.appendChild(popup);
        document.body.appendChild(overlay);
    },
};

// Инициализация
document.addEventListener('DOMContentLoaded', async () => {
    await DocumentManager.loadDocuments();
    document.getElementById('my-documents-btn').addEventListener('click', () => DocumentManager.showPersonalDocuments());
    document.getElementById('available-documents-btn').addEventListener('click', () => DocumentManager.showAvailableDocuments());
    document.getElementById('new-document-button').addEventListener('click', () => FormManager.showForm({
        title: 'Создать документ',
        fields: [{ type: 'text', name: 'name', placeholder: 'Имя документа' }],
        onSubmit: async (data) => {
            try {
                const response = await ApiUtils.fetchData('http://localhost:5250/Document/New', {
                    method: 'POST',
                    body: JSON.stringify(data),
                });

                const newDocumentId = await response.text();
                window.location.href = `/Page/Convertor?documentId=${newDocumentId.replaceAll('"', '')}`;
            } catch (error) {
                alert(`Ошибка создания документа: ${error.message}`);
            }
        },
    }));
});