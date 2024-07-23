using AutoMapper;
using AutoMapper.QueryableExtensions;
using Manejo_de_Tareas.Entidades;
using Manejo_de_Tareas.Models;
using Manejo_de_Tareas.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manejo_de_Tareas.Controllers
{
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IMapper _mapper;

        public TareasController(ApplicationDBContext context,
            IServicioUsuarios servicioUsuarios,
            IMapper mapper)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioID();

            var tareas = await _context.Tareas
                .Where(t=>t.UsuarioCreacionId== usuarioId)
                .OrderBy(t=>t.Orden)
                .ProjectTo<TareaDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(tareas);
        }

        [HttpGet("{id}:int")]
        public async Task<ActionResult<Tarea>> Get(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioID();

            var tarea = await _context.Tareas
                .Include(t => t.Pasos.OrderBy(p => p.Orden))
                .Include(t => t.ArchivosAdjuntos.OrderBy(a=>a.Orden))
                .FirstOrDefaultAsync
                (t => t.Id == id && t.UsuarioCreacionId == usuarioId );

            if(tarea is null)
            {
                return NotFound();
            }

            return tarea;
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody]string titulo)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioID();

            bool existenTareas = await _context.Tareas.AnyAsync(t => t.UsuarioCreacionId == usuarioId);

            int ordenMayor = 0;

            if (existenTareas)
            {
                ordenMayor = await _context.Tareas
                    .Where(t => t.UsuarioCreacionId == usuarioId)
                    .Select(t => t.Orden)
                    .MaxAsync();
            }

            var tarea = new Tarea()
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                Orden = ordenMayor + 1,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Add(tarea);
            await _context.SaveChangesAsync();

            return tarea;
        }

        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioID();

            var tareas = await _context.Tareas
                .Where(t => t.UsuarioCreacionId == usuarioId)
                .ToListAsync();

            var tareasId = tareas.Select(t => t.Id);

            var tareasDict = tareas.ToDictionary(x => x.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var tarea = tareasDict[id];
                tarea.Orden = i + 1;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarTarea(int id, [FromBody] TareaEditarDTO tareaEditar)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioID();

            var tarea = await _context.Tareas
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioCreacionId == usuarioId );

            if(tarea is null)
            {
                return NotFound();
            }

            tarea.Titulo = tareaEditar.Titulo;
            tarea.Descripcion = tareaEditar.Descripcion;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> BorrarTarea(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioID();

            var tarea = _context.Tareas
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioCreacionId == usuarioId);

            if(tarea is null)
            {
                return NotFound();
            }

            _context.Remove(tarea);
            await _context.SaveChangesAsync();
            return Ok();
        }



    }
}
