using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using DAL;
using DAL.Models;

namespace SecSisBoletas.Areas.Empresas.Controllers
{
    [Authorize(Roles = "Empresa")]
    public class BoletaAportesController : Controller
    {
        private SecModel db = new SecModel();

        // GET: Empresas/BoletaAportes
        public ActionResult Index(int? mes, int? anio, int estadoPago = 0)
        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst("IdEmpresa");
            int IdEmpresa = Convert.ToInt32(claim.Value);
            var boletaAportes = db.BoletaAportes.Include(b => b.DeclaracionJurada)
                                                .Where(x => x.DeclaracionJurada.idEmpresa == IdEmpresa && x.DeBaja == false);

            if (estadoPago == 1)
            {
                boletaAportes = boletaAportes.Where(x => x.BoletaPagada == true);
            }
            if (estadoPago == 2)
            {
                boletaAportes = boletaAportes.Where(x => x.BoletaPagada == false);
            }

            if (mes != null && mes != 0 && anio != null && anio != 0)
            {
                boletaAportes = boletaAportes.Where(x => x.MesBoleta == mes && x.AnioBoleta == anio);
            }

            foreach (var boleta in boletaAportes)
            {
                decimal mora = (boleta.RecargoMora != null) ? (decimal)boleta.RecargoMora : 0;
                boleta.TotalDepositado = TruncateFunction(boleta.Aportes + boleta.AportesAfiliados + mora, 2);
            }

            ViewBag.Mes = mes;
            ViewBag.Anio = anio;
            ViewBag.estadoPago = estadoPago;

            return View(boletaAportes.ToList());
        }

        // GET: Empresas/BoletaAportes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BoletaAportes boletaAportes = db.BoletaAportes.Find(id);

            if (boletaAportes == null)
            {
                return HttpNotFound();
            }

            DeclaracionJurada ddjj = db.DeclaracionJurada.Where(x => x.IdDeclaracionJurada == boletaAportes.IdDeclaracionJurada).FirstOrDefault();

            decimal mora = (boletaAportes.RecargoMora != null) ? (decimal)boletaAportes.RecargoMora : 0;
            boletaAportes.TotalDepositado = TruncateFunction(boletaAportes.Aportes + boletaAportes.AportesAfiliados + mora, 2);

            ViewBag.IdEmpresa = ddjj.idEmpresa;

            return View(boletaAportes);
        }

        // GET: Empresas/BoletaAportes/Create
        public ActionResult Create()
        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst("IdEmpresa");
            int IdEmpresa = Convert.ToInt32(claim.Value);

            var declaracionesJuradas = db.DeclaracionJurada.Where(x => x.idEmpresa == IdEmpresa).ToList();
            foreach (DeclaracionJurada declaracion in declaracionesJuradas)
            {
                declaracion.MesAnio = declaracion.mes + "/" + declaracion.anio;
            }
            ViewBag.TotalSueldo = 0;

            ViewBag.IdDeclaracionJurada = new SelectList(declaracionesJuradas, "IdDeclaracionJurada", "MesAnio");
            return View();
        }

        // POST: Empresas/BoletaAportes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdBoleta,IdDeclaracionJurada,MesBoleta,AnioBoleta")] BoletaAportes boletaAportes)
        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst("IdEmpresa");
            int IdEmpresa = Convert.ToInt32(claim.Value);

            var declaracionesJuradas = db.DeclaracionJurada.Where(x => x.idEmpresa == IdEmpresa).ToList();

            foreach (DeclaracionJurada declaracion in declaracionesJuradas)
            {
                declaracion.MesAnio = declaracion.mes + "/" + declaracion.anio;
            }

            ViewBag.IdDeclaracionJurada = new SelectList(declaracionesJuradas, "IdDeclaracionJurada", "MesAnio", boletaAportes.IdDeclaracionJurada);

            int cantEmpleadosContratados = db.EmpleadoEmpresa.Where(x => x.idEmpresa == IdEmpresa &&
                                                              x.FechaAlta.Month <= boletaAportes.MesBoleta &&
                                                              x.FechaAlta.Year <= boletaAportes.AnioBoleta &&
                                                              (x.FechaBaja.Value == null)).Count();

            int cantDetalleDeclaracion = db.DetalleDeclaracionJurada.Include(t => t.DeclaracionJurada)
                                                                    .Include(t => t.EmpleadoEmpresa)
                                                                    .Where(x => x.EmpleadoEmpresa.idEmpresa == IdEmpresa &&
                                                                                x.DeclaracionJurada.mes == boletaAportes.MesBoleta &&
                                                                                x.DeclaracionJurada.anio == boletaAportes.AnioBoleta).Count();

            if(cantDetalleDeclaracion < cantEmpleadosContratados)
            {
                ModelState.AddModelError("IdDeclaracionJurada", "No todos los empleados contratados estan declarados en la declararion jurada");
                return View(boletaAportes);
            }

            if (db.BoletaAportes.Where(x => x.IdDeclaracionJurada == boletaAportes.IdDeclaracionJurada && x.DeBaja == false).FirstOrDefault() != null)
            {
                ModelState.AddModelError("IdDeclaracionJurada", "Ya Existe una boleta Generada para este mes y año");
                return View(boletaAportes);
            }

            var ddjj = db.DeclaracionJurada.Where(x => x.IdDeclaracionJurada == boletaAportes.IdDeclaracionJurada).FirstOrDefault();

            //boletaAportes.TotalSueldos = db.DetalleDeclaracionJurada.Where(x => x.IdDeclaracionJurada == boletaAportes.IdDeclaracionJurada).Sum(x => x.Sueldo);
            var detalles = db.DetalleDeclaracionJurada.Where(x => x.IdDeclaracionJurada == boletaAportes.IdDeclaracionJurada).ToList();

            int cantEmpleados = 0, cantAfiliado = 0;

            #region Calcular Aportes
            var CalcularAportes = db.ParametrosGenerales.AsNoTracking().Where(x => x.Key == "CalcularAportes").FirstOrDefault();

            if (CalcularAportes == null)
            {
                CalcularAportes = new ParametroGeneral();
                CalcularAportes.Key = "CalcularAportes";
                CalcularAportes.Value = "True";
            }
            #endregion

            #region CalcularAportesAfiliados
            var CalcularAportesAfiliados = db.ParametrosGenerales.AsNoTracking().Where(x => x.Key == "CalcularAportesAfiliados").FirstOrDefault();

            if (CalcularAportesAfiliados == null)
            {
                CalcularAportesAfiliados = new ParametroGeneral();
                CalcularAportesAfiliados.Key = "CalcularAportesAfiliados";
                CalcularAportesAfiliados.Value = "True";
            }
            #endregion

            foreach (var detalle in detalles)
            {
                var empEmpAux = db.EmpleadoEmpresa.Where(x => x.idEmpleadoEmpresa == detalle.IdEmpleadoEmpresa).FirstOrDefault();

                var empleado = db.Empleado.Where(x => x.IdEmpleado == empEmpAux.idEmpleado).FirstOrDefault();
                
                if(CalcularAportes.Value == "True")
                {
                    cantEmpleados++;
                    boletaAportes.TotalSueldos += detalle.Sueldo;
                }

                if(CalcularAportesAfiliados.Value == "True")
                {
                    var afiliado = db.Afiliado.Where(x => x.IdEmpleadoEmpresa == empEmpAux.idEmpleadoEmpresa).FirstOrDefault();
                    if (afiliado != null)
                    {
                        if (afiliado.FechaAlta.Year < ddjj.anio)
                        {
                            if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > ddjj.anio || (afiliado.FechaBaja.Value.Year == ddjj.anio && afiliado.FechaBaja.Value.Month >= ddjj.mes))
                            {
                                cantAfiliado++;
                                boletaAportes.TotalSueldosAfiliados += (empEmpAux.IdJornada == 1 || empEmpAux.IdJornada == 2) ? detalle.SueldoBase.Value : detalle.Sueldo;
                            }
                        }
                        else if (afiliado.FechaAlta.Year == ddjj.anio && afiliado.FechaAlta.Month <= ddjj.mes)
                        {
                            if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > ddjj.anio || (afiliado.FechaBaja.Value.Year == ddjj.anio && afiliado.FechaBaja.Value.Month >= ddjj.mes))
                            {
                                cantAfiliado++;
                                boletaAportes.TotalSueldosAfiliados += (empEmpAux.IdJornada == 1 || empEmpAux.IdJornada == 2) ? detalle.SueldoBase.Value : detalle.Sueldo;
                            }
                        }
                    }
                }
            }

            #region Aportes
            var porcAportes = db.ParametrosGenerales.AsNoTracking().Where(x => x.Key == "PorcAportes").FirstOrDefault();

            if(porcAportes == null)
            {
                porcAportes = new ParametroGeneral();
                porcAportes.Key = "PorcAportes";
                porcAportes.Value = "2";
            }

            boletaAportes.CantEmpleados = cantEmpleados;
            boletaAportes.Aportes = TruncateFunction((boletaAportes.TotalSueldos / 100) * decimal.Parse(porcAportes.Value), 2);
            #endregion

            #region AportesAfiliados
            var porcAportesAfiliados = db.ParametrosGenerales.AsNoTracking().Where(x => x.Key == "PorcAportesAfiliados").FirstOrDefault();

            if (porcAportesAfiliados == null)
            {
                porcAportesAfiliados = new ParametroGeneral();
                porcAportesAfiliados.Key = "PorcAportesAfiliados";
                porcAportesAfiliados.Value = "5";
            }

            boletaAportes.CantAfiliados = cantAfiliado;
            boletaAportes.AportesAfiliados = TruncateFunction((boletaAportes.TotalSueldosAfiliados / 100) * decimal.Parse(porcAportesAfiliados.Value), 2);
            #endregion

            boletaAportes.FechaBoleta = DateTime.Today;
            boletaAportes.FechaVencimiento = GenerarVencimiento(boletaAportes.MesBoleta, boletaAportes.AnioBoleta);
            boletaAportes.BoletaPagada = false;
            boletaAportes.RecargoMora = 0;

            if (ModelState.IsValid)
            {
                db.BoletaAportes.Add(boletaAportes);
                db.SaveChanges();
                return RedirectToAction("CreateMessage");
            }
            
            return View(boletaAportes);
        }

        public ActionResult CreateMessage()
        {
            return View();
        }

        public ActionResult DeleteMessage()
        {
            return View();
        }

        public ActionResult CantDeleteMessage()
        {
            return View();
        }

        public DateTime GenerarVencimiento(int mes, int anio)
        {
            //if(mes != 12)
            //{
                return db.FechaVencimiento.Where(x => x.mesBoleta == mes && x.anioBoleta == anio).FirstOrDefault().FechaVto;
            //}
            //else
            //{
            //    return db.FechaVencimiento.Where(x => x.FechaVto.Month == 1 && x.FechaVto.Year == (anio + 1)).FirstOrDefault().FechaVto;
            //}
        }

        public decimal TruncateFunction(decimal number, int digits)
        {
            decimal stepper = (decimal)(Math.Pow(10.0, (double)digits));
            int temp = (int)(stepper * number);
            return (decimal)temp / stepper;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
