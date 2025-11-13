using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloPedido.Pedido;
using SGCP.Application.Interfaces.ModuloPedido;

namespace SGCP.Web.Controllers.ModuloPedido
{
    public class PedidoController_MVC : Controller
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController_MVC(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }


        // GET: PedidoController_MVC
        public async Task<ActionResult> Index()
        {
            ServiceResult result = await _pedidoService.GetPedido();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);

        }

        // GET: PedidoController_MVC/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ServiceResult result = await _pedidoService.GetPedidoById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);

        }

        // GET: PedidoController_MVC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PedidoController_MVC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreatePedidoDTO createPedidoDTO)
        {
            try
            {
                ServiceResult result = await _pedidoService.CreatePedido(createPedidoDTO);
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

        // GET: PedidoController_MVC/Edit/5
        public async Task<ActionResult> Edit(int id)
        {

            ServiceResult result = await _pedidoService.GetPedidoById(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);

        }

        // POST: PedidoController_MVC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdatePedidoDTO updatePedidoDTO)
        {
            try
            {
                ServiceResult result = await _pedidoService.UpdatePedido(updatePedidoDTO);

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
