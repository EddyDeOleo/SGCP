using Microsoft.AspNetCore.Mvc;
using SGCP.Web.Models.ModuloUsuarios.AuthModels;
using System.Text.Json;

namespace SGCP.Web.Controllers.ModuloUsuarios
{
  
        public class AuthController_MVC : Controller
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public AuthController_MVC(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
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
        public async Task<IActionResult> Login(AuthLoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5138/api/");

            var response = await client.PostAsJsonAsync("Auth/login-user", model);

            if (!response.IsSuccessStatusCode)
            {

                TempData["Error"] = "Credenciales incorrectas.";
                return View(model);
            }

            var apiResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response_L_Result>(apiResponse);

            if (!result.success)
            {
                TempData["Error"] = result.message;
                return View(model);
            }

            var user = JsonSerializer.Deserialize<AuthResponseModel>(
     result.data.GetRawText(),
     new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
 );

            if (user == null)
            {
                TempData["Error"] = "No se recibieron datos válidos del API.";
                return View(model);
            }

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


