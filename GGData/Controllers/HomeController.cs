using Microsoft.AspNetCore.Mvc;
using GGData.Data;
using GGData.Models;
using System.Diagnostics;

namespace GGData.Controllers
{
    /// <summary>
    /// Controlador principal para a página inicial e páginas básicas como privacidade e erros.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que injeta logger e contexto da BD.
        /// </summary>
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Ação para a página inicial.
        /// Lista todos os jogos na BD e passa à View.
        /// </summary>
        public IActionResult Index()
        {
            var jogos = _context.Jogos.ToList();
            return View(jogos);
        }

        /// <summary>
        /// Página de política de privacidade.
        /// Apenas retorna a View.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Página de erro.
        /// Mostra detalhes básicos do erro atual.
        /// Sem cache para garantir que a informação está sempre atualizada.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
