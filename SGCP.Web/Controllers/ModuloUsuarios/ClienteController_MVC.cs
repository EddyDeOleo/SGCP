using Microsoft.AspNetCore.Mvc;
using SGCP.Web.Models.ModuloUsuarios.AdministradorModels;
using SGCP.Web.Models.ModuloUsuarios.ClienteModels;
using System.Text.Json;

namespace SGCP.Web.Controllers.ModuloUsuarios
{
    public class ClienteController_MVC : Controller
    {
       
        // GET: ClienteController
        public async Task<ActionResult> Index()
        {
            Response_GAC_Result getallresponse = null;


            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri("http://localhost:5138/api/");

                    var response = await client.GetAsync("Cliente/get-cliente");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getallresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GAC_Result>(apiResponse);
                    }
                    else
                    {
                        getallresponse = new Response_GAC_Result
                        {
                            success = false,
                            message = "Error al obtener los clientes.",
                        };
                    }

                }
            }
            catch (Exception ex)
            {

                getallresponse = new Response_GAC_Result
                {
                    success = false,
                    message = $"Error al obtener los clientes. {ex.Message}",
                };
                throw;
            }

            return View(getallresponse.data);

        }

        // GET: ClienteController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Response_GC_Result getbyidresponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5138/api/");

                    var response = await client.GetAsync($"Cliente/getbyid-cliente?id={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getbyidresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GC_Result>(apiResponse);
                    }
                    else
                    {
                        getbyidresponse = new Response_GC_Result
                        {
                            success = false,
                            message = "Error al obtener el cliente.",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                getbyidresponse = new Response_GC_Result
                {
                    success = false,
                    message = $"Error al obtener los clientes. {ex.Message}",
                };
            }

            return View(getbyidresponse.data);
        }


        // GET: ClienteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClienteCreateModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            Response_CC_Result createResponse = null;

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5138/api/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsJsonAsync("Cliente/create-cliente", model);
                string apiResponse = await response.Content.ReadAsStringAsync();

                createResponse = JsonSerializer.Deserialize<Response_CC_Result>(
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


        // GET: ClienteController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {


            Response_GC_Result getbyidresponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5138/api/");

                    var response = await client.GetAsync($"Cliente/getbyid-cliente?id={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getbyidresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GC_Result>(apiResponse);
                    }
                    else
                    {
                        getbyidresponse = new Response_GC_Result
                        {
                            success = false,
                            message = "Error al obtener el cliente.",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                getbyidresponse = new Response_GC_Result
                {
                    success = false,
                    message = $"Error al obtener los clientes. {ex.Message}",
                };
            }

            return View(getbyidresponse.data);
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ClienteEditModel model)
        {


            var token = HttpContext.Session.GetString("Token");
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Login");

            model.UsuarioModificacion = int.Parse(userIdString);

            Response_EC_Result editResponse = null;

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5138/api/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PutAsJsonAsync("Cliente/update-cliente", model);

                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    editResponse = JsonSerializer.Deserialize<Response_EC_Result>(
                        apiResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }

                    );

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    editResponse = new Response_EC_Result
                    {
                        success = false,
                        message = "Error al actualizar el cliente."
                    };
                }

            }

            catch (Exception ex)
            {
                editResponse = new Response_EC_Result
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

