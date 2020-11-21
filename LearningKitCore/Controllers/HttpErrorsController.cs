using Microsoft.AspNetCore.Mvc;

namespace LearningKitCore.Controllers
{
    public class HttpErrorsController : Controller
    {
        public IActionResult Error(int code)
        {
            if (code == 404)
            {
                return View("NotFound");
            }

            return StatusCode(code);
        }
    }
}
