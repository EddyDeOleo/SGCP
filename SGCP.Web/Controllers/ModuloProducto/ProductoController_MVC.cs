using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloProducto.Producto;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Interfaces.ModuloProducto;
using SGCP.Web.Models.ModuloProducto;
using SGCP.Web.Models.ModuloUsuarios.AdministradorModels;
using SGCP.Web.Models.ModuloUsuarios.ClienteModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SGCP.Web.Controllers.ModuloProducto
{
    public class ProductoController_MVC : Controller
    {
        private readonly IProductoService _productoService;


         public async Task<ActionResult> Index()
        {
            Response_GAP_Result getallresponse = null;


            try
            {
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri("http://localhost:5287/api/");

                    var response = await client.GetAsync("Producto/get-productos");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getallresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GAP_Result>(apiResponse);
                    }
                    else
                    {
                        getallresponse = new Response_GAP_Result
                        {
                            success = false,
                            message = "Error al obtener los productos.",
                        };
                    }

                }
            }
            catch (Exception ex)
            {

                getallresponse = new Response_GAP_Result
                {
                    success = false,
                    message = $"Error al obtener los productos. {ex.Message}",
                };
                throw;
            }

            return View(getallresponse.data);

        }


        // GET: ProductoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            Response_GP_Result getbyidresponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5287/api/");

                    var response = await client.GetAsync($"Producto/getbyid-productos?id={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getbyidresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GP_Result>(apiResponse);
                    }
                    else
                    {
                        getbyidresponse = new Response_GP_Result
                        {
                            success = false,
                            message = "Error al obtener el cliente.",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                getbyidresponse = new Response_GP_Result
                {
                    success = false,
                    message = $"Error al obtener los clientes. {ex.Message}",
                };
            }

            return View(getbyidresponse.data);
        }


        // GET: ProductoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductoCreateModel model)
        {

            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");


            Response_CP_Result createResponse = null;

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5287/api/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsJsonAsync("Producto/create-productos", model);


                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    createResponse = JsonSerializer.Deserialize<Response_CP_Result>(
                        apiResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }

                    );

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    createResponse = new Response_CP_Result
                    {
                        success = false,
                        message = "Error al crear el producto."
                    };
                }

            }

            catch (Exception ex)
            {
                createResponse = new Response_CP_Result
                {
                    success = false,
                    message = $"Error en la operación: {ex.Message}"
                };
            }

            return View();
        }
        


        // GET: ProductoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {

            Response_GP_Result getbyidresponse = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5287/api/");

                    var response = await client.GetAsync($"Producto/getbyid-productos?id={id}");

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        getbyidresponse = System.Text.Json.JsonSerializer.Deserialize<Response_GP_Result>(apiResponse);
                    }
                    else
                    {
                        getbyidresponse = new Response_GP_Result
                        {
                            success = false,
                            message = "Error al obtener el cliente.",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                getbyidresponse = new Response_GP_Result
                {
                    success = false,
                    message = $"Error al obtener los clientes. {ex.Message}",
                };
            }

            return View(getbyidresponse.data);
        }

        // POST: ProductoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductoEditModel model)
        {


            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");


            Response_EP_Result editResponse = null;

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5287/api/");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PutAsJsonAsync("Producto/update-productos", model);

                string apiResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    editResponse = JsonSerializer.Deserialize<Response_EP_Result>(
                        apiResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }

                    );

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    editResponse = new Response_EP_Result
                    {
                        success = false,
                        message = "Error al actualizar el producto."
                    };
                }

            }

            catch (Exception ex)
            {
                editResponse = new Response_EP_Result
                {
                    success = false,
                    message = $"Error en la operación: {ex.Message}"
                };
            }

            return View();
        }
    }
}
