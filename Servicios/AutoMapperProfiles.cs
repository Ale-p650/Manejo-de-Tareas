using AutoMapper;
using Manejo_de_Tareas.Entidades;
using Manejo_de_Tareas.Models;

namespace Manejo_de_Tareas.Servicios
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Tarea, TareaDTO>().ReverseMap();
        }
    }
}
