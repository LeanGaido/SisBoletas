using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.ViewModels;
using DAL.Models;
using DAL;
using Zen.Barcode;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Reporting.WebForms;

namespace SecSisBoletas.Areas.Empresas.Controllers
{
    public class ImpresionBoletaController : Controller
    {
        private SecModel db = new SecModel();
        private static int idDeclaracion;
        private static string banco;

        public RedirectResult ImpresionBoleta(int idBoleta, string _banco)
        {
            banco = _banco;
            Session["IdBoleta"] = idBoleta;
            Session["Banco"] = banco;
            if(banco == "5525")
            {
                return Redirect("~/Areas/Empresas/Reports/ImpresionBoleta.aspx");
            }
            else if(banco == "2354")
            {
                return Redirect("~/Areas/Empresas/Reports/ImpresionBoletaBcoHip.aspx");
            }
            return Redirect("https://xindicoweb.com.ar/SecSanFrancisco");
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Boleta> GetBoletas(int id)
        {
            List<Boleta> boletas = new List<Boleta>();
            var boletaAux = db.BoletaAportes.Where(x => x.IdBoleta == id).FirstOrDefault();

            if (boletaAux != null)
            {
                Boleta boletaNueva = new Boleta();
                boletaNueva.Banco = banco;

                idDeclaracion = boletaAux.IdDeclaracionJurada;
                DeclaracionJurada ddjj = db.DeclaracionJurada.Where(x => x.IdDeclaracionJurada == idDeclaracion).FirstOrDefault();
                Empresa empresa = db.Empresa.Where(x => x.IdEmpresa == ddjj.idEmpresa).FirstOrDefault();
                Localidad localidad = db.Localidad.Where(x => x.IdLocalidad == empresa.IdLocalidad).FirstOrDefault();

                boletaNueva.RazonSocial = empresa.RazonSocial;
                boletaNueva.Cuit = empresa.Cuit;
                boletaNueva.Domicilio = empresa.Calle + " " + empresa.Altura.ToString();
                boletaNueva.CodPostal = localidad.CodPostal.ToString();
                boletaNueva.Localidad = localidad.Nombre;

                string telefono = (empresa.TelefonoFijo != null) ? empresa.TelefonoFijo.ToString() : "";
                if (telefono == ""){ telefono = (empresa.TelefonoCelular != null) ? empresa.TelefonoCelular.ToString() : ""; }
                boletaNueva.Telefono = (empresa.TelefonoFijo != null) ? empresa.TelefonoFijo.ToString() : "";

                boletaNueva.Mes = ddjj.mes.ToString();
                boletaNueva.Anio = ddjj.anio.ToString();

                DateTime fechaVencimiento = boletaAux.FechaVencimiento;
                boletaNueva.FechaVencimiento = fechaVencimiento.ToShortDateString();
                boletaNueva.RecargoPorMora = (boletaAux.RecargoMora == null) ? "0" : boletaAux.RecargoMora.ToString();

                boletaNueva.CantEmpleados = boletaAux.CantEmpleados.ToString();
                boletaNueva.TotalSueldos = boletaAux.TotalSueldos.ToString();
                
                boletaNueva.Aportes = (Math.Truncate(((boletaAux.TotalSueldos / 100) * 2) * 100) / 100).ToString();

                boletaNueva.CantAfiliados = boletaAux.TotalSueldosAfiliados.ToString();
                boletaNueva.TotalSueldosAfiliados = boletaAux.TotalSueldosAfiliados.ToString();
                
                boletaNueva.AportesAfiliados = (Math.Truncate(((boletaAux.TotalSueldosAfiliados / 100) * 5) * 100) / 100).ToString();
                boletaNueva.Total = (Convert.ToDouble(boletaNueva.UnPorcFamiliaresACargo) + Convert.ToDouble(boletaNueva.RecargoPorMora)).ToString();
                boletaNueva.TotalDepositado = (Convert.ToDouble(boletaNueva.Aportes) + Convert.ToDouble(boletaNueva.AportesAfiliados) + Convert.ToDouble(boletaNueva.Total)).ToString();

                string totalDepositadoEntero = boletaNueva.TotalDepositado.Split(new Char[] { ',', '.' })[0];
                string totalDepositadoDecimal = (boletaNueva.TotalDepositado.Split(new Char[] { ',', '.' }).Count() > 1) ? boletaNueva.TotalDepositado.Split(new Char[] { ',', '.' })[1] : "00";

                boletaNueva.CodBarra = boletaNueva.Banco + idDeclaracion.ToString().PadLeft(8, '0') + fechaVencimiento.DayOfYear.ToString().PadLeft(3, '0') + (fechaVencimiento.Year - 2000).ToString().PadLeft(2, '0') + boletaNueva.Anio.PadLeft(4, '0') + boletaNueva.Mes.PadLeft(2, '0') + totalDepositadoEntero.PadLeft(6, '0') + totalDepositadoDecimal.PadLeft(2, '0');

                int nv = 0;
                int resto = 0;
                int resto1 = 0;
                string digi2;

                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(0, 1)) * 7);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(1, 1)) * 6);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(2, 1)) * 5);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(3, 1)) * 4);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(4, 1)) * 3);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(5, 1)) * 2);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(6, 1)) * 7);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(7, 1)) * 6);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(8, 1)) * 5);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(9, 1)) * 4);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(10, 1)) * 3);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(11, 1)) * 2);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(12, 1)) * 7);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(13, 1)) * 6);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(14, 1)) * 5);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(15, 1)) * 4);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(16, 1)) * 3);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(17, 1)) * 2);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(18, 1)) * 7);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(19, 1)) * 6);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(20, 1)) * 5);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(21, 1)) * 4);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(22, 1)) * 3);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(23, 1)) * 2);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(24, 1)) * 7);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(25, 1)) * 6);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(26, 1)) * 5);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(27, 1)) * 4);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(28, 1)) * 3);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(29, 1)) * 2);
                nv = nv + (int.Parse(boletaNueva.CodBarra.Substring(30, 1)) * 7);

                resto = nv / 11;
                resto1 = 11 - (nv - (resto * 11));

                if (resto1 == 10)
                {
                    digi2 = "0";
                }
                else
                {
                    if (resto1 == 11)
                    {
                        digi2 = "1";
                    }
                    else
                    {
                        digi2 = resto1.ToString().Trim();
                    }
                }

                boletaNueva.CodBarra += digi2;
                boletaNueva.BarCode = creabarcode(boletaNueva.CodBarra);


                switch (boletaNueva.Mes)
                {
                    case "1":
                        boletaNueva.Mes = "Enero";
                        break;
                    case "2":
                        boletaNueva.Mes = "Febrero";
                        break;
                    case "3":
                        boletaNueva.Mes = "Marzo";
                        break;
                    case "4":
                        boletaNueva.Mes = "Abril";
                        break;
                    case "5":
                        boletaNueva.Mes = "Mayo";
                        break;
                    case "6":
                        boletaNueva.Mes = "Junio y SAC";
                        break;
                    case "7":
                        boletaNueva.Mes = "Julio";
                        break;
                    case "8":
                        boletaNueva.Mes = "Agosto";
                        break;
                    case "9":
                        boletaNueva.Mes = "Septiembre";
                        break;
                    case "10":
                        boletaNueva.Mes = "Octubre";
                        break;
                    case "11":
                        boletaNueva.Mes = "Noviembre";
                        break;
                    case "12":
                        boletaNueva.Mes = "Diciembre y SAC";
                        break;
                    default:
                        break;
                }
                boletas.Add(boletaNueva);

                //return boletas.ToList();
            }

            return boletas.ToList();
        }


        public RedirectResult ImpresionBoletaEspecial(int idBoletaEspecial, string _banco)
        {
            banco = _banco;
            Session["IdBoletaEspecial"] = idBoletaEspecial;
            Session["Banco"] = banco;
            if (banco == "5525")
            {
                return Redirect("~/Areas/Empresas/Reports/ImpresionBoletaEspecial.aspx");
            }
            else if (banco == "2354")
            {
                return Redirect("~/Areas/Empresas/Reports/ImpresionBoletaEspecial.aspx");
            }
            return Redirect("https://xindicoweb.com.ar/SecSanFrancisco");
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BoletaEspecial> GetBoletaEspecial(int id)
        {
            List<BoletaEspecial> boletas = new List<BoletaEspecial>();
            var boletaAux = db.BoletaAportesEspeciales.Where(x => x.IdBoleta == id).FirstOrDefault();
            if (boletaAux != null)
            {
                BoletaEspecial boletaNueva = new BoletaEspecial();
                boletaNueva.Banco = banco;

                boletaNueva.RazonSocial = boletaAux.RazonSocial;
                boletaNueva.Cuit = boletaAux.Cuit;
                boletaNueva.Domicilio = boletaAux.Calle + " " + boletaAux.Altura.ToString();
                boletaNueva.CodPostal = boletaAux.CodPostal.ToString();
                boletaNueva.Localidad = boletaAux.Localidad;
                string telefono = (boletaAux.TelefonoFijo != null) ? boletaAux.TelefonoFijo.ToString() : "";
                if (telefono == "") { telefono = (boletaAux.TelefonoCelular != null) ? boletaAux.TelefonoCelular.ToString() : ""; }
                boletaNueva.Telefono = telefono;

                boletaNueva.Periodo = boletaAux.Periodo;

                boletaNueva.CantEmpleados = boletaAux.CantEmpleados.ToString();
                boletaNueva.TotalSueldos = boletaAux.TotalSueldos.ToString();
                boletaNueva.Aportes = boletaAux.Aportes.ToString();

                boletaNueva.CantAfiliados = boletaAux.CantAfiliados.ToString();
                boletaNueva.TotalSueldosAfiliados = boletaAux.TotalSueldosAfiliados.ToString();
                boletaNueva.AportesAfiliados = boletaAux.AportesAfiliados.ToString();

                boletaNueva.FechaVencimiento = boletaAux.FechaVencimiento.ToShortDateString();
                boletaNueva.RecargoPorMora = (boletaAux.RecargoMora == null) ? "0" : boletaAux.RecargoMora.ToString();
                boletaNueva.Total = boletaAux.Total.ToString();
                boletaNueva.TotalDepositado = boletaAux.TotalDepositado.ToString();

                boletas.Add(boletaNueva);
            }

            return boletas.ToList();
        }


        public byte[] creabarcode(String barcode)
        {
            BarcodeMetrics tamccbb = new BarcodeMetrics(2, 90);
            System.Drawing.Image imagen;

            imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code128).Draw(barcode, tamccbb);

            ImageFormat format = ImageFormat.Bmp;

            MemoryStream mm = new MemoryStream();
            imagen.Save(mm, format);
            imagen.Dispose();

            byte[] bytearray = mm.ToArray();
            mm.Close();
            mm.Dispose();

            return bytearray;
        }
    }
}