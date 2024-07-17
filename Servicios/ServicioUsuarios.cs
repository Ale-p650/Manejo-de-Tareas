using System.Security.Claims;

namespace Manejo_de_Tareas.Servicios
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private HttpContext _httpContext;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public string obtenerUsuarioID()
        {
            if (_httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = _httpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                return idClaim;
            }
            else
            {
                throw new Exception("Error de Autenticacion");
            }
        }
    }
}
