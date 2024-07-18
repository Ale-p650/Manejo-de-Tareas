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


$(function () {
    $("#reordenable").sortable({
        axis: 'y',
        stop: async function () {
            await actualizarOrdenTareas();
        }
    })
})