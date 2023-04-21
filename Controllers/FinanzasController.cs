using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MTC.WebApp.BackOffice.Models;
using Newtonsoft.Json.Linq;
using MTC.WebApp.BackOffice.Helpers;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Xml.XPath;
using System.Xml.Xsl;
using OfficeOpenXml.FormulaParsing.Utilities;
using Microsoft.Extensions.Hosting;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Finanzas")]
    public class FinanzasController : Controller
    {
        private readonly AppSettings __appSettings;
        private RequestHelper request; 
        private readonly IHostEnvironment _env;
        private Utils util;
        public FinanzasController(IOptions<AppSettings> appSetings, IHostEnvironment _env)
        {
            this.__appSettings = appSetings.Value;
            this.request = new RequestHelper(this.__appSettings);
            this.util = new Utils();
            this._env = _env;
        }

        [HttpPost]
        [Route("BuscarCFDI")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_01")]
        public FinanzasBCFDIResponse BuscarCFDI([FromBody]FinanzasRequestBCFDIModel busqueda)
        {
            FinanzasBCFDIResponse responseD = new FinanzasBCFDIResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    if (busqueda.fechaDesde == null || busqueda.fechaHasta == null || busqueda.tipoComprobante == "")
                    {
                        responseD.CodigoRespuesta = -7;
                        responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";
                        return responseD;
                    }

                    int dif = (busqueda.fechaHasta.Month - busqueda.fechaDesde.Month) + 12 * (busqueda.fechaHasta.Year - busqueda.fechaDesde.Year);
                    if (dif > 3)
                    {
                        responseD.CodigoRespuesta = -8;
                        responseD.MensajeRespuesta = "Fechas Invalidas, las fechas no deben ser mayores a 3 meses.";
                        return responseD;
                    }

               

                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/BuscarCFDI", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasBCFDIResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            throw new System.ArgumentException("Parametros Invalidos: ", responseD.ToString());
                        }

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


        [HttpPost]
        [Route("BuscarCFDIExcel")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_01")]
        public FinanzasBCFDIResponseExcel BuscarCFDIExcel([FromBody]FinanzasRequestBCFDIModel busqueda)
        {
            //FinanzasBCFDIResponse responseD = new FinanzasBCFDIResponse();
            FinanzasBCFDIResponseExcel responseDExcel = new FinanzasBCFDIResponseExcel();
            try
            {
                if (ModelState.IsValid)
                {

                    if (busqueda.fechaDesde == null || busqueda.fechaHasta == null || busqueda.tipoComprobante == "")
                    {
                        responseDExcel.CodigoRespuesta = -7;
                        responseDExcel.MensajeRespuesta = "Ingrese todos los campos necesarios";
                        return responseDExcel;
                    }

                    int dif = (busqueda.fechaHasta.Month - busqueda.fechaDesde.Month) + 12 * (busqueda.fechaHasta.Year - busqueda.fechaDesde.Year);
                    if (dif > 3)
                    {
                        responseDExcel.CodigoRespuesta = -8;
                        responseDExcel.MensajeRespuesta = "Fechas Invalidas, las fechas no deben ser mayores a 3 meses.";
                        return responseDExcel;
                    }

                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/BuscarCFDI", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseDExcel = JsonConvert.DeserializeObject<FinanzasBCFDIResponseExcel>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseDExcel);
                        if (responseDExcel.CodigoRespuesta < 0)
                        {
                            throw new System.ArgumentException("Parametros Invalidos: ", responseDExcel.ToString());
                        }

                        responseDExcel.MensajeRespuesta = "Reporte CFDI";

                        ExcelPackage workbook = new ExcelPackage();
                        workbook.Workbook.Worksheets.Add("Reporte CFDI");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "ID.";
                        hojaActual.Cells["B1"].Value = "Lote BE";
                        hojaActual.Cells["C1"].Value = "Fecha Emisión";
                        hojaActual.Cells["D1"].Value = "RFC";
                        hojaActual.Cells["E1"].Value = "Razón Social";
                        hojaActual.Cells["F1"].Value = "SubTotal";
                        hojaActual.Cells["G1"].Value = "IVA";
                        hojaActual.Cells["H1"].Value = "Total";
                        hojaActual.Cells["I1"].Value = "Tipo Comprobante";
                        hojaActual.Cells["J1"].Value = "Folio Serie";
                        hojaActual.Cells["K1"].Value = "Tasa IVA";
                        hojaActual.Cells["L1"].Value = "UUID";
                        hojaActual.Cells["M1"].Value = "Descripción";
                        hojaActual.Cells["N1"].Value = "Observaciones Generales";
                        hojaActual.Cells["O1"].Value = "Estatus";
                        hojaActual.Cells["P1"].Value = "Metodo de Pago";
                        hojaActual.Cells["Q1"].Value = "DocumentoRelacionado";

                        hojaActual.Cells["A1:Q1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (FinanzasBCFDIModel cfdi in responseDExcel.finanzas)
                        {
                            
                            hojaActual.Cells["A" + linea].Value = cfdi.id.ToString();
                            hojaActual.Cells["B" + linea].Value = cfdi.idLoteBE.ToString();
                            hojaActual.Cells["C" + linea].Value = cfdi.fechaEmision.ToString();
                            hojaActual.Cells["D" + linea].Value = cfdi.RFC;
                            hojaActual.Cells["E" + linea].Value = cfdi.razonSocial;
                            hojaActual.Cells["G" + linea].Value = cfdi.iva;
                            hojaActual.Cells["H" + linea].Value = cfdi.total;
                            hojaActual.Cells["I" + linea].Value = cfdi.tipoComprobante;
                            hojaActual.Cells["J" + linea].Value = cfdi.folio_serie;
                            hojaActual.Cells["K" + linea].Value = cfdi.tasaIVA;
                            hojaActual.Cells["L" + linea].Value = cfdi.uuid;
                            hojaActual.Cells["M" + linea].Value = cfdi.concepto;
                            hojaActual.Cells["N" + linea].Value = cfdi.observacionesGrales;
                            hojaActual.Cells["O" + linea].Value = cfdi.cancelado;
                            hojaActual.Cells["P" + linea].Value = cfdi.MetodoDePago;
                            hojaActual.Cells["Q" + linea].Value = cfdi.documentoRelacionado;

                            linea++;
                        }

                        var range = hojaActual.Cells[1, 1, linea - 1, 17];
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

                        }
                        responseDExcel.respuesta = base64Excel;
                        responseDExcel.reporteNombre = "Reporte CFDI.xlsx";
                    }
                }
                else
                {
                    responseDExcel.CodigoRespuesta = -6;
                    responseDExcel.MensajeRespuesta = "Ingrese todos los campos necesarios";
                    return responseDExcel;
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return responseDExcel;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("BuscarCFDIGenerico")]
        [MTCAuthorizationHandler("PERM_03_01_02")]
        public FinanzasBCFDIGResponseModel BuscarCFDIGenerico([FromBody]FinanzasRequestBCFDIGModel busqueda)
        {
            FinanzasBCFDIGResponseModel responseD = new FinanzasBCFDIGResponseModel();
            try
            {
                if (ModelState.IsValid)
                {

                    if (busqueda.fechaDesde == null || busqueda.fechaHasta == null )
                    {
                        responseD.CodigoRespuesta = -7;
                        responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";
                        return responseD;
                    }

                    int dif = (busqueda.fechaHasta.Month - busqueda.fechaDesde.Month) + 12 * (busqueda.fechaHasta.Year - busqueda.fechaDesde.Year);
                    if (dif > 3)
                    {
                        responseD.CodigoRespuesta = -8;
                        responseD.MensajeRespuesta = "Fechas Invalidas, las fechas no deben ser mayores a 3 meses.";
                        return responseD;
                    }
                    busqueda.fechaHasta = busqueda.fechaHasta.AddHours(18);
                    busqueda.fechaDesde = busqueda.fechaDesde.AddHours(-5);

                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/BuscarCFDIG", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasBCFDIGResponseModel>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            throw new System.ArgumentException("Parametros Invalidos: ", responseD.ToString());
                        }

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CancelarFactura")]
        [MTCAuthorizationHandler("PERM_03_01_02")]
        public FinanzasCancelarFacturaResponse cancelarFactura([FromBody]FinanzasRequestCancelarFactura busqueda)
        {
            FinanzasCancelarFacturaResponse responseD = new FinanzasCancelarFacturaResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/CancelarFactura", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCancelarFacturaResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta == 0 && responseD.respuesta)
                        {
                            responseD.MensajeRespuesta = "Factura enviada  a pendiente por cancelar";
                        }
                        else if (responseD.CodigoRespuesta < 0){
                            responseD.MensajeRespuesta = "Error al agregar factura a pendiente [01]";
                        }
                        else
                        {
                            responseD.MensajeRespuesta = "Error al agregar factura a pendiente";
                        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Generarcomplemento")]
        [MTCAuthorizationHandler("PERM_03_01_02")]
        public FinanzasCancelarFacturaResponse Generarcomplemento([FromBody]FinanzasRequestCancelarFactura busqueda)
        {
            FinanzasCancelarFacturaResponse responseD = new FinanzasCancelarFacturaResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    busqueda.Ip = Seguridad.GetIPAddress();
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/Generarcomplemento", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCancelarFacturaResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta == 0 && responseD.respuesta)
                        {
                            responseD.MensajeRespuesta = "Complemento generado exitosamente";
                        }
                        else if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al generar el complemento [01]";
                        }
                        else
                        {
                            responseD.MensajeRespuesta = "Error al generar el complemento";
                        }

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


        [HttpPost]
        [Route("MostrarDoc")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_02")]
        public FinanzastMostarXSLSDocDResponse MostrarDoc([FromBody]FinanzasRequestMostarXSLSDocModel busqueda)
        {
            FinanzastMostarXSLSDocDResponse responseD = new FinanzastMostarXSLSDocDResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/MostrarFacturaXSLS", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzastMostarXSLSDocDResponse>(response);
               
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta == 0)
                        {
                            responseD.MensajeRespuesta = "Factura cargada correctamente";
                        }
                        else if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al cargar Factura";
                        }

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -1;
                        responseD.MensajeRespuesta = "Error al cargar Factura";
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIPrevisualizar")]
        [MTCAuthorizationHandler("PERM_03_01_02")]
        public FinanzastCFDIPrevizualisarDResponse CFDIPrevisualizar([FromBody]FinanzasRequestCFDIPrevisualizarModel busqueda)
        {
            FinanzastCFDIPrevizualisarDResponse responseD = new FinanzastCFDIPrevizualisarDResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIPrevisualizar", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzastCFDIPrevizualisarDResponse>(response);
                        /*util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Facturación",
                            Actividad = "Finanzas/CFDIPrevisualizar",
                            Detalles = "Visualización Factura",
                            Objeto = JsonConvert.SerializeObject(responseD)
                        }, __appSettings.LoggerFacturacion);*/
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta == 0)
                        {
                            responseD.MensajeRespuesta = "Factura cargada correctamente";
                        }
                        else if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al cargar Factura";
                        }

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -1;
                        responseD.MensajeRespuesta = "Error al cargar Factura";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIGenericoFacturaGenerar")]
        [MTCAuthorizationHandler("PERM_03_01_02")]
        public FinanzastCFDIGenericoFacturaResponse CFDIGenericoFacturaGenerar([FromBody]FinanzasRequestCFDIGenericoFacturaModel busqueda)
        {
            FinanzastCFDIGenericoFacturaResponse responseD = new FinanzastCFDIGenericoFacturaResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));

                    if (busqueda.documento.id == 2 )
                    {
                        string auxObservaciones = string.Empty;
                        auxObservaciones = busqueda.uuid.Trim() + "~" + busqueda.obervaciones.Trim();
                        busqueda.obervaciones = auxObservaciones;
                    }

                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIGenericoFacturaGenerar", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzastCFDIGenericoFacturaResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -1;
                        responseD.MensajeRespuesta = "Error al cargar Factura";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIRefacturar")]
        [MTCAuthorizationHandler("PERM_03_01_02")]
        public ResponseModel CFDIRefacturar([FromBody]FinanzasRequestRefacturarModel busqueda)
        {
            ResponseModel responseD = new FinanzastCFDIGenericoFacturaResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIGetFacturaProductos", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        FinanzasResponseCFDIGenericoFacturaModel responseD1 = new FinanzasResponseCFDIGenericoFacturaModel();
                        responseD1 = JsonConvert.DeserializeObject<FinanzasResponseCFDIGenericoFacturaModel>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD1);

                        if (responseD1.CodigoRespuesta == 0)
                        {
                            responseD1.respuesta.cliente.id = busqueda.idCliente;

                            FinanzastCFDIGenericoFacturaResponse responseD2 = this.CFDIGenericoFacturaGenerar(responseD1.respuesta);

                            if (responseD2.CodigoRespuesta== 0)
                            {
                                FinanzasCancelarFacturaResponse responseD3 = this.cancelarFactura(new FinanzasRequestCancelarFactura { id = busqueda.idFactura, idUsuario = busqueda.idUsuario });
                                if (responseD3.CodigoRespuesta == 0)
                                {
                                    responseD.CodigoRespuesta = 0;
                                    responseD.MensajeRespuesta = "Refacturación generada exitosamente";
                                }
                                else
                                {
                                    responseD.CodigoRespuesta = -4;
                                    responseD.MensajeRespuesta = "Error al cancelar la factura";
                                }
                            }
                            else
                            {
                                responseD.CodigoRespuesta = -3;
                                responseD.MensajeRespuesta = "Error al Generar Factura";
                            }

                        }
                        else
                        {
                            responseD.CodigoRespuesta = -2;
                            responseD.MensajeRespuesta = "Error al cargar Factura [01]";
                        }

                    }
                    else
                    {
                        responseD.CodigoRespuesta = -1;
                        responseD.MensajeRespuesta = "Error al cargar Factura";
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




        [HttpPost]
        [Route("getDocsFactura")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_02")]
        public FinanzasRDocsResponseModel getTipoDocumento()
        {
            //Obtener datos de la Factura
            FinanzasRDocsResponseModel responseGlobal = new FinanzasRDocsResponseModel();
            try
            {

                Task<string> taskResponseTD = request.doPost("Finanzas/getTipoDocumento", null, HttpContext.Session.GetString("token"));
                Task<string> taskResponseSU = request.doPost("Finanzas/getSucursalesLabel", null, HttpContext.Session.GetString("token"));
                Task<string> taskResponseUCFDI = request.doPost("Finanzas/getUsoCFDI", null, HttpContext.Session.GetString("token"));
                Task<string> taskResponseGMP = request.doPost("Finanzas/getMetodosPago", null, HttpContext.Session.GetString("token"));
                Task<string> taskResponseGFP = request.doPost("Finanzas/getFormasPago", null, HttpContext.Session.GetString("token"));
                Task<string> taskResponseGCC = request.doPost("Finanzas/getCatalogoClientes", Seguridad.Encrypt(JsonConvert.SerializeObject(new FinanzasRequestCClientesModel())), HttpContext.Session.GetString("token"));
                Task<string> taskResponseGCP = request.doPost("Finanzas/getCatalogoProductos", null, HttpContext.Session.GetString("token"));
                Task<string> taskResponseGCCU = request.doPost("Finanzas/getCatalogoClaveUnidad", null, HttpContext.Session.GetString("token"));
                Task<string> taskResponseGCU = request.doPost("Finanzas/getCatalogoUnidades", null, HttpContext.Session.GetString("token"));
                Task<string> taskResponseGCCPS = request.doPost("Finanzas/getCatalogoClaveProdServ", null, HttpContext.Session.GetString("token"));
       
                

                Task.WhenAll(taskResponseTD, taskResponseSU, taskResponseUCFDI, taskResponseGMP, taskResponseGFP, taskResponseGCC, taskResponseGCP, taskResponseGCCU, taskResponseGCU, taskResponseGCCPS);

                FinanzasGTDResponseModel responseTD = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseTD.Result);
                if (responseTD.CodigoRespuesta == 0)
                {
                    responseGlobal.objTiposDocumento = responseTD.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -1;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Tipo Documento)";
                }

                FinanzasGTDResponseModel responseSU = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseSU.Result);
                if (responseSU.CodigoRespuesta == 0)
                {
                    responseGlobal.objSucursales = responseSU.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -2;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Sucursales)";
                }

                FinanzasGTDResponseModel responseUCFDI = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseUCFDI.Result);
                if (responseUCFDI.CodigoRespuesta == 0)
                {
                    responseGlobal.objUsoCFDI = responseUCFDI.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -3;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Uso CFDI)";
                }

                FinanzasGTDResponseModel responseGMP = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseGMP.Result);
                if (responseGMP.CodigoRespuesta == 0)
                {
                    responseGlobal.objMetodosPago = responseGMP.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -4;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Metodos de Pago)";
                }

                FinanzasGTDResponseModel responseGFP = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseGFP.Result);
                if (responseGMP.CodigoRespuesta == 0)
                {
                    responseGlobal.objFormasPago = responseGFP.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -5;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Formas de Pago)";
                }


                FinanzasGTDResponseModel responseCC = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseGCC.Result);
                if (responseCC.CodigoRespuesta == 0)
                {
                    responseGlobal.objClientes = responseCC.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -6;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Clientes)";
                }


                FinanzasGTDResponseModel responseCP = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseGCP.Result);
                if (responseCC.CodigoRespuesta == 0)
                {
                    responseGlobal.objProductos = responseCP.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -7;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Productos)";
                }


                FinanzasGTDResponseModel responseCCU = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseGCCU.Result);
                if (responseCC.CodigoRespuesta == 0)
                {
                    responseGlobal.objClaveUnidad = responseCCU.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -8;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Clave Unidad)";
                }

                FinanzasGTDResponseModel responseCU = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseGCU.Result);
                if (responseCC.CodigoRespuesta == 0)
                {
                    responseGlobal.objUnidadades = responseCU.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -9;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Unidades)";
                }

                FinanzasGTDResponseModel responseCCPS = JsonConvert.DeserializeObject<FinanzasGTDResponseModel>(taskResponseGCCPS.Result);
                if (responseCC.CodigoRespuesta == 0)
                {
                    responseGlobal.objCCProdServ = responseCCPS.lista;
                }
                else
                {
                    responseGlobal.CodigoRespuesta = -10;
                    responseGlobal.MensajeRespuesta = "Catalogo Vacio (Clave Producto Servicio)";
                }



            }
            catch (Exception ex)
            {
                responseGlobal.CodigoRespuesta = -100;
                responseGlobal.MensajeRespuesta = "Problemas Con la carga Masiva de datos";
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return responseGlobal;
        }


        #region CFDI Montos

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("BuscarCFDICadena")]
        [MTCAuthorizationHandler("PERM_03_01_03")]
        public FinanzasCadenaResponse CFDICadenaBuscar([FromBody]FinanzasRequestCFDICadenaMontos busqueda)
        {
            FinanzasCadenaResponse responseD = new FinanzasCadenaResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/BuscarCadena", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCadenaResponse>(response);
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


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("BuscarCFDIMontos")]
        [MTCAuthorizationHandler("PERM_03_01_03")]
        public FinanzasBCFDIMontosResponse CFDIMontosBuscar()
        {
            FinanzasBCFDIMontosResponse responseD = new FinanzasBCFDIMontosResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    Task<string> taskResponse = request.doPost("Finanzas/getCFDIMontos", null, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasBCFDIMontosResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al cargar datos";
                        }
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIMontosAgregar")]
        [MTCAuthorizationHandler("PERM_03_01_03")]
        public FinanzasCFDIMontosAddResponse CFDIMontosAgregar([FromBody]FinanzasRequestCFDIMontosAdd busqueda)
        {
            FinanzasCFDIMontosAddResponse responseD = new FinanzasCFDIMontosAddResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIMontosAgregar", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIMontosAddResponse>(response);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIMontosEditar")]
        [MTCAuthorizationHandler("PERM_03_01_03")]
        public FinanzasCFDIMontosEditResponse CFDIMontosEditar([FromBody]FinanzasRequestCFDIMontosEdit busqueda)
        {
            FinanzasCFDIMontosEditResponse responseD = new FinanzasCFDIMontosEditResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIMontosEditar", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIMontosEditResponse>(response);
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIMontosEliminar")]
        [MTCAuthorizationHandler("PERM_03_01_03")]
        public FinanzasCFDIMontosDeleteResponse CFDIMontosEliminar([FromBody]FinanzasRequestCFDIMontosDelete busqueda)
        {
            FinanzasCFDIMontosDeleteResponse responseD = new FinanzasCFDIMontosDeleteResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIMontosEliminar", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIMontosDeleteResponse>(response);
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

        #region Clientes
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("BuscarCFDIClientes")]
        [MTCAuthorizationHandler("PERM_03_01_04")]
        public FinanzasBCFDIBClientesResponse BuscarCFDIClientes([FromBody]FinanzasRequestCFDIBClientes busqueda)
        {
            FinanzasBCFDIBClientesResponse responseD = new FinanzasBCFDIBClientesResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<string> taskResponse = request.doPost("Finanzas/BuscarCFDIClientes", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasBCFDIBClientesResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al agregar cargar datos";
                        }
                    }
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
            }
            return responseD;
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("GetCatalogoEstados")]
        [MTCAuthorizationHandler("PERM_03_01_04")]
        public FinanzasCFDICEstadosResponse GetCatalogoEstados()
        {
            FinanzasCFDICEstadosResponse responseD = new FinanzasCFDICEstadosResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    Task<string> taskResponse = request.doPost("Finanzas/GetCatalogoEstados", null, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDICEstadosResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al cargar datos";
                        }
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("GetCatalogoCiudades")]
        [MTCAuthorizationHandler("PERM_03_01_04")]
        public FinanzasCFDICiudadesResponse GetCatalogoCiudades([FromBody]FinanzasRequestCFDIBCiudades busqueda)
        {
            FinanzasCFDICiudadesResponse responseD = new FinanzasCFDICiudadesResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<string> taskResponse = request.doPost("Finanzas/GetCatalogoCiudades", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDICiudadesResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al cargar datos";
                        }
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


        [HttpPost]
        [Route("CFDIClienteAdd")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_04")]
        public FinanzasCFDIClienteAddResponse CFDIClienteAdd([FromBody]FinanzasRequestCFDIClientesAdd busqueda)
        {
            FinanzasCFDIClienteAddResponse responseD = new FinanzasCFDIClienteAddResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (Seguridad.ValidarRFC(busqueda.rfc))
                    {
                        busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                        string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                        Task<string> taskResponse = request.doPost("Finanzas/CFDIClienteAdd", postData, HttpContext.Session.GetString("token"));
                        Task.WhenAll(taskResponse);
                        string response = taskResponse.Result;
                        if (request.codigoRespuesta == 0)
                        {
                            responseD = JsonConvert.DeserializeObject<FinanzasCFDIClienteAddResponse>(response);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                  
                        }
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -7;
                        responseD.MensajeRespuesta = "RFC Inavalido";
                        return responseD;
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


        [HttpPost]
        [Route("CFDIClienteEdit")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_04")]
        public FinanzasCFDIClienteEditResponse CFDIClienteEdit([FromBody]FinanzasRequestCFDIClientesEdit busqueda)
        {
            FinanzasCFDIClienteEditResponse responseD = new FinanzasCFDIClienteEditResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (Seguridad.ValidarRFC(busqueda.rfc))
                    {
                        busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                        string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                        Task<string> taskResponse = request.doPost("Finanzas/CFDIClienteEdit", postData, HttpContext.Session.GetString("token"));
                        Task.WhenAll(taskResponse);
                        string response = taskResponse.Result;
                        if (request.codigoRespuesta == 0)
                        {
                            responseD = JsonConvert.DeserializeObject<FinanzasCFDIClienteEditResponse>(response);
                            Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                        }
                    }
                    else
                    {
                        responseD.CodigoRespuesta = -7;
                        responseD.MensajeRespuesta = "RFC Inavalido";
                        return responseD;
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




        [HttpPost]
        [Route("CFDIClienteDel")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_04")]
        public FinanzasCFDIClienteDellResponse CFDIClienteDel([FromBody]FinanzasRequestCFDIClientesDell busqueda)
        {
            FinanzasCFDIClienteDellResponse responseD = new FinanzasCFDIClienteDellResponse();
            try
            {
                if (ModelState.IsValid)
                {
       
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIClienteDel", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIClienteDellResponse>(response);
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

        #region Productos


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("BuscarCFDIProductos")]
        [MTCAuthorizationHandler("PERM_03_01_05")]
        public FinanzasCFDIProductosResponse BuscarCFDIProductos()
        {
            FinanzasCFDIProductosResponse responseD = new FinanzasCFDIProductosResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    Task<string> taskResponse = request.doPost("Finanzas/getCatalogoProductos", null, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIProductosResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al cargar datos";
                        }
                    }
                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Estado invalido";
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return responseD;
        }


        [HttpPost]
        [Route("CFDIProductoDel")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_05")]
        public FinanzasCFDIProductoDellResponse CFDIProductoDel([FromBody]FinanzasRequestCFDIProductoDell busqueda)
        {
            FinanzasCFDIProductoDellResponse responseD = new FinanzasCFDIProductoDellResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIProductoDel", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIProductoDellResponse>(response);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("BuscarCFDIUnidades")]
        [MTCAuthorizationHandler("PERM_03_01_05")]
        public FinanzasCFDIUnidadesResponse BuscarCFDIUnidades()
        {
            FinanzasCFDIUnidadesResponse responseD = new FinanzasCFDIUnidadesResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    Task<string> taskResponse = request.doPost("Finanzas/getCatalogoUnidades", null, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIUnidadesResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al cargar datos";
                        }
                    }
                }
                else
                {
                    responseD.CodigoRespuesta = -6;
                    responseD.MensajeRespuesta = "Estado invalido";
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return responseD;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIProductoAdd")]
        [MTCAuthorizationHandler("PERM_03_01_05")]
        public FinanzasCFDIProductoAddResponse CFDIProductoAdd([FromBody]FinanzasRequestCFDIProductoAdd busqueda)
        {
            FinanzasCFDIProductoAddResponse responseD = new FinanzasCFDIProductoAddResponse();
            try
            {
                if (ModelState.IsValid)
                {
              
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIProductoAdd", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIProductoAddResponse>(response);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIProductoEdit")]
        [MTCAuthorizationHandler("PERM_03_01_05")]
        public FinanzasCFDIProductoEditResponse CFDIProductoEdit([FromBody]FinanzasRequestCFDIProductoEdit busqueda)
        {
            FinanzasCFDIProductoEditResponse responseD = new FinanzasCFDIProductoEditResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIProductoEdit", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIProductoEditResponse>(response);
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

        #region Cancelar CFDI


        //Cancelar CFDI (Listado)
        [HttpPost]
        [Route("CancelarCFDI")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_06")]
        public FinanzasCancelarCFDIResponseModel CancelarCFDI([FromBody]FinanzasRequestCancelarCFDIModel busqueda)
        {
            FinanzasCancelarCFDIResponseModel responseD = new FinanzasCancelarCFDIResponseModel();
            try
            {
                if (ModelState.IsValid)
                {

                    if (busqueda.fechaDesde == null || busqueda.fechaHasta == null)
                    {
                        responseD.CodigoRespuesta = -7;
                        responseD.MensajeRespuesta = "Ingrese todos los campos necesarios";
                        return responseD;
                    }

                    int dif = (busqueda.fechaHasta.Month - busqueda.fechaDesde.Month) + 12 * (busqueda.fechaHasta.Year - busqueda.fechaDesde.Year);
                    if (dif > 3)
                    {
                        responseD.CodigoRespuesta = -8;
                        responseD.MensajeRespuesta = "Fechas Invalidas, las fechas no deben ser mayores a 3 meses.";
                        return responseD;
                    }
                    busqueda.Ip = Seguridad.GetIPAddress();
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    busqueda.idRol = Int32.Parse(HttpContext.Session.GetString("rol"));

                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Finanzas/CancelarCFDI", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCancelarCFDIResponseModel>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta < 0)
                        {
                            throw new System.ArgumentException("Parametros Invalidos: ", responseD.ToString());
                        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDIBorrarFacturaPPC")]
        [MTCAuthorizationHandler("PERM_03_01_06")]
        public FinanzasCFDIBorrarFacturaPPCResponse CFDIBorrarFacturaPPC([FromBody]FinanzasRequestCFDIBorrarFacturaPPC busqueda)
        {
            FinanzasCFDIBorrarFacturaPPCResponse responseD = new FinanzasCFDIBorrarFacturaPPCResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<string> taskResponse = request.doPost("Finanzas/CFDIBorrarFacturaPPC", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDIBorrarFacturaPPCResponse>(response);
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CFDICancelarFactura")]
        [MTCAuthorizationHandler("PERM_03_01_06")]
        public FinanzasCFDICancelarFacturaResponse CFDICancelarFactura([FromBody]FinanzasRequestCFDICancelarFactura busqueda)
        {
            FinanzasCFDICancelarFacturaResponse responseD = new FinanzasCFDICancelarFacturaResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<string> taskResponse = request.doPost("Finanzas/CFDICancelarFactura", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<FinanzasCFDICancelarFacturaResponse>(response);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("getMotivosCFDI")]
        [MTCAuthorizationHandler("PERM_03_01_06")]
        public MotivosCFDIResponseModel getMotivosCFDI()
        {
            MotivosCFDIResponseModel responseD = new MotivosCFDIResponseModel();
            try
            {
                Task<string> taskResponse = request.doPost("Finanzas/getMotivosCFDI", "", HttpContext.Session.GetString("token"));
                Task.WhenAll(taskResponse);
                string response = taskResponse.Result;
                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<MotivosCFDIResponseModel>(response);
                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return responseD;
        }


        #endregion

        #region Facturación POS
        [Route("FacturacionPOS/BuscarCadenaCFDI")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_08")]
        public FinanzasBuscarCadenaCFDIResponse BuscarCadenaCFDI(FinanzasRequestBuscarCadenaCFDIModel item)
        {
            FinanzasBuscarCadenaCFDIResponse responseD = new FinanzasBuscarCadenaCFDIResponse();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Finanzas/FacturacionPOS/BuscarCadenaCFDI", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<FinanzasBuscarCadenaCFDIResponse>(response);
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

        #region Subir Facturas

        [HttpPost]
        [Route("SubirFacturaSAT")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_07")]
        public FinanzasResponseSubirFacturas subirFacturaSAT([FromBody] FinanzasRequestSubirFacturas item)
        {
            
            FinanzasResponseSubirFacturas response = new FinanzasResponseSubirFacturas();
            response.insertado = false;
            try
            {
                if (!string.IsNullOrEmpty(item.archivo))
                {
                    if (item.ext == "zip")
                    {
                        System.IO.File.WriteAllBytes(@""+__appSettings.FacturasSatDir+item.nombre +"." +item.ext, Convert.FromBase64String(item.archivo));
                        response.insertado = true;
                        response.CodigoRespuesta = 0;
                        response.MensajeRespuesta = "El archivo "+item.nombre+" se guardo con exito";
                    }
                    else
                    {
                        response.CodigoRespuesta = -1;
                        response.MensajeRespuesta = "El tipo de archivo no es valido";
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Archivo inválido";
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = "No se pudo cargar la factura";
            }
            return response;
        }

        #endregion


        #region Visor Facturas

        [HttpPost]
        [Route("VisorFacturas/getComprobantes")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_08")]
        public async Task<FinanzasResponseTipoComprobantes> getTiposComprobantes()
        {

            FinanzasResponseTipoComprobantes response = new FinanzasResponseTipoComprobantes();
            try
            {
                string reqResponse = await request.doPost("Finanzas/VisorFacturas/getComprobantes", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<FinanzasResponseTipoComprobantes>(reqResponse);
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

        [HttpPost]
        [Route("VisorFacturas/getDocumento")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_08")]
        public async Task<FinanzasResponseDocumentoSAT> GetDocumentosSAT([FromBody] FinanzasRequestDocumentoSAT item)
        {

            FinanzasResponseDocumentoSAT response = new FinanzasResponseDocumentoSAT();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Finanzas/VisorFacturas/getDocumento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<FinanzasResponseDocumentoSAT>(reqResponse);
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
        
        [HttpPost]
        [Route("VisorFacturas/getDocumentoExcell")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_08")]
        public async Task<FinanzasResponseDocumentoSATExcel> GetDocumentosSATExcel([FromBody] FinanzasResponseDocumentoSAT item)
        {

            FinanzasResponseDocumentoSATExcel responseD1 = new FinanzasResponseDocumentoSATExcel();
            ExcelPackage workbook = new ExcelPackage();
            try
            {
                //string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                //Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                //Task<string> taskResponse = request.doPost("Finanzas/VisorFacturas/getDocumento", data, HttpContext.Session.GetString("token"));
                //await Task.WhenAll(taskLog, taskResponse);
                //string response = taskResponse.Result;


                Logger.DoLog(MethodBase.GetCurrentMethod(), item);

                item.MensajeRespuesta = "CFDIs SAT recibidas generado exitosamente";
                responseD1.listadoDocumentos = item.listadoDocumentos;

                workbook.Workbook.Worksheets.Add("CFDIs SAT recibidas");
                var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                hojaActual = workbook.Workbook.Worksheets[0];

                hojaActual.Cells["A1"].Value = "ID";
                hojaActual.Cells["B1"].Value = "LoteBE";
                hojaActual.Cells["C1"].Value = "Fecha Emisión";
                hojaActual.Cells["D1"].Value = "RFC";
                hojaActual.Cells["E1"].Value = "Razón Social";
                hojaActual.Cells["F1"].Value = "Sub Total";
                hojaActual.Cells["G1"].Value = "IVA";
                hojaActual.Cells["H1"].Value = "Total";
                hojaActual.Cells["I1"].Value = "Tipo Comprobante";
                hojaActual.Cells["J1"].Value = "Folio serie";
                hojaActual.Cells["K1"].Value = "UUID";
                hojaActual.Cells["L1"].Value = "Descripción";
                hojaActual.Cells["M1"].Value = "Tasa IVA";
                hojaActual.Cells["N1"].Value = "Observaciones Generales";
                hojaActual.Cells["O1"].Value = "Método Pago";
                hojaActual.Cells["P1"].Value = "Depósito ID";
                hojaActual.Cells["Q1"].Value = "Fecha de Depósito";
                hojaActual.Cells["R1"].Value = "Estatus";
                hojaActual.Cells["A1:R1"].Style.Font.Bold = true;
                int linea = 2;
                foreach (DocumentoSAT carteraNew in item.listadoDocumentos)
                {
                    double bandera = 0;

                    hojaActual.Cells["A" + linea].Value = int.Parse(carteraNew.id.ToString());
                    hojaActual.Cells["B" + linea].Value = carteraNew.idLoteBE.ToString();
                    hojaActual.Cells["C" + linea].Value = carteraNew.fechaEmision.ToString();
                    hojaActual.Cells["D" + linea].Value = carteraNew.rfc.ToString();
                    hojaActual.Cells["E" + linea].Value = carteraNew.razonSocial.ToString();
                    hojaActual.Cells["F" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.subTotal.ToString()), 2);
                    hojaActual.Cells["G" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.iva.ToString()), 2);
                    hojaActual.Cells["H" + linea].Value = Decimal.Round(Decimal.Parse(carteraNew.total.ToString()), 2);
                    hojaActual.Cells["I" + linea].Value = carteraNew.tipoComprobante.ToString();
                    hojaActual.Cells["J" + linea].Value = carteraNew.folioSerie.ToString();
                    hojaActual.Cells["K" + linea].Value = carteraNew.uuid.ToString();
                    hojaActual.Cells["L" + linea].Value = carteraNew.concepto.ToString();
                    hojaActual.Cells["M" + linea].Value = carteraNew.tasaIVA.ToString();

                    hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                    hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";
                    hojaActual.Cells["H" + linea].Style.Numberformat.Format = "$#,##0.00";

                    hojaActual.Cells["N" + linea].Value = carteraNew.observacionesGenerales.ToString();
                    hojaActual.Cells["O" + linea].Value = carteraNew.metodoPago.ToString();
                    hojaActual.Cells["P" + linea].Value = int.Parse(carteraNew.idDeposito.ToString());
                    hojaActual.Cells["Q" + linea].Value = carteraNew.fechaDeposito.ToString();
                    hojaActual.Cells["R" + linea].Value = carteraNew.cancelado.ToString();

                    linea++;
                }

                var range = hojaActual.Cells[1, 1, linea - 1, 18];
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
                responseD1.reporteNombre = "CFDIs SAT recibidas.xlsx";

                responseD1.MensajeRespuesta = item.MensajeRespuesta;
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                item.CodigoRespuesta = -9;
                item.MensajeRespuesta = ex.Message;
            }
            finally
            {
                workbook.Dispose();
            }
            return responseD1;
        }

        [HttpPost]
        [Route("VisorFacturas/cancelarFactura")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_08")]
        public async Task<FinanzasResponseOperacionFactura> cancelarFacturaPendiente([FromBody] FinanzasRequestOperacionFactura item)
        {

            FinanzasResponseOperacionFactura response = new FinanzasResponseOperacionFactura();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Finanzas/VisorFacturas/cancelarFacturaPendiente", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<FinanzasResponseOperacionFactura>(reqResponse);
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

        [HttpPost]
        [Route("VisorFacturas/generarCompletemento")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_08")]
        public async Task<FinanzasResponseOperacionFactura> generarComplemento([FromBody] FinanzasRequestOperacionFactura item)
        {

            FinanzasResponseOperacionFactura response = new FinanzasResponseOperacionFactura();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Finanzas/VisorFacturas/generarCompletemento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<FinanzasResponseOperacionFactura>(reqResponse);
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

        [Route("VisorFacturas/getDocumentoExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_08")]
        public async Task<FinanzasResponseReporteDocumentoSATExcel> getDocumentosSATExcel([FromBody]LeadsRequestDatosReportes item)
        {
            FinanzasResponseDocumentoSAT response = new FinanzasResponseDocumentoSAT();
            FinanzasResponseReporteDocumentoSATExcel responseExcel = new FinanzasResponseReporteDocumentoSATExcel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Finanzas/VisorFacturas/getDocumento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<FinanzasResponseDocumentoSAT>(reqResponse);

                    ExcelPackage workbook = new ExcelPackage();
                    workbook.Workbook.Worksheets.Add("reporteCFDIs");
                    var hojaActual = workbook.Workbook.Worksheets["reporteCFDIs"];
                    hojaActual = workbook.Workbook.Worksheets[0];


                    hojaActual.Cells["A1"].Value = "ID";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Lote BE";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Fecha Emision";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "RFC";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Razón Social";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "SubTotal";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["G1"].Value = "IVA";
                    hojaActual.Cells["G1"].AutoFitColumns();
                    hojaActual.Cells["H1"].Value = "Total";
                    hojaActual.Cells["H1"].AutoFitColumns();
                    hojaActual.Cells["I1"].Value = "Tipo Comprobante";
                    hojaActual.Cells["I1"].AutoFitColumns();
                    hojaActual.Cells["J1"].Value = "Folio serie";
                    hojaActual.Cells["J1"].AutoFitColumns();
                    hojaActual.Cells["K1"].Value = "UUID";
                    hojaActual.Cells["K1"].AutoFitColumns();
                    hojaActual.Cells["L1"].Value = "Descripción";
                    hojaActual.Cells["L1"].AutoFitColumns();
                    hojaActual.Cells["M1"].Value = "Tasa IVA";
                    hojaActual.Cells["M1"].AutoFitColumns();
                    hojaActual.Cells["N1"].Value = "Observaciones Generales";
                    hojaActual.Cells["N1"].AutoFitColumns();
                    hojaActual.Cells["O1"].Value = "Método de Pago";
                    hojaActual.Cells["O1"].AutoFitColumns();
                    hojaActual.Cells["P1"].Value = "Depósito ID";
                    hojaActual.Cells["P1"].AutoFitColumns();
                    hojaActual.Cells["Q1"].Value = "Fecha Depósito";
                    hojaActual.Cells["Q1"].AutoFitColumns();
                    hojaActual.Cells["R1"].Value = "Estatus";
                    hojaActual.Cells["R1"].Value = "Estatus";


                    hojaActual.Cells["A1:R1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (DocumentoSAT documento in response.listadoDocumentos)
                    {

                        hojaActual.Cells["A" + linea].Value = int.Parse(documento.id.ToString());
                        hojaActual.Cells["B" + linea].Value = documento.idLoteBE.ToString();
                        hojaActual.Cells["C" + linea].Value = documento.fechaEmision.ToString();
                        hojaActual.Cells["D" + linea].Value = documento.rfc.ToString();
                        hojaActual.Cells["E" + linea].Value = documento.razonSocial.ToString();
                        hojaActual.Cells["F" + linea].Value = Convert.ToDecimal(documento.subTotal.ToString());
                        hojaActual.Cells["F" + linea].Style.Numberformat.Format = "###,###,##0.00";
                        hojaActual.Cells["G" + linea].Value = Convert.ToDecimal(documento.iva.ToString());
                        hojaActual.Cells["G" + linea].Style.Numberformat.Format = "###,###,##0.00";
                        hojaActual.Cells["H" + linea].Value = Convert.ToDecimal(documento.total.ToString());
                        hojaActual.Cells["H" + linea].Style.Numberformat.Format = "###,###,##0.00";
                        hojaActual.Cells["I" + linea].Value = documento.tipoComprobante.ToString();
                        hojaActual.Cells["J" + linea].Value = documento.folioSerie.ToString();
                        hojaActual.Cells["K" + linea].Value = documento.uuid.ToString();
                        hojaActual.Cells["L" + linea].Value = documento.concepto.ToString();
                        hojaActual.Cells["M" + linea].Value = Convert.ToDecimal(documento.tasaIVA.ToString());
                        hojaActual.Cells["M" + linea].Style.Numberformat.Format = "###,###,##0.00";
                        hojaActual.Cells["N" + linea].Value = documento.observacionesGenerales.ToString();
                        hojaActual.Cells["O" + linea].Value = documento.metodoPago.ToString();
                        hojaActual.Cells["P" + linea].Value = documento.idDeposito.ToString();
                        hojaActual.Cells["Q" + linea].Value = documento.fechaDeposito.ToString();
                        hojaActual.Cells["R" + linea].Value = documento.cancelado.ToString();


                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 18];
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


                    }

                    // retornar datos
                    responseExcel.CodigoRespuesta = 0;
                    responseExcel.reporteNombre = "reporteCFDIs" + DateTime.Now.ToString("dd/mm/yyyy") + ".xlsx";
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
            return responseExcel;
        }


        [HttpPost]
        [Route("VisorFacturas/getFactura")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_03_01_08")]
        public async Task<FinanzasResponseFactua> getFactura([FromBody] FinanzasRequestGetXMLFactura item)
        {

            FinanzasResponseFactua response = new FinanzasResponseFactua();

            Is3_3 is3 = new Is3_3();
            XslTransform myXslTransform = new XslTransform();
            XPathDocument doc = null;
            StringWriter fs = new StringWriter();
            string docto = string.Empty;

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Finanzas/VisorFacturas/FinanzasRequestGetXMLFactura", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<FinanzasResponseFactua>(reqResponse);

                    is3 = util.try3_3(response.xml);

                    if (is3.is3_3 && !is3.isComplemento)
                    {
                        myXslTransform.Load(Path.Combine(_env.ContentRootPath, @"\xsls\Sender19877A_Pv2dll_3_3.xsl"));
                    }
                    else if (is3.is3_3 && is3.isComplemento)
                    {
                        myXslTransform.Load(Path.Combine(_env.ContentRootPath, @"\xsls\Sender_Generico_3_3CPago.xsl"));
                    }
                    else
                    {
                        myXslTransform.Load(Path.Combine(_env.ContentRootPath, @"\xsls\Sender19877A_Pv2dll.xsl"));
                    }
                    doc = new XPathDocument(docto);
                    myXslTransform.Transform(doc, null, fs, null);


                    response.xml = fs.ToString();
                    response.ruta = docto;
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

    }


}
