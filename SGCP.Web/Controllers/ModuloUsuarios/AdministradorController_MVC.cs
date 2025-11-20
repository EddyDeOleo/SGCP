using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Web.Models.ModuloUsuarios.AdministradorModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SGCP.Web.Controllers.ModuloUsuarios
{
    public class AdministradorController_MVC : Controller

    {
       

        // GET: AdminController
        public async Task<ActionResult> Index()
        {
            Response_GAA_Result getallresponse = null;


            try { 
                using (var client = new HttpClient())
                {
                    
                    client.BaseAddress = new Uri("http://localhost:5138/api/");
                   
                    var response = await client.GetAsync("Administrador/get-admin");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getallresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GAA_Result>(apiResponse);
                    }
                   else
                    {
                       getallresponse = new Response_GAA_Result
                        {
                            success = false,
                            message = "Error al obtener los administradores.",
                        };
                    }

                }
            }
            catch (Exception ex)
            {

                getallresponse = new Response_GAA_Result
                {
                    success = false,
                    message = $"Error al obtener los administradores. {ex.Message}",
                };
                throw;
            }

            return View(getallresponse.data);

        }

        // GET: AdminController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Response_GA_Result getbyidresponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5138/api/");

                    var response = await client.GetAsync($"Administrador/getbyid-admin?id={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getbyidresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GA_Result>(apiResponse);
                    }
                    else
                    {
                        getbyidresponse = new Response_GA_Result
                        {
                            success = false,
                            message = "Error al obtener el administrador.",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                getbyidresponse = new Response_GA_Result
                {
                    success = false,
                    message = $"Error al obtener los administradores. {ex.Message}",
                };
            }

            return View(getbyidresponse.data);
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminCreateModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            Response_CA_Result createResponse = null;

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5138/api/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsJsonAsync("Administrador/create-admin", model);
                string apiResponse = await response.Content.ReadAsStringAsync();

                createResponse = JsonSerializer.Deserialize<Response_CA_Result>(
                    apiResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (!createResponse.success)
                {
                    ViewBag.Error = createResponse.message;
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error en la operación: {ex.Message}";
                return View(model);
            }
        }

        // GET: AdminController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {


            Response_GA_Result getbyidresponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5138/api/");

                    var response = await client.GetAsync($"Administrador/getbyid-admin?id={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getbyidresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GA_Result>(apiResponse);
                    }
                    else
                    {
                        getbyidresponse = new Response_GA_Result
                        {
                            success = false,
                            message = "Error al obtener el administrador.",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                getbyidresponse = new Response_GA_Result
                {
                    success = false,
                    message = $"Error al obtener los administradores. {ex.Message}",
                };
            }

            return View(getbyidresponse.data);
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<ActionResult> Edit(AdminEditModel model)
        {
        

            var token = HttpContext.Session.GetString("Token");
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Login");

            model.UsuarioModificacion = int.Parse(userIdString);

            Response_EA_Result editResponse = null;

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5138/api/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PutAsJsonAsync("Administrador/update-admin", model);

                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    editResponse = JsonSerializer.Deserialize<Response_EA_Result>(
                        apiResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }

                    );

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    editResponse = new Response_EA_Result
                    {
                        success = false,
                        message = "Error al actualizar el administrador."
                    };
                }

            }

            catch (Exception ex)
            {
                editResponse = new Response_EA_Result
                {
                    success = false,
                    message = $"Error en la operación: {ex.Message}"
                };
            }

            AdminGetModel adminModel = null;
            if (editResponse?.data != null)
            {
                if (editResponse.data is JsonElement element)
                {
                    adminModel = JsonSerializer.Deserialize<AdminGetModel>(
                        element.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
            }

            if (editResponse != null && !editResponse.success)
                ViewBag.Error = editResponse.message;

            return View(adminModel);
        }

    }
}
