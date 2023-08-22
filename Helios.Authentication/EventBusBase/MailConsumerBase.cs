using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Helios.Authentication.EventBusBase
{
    public abstract class MailConsumerBase<T> : HeliosConsumerBase<T> where T : class
    {
        protected string host;
        protected int port;
        protected bool enableSSL;
        protected string userName;
        protected string password;
        protected string DisplayName;
        protected readonly SmtpClient smtpClient;
        protected readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment environment;
        public MailConsumerBase(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, IConfiguration config, SmtpClient smtpClient) : base(config)
        {

            this.host = config["EmailSender:Host"];
            this.port = config.GetValue<int>("EmailSender:Port");
            this.enableSSL = config.GetValue<bool>("EmailSender:EnableSSL");
            this.userName = config["EmailSender:UserName"];
            this.password = config["EmailSender:Password"];
            this.DisplayName = config["EmailSender:DisplayName"];
            this.environment = environment;
            this.smtpClient = smtpClient;
        }

        public override string QueueName => _config["AppConfig:RabbitMailQueueName"];


        protected bool IsValidEmail(string email)
        {

            if (string.IsNullOrWhiteSpace(email))
                return false;


            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);


            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}