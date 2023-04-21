using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using Newtonsoft.Json;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Administracion")]
    public class AdministracionController : Controller
    {
        private readonly AppSettings __appSettings;
        private RequestHelper request;
        public AdministracionController(IOptions<AppSettings> appSetings)
        {
            this.__appSettings = appSetings.Value;
            this.request = new RequestHelper(this.__appSettings);
        }
        

        [HttpPost]
        [Route("PermisosVista")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_02")]
        public AdministracionPermisosResponse PermisosVista()
        {
            AdministracionPermisosResponse responseD = new AdministracionPermisosResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(new AdministracionRequestBUsuariosModel()));
                    RequestHelper requestU = new RequestHelper(this.__appSettings);
                    RequestHelper requestP = new RequestHelper(this.__appSettings);
                    RequestHelper requestD = new RequestHelper(this.__appSettings);

                    Task<string> taskResponseU = requestU.doPost("Administracion/BuscarUsuarios", postData, HttpContext.Session.GetString("token"));

                    Task<string> taskResponseP = requestP.doPost("Administracion/PermisosLista", null, HttpContext.Session.GetString("token"));

                    Task<string> taskResponseD = requestD.doPost("Administracion/DashboardsLista", null, HttpContext.Session.GetString("token"));

                    Task.WhenAll(taskResponseU, taskResponseP, taskResponseD);
                    string responseU = taskResponseU.Result;
                    string responseP = taskResponseP.Result;
                    string responseDa = taskResponseD.Result;

                    if (requestU.codigoRespuesta == 0 && requestP.codigoRespuesta == 0)
                    {
                        AdministracionBUsuariosResponse responseUO = JsonConvert.DeserializeObject<AdministracionBUsuariosResponse>(responseU);
                        AdministracionBPermisosResponse responsePO = JsonConvert.DeserializeObject<AdministracionBPermisosResponse>(responseP);
                        AdministracionDashboardsResponse responseDAS = JsonConvert.DeserializeObject<AdministracionDashboardsResponse>(responseDa);

                        var root = responsePO.respuesta.GenerateTree(c => c.Id_permiso, c => c.FK_Id_permiso);


                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseUO);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responsePO);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseDAS);

                        if (responseUO.CodigoRespuesta  == 0 && responsePO.CodigoRespuesta == 0)
                        {
                            responseD.CodigoRespuesta = 0;
                            responseD.MensajeRespuesta = "";
                            responseD.usuarios = responseUO.respuesta;
                            responseD.permisosGenerales = responsePO.respuesta;
                            responseD.dashboards = responseDAS.respuesta;
                            responseD.permisosJson = JsonConvert.SerializeObject(root);

                        }
                        else
                        {
                            throw new System.ArgumentException("Conexion Fallida: ", responseD.ToString());
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
        [Route("SetPermisos")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_02_01")]
        public ResponseModel SetPermisos([FromBody]AdministracionRequestSetPermisosModel busqueda)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuarioIn = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Administracion/SetPermisos", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta == 0)
                        {
                            responseD.MensajeRespuesta = "Permisos guardados exitosamente";
                        }
                        else if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al guardar permisos";
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
        [Route("SetDashboard")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_02_01")]
        public ResponseModel SetDashboard([FromBody]AdministracionRequestSetDashboardModel busqueda)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    busqueda.idUsuarioIn = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Administracion/SetDashboard", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta == 0)
                        {
                            responseD.MensajeRespuesta = "Dashboards guardados exitosamente";
                        }
                        else if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al guardar Dashboards";
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
        [Route("GetDashboardInfo")]
        [ValidateAntiForgeryToken]
        
        public AdministracionDashboardInfoResponse GetDashboardInfo([FromBody]AdministracionRequestInfoDashboardModel req)
        {
            AdministracionDashboardInfoResponse responseD = new AdministracionDashboardInfoResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    req.idUsuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(req));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), req);
                    Task<string> taskResponse = request.doPost("Administracion/WidgetSelected", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<AdministracionDashboardInfoResponse>(response);
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
        [Route("GetPermisosPorRol")]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_02_01")]
        public AdministracionPermisosRolResponse GetPermisosRol([FromBody] AdministracionRequestPermisosRol busqueda)
        {
            AdministracionPermisosRolResponse responseD = new AdministracionPermisosRolResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(busqueda));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), busqueda);
                    Task<string> taskResponse = request.doPost("Administracion/GetPermisosPorRol", postData, HttpContext.Session.GetString("token"));
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        responseD = JsonConvert.DeserializeObject<AdministracionPermisosRolResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);
                        if (responseD.CodigoRespuesta == 0)
                        {
                            responseD.MensajeRespuesta = "Permisos obtenidos.";
                        }
                        else if (responseD.CodigoRespuesta < 0)
                        {
                            responseD.MensajeRespuesta = "Error al obtener permisos";
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
    }
}