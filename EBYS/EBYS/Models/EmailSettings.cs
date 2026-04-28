namespace EBYS.Models
{
    public class EmailSettings
    {
        public string Sender { get; set; } = "";
        public string Password { get; set; } = "";
        public string Smtp { get; set; } = "";
        public int Port { get; set; }
    }
}
