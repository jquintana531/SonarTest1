using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using static MTC.WebApp.BackOffice.Models.RecursosModel;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Recursos")]
    public class RecursosController : Controller
    {
        private readonly AppSettings __appSettings;
        private RequestHelper request;
        private Utils util;
        public RecursosController(IOptions<AppSettings> appSetings)
        {
            this.__appSettings = appSetings.Value;
            this.request = new RequestHelper(this.__appSettings);
            util = new Utils();
        }


        #region Buscador Universal

        [Route("BuscadorUniversal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_01")]
        public async Task<RecursosResponseBuscadorUniversal> ReporteBuscadorUniversalAsync([FromBody]RecursosRequestBuscador item)
        {
            RecursosResponseBuscadorUniversal response = new RecursosResponseBuscadorUniversal();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/BuscadorUniversal/buscar", data, HttpContext.Session.GetString("token"));
                

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseBuscadorUniversal>(reqResponse);

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
                response.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return response;
        }



        [Route("BuscadorUniversalExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_01")]
        public async Task<RecursosResponseBuscadorUniversalExcel> ReporteBuscadorUniversalExcelAsync([FromBody]RecursosRequestBuscador item)
        {
            RecursosResponseBuscadorUniversal response = new RecursosResponseBuscadorUniversal();
            RecursosResponseBuscadorUniversalExcel responseExcel = new RecursosResponseBuscadorUniversalExcel();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/BuscadorUniversal/buscar", data, HttpContext.Session.GetString("token"));
                
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseBuscadorUniversal>(reqResponse);



                    if (response.CodigoRespuesta == 0)
                    {
                        responseExcel.respuestaLista =  "Reporte Buscador Universal";

                        response.MensajeRespuesta = "Reporte Buscador Universal";
                        responseExcel.respuestaLista = response.respuesta;


                        ExcelPackage workbook = new ExcelPackage();
                        workbook.Workbook.Worksheets.Add("Reporte Buscador Universal");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        int linea = 2;

                        int lineashoja = 0;
                        if (item.tipo == 1)
                        {
                            lineashoja = 2;
                            hojaActual.Cells["A1"].Value = "ID";
                            hojaActual.Cells["B1"].Value = "Cadena";

                            hojaActual.Cells["A1:B1"].Style.Font.Bold = true;

                            String iteratorJson = JsonConvert.SerializeObject(response.respuesta);
                            List<RecursosBuscadorUniversalCadena> iterador = JsonConvert.DeserializeObject<List<RecursosBuscadorUniversalCadena>>(iteratorJson);

                            foreach (RecursosBuscadorUniversalCadena datos in iterador)
                            {
                                hojaActual.Cells["A" + linea].Value = datos.id.ToString();
                                hojaActual.Cells["B" + linea].Value = datos.label.ToString();
                                linea++;
                            }
                        }
                        if (item.tipo == 2)
                        {
                            lineashoja = 3;
                            hojaActual.Cells["A1"].Value = "Id Cadena";
                            hojaActual.Cells["B1"].Value = "Id Establecimiento";
                            hojaActual.Cells["C1"].Value = "Establecimiento";

                            hojaActual.Cells["A1:C1"].Style.Font.Bold = true;

                            String iteratorJson = JsonConvert.SerializeObject(response.respuesta);
                            List<RecursosBuscadorUniversalEstablecimiento> iterador = JsonConvert.DeserializeObject<List<RecursosBuscadorUniversalEstablecimiento>>(iteratorJson);

                            foreach (RecursosBuscadorUniversalEstablecimiento datos in iterador)
                            {
                                hojaActual.Cells["A" + linea].Value = datos.idCadena.ToString();
                                hojaActual.Cells["B" + linea].Value = datos.idEstablecimiento.ToString();
                                hojaActual.Cells["C" + linea].Value = datos.establecimiento.ToString();
                                linea++;
                            }
                        }
                        if (item.tipo == 3)
                        {
                            lineashoja = 4;
                            hojaActual.Cells["A1"].Value = "Id Cadena";
                            hojaActual.Cells["B1"].Value = "Id Establecimiento";
                            hojaActual.Cells["C1"].Value = "ID";
                            hojaActual.Cells["D1"].Value = "Cajero";

                            hojaActual.Cells["A1:D1"].Style.Font.Bold = true;

                            String iteratorJson = JsonConvert.SerializeObject(response.respuesta);
                            List<RecursosBuscadorUniversalCajero> iterador = JsonConvert.DeserializeObject<List<RecursosBuscadorUniversalCajero>>(iteratorJson);

                            foreach (RecursosBuscadorUniversalCajero datos in iterador)
                            {
                                hojaActual.Cells["A" + linea].Value = datos.idCadena.ToString();
                                hojaActual.Cells["B" + linea].Value = datos.idEstablecimiento.ToString();
                                hojaActual.Cells["C" + linea].Value = datos.idCajero.ToString();
                                hojaActual.Cells["D" + linea].Value = datos.cajero.ToString();
                                linea++;
                            }
                        }
                        if (item.tipo == 4)
                        {
                            lineashoja = 4;
                            hojaActual.Cells["A1"].Value = "Id Cadena";
                            hojaActual.Cells["B1"].Value = "Id Establecimiento";
                            hojaActual.Cells["C1"].Value = "Número Terminal";
                            hojaActual.Cells["D1"].Value = "Fecha Alta";

                            hojaActual.Cells["A1:D1"].Style.Font.Bold = true;

                            String iteratorJson = JsonConvert.SerializeObject(response.respuesta);
                            List<RecursosBuscadorUniversalTerminal> iterador = JsonConvert.DeserializeObject<List<RecursosBuscadorUniversalTerminal>>(iteratorJson);

                            foreach (RecursosBuscadorUniversalTerminal datos in iterador)
                            {
                                hojaActual.Cells["A" + linea].Value = datos.idCadena.ToString();
                                hojaActual.Cells["B" + linea].Value = datos.idEstablecimiento.ToString();
                                hojaActual.Cells["C" + linea].Value = datos.numero.ToString();
                                hojaActual.Cells["D" + linea].Value = datos.fechaAlta.ToString();
                                linea++;
                            }
                        }
                        if (item.tipo == 6)
                        {
                            lineashoja = 2;
                            hojaActual.Cells["A1"].Value = "Id Cadena";
                            hojaActual.Cells["B1"].Value = "Nombre Comercial";

                            hojaActual.Cells["A1:B1"].Style.Font.Bold = true;

                            String iteratorJson = JsonConvert.SerializeObject(response.respuesta);
                            List<RecursosBuscadorUniversalRFC> iterador = JsonConvert.DeserializeObject<List<RecursosBuscadorUniversalRFC>>(iteratorJson);

                            foreach (RecursosBuscadorUniversalRFC datos in iterador)
                            {
                                hojaActual.Cells["A" + linea].Value = datos.id.ToString();
                                hojaActual.Cells["B" + linea].Value = datos.nombreComercial.ToString();
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

                        }
                        responseExcel.respuesta = base64Excel;
                        responseExcel.reporteNombre = "Reporte Buscador Universal.xlsx";

                    }
                    else
                    {
                        response.CodigoRespuesta = -5;
                        response.MensajeRespuesta = "Error al generar el reporte";
                    }

                    response.MensajeRespuesta = response.MensajeRespuesta;

                }
                else
                {
                    response.CodigoRespuesta = -6;
                    response.MensajeRespuesta = "Ingrese todos los campos necesarios";

                }
            }
            catch (Exception ex)
            {
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = "Se produjo un error inesperado";
            }
            return responseExcel;
        }

        #endregion

        #region Tickets


        [Route("Tickets/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoTickets> listadoTickets([FromBody] RecursosRequestListadoTickets item)
        {
            RecursosResponseListadoTickets response = new RecursosResponseListadoTickets();
            try
            {
                if (!item.firstTime)
                {
                    if (!string.IsNullOrEmpty(item.desde))
                    {
                        item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                        item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                    }
                }
                item.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoTickets>(reqResponse);
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
        
        [Route("Tickets/listadoTemas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoTemas> listadoTemas()
        {
            RecursosResponseListadoTemas response = new RecursosResponseListadoTemas();
            try
            {
                RequestCatalogo item = new RequestCatalogo() { id = 0, menu = 0 };
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/listadoTemas", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoTemas>(reqResponse);
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
               
        [Route("Tickets/listadoAsuntos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoAsuntos> listadoAsuntos([FromBody] RequestCatalogoAsunto item)
        {
            RecursosResponseListadoAsuntos response = new RecursosResponseListadoAsuntos();
            try
            {
                item.id = 0;
                item.menu = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/listadoAsuntos", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoAsuntos>(reqResponse);
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
        
        [Route("Tickets/listadoDepartamentos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoDepartamentos> listadoDepartamentos()
        {
            RecursosResponseListadoDepartamentos response = new RecursosResponseListadoDepartamentos();
            try
            {
                string reqResponse = await request.doPost("Recursos/Tickets/listadoDepartamentos", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoDepartamentos>(reqResponse);
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

        [Route("Tickets/listadoViaContacto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoContacto> listadoViaContacto()
        {
            RecursosResponseListadoContacto response = new RecursosResponseListadoContacto();
            try
            {
                RequestCatalogo item = new RequestCatalogo() { id = 0, menu = 1 };
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/listadoViaContacto", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoContacto>(reqResponse);
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
        
        [Route("Tickets/listadoPrioridades")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoPrioridades> listadoPrioridades()
        {
            RecursosResponseListadoPrioridades response = new RecursosResponseListadoPrioridades();
            try
            {
                RequestCatalogo item = new RequestCatalogo() { id = 0, menu = 1 };
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/listadoPrioridades", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoPrioridades>(reqResponse);
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

        [Route("Tickets/listadoEstablecimientos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoEstablecimientos> listadoEstablecimientos([FromBody] RecursosRequestListadoEstablecimiento item)
        {
            RecursosResponseListadoEstablecimientos response = new RecursosResponseListadoEstablecimientos();
            try
            {
                item.activo = 2;
                item.menu = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/listadoEstablecimientos", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoEstablecimientos>(reqResponse);
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

        [Route("Tickets/listadoCajeros")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoCajeros> listadoCajeros([FromBody] RecursosRequestListadoCajeros item)
        {
            RecursosResponseListadoCajeros response = new RecursosResponseListadoCajeros();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/listadoCajeros", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoCajeros>(reqResponse);
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

        [Route("Tickets/agregarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseAgregarTicket> agregarTicket([FromBody] RequestNuevoTicket item)
        {
            RecursosResponseAgregarTicket response = new RecursosResponseAgregarTicket();
            try
            {
                if (item.tipoTicket == 1)
                {
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                }

                item.idUsuarioAux = int.Parse(HttpContext.Session.GetString("idUsuario"));
                //if (item.idUsuario == 0)
                //{
                //    item.idUsuario = item.idUsuarioAux;
                //}
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));


                string reqResponse = await request.doPost("Recursos/Tickets/agregarTicket", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseAgregarTicket>(reqResponse);

                    if (item.cerrarTicket && response.CodigoRespuesta == 0 && response.idTicket != 0)
                    {
                        RequestCambioEstatusTicket requestCambio = new RequestCambioEstatusTicket();
                        requestCambio.ticketID = response.idTicket;
                        requestCambio.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                        requestCambio.idCatalogoStatusTicket = 2;
                        data = Seguridad.Encrypt(JsonConvert.SerializeObject(requestCambio));

                        string reqCambioResponse = await request.doPost("Recursos/Tickets/cambioEstatusTicket", data, HttpContext.Session.GetString("token"));
                    }

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


        [Route("Tickets/detalleTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosReponseDetalleTicket> getTicket([FromBody] RequestDetalleTicket item)
        {
            RecursosReponseDetalleTicket response = new RecursosReponseDetalleTicket();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/detalleTicket", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosReponseDetalleTicket>(reqResponse);

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


        [Route("Tickets/conversacionTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseConversacionTicket> getTicketConversacion([FromBody] RequestDetalleTicket item)
        {
            RecursosResponseConversacionTicket response = new RecursosResponseConversacionTicket();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/conversacionTicket", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseConversacionTicket>(reqResponse);

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


        [Route("Tickets/listadoPrioridad")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseCatalogoPrioridad> getCatalogoPrioridad()
        {
            RecursosResponseCatalogoPrioridad response = new RecursosResponseCatalogoPrioridad();
            try
            {

                string reqResponse = await request.doPost("Recursos/Tickets/listadoPrioridad", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseCatalogoPrioridad>(reqResponse);

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

        [Route("Tickets/cambioPrioridad")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseCambioPrioridadTicket> cambioPrioridad([FromBody] RequestCambioPrioridadTicket item)
        {
            RecursosResponseCambioPrioridadTicket response = new RecursosResponseCambioPrioridadTicket();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.Ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/cambioPrioridad", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseCambioPrioridadTicket>(reqResponse);
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

        [Route("Tickets/setVencido")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseCambioVencido> setVencido([FromBody] RequestCambioVencido item)
        {
            RecursosResponseCambioVencido response = new RecursosResponseCambioVencido();
            try
            {
                item.vencido = true;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.Ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/setVencido", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseCambioVencido>(reqResponse);
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


        [Route("Tickets/liberarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseLiberarTicketModel> liberarTicket([FromBody] RequestLiberarTicket item)
        {
            RecursosResponseLiberarTicketModel response = new RecursosResponseLiberarTicketModel();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.Ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/liberarTicket", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseLiberarTicketModel>(reqResponse);
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


        [Route("Tickets/getUsuarioDepartamento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
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


        [Route("Tickets/asignarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseAsignarTicket> asignarTicketv2([FromBody] RequestAsignarTicket item)
        {
            RecursosResponseAsignarTicket response = new RecursosResponseAsignarTicket();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.Ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/asignarTicket", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseAsignarTicket>(reqResponse);
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


        [Route("Tickets/transferirTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseTransferirTicket> transferirTicket([FromBody] RequestTransferirTicket item)
        {
            RecursosResponseTransferirTicket response = new RecursosResponseTransferirTicket();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.Ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/transferirTicket", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseTransferirTicket>(reqResponse);
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

        [Route("Tickets/cerrarTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseCambioEstatusTicket> cerrarTicket([FromBody] RequestCambioEstatusTicket item)
        {
            RecursosResponseCambioEstatusTicket response = new RecursosResponseCambioEstatusTicket();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.idCatalogoStatusTicket = 2;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/cambioEstatusTicket", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseCambioEstatusTicket>(reqResponse);
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


        [Route("Tickets/respuestaTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseRespuestaTicket> respuestaTicket([FromBody] RequestNuevaRespuesta item)
        {
            RecursosResponseRespuestaTicket response = new RecursosResponseRespuestaTicket();
            
            try
            {
                string reqResponse = string.Empty;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));

                RequestNuevaRespuestaAux auxItem = new RequestNuevaRespuestaAux()
                {
                    ticketID = item.ticketID,
                    mensaje = item.mensaje,
                    titulo = item.titulo,
                    tipo = item.tipo,
                    idUsuario = item.idUsuario

                };

                if (item.tipo == 1)
                {
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(auxItem));
                    reqResponse = await request.doPost("Recursos/Tickets/nuevaRespuesta", data, HttpContext.Session.GetString("token"));

                }
                else
                {
                    auxItem.titulo = "Nota Interna";
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(auxItem));
                    reqResponse = await request.doPost("Recursos/Tickets/nuevaNota", data, HttpContext.Session.GetString("token"));

                }


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseRespuestaTicket>(reqResponse);
                    

                    if (item.file != null)
                    {
                        //bool resul = FileSanitizer.uploadFile64(item.file.archivo,item.file.nombre,item.file.ext,this.__appSettings.TicketDir + item.ticketID + "\\",DateTime.Now.ToString("ddMMyyyyHHmmss"));
                        string hashName1 = DateTime.Now.ToString("ddMMyyyyHHmmss");




                        bool resul = FileSanitizer.uploadFile64(item.file.archivo, item.file.nombre, item.file.ext, this.__appSettings.TicketDir, hashName1);
                        //bool resul = FileSanitizer.uploadFile64(item.file.archivo,item.file.nombre,item.file.ext, ticketDir+"\\", hashName1);

                        if (resul)
                        {
                            RequestNuevoTicketAdjunto nuevoTicketRequest = new RequestNuevoTicketAdjunto {
                                ticketID = item.ticketID,
                                idConversacion = response.resultado,
                                tipoConversacion = item.tipo == 1 ? "R" : "N",
                                tipoArchivo = "."+item.file.ext,
                                tamano = item.file.size,
                                nombre = item.file.nombre,
                                hashName = hashName1
                            };

                            string data = Seguridad.Encrypt(JsonConvert.SerializeObject(nuevoTicketRequest));
                            reqResponse = await request.doPost("Recursos/Tickets/nuevoTicketAdjunto", data, HttpContext.Session.GetString("token"));

                        }
                    }

                    item.file.archivo = "";

                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Tickets",
                        Actividad = "Respuesta Ticket",
                        Detalles = "Respuesta",
                        Objeto = JsonConvert.SerializeObject(item)
                    });


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

        [Route("Tickets/reporteMisTickets")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursossResponseMisTickets> reporteMisTickets([FromBody] RequestMisTickets item)
        {
            RecursossResponseMisTickets response = new RecursossResponseMisTickets();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/reporteMisTickets", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursossResponseMisTickets>(reqResponse);
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

        [Route("Tickets/reporteMisTicketsExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseReporteMisTicketsExcel> reporteMisTicketsExcel([FromBody] RequestMisTickets item)
        {
            RecursossResponseMisTickets response = new RecursossResponseMisTickets();
            RecursosResponseReporteMisTicketsExcel responseExcel = new RecursosResponseReporteMisTicketsExcel();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/reporteMisTickets", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursossResponseMisTickets>(reqResponse);
                    ExcelPackage workbook = new ExcelPackage();
                    workbook.Workbook.Worksheets.Add("Mis Tickets");
                    var hojaActual = workbook.Workbook.Worksheets["Mis Tickets"];
                    hojaActual = workbook.Workbook.Worksheets[0];


                    hojaActual.Cells["A1"].Value = "No. Ticket";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Tipo Ticket";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Asunto";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "Medio";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Atendio";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "Fecha creado";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["G1"].Value = "Fecha vencimiento";
                    hojaActual.Cells["G1"].AutoFitColumns();

                    int linea = 2;

                    foreach (MisTickets reporte in response.listadoMisTickets)
                    {

                        hojaActual.Cells["A" + linea].Value = reporte.ticketID.ToString();
                        hojaActual.Cells["B" + linea].Value = reporte.tipoTicket.ToString();
                        hojaActual.Cells["C" + linea].Value = reporte.asunto.ToString();
                        hojaActual.Cells["D" + linea].Value = reporte.medio.ToString();
                        hojaActual.Cells["E" + linea].Value = reporte.atendio.ToString();
                        hojaActual.Cells["F" + linea].Value = reporte.fechaAlta.ToString();
                        hojaActual.Cells["G" + linea].Value = reporte.fechaVencimiento.ToString();

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


                    }

                    // retornar datos
                    responseExcel.CodigoRespuesta = 0;
                    responseExcel.reporteNombre = "ReporteMisTickets.xlsx";
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
                responseExcel.CodigoRespuesta = -9;
                responseExcel.MensajeRespuesta = ex.Message;
            }
            return responseExcel;
        }


        [Route("Tickets/listadoEstatusTickets")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseListadoEstatusTickets> listadoEstatusTickets()
        {
            RecursosResponseListadoEstatusTickets response = new RecursosResponseListadoEstatusTickets();
            try
            {

                string reqResponse = await request.doPost("Recursos/Tickets/listadoEstatusTickets", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseListadoEstatusTickets>(reqResponse);

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


        [Route("Tickets/historialTickets")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseHistorialTickets> historialTickets([FromBody] RequestHistorial item)
        {
            RecursosResponseHistorialTickets response = new RecursosResponseHistorialTickets();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/historialTickets", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseHistorialTickets>(reqResponse);
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

        [Route("Tickets/reporteHistorialTicketsExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseReporteHistorialExcel> reporteHistorialExcel([FromBody] RequestHistorial item)
        {
            RecursosResponseHistorialTickets response = new RecursosResponseHistorialTickets();
            RecursosResponseReporteHistorialExcel responseExcel = new RecursosResponseReporteHistorialExcel();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/historialTickets", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseHistorialTickets>(reqResponse);
                    ExcelPackage workbook = new ExcelPackage();
                    workbook.Workbook.Worksheets.Add("Historial");
                    var hojaActual = workbook.Workbook.Worksheets["Historial"];
                    hojaActual = workbook.Workbook.Worksheets[0];


                    hojaActual.Cells["A1"].Value = "No. Ticket";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Tipo Ticket";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Tema";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "Asunto";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Estatus";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "Medio";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["G1"].Value = "Atendio";
                    hojaActual.Cells["G1"].AutoFitColumns();
                    hojaActual.Cells["H1"].Value = "Fecha creado";
                    hojaActual.Cells["H1"].AutoFitColumns();
                    hojaActual.Cells["I1"].Value = "Fecha vencimiento";
                    hojaActual.Cells["I1"].AutoFitColumns();
                    hojaActual.Cells["J1"].Value = "Departamento";
                    hojaActual.Cells["J1"].AutoFitColumns();

                    int linea = 2;

                    foreach (RecursosModel.HistorialTickets reporte in response.listadoHistorial)
                    {

                        hojaActual.Cells["A" + linea].Value = reporte.ticketID.ToString();
                        hojaActual.Cells["B" + linea].Value = reporte.tipoTicket.ToString();
                        hojaActual.Cells["C" + linea].Value = reporte.tema.ToString();
                        hojaActual.Cells["D" + linea].Value = reporte.asunto.ToString();
                        hojaActual.Cells["E" + linea].Value = reporte.estatus.ToString();
                        hojaActual.Cells["F" + linea].Value = reporte.medio.ToString();
                        hojaActual.Cells["G" + linea].Value = reporte.atendio.ToString();
                        hojaActual.Cells["H" + linea].Value = reporte.fechaAlta.ToString();
                        hojaActual.Cells["I" + linea].Value = reporte.fechaVencimiento.ToString();
                        hojaActual.Cells["J" + linea].Value = reporte.nombre.ToString();

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


                    }

                    // retornar datos
                    responseExcel.CodigoRespuesta = 0;
                    responseExcel.reporteNombre = "ReporteHistorial.xlsx";
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
                responseExcel.CodigoRespuesta = -9;
                responseExcel.MensajeRespuesta = ex.Message;
            }
            return responseExcel;
        }


        [Route("Tickets/ticketsUsuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseTicketsUsuario> historialTickets([FromBody] RequestTicketUsuario item)
        {
            RecursosResponseTicketsUsuario response = new RecursosResponseTicketsUsuario();
            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                item.idUsuarioAdministrador = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/ticketsUsuario", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseTicketsUsuario>(reqResponse);
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


        [Route("Tickets/ticketsUsuarioExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseReporteTicketsUsuarioExcel> reporteHistorialExcel([FromBody] RequestTicketUsuario item)
        {
            RecursosResponseTicketsUsuario response = new RecursosResponseTicketsUsuario();
            RecursosResponseReporteTicketsUsuarioExcel responseExcel = new RecursosResponseReporteTicketsUsuarioExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Recursos/Tickets/ticketsUsuario", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<RecursosResponseTicketsUsuario>(reqResponse);
                    
                    workbook.Workbook.Worksheets.Add("Tickets");
                    var hojaActual = workbook.Workbook.Worksheets["Tickets"];
                    hojaActual = workbook.Workbook.Worksheets[0];


                    hojaActual.Cells["A1"].Value = "Medio";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Total";
                    hojaActual.Cells["B1"].AutoFitColumns();

                    int linea = 2;

                    foreach (TicketsUsuario reporte in response.listadoTicketsUsuario)
                    {

                        hojaActual.Cells["A" + linea].Value = reporte.medio.ToString();
                        hojaActual.Cells["B" + linea].Value = reporte.total.ToString();

                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 2];
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
                    responseExcel.reporteNombre = "ReporteTicketsUsuario.xlsx";
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
                responseExcel.CodigoRespuesta = -9;
                responseExcel.MensajeRespuesta = ex.Message;
            }
            finally
            {
                workbook.Dispose();
            }
            return responseExcel;
        }



        [Route("Tickets/visorTicket")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_09_02")]
        public async Task<RecursosResponseTicketVisor> visorTicket()
        {
            RecursosResponseTicketVisor response = new RecursosResponseTicketVisor();
            try
            {

                response.dir = __appSettings.TicketVisor;
            }
            catch (Exception ex)
            {
                response.dir = "";
            }
            return response;
        }



        #endregion
    }
}
