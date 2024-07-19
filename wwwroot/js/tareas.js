function agregarNuevaTarea() {
    tareaListadoViewModel.tareas.push(new tareaElementoListadoViewModel({ id: 0, titulo: '' }));
    
    $("[name=titulo-tarea]").last().focus();
}

async function manejarFocusOutTituloTarea(tarea) {
    const titulo = tarea.titulo();
    if(!titulo) {
        tareaListadoViewModel.tareas.pop();
        return;
    }

    const data = JSON.stringify(titulo);

    const respuesta = await fetch(urlTareas, {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (respuesta.ok) {
        const json = await respuesta.json();
        tarea.id(json.id);
    }
    else {
        manejarErrorAPI(respuesta)
    }
}

async function obtenerTareas() {
    tareaListadoViewModel.cargando(true);

    const respuesta = await fetch(urlTareas, {
        method: 'GET',
        headers: {
            'Content-Type':'application/json'
        }
    })

    if (!respuesta.ok) {
        manejarErrorAPI(respuesta)
        return;
    }

    const json = await respuesta.json();
    tareaListadoViewModel.tareas([]);

    json.forEach(valor => {
        tareaElementoListadoViewModel.tareas.push(new tareaElementoListadoViewModel(valor));
    })

    tareaListadoViewModel.cargando(false);

}

async function actualizarOrdenTareas() {
    const ids = obtenerIDsTareas();
    await enviarTareasAlBackend(ids);


    const arregloOrdenado = tareaListadoViewModel.tareas.sorted(function (a, b) {
        return ids.indexOf(a.id().toString()) - ids.indexOf(b.id().toString());
    });

    tareaElementoListadoViewModel.tareas([]);
    tareaElementoListadoViewModel.tareas(arregloOrdenado);
}

function obtenerIDsTareas() {
    const ids = $("[name=titulo-tarea]").map(function () {
        return $(this).attr("data-id");
    })
}

async function enviarTareasAlBackend(ids) {
    var data = JSON.stringify(ids);
    await fetch('${urlTareas}/ordenar', {
        method: 'POST',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });
}

async function manejarClickTarea(tarea) {
    if (tarea.esNuevo()) {
        return;
    }

    const respuesta = await fetch('${urlTareas}/${tarea.id()}', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorAPI(respuesta);
        return;
    }

    const json = await respuesta.json();

    modalEditarTareaBootstrap.show()

    tareaEditarVM.id = json.id;
    tareaEditarVM.titulo(json.titulo);
    tareaEditarVM.descripcion(json.descripcion);

    tareaEditarVM.pasos([]);

    json.pasos.forEach(paso => {
        tareaEditarVM.pasos.push(new pasoViewModel({ ...paso, modoEdicion: false }));
    })

    modalEditarTareaBootstrap.show();

}

async function manejarCambioEditarTarea() {
    const obj = {
        id: tareaEditarVM.id,
        titulo: tareaEditarVM.titulo(),
        descripcion: tareaEditarVM.descripcion()
    };

    if (!obj.titulo) {
        return;
    }

    await editarTareaCompleta(obj);

    const index = tareaListadoViewModel.tareas().findIndex(t => t.id() === obj.id);
    const tarea = tareaListadoViewModel.tareas()[index];
    tarea.titulo(obj.titulo);
}

async function editarTareaCompleta(tarea) {
    const data = JSON.stringify(tarea);

    const respuesta = await fetch('${urlTareas}/${tarea.id}', {
        method: 'PUT',
        body: data,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    if (!respuesta.ok) {
        manejarErrorAPI(respuesta);
        throw "error";
    }
}


$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTareas();
        }
    })
})

function intentarBorrarTarea(tarea) {
    modalEditarTareaBootstrap.hide();

    confirmarAccion({
        callBackAceptar: () => {
            borrarTarea(tarea);
        },
        callBackCancelar: () => {
            modalEditarTareaBootstrap.show();
        },
        titulo: 'Desea borrar la tarea ${tarea.titulo()}'
    })
}

async function borrarTarea(tarea) {
    const idTarea = tarea.id;

    const respuesta = await fetch('${urlTareas}/${idTarea}', {
        method: 'DELETE',
        headers: {
            'Content-Type':'application/json'
        }
    })

    if (respuesta.ok) {
        const indice = obtenerIndiceTareaEnEdicion();
        tareaListadoViewModel.tareas.splice(indice, 1);
    }
}

function obtenerIndiceTareaEnEdicion() {
    return tareaListadoViewModel.tareas().findIndex(t => t.id == tareaEditarVM.id)
}