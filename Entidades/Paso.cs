using System.Reflection.Metadata.Ecma335;

namespace Manejo_de_Tareas.Entidades
{
    public class Paso
    {
        public Guid Id { get; set; }
        public int TareaId { get; set; }
        public Tarea Tarea { get; set; }
        public string Descripcion { get; set; }
        public bool Realizado { get; set; }
        public int Orden { get; set; }

    }
}
