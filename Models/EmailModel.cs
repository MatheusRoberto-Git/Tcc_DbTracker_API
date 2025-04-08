namespace Tcc_DbTracker_API.Models
{
    public class EmailModel
    {
        public List<string> Destinatarios { get; set; }

        public string Remetente { get; set; }

        public string NomeRemetente { get; set; }

        public string SenhaApp { get; set; }
    }
}