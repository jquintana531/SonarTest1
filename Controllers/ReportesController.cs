using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MTC.WebApp.BackOffice.DataManager;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using MTCenter.GRPC.GDBBO.DBProtos;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using static MTC.WebApp.BackOffice.Models.RecursosModel;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Reportes")]
    public class ReportesController : Controller
    {
        private readonly AppSettings __appSettings;
        private RequestHelper request;
        private readonly ReportesManager _manager;

        public ReportesController(IOptions<AppSettings> appSetings, DBData.DBDataClient client)
        {
            this.__appSettings = appSetings.Value;
            this.request = new RequestHelper(this.__appSettings);
            this._manager = new ReportesManager(client); 
        }

        #region Reporte Ventas
        
        [Route("ReporteVentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_01")]
        public async Task<string> ReporteVentas([FromBody]ReporteRequestReporteVentasModel item)
        {
            ReporteResponseReporteVentas responseD = new ReporteResponseReporteVentas();
            try
            {

                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteVentas", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;
                return response;
                //if (request.codigoRespuesta == 0)
                //{
                //    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteVentasExcel>(response);

                //}
                //else
                //{
                //    responseD.codigoRespuesta = request.codigoRespuesta;
                //    responseD.mensajeRespuesta = request.mensajeRespuesta;
                //}

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
                return "";
            }
            //return responseD;
        }

        [Route("ReporteVentasExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_01")]
        public async Task<ReporteResponseReporteVentasExcel> ReporteVentasExcel([FromBody] ReporteResponseReporteVentas item)
        {
            //ReporteResponseReporteVentas responseD = new ReporteResponseReporteVentas();
            ReporteResponseReporteVentasExcel responseD1 = new ReporteResponseReporteVentasExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {

                //item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                //item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                //string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                //Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                //Task<string> taskResponse = request.doPost("Reportes/ReporteVentas", data, HttpContext.Session.GetString("token"));
                //await Task.WhenAll(taskLog, taskResponse);
                //string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    //responseD = JsonConvert.DeserializeObject<ReporteResponseReporteVentas>(response);

                   
                    responseD1.CodigoRespuesta = item.CodigoRespuesta;
                       

                    Logger.DoLog(MethodBase.GetCurrentMethod(), item);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        item.MensajeRespuesta = "Reporte de ventas generado exitosamente";
                        responseD1.respuestaLista = item.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte de ventas");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Fecha Hora";
                        hojaActual.Cells["B1"].Value = "MTCID";
                        hojaActual.Cells["C1"].Value = "Establecimiento";
                        hojaActual.Cells["D1"].Value = "Cajero";
                        hojaActual.Cells["E1"].Value = "Clasificación";
                        hojaActual.Cells["F1"].Value = "Producto";
                        hojaActual.Cells["G1"].Value = "Proveedor";
                        hojaActual.Cells["H1"].Value = "Referencia";
                        hojaActual.Cells["I1"].Value = "Ref2/Afiliación";
                        hojaActual.Cells["J1"].Value = "Entradas";
                        hojaActual.Cells["K1"].Value = "Salidas";
                        hojaActual.Cells["L1"].Value = "Comisión TAE";
                        hojaActual.Cells["M1"].Value = "Comisión Establecimiento";
                        hojaActual.Cells["N1"].Value = "No. Autorización";
                        hojaActual.Cells["O1"].Value = "Tipo Terminal";
                        hojaActual.Cells["P1"].Value = "Estatus";
                        hojaActual.Cells["A1:P1"].Style.Font.Bold = true;
                        int linea = 2;
                        foreach (ReporteReporteVentasModel carteraNew in item.respuesta)
                        {
                            long bandera = 0;
                            hojaActual.Cells["A" + linea].Value = carteraNew.fechaHora.ToString();
                            hojaActual.Cells["B" + linea].Value = int.Parse(carteraNew.MTCid.ToString());
                            hojaActual.Cells["C" + linea].Value = carteraNew.tienda.ToString();
                            hojaActual.Cells["D" + linea].Value = carteraNew.cajero.ToString();
                            if(carteraNew.clasificacion == null)
                                hojaActual.Cells["E" + linea].Value = carteraNew.clasificacion;
                            else
                                hojaActual.Cells["E" + linea].Value = carteraNew.clasificacion.ToString();
                            hojaActual.Cells["F" + linea].Value = carteraNew.producto.ToString();
                            hojaActual.Cells["G" + linea].Value = carteraNew.proveedor.ToString();
                            if(long.TryParse(carteraNew.referencia1.ToString(), out bandera))
                                hojaActual.Cells["H" + linea].Value = long.Parse(carteraNew.referencia1.ToString());
                            else
                                hojaActual.Cells["H" + linea].Value = carteraNew.referencia1.ToString();
                            hojaActual.Cells["I" + linea].Value = carteraNew.referencia2.ToString();
                            hojaActual.Cells["J" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.entrada.ToString()),2);
                            hojaActual.Cells["K" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.salida.ToString()), 2);
                            hojaActual.Cells["L" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.comisionRecarga.ToString()), 2);
                            hojaActual.Cells["M" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.comisionEstablecimiento.ToString()), 2);
                            hojaActual.Cells["J" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["K" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["L" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["M" + linea].Style.Numberformat.Format = "$#,##0.00";
                            if(carteraNew.autorizacion != null && carteraNew.autorizacion != "")
                                hojaActual.Cells["N" + linea].Value = decimal.Parse(carteraNew.autorizacion.ToString(), System.Globalization.NumberStyles.Float);
                            else
                                hojaActual.Cells["N" + linea].Value = carteraNew.autorizacion;
                            hojaActual.Cells["O" + linea].Value = carteraNew.tipoTerminal.ToString();
                            hojaActual.Cells["P" + linea].Value = carteraNew.estatusCancelada.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 16];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte de Ventas.xlsx";

                    }
                    else
                    {
                        item.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = item.MensajeRespuesta;
                    
                }
                else
                {
                    item.CodigoRespuesta = -6;
                    item.MensajeRespuesta = "Ingrese todos los campos necesarios";
       
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                item.CodigoRespuesta = -9;
                item.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }



        [Route("ReporteVentasExcelMenosOpciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_01")]
        public async Task<ReporteResponseReporteVentasExcel> ReporteVentasExcelMenosOpciones([FromBody] ReporteResponseReporteVentas item)
        {
            ReporteResponseReporteVentasExcel responseD1 = new ReporteResponseReporteVentasExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {

                //item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                //item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                //string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                //Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                //Task<string> taskResponse = request.doPost("Reportes/ReporteVentas", data, HttpContext.Session.GetString("token"));
                //await Task.WhenAll(taskLog, taskResponse);
                //string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    //responseD = JsonConvert.DeserializeObject<ReporteResponseReporteVentas>(response);


                    //responseD1.CodigoRespuesta = item.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), item);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        item.MensajeRespuesta = "Reporte de ventas generado exitosamente";
                        responseD1.respuestaLista = item.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte de ventas");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Fecha Hora";
                        hojaActual.Cells["B1"].Value = "MTCID";
                        hojaActual.Cells["C1"].Value = "Establecimiento";
                        hojaActual.Cells["D1"].Value = "Cajero";
                        hojaActual.Cells["E1"].Value = "Clasificación";
                        hojaActual.Cells["F1"].Value = "Producto";
                        hojaActual.Cells["G1"].Value = "Referencia";
                        hojaActual.Cells["H1"].Value = "Entradas";
                        hojaActual.Cells["I1"].Value = "Salidas";
                        hojaActual.Cells["J1"].Value = "Comisión TAE";
                        hojaActual.Cells["K1"].Value = "Comisión Establecimiento";
                        hojaActual.Cells["L1"].Value = "No. Autorización";
                        hojaActual.Cells["M1"].Value = "Estatus";
                        hojaActual.Cells["A1:M1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteVentasModel carteraNew in item.respuesta)
                        {

                            hojaActual.Cells["A" + linea].Value = carteraNew.fechaHora.ToString();
                            hojaActual.Cells["B" + linea].Value = carteraNew.MTCid.ToString();
                            hojaActual.Cells["C" + linea].Value = carteraNew.tienda.ToString();
                            hojaActual.Cells["D" + linea].Value = carteraNew.cajero.ToString();
                            if (carteraNew.clasificacion == null)
                                hojaActual.Cells["E" + linea].Value = carteraNew.clasificacion;
                            else
                                hojaActual.Cells["E" + linea].Value = carteraNew.clasificacion.ToString();
                            hojaActual.Cells["F" + linea].Value = carteraNew.producto.ToString();
                            hojaActual.Cells["G" + linea].Value = carteraNew.referencia1.ToString();
                            hojaActual.Cells["H" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.entrada.ToString()), 2);
                            hojaActual.Cells["I" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.salida.ToString()), 2);
                            hojaActual.Cells["J" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.comisionRecarga.ToString()), 2);
                            hojaActual.Cells["K" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.comisionEstablecimiento.ToString()),2);
                            hojaActual.Cells["H" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["I" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["J" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["K" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["L" + linea].Value = carteraNew.autorizacion.ToString();
                            hojaActual.Cells["M" + linea].Value = carteraNew.estatusCancelada.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 16];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte de Ventas.xlsx";

                    }
                    else
                    {
                        item.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = item.MensajeRespuesta;

                }
                else
                {
                    item.CodigoRespuesta = -6;
                    item.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                item.CodigoRespuesta = -9;
                item.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }


        [Route("getEstablecimientos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06")]
        public ReporteResponseEstablecimientos getEstablecimientos([FromBody]ReporteRequestGetEstablecimientosModel item)
        {
            ReporteResponseEstablecimientos responseD = new ReporteResponseEstablecimientos();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/getEstablecimientos", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseEstablecimientos>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }

        [Route("listadoClasificacionProducto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06")]
        public async Task<ReporteResponseClasificacionProducto> listadoClasificacionProducto()
        {
            ReporteResponseClasificacionProducto response = new ReporteResponseClasificacionProducto();
            try
            {

                string reqResponse = await request.doPost("Reportes/listadoClasificacionProducto", "", HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteResponseClasificacionProducto>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("listadoProveedores")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06")]
        public async Task<ReporteResponseProveedores> listadoProveedores([FromBody] ReportesRequestListadoProveedores item)
        {
            ReporteResponseProveedores response = new ReporteResponseProveedores();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/listadoProveedores", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteResponseProveedores>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("listadoProductos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06")]
        public async Task<ReporteResponseProductos> listadoProductos([FromBody] ReportesRequestCatalogoProducto item)
        {
            ReporteResponseProductos response = new ReporteResponseProductos();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/listadoProductos", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteResponseProductos>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        #endregion

        #region Reporte Compras

        [Route("ReporteCompras")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_02")]
        public async Task<ReporteResponseReporteCompras> ReporteCompras([FromBody]ReporteRequestReporteComprasModel item)
        {
            ReporteResponseReporteCompras responseD = new ReporteResponseReporteCompras();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteCompras", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteCompras>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
                
        [Route("ReporteComprasExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_02")]
        public async Task<ReporteResponseReporteComprasExcel> ReporteComprasExcel([FromBody]ReporteRequestReporteComprasModel item)
        {
            ReporteResponseReporteCompras responseD = new ReporteResponseReporteCompras();
            ReporteResponseReporteComprasExcel responseD1 = new ReporteResponseReporteComprasExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteCompras", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteCompras>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de compras generado exitosamente";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte de Compras");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Cadena";
                        hojaActual.Cells["B1"].Value = "Cajero";
                        hojaActual.Cells["C1"].Value = "Fecha";
                        hojaActual.Cells["D1"].Value = "Nombre";
                        hojaActual.Cells["E1"].Value = "Cantidad";
                        hojaActual.Cells["F1"].Value = "Costo";
                        hojaActual.Cells["G1"].Value = "Importe Comision";
                        hojaActual.Cells["H1"].Value = "Comision Recarga";
                        hojaActual.Cells["I1"].Value = "Total";
                        hojaActual.Cells["J1"].Value = "Estatus";
                        hojaActual.Cells["K1"].Value = "Observaciones";
                        hojaActual.Cells["A1:K1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteComprasModel compra in responseD.respuesta)
                        {
              
                            hojaActual.Cells["A" + linea].Value = compra.idCadena.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.cajero.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.fechaHora.ToString();
                            hojaActual.Cells["D" + linea].Value = compra.nombre.ToString();
                            hojaActual.Cells["E" + linea].Value = compra.cantidad.ToString();
                            hojaActual.Cells["F" + linea].Value = Decimal.Parse(compra.costo.ToString());
                            hojaActual.Cells["G" + linea].Value = Decimal.Parse(compra.importeComision.ToString());
                            hojaActual.Cells["H" + linea].Value = Decimal.Parse(compra.comisionRecarga.ToString());
                            hojaActual.Cells["I" + linea].Value = Decimal.Parse(compra.total.ToString());
                            hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["H" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["I" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["J" + linea].Value = compra.estatus.ToString();
                            hojaActual.Cells["K" + linea].Value = compra.observaciones.ToString();
                      
                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 11];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData); 
                            memoryStream.Dispose();


                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte de Compras.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Depositos

        [Route("ReporteDepositos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_03")]
        public async Task<ReporteResponseReporteDepositos> ReporteDepositos([FromBody]ReporteRequestReporteDepositosModel item)
        {
            ReporteResponseReporteDepositos responseD = new ReporteResponseReporteDepositos();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteDepositos", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteDepositos>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
        

        [Route("ReporteDepositosExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_03")]
        public async Task<ReporteResponseReporteDepositosExcel> ReporteDepositosExcel([FromBody]ReporteRequestReporteDepositosModel item)
        {
            ReporteResponseReporteDepositos responseD = new ReporteResponseReporteDepositos();
            ReporteResponseReporteDepositosExcel responseD1 = new ReporteResponseReporteDepositosExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteDepositos", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteDepositos>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de depositos generado exitosamente";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte de Depositos");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Id Deposito";
                        hojaActual.Cells["B1"].Value = "Nombre Usuario";
                        hojaActual.Cells["C1"].Value = "ID";
                        hojaActual.Cells["D1"].Value = "Nombre Comercial";
                        hojaActual.Cells["E1"].Value = "Banco - Cuenta";
                        hojaActual.Cells["F1"].Value = "Monto";
                        hojaActual.Cells["G1"].Value = "Comisión";
                        hojaActual.Cells["H1"].Value = "Fecha Depósito";
                        hojaActual.Cells["I1"].Value = "Hora Depósito";
                        hojaActual.Cells["J1"].Value = "Hora Liberación";
                        hojaActual.Cells["K1"].Value = "No. Auto.";
                        hojaActual.Cells["L1"].Value = "Forma de Pago";
                        
                        hojaActual.Cells["A1:L1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteDepositosModel compra in responseD.respuesta)
                        {
                            long bandera = 0;

                            hojaActual.Cells["A" + linea].Value = int.Parse(compra.idDeposito.ToString());
                            hojaActual.Cells["B" + linea].Value = compra.nombreUsuario.ToString();
                            hojaActual.Cells["C" + linea].Value = int.Parse(compra.id.ToString());
                            hojaActual.Cells["D" + linea].Value = compra.nombreComercial.ToString();
                            hojaActual.Cells["E" + linea].Value = compra.banco.ToString();
                            hojaActual.Cells["F" + linea].Value = Decimal.Round(Decimal.Parse(compra.monto.ToString()), 2);
                            hojaActual.Cells["G" + linea].Value = Decimal.Round(Decimal.Parse(compra.comision.ToString()), 2);
                            hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["H" + linea].Value = compra.fechaDeposito.ToString("dd/MM/yyyyy");
                            hojaActual.Cells["I" + linea].Value = compra.horaDeposito.ToString("hh:mm:ss tt");
                            hojaActual.Cells["J" + linea].Value = compra.horaLiberacion.ToString();
                            if(long.TryParse(compra.referencia2.ToString(), out bandera))
                                hojaActual.Cells["K" + linea].Value = long.Parse(compra.referencia2.ToString());
                            else
                                hojaActual.Cells["K" + linea].Value = compra.referencia2.ToString();
                            hojaActual.Cells["L" + linea].Value = compra.formaPago.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 12];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData); 
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte de Depositos.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ReporteDepositos/CuentasBanco")]
        [MTCAuthorizationHandler("PERM_06")]
        public ContabilidadListadoCuentasResponse ContabilidadCuentasBanco()
        {
            ContabilidadListadoCuentasResponse responseD = new ContabilidadListadoCuentasResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    Task<string> taskResponse = request.doPost("Contabilidad/listadoCuentas", null, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<ContabilidadListadoCuentasResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";
                    return responseD;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return responseD;
        }

        #endregion

        #region Reporte Depositos Banca

        [Route("ReporteDepositosBanca")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_04")]
        public async Task<ReporteResponseReporteDepositosBanca> ReporteDepositosBanca([FromBody]ReporteRequestReporteDepositosBancaModel item)
        {
            ReporteResponseReporteDepositosBanca responseD = new ReporteResponseReporteDepositosBanca();
            try
            {
                if(item.idCatalogoCuenta.Split(',').Length > 1)
                    item.idCatalogoCuenta = item.idCatalogoCuenta.Remove(item.idCatalogoCuenta.Length-1);


                item.idUsuario= Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteDepositosBanca", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteDepositosBanca>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
               
        [Route("ReporteDepositosBancaExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_04")]
        public async Task<ReporteResponseReporteDepositosBancaExcel> ReporteDepositosBancoExcel([FromBody] ReporteResponseReporteDepositosBanca item)
        {
            ReporteResponseReporteDepositosBancaExcel responseD1 = new ReporteResponseReporteDepositosBancaExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {

                Logger.DoLog(MethodBase.GetCurrentMethod(), item);


                item.MensajeRespuesta = "Reporte de depositos generado exitosamente";
                responseD1.respuestaLista = item.respuesta;

                workbook.Workbook.Worksheets.Add("Reporte de Depositos (Banca)");
                var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                hojaActual = workbook.Workbook.Worksheets[0];

                hojaActual.Cells["A1"].Value = "ID Deposito";
                hojaActual.Cells["B1"].Value = "Tipo de Deposito";
                hojaActual.Cells["C1"].Value = "No. Auto.";
                hojaActual.Cells["D1"].Value = "Forma de Pago";
                hojaActual.Cells["E1"].Value = "Monto";
                hojaActual.Cells["F1"].Value = "Descripcion";
                hojaActual.Cells["G1"].Value = "Referencia Contable";
                hojaActual.Cells["H1"].Value = "Sucursal";
                hojaActual.Cells["I1"].Value = "Codigo";
                hojaActual.Cells["J1"].Value = "Referencia";
                hojaActual.Cells["K1"].Value = "Fecha";
                hojaActual.Cells["L1"].Value = "Alta";
                hojaActual.Cells["M1"].Value = "Liberado";
                hojaActual.Cells["N1"].Value = "Banco - Cuenta";

                hojaActual.Cells["A1:N1"].Style.Font.Bold = true;
                int linea = 2;

                foreach (ReporteReporteDepositosBancaModel compra in item.respuesta)
                {
                    int bandera = 0;
                            
                    hojaActual.Cells["A" + linea].Value = compra.idDeposito.ToString();
                    hojaActual.Cells["B" + linea].Value = compra.tipoDeposito.ToString();
                    if(int.TryParse(compra.numeroAutorizacion.ToString(), out bandera))
                        hojaActual.Cells["C" + linea].Value = int.Parse(compra.numeroAutorizacion.ToString());
                    else
                        hojaActual.Cells["C" + linea].Value = compra.numeroAutorizacion.ToString();
                    hojaActual.Cells["D" + linea].Value = compra.formaPago.ToString();
                    hojaActual.Cells["E" + linea].Value = Decimal.Round(Decimal.Parse(compra.monto.ToString()), 2);
                    hojaActual.Cells["E" + linea].Style.Numberformat.Format = "$#,##0.00";
                    hojaActual.Cells["F" + linea].Value = compra.descripciones.ToString();
                    hojaActual.Cells["G" + linea].Value = compra.referenciaContabilidad.ToString();
                    hojaActual.Cells["H" + linea].Value = int.Parse(compra.sucursal.ToString());
                    hojaActual.Cells["I" + linea].Value = compra.codigoDeposito.ToString();
                    hojaActual.Cells["J" + linea].Value = compra.referencia.ToString();
                    hojaActual.Cells["K" + linea].Value = compra.fechaHora.ToString();
                    hojaActual.Cells["L" + linea].Value = compra.fechaHoraAlta.ToString();
                    hojaActual.Cells["M" + linea].Value = compra.fechaLiberacion.ToString();
                    hojaActual.Cells["N" + linea].Value = compra.banco.ToString();

                    linea++;
                }

                var range = hojaActual.Cells[1, 1, linea - 1, 14];
                var table = hojaActual.Tables.Add(range, "table");
                //table.ShowTotal = true;
                table.TableStyle = TableStyles.Medium13;
                hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                // identificador para su guardado
                string base64Excel = string.Empty;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    memoryStream.Position = 0;
                    byte[] excelData = memoryStream.ToArray();

                    base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                    memoryStream.Dispose();

                }
                responseD1.respuesta = base64Excel;
                responseD1.reporteNombre = "Reporte de Depositos (Banca).xlsx";

               

                responseD1.MensajeRespuesta = item.MensajeRespuesta;

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD1.CodigoRespuesta = -9;
                responseD1.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CatalogosDepositosBanca")]
        [MTCAuthorizationHandler("PERM_06")]
        public ReporteCatalogosDepositosBancaResponse ContabilidadCuentasBancoDepBancas()
        {
            ReporteCatalogosDepositosBancaResponse responseD = new ReporteCatalogosDepositosBancaResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    RequestHelper request1 = new RequestHelper(this.__appSettings);
                    RequestHelper request2 = new RequestHelper(this.__appSettings);
                    RequestHelper request3 = new RequestHelper(this.__appSettings);
                    Task<string> taskResponse = request1.doPost("Contabilidad/listadoCuentas", null, HttpContext.Session.GetString("token"));
                    Task<string> taskResponse2 = request2.doPost("Contabilidad/formasPago", null, HttpContext.Session.GetString("token"));
                    Task<string> taskResponse3 = request3.doPost("Liberaciones/DepositosNoRelacionados/tipoDepositos", null, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse, taskResponse2, taskResponse3);
                    string response = taskResponse.Result;
                    string response2 = taskResponse2.Result;
                    string response3 = taskResponse3.Result;
                    if (request.codigoRespuesta == 0 && request2.codigoRespuesta == 0 && request3.codigoRespuesta == 0)
                    {
                        //Catalogo de cuentas
                        ContabilidadListadoCuentasResponse responseD1 = new ContabilidadListadoCuentasResponse();
                        responseD1 = JsonConvert.DeserializeObject<ContabilidadListadoCuentasResponse>(response);

                        //Catalogo de Formas de pago
                        ContabilidadFormasPagoResponse responseD2 = new ContabilidadFormasPagoResponse();
                        responseD2 = JsonConvert.DeserializeObject<ContabilidadFormasPagoResponse>(response2);

                        //Catalogo Opciones
                        LiberacionesResponseListadoTipoDepositos responseD3 = new LiberacionesResponseListadoTipoDepositos();
                        responseD3 = JsonConvert.DeserializeObject<LiberacionesResponseListadoTipoDepositos>(response3);

                        //Asignar catalogos

                        if (responseD3.CodigoRespuesta == 0 && responseD2.CodigoRespuesta == 0 && responseD1.CodigoRespuesta == 0)
                        {
                            responseD.CodigoRespuesta = 0;
                            responseD.MensajeRespuesta = "";

                            responseD.listadoCuentas = responseD1.listadoCuentas;
                            responseD.listadoFormasPago = responseD2.listadoFormasPago;
                            responseD.listadoTipoDepositos = responseD3.listadoTipoDepositos;

                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        }
                        else
                        {
                            responseD.CodigoRespuesta = -5;
                            responseD.MensajeRespuesta = "Se produjo un error inesperado [02]";
                        }

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -6;
                        responseD.MensajeRespuesta = "Se produjo un error inesperado [01]";
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = -7;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }


        #endregion

        #region Reporte Balance General

        [Route("ReporteBalanceGeneral")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_05")]
        public async Task<ReporteResponseReporteBalanceGeneral> ReporteBalanceGeneral([FromBody] ReporteRequestReporteBalanceGeneralModel item)
        {
            ReporteResponseReporteBalanceGeneral responseD = new ReporteResponseReporteBalanceGeneral();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteBalanceGeneral", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteBalanceGeneral>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
               
        [Route("ReporteBalanceGeneralExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_05")]
        public async Task<ReporteResponseReporteBalanceGeneralExcel> ReporteBalanceGeneralExcel([FromBody] ReporteRequestReporteBalanceGeneralModel item)
        {
            ReporteResponseReporteBalanceGeneral responseD = new ReporteResponseReporteBalanceGeneral();
            ReporteResponseReporteBalanceGeneralExcel responseD1 = new ReporteResponseReporteBalanceGeneralExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteBalanceGeneral", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteBalanceGeneral>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "El Reporte de Balance General se genero exitosamente";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Balance General");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Fecha";
                        hojaActual.Cells["B1"].Value = "Saldo Inicial";
                        hojaActual.Cells["C1"].Value = "Depositos";
                        hojaActual.Cells["D1"].Value = "Comision Depositos";
                        hojaActual.Cells["E1"].Value = "Entradas";
                        hojaActual.Cells["F1"].Value = "Salidas";
                        hojaActual.Cells["G1"].Value = "Compras";
                        hojaActual.Cells["H1"].Value = "Comision Establecimiento";
                        hojaActual.Cells["I1"].Value = "Comision Recarga";
                        hojaActual.Cells["J1"].Value = "Crédito";
                        hojaActual.Cells["K1"].Value = "SaldoFinal";
                        hojaActual.Cells["L1"].Value = "Calculado";

                        hojaActual.Cells["A1:L1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteBalanceGeneralModel compra in responseD.respuesta)
                        {
                            hojaActual.Cells["A" + linea].Value = compra.fechaInicial.ToString();
                            hojaActual.Cells["B" + linea].Value = Decimal.Round(Decimal.Parse(compra.saldoInicial.ToString()), 2);
                            hojaActual.Cells["C" + linea].Value = Decimal.Round(Decimal.Parse(compra.depositos.ToString()), 2);
                            hojaActual.Cells["D" + linea].Value = Decimal.Round(Decimal.Parse(compra.comisionDepositos.ToString()), 2);
                            hojaActual.Cells["E" + linea].Value = Decimal.Round(Decimal.Parse(compra.entradas.ToString()), 2);
                            hojaActual.Cells["F" + linea].Value = Decimal.Round(Decimal.Parse(compra.salidas.ToString()), 2);
                            hojaActual.Cells["G" + linea].Value = Decimal.Round(Decimal.Parse(compra.compras.ToString()), 2);
                            hojaActual.Cells["H" + linea].Value = Decimal.Round(Decimal.Parse(compra.comisionEstablecimiento.ToString()), 2);
                            hojaActual.Cells["I" + linea].Value = Decimal.Round(Decimal.Parse(compra.comisionRecarga.ToString()), 2);
                            hojaActual.Cells["J" + linea].Value = Decimal.Round(Decimal.Parse(compra.credito.ToString()), 2);
                            hojaActual.Cells["K" + linea].Value = Decimal.Round(Decimal.Parse(compra.saldoFinal.ToString()), 2);
                            hojaActual.Cells["L" + linea].Value = Decimal.Round(Decimal.Parse(compra.saldoCalculado.ToString()), 2);
                            hojaActual.Cells["B" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["C" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["D" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["E" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["H" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["I" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["J" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["K" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["L" + linea].Style.Numberformat.Format = "$#,##0.00";
                            //hojaActual.Cells["L" + linea].Value = Decimal.Parse(compra.saldoCalculado.ToString("C", new CultureInfo("es-MX")));


                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 12];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Balance General.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Monedero

        [Route("ReporteMonedero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_06")]
        public async Task<ReporteResponseReporteMonedero> ReporteMonedero([FromBody]ReporteRequestReporteMonederoModel item)
        {
            ReporteResponseReporteMonedero responseD = new ReporteResponseReporteMonedero();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteMonedero", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteMonedero>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }



        [Route("ReporteMonederoExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_06")]
        public async Task<ReporteResponseReporteMonederoExcel> ReporteMonederoExcel([FromBody]ReporteRequestReporteMonederoModel item)
        {
            ReporteResponseReporteMonedero responseD = new ReporteResponseReporteMonedero();
            ReporteResponseReporteMonederoExcel responseD1 = new ReporteResponseReporteMonederoExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteMonedero", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteMonedero>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "El Reporte de Monedero se genero exitosamente";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Monedero");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Autorizacion";
                        hojaActual.Cells["B1"].Value = "Monedero";
                        hojaActual.Cells["C1"].Value = "Nombre";
                        hojaActual.Cells["D1"].Value = "Fecha";
                        hojaActual.Cells["E1"].Value = "Descripcion";
                        hojaActual.Cells["F1"].Value = "Monto";
                        hojaActual.Cells["G1"].Value = "Saldo Monedero";

                        hojaActual.Cells["A1:L1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteMonederoModel compra in responseD.respuesta)
                        {
                 
                            hojaActual.Cells["A" + linea].Value = compra.Autorizacion.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.idMonedero.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.nombre.ToString();
                            hojaActual.Cells["D" + linea].Value = compra.fechaHora.ToString();
                            if (compra.descripcion == null)
                                hojaActual.Cells["E" + linea].Value = compra.descripcion;
                            else
                                hojaActual.Cells["E" + linea].Value = compra.descripcion.ToString();
                            hojaActual.Cells["F" + linea].Value = Decimal.Parse(compra.monto.ToString());
                            hojaActual.Cells["G" + linea].Value = Decimal.Parse(compra.saldoMonedero.ToString());
                            hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 7];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Monedero.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Credito

        [Route("ReporteCredito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_07")]
        public async Task<ReporteResponseReporteCredito> ReporteCredito([FromBody]ReporteRequestReporteCreditoModel item)
        {
            ReporteResponseReporteCredito responseD = new ReporteResponseReporteCredito();
            try
            {
                item.tbDesde = DateTime.Parse(item.tbDesde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.tbHasta = DateTime.Parse(item.tbHasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteCredito", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {

                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteCredito>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }



        [Route("ReporteCreditoExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_07")]
        public async Task<ReporteResponseReporteCreditoExcel> ReporteCreditoExcel([FromBody]ReporteRequestReporteCreditoModel item)
        {
            ReporteResponseReporteCredito responseD = new ReporteResponseReporteCredito();
            ReporteResponseReporteCreditoExcel responseD1 = new ReporteResponseReporteCreditoExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {

                item.tbDesde = DateTime.Parse(item.tbDesde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.tbHasta = DateTime.Parse(item.tbHasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteCredito", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteCredito>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte Credito";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Credito");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];
                        int linea = 2;
                        int lineashoja = 0;
                        if (item.tipoReporte == 1)
                        {
                            lineashoja = 4;
                            hojaActual.Cells["A1"].Value = "Fecha";
                            hojaActual.Cells["B1"].Value = "Ejecutivo";
                            hojaActual.Cells["C1"].Value = "Cadena";
                            hojaActual.Cells["D1"].Value = "Cadena/Monto";

                            hojaActual.Cells["A1:D1"].Style.Font.Bold = true;

                            String iteratorJson = JsonConvert.SerializeObject(responseD.respuesta);
                            List<ReporteReporteCreditoModel> iterador = JsonConvert.DeserializeObject<List<ReporteReporteCreditoModel>>(iteratorJson);

                            foreach (ReporteReporteCreditoModel datos in iterador)
                            {
                                hojaActual.Cells["A" + linea].Value = datos.fecha.ToString();
                                hojaActual.Cells["B" + linea].Value = datos.ejecutivo.ToString();
                                hojaActual.Cells["C" + linea].Value = datos.cadena.ToString();
                                hojaActual.Cells["D" + linea].Value = Decimal.Parse(datos.cadenaMonto.ToString());
                                hojaActual.Cells["D" + linea].Style.Numberformat.Format = "$#,##0.00";
                                linea++;
                            }
                        }
                        else if(item.tipoReporte == 2)
                        {
                            lineashoja = 2;
                            hojaActual.Cells["A1"].Value = "MTCid";
                            hojaActual.Cells["B1"].Value = "Crédito";

                            hojaActual.Cells["A1:B1"].Style.Font.Bold = true;

                            String iteratorJson = JsonConvert.SerializeObject(responseD.respuesta);
                            List<ReporteReporteCredito2Model> iterador = JsonConvert.DeserializeObject<List<ReporteReporteCredito2Model>>(iteratorJson);


                            foreach (ReporteReporteCredito2Model datos in iterador)
                            {
                                hojaActual.Cells["A" + linea].Value = datos.idCadena.ToString();
                                hojaActual.Cells["B" + linea].Value = datos.credito.ToString();
                                linea++;
                            }
                        }
                        else if (item.tipoReporte == 3 || item.tipoReporte == 4)
                        {
                            lineashoja = 3;
                            hojaActual.Cells["A1"].Value = "MTCid";
                            hojaActual.Cells["B1"].Value = "Crédito";
                            hojaActual.Cells["C1"].Value = "Modalidad";

                            hojaActual.Cells["A1:C1"].Style.Font.Bold = true;

                            String iteratorJson = JsonConvert.SerializeObject(responseD.respuesta);
                            List<ReporteReporteCredito3Model> iterador = JsonConvert.DeserializeObject<List<ReporteReporteCredito3Model>>(iteratorJson);


                            foreach (ReporteReporteCredito3Model datos in iterador)
                            {
                                hojaActual.Cells["A" + linea].Value = datos.idCadena.ToString();
                                hojaActual.Cells["B" + linea].Value = datos.credito.ToString();
                                hojaActual.Cells["C" + linea].Value = datos.modalidad.ToString();
                                linea++;
                            }
                        }


                            var range = hojaActual.Cells[1, 1, linea - 1, lineashoja];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Credito.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }


                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Credito (Cadena)

        [Route("ReporteCreditoCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_08")]
        public async Task<ReporteResponseReporteCreditoCadena> ReporteCredito([FromBody]ReporteRequestReporteCreditoCadenaModel item)
        {
            ReporteResponseReporteCreditoCadena responseD = new ReporteResponseReporteCreditoCadena();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteCreditoCadena", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteCreditoCadena>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
               
        [Route("ReporteCreditoCadenaExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_08")]
        public async Task<ReporteResponseReporteCreditoCadenaExcel> ReporteCreditoExcel([FromBody]ReporteRequestReporteCreditoCadenaModel item)
        {
            ReporteResponseReporteCreditoCadena responseD = new ReporteResponseReporteCreditoCadena();
            ReporteResponseReporteCreditoCadenaExcel responseD1 = new ReporteResponseReporteCreditoCadenaExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteCreditoCadena", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteCreditoCadena>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "El Reporte de Monedero se genero exitosamente";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Monedero");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Clasificacion";
                        hojaActual.Cells["B1"].Value = "Cadena";
                        hojaActual.Cells["C1"].Value = "Transacciones";
                        hojaActual.Cells["D1"].Value = "Entrada";
                        hojaActual.Cells["E1"].Value = "Salida";
                        hojaActual.Cells["F1"].Value = "Comision Recarga";
                        hojaActual.Cells["G1"].Value = "comision Establecimiento";

                        hojaActual.Cells["A1:L1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteCreditoCadenaModel compra in responseD.respuesta)
                        {
                       
                            hojaActual.Cells["A" + linea].Value = compra.Clasificacion.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.idCadena.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.TRX.ToString();
                            hojaActual.Cells["D" + linea].Value = Decimal.Parse(compra.entrada.ToString());
                            hojaActual.Cells["E" + linea].Value = Decimal.Parse(compra.salida.ToString());
                            hojaActual.Cells["F" + linea].Value = Decimal.Parse(compra.comisionRecarga);
                            hojaActual.Cells["G" + linea].Value = Decimal.Parse(compra.comisionEstablecimiento);
                            hojaActual.Cells["D" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["E" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 7];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Monedero.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Pago Tarjeta

        [Route("ReportePagoTarjeta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_09")]
        public async Task<ReporteResponseReportePagoTarjeta> ReportePagoTarjeta([FromBody]ReporteRequestReportePagoTarjetaModel item)
        {
            ReporteResponseReportePagoTarjeta responseD = new ReporteResponseReportePagoTarjeta();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task<string> taskResponse = request.doPost("Reportes/ReportePagoTarjeta", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReportePagoTarjeta>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
               
        [Route("ReportePagoTarjetaExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_09")]
        public async Task<ReporteResponseReportePagoTarjetaExcel> ReportePagoTarjetaExcel([FromBody] ReporteResponseReportePagoTarjeta item)
        {
            ReporteResponseReportePagoTarjetaExcel responseD1 = new ReporteResponseReportePagoTarjetaExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                responseD1.respuestaLista = item.respuesta;
                Logger.DoLog(MethodBase.GetCurrentMethod(), item);
                
                item.MensajeRespuesta = "Reporte Pago (TPV) generado exitosamente";

                workbook.Workbook.Worksheets.Add("Reporte Pago (TPV)");
                var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                hojaActual = workbook.Workbook.Worksheets[0];

                hojaActual.Cells["A1"].Value = "Fecha";
                hojaActual.Cells["B1"].Value = "MTCid";
                hojaActual.Cells["C1"].Value = "Establecimiento";
                hojaActual.Cells["D1"].Value = "Tarjeta";
                hojaActual.Cells["E1"].Value = "Afiliacion";
                hojaActual.Cells["F1"].Value = "Monto";
                hojaActual.Cells["G1"].Value = "Autorizacion";
                hojaActual.Cells["H1"].Value = "Tipo";
                hojaActual.Cells["I1"].Value = "Proveedor";
                hojaActual.Cells["J1"].Value = "Comisión MTC";
                hojaActual.Cells["K1"].Value = "Comisión Establecimiento";

                hojaActual.Cells["A1:K1"].Style.Font.Bold = true;
                int linea = 2;

                foreach (ReporteReportePagoTarjetaModel compra in item.respuesta)
                {
                    long intento = 0;


                    hojaActual.Cells["A" + linea].Value = compra.fechaHora;
                    hojaActual.Cells["B" + linea].Value = int.Parse(compra.MTCid.ToString());
                    hojaActual.Cells["C" + linea].Value = compra.tienda.ToString();
                    hojaActual.Cells["D" + linea].Value = compra.tarjeta.ToString();
                    if (long.TryParse(compra.afiliacion, out intento))
                        hojaActual.Cells["E" + linea].Value = long.Parse(compra.afiliacion);
                    else
                        hojaActual.Cells["E" + linea].Value = compra.afiliacion;

                    hojaActual.Cells["F" + linea].Value = Decimal.Round(Decimal.Parse(compra.monto.ToString()), 2);
                    hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                    if (long.TryParse(compra.autorizacion, out intento))
                        hojaActual.Cells["G" + linea].Value = long.Parse(compra.autorizacion);
                    else
                        hojaActual.Cells["G" + linea].Value = compra.autorizacion;


                    hojaActual.Cells["H" + linea].Value = compra.tipo;
                    hojaActual.Cells["I" + linea].Value = compra.proveedor;
                    hojaActual.Cells["J" + linea].Value = Decimal.Round(Decimal.Parse(compra.comisionMTC.ToString()), 2);
                    hojaActual.Cells["K" + linea].Value = Decimal.Round(Decimal.Parse(compra.comisionEstablecimiento.ToString()), 2);
                    hojaActual.Cells["J" + linea].Style.Numberformat.Format = "$#,##0.00";
                    hojaActual.Cells["K" + linea].Style.Numberformat.Format = "$#,##0.00";

                    linea++;
                }

                var range = hojaActual.Cells[1, 1, linea - 1, 11];
                var table = hojaActual.Tables.Add(range, "table");
                //table.ShowTotal = true;
                table.TableStyle = TableStyles.Medium13;
                hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                // identificador para su guardado
                string base64Excel = string.Empty;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    memoryStream.Position = 0;
                    byte[] excelData = memoryStream.ToArray();

                    base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                    memoryStream.Dispose();

                }
                responseD1.respuesta = base64Excel;
                responseD1.reporteNombre = "Reporte Pago (TPV).xlsx";
                
                responseD1.MensajeRespuesta = item.MensajeRespuesta;
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD1.CodigoRespuesta = -9;
                responseD1.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Ventas (Recomendados)

        [Route("ReporteVentasRecomendados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_12")]
        public async Task<ReporteResponseReporteVentasRecomendados> ReporteVentasRecomendados([FromBody]ReporteRequestReporteVentasRecomendadosModel item)
        {
            ReporteResponseReporteVentasRecomendados responseD = new ReporteResponseReporteVentasRecomendados();
            try
            {

                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".999";

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteVentasRecomendados", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteVentasRecomendados>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
             
        [Route("ReporteVentasRecomendadosExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_12")]
        public async Task<ReporteResponseReporteVentasRecomendadosExcel> ReporteVentasRecomendadosExcel([FromBody]ReporteRequestReporteVentasRecomendadosModel item)
        {
            ReporteResponseReporteVentasRecomendados responseD = new ReporteResponseReporteVentasRecomendados();
            ReporteResponseReporteVentasRecomendadosExcel responseD1 = new ReporteResponseReporteVentasRecomendadosExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {

                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteVentasRecomendados", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteVentasRecomendados>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte Leads generado exitosamente";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Leads");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Fecha Recomendación";
                        hojaActual.Cells["B1"].Value = "MTCid Beneficiario";
                        hojaActual.Cells["C1"].Value = "Beneficiario";
                        hojaActual.Cells["D1"].Value = "idKit";
                        hojaActual.Cells["E1"].Value = "Contacto";
                        hojaActual.Cells["F1"].Value = "Email";
                        hojaActual.Cells["G1"].Value = "Teléfono";
                        hojaActual.Cells["H1"].Value = "Ejecutivo";
                        hojaActual.Cells["I1"].Value = "Tarea";
                        hojaActual.Cells["J1"].Value = "Comentario";
                        hojaActual.Cells["K1"].Value = "Estatus";

                        hojaActual.Cells["A1:N1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteVentasRecomendadosModel compra in responseD.respuesta)
                        {

                            hojaActual.Cells["A" + linea].Value = compra.FechaRecomendacion.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.MTCidBeneficiario.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.NombreComercialBeneficiario.ToString();
                            hojaActual.Cells["D" + linea].Value = compra.MTCidRecomendado;
                            hojaActual.Cells["E" + linea].Value = compra.ContactoRecomendado;
                            hojaActual.Cells["F" + linea].Value = compra.EmailRecomendado;
                            hojaActual.Cells["G" + linea].Value = compra.Tel1Recomendado;
                            hojaActual.Cells["H" + linea].Value = compra.ejecutivo;
                            hojaActual.Cells["I" + linea].Value = compra.tarea;
                            hojaActual.Cells["J" + linea].Value = compra.comentario;
                            hojaActual.Cells["K" + linea].Value = compra.activo;

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 11];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Ventas Recomendados.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }
       
        #endregion

        #region Reporte Cliente Factura

        [Route("ReporteClientesFactura")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_13")]
        public async Task<ReporteResponseReporteClienteFactura> ReporteClienteFactura([FromBody]ReporteRequestReporteClienteFacturaModel item)
        {
            ReporteResponseReporteClienteFactura responseD = new ReporteResponseReporteClienteFactura();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteClientesFactura", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteClienteFactura>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
                
        [Route("ReporteClientesFacturaExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_13")]
        public async Task<ReporteResponseReporteClienteFacturaExcel> ReporteClienteFacturaExcel([FromBody]ReporteRequestReporteClienteFacturaModel item)
        {
            ReporteResponseReporteClienteFactura responseD = new ReporteResponseReporteClienteFactura();
            ReporteResponseReporteClienteFacturaExcel responseD1 = new ReporteResponseReporteClienteFacturaExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteClientesFactura", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteClienteFactura>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de clientes tipo factura generado exitosamente";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Leads");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "MTCID";
                        hojaActual.Cells["B1"].Value = "RFC";
                        hojaActual.Cells["C1"].Value = "Tipo Factura";
                        hojaActual.Cells["D1"].Value = "Teléfono";
                        hojaActual.Cells["E1"].Value = "Correo";

                        hojaActual.Cells["A1:E1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteClienteFacturaModel compra in responseD.respuesta)
                        {
                            hojaActual.Cells["A" + linea].Value = compra.idCadena.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.rfc.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.tipoFactura.ToString();
                            hojaActual.Cells["D" + linea].Value = compra.telefono.ToString();
                            hojaActual.Cells["E" + linea].Value = compra.correo.ToString();
                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 5];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte de clientes tipo factura.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        [Route("ReporteTipoFactura")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_13")]
        public async Task<ReporteResponseReporteTipoFactura> ReporteClienteFactura()
        {
            ReporteResponseReporteTipoFactura responseD = new ReporteResponseReporteTipoFactura();
            try
            {
                ReporteRequestReporteTipoFacturaModel item = new ReporteRequestReporteTipoFacturaModel();
                item.id = 0;
                item.menu = true;

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteTipoFactura", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteTipoFactura>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
        #endregion

        #region Reporte Cash Out

        [Route("ReporteCashOut")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_14")]
        public async Task<ReporteResponseReporteCashOut> ReporteCashOut([FromBody]ReporteRequestReporteCashOutModel item)
        {
            ReporteResponseReporteCashOut responseD = new ReporteResponseReporteCashOut();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteCashOut", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteCashOut>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
               
        [Route("ReporteCashOutExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_14")]
        public async Task<ReporteResponseReporteCashOutExcel> ReporteCashOutExcel([FromBody] ReporteRequestReporteCashOutModel item)
        {
            ReporteResponseReporteCashOut responseD = new ReporteResponseReporteCashOut();
            ReporteResponseReporteCashOutExcel responseD1 = new ReporteResponseReporteCashOutExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteCashOut", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteCashOut>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte Cash Out";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Cash Out");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Nombre";
                        hojaActual.Cells["B1"].Value = "MTCid";
                        hojaActual.Cells["C1"].Value = "Nombre Comercial";
                        hojaActual.Cells["D1"].Value = "Cuenta";
                        hojaActual.Cells["E1"].Value = "Monto";
                        hojaActual.Cells["F1"].Value = "Hora Liberación";

                        hojaActual.Cells["A1:F1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteCashOutModel compra in responseD.respuesta)
                        {

                            hojaActual.Cells["A" + linea].Value = compra.nombre.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.mtcid.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.nombreComercial.ToString();
                            hojaActual.Cells["D" + linea].Value = compra.cuenta;
                            hojaActual.Cells["E" + linea].Value = Decimal.Parse(compra.monto.ToString());
                            hojaActual.Cells["E" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["F" + linea].Value = compra.horaLiberacion;

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 14];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Cash Out.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion        

        #region Reporte (Visor Notificaciones)

        [Route("ReporteVisorNotificaciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_15")]
        public async Task<ReporteResponseReporteVisorNotificaciones> ReporteVisorNotificaciones([FromBody]ReporteRequestReporteVisorNotificacionesModel item)
        {
            ReporteResponseReporteVisorNotificaciones responseD = new ReporteResponseReporteVisorNotificaciones();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteVisorNotificaciones", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteVisorNotificaciones>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
                
        [Route("ReporteVisorNotificacionesExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_15")]
        public async Task<ReporteResponseReporteVisorNotificacionesExcel> ReporteVisorNotificacionesExcel([FromBody]ReporteRequestReporteVisorNotificacionesModel item)
        {
            ReporteResponseReporteVisorNotificaciones responseD = new ReporteResponseReporteVisorNotificaciones();
            ReporteResponseReporteVisorNotificacionesExcel responseD1 = new ReporteResponseReporteVisorNotificacionesExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteVisorNotificaciones", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteVisorNotificaciones>(response);

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;

                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte Cash Out";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Visor Notificaciones");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Fecha";
                        hojaActual.Cells["B1"].Value = "Mensaje";
                        hojaActual.Cells["C1"].Value = "Estatus";

                        hojaActual.Cells["A1:C1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteVisorNotificacionesModel itemComp in responseD.respuesta)
                        {

                            hojaActual.Cells["A" + linea].Value = itemComp.fecha.ToString();
                            hojaActual.Cells["B" + linea].Value = Regex.Replace(itemComp.mensaje, "<.*?>", String.Empty);
                            hojaActual.Cells["C" + linea].Value = itemComp.actividad;

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 3];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte (Visor Notificaciones).xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Resumen Transacciones

        [Route("ReporteResumenTransacciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_15")]
        public async Task<ReporteResponseReporteResumenTransacciones> ReporteResumenTransacciones([FromBody]ReporteRequestReporteResumenTransaccionesModel item)
        {
            ReporteResponseReporteResumenTransacciones responseD = new ReporteResponseReporteResumenTransacciones();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteResumenTransacciones", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteResumenTransacciones>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
               
        [Route("ReporteResumenTransaccionesExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_15")]
        public async Task<ReporteResponseReporteResumenTransaccionesExcel> ReporteResumenTransaccionesExcel([FromBody]ReporteRequestReporteResumenTransaccionesModel item)
        {
            ReporteResponseReporteResumenTransacciones responseD = new ReporteResponseReporteResumenTransacciones();
            ReporteResponseReporteResumenTransaccionesExcel responseD1 = new ReporteResponseReporteResumenTransaccionesExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteResumenTransacciones", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteResumenTransacciones>(response);

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;

                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte Resumen Transacciones";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Resumen Transacciones");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "MTCID";
                        hojaActual.Cells["B1"].Value = "Sucursal";
                        hojaActual.Cells["C1"].Value = "Referencia";
                        hojaActual.Cells["D1"].Value = "Autorización";
                        hojaActual.Cells["E1"].Value = "Monto";
                        hojaActual.Cells["F1"].Value = "Fecha";
                        hojaActual.Cells["G1"].Value = "Empresa";
                        hojaActual.Cells["H1"].Value = "Producto";

                        hojaActual.Cells["A1:H1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteResumenTransaccionesModel itemComp in responseD.respuesta)
                        {
                            
                            hojaActual.Cells["A" + linea].Value = itemComp.mtcid.ToString();
                            hojaActual.Cells["B" + linea].Value = itemComp.sucursal.ToString();
                            hojaActual.Cells["C" + linea].Value = itemComp.referencia1;
                            hojaActual.Cells["D" + linea].Value = itemComp.numeroAutorizacion.ToString();
                            hojaActual.Cells["E" + linea].Value = Decimal.Parse(itemComp.entrada.ToString());
                            hojaActual.Cells["E" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["F" + linea].Value = itemComp.fechaHoraResponse;
                            hojaActual.Cells["G" + linea].Value = itemComp.empresa.ToString();
                            hojaActual.Cells["H" + linea].Value = itemComp.producto.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 8];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Resumen Transacciones.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Tickets

        [Route("ReporteTickets")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public async Task<ReporteResponseReporteTickets> ReporteTickets([FromBody]ReporteRequestReporteTicketsModel item)
        {
            ReporteResponseReporteTickets responseD = new ReporteResponseReporteTickets();
            try
            {
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteTickets", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteTickets>(response);

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
               
        [Route("ReporteTicketsExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public async Task<ReporteResponseReporteTicketsExcel> ReporteTicketsExcel([FromBody]ReporteRequestReporteTicketsModel item)
        {
            ReporteResponseReporteTickets responseD = new ReporteResponseReporteTickets();
            ReporteResponseReporteTicketsExcel responseD1 = new ReporteResponseReporteTicketsExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/ReporteTickets", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteTickets>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte Tickets";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte Tickets");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Nombre";
                        hojaActual.Cells["B1"].Value = "MTCid";
                        hojaActual.Cells["C1"].Value = "Nombre Comercial";
                        hojaActual.Cells["D1"].Value = "Cuenta";
                        hojaActual.Cells["E1"].Value = "Monto";
                        hojaActual.Cells["F1"].Value = "Hora Liberación";

                        hojaActual.Cells["A1:F1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteTicketsModel compra in responseD.respuesta)
                        {
                            /*
                            hojaActual.Cells["A" + linea].Value = compra.nombre.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.mtcid.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.nombreComercial.ToString();
                            hojaActual.Cells["D" + linea].Value = compra.cuenta;
                            hojaActual.Cells["E" + linea].Value = compra.monto;
                            hojaActual.Cells["F" + linea].Value = compra.horaLiberacion;
                            */
                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 14];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();


                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Tickets.xlsx";

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -5;
                        responseD.MensajeRespuesta = "Error al generar el reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }
                     
        [Route("ObtenerTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseTicket ObtenerTicket([FromBody]ReporteRequestObtenerTicketModel item)
        {
            ReporteResponseTicket responseD = new ReporteResponseTicket();
            ReporteResponseObtenerTicket responseD1 = new ReporteResponseObtenerTicket();
            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
            ReporteResponsePrioridadCatalogo responseD3 = new ReporteResponsePrioridadCatalogo();
            ReporteResponseUsuarioDepartamento responseD4 = new ReporteResponseUsuarioDepartamento();
            ReporteResponseCatalogoDepartamento responseD5 = new ReporteResponseCatalogoDepartamento();
            try
            {
                RequestHelper request1 = new RequestHelper(this.__appSettings);
                RequestHelper request2 = new RequestHelper(this.__appSettings);
                RequestHelper request3 = new RequestHelper(this.__appSettings);
                RequestHelper request4 = new RequestHelper(this.__appSettings);
                RequestHelper request5 = new RequestHelper(this.__appSettings);

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string data3 = Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestPrioridadCatalogoModel { id = 0, menu = true }));
                string data4 = Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestUsuarioDepartamentoModel { idUsuario= Int32.Parse(HttpContext.Session.GetString("idUsuario")) }));

                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse1 = request1.doPost("Reporte/ObtenerTicket", data, HttpContext.Session.GetString("token"));
                Task<string> taskResponse2 = request2.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                Task<string> taskResponse3 = request3.doPost("Reporte/PrioridadCatalogo", data3, HttpContext.Session.GetString("token"));
                Task<string> taskResponse4 = request4.doPost("Reporte/UsuarioDepartamento", data4, HttpContext.Session.GetString("token"));
                Task<string> taskResponse5 = request5.doPost("Reporte/CatalogoDepartamento", null, HttpContext.Session.GetString("token"));



                Task.WhenAll(taskLog, taskResponse1, taskResponse2,taskResponse3, taskResponse4);
                string response1 = taskResponse1.Result;
                string response2 = taskResponse2.Result;
                string response3 = taskResponse3.Result;
                string response4 = taskResponse4.Result;
                string response5 = taskResponse5.Result;

                if (request1.codigoRespuesta == 0 && request2.codigoRespuesta == 0 && request3.codigoRespuesta == 0 && request4.codigoRespuesta == 0 && request5.codigoRespuesta == 0)
                {
                    responseD1 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicket>(response1);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD1);

                    responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);

                    responseD3 = JsonConvert.DeserializeObject<ReporteResponsePrioridadCatalogo>(response3);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD3);

                    responseD4 = JsonConvert.DeserializeObject<ReporteResponseUsuarioDepartamento>(response4);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD4);

                    responseD5 = JsonConvert.DeserializeObject<ReporteResponseCatalogoDepartamento>(response5);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD5);


                    if (responseD1.CodigoRespuesta == 0 && responseD2.CodigoRespuesta == 0 && responseD3.CodigoRespuesta == 0 && responseD4.CodigoRespuesta == 0 && responseD5.CodigoRespuesta == 0)
                    {
                        responseD.respuesta = responseD1.respuesta;

                       

                        responseD.respuesta3 = responseD3.respuesta;
                        responseD.respuesta4 = responseD4.respuesta;
                        responseD.respuesta5 = responseD5.respuesta;

                        foreach (ReporteObtenerTicketConversacionModel itemLista in responseD2.respuesta.Where(x => x.adjunto == 1).ToList())
                        {
                            string nombre = this.__appSettings.TicketDir + item.ticketID + "\\" + itemLista.hashName;

                            Byte[] bytes = null;
                            String file = "";
                            if (System.IO.File.Exists(nombre))
                            {
                                bytes = System.IO.File.ReadAllBytes(nombre);
                                file = "data:text/plain;base64," + Convert.ToBase64String(bytes);
                            }

                            responseD2.respuesta[responseD2.respuesta.FindIndex(x => x == itemLista)].base64 = file;
                        }

                        responseD.respuesta2 = responseD2.respuesta;

                        responseD.CodigoRespuesta = 0;
                        responseD.MensajeRespuesta = "";
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -8;
                        responseD.MensajeRespuesta = "Ocurrio un error al obtener la respuesta";
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = -7;
                    responseD.MensajeRespuesta = "Ocurrio un problema al cargar la información";
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }

        [Route("CambioPrioridad")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseCambioPrioridad CambioPrioridad([FromBody]ReporteRequestCambioPrioridadModel item)
        {
            ReporteResponseCambioPrioridad responseD = new ReporteResponseCambioPrioridad();
            try
            {
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reporte/CambioPrioridad", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseCambioPrioridad>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD.CodigoRespuesta == 0)
                    {

                        Task<string> taskResponse2 = request.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                        Task<Boolean> taskLog2 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                        Task.WhenAll(taskLog2, taskResponse2);
                        string response2 = taskResponse2.Result;
                        if (request.codigoRespuesta == 0)
                        {
                            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
                            responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                            if (responseD2.CodigoRespuesta == 0)
                            {
                                responseD.lista = responseD2.respuesta;
                            }
                            
                        }
                    }
                    
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }

               
        [Route("CambioVencido")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseCambioVencido CambioVencido([FromBody]ReporteRequestCambioVencidoModel item)
        {
            ReporteResponseCambioVencido responseD = new ReporteResponseCambioVencido();
            try
            {
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                item.vencido = true;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reporte/CambioVencido", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseCambioVencido>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD.CodigoRespuesta == 0)
                    {

                        Task<string> taskResponse2 = request.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                        Task<Boolean> taskLog2 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                        Task.WhenAll(taskLog2, taskResponse2);
                        string response2 = taskResponse2.Result;
                        if (request.codigoRespuesta == 0)
                        {
                            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
                            responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                            if (responseD2.CodigoRespuesta == 0)
                            {
                                responseD.lista = responseD2.respuesta;
                            }

                        }
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }


        [Route("LiberarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseLiberarTicket LiberarTicket([FromBody]ReporteRequestLiberarTicketModel item)
        {
            ReporteResponseLiberarTicket responseD = new ReporteResponseLiberarTicket();
            try
            {
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reporte/LiberarTicket", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseLiberarTicket>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD.CodigoRespuesta == 0)
                    {

                        Task<string> taskResponse2 = request.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                        Task<Boolean> taskLog2 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                        Task.WhenAll(taskLog2, taskResponse2);
                        string response2 = taskResponse2.Result;
                        if (request.codigoRespuesta == 0)
                        {
                            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
                            responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                            if (responseD2.CodigoRespuesta == 0)
                            {
                                responseD.lista = responseD2.respuesta;
                            }

                        }
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }

        [Route("AsignarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseAsignarTicket AsignarTicket([FromBody]ReporteRequestAsignarTicketModel item)
        {
            ReporteResponseAsignarTicket responseD = new ReporteResponseAsignarTicket();
            try
            {
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reporte/AsignarTicket", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseAsignarTicket>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD.CodigoRespuesta == 0)
                    {

                        Task<string> taskResponse2 = request.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                        Task<Boolean> taskLog2 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                        Task.WhenAll(taskLog2, taskResponse2);
                        string response2 = taskResponse2.Result;
                        if (request.codigoRespuesta == 0)
                        {
                            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
                            responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                            if (responseD2.CodigoRespuesta == 0)
                            {
                                responseD.lista = responseD2.respuesta;
                            }

                        }
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }

               
        [Route("TransferirTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseTransferirTicket TransferirTicket([FromBody]ReporteRequestTransferirTicketModel item)
        {
            ReporteResponseTransferirTicket responseD = new ReporteResponseTransferirTicket();
            try
            {
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reporte/TransferirTicket", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseTransferirTicket>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD.CodigoRespuesta == 0)
                    {

                        Task<string> taskResponse2 = request.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                        Task<Boolean> taskLog2 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                        Task.WhenAll(taskLog2, taskResponse2);
                        string response2 = taskResponse2.Result;
                        if (request.codigoRespuesta == 0)
                        {
                            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
                            responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                            if (responseD2.CodigoRespuesta == 0)
                            {
                                responseD.lista = responseD2.respuesta;
                            }

                        }
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
               
        [Route("CerrarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseCerrarTicket CerrarTicket([FromBody]ReporteRequestCerrarTicketModel item)
        {
            ReporteResponseCerrarTicket responseD = new ReporteResponseCerrarTicket();
            try
            {
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                item.idCatalogoStatusTicket = 2;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reporte/CerrarTicket", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseCerrarTicket>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD.CodigoRespuesta == 0)
                    {

                        Task<string> taskResponse2 = request.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                        Task<Boolean> taskLog2 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                        Task.WhenAll(taskLog2, taskResponse2);
                        string response2 = taskResponse2.Result;
                        if (request.codigoRespuesta == 0)
                        {
                            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
                            responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                            if (responseD2.CodigoRespuesta == 0)
                            {
                                responseD.lista = responseD2.respuesta;
                            }

                        }
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
                     
        [Route("NuevaRespuesta")]
        [HttpPost, DisableRequestSizeLimit]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseNuevaRespuesta NuevaRespuesta()
        {
            ReporteRequestNuevaRespuestaModel item = new ReporteRequestNuevaRespuestaModel();
        
            ReporteResponseNuevaRespuesta responseD = new ReporteResponseNuevaRespuesta();
            try
            {
                item.ticketID = int.Parse(Request.Form["ticketID"].ToString());
                item.mensaje = Request.Form["mensaje"].ToString();
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                if (Request.Form.Files.Count > 0)
                {
                    item.file = Request.Form.Files[0];
                }
                
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reporte/NuevaRespuesta", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseNuevaRespuesta>(response);

                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                    Task<Boolean> taskResponse3 = FileSanitizer.uploadFile(item.file, this.__appSettings.TicketDir + item.ticketID + "\\", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                    taskResponse3.Wait();
                    Boolean response3 =  taskResponse3.Result;
                    if (response3)
                    {
                        ReporteRequestNuevoTicketAdjuntoModel data2r = new ReporteRequestNuevoTicketAdjuntoModel();
                        data2r.ticketID = item.ticketID;
                        data2r.idConversacion = responseD.respuesta.resultado;
                        data2r.tipoConversacion = "R";
                        data2r.tipoArchivo = Path.GetExtension(item.file.FileName);
                        data2r.tamano = item.file.Length.ToString();
                        data2r.hashName = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        data2r.nombre = item.file.FileName;
                        data2r.ip = Seguridad.GetIPAddress();
                        string data2 = Seguridad.Encrypt(JsonConvert.SerializeObject(data2r));
                        Task<string> taskResponse4 = request.doPost("Reporte/NuevoTicketAdjunto", data2, HttpContext.Session.GetString("token"));
                        Logger.DoLog(MethodBase.GetCurrentMethod(), data2r);
                        taskResponse4.Wait();
                    }

                    if (responseD.CodigoRespuesta == 0)
                    {

                        Task<string> taskResponse2 = request.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                        Logger.DoLog(MethodBase.GetCurrentMethod(), data);
                        taskResponse2.Wait();
                        string response2 = taskResponse2.Result;
                        
                        if (request.codigoRespuesta == 0)
                        {
                            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
                            responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                            if (responseD2.CodigoRespuesta == 0)
                            {

                                foreach (ReporteObtenerTicketConversacionModel itemLista in responseD2.respuesta.Where(x => x.adjunto == 1).ToList())
                                {
                                    string nombre = this.__appSettings.TicketDir + item.ticketID + "\\" + itemLista.hashName;

                                    Byte[] bytes = null;
                                    String file = "";
                                    if (System.IO.File.Exists(nombre))
                                    {
                                        bytes = System.IO.File.ReadAllBytes(nombre);
                                        file = "data:text/plain;base64," + Convert.ToBase64String(bytes);
                                    }

                                    responseD2.respuesta[responseD2.respuesta.FindIndex(x => x == itemLista)].base64 = file;
                                }


                                responseD.lista = responseD2.respuesta;
                            }
                        }
                    }
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }
        
        [Route("NuevaNota")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseNuevaNota NuevaNota()
        {
            ReporteResponseNuevaNota responseD = new ReporteResponseNuevaNota();
            ReporteRequestNuevaNotaModel item = new ReporteRequestNuevaNotaModel();
            try
            {
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ticketID = int.Parse(Request.Form["ticketID"].ToString());
                item.mensaje = Request.Form["mensaje"].ToString();
                item.titulo = "Nota Interna";
                if (Request.Form.Files.Count > 0)
                {
                    item.file = Request.Form.Files[0];
                }

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reporte/NuevaNota", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseNuevaNota>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                    Task<Boolean> taskResponse3 = FileSanitizer.uploadFile(item.file, this.__appSettings.TicketDir + item.ticketID + "\\", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                    taskResponse3.Wait();
                    Boolean response3 = taskResponse3.Result;
                    if (response3)
                    {
                        ReporteRequestNuevoTicketAdjuntoModel data2r = new ReporteRequestNuevoTicketAdjuntoModel();
                        data2r.ticketID = item.ticketID;
                        data2r.idConversacion = responseD.respuesta.resultado;
                        data2r.tipoConversacion = "N";
                        data2r.tipoArchivo = Path.GetExtension(item.file.FileName);
                        data2r.tamano = item.file.Length.ToString();
                        data2r.hashName = DateTime.Now.ToString("ddMMyyyyHHmmss");
                        data2r.nombre = item.file.FileName;
                        data2r.ip = Seguridad.GetIPAddress();
                        string data2 = Seguridad.Encrypt(JsonConvert.SerializeObject(data2r));
                        Task<string> taskResponse4 = request.doPost("Reporte/NuevoTicketAdjunto", data2, HttpContext.Session.GetString("token"));
                        Logger.DoLog(MethodBase.GetCurrentMethod(), data2r);
                        taskResponse4.Wait();
                    }


                    if (responseD.CodigoRespuesta == 0)
                    {

                        Task<string> taskResponse2 = request.doPost("Reporte/ObtenerTicketConversacion", data, HttpContext.Session.GetString("token"));
                        Logger.DoLog(MethodBase.GetCurrentMethod(), data);
                        taskResponse2.Wait();
                        string response2 = taskResponse2.Result;
                        
                        if (request.codigoRespuesta == 0)
                        {
                            ReporteResponseObtenerTicketConversacion responseD2 = new ReporteResponseObtenerTicketConversacion();
                            responseD2 = JsonConvert.DeserializeObject<ReporteResponseObtenerTicketConversacion>(response2);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                            if (responseD2.CodigoRespuesta == 0)
                            {

                                foreach (ReporteObtenerTicketConversacionModel itemLista in responseD2.respuesta.Where(x => x.adjunto == 1).ToList())
                                {
                                    string nombre = this.__appSettings.TicketDir + item.ticketID + "\\" + itemLista.hashName;

                                    Byte[] bytes = null;
                                    String file = "";
                                    if (System.IO.File.Exists(nombre))
                                    {
                                        bytes = System.IO.File.ReadAllBytes(nombre);
                                        file = "data:text/plain;base64," + Convert.ToBase64String(bytes);
                                    }

                                    responseD2.respuesta[responseD2.respuesta.FindIndex(x => x == itemLista)].base64 = file;
                                }


                                responseD.lista = responseD2.respuesta;


                            }
                        }
                    }
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }

        [Route("listadoCatalogosAgregarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public OperacionesResponselistadoCatalogosAgregarTicket listadoCatalogosAgregarTicket()
        {

            OperacionesResponselistadoCatalogosAgregarTicket responseD = new OperacionesResponselistadoCatalogosAgregarTicket();
            try
            {
                RequestHelper request_temas = new RequestHelper(this.__appSettings);
                RequestHelper request_asuntos = new RequestHelper(this.__appSettings);
                RequestHelper request_departamentos = new RequestHelper(this.__appSettings);
                RequestHelper request_vias = new RequestHelper(this.__appSettings);
                RequestHelper request_prioridad = new RequestHelper(this.__appSettings);



                Task<string> taskResponse_temas = request_temas.doPost("Reporte/ListadoCatalogoTemas", Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestCatalogo { id = 0, menu = 1 })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_asuntos = request_asuntos.doPost("Reporte/ListadoCatalogoAsuntos", Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestCatalogoAsunto { id = 0, menu = 1, idTema =0 })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_departamentos = request_departamentos.doPost("Reporte/ListadoCatalogoDepartamentos", "", HttpContext.Session.GetString("token"));
                Task<string> taskResponse_vias = request_vias.doPost("Reporte/ListadoCatalogoViaContacto", Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestCatalogo { id = 0, menu = 1 })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_prioridad = request_prioridad.doPost("Reporte/ListadoCatalogoPrioridad", Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestCatalogo { id = 0, menu = 1 })), HttpContext.Session.GetString("token"));



                Task.WhenAll(taskResponse_temas, taskResponse_asuntos, taskResponse_departamentos, taskResponse_vias, taskResponse_prioridad);
                string response_temas = taskResponse_temas.Result;
                string response_asuntos = taskResponse_asuntos.Result;
                string response_departamentos = taskResponse_departamentos.Result;
                string response_vias = taskResponse_vias.Result;
                string response_prioridad = taskResponse_prioridad.Result;

                if (request_temas.codigoRespuesta == 0 && request_asuntos.codigoRespuesta == 0 && request_vias.codigoRespuesta == 0 && request_departamentos.codigoRespuesta == 0 && request_departamentos.codigoRespuesta == 0)
                {
                    responseD.temas = JsonConvert.DeserializeObject<ReporteResponseListadoTemas>(response_temas);
                    responseD.asuntos = JsonConvert.DeserializeObject<ReporteResponseListadoGenerico>(response_asuntos);
                    responseD.departamentos = JsonConvert.DeserializeObject<ReporteResponseListadoGenerico>(response_departamentos);
                    responseD.vias = JsonConvert.DeserializeObject<ReporteResponseListadoGenerico>(response_vias);
                    responseD.prioridades = JsonConvert.DeserializeObject<ReporteResponseListadoGenerico>(response_prioridad);

                    if (responseD.temas.CodigoRespuesta == 0 && responseD.asuntos.CodigoRespuesta == 0 && responseD.departamentos.CodigoRespuesta == 0)
                    {
                        responseD.CodigoRespuesta = 0;
                        responseD.MensajeRespuesta = "";
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -2;
                        responseD.MensajeRespuesta = "Problemas al obtener los catalogos";
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = -1;
                    responseD.MensajeRespuesta = "Problemas al obtener los catalogos";
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = ex.Message;
            }
            return responseD;
        }
        

        [Route("AgregarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_17")]
        public ReporteResponseNuevoTicket AgregarTicket([FromBody]ReporteRequestNuevoTicketModel item)
        {
            ReporteResponseNuevoTicket responseD = new ReporteResponseNuevoTicket();

            try
            {
                item.Ip = Seguridad.GetIPAddress();
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Reporte/AgregarTicket", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseNuevoTicket>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = ex.Message;
            }
            return responseD;
        }



        #endregion

        #region Reporte de Errores

        [Route("ObtenerCadenaError")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_18")]
        public async Task<ReporteResponseObtenerCadenaError> ObtenerCadenaError([FromBody]ReporteRequestObtenerCadenaErrorModel item)
        {
            ReporteResponseObtenerCadenaError responseD = new ReporteResponseObtenerCadenaError();
            ReporteResponseEstablecimientos responseD1 = new ReporteResponseEstablecimientos();
            ReporteResponseCatalogoClasificacionProducto responseD2 = new ReporteResponseCatalogoClasificacionProducto();
    
            try
            {
                RequestHelper request1 = new RequestHelper(this.__appSettings);
                RequestHelper request2 = new RequestHelper(this.__appSettings);

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequest2ObtenerCadenaErrorModel { idCadena= item.idCadena,activo = 2,menu = true }));
                string data2 = Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestCatalogoClasificacionProductoModel { id = 0, menu = true }));
  

                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse1 = request1.doPost("Reportes/getEstablecimientos", data, HttpContext.Session.GetString("token"));
                Task<string> taskResponse2 = request2.doPost("Reportes/CatalogoClasificacionProducto", data2, HttpContext.Session.GetString("token"));
   


                await Task.WhenAll(taskLog, taskResponse1, taskResponse2);
                string response1 = taskResponse1.Result;
                string response2 = taskResponse2.Result;

                if (request1.codigoRespuesta == 0 && request2.codigoRespuesta == 0 )
                {
                    responseD1 = JsonConvert.DeserializeObject<ReporteResponseEstablecimientos>(response1);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD1);


                    responseD2 = JsonConvert.DeserializeObject<ReporteResponseCatalogoClasificacionProducto>(response2);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD2);
                                           
                    
                    if (responseD1.CodigoRespuesta == 0 && responseD2.CodigoRespuesta == 0 )
                    {
                        responseD.respuesta = responseD1.respuesta;
                        responseD.respuesta2 = responseD2.respuesta;
                        responseD.CodigoRespuesta = 0;
                        responseD.MensajeRespuesta = "";
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -8;
                        responseD.MensajeRespuesta = "Ocurrio un error al obtener la respuesta";
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = -7;
                    responseD.MensajeRespuesta = "Ocurrio un problema al cargar la información";
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }


        [Route("ObtenerCatalogosError")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_18")]
        public ReporteResponseObtenerCatalogosError ObtenerCatalogosErrorProducto([FromBody]ReporteRequestObtenerCatalogosErrorModel item)
        {
            ReporteResponseObtenerCatalogosError responseD = new ReporteResponseObtenerCatalogosError();

            ReporteResponseCatalogoProducto responseD3 = new ReporteResponseCatalogoProducto();
            ReporteResponseCatalogoProveedores responseD4 = new ReporteResponseCatalogoProveedores();

            try
            {

                RequestHelper request3 = new RequestHelper(this.__appSettings);
                RequestHelper request4 = new RequestHelper(this.__appSettings);


                string data3 = Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestCatalogoProductoModel { id = 0, menu = true, activo = 2 }));
                string data4 = Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestCatalogoProveedoresModel { id = 0, menu = true, idClasificacionProducto = item.idClasificacion, idCatalogoIntermediario = 0 }));


                Task<Boolean> taskLog3 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data3);
                Task<Boolean> taskLog4 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data4);
                Task<string> taskResponse3 = request3.doPost("Reportes/CatalogoProducto", data3, HttpContext.Session.GetString("token"));
                Task<string> taskResponse4 = request3.doPost("Reportes/CatalogoProveedores", data4, HttpContext.Session.GetString("token"));



                Task.WhenAll(taskLog3, taskLog4, taskResponse3, taskResponse4);

                string response3 = taskResponse3.Result;
                string response4 = taskResponse4.Result;

                if (request3.codigoRespuesta == 0 && request4.codigoRespuesta == 0)
                {


                    responseD3 = JsonConvert.DeserializeObject<ReporteResponseCatalogoProducto>(response3);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD3);

                    responseD4 = JsonConvert.DeserializeObject<ReporteResponseCatalogoProveedores>(response4);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD4);


                    if (responseD3.CodigoRespuesta == 0 && responseD4.CodigoRespuesta == 0 )
                    {
                        responseD.respuesta3 = responseD3.respuesta;
                        responseD.respuesta4 = responseD4.respuesta;
                        responseD.CodigoRespuesta = 0;
                        responseD.MensajeRespuesta = "";
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -8;
                        responseD.MensajeRespuesta = "Ocurrio un error al obtener la respuesta";
                    }

                }
                else
                {
                    responseD.CodigoRespuesta = -7;
                    responseD.MensajeRespuesta = "Ocurrio un problema al cargar la información";
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }

        [Route("ReporteVentaError")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_18")]
        public ReporteResponseReporteError ReporteVentaError([FromBody]ReporteRequestReporteVentaErrorModel item)
        {
            ReporteResponseReporteError responseD = new ReporteResponseReporteError();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                ReporteResponseReporteVentaError responseD3 = new ReporteResponseReporteVentaError();
                ReporteResponseReporteVentaErrorRef responseD4 = new ReporteResponseReporteVentaErrorRef();
                RequestHelper request3 = new RequestHelper(this.__appSettings);
                RequestHelper request4 = new RequestHelper(this.__appSettings);

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));


                Task<Boolean> taskLog3 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse3 = request3.doPost("Reportes/ReporteVentaError", data, HttpContext.Session.GetString("token"));
                Task<string> taskResponse4 = request4.doPost("Reportes/ReporteVentaErrorRef", data, HttpContext.Session.GetString("token"));



                Task.WhenAll(taskLog3, taskResponse3, taskResponse4);

                string response3 = taskResponse3.Result;
                string response4 = taskResponse4.Result;


                if (request.codigoRespuesta == 0)
                {
                    responseD3 = JsonConvert.DeserializeObject<ReporteResponseReporteVentaError>(response3);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD3);
                    responseD4 = JsonConvert.DeserializeObject<ReporteResponseReporteVentaErrorRef>(response4);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD4);
                    if (responseD3.CodigoRespuesta == 0 && responseD4.CodigoRespuesta == 0)
                    {
                        responseD.respuesta3 = responseD3.respuesta;
                        responseD.respuesta4 = responseD4.respuesta;
                        responseD.CodigoRespuesta = 0;
                        responseD.MensajeRespuesta = "";
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -8;
                        responseD.MensajeRespuesta = "Sin resultados";
                    }
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }

               
        [Route("ReporteVentaErrorExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_18")]
        public async Task<ReporteResponseReporteErrorExcel> ReporteVentaErrorExcel([FromBody]ReporteRequestReporteVentaErrorModel item)
        {
            ReporteResponseReporteErrorExcel responseD = new ReporteResponseReporteErrorExcel();
            ExcelPackage workbook = new ExcelPackage();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                ReporteResponseReporteVentaErrorRef responseD4 = new ReporteResponseReporteVentaErrorRef();
                RequestHelper request4 = new RequestHelper(this.__appSettings);

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));


                Task<Boolean> taskLog3 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse4 = request4.doPost("Reportes/ReporteVentaErrorRef", data, HttpContext.Session.GetString("token"));



                await Task.WhenAll(taskLog3, taskResponse4);

                string response4 = taskResponse4.Result;


                if (request.codigoRespuesta == 0)
                {
                    responseD4 = JsonConvert.DeserializeObject<ReporteResponseReporteVentaErrorRef>(response4);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD4);
                    if ( responseD4.CodigoRespuesta == 0)
                    {
                        responseD.respuesta4 = responseD4.respuesta;
                        workbook.Workbook.Worksheets.Add("Reporte de Errores");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Fecha Hora";
                        hojaActual.Cells["B1"].Value = "MTCID";
                        hojaActual.Cells["C1"].Value = "Cadena";
                        hojaActual.Cells["D1"].Value = "Establecimiento";
                        hojaActual.Cells["E1"].Value = "Producto";
                        hojaActual.Cells["F1"].Value = "Referencia";
                        hojaActual.Cells["G1"].Value = "Ref. 2";
                        hojaActual.Cells["H1"].Value = "Entradas";
                        hojaActual.Cells["I1"].Value = "Salidas";
                        hojaActual.Cells["J1"].Value = "Codigo";
                        hojaActual.Cells["K1"].Value = "Descripcion";

                        hojaActual.Cells["A1:K1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ReporteReporteVentaErrorRefModel compra in responseD4.respuesta)
                        {
                            hojaActual.Cells["A" + linea].Value = compra.fechaHora.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.MTCid.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.cadena.ToString();
                            hojaActual.Cells["D" + linea].Value = compra.tienda.ToString();
                            hojaActual.Cells["E" + linea].Value = compra.producto.ToString();
                            hojaActual.Cells["F" + linea].Value = compra.referencia1.ToString();
                            hojaActual.Cells["G" + linea].Value = compra.referencia2.ToString();
                            hojaActual.Cells["H" + linea].Value = Decimal.Parse(compra.entrada.ToString());
                            hojaActual.Cells["I" + linea].Value = Decimal.Parse(compra.salida.ToString());
                            hojaActual.Cells["H" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["I" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["J" + linea].Value = compra.codigoRespuesta.ToString();
                            hojaActual.Cells["K" + linea].Value = compra.descripcionError.ToString();
                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 11];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD.respuesta = base64Excel;
                        responseD.reporteNombre = "Reporte de Errores.xlsx";

                        responseD.CodigoRespuesta = 0;
                        responseD.MensajeRespuesta = "";
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -8;
                        responseD.MensajeRespuesta = "Ocurrio un error al obtener la respuesta";
                    }
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD;
        }

        #endregion

        #region Reporte Venta Error Six

        [Route("Six/listadoClasificacionProducto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_19")]
        public async Task<ReporteResponseClasificacionProducto> listadoClasificacionProductoSix()
        {
            ReporteResponseClasificacionProducto response = new ReporteResponseClasificacionProducto();
            try
            {

                string reqResponse = await request.doPost("Reportes/Six/listadoClasificacionProducto", "", HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteResponseClasificacionProducto>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("Six/listadoProveedores")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_19")]
        public async Task<ReporteResponseProveedores> listadoProveedoresSix([FromBody]ReportesRequestListadoProveedores item)
        {
            ReporteResponseProveedores response = new ReporteResponseProveedores();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/listadoProveedores", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteResponseProveedores>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("Six/listadoProductos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_19")]
        public async Task<ReporteResponseProductos> listadoProductosSix([FromBody]ReportesRequestCatalogoProducto item)
        {
            ReporteResponseProductos response = new ReporteResponseProductos();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/listadoProductos", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteResponseProductos>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("Six/ReporteVentaErrorResumenSix")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_19")]
        public async Task<ReportesResponseVentaErrorResumenSix> reporteVentaErrorResumenSix([FromBody]ReportesRequestReporteVentaErrorSix item)
        {
            ReportesResponseVentaErrorResumenSix response = new ReportesResponseVentaErrorResumenSix();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/ReporteVentaErrorResumenSix", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReportesResponseVentaErrorResumenSix>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("Six/ReporteVentaErrorSix")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_19")]
        public async Task<ReportesResponseVentaErrorSix> reporteVentaErrorSix([FromBody] ReportesRequestReporteVentaErrorSix item)
        {
            ReportesResponseVentaErrorSix response = new ReportesResponseVentaErrorSix();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/ReporteVentaErrorSix", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReportesResponseVentaErrorSix>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("Six/ReporteVentaErrorSixExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06")]
        public async Task<ReportesResponseVentaErrorSixExcel> reporteVentaErrorSixExcel([FromBody] ReportesRequestReporteVentaErrorSix item)
        {
            ReportesResponseVentaErrorSix response = new ReportesResponseVentaErrorSix();
            ReportesResponseVentaErrorSixExcel responseExcel = new ReportesResponseVentaErrorSixExcel();
            ExcelPackage workbook = new ExcelPackage();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/ReporteVentaErrorSix", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<ReportesResponseVentaErrorSix>(reqResponse);

                    workbook.Workbook.Worksheets.Add("Reporte concentrado");
                    var hojaActual = workbook.Workbook.Worksheets["Reporte concentrado"];
                    hojaActual = workbook.Workbook.Worksheets[0];

                    hojaActual.Cells["A1"].Value = "Fecha Hora";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "MTCID";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Cadena";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "Tienda";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Cajero";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "Casificación";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["G1"].Value = "Producto";
                    hojaActual.Cells["G1"].AutoFitColumns();
                    hojaActual.Cells["H1"].Value = "Proveedor";
                    hojaActual.Cells["H1"].AutoFitColumns();
                    hojaActual.Cells["I1"].Value = "Referencia 1";
                    hojaActual.Cells["I1"].AutoFitColumns();
                    hojaActual.Cells["J1"].Value = "Referencia 2";
                    hojaActual.Cells["J1"].AutoFitColumns();
                    hojaActual.Cells["K1"].Value = "Entrada";
                    hojaActual.Cells["K1"].AutoFitColumns();
                    hojaActual.Cells["L1"].Value = "Salida";
                    hojaActual.Cells["L1"].AutoFitColumns();
                    hojaActual.Cells["M1"].Value = "Código Respuesta";
                    hojaActual.Cells["M1"].AutoFitColumns();
                    hojaActual.Cells["N1"].Value = "Descripción Error";
                    hojaActual.Cells["N1"].AutoFitColumns();
                    hojaActual.Cells["A1:N1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (ReporteVentaErrorSix reporte in response.listaVentaError)
                    {

                        hojaActual.Cells["A" + linea].Value = reporte.fechaHora.ToString();
                        hojaActual.Cells["B" + linea].Value = reporte.MTCid.ToString();
                        hojaActual.Cells["C" + linea].Value = reporte.cadena.ToString();
                        hojaActual.Cells["D" + linea].Value = reporte.tienda.ToString();
                        hojaActual.Cells["E" + linea].Value = reporte.cajero.ToString();
                        hojaActual.Cells["F" + linea].Value = reporte.clasificacion.ToString();
                        hojaActual.Cells["G" + linea].Value = reporte.producto.ToString();
                        hojaActual.Cells["H" + linea].Value = reporte.proveedor.ToString();
                        hojaActual.Cells["I" + linea].Value = reporte.referencia1.ToString();
                        hojaActual.Cells["J" + linea].Value = reporte.referencia2.ToString();
                        hojaActual.Cells["K" + linea].Value = Decimal.Parse(reporte.entrada.ToString());
                        hojaActual.Cells["L" + linea].Value = Decimal.Parse(reporte.salida.ToString());
                        hojaActual.Cells["M" + linea].Value = reporte.codigoRespuesta.ToString();
                        hojaActual.Cells["N" + linea].Value = reporte.descripcionError.ToString();

                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 14];
                    var table = hojaActual.Tables.Add(range, "table");
                    //table.ShowTotal = true;
                    table.TableStyle = TableStyles.Medium13;
                    hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                    // identificador para su guardado
                    string base64Excel = string.Empty;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.Position = 0;
                        byte[] excelData = memoryStream.ToArray();

                        base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                        memoryStream.Dispose();


                    }

                    // retornar datos
                    responseExcel.CodigoRespuesta = 0;
                    responseExcel.reporteNombre = "ReporteVentasErrorSIX.xlsx";
                    responseExcel.reporteExcel = base64Excel;
                }

                else
                {
                    responseExcel.CodigoRespuesta = request.codigoRespuesta;
                    responseExcel.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseExcel.CodigoRespuesta = -9;
                responseExcel.MensajeRespuesta = ex.Message;
            }
            finally
            {
                workbook.Dispose();
            }
            return responseExcel;
        }

        #endregion

        #region Reporte Venta Six

        [Route("Six/ReporteVentaSix")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_20")]
        public async Task<ReportesResponseVentaSix> reporteVentaSix([FromBody] ReportesRequestReporteVentaSix item)
        {
            ReportesResponseVentaSix response = new ReportesResponseVentaSix();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/ReporteVentaSix", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReportesResponseVentaSix>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("Six/ReporteVentaSixExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_20")]
        public async Task<ReportesResponseVentaSixExcel> reporteVentaSixExcel([FromBody] ReportesRequestReporteVentaSix item)
        {
            ReportesResponseVentaSix response = new ReportesResponseVentaSix();
            ReportesResponseVentaSixExcel responseExcel = new ReportesResponseVentaSixExcel();
            ExcelPackage workbook = new ExcelPackage();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/ReporteVentaSix", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<ReportesResponseVentaSix>(reqResponse);

                    workbook.Workbook.Worksheets.Add("Reporte concentrado");
                    var hojaActual = workbook.Workbook.Worksheets["Reporte concentrado"];
                    hojaActual = workbook.Workbook.Worksheets[0];

                    hojaActual.Cells["A1"].Value = "Fecha Hora";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "MTCID";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Cadena";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "Tienda";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Cajero";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "Producto";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["G1"].Value = "Proveedor";
                    hojaActual.Cells["G1"].AutoFitColumns();
                    hojaActual.Cells["H1"].Value = "Referencia 1";
                    hojaActual.Cells["H1"].AutoFitColumns();
                    hojaActual.Cells["I1"].Value = "Referencia 2";
                    hojaActual.Cells["I1"].AutoFitColumns();
                    hojaActual.Cells["J1"].Value = "Entrada";
                    hojaActual.Cells["J1"].AutoFitColumns();
                    hojaActual.Cells["K1"].Value = "Salida";
                    hojaActual.Cells["K1"].AutoFitColumns();
                    hojaActual.Cells["L1"].Value = "Comision Recarga";
                    hojaActual.Cells["L1"].AutoFitColumns();
                    hojaActual.Cells["M1"].Value = "Comision Establecimiento";
                    hojaActual.Cells["M1"].AutoFitColumns();
                    hojaActual.Cells["N1"].Value = "Autorizacion";
                    hojaActual.Cells["N1"].AutoFitColumns();
                    hojaActual.Cells["O1"].Value = "Tipo Terminal";
                    hojaActual.Cells["O1"].AutoFitColumns();
                    hojaActual.Cells["A1:O1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (ReporteVentaSix reporte in response.listaVentaExitosa)
                    {

                        hojaActual.Cells["A" + linea].Value = reporte.fechaHora.ToString();
                        hojaActual.Cells["B" + linea].Value = reporte.MTCid.ToString();
                        hojaActual.Cells["C" + linea].Value = reporte.cadena.ToString();
                        hojaActual.Cells["D" + linea].Value = reporte.tienda.ToString();
                        hojaActual.Cells["E" + linea].Value = reporte.cajero.ToString();
                        hojaActual.Cells["F" + linea].Value = reporte.producto.ToString();
                        hojaActual.Cells["G" + linea].Value = reporte.proveedor.ToString();
                        hojaActual.Cells["H" + linea].Value = reporte.referencia1.ToString();
                        hojaActual.Cells["I" + linea].Value = reporte.referencia2.ToString();
                        hojaActual.Cells["J" + linea].Value = Decimal.Parse(reporte.entrada.ToString());
                        hojaActual.Cells["K" + linea].Value = Decimal.Parse(reporte.salida.ToString());
                        hojaActual.Cells["L" + linea].Value = Decimal.Parse(reporte.comisionRecarga.ToString());
                        hojaActual.Cells["M" + linea].Value = Decimal.Parse(reporte.comisionEstablecimiento.ToString());
                        hojaActual.Cells["J" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["K" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["L" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["M" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["N" + linea].Value = reporte.autorizacion.ToString();
                        hojaActual.Cells["O" + linea].Value = reporte.tipoTerminal.ToString();

                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 15];
                    var table = hojaActual.Tables.Add(range, "table");
                    //table.ShowTotal = true;
                    table.TableStyle = TableStyles.Medium13;
                    hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                    // identificador para su guardado
                    string base64Excel = string.Empty;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.Position = 0;
                        byte[] excelData = memoryStream.ToArray();

                        base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                        memoryStream.Dispose();


                    }

                    // retornar datos
                    responseExcel.CodigoRespuesta = 0;
                    responseExcel.reporteNombre = "ReporteVentasSIX.xlsx";
                    responseExcel.reporteExcel = base64Excel;
                }

                else
                {
                    responseExcel.CodigoRespuesta = request.codigoRespuesta;
                    responseExcel.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseExcel.CodigoRespuesta = -9;
                responseExcel.MensajeRespuesta = ex.Message;
            }
            finally
            {
                workbook.Dispose();
            }
            return responseExcel;
        }


        #endregion

        #region Reporte Balance Six

        [Route("Six/ReporteBalanceSix")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_21")]
        public async Task<ReportesResponseBalanceSix> reporteBalanceSix([FromBody] ReportesRequestBalanceSix item)
        {
            ReportesResponseBalanceSix response = new ReportesResponseBalanceSix();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/ReporteBalanceSix", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReportesResponseBalanceSix>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("Six/ReporteBalanceSixExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_21")]
        public async Task<ReportesResponseBalanceSixExcel> reporteBalanceSixExcel([FromBody] ReportesRequestBalanceSix item)
        {
            ReportesResponseBalanceSix response = new ReportesResponseBalanceSix();
            ReportesResponseBalanceSixExcel responseExcel = new ReportesResponseBalanceSixExcel();
            ExcelPackage workbook = new ExcelPackage();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/Six/ReporteBalanceSixExcel", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<ReportesResponseBalanceSix>(reqResponse);

                    workbook.Workbook.Worksheets.Add("Reporte concentrado");
                    var hojaActual = workbook.Workbook.Worksheets["Reporte concentrado"];
                    hojaActual = workbook.Workbook.Worksheets[0];

                    hojaActual.Cells["A1"].Value = "Fecha Inicial";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Saldo Inicial";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Depositos";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "Comisión Depósitos";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Entradas";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "Salidas";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["G1"].Value = "Compras";
                    hojaActual.Cells["G1"].AutoFitColumns();
                    hojaActual.Cells["H1"].Value = "Comisión Establecimiento";
                    hojaActual.Cells["H1"].AutoFitColumns();
                    hojaActual.Cells["I1"].Value = "Comisioón Recarga";
                    hojaActual.Cells["I1"].AutoFitColumns();
                    hojaActual.Cells["J1"].Value = "Crédito";
                    hojaActual.Cells["J1"].AutoFitColumns();
                    hojaActual.Cells["K1"].Value = "Saldo Final";
                    hojaActual.Cells["K1"].AutoFitColumns();
                    hojaActual.Cells["L1"].Value = "Saldo Calculado";
                    hojaActual.Cells["L1"].AutoFitColumns();
                    hojaActual.Cells["A1:L1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (BalanceSix reporte in response.listadoBalance)
                    {

                        hojaActual.Cells["A" + linea].Value = reporte.fechaInicial.ToString();
                        hojaActual.Cells["B" + linea].Value = reporte.saldoInicial.ToString();
                        hojaActual.Cells["C" + linea].Value = reporte.depositos.ToString();
                        hojaActual.Cells["D" + linea].Value = reporte.comisionDepositos.ToString();
                        hojaActual.Cells["E" + linea].Value = Decimal.Parse(reporte.entradas.ToString());
                        hojaActual.Cells["F" + linea].Value = Decimal.Parse(reporte.salidas.ToString());
                        hojaActual.Cells["G" + linea].Value = Decimal.Parse(reporte.compras.ToString());
                        hojaActual.Cells["H" + linea].Value = Decimal.Parse(reporte.comisionEstablecimiento.ToString());
                        hojaActual.Cells["I" + linea].Value = Decimal.Parse(reporte.comisionRecarga.ToString());
                        hojaActual.Cells["J" + linea].Value = Decimal.Parse(reporte.credito.ToString());
                        hojaActual.Cells["K" + linea].Value = Decimal.Parse(reporte.saldoFinal.ToString());
                        hojaActual.Cells["L" + linea].Value = Decimal.Parse(reporte.saldoCalculado.ToString());
                        hojaActual.Cells["E" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["H" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["I" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["J" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["K" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["L" + linea].Style.Numberformat.Format = "$#,##0.00";

                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 12];
                    var table = hojaActual.Tables.Add(range, "table");
                    //table.ShowTotal = true;
                    table.TableStyle = TableStyles.Medium13;
                    hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                    // identificador para su guardado
                    string base64Excel = string.Empty;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.Position = 0;
                        byte[] excelData = memoryStream.ToArray();

                        base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                        memoryStream.Dispose();
                    }

                    // retornar datos
                    responseExcel.CodigoRespuesta = 0;
                    responseExcel.reporteNombre = "ReporteBalanceSix.xlsx";
                    responseExcel.reporteExcel = base64Excel;
                }

                else
                {
                    responseExcel.CodigoRespuesta = request.codigoRespuesta;
                    responseExcel.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseExcel.CodigoRespuesta = -9;
                responseExcel.MensajeRespuesta = ex.Message;
            }
            finally
            {
                workbook.Dispose();
            }
            return responseExcel;
        }

        #endregion

        #region Liberacion Ejecutivos

        [Route("liberacionesEjecutivos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_22")]
        public async Task<ReportesResponseDepositosEjecutivo> reporteLiberacionesEjecutivos([FromBody] ReportesRequestDepositosEjecutivo item)
        {
            ReportesResponseDepositosEjecutivo response = new ReportesResponseDepositosEjecutivo();
            try
            {

                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/liberacionesEjecutivos", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReportesResponseDepositosEjecutivo>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("liberacionesEjecutivos/getUsuarioDepartamento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_22")]
        public async Task<RecursosResponseListaUsuarios> getUsuarioDepartamento()
        {
            RequestUsuarioDepartamento item = new RequestUsuarioDepartamento();
            RecursosResponseListaUsuarios response = new RecursosResponseListaUsuarios();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.Ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/getUsuarioDepartamento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListaUsuarios>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("liberacionesEjecutivos/listadoEstatus")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_22")]
        public async Task<LiberacionesResponseListadoEstatus> listadoEstatus()
        {
            LiberacionesResponseListadoEstatus response = new LiberacionesResponseListadoEstatus();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/EstatusDepositos/listadoEstatus", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatus>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("liberacionesEjecutivos/reporteExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_22")]
        public async Task<ReportesResponseDepositosEjecutivoExcel> reporteConcentradoExcel([FromBody] ReportesRequestDepositosEjecutivo item)
        {
            ReportesResponseDepositosEjecutivo response = new ReportesResponseDepositosEjecutivo();
            ReportesResponseDepositosEjecutivoExcel responseExcel = new ReportesResponseDepositosEjecutivoExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                if ((!string.IsNullOrEmpty(item.desde) && (!string.IsNullOrEmpty(item.hasta))) && (DateTime.Parse(item.desde.Split("T")[0]) <= DateTime.Parse(item.desde.Split("T")[0])))
                {
                    item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                    item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".999";
                 

                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Reportes/liberacionesEjecutivos", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {

                        response = JsonConvert.DeserializeObject<ReportesResponseDepositosEjecutivo>(reqResponse);

                        workbook.Workbook.Worksheets.Add("Reporte Liberaciones Ejecutivo");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte Liberaciones Ejecutivo"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Deposito";
                        hojaActual.Cells["A1"].AutoFitColumns();
                        hojaActual.Cells["B1"].Value = "MTCID";
                        hojaActual.Cells["B1"].AutoFitColumns();
                        hojaActual.Cells["C1"].Value = "Monto";
                        hojaActual.Cells["C1"].AutoFitColumns();
                        hojaActual.Cells["D1"].Value = "Banco";
                        hojaActual.Cells["D1"].AutoFitColumns();
                        hojaActual.Cells["E1"].Value = "Forma de pago";
                        hojaActual.Cells["E1"].AutoFitColumns();
                        hojaActual.Cells["F1"].Value = "Referencia";
                        hojaActual.Cells["F1"].AutoFitColumns();
                        hojaActual.Cells["G1"].Value = "Hora solicitud";
                        hojaActual.Cells["G1"].AutoFitColumns();
                        hojaActual.Cells["H1"].Value = "Hora Liberación";
                        hojaActual.Cells["H1"].AutoFitColumns();
                        hojaActual.Cells["I1"].Value = "Estatus";
                        hojaActual.Cells["I1"].AutoFitColumns();
                        hojaActual.Cells["J1"].Value = "Usuario";
                        hojaActual.Cells["J1"].AutoFitColumns();
                        hojaActual.Cells["A1:J1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (DepositosEjecutivo reporte in response.listadoLiberaciones)
                        {

                            hojaActual.Cells["A" + linea].Value = reporte.idDeposito.ToString();
                            hojaActual.Cells["B" + linea].Value = reporte.idCadena.ToString();
                            hojaActual.Cells["C" + linea].Value = Decimal.Parse(reporte.monto.ToString());
                            hojaActual.Cells["C" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["D" + linea].Value = reporte.banco.ToString();
                            hojaActual.Cells["E" + linea].Value = reporte.formaPago.ToString();
                            hojaActual.Cells["F" + linea].Value = reporte.referencia.ToString();
                            hojaActual.Cells["G" + linea].Value = reporte.horaSolicitud.ToString();
                            hojaActual.Cells["H" + linea].Value = reporte.horaLiberacion.ToString();

                            string status = "";

                            switch (reporte.idCatalogoEstatusDeposito)
                            {
                                case "1":
                                    status = "Pendiente";
                                    break;
                                case "2":
                                    status = "Bloqueado";
                                    break;
                                case "3":
                                    status = "Autorizado";
                                    break;
                                case "4":
                                    status = "Rechazado";
                                    break;
                                default:
                                    status = "";
                                    break;
                            }

                            hojaActual.Cells["I" + linea].Value = status;
                            hojaActual.Cells["J" + linea].Value = reporte.usuario.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 10];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();


                        }

                        // retornar datos
                        responseExcel.CodigoRespuesta = 0;
                        responseExcel.reporteNombre = "ReporteLiberacionesEjecutivo.xlsx";
                        responseExcel.reporteExcel = base64Excel;
                    }

                    else
                    {
                        responseExcel.CodigoRespuesta = request.codigoRespuesta;
                        responseExcel.MensajeRespuesta = request.mensajeRespuesta;
                    }

                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Fechas incorrectas";
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseExcel.CodigoRespuesta = -9;
                responseExcel.MensajeRespuesta = ex.Message;
            }
            finally
            {
                workbook.Dispose();
            }
            return responseExcel;
        }


        #endregion

        #region Reporte Conciliaciones

        [Route("ReporteConciliaciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_23")]
        public async Task<ReporteConciliacionesResponse> ReporteConciliaciones([FromBody] ReporteConciliacionesRequest item)
        {
            ReporteConciliacionesResponse responseD = new ReporteConciliacionesResponse();
            DataRequestReply reply = new DataRequestReply();
            try
            {

                item.desde = DateTime.Parse(item.desde.ToString("dd/MM/yyyy 00:00:00.000"));
                item.hasta = DateTime.Parse(item.hasta.ToString("dd/MM/yyyy 23:58:00.000"));
                //string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                //Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                //Task<string> taskResponse = request.doPost("Reportes/reporteConciliaciones", data, HttpContext.Session.GetString("token"));
                //await Task.WhenAll(taskLog, taskResponse);

                reply = _manager.ReporteConciliaciones(item);

                if (reply.CodigoRespuesta == 0)
                {
                    responseD.lista = JsonConvert.DeserializeObject<List<ReporteConciliacionesModel>>(reply.Data.ToStringUtf8());
                    responseD.CodigoRespuesta = 0;
                    responseD.MensajeRespuesta = "";

                }
                else
                {
                    responseD.CodigoRespuesta = reply.CodigoRespuesta;
                    responseD.MensajeRespuesta = reply.MensajeRespuesta;
                }
                return responseD;

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
                return responseD;
            }
            //return responseD;
        }

        [Route("ReporteConciliacionesExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_23")]
        public async Task<ReporteConciliacionesExcelResponse> ReporteConciliacionesExcel([FromBody] ReporteConciliacionesResponse item)
        {
            ReporteConciliacionesExcelResponse responseD1 = new ReporteConciliacionesExcelResponse();
            ExcelPackage workbook = new ExcelPackage();

            try
            {

                if (request.codigoRespuesta == 0)
                {
                    Logger.DoLog(MethodBase.GetCurrentMethod(), item);

                    item.MensajeRespuesta = "Reporte de conciliaciones generado exitosamente";
                    responseD1.respuestaLista = item.lista;

                    workbook.Workbook.Worksheets.Add("Reporte de conciliaciones");
                    var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                    hojaActual = workbook.Workbook.Worksheets[0];

		            //'autorizacion'

                    hojaActual.Cells["A1"].Value = "Fecha Hora";
                    hojaActual.Cells["B1"].Value = "MTCID";
                    hojaActual.Cells["C1"].Value = "Establecimiento";
                    hojaActual.Cells["D1"].Value = "Razón Social";
                    hojaActual.Cells["E1"].Value = "Producto";
                    hojaActual.Cells["F1"].Value = "Referencia";
                    hojaActual.Cells["G1"].Value = "Número de Transacción";
                    hojaActual.Cells["H1"].Value = "Entradas";
                    hojaActual.Cells["I1"].Value = "No. Autorización";
                    hojaActual.Cells["A1:I1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (ReporteConciliacionesModel carteraNew in item.lista)
                    {

                        hojaActual.Cells["A" + linea].Value = carteraNew.fechaHora.ToString();
                        hojaActual.Cells["B" + linea].Value = carteraNew.MTCid.ToString();
                        hojaActual.Cells["C" + linea].Value = carteraNew.idTienda.ToString();
                        hojaActual.Cells["D" + linea].Value = carteraNew.razonSocial.ToString();

                        hojaActual.Cells["E" + linea].Value = carteraNew.producto.ToString();
                        hojaActual.Cells["F" + linea].Value = carteraNew.referencia1.ToString();
                        hojaActual.Cells["G" + linea].Value = carteraNew.numeroTransaccion.ToString();
                        hojaActual.Cells["H" + linea].Value = float.Parse(carteraNew.entrada.ToString());
                        hojaActual.Cells["H" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["I" + linea].Value = carteraNew.autorizacion.ToString();

                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 9];
                    var table = hojaActual.Tables.Add(range, "table");
                    //table.ShowTotal = true;
                    table.TableStyle = TableStyles.Medium13;
                    hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                    // identificador para su guardado
                    string base64Excel = string.Empty;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.Position = 0;
                        byte[] excelData = memoryStream.ToArray();

                        base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                        memoryStream.Dispose();

                    }
                    responseD1.respuesta = base64Excel;
                    responseD1.reporteNombre = "Reporte de Conciliaciones.xlsx";


                    responseD1.MensajeRespuesta = item.MensajeRespuesta;

                }
                else
                {
                    item.CodigoRespuesta = -6;
                    item.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                item.CodigoRespuesta = -9;
                item.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region reporte Clientes Activos

        [Route("listaUsuarioPorRol")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ResponseUsuarioRolModel> listaUsuarioPorRol([FromBody] LeadsRequestListaTipoRol item)
        {
            ResponseUsuarioRolModel response = new ResponseUsuarioRolModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/listaUsuarioPorRol", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseUsuarioRolModel>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }
        [Route("getGruposTrabajo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ResponseUsuarioRolModel> getGruposTrabajo([FromBody] LeadsRequestGruposTrabajoCoordinador item)
        {
            ResponseUsuarioRolModel response = new ResponseUsuarioRolModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/GruposTrabajo/getGruposTrabajo", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseUsuarioRolModel>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("reporteClientesActivos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ReporteClientesActivosResponse> reporteClientesActivos([FromBody] ReporteClientesActivosRequest item)
        {
            ReporteClientesActivosResponse response = new ReporteClientesActivosResponse();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/reporteClientesActivos", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteClientesActivosResponse>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("reporteAltasEjecutivo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ReporteAltasEjecutivoResponse> reporteAltasEjecutivo([FromBody] ReporteClientesActivosRequest item)
        {
            ReporteAltasEjecutivoResponse response = new ReporteAltasEjecutivoResponse();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/reporteAltasEjecutivo", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteAltasEjecutivoResponse>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("reporteMedicionDesempeno")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ReporteMedicionDesempenoResponse> reporteMedicionDesempeno([FromBody] ReporteClientesActivosRequest item)
        {
            ReporteMedicionDesempenoResponse response = new ReporteMedicionDesempenoResponse();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/reporteMedicionDesempeno", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteMedicionDesempenoResponse>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("reporteCanalesMensuales")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<CanalesMensualesResponse> reporteCanalesMensuales([FromBody] CanalMensualesRequest item)
        {
            CanalesMensualesResponse response = new CanalesMensualesResponse();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/canalesMensuales", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {   
                    response = JsonConvert.DeserializeObject<CanalesMensualesResponse>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("reporteClientesActivosExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ReporteClientesActivosExcelResponse> ReporteClientesActivosExcel([FromBody] ReporteClientesActivosRequest item)
        {
            ReporteClientesActivosResponse responseD = new ReporteClientesActivosResponse();
            ReporteClientesActivosExcelResponse responseD1 = new ReporteClientesActivosExcelResponse();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/reporteClientesActivos", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;


                if (request.codigoRespuesta == 0)
                {

                    responseD = JsonConvert.DeserializeObject<ReporteClientesActivosResponse>(response);

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de Clientes Activos generado exitosamente";
                        responseD1.respuestaLista = responseD.lista;

                        workbook.Workbook.Worksheets.Add("Reporte de Clientes Activos");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Coordinador";
                        hojaActual.Cells["B1"].Value = "Ejecutivo";
                        hojaActual.Cells["C1"].Value = "Razón Social";
                        hojaActual.Cells["D1"].Value = "MTCID";
                        hojaActual.Cells["E1"].Value = "Fecha Alta";
                        hojaActual.Cells["F1"].Value = "Fecja Depósito";
                        hojaActual.Cells["G1"].Value = "monto";
                        hojaActual.Cells["H1"].Value = "Venta TAE";
                        hojaActual.Cells["I1"].Value = "Saldo Cadena";
                        hojaActual.Cells["A1:I1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (clientesActivos carteraNew in responseD.lista)
                        {

                            hojaActual.Cells["A" + linea].Value = carteraNew.coordinador.ToString();
                            hojaActual.Cells["B" + linea].Value = carteraNew.ejecutivo.ToString();
                            hojaActual.Cells["C" + linea].Value = carteraNew.razonSocial.ToString();
                            hojaActual.Cells["D" + linea].Value = carteraNew.id.ToString();
                            hojaActual.Cells["E" + linea].Value = carteraNew.fechaAlta.ToString();
                            hojaActual.Cells["F" + linea].Value = carteraNew.fechaDeposito.ToString();
                            hojaActual.Cells["G" + linea].Value = Decimal.Parse(carteraNew.monto.ToString());
                            hojaActual.Cells["H" + linea].Value = Decimal.Parse(carteraNew.ventaTae.ToString());
                            hojaActual.Cells["I" + linea].Value = Decimal.Parse(carteraNew.saldoCadena.ToString());
                            hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["H" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["I" + linea].Style.Numberformat.Format = "$#,##0.00";

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 9];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Clientes Activos.xlsx";

                    }
                    else
                    {
                        responseD.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;
                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
                    
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        [Route("reporteAltasEjecutivosExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ReporteAltasEjecutivoExcelResponse> ReporteAltasEjecutivosExcel([FromBody] ReporteClientesActivosRequest item)
        {
            ReporteAltasEjecutivoResponse responseD = new ReporteAltasEjecutivoResponse();
            ReporteAltasEjecutivoExcelResponse responseD1 = new ReporteAltasEjecutivoExcelResponse();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/reporteAltasEjecutivo", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;


                if (request.codigoRespuesta == 0)
                {

                    responseD = JsonConvert.DeserializeObject<ReporteAltasEjecutivoResponse>(response);

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de Altas Ejecutivo generado exitosamente";
                        responseD1.respuestaLista = responseD.lista;

                        workbook.Workbook.Worksheets.Add("Reporte de Altas Ejecutivo");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Coordinador";
                        hojaActual.Cells["B1"].Value = "Ejecutivo";
                        hojaActual.Cells["C1"].Value = "Razón Social";
                        hojaActual.Cells["D1"].Value = "MTCID";
                        hojaActual.Cells["E1"].Value = "Fecha Alta";
                        hojaActual.Cells["A1:I1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (AltasEjecutivo carteraNew in responseD.lista)
                        {

                            hojaActual.Cells["A" + linea].Value = carteraNew.coordinador.ToString();
                            hojaActual.Cells["B" + linea].Value = carteraNew.ejecutivo.ToString();
                            hojaActual.Cells["C" + linea].Value = carteraNew.razonSocial.ToString();
                            hojaActual.Cells["D" + linea].Value = carteraNew.id.ToString();
                            hojaActual.Cells["E" + linea].Value = carteraNew.fechaAlta.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 5];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Alta Ejecutivos.xlsx";

                    }
                    else
                    {
                        responseD.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;
                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }
        
        [Route("reporteMedicionDesenpenoExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ReporteMedicionDesempenoExcelResponse> ReporteMedicionDesenpenoExcel([FromBody] ReporteClientesActivosRequest item)
        {
            ReporteMedicionDesempenoResponse responseD = new ReporteMedicionDesempenoResponse();
            MetasMensualesResponse responseMetas = new MetasMensualesResponse();
            ReporteMedicionDesempenoExcelResponse responseD1 = new ReporteMedicionDesempenoExcelResponse();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/reporteMedicionDesempeno", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                //prepara data para consulta de metas anteriores
                MetasMensualesRequest requestMetasModel = new MetasMensualesRequest();
                requestMetasModel.mes = item.fechaInicio.Month;
                requestMetasModel.anio = item.fechaInicio.Year;

                string dataMetas = Seguridad.Encrypt(JsonConvert.SerializeObject(requestMetasModel));
                taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), dataMetas);
                taskResponse = request.doPost("Reportes/metasMensuales", dataMetas, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string responseMetasMensuales = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {

                    responseD = JsonConvert.DeserializeObject<ReporteMedicionDesempenoResponse>(response);
                    responseMetas = JsonConvert.DeserializeObject<MetasMensualesResponse>(responseMetasMensuales);

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;

                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                    if (responseD1.CodigoRespuesta == 0)
                    {

                        //variables totales
                        int totalLeads = 0;
                        int totalClientesConDep = 0;
                        int totalClientesSinVenta = 0;
                        int totalClientesConVenta = 0;
                        int totalAvanceMeta = 0;
                        int totalConversionConDep = 0;
                        int totalConversionConVenta = 0;

                        responseD.MensajeRespuesta = "Reporte de Medición de desempeño generado exitosamente";
                        responseD1.respuestaLista = responseD.lista;

                        workbook.Workbook.Worksheets.Add("Reporte de Medición de desempeño");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];


                        hojaActual.Cells["B3:E3"].Value = "CIERRE PARCIAL PARA EL MES DE " + responseMetas.lista.mes +" "+ item.fechaFin.Day + " DE 2022";
                        hojaActual.Cells["B3"].Style.Font.Bold = true;
                        hojaActual.Cells["B3"].Style.Font.Size = 18;
                        hojaActual.Cells["B3"].Style.Font.Color.SetColor(Color.Black);
                        hojaActual.DefaultRowHeight = 15;
                        hojaActual.Cells["B3:E3"].Merge  =true;
                        hojaActual.Cells["B3:E3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        hojaActual.Cells["B3:E3"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 192, 0));
                        hojaActual.Cells["B3:E3"].Worksheet.DefaultRowHeight = 30;

                        hojaActual.Cells["D5:E5"].Value = "META GLOBAL: " + responseMetas.lista.metaClientes ;
                        hojaActual.Cells["D5"].Style.Font.Bold = true;
                        hojaActual.Cells["D5"].Style.Font.Size = 12;
                        hojaActual.Cells["D5"].Style.Font.Color.SetColor(Color.Black);
                        hojaActual.DefaultRowHeight = 10;
                        hojaActual.Cells["D5:E5"].Merge = true;
                        hojaActual.Cells["D5:E5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        hojaActual.Cells["D5:E5"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));

                        hojaActual.Cells["F5:G5"].Value = "% AVANCE GLOBAL: 72%" ;
                        hojaActual.Cells["F5"].Style.Font.Bold = true;
                        hojaActual.Cells["F5"].Style.Font.Size = 12;
                        hojaActual.Cells["F5"].Style.Font.Color.SetColor(Color.Black);
                        hojaActual.DefaultRowHeight = 10;
                        hojaActual.Cells["F5:G5"].Merge = true;
                        hojaActual.Cells["F5:G5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        hojaActual.Cells["F5:G5"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 0, 0));

                        hojaActual.Cells["D7:F7"].Value = "RESUMEN DEL MES (" + responseMetas.lista.mes +")";
                        hojaActual.Cells["D7"].Style.Font.Bold = true;
                        hojaActual.Cells["D7"].Style.Font.Color.SetColor(Color.White);
                        hojaActual.Cells["D7"].Style.Font.Size = 12;
                        hojaActual.DefaultRowHeight = 10;
                        hojaActual.Cells["D7:F7"].Merge = true;
                        hojaActual.Cells["D7:F7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        hojaActual.Cells["D7:F7"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 73, 125));


                        hojaActual.Cells["A8"].Value = "Num.";
                        hojaActual.Cells["B8"].Value = "Coordinador";
                        hojaActual.Cells["C8"].Value = "Ejecutivo";
                        hojaActual.Cells["D8"].Value = "Leads Asignados";
                        hojaActual.Cells["E8"].Value = "Clientes con Depósitos";
                        hojaActual.Cells["F8"].Value = "Clientes con Depósito sin Venta";
                        hojaActual.Cells["G8"].Value = "Clientes con Venta";
                        hojaActual.Cells["H8"].Value = "Meta";
                        hojaActual.Cells["I8"].Value = "Avance de Meta";
                        hojaActual.Cells["J8"].Value = "% Conversión con Depósito";
                        hojaActual.Cells["k8"].Value = "% Conversión con Venta";
                        hojaActual.Cells["A8:K8"].Style.Font.Bold = true;
                        hojaActual.Cells["A8:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        hojaActual.Cells["A8:K8"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                        hojaActual.DefaultRowHeight = 20;
                        hojaActual.Cells["A8:K8"].Style.Font.Color.SetColor(Color.Black);

                        int linea = 9;
                        int auto = 1;
                        foreach (medicionDesempeno carteraNew in responseD.lista)
                        {

                            hojaActual.Cells["A" + linea].Value = auto;
                            hojaActual.Cells["B" + linea].Value = carteraNew.coordinador.ToString();
                            hojaActual.Cells["C" + linea].Value = carteraNew.ejecutivo.ToString();
                            hojaActual.Cells["D" + linea].Value = carteraNew.leadsAsignados.ToString();
                            hojaActual.Cells["E" + linea].Value = carteraNew.clientesConDep.ToString();
                            hojaActual.Cells["F" + linea].Value = carteraNew.clientesSinVentas.ToString();
                            hojaActual.Cells["G" + linea].Value = carteraNew.clientesConVentas.ToString();
                            hojaActual.Cells["H" + linea].Value = carteraNew.meta.ToString();
                            hojaActual.Cells["I" + linea].Value = carteraNew.avanceMeta.ToString() + "%";
                            hojaActual.Cells["J" + linea].Value = carteraNew.conversionConDep.ToString() + "%";
                            hojaActual.Cells["K" + linea].Value = carteraNew.conversionConVenta.ToString() + "%";

                            hojaActual.Cells["A"+linea+":K"+linea].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            hojaActual.Cells["A" + linea + ":K" + linea].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            linea++;
                            auto++;
                            //Suma de totales
                            totalLeads += int.Parse(carteraNew.leadsAsignados.ToString());
                            totalClientesConDep += int.Parse(carteraNew.clientesConDep.ToString());
                            totalClientesSinVenta +=  int.Parse(carteraNew.clientesSinVentas.ToString());
                            totalClientesConVenta +=  int.Parse(carteraNew.clientesConVentas.ToString());
                        }

                        totalAvanceMeta = ((totalClientesConVenta * 100) / responseMetas.lista.metaClientes);
                        totalConversionConDep = ((totalClientesConDep * 100) / totalLeads);
                        totalConversionConVenta = ((totalClientesConVenta * 100) / totalLeads);
                        //introduce los totales al excel
                        linea++;
                        hojaActual.Cells["C" + linea].Value = "TOTAL";
                        hojaActual.Cells["D" + linea].Value = totalLeads;
                        hojaActual.Cells["E" + linea].Value = totalClientesConDep;
                        hojaActual.Cells["F" + linea].Value = totalClientesSinVenta;
                        hojaActual.Cells["G" + linea].Value = totalClientesConVenta;
                        hojaActual.Cells["H" + linea].Value = responseMetas.lista.metaClientes + "%";
                        hojaActual.Cells["I" + linea].Value = totalAvanceMeta + "%";
                        hojaActual.Cells["J" + linea].Value = totalConversionConDep + "%";
                        hojaActual.Cells["K" + linea].Value = totalConversionConVenta + "%";


                        var range = hojaActual.Cells[8, 1, linea - 1, 11];
                        var table = hojaActual.Tables.Add(range, "table");
                        hojaActual.Cells["A8:K8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        hojaActual.Cells["A8:C8"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(196, 189, 151));
                        hojaActual.Cells["D8"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(141,182,226));
                        hojaActual.Cells["E8:F8"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(196, 189, 151));
                        hojaActual.Cells["G8"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(250, 191, 143));
                        hojaActual.Cells["H8:J8"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(196, 189, 151));
                        hojaActual.Cells["K8"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(250, 191, 143));

                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Alta Ejecutivos.xlsx";

                    }
                    else
                    {
                        responseD.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;
                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }


        [Route("reporteCanalesMensualesExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_24")]
        public async Task<ReporteCanalesMensualesExcelResponse> ReporteCanalesMensualesExcel([FromBody] ReporteClientesActivosRequest item)
        {
            CanalesMensualesResponse responseD = new CanalesMensualesResponse();
            ReporteCanalesMensualesExcelResponse responseD1 = new ReporteCanalesMensualesExcelResponse();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/canalesMensuales", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;


                if (request.codigoRespuesta == 0)
                {

                    responseD = JsonConvert.DeserializeObject<CanalesMensualesResponse>(response);

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;

                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de Canales Mensuales generado exitosamente";
                        responseD1.respuestaLista = responseD.lista;

                        workbook.Workbook.Worksheets.Add("Reporte de Canales Mensuales");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Canal";
                        hojaActual.Cells["B1"].Value = "Día 1";
                        hojaActual.Cells["C1"].Value = "Día 2";
                        hojaActual.Cells["D1"].Value = "Día 3";
                        hojaActual.Cells["E1"].Value = "Día 4";
                        hojaActual.Cells["F1"].Value = "Día 5";
                        hojaActual.Cells["G1"].Value = "Día 6";
                        hojaActual.Cells["H1"].Value = "Día 7";
                        hojaActual.Cells["I1"].Value = "Día 8";
                        hojaActual.Cells["J1"].Value = "Día 9";
                        hojaActual.Cells["K1"].Value = "Día 10";
                        hojaActual.Cells["L1"].Value = "Día 11";
                        hojaActual.Cells["M1"].Value = "Día 12";
                        hojaActual.Cells["N1"].Value = "Día 13";
                        hojaActual.Cells["O1"].Value = "Día 14";
                        hojaActual.Cells["P1"].Value = "Día 15";
                        hojaActual.Cells["Q1"].Value = "Día 16";
                        hojaActual.Cells["R1"].Value = "Día 17";
                        hojaActual.Cells["S1"].Value = "Día 18";
                        hojaActual.Cells["T1"].Value = "Día 19";
                        hojaActual.Cells["U1"].Value = "Día 20";
                        hojaActual.Cells["V1"].Value = "Día 21";
                        hojaActual.Cells["W1"].Value = "Día 22";
                        hojaActual.Cells["X1"].Value = "Día 23";
                        hojaActual.Cells["Y1"].Value = "Día 24";
                        hojaActual.Cells["Z1"].Value = "Día 25";
                        hojaActual.Cells["AA1"].Value = "Día 26";
                        hojaActual.Cells["AB1"].Value = "Día 27";
                        hojaActual.Cells["AC1"].Value = "Día 28";
                        hojaActual.Cells["AD1"].Value = "Día 29";
                        hojaActual.Cells["AE1"].Value = "Día 30";

                        if (DateTime.Now.AddMonths(1).AddDays(-1).Day > 30)
                        {
                            hojaActual.Cells["AF1"].Value = "Día 31";
                            hojaActual.Cells["A1:AF1"].Style.Font.Bold = true;
                        }
                        else
                        {
                            hojaActual.Cells["A1:AE1"].Style.Font.Bold = true;
                        }

                        int linea = 2;

                        foreach (CanalMensualesModel carteraNew in responseD.lista)
                        {
                            hojaActual.Cells["A" + linea].Value = carteraNew.canal.ToString();
                            hojaActual.Cells["B" + linea].Value = int.Parse(carteraNew.dia1.ToString());
                            hojaActual.Cells["C" + linea].Value = int.Parse(carteraNew.dia2.ToString());
                            hojaActual.Cells["D" + linea].Value = int.Parse(carteraNew.dia3.ToString());
                            hojaActual.Cells["E" + linea].Value = int.Parse(carteraNew.dia4.ToString());
                            hojaActual.Cells["F" + linea].Value = int.Parse(carteraNew.dia5.ToString());
                            hojaActual.Cells["G" + linea].Value = int.Parse(carteraNew.dia6.ToString());
                            hojaActual.Cells["H" + linea].Value = int.Parse(carteraNew.dia7.ToString());
                            hojaActual.Cells["I" + linea].Value = int.Parse(carteraNew.dia8.ToString());
                            hojaActual.Cells["J" + linea].Value = int.Parse(carteraNew.dia9.ToString());
                            hojaActual.Cells["K" + linea].Value = int.Parse(carteraNew.dia10.ToString());
                            hojaActual.Cells["L" + linea].Value = int.Parse(carteraNew.dia11.ToString());
                            hojaActual.Cells["M" + linea].Value = int.Parse(carteraNew.dia12.ToString());
                            hojaActual.Cells["N" + linea].Value = int.Parse(carteraNew.dia13.ToString());
                            hojaActual.Cells["O" + linea].Value = int.Parse(carteraNew.dia14.ToString());
                            hojaActual.Cells["P" + linea].Value = int.Parse(carteraNew.dia15.ToString());
                            hojaActual.Cells["Q" + linea].Value = int.Parse(carteraNew.dia16.ToString());
                            hojaActual.Cells["R" + linea].Value = int.Parse(carteraNew.dia17.ToString());
                            hojaActual.Cells["S" + linea].Value = int.Parse(carteraNew.dia18.ToString());
                            hojaActual.Cells["T" + linea].Value = int.Parse(carteraNew.dia19.ToString());
                            hojaActual.Cells["U" + linea].Value = int.Parse(carteraNew.dia20.ToString());
                            hojaActual.Cells["V" + linea].Value = int.Parse(carteraNew.dia21.ToString());
                            hojaActual.Cells["W" + linea].Value = int.Parse(carteraNew.dia22.ToString());
                            hojaActual.Cells["X" + linea].Value = int.Parse(carteraNew.dia23.ToString());
                            hojaActual.Cells["Y" + linea].Value = int.Parse(carteraNew.dia24.ToString());
                            hojaActual.Cells["Z" + linea].Value = int.Parse(carteraNew.dia25.ToString());
                            hojaActual.Cells["AA" + linea].Value = int.Parse(carteraNew.dia26.ToString());
                            hojaActual.Cells["AB" + linea].Value = int.Parse(carteraNew.dia27.ToString());
                            hojaActual.Cells["AC" + linea].Value = int.Parse(carteraNew.dia28.ToString());
                            hojaActual.Cells["AD" + linea].Value = int.Parse(carteraNew.dia29.ToString());
                            hojaActual.Cells["AE" + linea].Value = int.Parse(carteraNew.dia30.ToString());
                            if (DateTime.Now.AddMonths(1).AddDays(-1).Day > 30)
                            {
                                hojaActual.Cells["AF" + linea].Value = carteraNew.dia31.ToString();
                            }
                            linea++;
                        }
                        var range = (dynamic)null;
                        if (DateTime.Now.AddMonths(1).AddDays(-1).Day > 30)
                        {
                            range = hojaActual.Cells[1, 1, linea - 1, 32];
                        }
                        else
                        {
                            range = hojaActual.Cells[1, 1, linea - 1, 31];
                        }
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Canales Mensuales.xlsx";

                    }
                    else
                    {
                        responseD.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;
                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte clientes Familias/Grupos

        [Route("reporteClientesFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_25")]
        public async Task<ReporteClientesBusquedaResponse> reporteClientesFamilia([FromBody] ReporteClientesBusquedaRequest item)
        {
            ReporteClientesBusquedaResponse response = new ReporteClientesBusquedaResponse();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/reporteClientesFamilia", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteClientesBusquedaResponse>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("ReporteClientesFamiliaExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_25")]
        public async Task<ReporteClientesBusquedaExcelResponse> ReporteClientesFamiliaExcel([FromBody] ReporteClientesBusquedaRequest item)
        {
            ReporteClientesBusquedaResponse responseD = new ReporteClientesBusquedaResponse();
            ReporteClientesBusquedaExcelResponse responseD1 = new ReporteClientesBusquedaExcelResponse();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/reporteClientesFamilia", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteClientesBusquedaResponse>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de Cadenas por Familia generado exitosamente";
                        responseD1.respuestaLista = responseD.lista;

                        workbook.Workbook.Worksheets.Add("Reporte de Cadenas por Familia");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        //'autorizacion'

                        hojaActual.Cells["A1"].Value = "MTCID";
                        hojaActual.Cells["B1"].Value = "Nombre Comercial";
                        hojaActual.Cells["C1"].Value = "ID Establecimiento";
                        hojaActual.Cells["D1"].Value = "Establecimiento";
                        hojaActual.Cells["A1:D1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (clientesBusqueda carteraNew in responseD.lista)
                        {

                            hojaActual.Cells["A" + linea].Value = carteraNew.idCadena.ToString();
                            hojaActual.Cells["B" + linea].Value = carteraNew.nombreComercial.ToString();
                            hojaActual.Cells["C" + linea].Value = carteraNew.idEstablecimiento.ToString();
                            hojaActual.Cells["D" + linea].Value = carteraNew.nombreEstablecimiento.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 4];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte de Cadenas por Familia.xlsx";

                    }
                    else
                    {
                        responseD.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }



        [Route("reporteClientesGrupo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_26")]
        public async Task<ReporteClientesBusquedaResponse> reporteClientesGrupo([FromBody] ReporteClientesBusquedaRequest item)
        {
            ReporteClientesBusquedaResponse response = new ReporteClientesBusquedaResponse();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Reportes/reporteClientesGrupo", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ReporteClientesBusquedaResponse>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("ReporteClientesGrupoExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_26")]
        public async Task<ReporteClientesBusquedaExcelResponse> ReporteClientesGrupoExcel([FromBody] ReporteClientesBusquedaRequest item)
        {
            ReporteClientesBusquedaResponse responseD = new ReporteClientesBusquedaResponse();
            ReporteClientesBusquedaExcelResponse responseD1 = new ReporteClientesBusquedaExcelResponse();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/reporteClientesGrupo", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteClientesBusquedaResponse>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de Cadenas por Grupos generado exitosamente";
                        responseD1.respuestaLista = responseD.lista;

                        workbook.Workbook.Worksheets.Add("Reporte de Cadenas por Grupos");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        //'autorizacion'

                        hojaActual.Cells["A1"].Value = "MTCID";
                        hojaActual.Cells["B1"].Value = "Nombre Comercial";
                        hojaActual.Cells["C1"].Value = "ID Establecimiento";
                        hojaActual.Cells["D1"].Value = "Establecimiento";
                        hojaActual.Cells["A1:D1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (clientesBusqueda carteraNew in responseD.lista)
                        {

                            hojaActual.Cells["A" + linea].Value = carteraNew.idCadena.ToString();
                            hojaActual.Cells["B" + linea].Value = carteraNew.nombreComercial.ToString();
                            hojaActual.Cells["C" + linea].Value = carteraNew.idEstablecimiento.ToString();
                            hojaActual.Cells["D" + linea].Value = carteraNew.nombreEstablecimiento.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 4];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte de Cadenas por Grupos.xlsx";

                    }
                    else
                    {
                        responseD.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }


        [Route("listadoFamiliasComerciales")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_25")]
        public async Task<SoporteResponseFamiliasComerciales> listadoFamiliasComerciales()
        {
            SoporteRequestFamiliaComercial item = new SoporteRequestFamiliaComercial();
            SoporteResponseFamiliasComerciales response = new SoporteResponseFamiliasComerciales();
            try
            {
                item.id = 0;
                item.menu = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoFamiliasComerciales", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseFamiliasComerciales>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("listadoGrupos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_26")]
        public async Task<SoporteResponseListadoGrupos> listadoGrupos()
        {
            SoporteRequestListadoGrupos item = new SoporteRequestListadoGrupos();
            SoporteResponseListadoGrupos response = new SoporteResponseListadoGrupos();
            try
            {
                item.id = 0;
                item.activo = 2;
                item.menu = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoGrupos", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoGrupos>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        #endregion

        #region Reporte Ventas Mayoristas

        [Route("reporteVentasMayoristas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_25")]
        public async Task<string> reporteVentasMayoristas([FromBody] ReporteRequestReporteVentasModel item)
        {
            ReporteResponseReporteVentas responseD = new ReporteResponseReporteVentas();
            try
            {

                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/reporteVentasMayoristas", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;
                return response;
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
                return "";
            }
            //return responseD;
        }

        [Route("ReporteVentasMayoristasExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_25")]
        public async Task<ReporteResponseReporteVentasExcel> ReporteVentasMayoristasExcel([FromBody] ReporteRequestReporteVentasModel item)
        {
            ReporteResponseReporteVentas responseD = new ReporteResponseReporteVentas();
            ReporteResponseReporteVentasExcel responseD1 = new ReporteResponseReporteVentasExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {

                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Reportes/reporteVentasMayoristas", data, HttpContext.Session.GetString("token"));
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ReporteResponseReporteVentas>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de ventas mayoristas generado exitosamente";
                        responseD1.respuestaLista = responseD.respuesta;

                        workbook.Workbook.Worksheets.Add("Reporte de ventas mayoristas");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Fecha Hora";
                        hojaActual.Cells["B1"].Value = "MTCID";
                        hojaActual.Cells["C1"].Value = "Establecimiento";
                        hojaActual.Cells["D1"].Value = "Cajero";
                        hojaActual.Cells["E1"].Value = "Clasificación";
                        hojaActual.Cells["F1"].Value = "Producto";
                        hojaActual.Cells["G1"].Value = "Proveedor";
                        hojaActual.Cells["H1"].Value = "Referencia";
                        hojaActual.Cells["I1"].Value = "Ref2/Afiliación";
                        hojaActual.Cells["J1"].Value = "Entradas";
                        hojaActual.Cells["K1"].Value = "Salidas";
                        hojaActual.Cells["L1"].Value = "Comisión TAE";
                        hojaActual.Cells["M1"].Value = "Comisión Establecimiento";
                        hojaActual.Cells["N1"].Value = "No. Autorización";
                        hojaActual.Cells["O1"].Value = "Tipo Terminal";
                        hojaActual.Cells["P1"].Value = "Estatus";
                        hojaActual.Cells["A1:P1"].Style.Font.Bold = true;
                        int linea = 2;
                        foreach (ReporteReporteVentasModel carteraNew in responseD.respuesta)
                        {
                            long bandera = 0;
                            hojaActual.Cells["A" + linea].Value = carteraNew.fechaHora.ToString();
                            hojaActual.Cells["B" + linea].Value = int.Parse(carteraNew.MTCid.ToString());
                            hojaActual.Cells["C" + linea].Value = carteraNew.tienda.ToString();
                            hojaActual.Cells["D" + linea].Value = carteraNew.cajero.ToString();
                            if (carteraNew.clasificacion == null)
                                hojaActual.Cells["E" + linea].Value = carteraNew.clasificacion;
                            else
                                hojaActual.Cells["E" + linea].Value = carteraNew.clasificacion.ToString();
                            hojaActual.Cells["F" + linea].Value = carteraNew.producto.ToString();
                            hojaActual.Cells["G" + linea].Value = carteraNew.proveedor.ToString();
                            if (long.TryParse(carteraNew.referencia1.ToString(), out bandera))
                                hojaActual.Cells["H" + linea].Value = long.Parse(carteraNew.referencia1.ToString());
                            else
                                hojaActual.Cells["H" + linea].Value = carteraNew.referencia1.ToString();
                            hojaActual.Cells["I" + linea].Value = carteraNew.referencia2.ToString();
                            hojaActual.Cells["J" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.entrada.ToString()), 2);
                            hojaActual.Cells["K" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.salida.ToString()), 2);
                            hojaActual.Cells["L" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.comisionRecarga.ToString()), 2);
                            hojaActual.Cells["M" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.comisionEstablecimiento.ToString()), 2);
                            hojaActual.Cells["J" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["K" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["L" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["M" + linea].Style.Numberformat.Format = "$#,##0.00";
                            if (carteraNew.autorizacion != null && carteraNew.autorizacion != "")
                                hojaActual.Cells["N" + linea].Value = decimal.Parse(carteraNew.autorizacion.ToString(), System.Globalization.NumberStyles.Float);
                            else
                                hojaActual.Cells["N" + linea].Value = carteraNew.autorizacion;
                            hojaActual.Cells["O" + linea].Value = carteraNew.tipoTerminal.ToString();
                            hojaActual.Cells["P" + linea].Value = carteraNew.estatusCancelada.ToString();

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 16];
                        var table = hojaActual.Tables.Add(range, "table");
                        //table.ShowTotal = true;
                        table.TableStyle = TableStyles.Medium13;
                        hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                        // identificador para su guardado
                        string base64Excel = string.Empty;

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.Position = 0;
                            byte[] excelData = memoryStream.ToArray();

                            base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                            memoryStream.Dispose();

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte de Ventas mayoristas.xlsx";

                    }
                    else
                    {
                        responseD.MensajeRespuesta = "Error al generar reporte";
                    }

                    responseD1.MensajeRespuesta = responseD.MensajeRespuesta;

                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        #endregion

        #region Reporte Credito MO


        [Route("reporteCreditoMO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_28")]
        public async Task<ReportesCreditoResponseModelFile> reporteCreditoMO([FromBody] ReportesCreditoRequestModel data)
        {
            ReportesCreditoResponseModelFile responseD = new ReportesCreditoResponseModelFile();
            try
            {
                if (data.tipoReporte == 6)
                    data.fechaInicio = DateTime.Parse(data.fechaInicio).ToString("d/M/yyyy");
                else
                    data.fechaInicio = DateTime.Parse(data.fechaInicio).ToString("dd/MM/yyyy");

                string item = Seguridad.Encrypt(JsonConvert.SerializeObject(data));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPostAWS("getDocumentBucket", item, "");
                await Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;
                ReportesCreditoResponseModel respuesta = JsonConvert.DeserializeObject<ReportesCreditoResponseModel>(response);
                ///Aqui se tiene que crear un zip
                /// 

                // identificador para su guardado
                string base64Excel = string.Empty;
                if (respuesta != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream(respuesta.respuesta.FileContents))
                    {
                        memoryStream.Position = 0;
                        byte[] excelData = memoryStream.ToArray();

                        base64Excel = "data:text/plain;base64," + Convert.ToBase64String(excelData);
                        memoryStream.Dispose();

                    }

                    responseD.respuesta = base64Excel;
                    responseD.reporteNombre = respuesta.respuesta.fileName;
                }
                else
                {
                    responseD.CodigoRespuesta = -1;
                    responseD.MensajeRespuesta = "No se encontró el archivo";
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }


        [Route("subirDocumentosPM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06,PERM_06_28")]
        public async Task<ResponseModel> subirDocumentosPM([FromBody] ReportesDocumentosPMRequestModel data)
        {
            DataTable dt = new DataTable();
            System.IO.StreamReader reader = null;
            UploadFilesRequestModel uploadFiles = new UploadFilesRequestModel();
            ResponseModel response = new ResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(data.archivo))
                {
                    if (data.ext == "csv" || data.ext == "html")
                    {
                        var contents = new StreamContent(new MemoryStream(Convert.FromBase64String(data.archivo)));
                        uploadFiles.clave = "U=v6XgYS";
                        uploadFiles.tipoDocumento = data.tipoDocumento;
                        uploadFiles.idCadena = data.idCadena;
                        uploadFiles.archivo = data.archivo;
                        uploadFiles.filesBytes = contents.ReadAsByteArrayAsync().Result;

                        string item = JsonConvert.SerializeObject(uploadFiles);

                        Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                        Task<string> taskResponse = request.doPostAWS("uploadFilesPM", item, "");
                        await Task.WhenAll(taskLog, taskResponse);
                        string respuesta = taskResponse.Result;
                        response = JsonConvert.DeserializeObject<ResponseModel>(respuesta);

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }
        #endregion

    }
}