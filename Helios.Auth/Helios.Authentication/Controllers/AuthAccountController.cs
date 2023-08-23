using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Helios.Authentication.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Security.Principal;

namespace Helios.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthAccountController : Controller
    {
        private AuthenticationContext _context;
        readonly UserManager<ApplicationUser> _userManager;
        private readonly IBus _backgorundWorker;

        public AuthAccountController(AuthenticationContext context, UserManager<ApplicationUser> userManager, IBus _bus)
        {
            _context = context;
            _userManager = userManager;
            _backgorundWorker = _bus;
        }

        //login
    }
}
