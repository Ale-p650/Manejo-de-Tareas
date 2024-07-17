function agregarNuevaTarea() {
    tareaListadoViewModel.tareas.push(new tareaElementoListadoViewModel({ id: 0, titulo: '' }));

    $("[name=titulo-tarea]").last().focus();
}

function manejarFocusOutTituloTarea(tarea) {
    const titulo = tareas.titulo();
    if (!titulo) {
        tareaElementoListadoViewModel.tareas.pop();
        return;
    }

    tarea.id(1);
}