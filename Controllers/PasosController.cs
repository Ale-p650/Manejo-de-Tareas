using Manejo_de_Tareas.Entidades;
using Manejo_de_Tareas.Models;
using Manejo_de_Tareas.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manejo_de_Tareas.Controllers
{
    [Route("api/pasos")]
    public class PasosController :ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;

        public PasosController(ApplicationDBContext context,
                                IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
        }

        [HttpPost("{tareaId:int}")]
        public async Task<ActionResult<Paso>> Post(int tareaId, 
            [FromBody] PasoCrearDTO pasoCrearDTO)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioID();

            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaId);

            if (tarea is null)
            {
                return NotFound();
            }

            if(tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            var existenPasos = await _context.Pasos.AnyAsync(p => p.TareaId == tarea.Id);

            var ordenMayor = 0;

            if (existenPasos)
            {
                ordenMayor = await _context.Pasos
                .Where(p => p.TareaId == tarea.Id)
                .Select(p => p.Orden)
                .MaxAsync();
            }

            Paso paso = new()
            {
                TareaId = tareaId,
                Orden = ordenMayor + 1,
                Descripcion = pasoCrearDTO.Descripcion,
                Realizado = pasoCrearDTO.Realizado
            };

            _context.Add(paso);
            await _context.SaveChangesAsync();

            return paso;

        }
    }
}
