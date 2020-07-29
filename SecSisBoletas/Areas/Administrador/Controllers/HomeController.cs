using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecSisBoletas.Areas.Administrador.Controllers
{
    [Authorize(Roles = "Admin, Fiscalizacion, Finanzas")]
    public class HomeController : Controller
    {
        // GET: Administrador/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}