using Manejo_de_Tareas.Models;

namespace Manejo_de_Tareas.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task Borrar(string ruta, string contenedor);
        Task<AlmacenarArchivoResultado[]> Almacenar(string contenedor,
                                                    IEnumerable<IFormFile> archivos);
    }
}
