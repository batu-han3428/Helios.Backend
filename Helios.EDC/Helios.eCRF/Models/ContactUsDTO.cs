using MassTransit.Futures.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace Helios.eCRF.Models
{
    public class ContactUsDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string Message { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string StudyCode { get; set; }
        public string Subject { get => "Helios e-CRF Contact Us"; }
        public List<MailAddress> MailAddresses { get =>
            new List<MailAddress>()
            {
                //new MailAddress("accounts@helios-crf.com"),
                //new MailAddress("zeynepmineh@monitorcro.com"),
                //new MailAddress("inans@monitorcro.com"),
                //new MailAddress("cano@monitorcro.com"),
                //new MailAddress("gamzea@monitorcro.com")
                 new MailAddress("batu_6407@hotmail.com.tr")
            };
        }
        //public List<MailAddress> GetMailAddresses()
        //{
        //    return new List<MailAddress>()
        //    {
        //        new MailAddress("accounts@helios-crf.com"),
        //            new MailAddress("zeynepmineh@monitorcro.com"),
        //            new MailAddress("inans@monitorcro.com"),
        //            new MailAddress("cano@monitorcro.com"),
        //            new MailAddress("gamzea@monitorcro.com")
        //        };
        //}
    }
}


//new MailAddress("zeynepmineh@monitorcro.com"); new MailAddress("inans@monitorcro.com"); new MailAddress("cano@monitorcro.com"); new MailAddress("gamzea@monitorcro.com");