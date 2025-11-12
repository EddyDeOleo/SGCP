using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloReporte.Reporte;
using SGCP.Application.Interfaces.ModuloReporte;

namespace SGCP.Web.Controllers.ModuloReporte
{
    public class ReporteController_MVC : Controller
    {
       private readonly IReporteService _reporteService;

        public ReporteController_MVC(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        // GET: ReporteController_MVC
        public async Task<ActionResult> Index()
        {

            ServiceResult result = await _reporteService.GetReporte();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: ReporteController_MVC/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ServiceResult result = await _reporteService.GetReporteById(id);
            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            return View(result.Data);
        }

        // GET: ReporteController_MVC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReporteController_MVC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Create(CreateReporteDTO createReporteDTO)
        {
            try
            {
                ServiceResult result = await _reporteService.CreateReporte(createReporteDTO);
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

        // GET: ReporteController_MVC/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ServiceResult result = await _reporteService.GetReporteById(id);
            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            return View(result.Data);
        }

        // POST: ReporteController_MVC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateReporteDTO updateReporteDTO)
        {
            try
            {
                ServiceResult result = await _reporteService.UpdateReporte(updateReporteDTO);
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
