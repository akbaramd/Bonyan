using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonyan.IdentityManagement.BonWeb.Mvc.Controllers.IdentityManagement;

[Area("IdentityManagement")]
[Authorize]
[Route("IdentityManagement/[controller]")]
public class UsersController : Controller
{
    [HttpGet]
    [Route("")]
    [Route("[action]")]
    public IActionResult Index()
    {
        return View();
    }
}
