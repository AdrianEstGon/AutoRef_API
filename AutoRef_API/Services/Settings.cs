namespace AutoRef_API.Services
{
    public class MailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }

    public class GoogleMapsSettings
    {
        public string ApiKey { get; set; }
    }
}
