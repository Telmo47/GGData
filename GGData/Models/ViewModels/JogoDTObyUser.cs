/// <summary>
/// Data Transfer Object para apresentar informações de jogos formatadas para o utilizador.
/// </summary>
public class JogoDTObyUser
{
    /// <summary>
    /// Identificador único do jogo.
    /// </summary>
    public int JogoId { get; set; }

    /// <summary>
    /// Nome do jogo.
    /// </summary>
    public string Nome { get; set; }

    /// <summary>
    /// Plataforma em que o jogo está disponível (ex: PC, Xbox, PS5).
    /// </summary>
    public string Plataforma { get; set; }

    /// <summary>
    /// Data de lançamento do jogo.
    /// </summary>
    public DateTime DataLancamento { get; set; }

    /// <summary>
    /// Géneros do jogo concatenados numa única string (ex: "Ação, Aventura").
    /// </summary>
    public string Genero { get; set; }

    /// <summary>
    /// URL da imagem do jogo (pode ser nula).
    /// </summary>
    public string? ImagemUrl { get; set; }
}
