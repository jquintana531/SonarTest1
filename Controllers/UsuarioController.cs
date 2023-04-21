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
using Grpc.Net.Client;
using MTCenter.GRPC.GDBBO.DBProtos;
using Google.Protobuf.WellKnownTypes;
using System.Xml.Linq;
using MTC.WebApp.BackOffice.DataManager;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Usuario")]
    public class UsuarioController : Controller
    {
        private readonly GrpcChannel _channel;
        private readonly AppSettings __appSettings;
        private readonly UsuarioManager _manager;
        private RequestHelper request;

        public UsuarioController(IOptions<AppSettings> appSetings, DBData.DBDataClient client)
        {
            this.__appSettings = appSetings.Value;
            this.request = new RequestHelper(this.__appSettings);
            this._manager = new UsuarioManager(client);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login2")]

        public async Task<IActionResult> LoginAsync2([FromBody] Credential credenciales)
        {
            UserResponse response = new UserResponse();
            UsuarioResponseModel user = new UsuarioResponseModel();
            LoginData loginData = new LoginData();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                if (!string.IsNullOrEmpty(credenciales.Usuario) && !string.IsNullOrEmpty(credenciales.Password))
                {
                    reply = _manager.LoginManager(credenciales);

                    if (reply.CodigoRespuesta == 0)
                    {
                        string dataJson = reply.Data.ToStringUtf8();
                        List<LoginData> dbData = JsonConvert.DeserializeObject<List<LoginData>>(dataJson);


                        if (dbData != null && dbData.Count > 0)
                        {
                            loginData = dbData.ElementAt(0);

                            if (loginData.CodigoRespuesta == 0)
                            {
                                if (loginData.TipoAplicacion == 0 || loginData.TipoAplicacion == 2)
                                {
                                    
                                    user.IdUsuario = loginData.Claim_idUsuario;
                                    user.Nombre = loginData.Claim_nombre;
                                    user.ApellidoPaterno = loginData.Claim_aPaterno;
                                    user.ApellidoMaterno = loginData.Claim_aMaterno;
                                    user.IdRol = loginData.Claim_idRol;
                                    user.StatusClave = loginData.StatusClave;
                                    user.IdGrupoUsuario = loginData.Claim_idGrupoUsuario;
                                    user.IdDepartamento = loginData.Claim_idDepartamento;

                                    string XMLpermisos = loginData.Permisos.ToString();
                                    XDocument? doc = XDocument.Parse(XMLpermisos);

                                    List<string> permisos = doc.Root.Elements("codigo").Select(element => element.Value).ToList();

                                    foreach (string permiso in permisos)
                                    {
                                        user.Permisos.Add(permiso);
                                    }

                                    //Variables de sesión
                                    HttpContext.Session.SetString("userData", JsonConvert.SerializeObject(user));
                                    HttpContext.Session.SetString("idUsuario", user.IdUsuario.ToString());
                                    HttpContext.Session.SetString("rol", user.IdRol.ToString());
                                    HttpContext.Session.SetString("departamento", user.IdDepartamento.ToString());

                                    HttpContext.Session.SetString("roles", JsonConvert.SerializeObject(permisos));
                                    HttpContext.Session.SetString("user", credenciales.Usuario);
                                    HttpContext.Session.SetString("nombre", user.Nombre + " " + user.ApellidoPaterno + " " + user.ApellidoMaterno);
                                    //Variables de retorno

                                    response.CodigoRespuesta = 1;
                                    response.MensajeRespuesta = "Autentificación Correcta";
                                    response.Username = credenciales.Usuario;
                                    response.Fullname = user.Nombre + " " + user.ApellidoPaterno + " " + user.ApellidoMaterno;
                                    response.Roles = JToken.Parse(JsonConvert.SerializeObject(permisos));
                                }
                                else
                                {
                                    response.CodigoRespuesta = -1;
                                    response.MensajeRespuesta = "No autorizado";
                                }
                            }
                            else
                            {
                                if (reply.CodigoRespuesta == -512)
                                {
                                    HttpContext.Session.SetString("idUsuario", user.IdUsuario.ToString());
                                }

                                response.CodigoRespuesta = loginData.CodigoRespuesta;
                                response.MensajeRespuesta = loginData.MensajeRespuesta;
                            }

                        }
                        else
                        {
                            response.CodigoRespuesta = -101;
                            response.MensajeRespuesta = "Sin datos";
                        }

                    }
                    else
                    {
                        
                        response.CodigoRespuesta = reply.CodigoRespuesta;
                        response.MensajeRespuesta = "No se pueden obtener los datos";
                    }


                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Campos Invalidos";
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            finally
            {

            }
            return Ok(response);
        }

        [HttpGet]
        [IgnoreAntiforgeryToken]
        [Route("ValidateUser2")]
        public Boolean ValidateUser2()
        {
            if (HttpContext.Session.GetString("userData") != null)
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        [Route("ValidateAccess2")]
        public int ValidateAccess2([FromBody] String[] requiredRols)
        {
            if (HttpContext.Session.GetString("userData") != null)
            {
                var serial = HttpContext.Session.GetString("roles");
                String[] currentRols = JsonConvert.DeserializeObject<String[]>(serial);
                if (currentRols.Length > 0 && requiredRols.Length > 0)
                {
                    if (!requiredRols.Except(currentRols).Any())
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return -1;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CambioClave2")]
        public async Task<IActionResult> CambioClaveAsync2([FromBody] NewPasswordModel credenciales)
        {
            UserResponse response = new UserResponse();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                if (!string.IsNullOrEmpty(credenciales.PassAntigua) && !string.IsNullOrEmpty(credenciales.PassNueva))
                {

                    reply = _manager.CambioClaveManager(credenciales);

                    if (reply.CodigoRespuesta == 0)
                    {
                        var dbResult = JsonConvert.DeserializeObject<dynamic[]>(reply.Data.ToStringUtf8()).ElementAt(0);

                        if (dbResult.Column1 == -1)
                        {

                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        response.CodigoRespuesta = reply.CodigoRespuesta;
                        response.MensajeRespuesta = reply.MensajeRespuesta;
                    }
                }
                else
                {

                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Campos Invalidos";
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return Ok(response);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public UserResponse Login([FromBody]Credential credenciales)
        {
            UserResponse resp = new UserResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(credenciales));
                    credenciales.Password = "";
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), credenciales);
                    Task<string> taskResponse = request.doPost("Login/login", postData, null);
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                    if (request.codigoRespuesta == 0)
                    {
                        LoginResponse responseD = JsonConvert.DeserializeObject<LoginResponse>(response);
                        Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);


                        if (responseD.CodigoRespuesta == 0)
                        {

                            //Variables de sesión
                            HttpContext.Session.SetString("idUsuario", responseD.usuario.IdUsuario.ToString());
                            HttpContext.Session.SetString("token", responseD.authorization.Access_token.ToString());
                            HttpContext.Session.SetString("rol", responseD.usuario.IdRol.ToString());
                            HttpContext.Session.SetString("departamento", responseD.usuario.IdDepartamento.ToString());


                            //Permisos Leads                           PERM_01
                            //Listado de leads                         PERM_01_01
                            //Mis leads                                PERM_01_02
                            //Reportes                                 PERM_01_03
                            //Migrar leads                             PERM_01_04
                            //Reporte concentrado                      PERM_01_05


                            //Permisos Facturación                     PERM_03
                            //Facturación Interna                      PERM_03_01
                            //  Facturación (Buscar CFDI's)            PERM_03_01_01
                            //  Facturación (CFDI's Genericos)         PERM_03_01_02
                            //  Facturación (CFDI's Montos)            PERM_03_01_03
                            //  Facturación (Clientes)                 PERM_03_01_04
                            //  Facturación (Productos)                PERM_03_01_05
                            //  Facturación (Cancelar CFDI)            PERM_03_01_06
                            //Facturación POS                          PERM_03_02
                            //Administrar Receptores POS               PERM_03_03

                            //Permisos Administracion                  PERM_02
                            //Administrar Permisos                     PERM_02_01



                            //Roles Permitidos Facturación
                            //int[] rolesPermitidosFact = { 9, 3, 4, 5, 6, 10, 11, 13, 14, 26 };

                            List<string> permisos = responseD.usuario.Permisos;

                            HttpContext.Session.SetString("roles", JsonConvert.SerializeObject(permisos));
                            HttpContext.Session.SetString("user", credenciales.Usuario);
                            HttpContext.Session.SetString("nombre", responseD.usuario.Nombre+" "+ responseD.usuario.ApellidoPaterno + " "+ responseD.usuario.ApellidoMaterno);
                            //Variables de retorno

                            resp.CodigoRespuesta = 1;
                            resp.MensajeRespuesta = "Autentificación Correcta";
                            resp.Username = credenciales.Usuario;
                            resp.Fullname = responseD.usuario.Nombre + " " + responseD.usuario.ApellidoPaterno + " " + responseD.usuario.ApellidoMaterno;
                            resp.Roles = JToken.Parse(JsonConvert.SerializeObject(permisos));
                        }
                        else
                        {
                            if (responseD.CodigoRespuesta == -512)
                            {
                                HttpContext.Session.SetString("idUsuario", responseD.usuario.IdUsuario.ToString());
                            }
                            resp.CodigoRespuesta = responseD.CodigoRespuesta;
                            resp.MensajeRespuesta = responseD.MensajeRespuesta;
                        }

                    }
                    else
                    {

                        resp.CodigoRespuesta = request.codigoRespuesta;
                        resp.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    resp.CodigoRespuesta = -1;
                    resp.MensajeRespuesta = "Campos Invalidos";
                    throw new System.ArgumentException("Los datos ingresados son incorrectos", credenciales.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return resp;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CambioClave")]
        public UserResponse CambioClave([FromBody] NewPasswordModel credenciales)
        {
            UserResponse resp = new UserResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    credenciales.Usuario = Int32.Parse(HttpContext.Session.GetString("idUsuario"));
                    string postData = Seguridad.Encrypt(JsonConvert.SerializeObject(credenciales));
                    Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), credenciales);
                    Task<string> taskResponse = request.doPost("Login/login1", postData, null);
                    Task.WhenAll(taskLog, taskResponse);
                    string response = taskResponse.Result;
                }
                else
                {
                    resp.CodigoRespuesta = -1;
                    resp.MensajeRespuesta = "Campos Invalidos";
                    throw new System.ArgumentException("Los datos ingresados son incorrectos", credenciales.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
            }
            return resp;
        }


        [HttpPost]
        [Route("ValidateAccess")]
        public int ValidateAccess([FromBody] String[] requiredRols)
        {
            if (HttpContext.Session.GetString("token") !=null)
            {
                var serial = HttpContext.Session.GetString("roles");
                String[] currentRols = JsonConvert.DeserializeObject<String[]>(serial);
                if (currentRols.Length > 0 && requiredRols.Length > 0)
                {
                    if (!requiredRols.Except(currentRols).Any())
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return -1;
        }

        [HttpGet]
        [IgnoreAntiforgeryToken]
        [Route("ValidateUser")]
        public Boolean ValidateUser()
        {
            if (HttpContext.Session.GetString("token") != null)
            {
                return true;
            }
            return false;
        }
                

        [HttpGet]
        [IgnoreAntiforgeryToken]
        [Route("logout")]
        public Boolean logout()
        {
            HttpContext.Session.Clear();
            return true;
        }

    }

}