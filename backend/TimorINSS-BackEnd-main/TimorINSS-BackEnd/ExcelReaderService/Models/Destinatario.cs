using CsvHelper.Configuration.Attributes;

namespace TimorINSSBackEnd.ExcelReaderService.Models
{
    public class Destinatario: Pagamento
    {
        [Name("Niss")]
        public string Niss { get; set; }

        [Name("Tin")]
        public string Tin { get; set; }

        [Name("Name")]
        public string Name { get; set; }

        [Name("Address")]
        public string Address { get; set; }
    }

    public class Pagamento
    {

        [Name("IBAN")]
        public string IBAN { get; set; }

        [Name("Swift")]
        public string Swift { get; set; }

        [Name("Account Number")]
        public string AccountNumber { get; set; }

        [Name("Amount")]
        public decimal? Amount { get; set; }

    }
}
