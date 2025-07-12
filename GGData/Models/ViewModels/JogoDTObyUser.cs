public class JogoDTObyUser
{
    public int JogoId { get; set; }
    public string Nome { get; set; }
    public string Plataforma { get; set; }
    public DateTime DataLancamento { get; set; }
    public string Genero { get; set; } // para o GET: string concatenada

    // Para o POST, tens outro DTO separado
}


