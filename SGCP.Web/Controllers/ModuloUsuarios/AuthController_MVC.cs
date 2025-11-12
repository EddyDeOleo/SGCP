using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Dtos.ModuloUsuarios.Authetication;
using SGCP.Application.Interfaces.ModuloUsuarios;

namespace SGCP.Web.Controllers.ModuloUsuarios
{
    public class AuthController_MVC : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController_MVC> _logger;

        public AuthController_MVC(IAuthService authService, ILogger<AuthController_MVC> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.Login(model);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View(model);
            }

            var user = (AuthResponseDTO)result.Data;

            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Token", user.Token);
            HttpContext.Session.SetString("FullName", $"{user.Nombre} {user.Apellido}");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
