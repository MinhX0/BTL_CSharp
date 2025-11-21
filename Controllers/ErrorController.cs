using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("{statusCode}")]
        public IActionResult StatusCodeHandler(int statusCode)
        {
            Response.StatusCode = statusCode;
            // Return a generic status code view that accepts the status code as the model
            return View("StatusCode", statusCode);
        }

        [Route("/Error/500")]
        public IActionResult Exception()
        {
            Response.StatusCode = 500;
            // Use generic status code view as well
            return View("StatusCode", 500);
        }
    }
}
