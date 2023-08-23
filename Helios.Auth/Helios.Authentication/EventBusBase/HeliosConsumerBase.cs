using MassTransit;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Helios.Authentication.EventBusBase
{
    public abstract class HeliosConsumerBase<T> : IHeliosConsumer<T> where T : class
    {
        protected readonly IConfiguration _config;
        public HeliosConsumerBase(IConfiguration config)
        {
            _config = config;
        }
        public abstract string QueueName { get; }

        public Uri RabbitMqUri => new Uri(_config["AppConfig:RabbitMQMail"]);

        public string Username => _config["AppConfig:RabbitMQUserName"];

        public string Password => _config["AppConfig:RabbitMQPassword"];

        public virtual Task Consume(ConsumeContext<T> context)
        {
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
        }

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

    public interface IHeliosConsumer<T> : IConsumer<T>, IDisposable where T : class
    {
        string QueueName { get; }
        Uri RabbitMqUri { get; }
        string Username { get; }
        string Password { get; }
    }
}
