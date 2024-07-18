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