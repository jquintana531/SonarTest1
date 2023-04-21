using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MTC.WebApp.BackOffice.DataManager;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using MTCenter.GRPC.GDBBO.DBProtos;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Table;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Leads")]
    public class LeadsController : Controller
    {
        private readonly AppSettings __appSettings;
        private RequestHelper request;
        private Utils util;
        private readonly LeadsManager _manager;

        public LeadsController(IOptions<AppSettings> appSetings, DBData.DBDataClient client)
        {
            this.__appSettings = appSetings.Value;
            this.request = new RequestHelper(this.__appSettings);
            this._manager = new LeadsManager(client);
            util = new Utils();
        }

        #region Listas

        [Route("ListaUsuarios")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseListaUsuarios> listaUsuarios()
        {
            LeadsResponseListaUsuarios response = new LeadsResponseListaUsuarios();
            try
            {

                string reqResponse = await request.doPost("Leads/listaUsuarios", "", HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseListaUsuarios>(reqResponse);
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

        [Route("ListaLandingPage")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCatalogoLandingPage> listaLandingPage()
        {
            LeadsResponseCatalogoLandingPage response = new LeadsResponseCatalogoLandingPage();
            try
            {

                string reqResponse = await request.doPost("Leads/listaLandingPage", "", HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCatalogoLandingPage>(reqResponse);
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

        [Route("ListaCatalogoCanalLead")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCatalogoCanalLead> listaCatalogoCanal()
        {
            LeadsResponseCatalogoCanalLead response = new LeadsResponseCatalogoCanalLead();
            try
            {

                string reqResponse = await request.doPost("Leads/listaCatalogoCanalLead", "", HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCatalogoCanalLead>(reqResponse);
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

        [Route("ListaCatalogoObservacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCatalogoObservacion> listaCatalogoObservacio()
        {
            LeadsResponseCatalogoObservacion response = new LeadsResponseCatalogoObservacion();
            try
            {

                string reqResponse = await request.doPost("Leads/listaCatalogoObservacion", "", HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCatalogoObservacion>(reqResponse);
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

        [Route("ListaFases")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCatalogoFaseLead> listafases()
        {
            LeadsResponseCatalogoFaseLead response = new LeadsResponseCatalogoFaseLead();
            try
            {

                string reqResponse = await request.doPost("Leads/listaCatalogoFaseLead", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCatalogoFaseLead>(reqResponse);
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

        [Route("ListaCatalogoTareas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCatalogoTareas> listaCatalogoTareas([FromBody] LeadsRequestEstatusFase item)
        {
            LeadsResponseCatalogoTareas response = new LeadsResponseCatalogoTareas();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/tareasListaFase", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCatalogoTareas>(reqResponse);
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

        [Route("ListadoLeads")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_01")]
        public async Task<LeadsResponseListadoLeads> listadoLeads([FromBody]LeadsRequestListadoLeads item)
        {
            LeadsResponseListadoLeads response = new LeadsResponseListadoLeads();
            DataRequestReply replyListadoLeads = new DataRequestReply();

            try
            {
                if(item.PrimeraCarga == true /*|| (item.primeraCarga == false && item.fechaInicio == "" && item.fechaFin == "")*/)
                {
                    DateTime date = DateTime.Now;
                    DateTime mesActual = date.AddDays(1);
                    DateTime tresMesesAtras = date.AddMonths(-2);
                    item.FechaFin = mesActual.ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                    item.FechaInicio = tresMesesAtras.ToString("dd/MM/yyyy") + " 00:00:00" + ".999";
                }
                               
                if (!string.IsNullOrEmpty(item.FechaInicio) && !string.IsNullOrEmpty(item.FechaFin))
                {

                    item.FechaInicio = DateTime.Parse(item.FechaInicio.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                    item.FechaFin = DateTime.Parse(item.FechaFin.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".999";
                }

                replyListadoLeads = _manager.GetListadoLeadsJefe(item);

                if (replyListadoLeads.CodigoRespuesta == 0)
                {

                    string aux = replyListadoLeads.Data.ToStringUtf8();

                    response.listaLeads = JsonConvert.DeserializeObject<List<ListadoLeads>>(replyListadoLeads.Data.ToStringUtf8());

                }
                else
                {
                    response.CodigoRespuesta = replyListadoLeads.CodigoRespuesta;
                    response.MensajeRespuesta = "No se pueden obtener los datos";
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

        [Route("ListadoHistorialTareas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseHistorialTareas> listadoHistorialTareas([FromBody]LeadsRequestHistorialTareas item)
        {
            LeadsResponseHistorialTareas response = new LeadsResponseHistorialTareas();
            try
            {
                
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/listadoHistorialTareas", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseHistorialTareas>(reqResponse);
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

        [Route("ActualizaLeadUsuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_01_01")]
        public async Task<ResponseModel> actualizaLeadUsuario([FromBody]LeadsRequestActualizaUsuarioLead item)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/actualizaLeadUsuario", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Leads",
                        Actividad = "Asignar lead ejecutivo",
                        Detalles = "Lead Asignado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLeads);
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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


        [Route("Reasignar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_01_01")]
        public async Task<ResponseModel> reasignarLead()
        {
            ResponseModel response = new ResponseModel();
            try
            {

                string rol = HttpContext.Session.GetString("rol");
                if (!string.IsNullOrEmpty(rol) && int.Parse(rol) == 9)
                {
                    response.CodigoRespuesta = 0;
                    response.MensajeRespuesta = "Autorizado";
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "No autorizado";
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

        [Route("TomarLeadEjecutivo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> tomarLeadEjecutivo([FromBody]LeadsRequestActualizaUsuarioLead item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                item.idUsuario  = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/actualizaLeadUsuario", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Leads",
                        Actividad = "Tomar lead ejecutivo",
                        Detalles = "Lead tomado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLeads);
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        [Route("ListadoLeadsEjecutivo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_02")]
        public async Task<LeadsResponseListadoLeadsEjecutivo> listadoLeadsEjecutivo([FromBody]LeadsRequestListadoLeads item)
        {
            LeadsResponseListadoLeadsEjecutivo response = new LeadsResponseListadoLeadsEjecutivo();
            DataRequestReply replyListadoLeads = new DataRequestReply();

            try
            {
                if (item.PrimeraCarga == true)
                {
                    DateTime date = DateTime.Now;
                    DateTime mesActual = date.AddDays(1);
                    DateTime tresMesesAtras = date.AddMonths(-2);
                    item.FechaFin = mesActual.ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                    item.FechaInicio = tresMesesAtras.ToString("dd/MM/yyyy") + " 00:00:00" + ".999";
                }

                item.IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));

                if (!string.IsNullOrEmpty(item.FechaInicio) && !string.IsNullOrEmpty(item.FechaFin))
                {

                    item.FechaInicio = DateTime.Parse(item.FechaInicio.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                    item.FechaFin = DateTime.Parse(item.FechaFin.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".999";
                }

                replyListadoLeads = _manager.GetListadoLeadsEjecutivo(item);

                if (replyListadoLeads.CodigoRespuesta == 0)
                {

                    string aux = replyListadoLeads.Data.ToStringUtf8();

                    response.listaLeads = JsonConvert.DeserializeObject<List<ListadoLeadsEjecutivo>>(replyListadoLeads.Data.ToStringUtf8());

                }
                else
                {
                    response.CodigoRespuesta = replyListadoLeads.CodigoRespuesta;
                    response.MensajeRespuesta = "No se pueden obtener los datos";
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

        [Route("ListaCorreos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseListadoCorreos> listadoCorreos()
        {
            LeadsResponseListadoCorreos response = new LeadsResponseListadoCorreos();
            try
            {
                
                string reqResponse = await request.doPost("Leads/listaCorreos", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseListadoCorreos>(reqResponse);
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

        [Route("ProcesoGetEstado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseProcesoGetEstado> procesoGetEstado()
        {
            LeadsResponseProcesoGetEstado response = new LeadsResponseProcesoGetEstado();
            try
            {

                string reqResponse = await request.doPost("Leads/procesoGetEstado", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {


                    response = JsonConvert.DeserializeObject<LeadsResponseProcesoGetEstado>(reqResponse);

                    List<Estado> listaAux = response.listaEstados;
                    var itemToRemove = listaAux.Single(x => x.id == 33);
                    listaAux.Remove(itemToRemove);

                    response.listaEstados = listaAux;

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


        [Route("listadoTipoEstablecimientos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseTipoEstablecimientos> listadoTipoEstablecimientos()
        {
            ResponseTipoEstablecimientos response = new ResponseTipoEstablecimientos();
            try
            {

                string reqResponse = await request.doPost("Leads/listadoTiposEstablecimientos", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseTipoEstablecimientos>(reqResponse);
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

        #region Inserciones


        [Route("CrearLead")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> crearLead([FromBody]LeadsRequestCrearLead item)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/crearLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        [Route("CrearLeadEjecutivo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCrearLead> crearLeadEjecutivo([FromBody]LeadsRequestCrearLead item)
        {
            LeadsResponseCrearLead response = new LeadsResponseCrearLead();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")); 
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/crearLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCrearLead>(reqResponse);
                    response.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
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

        [Route("ExisteLead")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseExisteLead> existeLead([FromBody]LeadsRequestExisteLead item)
        {
            LeadsResponseExisteLead response = new LeadsResponseExisteLead();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/existeLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseExisteLead>(reqResponse);
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

        [Route("EnviaCorreo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> enviaCorreo([FromBody]LeadsRequestEnviaCorreo item)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/enviaCorreo", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        [Route("CrearTarea")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseAgregaTarea> crearTarea([FromBody]LeadsRequestAgregaTarea item)
        {
            LeadsResponseAgregaTarea response = new LeadsResponseAgregaTarea();
            try
            {
                int year = int.Parse(item.fechaRecordatorio.Split('-')[0].ToString());
                int mes = int.Parse(item.fechaRecordatorio.Split('-')[1].ToString());
                int dia = int.Parse(item.fechaRecordatorio.Split('-')[2].ToString());

                int hora = int.Parse(item.horaRecordatorio.Split(':')[0].ToString());
                int minuto = int.Parse(item.horaRecordatorio.Split(':')[1].ToString());

                string fechaRecordatorio = new DateTime(year, mes, dia, hora, minuto, 0).ToString("yyyy-dd-MM HH:mm:ss"); ;
                item.fechaRecordatorio = fechaRecordatorio;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/crearTarea", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseAgregaTarea>(reqResponse);
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

        [Route("ActualizaTarea")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> actualizaTarea([FromBody]LeadsRequestActualizaTarea item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
               
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/actualizaTarea", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        [Route("ActualizaTareaStatus")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> actualizaTareaStatus([FromBody]LeadsRequestActualizaTarea item)
        {
            ResponseModel response = new ResponseModel();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/actualizaTareaStatus", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        [Route("CancelaLead")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> cancelaLead([FromBody]LeadsRequestCancelaLead item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                item.idFase = 6;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/cancelaLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Leads",
                        Actividad = "CancelaLead",
                        Detalles = request.mensajeRespuesta
                    });
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

        [Route("ActualizaFaseLeadCapacitacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> actualizaFaseLeadCapacitacion([FromBody]LeadsRequestActualizaFase item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                item.idEstatusFase = 4;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/actualizaFaseLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
                   
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

        [Route("ActualizaFaseLeadConcluir")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> actualizaFaseLeadConcluir([FromBody]LeadsRequestActualizaFase item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                item.idEstatusFase = 5;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/actualizaFaseLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        [Route("ReactivarLead")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> reactivarLead([FromBody]LeadsRequestReactivarLead item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/reactivarLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        [Route("ActualizaFaseLead")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> actualizaFaseLead([FromBody]LeadsRequestActualizaFase item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/actualizaFaseLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        #region Alta Sistema


        [Route("ProcesoAltaSistema")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> procesoAltaSistema([FromBody]LeadsRequestProcesoAltaSistema item)
        {
            LeadsResponseProcesoAltaSistema response = new LeadsResponseProcesoAltaSistema();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoAltaSistema", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseProcesoAltaSistema>(reqResponse);
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


        [Route("ProcesoCrearPrecadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> procesoCrearPrecadena([FromBody]LeadsRequestCrearPrecadena item)
        {
            LeadsResponseCrearPrecadena response = new LeadsResponseCrearPrecadena();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoCrearPrecadena", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCrearPrecadena>(reqResponse);
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


        [Route("ProcesoGetCiudad")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseProcesoGetCiudad> procesoGetCiudad([FromBody]LeadsRequestGetCiudad item)
        {
            LeadsResponseProcesoGetCiudad response = new LeadsResponseProcesoGetCiudad();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoGetCiudad", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseProcesoGetCiudad>(reqResponse);
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


        [Route("ProcesoDatosPrecadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseProcesoDatosPrecadena> procesoDatosPrecadena([FromBody]LeadsRequestDatosPrecadena item)
        {
            LeadsResponseProcesoDatosPrecadena response = new LeadsResponseProcesoDatosPrecadena();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoDatosPrecadena", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseProcesoDatosPrecadena>(reqResponse);
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


        [Route("ProcesoDatosCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseProcesoDatosCadena> procesoDatosCadena([FromBody]LeadsRequestDatosCadena item)
        {
            LeadsResponseProcesoDatosCadena response = new LeadsResponseProcesoDatosCadena();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoDatosCadena", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseProcesoDatosCadena>(reqResponse);
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


        [Route("ProcesoActualizaPrecadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> procesoActualizaPrecadena([FromBody]LeadsRequestActualizaPrecadena item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoActualizaPrecadena", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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

        [Route("ProcesoCrearCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCrearCadena> procesoCrearCadea([FromBody]LeadsRequestCrearCadena item)
        {
            LeadsResponseCrearCadena response = new LeadsResponseCrearCadena();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoCrearCadena", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCrearCadena>(reqResponse);
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

        [Route("ProcesoCrearEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCrearEstablecimiento> procesoCrearEstablecimiento([FromBody]LeadsRequestCrearEstablecimiento item)
        {
            LeadsResponseCrearEstablecimiento response = new LeadsResponseCrearEstablecimiento();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoCrearEstablecimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCrearEstablecimiento>(reqResponse);
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

        [Route("ProcesoDatosEstablecimientoCreado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseDatosEstablecimiento> procesoDatosEstablecimientoCreado([FromBody]LeadsRequestDatosEstablecimiento item)
        {
            LeadsResponseDatosEstablecimiento response = new LeadsResponseDatosEstablecimiento();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoDatosEstablecimientoCreado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseDatosEstablecimiento>(reqResponse);
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

        [Route("ProcesoDatosListaCajerosEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseListaCajerosEstablecimiento> procesoDatosListaCajerosEstablecimiento([FromBody]LeadsRequestCajerosLista item)
        {
            LeadsResponseListaCajerosEstablecimiento response = new LeadsResponseListaCajerosEstablecimiento();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoDatosListaCajerosEstablecimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseListaCajerosEstablecimiento>(reqResponse);
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

        [Route("ProcesoDatosCrearCajero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseCrearCajero> procesoDatosCrearCajero([FromBody]LeadsRequestCrearCajero item)
        {
            LeadsResponseCrearCajero response = new LeadsResponseCrearCajero();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoDatosCrearCajero", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseCrearCajero>(reqResponse);
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

        [Route("ProcesoObtenerIdEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseIdEstablecimiento> procesoObtenerIdEstablecimiento([FromBody]LeadsRequestIdEstablecimiento item)
        {
            LeadsResponseIdEstablecimiento response = new LeadsResponseIdEstablecimiento();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoObtenerIdEstablecimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseIdEstablecimiento>(reqResponse);
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

        [Route("ProcesoEliminarCajero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseEliminarCajero> procesoEliminarCajero([FromBody]LeadsRequestEliminarCajero item)
        {
            LeadsResponseEliminarCajero response = new LeadsResponseEliminarCajero();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoEliminarCajero", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseEliminarCajero>(reqResponse);
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

        [Route("ProcesoAltaTerminal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseAltaTerminal> procesoAltaTerminal([FromBody]LeadsRequestNuevaTerminal item)
        {
            LeadsResponseAltaTerminal response = new LeadsResponseAltaTerminal();
            try
            {
                item.version = string.Empty;
                item.tipoUsuario = 2;
                item.appMovil = false;
                item.ip = Seguridad.GetIPAddress();
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/procesoAltaTerminal", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseAltaTerminal>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Leads",
                        Actividad = "ProcesoAltaTerminal",
                        Detalles = "Terminal creada exitosamente",
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
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("ProcesoListaTiposTerminales")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<LeadsResponseListaTerminales> procesoListaTiposTerminales()
        {
            LeadsResponseListaTerminales response = new LeadsResponseListaTerminales();
            try
            {
                string reqResponse = await request.doPost("Leads/procesoListaTiposTerminales", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseListaTerminales>(reqResponse);
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

        [Route("ProcesoTerminalTipoCuatro")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<string> procesoTerminalTipoCuatro()
        {
            string terminal = string.Empty;
            try
            {
                DateTime StartDate = DateTime.Parse("01-01-1970");
                Random rnd = new Random();
                terminal = (2000000000 + long.Parse(Math.Round((DateTime.Now - StartDate).TotalSeconds).ToString())).ToString() + rnd.Next(1, 9).ToString() + rnd.Next(1, 9).ToString();
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                return "";
            }
            return terminal;
        }

        [Route("ProcesoConcluirAlta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> procesoConcluirAlta([FromBody]LeadsRequestActualizaFase item)
        {
            LeadsResponseAltaTerminal response = new LeadsResponseAltaTerminal();
            try
            {
                item.idEstatusFase = 3;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/actualizaFaseLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LeadsResponseAltaTerminal>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Leads",
                        Actividad = "ProcesoConcluirAlta",
                        Detalles = "Lead dado de alta en sistema",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLeads);
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

        #region Reportes


        [Route("ReportesMenu")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_03")]
        public async Task<Object> reportesMenu([FromBody]LeadsRequestDatosReportes item)
        {
            var response = new ResponseModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/reportesMenu", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    switch (item.menu)
                    {
                        case 1:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteLeadsFase>(reqResponse);
                            break;
                        case 2:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteConversion>(reqResponse);
                            break;
                        case 3:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteCancelados>(reqResponse);
                            break;
                        case 4:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteLeadsAlta>(reqResponse);
                            break;
                        case 5:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteLeadsEjecutivo>(reqResponse);
                            break;
                        case 6:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteLeadsEjecutivo>(reqResponse);
                            break;
                        case 7:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteCanales>(reqResponse);
                            break;
                        case 8:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteLeadsVentasMensuales>(reqResponse);
                            break;
                        case 9:
                            response = JsonConvert.DeserializeObject<LeadsResponseReporteLeadsVentasAcumulado>(reqResponse);
                            break;
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
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("ReporteExcelAlta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_03")]
        public async Task<LeadsResponseReporteLeadExcel> reporteExcelAlta([FromBody]LeadsRequestDatosReportes item)
        {
            LeadsResponseReporteLeadsAlta response = new LeadsResponseReporteLeadsAlta();
            LeadsResponseReporteLeadExcel responseExcel = new LeadsResponseReporteLeadExcel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/reportesMenu", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LeadsResponseReporteLeadsAlta>(reqResponse);

                    ExcelPackage workbook = new ExcelPackage();
                    workbook.Workbook.Worksheets.Add("Reporte Alta");
                    var hojaActual = workbook.Workbook.Worksheets["Reporte Alta"];
                    hojaActual = workbook.Workbook.Worksheets[0];


                    hojaActual.Cells["A1"].Value = "Id Cadena";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Ejecutivo";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Fecha Alta";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "Primer Deposito";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Venta";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "Saldo";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["A1:F1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (DatosLeadsAltas alta in response.reporteLeadsAltas)
                    {

                        hojaActual.Cells["A" + linea].Value = int.Parse(alta.idCadena.ToString());
                        hojaActual.Cells["B" + linea].Value = alta.ejecutivo.ToString();
                        hojaActual.Cells["C" + linea].Value = alta.fechaAlta.ToString();
                        hojaActual.Cells["D" + linea].Value = alta.primerDeposito.ToString();
                        hojaActual.Cells["E" + linea].Value = Convert.ToDecimal(alta.venta.ToString());
                        hojaActual.Cells["E" + linea].Style.Numberformat.Format = "###,###,##0.00";
                        hojaActual.Cells["F" + linea].Value = Convert.ToDecimal(alta.saldo.ToString());
                        hojaActual.Cells["F" + linea].Style.Numberformat.Format = "###,###,##0.00";

                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 6];
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

                        base64Excel = "data:text/plain;base64,"+Convert.ToBase64String(excelData);
                        

                    }

                    // retornar datos
                    responseExcel.CodigoRespuesta = 0;
                    responseExcel.reporteNombre = "ReporteAlta.xlsx";
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

        [Route("ReporteExcelEjecutivosMismoMes")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_03")]
        public async Task<LeadsResponseReporteLeadExcel> reporteExcelEjecutivosMismoMes([FromBody]LeadsRequestDatosReportes item)
        {
            LeadsResponseReporteLeadsEjecutivo response = new LeadsResponseReporteLeadsEjecutivo();
            LeadsResponseReporteLeadExcel responseExcel = new LeadsResponseReporteLeadExcel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/reportesMenu", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LeadsResponseReporteLeadsEjecutivo>(reqResponse);

                    ExcelPackage workbook = new ExcelPackage();
                    workbook.Workbook.Worksheets.Add("Reporte Ejecutivos Mismo Mes");
                    var hojaActual = workbook.Workbook.Worksheets["Reporte Ejecutivos Mismo Mes"];
                    hojaActual = workbook.Workbook.Worksheets[0];


                    hojaActual.Cells["A1"].Value = "Ejecutivo";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Total clientes activos";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Total clientes venta mayor";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "Suma ventas clientes activos";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["A1:D1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (DatosLeadsEjecutivos ejecutivo in response.reporteLeadsEjecutivo)
                    {

                        hojaActual.Cells["A" + linea].Value = ejecutivo.ejecutivo.ToString();
                        hojaActual.Cells["B" + linea].Value = int.Parse(ejecutivo.totalClientesActivos.ToString());
                        hojaActual.Cells["C" + linea].Value = int.Parse(ejecutivo.totalClientesVentaMayor500.ToString());
                        hojaActual.Cells["D" + linea].Value = ejecutivo.sumaVentaClientes.ToString();
                        hojaActual.Cells["D" + linea].Style.Numberformat.Format = "###,###,##0.00";

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
                    }

                    // retornar datos
                    responseExcel.CodigoRespuesta = 0;
                    responseExcel.reporteNombre = "ReporteEjecutivosMismoMes.xlsx";
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

        #region Migrar leads

        [Route("MigrarLeads")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_04")]
        public async Task<LeadsResponseMigrarLeads> migrarLeads([FromBody]LeadsRequestMigrarLeads item)
        {
            LeadsResponseMigrarLeads response = new LeadsResponseMigrarLeads();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/migrarLeads", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LeadsResponseMigrarLeads>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Leads",
                        Actividad = "MigrarLeads",
                        Detalles = "Leads migrados exitosamente",
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
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        #endregion

        #region Reporte Concentrado

        [Route("ReporteConcentrado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_05")]
        public async Task<LeadsResponseReporteConcentrado> reporteConcentrado([FromBody]LeadsRequestReporteConcentrado item)
        {
            LeadsResponseReporteConcentrado response = new LeadsResponseReporteConcentrado();
            try
            {
                item.fechaDesde = DateTime.Parse(item.fechaDesde).ToString("dd/MM/yyyy");
                item.fechaHasta = DateTime.Parse(item.fechaHasta).ToString("dd/MM/yyyy");
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/reporteConcentrado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LeadsResponseReporteConcentrado>(reqResponse);
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

        [Route("ReporteConcentradoExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_05")]
        public async Task<LeadsResponseReporteLeadExcel> reporteConcentradoExcel([FromBody] LeadsResponseReporteConcentrado item)
        {
            LeadsResponseReporteLeadExcel responseExcel = new LeadsResponseReporteLeadExcel();
            try
            {

                ExcelPackage workbook = new ExcelPackage();
                workbook.Workbook.Worksheets.Add("Reporte concentrado");
                workbook.Workbook.Worksheets.Add("Listado Ejecutivos");
                var hojaActual = workbook.Workbook.Worksheets["Reporte concentrado"];
                var hoja2 = workbook.Workbook.Worksheets["Listado Ejecutivos"];
                hojaActual = workbook.Workbook.Worksheets[0];
                hoja2 = workbook.Workbook.Worksheets[1];


                hojaActual.Cells["A1"].Value = "Lead";
                hojaActual.Cells["A1"].AutoFitColumns();
                hojaActual.Cells["B1"].Value = "MTCID";
                hojaActual.Cells["B1"].AutoFitColumns();
                hojaActual.Cells["C1"].Value = "Recomendado";
                hojaActual.Cells["C1"].AutoFitColumns();
                hojaActual.Cells["D1"].Value = "Canal";
                hojaActual.Cells["D1"].AutoFitColumns();
                hojaActual.Cells["E1"].Value = "Fase";
                hojaActual.Cells["E1"].AutoFitColumns();
                hojaActual.Cells["F1"].Value = "Cliente";
                hojaActual.Cells["F1"].AutoFitColumns();
                hojaActual.Cells["G1"].Value = "Teléfono";
                hojaActual.Cells["G1"].AutoFitColumns();
                hojaActual.Cells["H1"].Value = "Correo";
                hojaActual.Cells["H1"].AutoFitColumns();
                hojaActual.Cells["I1"].Value = "Registro";
                hojaActual.Cells["I1"].AutoFitColumns();
                hojaActual.Cells["J1"].Value = "Ejecutivo";
                hojaActual.Cells["J1"].AutoFitColumns();
                hojaActual.Cells["K1"].Value = "Observaciones";
                hojaActual.Cells["K1"].AutoFitColumns();
                hojaActual.Cells["L1"].Value = "Mensaje";
                hojaActual.Cells["L1"].AutoFitColumns();
                hojaActual.Cells["M1"].Value = "Comentario";
                hojaActual.Cells["M1"].AutoFitColumns();
                hojaActual.Cells["N1"].Value = "Tarea";
                hojaActual.Cells["N1"].AutoFitColumns();
                hojaActual.Cells["O1"].Value = "Estatus tarea";
                hojaActual.Cells["O1"].AutoFitColumns();
                hojaActual.Cells["P1"].Value = "Descripcion";
                hojaActual.Cells["P1"].AutoFitColumns();
                hojaActual.Cells["Q1"].Value = "Fecha alta cadena";
                hojaActual.Cells["Q1"].AutoFitColumns();
                hojaActual.Cells["R1"].Value = "Fecha ultimo cambio";
                hojaActual.Cells["R1"].AutoFitColumns();
                hojaActual.Cells["S1"].Value = "Giro";
                hojaActual.Cells["S1"].AutoFitColumns();
                hojaActual.Cells["T1"].Value = "Tipo Contacto";
                hojaActual.Cells["T1"].AutoFitColumns();
                hojaActual.Cells["U1"].Value = "Concepto Contacto";
                hojaActual.Cells["U1"].AutoFitColumns();
                hojaActual.Cells["V1"].Value = "Horario Contacto";
                hojaActual.Cells["V1"].AutoFitColumns();
                hojaActual.Cells["A1:V1"].Style.Font.Bold = true;
                int linea = 2;

                foreach (ListadoReporteConcentrado reporte in item.reporteConcentrado)
                {

                    hojaActual.Cells["A" + linea].Value = reporte.idLead.ToString();
                    hojaActual.Cells["B" + linea].Value = reporte.idCadena.ToString();
                    hojaActual.Cells["C" + linea].Value = reporte.idRecomendado.ToString();
                    hojaActual.Cells["D" + linea].Value = reporte.canal.ToString();
                    hojaActual.Cells["E" + linea].Value = reporte.fase.ToString();
                    hojaActual.Cells["F" + linea].Value = reporte.cliente.ToString();
                    hojaActual.Cells["G" + linea].Value = reporte.telefono.ToString();
                    hojaActual.Cells["H" + linea].Value = reporte.correo.ToString();
                    hojaActual.Cells["I" + linea].Value = reporte.fechaRegistro.ToString();
                    hojaActual.Cells["J" + linea].Value = reporte.ejecutivo.ToString();
                    hojaActual.Cells["K" + linea].Value = reporte.observaciones.ToString();
                    hojaActual.Cells["L" + linea].Value = reporte.mensaje.ToString();
                    hojaActual.Cells["M" + linea].Value = reporte.comentario.ToString();
                    hojaActual.Cells["N" + linea].Value = reporte.tarea.ToString();
                    hojaActual.Cells["O" + linea].Value = reporte.estatusTarea.ToString();
                    hojaActual.Cells["P" + linea].Value = reporte.descripcion.ToString();
                    hojaActual.Cells["Q" + linea].Value = reporte.fechaConcluido.ToString();
                    hojaActual.Cells["R" + linea].Value = reporte.fechaUltimoCambio.ToString();
                    hojaActual.Cells["S" + linea].Value = reporte.giro.ToString();
                    if (!string.IsNullOrEmpty(reporte.tipoContacto.ToString()))
                    {
                        hojaActual.Cells["T" + linea].Value = reporte.tipoContacto.ToString().Split('-')[0];
                        hojaActual.Cells["U" + linea].Value = reporte.tipoContacto.ToString().Split('-')[1];
                    }
                    else
                    {
                        hojaActual.Cells["T" + linea].Value = "";
                        hojaActual.Cells["U" + linea].Value = "";
                    }
                    hojaActual.Cells["V" + linea].Value = reporte.horarioContacto.ToString();

                    linea++;
                }

                var range = hojaActual.Cells[1, 1, linea - 1, 22];
                var table = hojaActual.Tables.Add(range, "table");
                //table.ShowTotal = true;
                table.TableStyle = TableStyles.Medium13;
                hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

                //Hoja 2 
                LeadsResponseListaUsuarios listaUsuarios = new LeadsResponseListaUsuarios();
                    string resp = await request.doPost("Leads/listaUsuarios", "", HttpContext.Session.GetString("token"));
                listaUsuarios = JsonConvert.DeserializeObject<LeadsResponseListaUsuarios>(resp);

                hoja2.Cells["A1"].Value = "Ejecutivo";
                hoja2.Cells["A1"].AutoFitColumns();
                hojaActual.Cells["A1:A1"].Style.Font.Bold = true;

                linea = 2;
                foreach (UsuarioVentas usuarioVenta in listaUsuarios.listaUsuarioVentas)
                {
                    hoja2.Cells["A" + linea].Value = usuarioVenta.usuario.ToString();

                    linea++;
                }

                var range2 = hoja2.Cells[1, 1, linea - 1, 1];
                var table2 = hoja2.Tables.Add(range2, "table2");
                //table.ShowTotal = true;
                table2.TableStyle = TableStyles.Medium13;
                hoja2.Cells[hoja2.Dimension.Address].AutoFitColumns();


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
                responseExcel.reporteNombre = "ReporteConcentrado.xlsx";
                responseExcel.reporteExcel = base64Excel;
                
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

        #region Asignar Lead

        [Route("asignarLead")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_06")]
        public async Task<ResponseModel> liberaBancos([FromBody] LeadsRequestAsignarLeads item)
        {
            LeadsRequestAsignarLeads response = new LeadsRequestAsignarLeads();
            try
            {
                DataTable dt = new DataTable();
                System.IO.StreamReader reader = null;
                //Se pasa el archivo a un DataTable
                if (!string.IsNullOrEmpty(item.archivo))
                {
                    if (item.ext == "xlsx" || item.ext == "xlsm" || item.ext == "xls" )
                    {
                        IExcelDataReader excelReader;
                        DataSet dsResult = new DataSet();
                        var contents = new StreamContent(new MemoryStream(Convert.FromBase64String(item.archivo)));
                        var a = new MemoryStream();
                        await contents.CopyToAsync(a);
                        reader = new StreamReader(a);
                        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                        if (item.ext == "xlsx" || item.ext == "xls")
                        {
                            excelReader = ExcelReaderFactory.CreateReader(reader.BaseStream);
                            dsResult = excelReader.AsDataSet();
                            dt = dsResult.Tables[0];
                        }
                        else if (item.ext == "xlsm")
                        {
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(reader.BaseStream);
                            dsResult = excelReader.AsDataSet();
                            dt = dsResult.Tables[0];
                        }

                        //Comienza la asignación 
                        LeadsResponseListaUsuarios listaUsuarios = new LeadsResponseListaUsuarios();
                        string resp = await request.doPost("Leads/listaUsuarios", "", HttpContext.Session.GetString("token"));
                        listaUsuarios = JsonConvert.DeserializeObject<LeadsResponseListaUsuarios>(resp);
                        DataTable dataFinal = new DataTable();
                        List<LeadsRequestActualizaUsuarioLead> listaLeads = new List<LeadsRequestActualizaUsuarioLead>();
                        dt.Rows[0].Delete();
                        dt.AcceptChanges();
                        foreach (DataRow dataRow in dt.Rows)
                        {
                            UsuarioVentas usuarioVentas = new UsuarioVentas();
                            usuarioVentas = listaUsuarios.listaUsuarioVentas.Find(x => x.usuario.Equals(dataRow[9].ToString()));
                            LeadsRequestActualizaUsuarioLead usuario = new LeadsRequestActualizaUsuarioLead();
                            if (usuarioVentas != null)
                            {
                                usuario.idUsuario = usuarioVentas.id;
                                usuario.idLead = int.Parse(dataRow[0].ToString());
                                listaLeads.Add(usuario);
                            }
                        }
                        string data = JsonConvert.SerializeObject(listaLeads);
                        resp = await request.doPost("Leads/asignarLead", data, HttpContext.Session.GetString("token"));
                        ResponseModel response1 = JsonConvert.DeserializeObject<ResponseModel>(resp);
                        response.CodigoRespuesta = response1.CodigoRespuesta;
                        response.MensajeRespuesta = response1.MensajeRespuesta;
                    }
                    else
                    {
                        response.CodigoRespuesta = -1;
                        response.MensajeRespuesta = "Archivo inválido";
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Archivo inválido";
                }

                var csv = new StringBuilder();
                var csvPlantilla = new StringBuilder();
                elemento result = null;


            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("canceladosFaseAnterior")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_06")]
        public async Task<ResponseFaseAnteriorLead> canceladosFaseAnterior([FromBody] LeadsRequestReactivarLead item)
        {
            ResponseFaseAnteriorLead response = new ResponseFaseAnteriorLead();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/canceladosFaseAnterior", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseFaseAnteriorLead>(reqResponse);
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

        #region Grupos Trabajo

        [Route("listaUsuarioPorRol")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_07")]
        public async Task<ResponseUsuarioRolModel> listaUsuarioPorRol([FromBody]LeadsRequestListaTipoRol item)
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

        [Route("GruposTrabajo/setGruposTrabajo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01_07")]
        public async Task<ResponseModel> setGruposTrabajo([FromBody] LeadsRequestGruposTrabajo item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/GruposTrabajo/setGruposTrabajo", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoCuentasBloqueo>(reqResponse);
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

        [Route("GruposTrabajo/getGruposTrabajo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01,PERM_01_07")]
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

        #endregion

        #region Estatus de Contacto

        [Route("EstatusContacto/getTipoContacto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseTiposContactos> getTipoContacto()
        {
            
            ResponseTiposContactos response = new ResponseTiposContactos();
            try
            {
                string reqResponse = await request.doPost("Leads/EstatusContacto/getTipoContacto", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseTiposContactos>(reqResponse);
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


        [Route("EstatusContacto/getConceptosTiposContactos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseConceptoTiposContactos> getConceptosTiposContactos([FromBody] LeadsRequestConceptosTiposContacto item)
        {
            ResponseConceptoTiposContactos response = new ResponseConceptoTiposContactos();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/EstatusContacto/getConceptosTiposContactos", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseConceptoTiposContactos>(reqResponse);
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



        [Route("EstatusContacto/updateContactoLead")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_01")]
        public async Task<ResponseModel> updateContactoLead([FromBody] LeadsRequestTipoContacto item)
        {

            ResponseModel response = new ResponseModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Leads/EstatusContacto/updateContactoLead", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
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