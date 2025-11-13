using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Dtos.ModuloProducto.Producto;
using SGCP.Application.Interfaces.ModuloCarrito;

namespace SGCP.Web.Controllers.ModuloCarrito
{
    public class CarritoController_MVC : Controller
    {

        private readonly ICarritoService _carritoService;

        public CarritoController_MVC(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        // GET: CarritoController_MVC
        public async Task<ActionResult> Index()
        {
            var result = await _carritoService.GetCarrito();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return View(result.Data);
        }

        // GET: CarritoController_MVC/Details/5
        public async Task<ActionResult> Details(int id)
        {

            ServiceResult result = await _carritoService.GetCarritoById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);

        }

        // GET: CarritoController_MVC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CarritoController_MVC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Create(CreateCarritoDTO createCarritoDTO)
        {
         
            try
            {
                var result = await _carritoService.CreateCarrito(createCarritoDTO);
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

        // GET: CarritoController_MVC/Edit/5
        public async Task<ActionResult> Edit(int id)
        {

            ServiceResult result = await _carritoService.GetCarritoById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);



        }

        // POST: CarritoController_MVC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateCarritoDTO updateCarritoDTO)
        {

            try
            {
                ServiceResult result = await _carritoService.UpdateCarrito(updateCarritoDTO);

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
