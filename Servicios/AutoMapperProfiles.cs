using AutoMapper;
using Manejo_de_Tareas.Entidades;
using Manejo_de_Tareas.Models;

namespace Manejo_de_Tareas.Servicios
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Tarea, TareaDTO>()
                .ForMember(dto => dto.PasosTotales, ent => ent
                .MapFrom(t => t.Pasos.Count()))
                .ForMember(dto => dto.PasosRealizados, ent => ent
                .MapFrom(t => t.Pasos.Where(p => p.Realizado).Count()));
        }
    }
}
