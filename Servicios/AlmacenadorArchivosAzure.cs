using Azure.Storage.Blobs;
using Manejo_de_Tareas.Models;
using Azure.Storage.Blobs.Models;

namespace Manejo_de_Tareas.Servicios
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        private readonly string _connectionString;

        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<AlmacenarArchivoResultado[]> Almacenar(string contenedor, IEnumerable<IFormFile> archivos)
        {
            BlobContainerClient cliente = new(_connectionString, contenedor);

            await cliente.CreateIfNotExistsAsync();

            cliente.SetAccessPolicy(PublicAccessType.Blob);

            var tareas = archivos.Select(async archivo =>
            {
                var nombreArchivoOriginal = Path.GetFileName(archivo.FileName);
                var extension = Path.GetExtension(archivo.FileName);
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var blob = cliente.GetBlobClient(nombreArchivo);
                var blobHttpHeaders = new BlobHttpHeaders();
                blobHttpHeaders.ContentType = archivo.ContentType;
                await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);

                return new AlmacenarArchivoResultado
                {
                    URL = blob.Uri.ToString(),
                    Titulo = nombreArchivoOriginal
                };
            });

            var resultados = await Task.WhenAll(tareas);

            return resultados;
        }

        public async Task Borrar(string ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }

            var cliente = new BlobContainerClient(_connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            var nombreArchivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(nombreArchivo);
            await blob.DeleteIfExistsAsync();

        }
    }
}
