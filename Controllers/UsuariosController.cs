using Manejo_de_Tareas.Models;
using Manejo_de_Tareas.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Manejo_de_Tareas.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDBContext _context;

        public UsuariosController(UserManager<IdentityUser> userManager,
                                  SignInManager<IdentityUser> signInManager,
                                  ApplicationDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(Registro_ViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new IdentityUser()
            {
                Email = modelo.Email,
                UserName = modelo.Email
            };

            var resultado = await _userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach(var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
                }
            }

            return View(modelo);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string mensaje=null)
        {
            if(mensaje is not null)
            {
                ViewData["Mensaje"] = mensaje;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login_ViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado =
               await _signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame, false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Usuario y/o Contraseña incorrecto/s");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult LoginExterno(string proovedor,string urlRetorno = null)
        {
            var urlRedireccion = Url.Action("RegistrarUsuarioExterno", new { urlRetorno });
            var propiedades =
                _signInManager.ConfigureExternalAuthenticationProperties(proovedor, urlRedireccion);
            return new ChallengeResult(proovedor, propiedades);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null,
            string remoteError = null)
        {
            urlRetorno = urlRetorno ?? Url.Content("~/");

            var mensaje = "";

            if(remoteError is not null)
            {
                mensaje = $"Error de Autenticacion Externa: {remoteError}";
                return RedirectToAction("Login", routeValues: new { mensaje });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if(info is null)
            {
                mensaje = "Error cargando datos de Login Externo";
                return RedirectToAction("login", new { mensaje });
            }

            var resultadoLoginExterno =
                await _signInManager.ExternalLoginSignInAsync
                (info.LoginProvider, info.ProviderKey, true, true);

            if (resultadoLoginExterno.Succeeded)
            {
                return LocalRedirect(urlRetorno);
            }

            string email = "";

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                mensaje = "Error con Email provisto externamente";
                return RedirectToAction("Login", new { mensaje });
            }

            var usuario = new IdentityUser { Email = email, UserName = email };

            var resultadoCrearUsuario = await _userManager.CreateAsync(usuario);

            if (!resultadoCrearUsuario.Succeeded)
            {
                mensaje = resultadoCrearUsuario.Errors.First().Description;
                return RedirectToAction("Login", new { mensaje });
            }

            var resultadoAgregarLogin = await _userManager.AddLoginAsync(usuario, info);

            if (resultadoAgregarLogin.Succeeded)
            {
                await _signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }

            mensaje = "Ha ocurrido un error";
            return RedirectToAction("Login", new { mensaje });
        }



        [Authorize(Roles = Constantes.ROL_ADMIN)] 
        [HttpGet]
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            var usuarios = await _context.Users.Select(u=> new Usuario_ViewModel
            {
                Email= u.Email

            }).ToListAsync();

            var modelo = new UsuariosListado_ViewModel
            {
                Usuarios = usuarios,
                Mensaje = mensaje
            };

            return View(modelo);
        }

        [Authorize(Roles = Constantes.ROL_ADMIN)]
        [HttpPost]
        public async Task<IActionResult> HacerAdmin(string email)
        {
            var usuario = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        
            if (usuario is null)
            {
                return NotFound();
            }

            await _userManager.AddToRoleAsync(usuario, Constantes.ROL_ADMIN);

            return RedirectToAction("Listado",routeValues: new { mensaje = "Rol asignado a " + email });
        }

        [Authorize(Roles = Constantes.ROL_ADMIN)]
        [HttpPost]
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            var usuario = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (usuario is null)
            {
                return NotFound();
            }

            await _userManager.RemoveFromRoleAsync(usuario, Constantes.ROL_ADMIN);

            return RedirectToAction("Listado", routeValues: new { mensaje = "Rol removido a " + email });
        }
    }
}
