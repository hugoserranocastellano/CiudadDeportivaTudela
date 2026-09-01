// Editor de recorte cuadrado (mover + zoom) para las fotos que se suben a
// Supabase Storage. Un Map por canvasId permite tener varias instancias del
// componente FotoCropper en la misma página sin pisarse el estado.
const estados = new Map();

const ZOOM_MAX = 4;

function distancia(p1, p2) {
    const dx = p1.clientX - p2.clientX;
    const dy = p1.clientY - p2.clientY;
    return Math.sqrt(dx * dx + dy * dy);
}

function limitarPan(estado) {
    // No dejar que la imagen deje huecos en blanco dentro del cuadrado: el
    // offset máximo es la mitad de lo que la imagen escalada sobresale del canvas.
    const anchoEscalado = estado.imagen.width * estado.escala;
    const altoEscalado = estado.imagen.height * estado.escala;
    const maxX = Math.max(0, (anchoEscalado - estado.tamano) / 2);
    const maxY = Math.max(0, (altoEscalado - estado.tamano) / 2);

    estado.offsetX = Math.min(maxX, Math.max(-maxX, estado.offsetX));
    estado.offsetY = Math.min(maxY, Math.max(-maxY, estado.offsetY));
}

function dibujar(estado) {
    const ctx = estado.canvas.getContext('2d');
    ctx.clearRect(0, 0, estado.tamano, estado.tamano);

    const anchoEscalado = estado.imagen.width * estado.escala;
    const altoEscalado = estado.imagen.height * estado.escala;

    const x = estado.tamano / 2 - anchoEscalado / 2 - estado.offsetX;
    const y = estado.tamano / 2 - altoEscalado / 2 - estado.offsetY;

    ctx.drawImage(estado.imagen, x, y, anchoEscalado, altoEscalado);
}

function aplicarZoom(estado, factor, centroX, centroY) {
    const escalaMinima = estado.escalaMinima;
    const nuevaEscala = Math.min(ZOOM_MAX, Math.max(escalaMinima, estado.escala * factor));

    if (nuevaEscala === estado.escala) {
        return;
    }

    // El punto bajo el cursor/dedos se mantiene fijo al hacer zoom, en vez de
    // saltar hacia el centro del canvas.
    const proporcion = nuevaEscala / estado.escala;
    estado.offsetX = (estado.offsetX + centroX) * proporcion - centroX;
    estado.offsetY = (estado.offsetY + centroY) * proporcion - centroY;
    estado.escala = nuevaEscala;

    limitarPan(estado);
    dibujar(estado);
}

function enganchar(estado) {
    const canvas = estado.canvas;
    let arrastrando = false;
    let ultimoX = 0;
    let ultimoY = 0;
    const punteros = new Map();
    let distanciaPinchInicial = 0;
    let escalaPinchInicial = 1;

    const onPointerDown = (e) => {
        canvas.setPointerCapture(e.pointerId);
        punteros.set(e.pointerId, e);

        if (punteros.size === 1) {
            arrastrando = true;
            ultimoX = e.clientX;
            ultimoY = e.clientY;
        } else if (punteros.size === 2) {
            arrastrando = false;
            const [p1, p2] = Array.from(punteros.values());
            distanciaPinchInicial = distancia(p1, p2);
            escalaPinchInicial = estado.escala;
        }
    };

    const onPointerMove = (e) => {
        if (!punteros.has(e.pointerId)) {
            return;
        }
        punteros.set(e.pointerId, e);

        if (punteros.size === 2) {
            const [p1, p2] = Array.from(punteros.values());
            const distanciaActual = distancia(p1, p2);
            if (distanciaPinchInicial > 0) {
                const factor = (distanciaActual / distanciaPinchInicial) * escalaPinchInicial / estado.escala;
                aplicarZoom(estado, factor, 0, 0);
            }
            return;
        }

        if (!arrastrando) {
            return;
        }

        estado.offsetX -= e.clientX - ultimoX;
        estado.offsetY -= e.clientY - ultimoY;
        ultimoX = e.clientX;
        ultimoY = e.clientY;

        limitarPan(estado);
        dibujar(estado);
    };

    const onPointerUp = (e) => {
        punteros.delete(e.pointerId);
        arrastrando = punteros.size === 1;
        if (arrastrando) {
            const restante = Array.from(punteros.values())[0];
            ultimoX = restante.clientX;
            ultimoY = restante.clientY;
        }
    };

    const onWheel = (e) => {
        e.preventDefault();
        const factor = e.deltaY < 0 ? 1.1 : 1 / 1.1;
        aplicarZoom(estado, factor, 0, 0);
    };

    canvas.addEventListener('pointerdown', onPointerDown);
    canvas.addEventListener('pointermove', onPointerMove);
    canvas.addEventListener('pointerup', onPointerUp);
    canvas.addEventListener('pointercancel', onPointerUp);
    canvas.addEventListener('wheel', onWheel, { passive: false });

    estado.listeners = { onPointerDown, onPointerMove, onPointerUp, onWheel };
}

export function cargar(canvasId, dataUrl) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) {
        return Promise.reject(new Error(`No se encontró el canvas ${canvasId}`));
    }

    return new Promise((resolve, reject) => {
        const imagen = new Image();
        imagen.onload = () => {
            const tamano = canvas.width;
            // "cover": la escala mínima es la que hace que el lado más corto
            // de la imagen llene el cuadrado, sin dejar huecos en blanco.
            const escalaMinima = tamano / Math.min(imagen.width, imagen.height);

            const estado = {
                canvas,
                tamano,
                imagen,
                escala: escalaMinima,
                escalaMinima,
                offsetX: 0,
                offsetY: 0,
            };

            estados.set(canvasId, estado);
            enganchar(estado);
            dibujar(estado);
            resolve();
        };
        imagen.onerror = () => reject(new Error('No se pudo cargar la imagen.'));
        imagen.src = dataUrl;
    });
}

export function zoom(canvasId, delta) {
    const estado = estados.get(canvasId);
    if (!estado) {
        return;
    }
    aplicarZoom(estado, delta > 0 ? 1.2 : 1 / 1.2, 0, 0);
}

export function recortar(canvasId, tamanoSalida) {
    const estado = estados.get(canvasId);
    if (!estado) {
        return Promise.reject(new Error('No hay ninguna imagen cargada para recortar.'));
    }

    const salida = document.createElement('canvas');
    salida.width = tamanoSalida;
    salida.height = tamanoSalida;
    const ctx = salida.getContext('2d');

    // Mismo cálculo que dibujar(), pero escalado del tamaño de pantalla al
    // tamaño de exportación final.
    const factorSalida = tamanoSalida / estado.tamano;
    const anchoEscalado = estado.imagen.width * estado.escala * factorSalida;
    const altoEscalado = estado.imagen.height * estado.escala * factorSalida;
    const x = tamanoSalida / 2 - anchoEscalado / 2 - estado.offsetX * factorSalida;
    const y = tamanoSalida / 2 - altoEscalado / 2 - estado.offsetY * factorSalida;

    ctx.drawImage(estado.imagen, x, y, anchoEscalado, altoEscalado);

    return new Promise((resolve, reject) => {
        salida.toBlob((blob) => {
            if (!blob) {
                reject(new Error('No se pudo generar la imagen recortada.'));
                return;
            }
            const lector = new FileReader();
            lector.onload = () => resolve(lector.result.split(',')[1]);
            lector.onerror = () => reject(new Error('No se pudo leer la imagen recortada.'));
            lector.readAsDataURL(blob);
        }, 'image/jpeg', 0.85);
    });
}

export function destruir(canvasId) {
    const estado = estados.get(canvasId);
    if (!estado) {
        return;
    }

    const { canvas, listeners } = estado;
    canvas.removeEventListener('pointerdown', listeners.onPointerDown);
    canvas.removeEventListener('pointermove', listeners.onPointerMove);
    canvas.removeEventListener('pointerup', listeners.onPointerUp);
    canvas.removeEventListener('pointercancel', listeners.onPointerUp);
    canvas.removeEventListener('wheel', listeners.onWheel);

    estados.delete(canvasId);
}
