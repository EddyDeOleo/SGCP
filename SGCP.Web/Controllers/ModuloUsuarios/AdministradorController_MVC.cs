using Microsoft.AspNetCore.Mvc;
using SGCP.Web.Models.ModuloUsuarios.AdministradorModels;
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

            Response_CA_Result createResponse;

            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5138/api/");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("Administrador/create-admin", model);

            // Leer SIEMPRE el body
            string apiResponse = await response.Content.ReadAsStringAsync();

            // Intentar deserializar
            try
            {
                createResponse = JsonSerializer.Deserialize<Response_CA_Result>(
                    apiResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch
            {
                // Body no es JSON o está corrupto
                createResponse = new Response_CA_Result
                {
                    success = false,
                    message = "El servidor devolvió una respuesta no válida."
                };
            }

            // Validación por status code
            if (response.IsSuccessStatusCode && createResponse.success)
            {
                TempData["Success"] = createResponse.message;
                return RedirectToAction(nameof(Index));
            }

            // Error controlado
            TempData["Error"] = createResponse.message ?? "Error desconocido con el servidor";

            return View(model); // VOLVER A MOSTRAR EL FORMULARIO
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

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");


            Response_EA_Result editResponse = null;

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5138/api/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PutAsJsonAsync("Administrador/update-admin", model);


                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

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

            return View();
        }

    }
}
