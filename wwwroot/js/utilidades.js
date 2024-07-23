async function manejarErrorAPI(respuesta) {
    let mensajeError = '';

    if (respuesta.status === 400) {
        mensajeError = await respuesta.text();
    }
    else if (respuesta.status === 404) {
        mensajeError = "El recurso solicitado no fue encontrado";
    }
    else {
        mensajeError = "Ha ocurrido un error con la solicitud";
    }

    mostrarMensajeError(mensajeError)
}

function mostrarMensajeError(mensaje) {
    Swal.fire({
        icon: 'error',
        title: 'Error',
        text: mensaje
    })
}

function confirmarAccion({ callBackAceptar, callBackCancelar, titulo }) {
    Swal.fire({
        title: titulo || 'Desea Borrar La Tarea ?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si',
        focusConfirm: true
    }).then((resultado) => {
        if (resultado.isConfirmed) {
            callBackAceptar();
        } else if (callBackCancelar) {
            callBackCancelar();
        }
    })
}

function descargarArchivo(url, nombre) {
    var link = document.createElement('a');
    link.download = nombre;
    link.target = "_blank";
    link.href = url;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    delete link;
}