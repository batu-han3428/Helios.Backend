using Helios.Core.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace Helios.Core.Controllers
{
    public class CoreSubjectController : Controller
    {
        private CoreContext _context;

        public CoreSubjectController(CoreContext context)
        {
            _context = context;
        }
    }
}
