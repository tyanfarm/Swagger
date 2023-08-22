using Microsoft.AspNetCore.Mvc;
using MyProject1.MyLogging;

namespace MyProject1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : Controller
    {
        // 1. Strongly coupled / tightly coupled
        // readonly (chỉ đọc) là khai báo hằng
        private readonly IMyLogger _myLogger;

        // Constructor
        public DemoController()
        {
            _myLogger = new LogToDB();
        }

        [HttpGet]
        public IActionResult Index()
        {
            _myLogger.Log("Index method start");
            return Ok();
        }
    }
}
