function getSessionStorage() {
    return JSON.parse(window.sessionStorage.getItem('selectedFiles')) || [];
}
function setSessionStorage(files) {
    window.sessionStorage.setItem('selectedFiles', JSON.stringify(files));
}
function onLoad() {
    
    $("#file-uploader").fileinput({
        'language' : 'pl',
        'theme' : 'fas',
        'showUpload' : true,
        'uploadUrl' : '/cloud/upload',
        'uploadExtraData' : {
            'path' : window.location.pathname
        }
    }).on('fileuploaded', () => location.reload());
    
    let files = getSessionStorage();
    const element = document.getElementById('selectedAmount');
    if(element !== null) {
        element.textContent = files.length.toString();
    }
    files.forEach(x => {
        const file = document.getElementById(x+'-checkbox');
        if(file !== null) {
            file.checked = true;
        }
    });
}

function selectFile(id) {
    let files = getSessionStorage();
    if(!files.includes(id)) {
        files.push(id);
    }else {
        files = files.filter(x=>x!==id);
    }
    document.getElementById('selectedAmount').textContent = files.length.toString();
    setSessionStorage(files);
}

async function createDirectory() {
    const name = prompt('Wpisz nazwę nowego folderu', 'Nowy folder');
    if(name !== null) {
        await fetch(`/cloud/create-directory?path=${window.location.pathname}&name=${name}`, {
            method: 'POST'
        }).then(response => {
            if(response.ok) {
                location.reload();
            }else {
                alert('Wystąpił błąd podczas tworzenia nowego folderu!!!');
            }
        });
    }
}

async function removeFile(id) {
    if(confirm('Czy jesteś pewnien że chcesz usunąć ten plik?')) {
        await fetch(`/cloud/delete-file?id=${id}`, {
            method: 'DELETE'
        }).then(response => {
            if(response.ok) {
                location.reload();
            }else {
                alert('Wystąpił błąd podczas usuwania pliku!!!');
            }
        })
    }
}

async function removeFiles() {
    const ids = getSessionStorage();
    if(confirm('Czy jesteś pewnien że chcesz usunąć te pliki?')) {
        for (const id of ids) {
            await fetch(`/cloud/delete-file?id=${id}`, {
                method: 'DELETE'
            }).then(response => {
                if(!response.ok) {
                    alert('Wystąpił błąd podczas usuwania pliku!!!');
                }
            })
        }
    }
    setSessionStorage([]);
    location.reload();
}
async function removeFolder(id) {
    if(confirm('Czy jesteś pewnien że chcesz usunąć ten folder?')) {
        await fetch(`/cloud/delete-folder?id=${id}`, {
            method: 'DELETE'
        }).then(response => {
            if(response.ok) {
                location.reload();
            }else {
                alert('Wystąpił błąd podczas usuwania pliku (czy usunąłeś wszystkie pliki z folderu?)!!!');
            }
        })
    }
}
async function moveFiles() {
    const path = window.location.pathname;
    const ids = getSessionStorage();
    if(confirm('Czy jesteś pewnien że chcesz przenieść te pliki?')) {
        for (const id of ids) {
            await fetch(`/cloud/move?id=${id}&newPath=${path}`, {
                method: 'PUT'
            }).then(async response => {
                if(!response.ok) {
                    alert('Wystąpił błąd podczas przenoszenia pliku!!!');
                }
            })
        }
    }
    setSessionStorage([]);
    location.reload();
}

async function renameFile(id, oldName) {
    const name = prompt('Wpisz nową nazwę pliku: (Uwaga pamiętaj też o rozszerzeniu!)', oldName);
    if(name !== null) {
        await fetch(`/cloud/rename?id=${id}&newName=${name}`, {
            method: 'PUT'
        }).then(response => {
            if(response.ok) {
                location.reload();
            }else {
                alert('Wystąpił błąd podczas zmiany nazwy pliku!!!');
            }
        });
    }
}

async function share(id) {
    await fetch(`/cloud/share?id=${id}`, {
        method: 'PUT'
    }).then(response => {
        if(response.ok) {
            const element = document.getElementById(`${id}-share`);
            if(element.textContent === 'Udostępniony') {
                element.textContent = 'Nieudostępniony';
            }else {
                element.textContent = 'Udostępniony';
            }
        }else {
            alert('Wystąpił błąd podczas zmiany statusu udostępnienia!');
        }
    })
}

onLoad();