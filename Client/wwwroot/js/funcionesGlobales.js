window.manageLocalStorage = {
    setLocalStorage: (clave, valor) => {
        window.localStorage.setItem(clave, valor);
    },
    getLocalStorage: (clave) => {
        return window.localStorage.getItem(clave);
    }
}