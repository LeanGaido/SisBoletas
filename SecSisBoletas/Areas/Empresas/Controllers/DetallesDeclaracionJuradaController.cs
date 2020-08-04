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
    public class DetallesDeclaracionJuradaController : Controller
    {
        private SecModel db = new SecModel();
        private DeclaracionesJuradasController ddjj = new DeclaracionesJuradasController();

        // GET: DetallesDeclaracionJurada
        public ActionResult Index(int id, int idEmpleadoEmpresa = 0)
        {
            ViewBag.Errores = TempData["MensajeError"];
            Session["MensajeError"] = "";
            var detalleDeclaracionJurada = (db.DetalleDeclaracionJurada.Include(d => d.Categoria)
                                                                       .Include(d => d.DeclaracionJurada)
                                                                       .Include(d => d.EmpleadoEmpresa)
                                                                       .Include(d => d.Jornada)
                                                                       .Where(x => x.IdDeclaracionJurada == id)).ToList();
            ViewBag.IdDeclaracionJurada = id;
            decimal sueldos2 = 0, sueldos5 = 0;
            foreach (DetalleDeclaracionJurada detalle in detalleDeclaracionJurada)
            {
                #region Old
                /*
                detalle.EsAfiliado = false;
                var afiliado = db.Afiliado.Where(x => x.IdEmpleado == detalle.EmpleadoEmpresa.idEmpleado).FirstOrDefault();
                if (afiliado != null)
                {
                    if (afiliado.FechaAlta.Year < detalle.DeclaracionJurada.anio)
                    {
                        if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > detalle.DeclaracionJurada.anio || (afiliado.FechaBaja.Value.Year == detalle.DeclaracionJurada.anio && afiliado.FechaBaja.Value.Month >= detalle.DeclaracionJurada.mes))
                        {
                            detalle.EsAfiliado = true;
                        }
                    }
                    else if (afiliado.FechaAlta.Year == detalle.DeclaracionJurada.anio && afiliado.FechaAlta.Month <= detalle.DeclaracionJurada.mes)
                    {
                        if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > detalle.DeclaracionJurada.anio || (afiliado.FechaBaja.Value.Year == detalle.DeclaracionJurada.anio && afiliado.FechaBaja.Value.Month >= detalle.DeclaracionJurada.mes))
                        {
                            detalle.EsAfiliado = true;
                        }
                    }
                }

                detalle.LicenciaEmpleado = false;
                foreach (var licencia in db.LicenciaEmpleado.Where(x => x.IdEmpleadoEmpresa == detalle.IdEmpleadoEmpresa))
                {
                    int mes = detalle.DeclaracionJurada.mes;
                    int anio = detalle.DeclaracionJurada.anio;

                    if (licencia.FechaBajaLicencia.Value.Year == anio)
                    {
                        if (licencia.FechaAltaLicencia.Month <= mes && licencia.FechaBajaLicencia.Value.Month >= mes)
                        {
                            detalle.LicenciaEmpleado = true;
                        }
                    }
                    if (licencia.FechaBajaLicencia.Value.Year > anio)
                    {
                        if (licencia.FechaAltaLicencia.Year <= anio && licencia.FechaAltaLicencia.Month <= mes)
                        {
                            detalle.LicenciaEmpleado = true;
                        }
                    }
                    //if (licencia != null && licencia.FechaAltaLicencia.Year == detalle.DeclaracionJurada.anio)
                    //{
                    //    if (licencia.FechaBajaLicencia.Value.Year == detalle.DeclaracionJurada.anio)
                    //    {
                    //        if (licencia.FechaAltaLicencia.Month <= detalle.DeclaracionJurada.mes && licencia.FechaBajaLicencia.Value.Month >= detalle.DeclaracionJurada.mes)
                    //        {
                    //            detalle.LicenciaEmpleado = true;
                    //        }
                    //    }
                    //    if (licencia.FechaBajaLicencia.Value.Year > detalle.DeclaracionJurada.anio)
                    //    {
                    //        if (licencia.FechaBajaLicencia.Value.Month >= detalle.DeclaracionJurada.mes)
                    //        {
                    //            detalle.LicenciaEmpleado = true;
                    //        }
                    //    }
                    //}
                    //if (licencia.FechaBajaLicencia.Value.Year >= detalle.DeclaracionJurada.anio)
                    //{
                    //    if (licencia.FechaBajaLicencia.Value.Month >= detalle.DeclaracionJurada.mes)
                    //    {
                    //        detalle.LicenciaEmpleado = true;
                    //    }
                    //}
                }
                */
                #endregion
                detalle.EsAfiliado = false;
                sueldos2 += detalle.Sueldo;
                var afiliado = db.Afiliado.Where(x => x.IdEmpleadoEmpresa == detalle.IdEmpleadoEmpresa).FirstOrDefault();
                if (afiliado != null)
                {
                    if (afiliado.FechaAlta.Year < detalle.DeclaracionJurada.anio)
                    {
                        if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > detalle.DeclaracionJurada.anio || (afiliado.FechaBaja.Value.Year == detalle.DeclaracionJurada.anio && afiliado.FechaBaja.Value.Month >= detalle.DeclaracionJurada.mes))
                        {
                            sueldos5 += detalle.SueldoBase.Value;
                            detalle.EsAfiliado = true;
                        }
                    }
                    else if (afiliado.FechaAlta.Year == detalle.DeclaracionJurada.anio && afiliado.FechaAlta.Month <= detalle.DeclaracionJurada.mes)
                    {
                        if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > detalle.DeclaracionJurada.anio || (afiliado.FechaBaja.Value.Year == detalle.DeclaracionJurada.anio && afiliado.FechaBaja.Value.Month >= detalle.DeclaracionJurada.mes))
                        {
                            sueldos5 += detalle.SueldoBase.Value;
                            detalle.EsAfiliado = true;
                        }
                    }
                }

                detalle.LicenciaEmpleado = false;
                foreach (var licencia in db.LicenciaEmpleado.Where(x => x.IdEmpleadoEmpresa == detalle.IdEmpleadoEmpresa))
                {
                    #region Old
                    //if (licencia != null && licencia.FechaAltaLicencia.Year == detalle.DeclaracionJurada.anio)
                    //{
                    //    if(licencia.FechaBajaLicencia != null)
                    //    {
                    //        if (licencia.FechaBajaLicencia.Value.Year == detalle.DeclaracionJurada.anio)
                    //        {
                    //            if (licencia.FechaAltaLicencia.Month <= detalle.DeclaracionJurada.mes && licencia.FechaBajaLicencia.Value.Month >= detalle.DeclaracionJurada.mes)
                    //            {
                    //                detalle.LicenciaEmpleado = true;
                    //            }
                    //        }
                    //        if (licencia.FechaBajaLicencia.Value.Year > detalle.DeclaracionJurada.anio)
                    //        {
                    //            if (licencia.FechaBajaLicencia.Value.Month >= detalle.DeclaracionJurada.mes)
                    //            {
                    //                detalle.LicenciaEmpleado = true;
                    //            }
                    //        }
                    //    }
                    //}
                    //if(licencia.FechaBajaLicencia != null)
                    //{
                    //    if (licencia != null && licencia.FechaBajaLicencia.Value.Year >= detalle.DeclaracionJurada.anio)
                    //    {
                    //        if (licencia.FechaBajaLicencia.Value.Month >= detalle.DeclaracionJurada.mes)
                    //        {
                    //            detalle.LicenciaEmpleado = true;
                    //        }
                    //    }
                    //}
                    #endregion

                    if (licencia.IdLicenciaLaboral != 3)
                    {
                        if (licencia.FechaAltaLicencia.Year < detalle.DeclaracionJurada.anio ||
                            (licencia.FechaAltaLicencia.Year == detalle.DeclaracionJurada.anio && licencia.FechaAltaLicencia.Month <= detalle.DeclaracionJurada.mes))
                        {
                            if(licencia.FechaBajaLicencia.HasValue)
                            {
                                if (licencia.FechaBajaLicencia.Value.Year == detalle.DeclaracionJurada.anio)
                                {
                                    if (licencia.FechaBajaLicencia.Value.Month >= detalle.DeclaracionJurada.mes)
                                    {
                                        detalle.LicenciaEmpleado = true;
                                    }
                                }
                                else if (licencia.FechaBajaLicencia.Value.Year > detalle.DeclaracionJurada.anio)
                                {
                                    detalle.LicenciaEmpleado = true;
                                }
                            }
                            else
                            {
                                detalle.LicenciaEmpleado = true;
                            }
                        }
                    }
                    else
                    {

                        if (detalle.DeclaracionJurada.mes != 1)
                        {
                            if (licencia.FechaBajaLicencia.HasValue)
                            {
                                if (licencia.FechaBajaLicencia.Value.Year == detalle.DeclaracionJurada.anio)
                                {
                                    if (licencia.FechaBajaLicencia.Value.Month == detalle.DeclaracionJurada.mes)
                                    {
                                        detalle.LicenciaEmpleado = true;
                                    }
                                    else if (licencia.FechaBajaLicencia.Value.Month == (detalle.DeclaracionJurada.mes - 1))
                                    {
                                        detalle.LicenciaEmpleado = true;
                                    }
                                }
                            }
                            else
                            {
                                detalle.LicenciaEmpleado = true;
                            }
                        }
                        else
                        {
                            if (licencia.FechaBajaLicencia.HasValue)
                            {
                                if (licencia.FechaBajaLicencia.Value.Year == detalle.DeclaracionJurada.anio)
                                {
                                    if (licencia.FechaBajaLicencia.Value.Month == 1)
                                    {
                                        detalle.LicenciaEmpleado = true;
                                    }
                                }
                                else if (licencia.FechaBajaLicencia.Value.Year == (detalle.DeclaracionJurada.anio - 1))
                                {
                                    if (licencia.FechaBajaLicencia.Value.Month == 12)
                                    {
                                        detalle.LicenciaEmpleado = true;
                                    }
                                }
                            }
                            else
                            {
                                detalle.LicenciaEmpleado = true;
                            }
                        }
                    }
                }
                var LiquidacionProporcional = db.LiquidacionProporcionalEmpleado.Where(x => x.IdDetalleDeclaracionJurada == detalle.IdDetalleDeclaracionJurada).FirstOrDefault();
                detalle.LiquidacionProporcional = (LiquidacionProporcional != null) ? true : false;
                detalle.IdLiquidacionProporcional = detalle.IdLiquidacionProporcional;
            }

            ViewBag.Sueldos2 = sueldos2;
            ViewBag.Sueldos5 = sueldos5;

            var claim = ((ClaimsIdentity)User.Identity).FindFirst("IdEmpresa");
            int IdEmpresa = Convert.ToInt32(claim.Value);

            ViewBag.idDeclaracion = id;
            ViewBag.IdDeclaracionJurada = new SelectList(db.DeclaracionJurada, "IdDeclaracionJurada", "IdDeclaracionJurada", id);
            DeclaracionJurada ddjj = db.DeclaracionJurada.Where(x => x.IdDeclaracionJurada == id).FirstOrDefault();

            ViewBag.Mes = ddjj.mes;
            ViewBag.Anio = ddjj.anio;

            var EmpleadosEmpresa = db.EmpleadoEmpresa.AsNoTracking().Where(x => x.idEmpresa == IdEmpresa).ToList();

            List<EmpleadoEmpresa> listaEmpleadosEmpresa = new List<EmpleadoEmpresa>();
            foreach (var EmpleadoEmpresa in EmpleadosEmpresa)
            {
                if (EmpleadoEmpresa.FechaBaja == null)
                {
                    if(EmpleadoEmpresa.FechaAlta.Year < ddjj.anio || EmpleadoEmpresa.FechaAlta.Year == ddjj.anio && EmpleadoEmpresa.FechaAlta.Month <= ddjj.mes)
                    {
                        listaEmpleadosEmpresa.Add(EmpleadoEmpresa);
                    }
                }
                else
                {
                    if (EmpleadoEmpresa.FechaBaja.Value.Year > ddjj.anio
                        && (EmpleadoEmpresa.FechaAlta.Year < ddjj.anio || EmpleadoEmpresa.FechaAlta.Year == ddjj.anio && EmpleadoEmpresa.FechaAlta.Month <= ddjj.mes))
                    {
                        listaEmpleadosEmpresa.Add(EmpleadoEmpresa);

                    }
                    else if (EmpleadoEmpresa.FechaBaja.Value.Year == ddjj.anio && EmpleadoEmpresa.FechaBaja.Value.Month >= ddjj.mes
                             && (EmpleadoEmpresa.FechaAlta.Year < ddjj.anio || EmpleadoEmpresa.FechaAlta.Year == ddjj.anio && EmpleadoEmpresa.FechaAlta.Month <= ddjj.mes))
                    {
                        listaEmpleadosEmpresa.Add(EmpleadoEmpresa);
                    }
                }
            }

            List<EmpleadoEmpresa> empleadosRestantes = new List<EmpleadoEmpresa>();

            foreach (var emp in listaEmpleadosEmpresa)
            {
                if (db.DetalleDeclaracionJurada.Where(x => x.IdDeclaracionJurada == id && x.IdEmpleadoEmpresa == emp.idEmpleadoEmpresa).FirstOrDefault() == null)
                {
                    emp.Empleado.Nombre = emp.Empleado.Apellido + ", " + emp.Empleado.Nombre;
                    empleadosRestantes.Add(emp);
                }
            }
            ViewBag.IdEmpleadoEmpresa = new SelectList(empleadosRestantes, "idEmpleadoEmpresa", "Empleado.Nombre", idEmpleadoEmpresa);
            ViewBag.IdLiquidacionProporcional = new SelectList(db.LiquidacionProporcional, "idLiquidacionProporcional", "Descripcion");
            return View(detalleDeclaracionJurada);
        }

        // GET: DetallesDeclaracionJurada/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleDeclaracionJurada detalleDeclaracionJurada = db.DetalleDeclaracionJurada.Find(id);
            if (detalleDeclaracionJurada == null)
            {
                return HttpNotFound();
            }
            ViewBag.idDeclaracion = detalleDeclaracionJurada.IdDeclaracionJurada;
            return View(detalleDeclaracionJurada);
        }

        // GET: DetallesDeclaracionJurada/Create
        public ActionResult Create(int id)
        {
            var ddjj = db.DeclaracionJurada.Find(id);

            if (ddjj == null)
            {
                return HttpNotFound();
            }

            var claim = ((ClaimsIdentity)User.Identity).FindFirst("IdEmpresa");
            int IdEmpresa = Convert.ToInt32(claim.Value);

            ViewBag.idDeclaracion = id;
            ViewBag.IdDeclaracionJurada = new SelectList(db.DeclaracionJurada, "IdDeclaracionJurada", "IdDeclaracionJurada", ddjj.IdDeclaracionJurada);
            var EmpleadosEmpresa = db.EmpleadoEmpresa.Where(x => x.idEmpresa == IdEmpresa && (x.FechaBaja.Value == null)).ToList();

            List<EmpleadoEmpresa> listaEmpleadosEmpresa = new List<EmpleadoEmpresa>();
            foreach (var EmpleadoEmpresa in EmpleadosEmpresa)
            {
                if (EmpleadoEmpresa.FechaAlta.Year < ddjj.anio)
                {
                    listaEmpleadosEmpresa.Add(EmpleadoEmpresa);
                }
                else if (EmpleadoEmpresa.FechaAlta.Year == ddjj.anio && EmpleadoEmpresa.FechaAlta.Month <= ddjj.mes)
                {
                    listaEmpleadosEmpresa.Add(EmpleadoEmpresa);
                }
            }

            List<EmpleadoEmpresa> empleadosRestantes = new List<EmpleadoEmpresa>();

            foreach (var emp in listaEmpleadosEmpresa)
            {
                if (db.DetalleDeclaracionJurada.Where(x => x.IdDeclaracionJurada == id && x.IdEmpleadoEmpresa == emp.idEmpleadoEmpresa).FirstOrDefault() == null)
                {
                    empleadosRestantes.Add(emp);
                }
            }
            ViewBag.IdEmpleadoEmpresa = new SelectList(empleadosRestantes, "idEmpleadoEmpresa", "Empleado.Nombre");
            ViewBag.IdLiquidacionProporcional = new SelectList(db.LiquidacionProporcional, "idLiquidacionProporcional", "Descripcion");

            return View();
        }

        // POST: DetallesDeclaracionJurada/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdDetalleDeclaracionJurada,IdDeclaracionJurada,IdEmpleadoEmpresa,idCategoria,idJornadaLaboral,IdLiquidacionProporcional,Sueldo,SueldoBase,Presentismo")] DetalleDeclaracionJurada detalleDeclaracionJurada)
        {
            var boleta = db.BoletaAportes.Where(x => x.IdDeclaracionJurada == detalleDeclaracionJurada.IdDeclaracionJurada && x.BoletaPagada == true && x.DeBaja == false).FirstOrDefault();
            if (boleta != null)//Si ya hay una boleta de aportes de alta relacionada a esta declaracion Jurada, no puede declarar mas empleados
            {
                return RedirectToAction("CantCreateMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = detalleDeclaracionJurada.IdDeclaracionJurada });
            }

            var claim = ((ClaimsIdentity)User.Identity).FindFirst("IdEmpresa");
            int IdEmpresa = Convert.ToInt32(claim.Value);
            var declaracionJuradaAux = db.DeclaracionJurada.Find(detalleDeclaracionJurada.IdDeclaracionJurada);
            
            ViewBag.idDeclaracion = detalleDeclaracionJurada.IdDeclaracionJurada;
            ViewBag.idCategoria = new SelectList(db.Categoria, "IdCategoria", "Descripcion", detalleDeclaracionJurada.idCategoria);
            ViewBag.IdDeclaracionJurada = new SelectList(db.DeclaracionJurada, "IdDeclaracionJurada", "IdDeclaracionJurada", detalleDeclaracionJurada.IdDeclaracionJurada);

            var EmpleadosEmpresa = db.EmpleadoEmpresa.Where(x => x.idEmpresa == IdEmpresa).ToList();
            List<EmpleadoEmpresa> listaEmpleadosEmpresa = new List<EmpleadoEmpresa>();
            foreach (var EmpleadoEmpresa in EmpleadosEmpresa)//Obtengo empleados activos al momento de realizar la declaracion Jurada
            {
                if (EmpleadoEmpresa.FechaBaja == null)
                {
                    if (EmpleadoEmpresa.FechaAlta.Year < declaracionJuradaAux.anio || EmpleadoEmpresa.FechaAlta.Year == declaracionJuradaAux.anio && EmpleadoEmpresa.FechaAlta.Month <= declaracionJuradaAux.mes)
                    {
                        listaEmpleadosEmpresa.Add(EmpleadoEmpresa);
                    }
                }
                else
                {
                    if (EmpleadoEmpresa.FechaBaja.Value.Year > declaracionJuradaAux.anio
                        && (EmpleadoEmpresa.FechaAlta.Year < declaracionJuradaAux.anio || EmpleadoEmpresa.FechaAlta.Year == declaracionJuradaAux.anio && EmpleadoEmpresa.FechaAlta.Month <= declaracionJuradaAux.mes))
                    {
                        listaEmpleadosEmpresa.Add(EmpleadoEmpresa);

                    }
                    else if (EmpleadoEmpresa.FechaBaja.Value.Year == declaracionJuradaAux.anio && EmpleadoEmpresa.FechaBaja.Value.Month >= declaracionJuradaAux.mes
                             && (EmpleadoEmpresa.FechaAlta.Year < declaracionJuradaAux.anio || EmpleadoEmpresa.FechaAlta.Year == declaracionJuradaAux.anio && EmpleadoEmpresa.FechaAlta.Month <= declaracionJuradaAux.mes))
                    {
                        listaEmpleadosEmpresa.Add(EmpleadoEmpresa);
                    }
                }
            }

            List<EmpleadoEmpresa> empleadosRestantes = new List<EmpleadoEmpresa>();

            foreach (var emp in listaEmpleadosEmpresa)//Obtengo empleados activos al momento de realizar la declaracion Jurada que aun no estan declarados
            {
                if (db.DetalleDeclaracionJurada.Where(x => x.IdDeclaracionJurada == declaracionJuradaAux.IdDeclaracionJurada && x.IdEmpleadoEmpresa == emp.idEmpleadoEmpresa).FirstOrDefault() == null)
                {
                    empleadosRestantes.Add(emp);
                }
            }

            ViewBag.IdEmpleadoEmpresa = new SelectList(empleadosRestantes, "idEmpleadoEmpresa", "Empleado.Nombre", detalleDeclaracionJurada.IdEmpleadoEmpresa);
            ViewBag.idJornadaLaboral = new SelectList(db.Jornada, "IdJornada", "Descripcion", detalleDeclaracionJurada.idJornadaLaboral);
            ViewBag.IdLiquidacionProporcional = new SelectList(db.LiquidacionProporcional, "idLiquidacionProporcional", "Descripcion", detalleDeclaracionJurada.IdLiquidacionProporcional);

            TempData["MensajeError"] = "";

            if (ModelState.IsValid)
            {
                if (!detalleDeclaracionJurada.SueldoBase.HasValue)
                {
                    detalleDeclaracionJurada.SueldoBase = 0;
                }

                if (detalleDeclaracionJurada.IdLiquidacionProporcional != 1)//Si el IdLiquidacionProporcional es distinto a 1, significa que tiene una liquidacion proporcional y el sueldo puede ser menor que el minimo
                {
                    LiquidacionProporcionalEmpleado liquidacionProporcional = new LiquidacionProporcionalEmpleado();

                    liquidacionProporcional.IdLiquidacionProporcional = (int)detalleDeclaracionJurada.IdLiquidacionProporcional;
                    liquidacionProporcional.IdDetalleDeclaracionJurada = detalleDeclaracionJurada.IdDetalleDeclaracionJurada;

                    db.LiquidacionProporcionalEmpleado.Add(liquidacionProporcional);

                    db.DetalleDeclaracionJurada.Add(detalleDeclaracionJurada);
                    db.SaveChanges();

                    return RedirectToAction("CreateMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = detalleDeclaracionJurada.IdDeclaracionJurada });
                }

                //Datos del empleado en la empresa
                var empEmp = empleadosRestantes.Where(x => x.idEmpleadoEmpresa == detalleDeclaracionJurada.IdEmpleadoEmpresa).FirstOrDefault();

                //Fecha de la declaracion Jurada*
                DateTime fechaSueldo = new DateTime(declaracionJuradaAux.anio, declaracionJuradaAux.mes, DateTime.Today.Day);

                //Sueldo Basico para la categoria del empleado a declarar en la fecha de la declaracion jurada
                var sueldosBasicos = db.SueldoBasico.Where(x => x.IdCategoria == empEmp.IdCategoria &&
                                                                x.Desde <= fechaSueldo &&
                                                                x.Hasta >= fechaSueldo).ToList();

                
                if (sueldosBasicos != null && sueldosBasicos.Count > 0)
                {
                    //Si hay un sueldo basico para la Fecha de la declaracion Jurada

                    //Compruebo si el sueldo es mayor o menor del basico --True = Mayor al basico --False = Menor al basico
                    if (!ddjj.comprobarSueldoBasico(detalleDeclaracionJurada.IdEmpleadoEmpresa, declaracionJuradaAux.IdDeclaracionJurada, detalleDeclaracionJurada.Sueldo))
                    {
                        TempData["MensajeError"] = "El Sueldo ingresado es menor que el minimo.";
                        ModelState.AddModelError("Sueldo", "El Sueldo ingresado es menor que el minimo.");
                        return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                    }


                    #region Afiliado
                    var afiliado = db.Afiliado.Where(x => x.IdEmpleadoEmpresa == empEmp.idEmpleadoEmpresa).FirstOrDefault();
                    if (afiliado != null)//compruebo que exista un registro de afiliado para este empleado
                    {
                        if (afiliado.FechaAlta.Year < declaracionJuradaAux.anio)
                        {
                            //si el año de alta del afiliado es menor al de la declaracion jurada
                            //compruebo que no tenga fecha de baja o si tiene que sea posterior a la fecha de la boleta
                            if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > declaracionJuradaAux.anio || (afiliado.FechaBaja.Value.Year == declaracionJuradaAux.anio && afiliado.FechaBaja.Value.Month >= declaracionJuradaAux.mes))
                            {
                                if(empEmp.IdJornada == 1 || empEmp.IdJornada == 2)//Si trabaja  a media Jornada o a Jornada por Horas compruebo el sueldo base
                                {
                                    //checkeo si el sueldo base ingresado es mayor al sueldo minimo para la categoria del empleado
                                    if (!ddjj.comprobarSueldoBase(detalleDeclaracionJurada.IdEmpleadoEmpresa, declaracionJuradaAux.IdDeclaracionJurada, detalleDeclaracionJurada.SueldoBase.Value))
                                    {
                                        TempData["MensajeError"] = "El Sueldo Base ingresado es menor que el minimo.";
                                        ModelState.AddModelError("SueldoBase", "El Sueldo Base ingresado es menor que el minimo.");
                                        return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                                    }

                                    //checkeo si el sueldo base ingresado es mayor al sueldo ingresado para el empleado
                                    if (detalleDeclaracionJurada.SueldoBase < detalleDeclaracionJurada.Sueldo)
                                    {
                                        TempData["MensajeError"] = "El Sueldo Base no puede ser menor que el sueldo ingresado.";
                                        ModelState.AddModelError("SueldoBase", "El Sueldo Base no puede ser menor que el sueldo ingresado.");
                                        return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                                    }
                                }
                            }
                        }
                        else if (afiliado.FechaAlta.Year == declaracionJuradaAux.anio && afiliado.FechaAlta.Month <= declaracionJuradaAux.mes)
                        {
                            //si el año de alta del afiliado es menorigual al de la declaracion jurada, compruebo que el mes sea menor al mes declarado
                            //y compruebo que no tenga fecha de baja o si tiene que sea posterior a la fecha de la boleta
                            if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > declaracionJuradaAux.anio || (afiliado.FechaBaja.Value.Year == declaracionJuradaAux.anio && afiliado.FechaBaja.Value.Month >= declaracionJuradaAux.mes))
                            {
                                if(empEmp.IdJornada == 1 || empEmp.IdJornada == 2)//Si trabaja  a media Jornada o a Jornada por Horas compruebo el sueldo base
                                {
                                    //checkeo si el sueldo base ingresado es mayor al sueldo minimo para la categoria del empleado
                                    if (!ddjj.comprobarSueldoBase(detalleDeclaracionJurada.IdEmpleadoEmpresa, declaracionJuradaAux.IdDeclaracionJurada, detalleDeclaracionJurada.SueldoBase.Value))
                                    {
                                        TempData["MensajeError"] = "El Sueldo Base ingresado es menor que el minimo.";
                                        ModelState.AddModelError("SueldoBase", "El Sueldo Base ingresado es menor que el minimo.");
                                        return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                                    }

                                    //checkeo si el sueldo base ingresado es mayor al sueldo ingresado para el empleado
                                    if (detalleDeclaracionJurada.SueldoBase < detalleDeclaracionJurada.Sueldo)
                                    {
                                        TempData["MensajeError"] = "El Sueldo Base no puede ser menor que el sueldo ingresado.";
                                        ModelState.AddModelError("SueldoBase", "El Sueldo 5% no puede ser menor que el sueldo ingresado.");
                                        return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    //Si no hay un sueldo basico para la Fecha de la declaracion Jurada

                    TempData["MensajeError"] = "No Hay Sueldos Basicos Cargados Para Esta Fecha.";
                    return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                }

                ViewBag.idDeclaracion = detalleDeclaracionJurada.IdDeclaracionJurada;
                db.DetalleDeclaracionJurada.Add(detalleDeclaracionJurada);
                db.SaveChanges();

                return RedirectToAction("CreateMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
               
            }

            return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
        }

        public ActionResult CreateMessage(int idDeclaracionJurada)
        {
            ViewBag.idDeclaracion = idDeclaracionJurada;
            return View();
        }

        public ActionResult CantCreateMessage(int idDeclaracionJurada)
        {
            ViewBag.idDeclaracion = idDeclaracionJurada;
            return View();
        }

        // GET: DetallesDeclaracionJurada/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleDeclaracionJurada detalleDeclaracionJurada = db.DetalleDeclaracionJurada.Find(id);
            if (detalleDeclaracionJurada == null)
            {
                return HttpNotFound();
            }
            if (db.BoletaAportes.Where(x => x.IdDeclaracionJurada == detalleDeclaracionJurada.IdDeclaracionJurada && x.DeBaja == false).FirstOrDefault() != null)
            {
                return RedirectToAction("CantEditMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = detalleDeclaracionJurada.IdDeclaracionJurada });
            }

            var claim = ((ClaimsIdentity)User.Identity).FindFirst("IdEmpresa");
            int IdEmpresa = Convert.ToInt32(claim.Value);

            ViewBag.idDeclaracion = detalleDeclaracionJurada.IdDeclaracionJurada;
            ViewBag.idCategoria = new SelectList(db.Categoria, "IdCategoria", "Descripcion", detalleDeclaracionJurada.idCategoria);
            ViewBag.IdDeclaracionJurada = new SelectList(db.DeclaracionJurada, "IdDeclaracionJurada", "IdDeclaracionJurada", detalleDeclaracionJurada.IdDeclaracionJurada);
            var empleadoEmpresa = db.EmpleadoEmpresa.AsNoTracking().Where(x => x.idEmpleadoEmpresa == detalleDeclaracionJurada.IdEmpleadoEmpresa).ToList();
            foreach (var emp in empleadoEmpresa)
            {
                emp.Empleado.Nombre = emp.Empleado.Apellido + ", " + emp.Empleado.Nombre;
            }
            ViewBag.IdEmpleadoEmpresa = new SelectList(empleadoEmpresa, "idEmpleadoEmpresa", "Empleado.Nombre", detalleDeclaracionJurada.IdEmpleadoEmpresa);
            ViewBag.idJornadaLaboral = new SelectList(db.Jornada, "IdJornada", "Descripcion", detalleDeclaracionJurada.idJornadaLaboral);
            var LiquidacionProporcional = db.LiquidacionProporcionalEmpleado.Where(x => x.IdDetalleDeclaracionJurada == detalleDeclaracionJurada.IdDetalleDeclaracionJurada).FirstOrDefault();
            ViewBag.IdLiquidacionProporcional = new SelectList(db.LiquidacionProporcional, "IdLiquidacionProporcional", "Descripcion", (LiquidacionProporcional != null) ? LiquidacionProporcional.IdLiquidacionProporcional : 1);
            return View(detalleDeclaracionJurada);
        }

        // POST: DetallesDeclaracionJurada/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdDetalleDeclaracionJurada,IdDeclaracionJurada,IdEmpleadoEmpresa,idCategoria,idJornadaLaboral,IdLiquidacionProporcional,Sueldo,SueldoBase,Presentismo")] DetalleDeclaracionJurada detalleDeclaracionJurada)
        {
            var boleta = db.BoletaAportes.Where(x => x.IdDeclaracionJurada == detalleDeclaracionJurada.IdDeclaracionJurada && x.BoletaPagada == true && x.DeBaja == false).FirstOrDefault();
            if(boleta != null)
            {
                return RedirectToAction("CantEditMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = detalleDeclaracionJurada.IdDeclaracionJurada });
            }
            var claim = ((ClaimsIdentity)User.Identity).FindFirst("IdEmpresa");
            int IdEmpresa = Convert.ToInt32(claim.Value);
            if (ModelState.IsValid)
            {
                DeclaracionJurada declaracionJurada = db.DeclaracionJurada.Find(detalleDeclaracionJurada.IdDeclaracionJurada);
                var liquidacionProporcional = db.LiquidacionProporcionalEmpleado.Where(x => x.IdDetalleDeclaracionJurada == detalleDeclaracionJurada.IdDetalleDeclaracionJurada).FirstOrDefault();
                
                if (detalleDeclaracionJurada.IdLiquidacionProporcional != null && detalleDeclaracionJurada.IdLiquidacionProporcional != 1)
                {
                    if (liquidacionProporcional != null)
                    {
                        if (liquidacionProporcional.IdLiquidacionProporcional != detalleDeclaracionJurada.IdLiquidacionProporcional)
                        {
                            liquidacionProporcional.IdLiquidacionProporcional = (int)detalleDeclaracionJurada.IdLiquidacionProporcional;
                        }
                    }
                    else
                    {
                        db.LiquidacionProporcionalEmpleado.Add(new LiquidacionProporcionalEmpleado()
                        {
                            IdLiquidacionProporcional = (int)detalleDeclaracionJurada.IdLiquidacionProporcional,
                            IdDetalleDeclaracionJurada = detalleDeclaracionJurada.IdDetalleDeclaracionJurada
                        });
                    }
                }
                else
                {
                    if (liquidacionProporcional != null)
                    {
                        db.LiquidacionProporcionalEmpleado.Remove(liquidacionProporcional);
                    }
                }

                ViewBag.idDeclaracion = detalleDeclaracionJurada.IdDeclaracionJurada;
                ViewBag.idCategoria = new SelectList(db.Categoria, "IdCategoria", "Descripcion", detalleDeclaracionJurada.idCategoria);
                ViewBag.IdDeclaracionJurada = new SelectList(db.DeclaracionJurada, "IdDeclaracionJurada", "IdDeclaracionJurada", detalleDeclaracionJurada.IdDeclaracionJurada);

                var ListaEmpEmp = db.EmpleadoEmpresa.AsNoTracking().Where(x => x.idEmpleadoEmpresa == detalleDeclaracionJurada.IdEmpleadoEmpresa).ToList();
                foreach (var empleadoAux in ListaEmpEmp)
                {
                    empleadoAux.Empleado.Nombre = empleadoAux.Empleado.Apellido + ", " + empleadoAux.Empleado.Nombre;
                }
                ViewBag.IdEmpleadoEmpresa = new SelectList(ListaEmpEmp, "idEmpleadoEmpresa", "Empleado.Nombre", detalleDeclaracionJurada.IdEmpleadoEmpresa);
                ViewBag.idJornadaLaboral = new SelectList(db.Jornada, "IdJornada", "Descripcion", detalleDeclaracionJurada.idJornadaLaboral);
                var Liquidacion = db.LiquidacionProporcionalEmpleado.Where(x => x.IdDetalleDeclaracionJurada == detalleDeclaracionJurada.IdDetalleDeclaracionJurada).FirstOrDefault();
                ViewBag.IdLiquidacionProporcional = new SelectList(db.LiquidacionProporcional, "idLiquidacionProporcional", "Descripcion", (Liquidacion != null) ? Liquidacion.IdLiquidacionProporcional : 1);

                if (detalleDeclaracionJurada.IdLiquidacionProporcional == 1 && !ddjj.comprobarSueldoBasico(detalleDeclaracionJurada.IdEmpleadoEmpresa, declaracionJurada.IdDeclaracionJurada, detalleDeclaracionJurada.Sueldo))
                {
                    ModelState.AddModelError("Sueldo", "El Sueldo ingresado es menor que el minimo.");
                    return View(detalleDeclaracionJurada);
                }
                var empEmp = db.EmpleadoEmpresa.AsNoTracking().Where(x => x.idEmpleadoEmpresa == detalleDeclaracionJurada.IdEmpleadoEmpresa).FirstOrDefault();


                #region Afiliado
                var afiliado = db.Afiliado.Where(x => x.IdEmpleadoEmpresa == empEmp.idEmpleadoEmpresa).FirstOrDefault();
                if (afiliado != null)//compruebo que exista un registro de afiliado para este empleado
                {
                    if (afiliado.FechaAlta.Year < declaracionJurada.anio)
                    {
                        //si el año de alta del afiliado es menor al de la declaracion jurada
                        //compruebo que no tenga fecha de baja o si tiene que sea posterior a la fecha de la boleta
                        if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > declaracionJurada.anio || (afiliado.FechaBaja.Value.Year == declaracionJurada.anio && afiliado.FechaBaja.Value.Month >= declaracionJurada.mes))
                        {
                            if (empEmp.IdCategoria == 1 || empEmp.IdCategoria == 2)//Si trabaja  a media Jornada o a Jornada por Horas compruebo el sueldo base
                            {
                                //checkeo si el sueldo base ingresado es mayor al sueldo minimo para la categoria del empleado
                                if (!ddjj.comprobarSueldoBase(detalleDeclaracionJurada.IdEmpleadoEmpresa, declaracionJurada.IdDeclaracionJurada, detalleDeclaracionJurada.SueldoBase.Value))
                                {
                                    TempData["MensajeError"] = "El Sueldo Base ingresado es menor que el minimo.";
                                    ModelState.AddModelError("SueldoBase", "El Sueldo Base ingresado es menor que el minimo.");
                                    return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                                }

                                //checkeo si el sueldo base ingresado es mayor al sueldo ingresado para el empleado
                                if (detalleDeclaracionJurada.SueldoBase < detalleDeclaracionJurada.Sueldo)
                                {
                                    TempData["MensajeError"] = "El Sueldo Base no puede ser menor que el sueldo ingresado.";
                                    ModelState.AddModelError("SueldoBase", "El Sueldo Base no puede ser menor que el sueldo ingresado.");
                                    return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                                }
                            }
                        }
                    }
                    else if (afiliado.FechaAlta.Year == declaracionJurada.anio && afiliado.FechaAlta.Month <= declaracionJurada.mes)
                    {
                        //si el año de alta del afiliado es menorigual al de la declaracion jurada, compruebo que el mes sea menor al mes declarado
                        //y compruebo que no tenga fecha de baja o si tiene que sea posterior a la fecha de la boleta
                        if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > declaracionJurada.anio || (afiliado.FechaBaja.Value.Year == declaracionJurada.anio && afiliado.FechaBaja.Value.Month >= declaracionJurada.mes))
                        {
                            if (empEmp.IdCategoria == 1 || empEmp.IdCategoria == 2)//Si trabaja  a media Jornada o a Jornada por Horas compruebo el sueldo base
                            {
                                //checkeo si el sueldo base ingresado es mayor al sueldo minimo para la categoria del empleado
                                if (!ddjj.comprobarSueldoBase(detalleDeclaracionJurada.IdEmpleadoEmpresa, declaracionJurada.IdDeclaracionJurada, detalleDeclaracionJurada.SueldoBase.Value))
                                {
                                    TempData["MensajeError"] = "El Sueldo Base ingresado es menor que el minimo.";
                                    ModelState.AddModelError("SueldoBase", "El Sueldo Base ingresado es menor que el minimo.");
                                    return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                                }

                                //checkeo si el sueldo base ingresado es mayor al sueldo ingresado para el empleado
                                if (detalleDeclaracionJurada.SueldoBase < detalleDeclaracionJurada.Sueldo)
                                {
                                    TempData["MensajeError"] = "El Sueldo Base no puede ser menor que el sueldo ingresado.";
                                    ModelState.AddModelError("SueldoBase", "El Sueldo 5% no puede ser menor que el sueldo ingresado.");
                                    return RedirectToAction("Index", "DetallesDeclaracionJurada", new { id = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
                                }
                            }
                        }
                    }
                }
                #endregion

                //var afiliado = db.Afiliado.Where(x => x.IdEmpleadoEmpresa == empEmp.idEmpleadoEmpresa).FirstOrDefault();
                //if (afiliado != null)
                //{
                //    if (afiliado.FechaAlta.Year < declaracionJurada.anio)
                //    {
                //        if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > declaracionJurada.anio || (afiliado.FechaBaja.Value.Year == declaracionJurada.anio && afiliado.FechaBaja.Value.Month >= declaracionJurada.mes))
                //        {
                //            if (detalleDeclaracionJurada.SueldoBase <= 0)
                //            {
                //                TempData["MensajeError"] = "El Sueldo 5% ingresado es menor que el minimo.";
                //                ModelState.AddModelError("SueldoBase", "El Sueldo ingresado tiene que ser mayor que 0.");

                //                return View(detalleDeclaracionJurada);
                //            }
                //        }
                //    }
                //    else if (afiliado.FechaAlta.Year == declaracionJurada.anio && afiliado.FechaAlta.Month <= declaracionJurada.mes)
                //    {
                //        if (afiliado.FechaBaja == null || afiliado.FechaBaja.Value.Year > declaracionJurada.anio || (afiliado.FechaBaja.Value.Year == declaracionJurada.anio && afiliado.FechaBaja.Value.Month >= declaracionJurada.mes))
                //        {
                //            if (detalleDeclaracionJurada.SueldoBase <= 0)
                //            {
                //                TempData["MensajeError"] = "El Sueldo 5% ingresado es menor que el minimo.";
                //                ModelState.AddModelError("SueldoBase", "El Sueldo ingresado tiene que ser mayor que 0.");

                //                return View(detalleDeclaracionJurada);
                //            }
                //        }
                //    }
                //}
                db.Entry(detalleDeclaracionJurada).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("EditMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = detalleDeclaracionJurada.IdDeclaracionJurada, idEmpleadoEmpresa = detalleDeclaracionJurada.IdEmpleadoEmpresa });
            }

            ViewBag.idDeclaracion = detalleDeclaracionJurada.IdDeclaracionJurada;
            ViewBag.idCategoria = new SelectList(db.Categoria, "IdCategoria", "Descripcion", detalleDeclaracionJurada.idCategoria);
            ViewBag.IdDeclaracionJurada = new SelectList(db.DeclaracionJurada, "IdDeclaracionJurada", "IdDeclaracionJurada", detalleDeclaracionJurada.IdDeclaracionJurada);
            var declaracionJuradaAux = db.DeclaracionJurada.Find(detalleDeclaracionJurada.IdDeclaracionJurada);
            var EmpleadosEmpresa = db.EmpleadoEmpresa.AsNoTracking().Where(x => x.idEmpresa == IdEmpresa && (x.FechaBaja.Value == null)).ToList();

            List<EmpleadoEmpresa> listaEmpleadosEmpresa = new List<EmpleadoEmpresa>();
            foreach (var EmpleadoEmpresa in EmpleadosEmpresa)
            {
                EmpleadoEmpresa.Empleado.Nombre = EmpleadoEmpresa.Empleado.Apellido + ", " + EmpleadoEmpresa.Empleado.Nombre;
                if (EmpleadoEmpresa.FechaAlta.Year < declaracionJuradaAux.anio)
                {
                    listaEmpleadosEmpresa.Add(EmpleadoEmpresa);
                }
                else if (EmpleadoEmpresa.FechaAlta.Year == declaracionJuradaAux.anio && EmpleadoEmpresa.FechaAlta.Month <= declaracionJuradaAux.mes)
                {
                    listaEmpleadosEmpresa.Add(EmpleadoEmpresa);
                }
            }
            ViewBag.IdEmpleadoEmpresa = new SelectList(listaEmpleadosEmpresa, "idEmpleadoEmpresa", "Empleado.Nombre", detalleDeclaracionJurada.IdEmpleadoEmpresa);
            ViewBag.idJornadaLaboral = new SelectList(db.Jornada, "IdJornada", "Descripcion", detalleDeclaracionJurada.idJornadaLaboral);
            var LiquidacionProporcional = db.LiquidacionProporcionalEmpleado.Where(x => x.IdDetalleDeclaracionJurada == detalleDeclaracionJurada.IdDetalleDeclaracionJurada).FirstOrDefault();
            ViewBag.IdLiquidacionProporcional = new SelectList(db.LiquidacionProporcional, "idLiquidacionProporcional", "Descripcion", (LiquidacionProporcional != null) ? LiquidacionProporcional.IdLiquidacionProporcional : 1);
            return View(detalleDeclaracionJurada);
        }

        public ActionResult EditMessage(int idDeclaracionJurada)
        {
            ViewBag.idDeclaracion = idDeclaracionJurada;
            return View();
        }

        public ActionResult CantEditMessage(int idDeclaracionJurada)
        {
            ViewBag.idDeclaracion = idDeclaracionJurada;
            return View();
        }

        // GET: DetallesDeclaracionJurada/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleDeclaracionJurada detalleDeclaracionJurada = db.DetalleDeclaracionJurada.Find(id);
            if (detalleDeclaracionJurada == null)
            {
                return HttpNotFound();
            }
            if (db.BoletaAportes.Where(x => x.IdDeclaracionJurada == detalleDeclaracionJurada.IdDeclaracionJurada && x.DeBaja == false).FirstOrDefault() != null)
            {
                return RedirectToAction("CantDeleteMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = detalleDeclaracionJurada.IdDeclaracionJurada });
            }
            ViewBag.idDeclaracion = detalleDeclaracionJurada.IdDeclaracionJurada;
            return View(detalleDeclaracionJurada);
        }

        // POST: DetallesDeclaracionJurada/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetalleDeclaracionJurada detalleDeclaracionJurada = db.DetalleDeclaracionJurada.Find(id);
            int idDDJJ = detalleDeclaracionJurada.IdDeclaracionJurada;
            if (db.BoletaAportes.Where(x => x.IdDeclaracionJurada == detalleDeclaracionJurada.IdDeclaracionJurada && x.BoletaPagada && x.DeBaja == false).FirstOrDefault() != null)
            {

                return RedirectToAction("CantDeleteMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = idDDJJ });
            }
            var liquidacion = db.LiquidacionProporcionalEmpleado.Where(x => x.IdDetalleDeclaracionJurada == id).FirstOrDefault();
            if(liquidacion != null)
            {
                db.LiquidacionProporcionalEmpleado.Remove(liquidacion);
                db.SaveChanges();
            }
            db.DetalleDeclaracionJurada.Remove(detalleDeclaracionJurada);
            db.SaveChanges();
            return RedirectToAction("DeleteMessage", "DetallesDeclaracionJurada", new { idDeclaracionJurada = idDDJJ });
        }

        public ActionResult DeleteMessage(int idDeclaracionJurada)
        {
            ViewBag.idDeclaracion = idDeclaracionJurada;
            return View();
        }

        public ActionResult CantDeleteMessage(int idDeclaracionJurada)
        {
            ViewBag.idDeclaracion = idDeclaracionJurada;
            return View();
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
