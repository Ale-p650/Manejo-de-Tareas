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


    }
}
