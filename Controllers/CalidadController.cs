using System;
using System.IO;
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
    [Route("api/Calidad")]
    [ApiController]
    public class CalidadController : Controller
    {
        private readonly AppSettings __appSettings;
        private readonly ConfigurationSettings __configurations;
        private RequestHelper request;
        private Utils util;

        public CalidadController(IOptions<AppSettings> appSetings, IOptions<ConfigurationSettings> configurations)
        {
            this.__appSettings = appSetings.Value;
            this.__configurations = configurations.Value;
            this.request = new RequestHelper(this.__appSettings);
            util = new Utils();
        }

        #region Lista Seguimiento

        [Route("Seguimiento/ListadoMotivoSeguimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseListadoMotivoSeguimiento> motivoSeguimiento()
        {
            CalidadResponseListadoMotivoSeguimiento response = new CalidadResponseListadoMotivoSeguimiento();
            try
            {
                string reqResponse = await request.doPost("Calidad/Seguimiento/ListadoMotivoSeguimiento", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseListadoMotivoSeguimiento>(reqResponse);
                    
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

        [Route("Seguimiento/ListadoEstatusSeguimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseListadoEstatusSeguimiento> estatusSeguimiento()
        {
            CalidadResponseListadoEstatusSeguimiento response = new CalidadResponseListadoEstatusSeguimiento();
            try
            {
                string reqResponse = await request.doPost("Calidad/Seguimiento/ListadoEstatusSeguimiento", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseListadoEstatusSeguimiento>(reqResponse);

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

        [Route("Seguimiento/ListadoPerdido")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseListadoPerdidaSeguimiento> listadoPerdido()
        {
            CalidadResponseListadoPerdidaSeguimiento response = new CalidadResponseListadoPerdidaSeguimiento();
            try
            {
                string reqResponse = await request.doPost("Calidad/Seguimiento/ListadoPerdido", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseListadoPerdidaSeguimiento>(reqResponse);

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

        [Route("Seguimiento/ListadoSeguimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadRespponseListaSeguimiento> listadoSeguimiento([FromBody] CalidadRequestListaSeguimiento item)
        {
            CalidadRespponseListaSeguimiento response = new CalidadRespponseListaSeguimiento();
            try
            {
                if (item.firstTime)
                {
                    item.orden = 1;
                    item.activo = -1;
                    item.incremento = 0;
                    item.idMotivo = 0;
                }
                else
                {

                    if (!string.IsNullOrEmpty(item.fechaIni) && !string.IsNullOrEmpty(item.fechaFin))
                    {

                        item.fechaIni = DateTime.Parse(item.fechaIni.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                        item.fechaFin = DateTime.Parse(item.fechaFin.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                    }

                }



                /*item.fechaIni = "01/01/2018";
                item.fechaFin = "30/06/2022";*/

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/ListadoSeguimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadRespponseListaSeguimiento>(reqResponse);

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

        [Route("Seguimiento/CambioEstatusIncremento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseEstatusIncremento> cambioEstatusIncremento([FromBody] CalidadRequestCambioEstatusComision item)
        {
            CalidadResponseEstatusIncremento response = new CalidadResponseEstatusIncremento();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/CambioEstatusIncremento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseEstatusIncremento>(reqResponse);
                    
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Calidad",
                        Actividad = "Seguimiento/CambioEstatusIncremento",
                        Detalles = "Estatus cambiado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerCalidad);
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

        [Route("Seguimiento/CambioEstatus")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseCambioEstatus> cambioEstatus([FromBody] CalidadRequestCambioEstatus item)
        {
            CalidadResponseCambioEstatus response = new CalidadResponseCambioEstatus();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/CambioEstatus", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseCambioEstatus>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Calidad",
                        Actividad = "Seguimiento/CambioEstatus",
                        Detalles = "Estatus cambiado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerCalidad);
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

        [Route("Seguimiento/HistorialSeguimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseListadoHistorialSeguimiento> historialSeguimiento([FromBody] CalidadRequestHistorialListaSeguimiento item)
        {
            CalidadResponseListadoHistorialSeguimiento response = new CalidadResponseListadoHistorialSeguimiento();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/HistorialSeguimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseListadoHistorialSeguimiento>(reqResponse);

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

        [Route("Seguimiento/ListadoCorreos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseListadoEnvioCorreo> listaCorreos()
        {
            CalidadResponseListadoEnvioCorreo response = new CalidadResponseListadoEnvioCorreo();
            try
            {
                string reqResponse = await request.doPost("Calidad/Seguimiento/ListadoCorreos", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseListadoEnvioCorreo>(reqResponse);

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

        [Route("Seguimiento/EnvioCorreo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseEnvioCorreo> EnvioCorreo([FromBody] CalidadRequestEnviaCorreo item)
        {
            CalidadResponseEnvioCorreo response = new CalidadResponseEnvioCorreo();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/EnvioCorreo", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseEnvioCorreo>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Calidad",
                        Actividad = "Seguimiento/EnvioCorreo",
                        Detalles = "Correo exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerCalidad);
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

        [Route("Seguimiento/ListaTareasSeguimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseListadoTareasSeguimiento> listaTareasSeguimiento([FromBody] CalidadRequestListadoTareas item)
        {
            CalidadResponseListadoTareasSeguimiento response = new CalidadResponseListadoTareasSeguimiento();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/ListaTareasSeguimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseListadoTareasSeguimiento>(reqResponse);

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

        [Route("Seguimiento/ActualizaEstadoTareaSeguimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseActualizaEstadoTarea> actualizaTareaSeguimiento([FromBody] CalidadRequestActualizaTarea item)
        {
            CalidadResponseActualizaEstadoTarea response = new CalidadResponseActualizaEstadoTarea();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/ActualizaEstadoTareaSeguimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseActualizaEstadoTarea>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Calidad",
                        Actividad = "Seguimiento/ActualizaEstadoTareaSeguimiento",
                        Detalles = "Tarea cambiada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerCalidad);
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

        [Route("Seguimiento/EliminaTareaSeguimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseActualizaEstadoTarea> eliminaTareaSeguimiento([FromBody] CalidadRequestActualizaTarea item)
        {
            CalidadResponseActualizaEstadoTarea response = new CalidadResponseActualizaEstadoTarea();
            try
            {
                item.idEstatusTarea = 2;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/EliminaTareaSeguimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseActualizaEstadoTarea>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Calidad",
                        Actividad = "Seguimiento/EliminaTareaSeguimiento",
                        Detalles = "Tarea eliminada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerCalidad);
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

        [Route("Seguimiento/ActualizaObservacionesTarea")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseActualizaEstadoTarea> actualizaObservacionesTarea([FromBody] CalidadRequestActualizaTarea item)
        {
            CalidadResponseActualizaEstadoTarea response = new CalidadResponseActualizaEstadoTarea();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/ActualizaObservacionesTarea", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseActualizaEstadoTarea>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Calidad",
                        Actividad = "Seguimiento/ActualizaObservacionesTarea",
                        Detalles = "Observaciones cambiadas exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerCalidad);
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

        [Route("Seguimiento/AgregaTarea")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseNuevaTarea> agregaTarea([FromBody] CalidadRequestNuevaTarea item)
        {
            CalidadResponseNuevaTarea response = new CalidadResponseNuevaTarea();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/AgregaTarea", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseNuevaTarea>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Calidad",
                        Actividad = "Seguimiento/AgregaTarea",
                        Detalles = "Tarea agregada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerCalidad);
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


        [Route("Seguimiento/ListadoFasesTarea")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseListadoFasesTarea> listadoFasesTarea()
        {
            CalidadResponseListadoFasesTarea response = new CalidadResponseListadoFasesTarea();
            try
            {
                string reqResponse = await request.doPost("Calidad/Seguimiento/ListadoFasesTarea", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseListadoFasesTarea>(reqResponse);

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


        [Route("Seguimiento/HistorialVenta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseHistorialVenta> historialVenta([FromBody] CalidadRequestHistorialVenta item)
        {
            CalidadResponseHistorialVenta response = new CalidadResponseHistorialVenta();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/HistorialVenta", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseHistorialVenta>(reqResponse);

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


        [Route("Seguimiento/AgregarCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseAgregaCadena> agregarCadena([FromBody] CalidadRequestAgregarCadena item)
        {
            CalidadResponseAgregaCadena response = new CalidadResponseAgregaCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/AgregarCadena", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseAgregaCadena>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Calidad",
                        Actividad = "Seguimiento/AgregarCadena",
                        Detalles = "Cadena agregada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerCalidad);
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


        [Route("Seguimiento/Reporte")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_02")]
        public async Task<CalidadResponseReporte> reportes([FromBody] CalidadRequestReporteSeguimiento item)
        {
            CalidadResponseReporte response = new CalidadResponseReporte();
            try
            {

                item.fechaIni = DateTime.Parse(item.fechaIni.Split("T")[0]).ToString("dd/MM/yyyy");
                item.fechaFin = DateTime.Parse(item.fechaFin.Split("T")[0]).ToString("dd/MM/yyyy");

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/Reporte", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseReporte>(reqResponse);

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


        [Route("Seguimiento/ListaeguimientoExport")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_11,PERM_11_01")]
        public async Task<CalidadResponseSeguimientoExcel> listaTareasSeguimientoExport([FromBody] CalidadRequestListaSeguimiento item)
        {
            CalidadResponseListaSeguimientoExport response = new CalidadResponseListaSeguimientoExport();
            CalidadResponseSeguimientoExcel responseExcel = new CalidadResponseSeguimientoExcel();
            ExcelPackage workbook = new ExcelPackage();


            try
            {
                if (item.firstTime)
                {
                    item.orden = 1;
                    item.activo = -1;
                    item.incremento = 0;
                    item.idMotivo = 0;
                }
                else
                {

                    if (!string.IsNullOrEmpty(item.fechaIni) && !string.IsNullOrEmpty(item.fechaFin))
                    {

                        item.fechaIni = DateTime.Parse(item.fechaIni.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                        item.fechaFin = DateTime.Parse(item.fechaFin.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";
                    }

                }

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Calidad/Seguimiento/ListaSeguimientoExport", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<CalidadResponseListaSeguimientoExport>(reqResponse);

                    DateTime dateActual = DateTime.Now;


                    workbook.Workbook.Worksheets.Add("Reporte Seguimiento");
                    var hojaActual = workbook.Workbook.Worksheets["Reporte Seguimiento"];
                    hojaActual = workbook.Workbook.Worksheets[0];

                    hojaActual.Cells["A1"].Value = "MTCID";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Razón";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Comisión";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "Grupo";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Fecha Alta";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "Fecha Inclusión";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["G1"].Value = "Contacto";
                    hojaActual.Cells["G1"].AutoFitColumns();
                    hojaActual.Cells["H1"].Value = "Teléfono 1";
                    hojaActual.Cells["H1"].AutoFitColumns();
                    hojaActual.Cells["I1"].Value = "Teléfono 2";
                    hojaActual.Cells["I1"].AutoFitColumns();
                    hojaActual.Cells["J1"].Value = "Email";
                    hojaActual.Cells["J1"].AutoFitColumns();
                    hojaActual.Cells["K1"].Value = "Activo";
                    hojaActual.Cells["K1"].AutoFitColumns();
                    hojaActual.Cells["L1"].Value = "Motivo";
                    hojaActual.Cells["L1"].AutoFitColumns();
                    hojaActual.Cells["M1"].Value = "Oficina";
                    hojaActual.Cells["M1"].AutoFitColumns();
                    hojaActual.Cells["N1"].Value = "Promotor";
                    hojaActual.Cells["N1"].AutoFitColumns();
                    hojaActual.Cells["O1"].Value = "Estatus";
                    hojaActual.Cells["O1"].AutoFitColumns();
                    hojaActual.Cells["P1"].Value = "Observaciones";
                    hojaActual.Cells["P1"].AutoFitColumns();
                    hojaActual.Cells["Q1"].Value = "Fecha Tarea";
                    hojaActual.Cells["Q1"].AutoFitColumns();
                    hojaActual.Cells["R1"].Value = "Observaciones Tarea";
                    hojaActual.Cells["R1"].AutoFitColumns();
                    hojaActual.Cells["S1"].Value = "Asginado";
                    hojaActual.Cells["S1"].AutoFitColumns();
                    hojaActual.Cells["T1"].Value = "Incremento";
                    hojaActual.Cells["T1"].AutoFitColumns();
                    hojaActual.Cells["U1"].Value = util.getMes(int.Parse(dateActual.AddMonths(-3).ToString("MM"))); ;
                    hojaActual.Cells["U1"].AutoFitColumns();
                    hojaActual.Cells["V1"].Value = util.getMes(int.Parse(dateActual.AddMonths(-2).ToString("MM")));
                    hojaActual.Cells["V1"].AutoFitColumns();
                    hojaActual.Cells["W1"].Value = util.getMes(int.Parse(dateActual.AddMonths(-1).ToString("MM")));
                    hojaActual.Cells["W1"].AutoFitColumns();
                    hojaActual.Cells["X1"].Value = util.getMes(int.Parse(dateActual.ToString("MM")));
                    hojaActual.Cells["X1"].AutoFitColumns();
                    hojaActual.Cells["A1:X1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (CalidadSeguimientoExport reporte in response.listadoSeguimiento)
                    {

                        hojaActual.Cells["A" + linea].Value = reporte.mtcid;
                        hojaActual.Cells["B" + linea].Value = reporte.razon;
                        hojaActual.Cells["C" + linea].Value = reporte.comisionRecarga;
                        hojaActual.Cells["D" + linea].Value = reporte.grupo;
                        hojaActual.Cells["E" + linea].Value = reporte.fechaAlta;
                        hojaActual.Cells["F" + linea].Value = reporte.fechaInclusion;
                        hojaActual.Cells["G" + linea].Value = reporte.contacto;
                        hojaActual.Cells["H" + linea].Value = reporte.telefono1;
                        hojaActual.Cells["I" + linea].Value = reporte.telefono2;
                        hojaActual.Cells["J" + linea].Value = reporte.email;
                        hojaActual.Cells["K" + linea].Value = reporte.activo;
                        hojaActual.Cells["L" + linea].Value = reporte.motivo;
                        hojaActual.Cells["M" + linea].Value = reporte.oficina;
                        hojaActual.Cells["N" + linea].Value = reporte.promotor;
                        hojaActual.Cells["O" + linea].Value = reporte.estatus;
                        hojaActual.Cells["P" + linea].Value = reporte.observaciones;
                        hojaActual.Cells["Q" + linea].Value = reporte.fechaTarea;
                        hojaActual.Cells["R" + linea].Value = reporte.observacionesTarea;
                        hojaActual.Cells["S" + linea].Value = reporte.asignado;
                        hojaActual.Cells["T" + linea].Value = reporte.incremento == 1 ? "Si" : "No";
                        hojaActual.Cells["U" + linea].Value = reporte.mes1;
                        hojaActual.Cells["U" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["V" + linea].Value = reporte.mes2;
                        hojaActual.Cells["V" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["W" + linea].Value = reporte.mes3;
                        hojaActual.Cells["W" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["X" + linea].Value = reporte.mes4;
                        hojaActual.Cells["X" + linea].Style.Numberformat.Format = "$#,##0.00";

                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 24];
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
                    responseExcel.reporteNombre = "ReporteSeguimiento_"+ util.getMes(int.Parse(dateActual.ToString("MM"))) + ".xlsx";
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


        #endregion
    }
}
