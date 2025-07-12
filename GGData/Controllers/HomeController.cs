using Microsoft.AspNetCore.Mvc;
using GGData.Data;
using GGData.Models;
using System.Diagnostics;

namespace GGData.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Ação principal que obtém a lista de jogos da base de dados e passa para a view Index.
        /// </summary>
        public IActionResult Index()
        {
            var jogos = _context.Jogos.ToList();
            return View(jogos);
        }

        /// <summary>
        /// Ação que devolve a view Privacy (política de privacidade).
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Ação para tratamento de erros.
        /// Não guarda cache da resposta e apresenta uma view com detalhes do erro e RequestId.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
