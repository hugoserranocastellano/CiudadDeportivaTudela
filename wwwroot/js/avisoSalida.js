// Aviso de "seguro que quieres salir" para el cierre de pestaña/recarga del
// navegador. NavigationLock (Blazor) ya cubre la navegación interna dentro de
// la app; esto cubre lo que NavigationLock no puede: beforeunload es un
// evento del propio navegador, no del circuito de Blazor.
function onBeforeUnload(e) {
    e.preventDefault();
    e.returnValue = '';
}

export function activar() {
    window.addEventListener('beforeunload', onBeforeUnload);
}

export function desactivar() {
    window.removeEventListener('beforeunload', onBeforeUnload);
}
