using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Application.Interfaces.ModuloUsuarios;

namespace SGCP.Web.Controllers.ModuloUsuarios
{
    public class AdministradorController_MVC : Controller

    {
        private readonly IAdminService _adminService;   
        private readonly ILogger<AdministradorController_MVC> _logger;
        public AdministradorController_MVC(IAdminService adminService, ILogger<AdministradorController_MVC> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        // GET: AdminController
        public async Task<ActionResult> Index()
        {

            ServiceResult result = await _adminService.GetAdmin();  

            if(!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(); 
            }   

            return View(result.Data);
        }

        // GET: AdminController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ServiceResult result = await _adminService.GetAdminById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAdminDTO createAdminDTO)
        {
            try
            {
                ServiceResult result = await _adminService.CreateAdmin(createAdminDTO);

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

        // GET: AdminController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ServiceResult result = await _adminService.GetAdminById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateAdminDTO updateAdminDTO)
        {
            try
            {
                ServiceResult result = await _adminService.UpdateAdmin(updateAdminDTO);

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
