using Helios.eCRF.Controllers.Base;
using Helios.eCRF.Models;
using Helios.eCRF.Services;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Mail;

namespace Helios.eCRF.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService authService;

        public AccountController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] AccountModel user)
        {
            var result = await authService.LoginAsync(user);

            return Ok("Form data received successfully");
        }

        [HttpPost]
        public async Task<bool> AddTenant(string name)
        {
            var model = new TenantModel { Name = name };
            var result = await authService.AddTenant(model);
            return result;
            //return Ok("Form data received successfully"); 
        }
    
        [HttpGet]
        public async Task<List<TenantModel>> GetTenantList()
        {
            var result = await authService.GetTenantList();

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(UserDTO model)
        {
            var result = await authService.AddUser(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PassiveOrActiveUser(UserDTO model)
        {
            var result = await authService.PassiveOrActiveUser(model);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> SendNewPasswordForUser(Guid userId)
        {
            var result = await authService.SendNewPasswordForUser(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserDTO model)
        {
            var result = await authService.UpdateUser(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfileResetPassword(UserDTO model)
        {
            var result = await authService.UpdateUser(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactUsDTO contactUsModel)
        {

            //var verified = true;
            //TokenResponseModel tokenResponse = new TokenResponseModel() { Success = false };

            //var values = new
            //{
            //    secret = googleSecretKey,
            //    response = captcha,
            //};
            //JsonContent content = JsonContent.Create(values);
            //using (var client = _clientFactory.CreateClient())
            //{
            //    var response = await client.PostAsync($"{googleRecaptchaVerifyApi}", content);
            //    var responseString = await response.Content.ReadAsStringAsync();
            //    tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(responseString);
            //}

            //// Recaptcha V3 Verify Api send score 0-1. If score is low such as less than 0.5, you can think that it is a bot and return false.     
            //// If token is not success then return false
            //if (!tokenResponse.Success)
            //    verified = false;

            //if (!verified)
            ////if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            //{

            //    ViewBag.captcha = "Captcha validation failed.";
            //    ViewBag.Success = false;
            //    return View(new ContactUsDTO());
            //}


            try
            {

                //var client = new SmtpClient("smtp.office365.com", 587)
                //{
                //    Credentials = new NetworkCredential("accounts@helios-crf.com", "Vuw24048"),
                //    EnableSsl = true
                //};


                //var subject = "Helios e-CRF Contact Us";
                //var mailadress = new List<MailAddress>() {
                //    new MailAddress("accounts@helios-crf.com"),
                //    new MailAddress("zeynepmineh@monitorcro.com"),
                //    new MailAddress("inans@monitorcro.com"),
                //    new MailAddress("cano@monitorcro.com"),
                //    new MailAddress("gamzea@monitorcro.com")
                //};
                await authService.ContactUsMailAsync(contactUsModel);
                return Ok(true);

                //await emailservice.ContactUsMailAsync(contactUsModel.Name, contactUsModel.Email, contactUsModel.Company, contactUsModel.StudyCode, contactUsModel.Message, mailadress, subject, environment, this.httpContextAccessor);
                //return Json(true);

                //string messageBody = $"{contactUsModel.Name}<br><br>{contactUsModel.Email}<br><br>{contactUsModel.Company}<br> Study : {contactUsModel.StudyCode}<br><br>Mesaj : {contactUsModel.Message}";

                //MailAddress inan = new MailAddress("inans@monitorcro.com");
                //MailAddress canMail = new MailAddress("cano@monitorcro.com");
                //MailAddress gamzeMail = new MailAddress("gamzea@monitorcro.com");
                //var mailMessage = new MailMessage("accounts@helios-crf.com", "zeynepmineh@monitorcro.com", "Helios e-CRF Contact Us", messageBody)
                //{
                //    IsBodyHtml = true,
                //};
                //mailMessage.CC.Add(inan);
                //mailMessage.CC.Add(canMail);
                //mailMessage.CC.Add(gamzeMail);
                //_SmtpClient.Send(mailMessage);
                //ViewBag.Success = true;
                //return Json(true);
            }
            catch (Exception ex)
            {
                return NotFound(false);
            }

        }
    }
}
