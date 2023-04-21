using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Contabilidad")]
    public class ContabilidadController : Controller
    {
        private readonly AppSettings __appSettings;
        private RequestHelper request;
        public ContabilidadController(IOptions<AppSettings> appSetings)
        {
            this.__appSettings = appSetings.Value;
            this.request = new RequestHelper(this.__appSettings);
        }

        #region Ajuste de saldo

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CuentasBanco")]
        [MTCAuthorizationHandler("PERM_05_01")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("ContabilidadFormaPago")]
        [MTCAuthorizationHandler("PERM_05_01")]
        public ContabilidadFormasPagoResponse ContabilidadFormaPago()
        {
            ContabilidadFormasPagoResponse responseD = new ContabilidadFormasPagoResponse();
            try
            {
                if (ModelState.IsValid)
                {

                    Task<string> taskResponse = request.doPost("Contabilidad/formasPago", null, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<ContabilidadFormasPagoResponse>(response);
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
        [Route("GuardarAjusteSaldo")]
        [MTCAuthorizationHandler("PERM_05_01")]
        public ContabilidadAjusteSaldoGuardarResponse ContabilidadGuardarAjusteSaldo([FromBody]ContabilidadAjusteSaldoGuardar datos)
        {
            ContabilidadAjusteSaldoGuardarResponse responseD = new ContabilidadAjusteSaldoGuardarResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    datos.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    datos.horaDeposito = DateTime.Now;
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(datos));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), datos);
                    Task<string> taskResponse = request.doPost("Contabilidad/GuardarAjusteSaldo", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<ContabilidadAjusteSaldoGuardarResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.respuesta > 0 )
                        {
                            responseD.MensajeRespuesta = "Ajuste de saldo realizado correctamente";
                        }
                        else
                        {
                            responseD.MensajeRespuesta = "Error al realizar el ajuste de saldo";
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


        [Route("GetCadenaInfo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_05_01")]
        public ContabilidadResponseInformacionCadena ContabilidadGetCadena([FromBody]LiberacionesCadenaRequestInformacionCadena item)
        {
            ContabilidadResponseInformacionCadena responseD = new ContabilidadResponseInformacionCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Contabilidad/GetCadenaInfo", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ContabilidadResponseInformacionCadena>(response);

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


        #region Cartera de Clientes
        [Route("GenerarCartera")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_05_02")]
        public ContabilidaGenerarCarteraResponse2 GenerarCartera([FromBody]ContabilidadGenerarCartera datos)
        {
            ContabilidaGenerarCarteraResponse2 responseD = new ContabilidaGenerarCarteraResponse2();
            try
            {
                if (ModelState.IsValid)
                {

                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(datos));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), datos);
                    Task<string> taskResponse = request.doPost("Contabilidad/GenerarCartera", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        ContabilidaGenerarCarteraResponse responseD1 = new ContabilidaGenerarCarteraResponse();
                        responseD1 = JsonConvert.DeserializeObject<ContabilidaGenerarCarteraResponse>(response);

                        responseD.CodigoRespuesta = responseD1.CodigoRespuesta;
                       

                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD1.CodigoRespuesta == 0)
                        {
                            responseD.MensajeRespuesta = "Cartera generada correctamente";
                            

                            ExcelPackage workbook = new ExcelPackage();
                            workbook.Workbook.Worksheets.Add("Cartera de Clientes");
                            var hojaActual = workbook.Workbook.Worksheets["Cartera"];
                            hojaActual = workbook.Workbook.Worksheets[0];

                            hojaActual.Cells["A1"].Value = "id";
                            hojaActual.Cells["B1"].Value = "nombreComercial";
                            hojaActual.Cells["C1"].Value = "tasaIVA";
                            hojaActual.Cells["D1"].Value = "Condicion Comercial";
                            hojaActual.Cells["E1"].Value = "Saldo Contable";
                            hojaActual.Cells["F1"].Value = "Recargas";
                            hojaActual.Cells["G1"].Value = "recargasInternacionales";
                            hojaActual.Cells["H1"].Value = "iusaCfeTarjeta";
                            hojaActual.Cells["I1"].Value = "Zuggy";
                            hojaActual.Cells["J1"].Value = "Tienda Virtual";
                            hojaActual.Cells["K1"].Value = "Celulares Pay and Go";
                            hojaActual.Cells["L1"].Value = "Comision Trasp. OPE";
                            hojaActual.Cells["M1"].Value = "Nota de Credito 16%";
                            hojaActual.Cells["N1"].Value = "Servicios Datalogic";
                            hojaActual.Cells["O1"].Value = "Elektra";
                            hojaActual.Cells["P1"].Value = "Regalii";
                            hojaActual.Cells["Q1"].Value = "Traspasos Operadora";
                            hojaActual.Cells["R1"].Value = "Traspaso Cargo";
                            hojaActual.Cells["S1"].Value = "Profarmex";
                            hojaActual.Cells["T1"].Value = "TimbradoFP";
                            hojaActual.Cells["U1"].Value = "iusaCfeRecarga";
                            hojaActual.Cells["V1"].Value = "Televia";
                            hojaActual.Cells["W1"].Value = "IAVE";
                            hojaActual.Cells["X1"].Value = "ViaPass";
                            hojaActual.Cells["Y1"].Value = "BANAMEX2376";
                            hojaActual.Cells["Z1"].Value = "BANCOMER989";
                            hojaActual.Cells["AA1"].Value = "BANCOMER1466";
                            hojaActual.Cells["AB1"].Value = "SANTANDER";
                            hojaActual.Cells["AC1"].Value = "BANORTE";
                            hojaActual.Cells["AD1"].Value = "AZTECA";
                            hojaActual.Cells["AE1"].Value = "BBVA1131";
                            hojaActual.Cells["AF1"].Value = "BBVA0728FARMA";
                            hojaActual.Cells["AG1"].Value = "BNX5907FARMA";
                            hojaActual.Cells["AH1"].Value = "BANBAJIO";
                            hojaActual.Cells["AI1"].Value = "SANTANDERFARMA";
                            hojaActual.Cells["AJ1"].Value = "BANORTEFARMA";
                            hojaActual.Cells["AK1"].Value = "ciBanco";
                            hojaActual.Cells["AL1"].Value = "Intermex";
                            hojaActual.Cells["AM1"].Value = "SantanderTPV";
                            hojaActual.Cells["AN1"].Value = "BancomerTPV";
                            hojaActual.Cells["AO1"].Value = "Lealtad";
                            hojaActual.Cells["AP1"].Value = "Decremento Recarga";
                            hojaActual.Cells["AQ1"].Value = "Decremento Servicio";
                            hojaActual.Cells["AR1"].Value = "transpasoComRecarga";
                            hojaActual.Cells["AS1"].Value = "compraComRecarga";
                            hojaActual.Cells["AT1"].Value = "zuggyComRecarga";
                            hojaActual.Cells["AU1"].Value = "Incremento Recarga";
                            hojaActual.Cells["A1:AU1"].Style.Font.Bold = true;
                            int linea = 2;

                            foreach (ContabilidadCarteraData carteraNew in responseD1.respuesta)
                            {

                                hojaActual.Cells["A" + linea].Value = carteraNew.id.ToString();
                                hojaActual.Cells["B" + linea].Value = carteraNew.nombreComercial.ToString();
                                hojaActual.Cells["C" + linea].Value = carteraNew.tasaIVA.ToString();
                                hojaActual.Cells["D" + linea].Value = carteraNew.CondicionComercial.ToString();
                                hojaActual.Cells["E" + linea].Value = carteraNew.SaldoContable.ToString();
                                hojaActual.Cells["F" + linea].Value = carteraNew.Recargas.ToString();
                                hojaActual.Cells["G" + linea].Value = carteraNew.recargasInternacionales.ToString();
                                hojaActual.Cells["H" + linea].Value = carteraNew.iusaCfeTarjeta.ToString();
                                hojaActual.Cells["I" + linea].Value = carteraNew.Zuggy.ToString();
                                hojaActual.Cells["J" + linea].Value = carteraNew.TiendaVirtual.ToString();
                                hojaActual.Cells["K" + linea].Value = carteraNew.CelularesPayandGo.ToString();
                                hojaActual.Cells["L" + linea].Value = carteraNew.ComisionTraspOPE.ToString();
                                hojaActual.Cells["M" + linea].Value = carteraNew.NotadeCredito.ToString();
                                hojaActual.Cells["N" + linea].Value = carteraNew.ServiciosDatalogic.ToString();
                                hojaActual.Cells["O" + linea].Value = carteraNew.Elektra.ToString();
                                hojaActual.Cells["P" + linea].Value = carteraNew.Regalii.ToString();
                                hojaActual.Cells["Q" + linea].Value = carteraNew.TraspasosOperadora.ToString();
                                hojaActual.Cells["R" + linea].Value = carteraNew.TraspasoCargo.ToString();
                                hojaActual.Cells["S" + linea].Value = carteraNew.Profarmex.ToString();
                                hojaActual.Cells["T" + linea].Value = carteraNew.TimbradoFP.ToString();
                                hojaActual.Cells["U" + linea].Value = carteraNew.iusaCfeRecarga.ToString();
                                hojaActual.Cells["V" + linea].Value = carteraNew.Televia.ToString();
                                hojaActual.Cells["W" + linea].Value = carteraNew.IAVE.ToString();
                                hojaActual.Cells["X" + linea].Value = carteraNew.ViaPass.ToString();
                                hojaActual.Cells["Y" + linea].Value = carteraNew.BANAMEX2376.ToString();
                                hojaActual.Cells["Z" + linea].Value = carteraNew.BANCOMER989.ToString();
                                hojaActual.Cells["AA" + linea].Value = carteraNew.BANCOMER1466.ToString();
                                hojaActual.Cells["AB" + linea].Value = carteraNew.SANTANDER.ToString();
                                hojaActual.Cells["AC" + linea].Value = carteraNew.BANORTE.ToString();
                                hojaActual.Cells["AD" + linea].Value = carteraNew.AZTECA.ToString();
                                hojaActual.Cells["AE" + linea].Value = carteraNew.BBVA1131.ToString();
                                hojaActual.Cells["AF" + linea].Value = carteraNew.BBVA0728FARMA.ToString();
                                hojaActual.Cells["AG" + linea].Value = carteraNew.BNX5907FARMA.ToString();
                                hojaActual.Cells["AH" + linea].Value = carteraNew.BANBAJIO.ToString();
                                hojaActual.Cells["AI" + linea].Value = carteraNew.SANTANDERFARMA.ToString();
                                hojaActual.Cells["AJ" + linea].Value = carteraNew.BANORTEFARMA.ToString();
                                hojaActual.Cells["AK" + linea].Value = carteraNew.ciBanco.ToString();
                                hojaActual.Cells["AL" + linea].Value = carteraNew.Intermex.ToString();
                                hojaActual.Cells["AM" + linea].Value = carteraNew.SantanderTPV.ToString();
                                hojaActual.Cells["AN" + linea].Value = carteraNew.BancomerTPV.ToString();
                                hojaActual.Cells["AO" + linea].Value = carteraNew.Lealtad.ToString();
                                hojaActual.Cells["AP" + linea].Value = carteraNew.DecrementoRecarga.ToString();
                                hojaActual.Cells["AQ" + linea].Value = carteraNew.DecrementoServicio.ToString();
                                hojaActual.Cells["AR" + linea].Value = carteraNew.transpasoComRecarga.ToString();
                                hojaActual.Cells["AS" + linea].Value = carteraNew.compraComRecarga.ToString();
                                hojaActual.Cells["AT" + linea].Value = carteraNew.zuggyComRecarga.ToString();
                                hojaActual.Cells["AU" + linea].Value = carteraNew.IncrementoRecarga.ToString();

                                linea++;
                            }

                            var range = hojaActual.Cells[1, 1, linea - 1, 47];
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
                            responseD.respuesta = base64Excel;
                            responseD.reporteNombre = "Cartera de Clientes.xlsx";

                        }
                        else
                        {
                            responseD.MensajeRespuesta = "Error al generar la cartera";
                        }

                        responseD.MensajeRespuesta = responseD1.MensajeRespuesta;
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
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseD;
        }


        #endregion
    }
}