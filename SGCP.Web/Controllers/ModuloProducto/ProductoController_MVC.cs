using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloProducto.Producto;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Interfaces.ModuloProducto;

namespace SGCP.Web.Controllers.ModuloProducto
{
    public class ProductoController_MVC : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductoController_MVC> _logger;

        public ProductoController_MVC(IProductoService productoService, ILogger<ProductoController_MVC> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        // GET: ProductoController
        public async Task<ActionResult> Index()
        {

            ServiceResult result = await _productoService.GetProducto();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: ProductoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ServiceResult result = await _productoService.GetProductoById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: ProductoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductoDTO createProductoDTO)
        {
            try
            {
                ServiceResult result = await _productoService.CreateProducto(createProductoDTO);

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

        // GET: ProductoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ServiceResult result = await _productoService.GetProductoById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: ProductoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateProductoDTO updateProductoDTO)
        {
            try
            {
                ServiceResult result = await _productoService.UpdateProducto(updateProductoDTO);

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
