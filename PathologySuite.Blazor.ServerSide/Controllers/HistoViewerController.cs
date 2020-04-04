using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PathologySuite.Blazor.ServerSide.Controllers
{
    public class HistoViewerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
