using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Interfaces.ModuloUsuarios;

namespace SGCP.Web.Controllers.ModuloUsuarios
{
    public class ClienteController_MVC : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClienteController_MVC> _logger;

        public ClienteController_MVC(IClienteService clienteService, ILogger<ClienteController_MVC> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        // GET: ClienteController
        public async Task<ActionResult> Index()
        {

            ServiceResult result = await _clienteService.GetCliente();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: ClienteController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ServiceResult result = await _clienteService.GetClienteById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: ClienteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateClienteDTO createClienteDTO)
        {
            try
            {
                ServiceResult result = await _clienteService.CreateCliente(createClienteDTO);

                if (!result.Success)
                {
                    ViewBag.ErrorMessage = result.Message;
                    return View();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ClienteController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ServiceResult result = await _clienteService.GetClienteById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateClienteDTO updateClienteDTO)
        {
            try
            {
                ServiceResult result = await _clienteService.UpdateCliente(updateClienteDTO);

                if (!result.Success)
                {
                    ViewBag.ErrorMessage = result.Message;
                    return View();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
