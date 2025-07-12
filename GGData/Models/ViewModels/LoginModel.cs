namespace GGData.Models.ViewModels
{
    /// <summary>
    /// Modelo para a autenticação de utilizadores via login.
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Email do utilizador que pretende autenticar-se.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Palavra-passe do utilizador.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
