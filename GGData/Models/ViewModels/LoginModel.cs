namespace GGData.Models.ViewModels
{
    /// <summary>
    /// Modelo usado para representar os dados de login de um utilizador.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Email do utilizador. Serve como nome de utilizador para autenticação.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Palavra-passe do utilizador.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
