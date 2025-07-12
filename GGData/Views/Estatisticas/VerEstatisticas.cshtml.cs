using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GGData.Views.Estatisticas
{
    /// <summary>
    /// Modelo da página Razor para visualizar as estatísticas.
    /// Esta página não tem lógica complexa no servidor,
    /// apenas uma ação GET vazia que prepara a página para renderizar.
    /// </summary>
    public class VerEstatisticasModel : PageModel
    {
        /// <summary>
        /// Método chamado quando a página é acedida via GET.
        /// Atualmente não realiza nenhuma ação.
        /// </summary>
        public void OnGet()
        {
            // Sem lógica no servidor para esta página
        }
    }
}
