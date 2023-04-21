using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MTC.WebApp.BackOffice.DataManager;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using MTCenter.GRPC.GDBBO.DBProtos;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Utilities;
using OfficeOpenXml.Table;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Liberaciones")]
    public class LiberacionesController : Controller
    {
        private readonly AppSettings __appSettings;
        private readonly ConfigurationSettings __configurations;
        private RequestHelper request;
        private Utils util;
        private readonly LiberacionesManager _manager;
        static DataTable dtBancomer = new DataTable();
        private readonly IWebHostEnvironment _hostingEnvironment;


        public LiberacionesController(IOptions<AppSettings> appSetings, IOptions<ConfigurationSettings> configurations, IWebHostEnvironment hostEnvironment, DBData.DBDataClient client)
        {
            this.__appSettings = appSetings.Value;
            this.__configurations = configurations.Value;
            this.request = new RequestHelper(this.__appSettings);
            this._manager = new LiberacionesManager(client);
            util = new Utils();
            _hostingEnvironment = hostEnvironment;

        }

        #region Listado Depositos

        [Route("ListadoDepositos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_01")]
        public Task<LiberacionesResponseListadoDepositos> ListadoDepositos()
        {
            LiberacionesResponseListadoDepositos response = new LiberacionesResponseListadoDepositos();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                LiberacionesRequestListadoDepositos item = new LiberacionesRequestListadoDepositos();
                item.Usuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.Rol = int.Parse(HttpContext.Session.GetString("rol"));

                reply = _manager.GetListadoDepositos(item);

                if (reply.CodigoRespuesta == 0)
                {
                    response.listaDepositos = JsonConvert.DeserializeObject<List<Depositos>>(reply.Data.ToStringUtf8());
                    response.CodigoRespuesta = 0;
                    response.MensajeRespuesta = "";
                }
                else
                {
                    response.CodigoRespuesta = reply.CodigoRespuesta;
                    response.MensajeRespuesta = "No se pueden obtener los datos";
                }

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = "Error de aplicación MTCenter";
            }
            return Task.FromResult(response);
        }

        [Route("DetallesDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_01,PERM_04_01_01")]
        public async Task<LiberacionesResponseDetalleDepositos> DetalleDepositos([FromBody]LiberacionesRequestDetalleDepositos item)
        {
            LiberacionesResponseDetalleDepositos response = new LiberacionesResponseDetalleDepositos();
            DataRequestReply replyDetalle = new DataRequestReply();
            DataRequestReply replyContador = new DataRequestReply();

            try
            {
                item.IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.IdDeposito = int.Parse(item.IdDepositoStr);

                List<Task> depositosTasks = new List<Task>
                {
                    Task.Run(() => { replyDetalle = _manager.DetalleDepositos(item); }),
                    Task.Run(() => { replyContador = _manager.ContadorDepositosErroneos(item); })
                };

                Task.WaitAll(depositosTasks.ToArray());

                if (replyDetalle.CodigoRespuesta == 0)
                {

                    var listadoDepositos = JsonConvert.DeserializeObject<List<DetalleDeposito>>(replyDetalle.Data.ToStringUtf8());
                    var dataContador = JsonConvert.DeserializeObject<dynamic[]>(replyContador.Data.ToStringUtf8()).ElementAt(0);


                    if (listadoDepositos.Count > 0)
                    {
                        response.DetalleDeposito = listadoDepositos.ElementAt(0);
                        response.DetalleDeposito.Ficha = util.FichaDeposito(__appSettings, response.DetalleDeposito.Id);
                        response.DetalleDeposito.FechaDeposito = util.FormatFechaT(response.DetalleDeposito.FechaDeposito);
                        response.TotalDepositos = dataContador.Total;
                        response.CodigoRespuesta = 0;
                        response.MensajeRespuesta = string.Empty;
                    }
                    else
                    {
                        response.CodigoRespuesta = -1;
                        response.MensajeRespuesta = "No se puede obtener el detalle";
                    }

                }
                else
                {
                    response.CodigoRespuesta = replyDetalle.CodigoRespuesta;
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

        [Route("CuentaLiberacionAutomatica")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_01,PERM_04_01_01")]
        public async Task<LiberacionesResponseLiberacionAutomatica> cuentaLiberacionAutomatica([FromBody]LiberacionesRequestLiberacionAutomatica item)
        {
            LiberacionesResponseLiberacionAutomatica response = new LiberacionesResponseLiberacionAutomatica();
            try
            {
                item.idCuenta = int.Parse(item.idCuentaStr);

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/ListadoDeposito/cuentaLiberacionAutomatica", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseLiberacionAutomatica>(reqResponse);
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

        [Route("DepositosCuenta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_01,PERM_04_01_01")]
        public async Task<LiberacionesResponseDepositosCuenta> depositosCuenta([FromBody]LiberacionesRequestDepositosCuenta item)
        {
            LiberacionesResponseDepositosCuenta response = new LiberacionesResponseDepositosCuenta();
            try
            {
                

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/ListadoDeposito/depositosCuenta", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosCuenta>(reqResponse);
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

        [Route("AutorizaDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_01,PERM_04_01_01")]
        public async Task<LiberacionesResponseAutorizaDeposito> autorizaDeposito([FromBody]LiberacionesRequestAutorizaDeposito item)
        {
            LiberacionesResponseAutorizaDeposito response = new LiberacionesResponseAutorizaDeposito();
            try
            {
                if (!string.IsNullOrEmpty(item.idDepositoStr) && !string.IsNullOrEmpty(item.idDepositoCuentaStr))
                {
                    item.idDeposito = int.Parse(item.idDepositoStr);
                    if (item.idDepositoCuentaStr == "0")
                    {
                        item.idDepositoCuenta = 0;
                    }
                    else
                    {
                        item.idDepositoCuenta = int.Parse(item.idDepositoCuentaStr);
                    }

                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                    string reqResponse = await request.doPost("Liberaciones/ListadoDeposito/autorizaDeposito", data, HttpContext.Session.GetString("token"));


                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseAutorizaDeposito>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "AutorizaDeposito",
                            Detalles = "DepositoAutorizado",
                            Objeto = JsonConvert.SerializeObject(item)
                        });

                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        [Route("RechazaDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_01,PERM_04_01_01")]
        public async Task<LiberacionesResponseRechazaDeposito> rechazaDeposito([FromBody]LiberacionesRequestRechazaDeposito item)
        {
            LiberacionesResponseRechazaDeposito response = new LiberacionesResponseRechazaDeposito();
            try
            {
                if (!string.IsNullOrEmpty(item.idDepositoStr))
                {
                    item.idDeposito = int.Parse(item.idDepositoStr);
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                    string reqResponse = await request.doPost("Liberaciones/ListadoDeposito/rechazaDeposito", data, HttpContext.Session.GetString("token"));


                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseRechazaDeposito>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "RechazaDeposito",
                            Detalles = "Deposito Rechazado",
                            Objeto = JsonConvert.SerializeObject(item)
                        });
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        #region Listado Depositos Bloqueados

        [Route("listadoDepositosBloqueados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_02")]
        public async Task<LiberacionesResponseListadoDepositosBloqueados> listadoDepositosBloqueados([FromBody]LiberacionesRequestListadoDepositosBloqueados item)
        {
            LiberacionesResponseListadoDepositosBloqueados response = new LiberacionesResponseListadoDepositosBloqueados();
            try
            {
                item.usuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.rol = int.Parse(HttpContext.Session.GetString("rol"));
                item.bloqueados = true;

                if (!string.IsNullOrEmpty(item.desde) && !string.IsNullOrEmpty(item.hasta))
                {
                    item.desde = DateTime.Parse(item.desde).ToString("dd/MM/yyyy");
                    item.hasta = DateTime.Parse(item.hasta).ToString("dd/MM/yyyy");
                }

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/DepositoBloqueado/listado", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosBloqueados>(reqResponse);
                    
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

        [Route("listadoFormasPago")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_02")]
        public async Task<LiberacionesResponseListadoDepositosBloqueadosFormasPago> listadoFormasPago()
        {
            LiberacionesResponseListadoDepositosBloqueadosFormasPago response = new LiberacionesResponseListadoDepositosBloqueadosFormasPago();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositoBloqueado/listadoFormasPago", "", HttpContext.Session.GetString("token"));
                
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosBloqueadosFormasPago>(reqResponse);
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

        [Route("listadoCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_02")]
        public async Task<LiberacionesResponseListadoDepositosBloqueadosCuentas> listadoCuentas()
        {
            LiberacionesResponseListadoDepositosBloqueadosCuentas response = new LiberacionesResponseListadoDepositosBloqueadosCuentas();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositoBloqueado/listadoCuentas", "", HttpContext.Session.GetString("token"));
                
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosBloqueadosCuentas>(reqResponse);
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

        [Route("DesbloqueaDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_02,PERM_04_02_02")]
        public async Task<LiberacionesResponseListadoDepositosBloqueadosDesbloquea> desbloqueaDeposito([FromBody]LiberacionesRequestListadoDepositosDesbloquea item)
        {
            LiberacionesResponseListadoDepositosBloqueadosDesbloquea response = new LiberacionesResponseListadoDepositosBloqueadosDesbloquea();
            try
            {
                if (!string.IsNullOrEmpty(item.idStr) )
                {
                   
                    item.id = int.Parse(item.idStr);
                    item.rol = int.Parse(HttpContext.Session.GetString("rol"));
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();

                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                    string reqResponse = await request.doPost("Liberaciones/DepositoBloqueado/desbloqueaDeposito", data, HttpContext.Session.GetString("token"));


                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosBloqueadosDesbloquea>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "DesbloqueaDeposito",
                            Detalles = "Deposito Desbloqueado",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        [Route("EditaDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_02,PERM_04_02_01")]
        public async Task<LiberacionesResponseListadoDepositosBloqueadosEdita> editaDeposito([FromBody]LiberacionesRequestListadoDepositosEdita item)
        {
            LiberacionesResponseListadoDepositosBloqueadosEdita response = new LiberacionesResponseListadoDepositosBloqueadosEdita();
            try
            {
                if (!string.IsNullOrEmpty(item.idStr))
                {

                    item.id = int.Parse(item.idStr);
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();

                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                    string reqResponse = await request.doPost("Liberaciones/DepositoBloqueado/editaDeposito", data, HttpContext.Session.GetString("token"));


                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosBloqueadosEdita>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "EditaDeposito",
                            Detalles = "Deposito Editado",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        #region Nuevo deposito

        [Route("NuevoDeposito/consultaCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_03")]
        public async Task<LiberacionesResponseInformacionCadenaDeposito> nuevoDepositoConsultaCadena([FromBody]LiberacionesRequestInformacionCadena item)
        {
            LiberacionesResponseInformacionCadenaDeposito response = new LiberacionesResponseInformacionCadenaDeposito();
            try
            {
                if (item.idCadena > 0)
                {
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/NuevoDeposito/consultaCadena", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseInformacionCadenaDeposito>(reqResponse);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        [Route("NuevoDeposito/nuevoDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_03")]
        public async Task<LiberacionesResponseNuevoDeposito> nuevoDepositoManual([FromBody]LiberacionesRequesrNuevoDepositoManual item)
        {
            LiberacionesResponseNuevoDeposito response = new LiberacionesResponseNuevoDeposito();
            bool montoValidoVentas = true;
            try
            {
                if (item.idCadena > 0)
                {
                    if (int.Parse(HttpContext.Session.GetString("departamento")) == 14 /*ventas*/)
                    {
                        if (float.Parse(item.monto.ToString()) > this.__configurations.montoMaximoDepositos)
                        {
                            montoValidoVentas = false;
                        }
                    }

                    if (montoValidoVentas)
                    {
                        if (!string.IsNullOrEmpty(item.folioFiscal))
                        {
                            item.observaciones = item.folioFiscal + "~" + item.observaciones ;
                        }

                        item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                        string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                        string reqResponse = await request.doPost("Liberaciones/NuevoDeposito/nuevoDeposito", data, HttpContext.Session.GetString("token"));

                        if (request.codigoRespuesta == 0)
                        {
                            response = JsonConvert.DeserializeObject<LiberacionesResponseNuevoDeposito>(reqResponse);

                            if (response.CodigoRespuesta == 0 && response.registrado)
                            {
                                LiberacionesRequestListadoEstatusDepositos newItem = new LiberacionesRequestListadoEstatusDepositos();
                                newItem.desde = DateTime.Now.ToString("dd/MM/yyyy") + " 00:00:00";
                                newItem.hasta = DateTime.Now.ToString("dd/MM/yyyy") + " 23:59:59";
                                newItem.idCadena = item.idCadena.ToString();
                                newItem.idEstatus = 3;
                                newItem.usuario = "";
                                newItem.monto = item.monto.ToString();
                                newItem.autorizacion = item.referencia2;
                                newItem.idCatalogoCuenta = item.idCatalogoCuenta;
                                newItem.idCatalogoFormaPago = 0;
                                newItem.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                                newItem.ip = Seguridad.GetIPAddress();

                                data = Seguridad.Encrypt(JsonConvert.SerializeObject(newItem));
                                reqResponse = await request.doPost("Liberaciones/EstatusDepositos/listadoEstatusDepositos", data, HttpContext.Session.GetString("token"));

                                LiberacionesResponseListadoEstatusDepositos listadoResponse = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDepositos>(reqResponse);
                                response.listadoEstatusDepositos = listadoResponse.listadoEstatusDepositos;
                            }
                            else
                            {
                                response.listadoEstatusDepositos = new List<DepositosEstatus>();
                            }

                            util.logActividadUsuario(new ActividadUsuario
                            {
                                IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                                Usuario = HttpContext.Session.GetString("user"),
                                Seccion = "Liberaciones",
                                Actividad = "NuevoDeposito/nuevoDeposito",
                                Detalles = "Deposito guardado",
                                Objeto = JsonConvert.SerializeObject(item)
                            },__appSettings.LoggerLiberaciones);
                        }
                        else
                        {
                            response.CodigoRespuesta = request.codigoRespuesta;
                            response.MensajeRespuesta = request.mensajeRespuesta;
                        }
                    }
                    else
                    {
                        response.CodigoRespuesta = -1;
                        response.MensajeRespuesta = "Restriccion - El monto de depósito no pude ser mayor a: " + this.__configurations.montoMaximoDepositos;
                    }

                    
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        [Route("NuevoDeposito/datosNotaFiscal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_03")]
        public async Task<LiberacionesResponseDatosNotaFiscal> datosNotaFiscal([FromBody]LiberacionesRequestInformacionCadena item)
        {
            LiberacionesResponseDatosNotaFiscal response = new LiberacionesResponseDatosNotaFiscal();
            try
            {
                if (item.idCadena > 0)
                {
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/NuevoDeposito/datosNotaFiscal", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseDatosNotaFiscal>(reqResponse);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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
        
        [Route("NuevoDeposito/listadoFormasPago")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_03")]
        public async Task<LiberacionesResponseListadoEstatusDepositosFormasPago> nuevoDepositoListadoFormasPagoEstatusDepositos()
        {
            LiberacionesResponseListadoEstatusDepositosFormasPago response = new LiberacionesResponseListadoEstatusDepositosFormasPago();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/NuevoDeposito/listadoFormasPago", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDepositosFormasPago>(reqResponse);
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

        [Route("NuevoDeposito/listadoCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_03")]
        public async Task<LiberacionesResponseListadoEstatusDepositosCuentas> nuevoDepositoListadoCuentasEstatusDepositos()
        {
            LiberacionesResponseListadoEstatusDepositosCuentas response = new LiberacionesResponseListadoEstatusDepositosCuentas();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/NuevoDeposito/listadoCuentas", "", HttpContext.Session.GetString("token"));
                 
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDepositosCuentas>(reqResponse);
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

        #region Subir deposito


        [Route("SubirDepositos/insertaDepositosBancos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_11")]
        public async Task<LiberacionesResponseSubirDepositos> insertaDepositosBancos([FromBody] LiberacionesRequestSubirDepositos item)
        {
            LiberacionesResponseSubirDepositos response = new LiberacionesResponseSubirDepositos();
            try
            {
                response.insertado = false;
                if (!string.IsNullOrEmpty(item.archivo))
                {
                    if (item.ext == "txt" || item.ext == "csv")
                    {
                        if (item.ext == "txt")
                        {
                            //System.IO.File.WriteAllBytes(@"E:\archivos_bancos\EDODECTA", Convert.FromBase64String(item.archivo));
                            System.IO.File.WriteAllBytes(@"" + __appSettings.ArchivosBancosDir + "EDODECTA", Convert.FromBase64String(item.archivo));
                            response.insertado = true;
                        }
                        else if (item.ext == "csv")
                        {
                            //System.IO.File.WriteAllBytes(@"E:\archivos_bancos\" + item.nombre +"."+ item.ext, Convert.FromBase64String(item.archivo));
                            System.IO.File.WriteAllBytes(@"" + __appSettings.ArchivosBancosDir + item.nombre +"."+ item.ext, Convert.FromBase64String(item.archivo));

                            if (this.__configurations.cargaDepositosBD == 1)
                            {
                                item.archivo = item.nombre;
                                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                                item.ip = "";
                                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                                string reqResponse = await request.doPost("Liberaciones/SubirDepositos/insertaDepositosBancos", data, HttpContext.Session.GetString("token"));

                                if (request.codigoRespuesta == 0)
                                {
                                    response.CodigoRespuesta = 0;
                                    response = JsonConvert.DeserializeObject<LiberacionesResponseSubirDepositos>(reqResponse);
                                    response.MensajeRespuesta = "El archivo " + item.nombre + " " + item.ext + " se guardo con exito en la Base de Datos";
                                    util.logActividadUsuario(new ActividadUsuario
                                    {
                                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                                        Usuario = HttpContext.Session.GetString("user"),
                                        Seccion = "Liberaciones/SubirDepositos",
                                        Actividad = "insertaDepositosBancos",
                                        Detalles = request.mensajeRespuesta
                                    }, __appSettings.LoggerLiberaciones);
                                    response.insertado = true;
                                }
                                else
                                {
                                    response.CodigoRespuesta = request.codigoRespuesta;
                                    response.MensajeRespuesta = "No fue posible guardar el archivo " + item.nombre + " " + item.ext;
                                }
                                
                            }
                            else
                            {
                                /*Solo sube el archivo*/
                                response.CodigoRespuesta = 0;
                                response.MensajeRespuesta = "Archivo "+item.nombre+" "+item.ext+". Se guardo con exito";
                            }
                            
                        }
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

        #region Información de clientes

        [Route("InformacionClientes")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_04")]
        public async Task<LiberacionesResponseInformacionCadena> informacionClientes([FromBody]LiberacionesCadenaRequestInformacionCadena item)
        {
            LiberacionesResponseInformacionCadena response = new LiberacionesResponseInformacionCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.tipoUsuario = 2;
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/InformacionClientes/informacion", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseInformacionCadena>(reqResponse);

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
        
        [Route("InformacionClientes/cajeros")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_04")]
        public async Task<LiberacionesResponseInformacionCadenaListadoCajeros> informacionClientesCajeros([FromBody]LiberacionesCadenaRequestListadoCajeros item)
        {
            LiberacionesResponseInformacionCadenaListadoCajeros response = new LiberacionesResponseInformacionCadenaListadoCajeros();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/InformacionClientes/informacion/cajeros", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseInformacionCadenaListadoCajeros>(reqResponse);

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

        [Route("InformacionClientes/terminales")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_04")]
        public async Task<LiberacionesResponseInformacionCadenaListadoTerminales> informacionClientesTerminales([FromBody]LiberacionesCadenaRequestListadoTerminales item)
        {
            LiberacionesResponseInformacionCadenaListadoTerminales response = new LiberacionesResponseInformacionCadenaListadoTerminales();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/InformacionClientes/informacion/terminales", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseInformacionCadenaListadoTerminales>(reqResponse);

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

        [Route("InformacionClientes/detalleEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04,PERM_04_04")]
        public async Task<LiberacionesResponseInformacionCadenaDetalleEstablecimiento> informacionDetalleEstablecimiento([FromBody]LiberacionesCadenaRequestDetalleEstablecimiento item)
        {
            LiberacionesResponseInformacionCadenaDetalleEstablecimiento response = new LiberacionesResponseInformacionCadenaDetalleEstablecimiento();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/InformacionClientes/informacion/detalleEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseInformacionCadenaDetalleEstablecimiento>(reqResponse);

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

        #region Saldo a credito

        [Route("SaldoCredito/listaCadenasCredito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_13")]
        public async Task<LiberacionesResponseListadoCadenasCredito> informacionClientesCajeros()
        {
            LiberacionesResponseListadoCadenasCredito response = new LiberacionesResponseListadoCadenasCredito();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/SaldoCredito/listaCadenasCredito", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoCadenasCredito>(reqResponse);

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

        [Route("SaldoCredito/saldoCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_13")]
        public async Task<LiberacionesResponseSaldoCadena> saldoCadena([FromBody] LiberacionesRequestSaldoCadena item)
        {
            LiberacionesResponseSaldoCadena response = new LiberacionesResponseSaldoCadena();
            try
            {
                if (item.idCadena != 0 )
                {
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                    string reqResponse = await request.doPost("Liberaciones/SaldoCredito/saldoCadena", data, HttpContext.Session.GetString("token"));


                    if (request.codigoRespuesta == 0)
                    {

                        response = JsonConvert.DeserializeObject<LiberacionesResponseSaldoCadena>(reqResponse);

                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        [Route("SaldoCredito/traspasoSaldoCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_13")]
        public async Task<LiberacionesResponseTraspasoSaldoCredito> traspasoSaldoCadena([FromBody] LiberacionesRequestTransfiereSaldoCredito item)
        {
            LiberacionesResponseTraspasoSaldoCredito response = new LiberacionesResponseTraspasoSaldoCredito();
            try
            {
                if (item.monto != 0)
                {
                    if (item.idCadena != 0)
                    {
                        item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                        item.tipoUsuario = 2;
                        item.ip = Seguridad.GetIPAddress();

                        string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                        string reqResponse = await request.doPost("Liberaciones/SaldoCredito/traspasoSaldoCadena", data, HttpContext.Session.GetString("token"));
                        
                        if (request.codigoRespuesta == 0)
                        {

                            response = JsonConvert.DeserializeObject<LiberacionesResponseTraspasoSaldoCredito>(reqResponse);

                            util.logActividadUsuario(new ActividadUsuario
                            {
                                IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                                Usuario = HttpContext.Session.GetString("user"),
                                Seccion = "Liberaciones",
                                Actividad = "SaldoCredito/traspasoSaldoCadena",
                                Detalles = "Traspaso realizado",
                                Objeto = JsonConvert.SerializeObject(item)
                            }, __appSettings.LoggerLiberaciones);

                        }
                        else
                        {
                            response.CodigoRespuesta = request.codigoRespuesta;
                            response.MensajeRespuesta = request.mensajeRespuesta;
                        }
                    }
                    else
                    {
                        response.CodigoRespuesta = -1;
                        response.MensajeRespuesta = "No se ha especificado la cadena";
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Sin monto";
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

        #region Estatus deposito
        
        [Route("EstatusDeposito/listadoEstatusDepositos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_05")]
        public async Task<LiberacionesResponseListadoEstatusDepositos> listadoEstatusDepositos([FromBody] LiberacionesRequestListadoEstatusDepositos item)
        {
            LiberacionesResponseListadoEstatusDepositos response = new LiberacionesResponseListadoEstatusDepositos();
            try
            {
                if ((!string.IsNullOrEmpty(item.desde) && (!string.IsNullOrEmpty(item.hasta))) && (DateTime.Parse(item.desde.Split("T")[0]) <= DateTime.Parse(item.desde.Split("T")[0])))
                {
                    item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                    item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".999";
                    //item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59" + ".999";
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();

                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/EstatusDepositos/listadoEstatusDepositos", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {

                        response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDepositos>(reqResponse);
                        if (int.Parse(HttpContext.Session.GetString("departamento")) == 14 && int.Parse(HttpContext.Session.GetString("rol")) != 9)
                        {
                            foreach (DepositosEstatus dep in response.listadoEstatusDepositos)
                            {
                                dep.ficha = "";
                            }
                        }                     

                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
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
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("EstatusDeposito/listadoFormasPago")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_05")]
        public async Task<LiberacionesResponseListadoEstatusDepositosFormasPago> listadoFormasPagoEstatusDepositos()
        {
            LiberacionesResponseListadoEstatusDepositosFormasPago response = new LiberacionesResponseListadoEstatusDepositosFormasPago();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/EstatusDepositos/listadoFormasPago", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDepositosFormasPago>(reqResponse);
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

        [Route("EstatusDeposito/listadoCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_05")]
        public async Task<LiberacionesResponseListadoEstatusDepositosCuentas> listadoCuentasEstatusDepositos()
        {
            LiberacionesResponseListadoEstatusDepositosCuentas response = new LiberacionesResponseListadoEstatusDepositosCuentas();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/EstatusDepositos/listadoCuentas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDepositosCuentas>(reqResponse);
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

        [Route("EstatusDeposito/listadoEstatus")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_05")]
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

        [Route("EstatusDeposito/detalleDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_05")]
        public async Task<LiberacionesResponseDetalleDeposito> detalleDeposito([FromBody] LiberacionesRequestDetalleDeposito item)
        {
            LiberacionesResponseDetalleDeposito response = new LiberacionesResponseDetalleDeposito();
            try
            {
                
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/EstatusDepositos/detalleDeposito", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDetalleDeposito>(reqResponse);
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

        [Route("EstatusDeposito/editarDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_05,PERM_04_05_1")]
        public async Task<LiberacionesResponseEstatusEditDeposito> editarDeposito([FromBody] LiberacionesRequestEstatusEditaDeposito item)
        {
            LiberacionesResponseEstatusEditDeposito response = new LiberacionesResponseEstatusEditDeposito();
            try
            {
                
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/EstatusDepositos/editarDeposito", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseEstatusEditDeposito>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "EstatusDeposito/editarDeposito",
                        Detalles = "Deposito editado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        [Route("EstatusDeposito/reporte")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_05")]
        public async Task<LiberacionesResponseReporteEstatusDepositoExcel> reporteConcentradoExcel([FromBody]LiberacionesRequestListadoEstatusDepositos item)
        {
            LiberacionesResponseListadoEstatusDepositos response = new LiberacionesResponseListadoEstatusDepositos();
            LiberacionesResponseReporteEstatusDepositoExcel responseExcel = new LiberacionesResponseReporteEstatusDepositoExcel();
            ExcelPackage workbook = new ExcelPackage();
            try
            {
                if ((!string.IsNullOrEmpty(item.desde) && (!string.IsNullOrEmpty(item.hasta))) && (DateTime.Parse(item.desde.Split("T")[0]) <= DateTime.Parse(item.desde.Split("T")[0])))
                {
                    item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".000";
                    item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00" + ".999";
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();

                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/EstatusDepositos/listadoEstatusDepositos", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {

                        response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDepositos>(reqResponse);

                        workbook.Workbook.Worksheets.Add("Reporte Estatus Deposito");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte Estatus Deposito"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Ejecutivo";
                        hojaActual.Cells["A1"].AutoFitColumns();
                        hojaActual.Cells["B1"].Value = "MTCID";
                        hojaActual.Cells["B1"].AutoFitColumns();
                        hojaActual.Cells["C1"].Value = "ID Establecimiento";
                        hojaActual.Cells["C1"].AutoFitColumns();
                        hojaActual.Cells["D1"].Value = "Forma de pago";
                        hojaActual.Cells["D1"].AutoFitColumns();
                        hojaActual.Cells["E1"].Value = "Banco";
                        hojaActual.Cells["E1"].AutoFitColumns();
                        hojaActual.Cells["F1"].Value = "Monto";
                        hojaActual.Cells["F1"].AutoFitColumns();
                        hojaActual.Cells["G1"].Value = "Fecha Depósito";
                        hojaActual.Cells["G1"].AutoFitColumns();
                        hojaActual.Cells["H1"].Value = "Hora Depósito";
                        hojaActual.Cells["H1"].AutoFitColumns();
                        hojaActual.Cells["I1"].Value = "Referencia";
                        hojaActual.Cells["I1"].AutoFitColumns();
                        hojaActual.Cells["J1"].Value = "No Autorización";
                        hojaActual.Cells["J1"].AutoFitColumns();
                        hojaActual.Cells["K1"].Value = "Estatus";
                        hojaActual.Cells["K1"].AutoFitColumns();
                        hojaActual.Cells["L1"].Value = "Hora Liberación";
                        hojaActual.Cells["L1"].AutoFitColumns();
                        hojaActual.Cells["A1:L1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (DepositosEstatus reporte in response.listadoEstatusDepositos)
                        {
                            long band = 0;

                            hojaActual.Cells["A" + linea].Value = reporte.nombreUsuario.ToString();
                            hojaActual.Cells["B" + linea].Value = int.Parse(reporte.idCadena.ToString());
                            hojaActual.Cells["C" + linea].Value = int.Parse(reporte.idEstablecimiento.ToString());
                            hojaActual.Cells["D" + linea].Value = reporte.formaDePago.ToString();
                            hojaActual.Cells["E" + linea].Value = reporte.banco.ToString();
                            hojaActual.Cells["F" + linea].Value = Decimal.Round(Decimal.Parse(reporte.monto.ToString(), System.Globalization.NumberStyles.Float), 2);
                            hojaActual.Cells["F" + linea].Style.Numberformat.Format = "$#,##0.00";
                            hojaActual.Cells["G" + linea].Value = reporte.fechaDeposito.ToString();
                            hojaActual.Cells["H" + linea].Value = reporte.horaDeposito.ToString();
                            if (long.TryParse(reporte.referencia.ToString(), out band))
                                hojaActual.Cells["I" + linea].Value = long.Parse(reporte.referencia.ToString());
                            else
                                hojaActual.Cells["I" + linea].Value = reporte.referencia.ToString();
                            band = 0;
                            if (long.TryParse(reporte.referencia2.ToString(), out band))
                                hojaActual.Cells["J" + linea].Value = long.Parse(reporte.referencia2.ToString());
                            else
                                hojaActual.Cells["J" + linea].Value = reporte.referencia2.ToString();

                            hojaActual.Cells["K" + linea].Value = reporte.status.ToString();
                            hojaActual.Cells["L" + linea].Value = reporte.horaLiberacion.ToString();

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
                        responseExcel.reporteNombre = "ReporteEstatusDeposito.xlsx";
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

        #region Bloquear Bancos

        [Route("BloquearBancos/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_06")]
        public async Task<LiberacionesResponseListadoBancosLiberacion> getListadoBancosLiberaciones()
        {
            LiberacionesResponseListadoBancosLiberacion response = new LiberacionesResponseListadoBancosLiberacion();
            try
            {
                
                string reqResponse = await request.doPost("Liberaciones/BloquearBancos/listado", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoBancosLiberacion>(reqResponse);

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

        [Route("BloquearBancos/bloqueaBanco")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_06,PERM_04_06_01")]
        public async Task<LiberacionesResponseBloqueaBanco> bloqueaBanco([FromBody] LiberacionesRequestBloqueaBanco item)
        {
            LiberacionesResponseBloqueaBanco response = new LiberacionesResponseBloqueaBanco();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/BloquearBancos/bloqueaBanco", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseBloqueaBanco>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "BloquearBancos/bloqueaBanco",
                        Detalles = "Banco bloqueado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);

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

        [Route("BloquearBancos/solicitudFichaDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_06,PERM_04_06_02")]
        public async Task<LiberacionesResponsesolicitudFichaDeposito> solicitudFichaDeposito([FromBody] LiberacionesRequestSolicitudFichaDeposito item)
        {
            LiberacionesResponsesolicitudFichaDeposito response = new LiberacionesResponsesolicitudFichaDeposito();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/BloquearBancos/solicitudFichaDeposito", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponsesolicitudFichaDeposito>(reqResponse);

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
        
        [Route("BloquearBancos/bloqueaLiberacionAutomatica")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_06,PERM_04_06_03")]
        public async Task<LiberacionesResponsebloqueaLiberacionAutomatica> solicitudFichaDeposito([FromBody] LiberacionesRequestBloqueaLiberacionAutomatica item)
        {
            LiberacionesResponsebloqueaLiberacionAutomatica response = new LiberacionesResponsebloqueaLiberacionAutomatica();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/BloquearBancos/bloqueaLiberacionAutomatica", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponsebloqueaLiberacionAutomatica>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "BloquearBancos/bloqueaLiberacionAutomatica",
                        Detalles = "Banco liberación automatica bloqueada",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);


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

        #region Relacionar Depositos

        [Route("RelacionarDepositos/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_07")]
        public async Task<LiberacionesResponseListadoDepositosError> getListadoDepositosBloqueados([FromBody] LiberacionesRequestBloqueaBanco item)
        {
            LiberacionesResponseListadoDepositosError response = new LiberacionesResponseListadoDepositosError();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/RelacionarDepositos/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosError>(reqResponse);

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

        [Route("RelacionarDepositos/DetallesDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_07")]
        public async Task<LiberacionesResponseDetalleDepositos> detalleDepositosRelacionar([FromBody]LiberacionesRequestDetalleDepositos item)
        {
            LiberacionesResponseDetalleDepositos response = new LiberacionesResponseDetalleDepositos();
            try
            {
                item.IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.IdDeposito = int.Parse(item.IdDepositoStr);

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/RelacionarDepositos/detalleDepositos", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDetalleDepositos>(reqResponse);
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

        [Route("RelacionarDepositos/CuentaLiberacionAutomatica")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_07")]
        public async Task<LiberacionesResponseLiberacionAutomatica> cuentaLiberacionAutomaticaRelacionar([FromBody]LiberacionesRequestLiberacionAutomatica item)
        {
            LiberacionesResponseLiberacionAutomatica response = new LiberacionesResponseLiberacionAutomatica();
            try
            {
                item.idCuenta = int.Parse(item.idCuentaStr);

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Liberaciones/ListadoDeposito/cuentaLiberacionAutomatica", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseLiberacionAutomatica>(reqResponse);
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

        [Route("RelacionarDepositos/DepositosCuenta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_07")]
        public async Task<LiberacionesResponseDepositosCuenta> depositosCuentaRelacionar([FromBody]LiberacionesRequestDepositosCuenta item)
        {
            LiberacionesResponseDepositosCuenta response = new LiberacionesResponseDepositosCuenta();
            try
            {

                item.idDeposito = int.Parse(item.idDepositoStr);
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/RelacionarDepositos/depositosCuenta", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosCuenta>(reqResponse);
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

        [Route("RelacionarDepositos/listadoCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_07")]
        public async Task<LiberacionesResponseListadoDepositosErrorCuentas> relacionarListadoCuentas()
        {
            LiberacionesResponseListadoDepositosErrorCuentas response = new LiberacionesResponseListadoDepositosErrorCuentas();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/RelacionarDepositos/listadoCuentas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosErrorCuentas>(reqResponse);
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

        [Route("RelacionarDepositos/relacionarDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_07")]
        public async Task<LiberacionesResponseRelacionaDeposito> relacionarDeposito([FromBody]LiberacionesRequestRelacionaDeposito item)
        {
            LiberacionesResponseRelacionaDeposito response = new LiberacionesResponseRelacionaDeposito();
            try
            {
                item.idDeposito = int.Parse(item.idDepositoStr);
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/RelacionarDepositos/relacionarDeposito", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseRelacionaDeposito>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "RelacionarDepositos/relacionarDeposito",
                        Detalles = "Deposito relacionado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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
        
        [Route("RelacionarDepositos/conciliarDepositos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_07")]
        public async Task<LiberacionesResponseRelacionaConciliaDepositosNoRelacionados> conciliarDepositos([FromBody]LiberacionesRequestConciliaDepositosNoRelacionados item)
        {
            LiberacionesResponseRelacionaConciliaDepositosNoRelacionados response = new LiberacionesResponseRelacionaConciliaDepositosNoRelacionados();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/RelacionarDepositos/conciliarDepositos", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseRelacionaConciliaDepositosNoRelacionados>(reqResponse);
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

        [Route("RelacionarDepositos/sucursalesAzteca")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_07")]
        public async Task<LiberacionesResponseRelacionaDeposito> sucursalesAzteca()
        {
            LiberacionesRequestConciliaDepositosNoRelacionados item = new LiberacionesRequestConciliaDepositosNoRelacionados();
            LiberacionesResponseRelacionaDeposito response = new LiberacionesResponseRelacionaDeposito();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/RelacionarDepositos/sucursalesAzteca", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseRelacionaDeposito>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "RelacionarDepositos/sucursalesAzteca",
                        Detalles = "sucursales Azteca",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        #region Depositos No Relacionados

        [Route("DepositosNoRelacionados/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_08")]
        public async Task<LiberacionesResponseListadoDepositosNoRelacionados> getListadoDepositosNoRelacionados([FromBody] LiberacionesRequestDepositosNoRelacionados item)
        {
            LiberacionesResponseListadoDepositosNoRelacionados response = new LiberacionesResponseListadoDepositosNoRelacionados();
            try
            {

                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy");
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy");

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosNoRelacionados/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosNoRelacionados>(reqResponse);
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

        [Route("DepositosNoRelacionados/listadoCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_08")]
        public async Task<LiberacionesResponseListadoNoRelacionadosCuentas> noRelacionadosListadoCuentas()
        {
            LiberacionesResponseListadoNoRelacionadosCuentas response = new LiberacionesResponseListadoNoRelacionadosCuentas();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositosNoRelacionados/listadoCuentas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoNoRelacionadosCuentas>(reqResponse);
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

        [Route("DepositosNoRelacionados/listadoFormasPago")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_08")]
        public async Task<LiberacionesResponseListadoNoRelacionadosFormasPago> noRelacionadosFormasPago()
        {
            LiberacionesResponseListadoNoRelacionadosFormasPago response = new LiberacionesResponseListadoNoRelacionadosFormasPago();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositosNoRelacionados/listadoFormasPago", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoNoRelacionadosFormasPago>(reqResponse);
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

        [Route("DepositosNoRelacionados/tipoDepositos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_08")]
        public async Task<LiberacionesResponseListadoTipoDepositos> noRelacionadosTipoDepositos()
        {
            LiberacionesResponseListadoTipoDepositos response = new LiberacionesResponseListadoTipoDepositos();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositosNoRelacionados/tipoDepositos", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoTipoDepositos>(reqResponse);
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

        [Route("DepositosNoRelacionados/reporte")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_08")]
        public async Task<LiberacionesResponseReporteNoRelacionadosExcel> reporteNoRelacionadoExcel([FromBody]LiberacionesRequestDepositosNoRelacionados item)
        {
            LiberacionesResponseListadoDepositosNoRelacionados response = new LiberacionesResponseListadoDepositosNoRelacionados();
            LiberacionesResponseReporteNoRelacionadosExcel responseExcel = new LiberacionesResponseReporteNoRelacionadosExcel();
            ExcelPackage workbook = new ExcelPackage();

            try
            {
                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy");
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy");

                LiberacionesResponseListadoTipoDepositos listadoDeposito = await noRelacionadosTipoDepositos();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones//DepositosNoRelacionados/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosNoRelacionados>(reqResponse);

                    workbook.Workbook.Worksheets.Add("Reporte No Relacionados");
                    var hojaActual = workbook.Workbook.Worksheets["Reporte No Relacionados"];
                    hojaActual = workbook.Workbook.Worksheets[0];

                    hojaActual.Cells["A1"].Value = "Fecha";
                    hojaActual.Cells["A1"].AutoFitColumns();
                    hojaActual.Cells["B1"].Value = "Banco";
                    hojaActual.Cells["B1"].AutoFitColumns();
                    hojaActual.Cells["C1"].Value = "Cuenta";
                    hojaActual.Cells["C1"].AutoFitColumns();
                    hojaActual.Cells["D1"].Value = "No. Autorizacion";
                    hojaActual.Cells["D1"].AutoFitColumns();
                    hojaActual.Cells["E1"].Value = "Referencia";
                    hojaActual.Cells["E1"].AutoFitColumns();
                    hojaActual.Cells["F1"].Value = "Sucursal";
                    hojaActual.Cells["F1"].AutoFitColumns();
                    hojaActual.Cells["G1"].Value = "Monto";
                    hojaActual.Cells["G1"].AutoFitColumns();
                    hojaActual.Cells["H1"].Value = "Forma de Pago";
                    hojaActual.Cells["H1"].AutoFitColumns();
                    hojaActual.Cells["I1"].Value = "Descripcion";
                    hojaActual.Cells["I1"].AutoFitColumns();
                    hojaActual.Cells["J1"].Value = "Observaciones";
                    hojaActual.Cells["J1"].AutoFitColumns();
                    hojaActual.Cells["K1"].Value = "Tipo Depósito";
                    hojaActual.Cells["K1"].AutoFitColumns();
                    hojaActual.Cells["L1"].Value = "Alta";
                    hojaActual.Cells["L1"].AutoFitColumns();
                    hojaActual.Cells["M1"].Value = "Folio Factura";
                    hojaActual.Cells["M1"].AutoFitColumns();
                    hojaActual.Cells["A1:M1"].Style.Font.Bold = true;
                    int linea = 2;

                    foreach (DepositosNoRelacionados reporte in response.listadoDepositosNoRelacionados)
                    {
                        long bandera = 0;
                        double parseo = 0;
                        

                        hojaActual.Cells["A" + linea].Value = reporte.fechaHoraAlta.ToString().Split(" ")[0];
                        hojaActual.Cells["B" + linea].Value = reporte.banco.ToString();
                        if (double.TryParse(reporte.cuenta.ToString(), out parseo))
                            hojaActual.Cells["C" + linea].Value = double.Parse(reporte.cuenta.ToString(), System.Globalization.NumberStyles.Float);
                        else
                            hojaActual.Cells["C" + linea].Value = reporte.cuenta.ToString();
                        if (double.TryParse(reporte.numeroAutorizacion.ToString(), out parseo))
                            hojaActual.Cells["D" + linea].Value = double.Parse(reporte.numeroAutorizacion.ToString(), System.Globalization.NumberStyles.Float);
                        else
                            hojaActual.Cells["D" + linea].Value = reporte.numeroAutorizacion.ToString();
                        if (long.TryParse(reporte.referencia.ToString(), out bandera))
                            hojaActual.Cells["E" + linea].Value = long.Parse(reporte.referencia.ToString());
                        else
                            hojaActual.Cells["E" + linea].Value = reporte.referencia.ToString();
                        hojaActual.Cells["F" + linea].Value = int.Parse(reporte.sucursal.ToString());
                        hojaActual.Cells["G" + linea].Value = Decimal.Round(Decimal.Parse(reporte.monto.ToString()), 2);
                        hojaActual.Cells["G" + linea].Style.Numberformat.Format = "$#,##0.00";
                        hojaActual.Cells["H" + linea].Value = reporte.fechaHoraAlta.ToString();
                        hojaActual.Cells["I" + linea].Value = reporte.formaPago.ToString();
                        hojaActual.Cells["J" + linea].Value = reporte.descripciones.ToString();
                        hojaActual.Cells["K" + linea].Value = listadoDeposito.listadoTipoDepositos.Single(x => x.id == reporte.idTipoDeposito).label.ToString();
                        hojaActual.Cells["L" + linea].Value = reporte.fechaHoraAlta.ToString();
                        hojaActual.Cells["M" + linea].Value = reporte.facturaSAT.ToString();

                        linea++;
                    }

                    var range = hojaActual.Cells[1, 1, linea - 1, 13];
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
                    responseExcel.reporteNombre = "ReporteDepositosNoRelacionados.xlsx";
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

        [Route("DepositosNoRelacionados/editaDepositoBanco")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_08")]
        public async Task<LiberacionesResponseSetContabilidadDepositoBanco> detalleDeposito([FromBody] LiberacionesRequestSetContabilidadDepositoBanco item)
        {
            LiberacionesResponseSetContabilidadDepositoBanco response = new LiberacionesResponseSetContabilidadDepositoBanco();
            try
            {

                item.id = int.Parse(item.idStr);
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosNoRelacionados/editaDepositoBanco", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseSetContabilidadDepositoBanco>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "DepositosNoRelacionados/editaDepositoBanco",
                        Detalles = "Deposito editado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        [Route("DepositosNoRelacionados/vincularDepositoFolioFactura")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_08")]
        public async Task<LiberacionesResponseSetContabilidadDepositoBanco> vincularDepositoFolio([FromBody] LiberacionesRequestSetDepositoFolio item)
        {
            LiberacionesResponseSetContabilidadDepositoBanco response = new LiberacionesResponseSetContabilidadDepositoBanco();
            try
            {

                item.id = int.Parse(item.idStr);
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosNoRelacionados/vincularDepositoFolioFactura", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseSetContabilidadDepositoBanco>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "DepositosNoRelacionados/vincularDepositoFolioFactura",
                        Detalles = "Deposito editado Folio SAT",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        #region Reverso Deposito

        [Route("ReversoDepositos/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_09")]
        public async Task<LiberacionesResponseListadoReversoDepositos> getListadoReversoDepositos([FromBody] LiberacionesRequestReversoDeposito item)
        {
            LiberacionesResponseListadoReversoDepositos response = new LiberacionesResponseListadoReversoDepositos();
            try
            {

                item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy");
                item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy");

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/ReversoDepositos/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoReversoDepositos>(reqResponse);
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
        
        [Route("ReversoDepositos/listadoEstatus")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_09")]
        public async Task<LiberacionesResponseListadoEstatusDeposito> getListadoEstatusDepositoReverso([FromBody] LiberacionesRequestReversoDeposito item)
        {
            LiberacionesResponseListadoEstatusDeposito response = new LiberacionesResponseListadoEstatusDeposito();
            try
            {


                string reqResponse = await request.doPost("Liberaciones/ReversoDepositos/listado", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDeposito>(reqResponse);
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

        [Route("ReversoDepositos/reversar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_09")]
        public async Task<LiberacionesResponseReverso> reversarDeposito([FromBody] LiberacionesRequestReversar item)
        {
            LiberacionesResponseReverso response = new LiberacionesResponseReverso();
            try
            {
                if (!string.IsNullOrEmpty(item.idDepositoStr))
                {
                    item.idDeposito = int.Parse(item.idDepositoStr);
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/ReversoDepositos/reversar", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseReverso>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "ReversoDepositos/reversar",
                            Detalles = "Deposito reversado",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos de depósito inválidos";
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

        #region Reporte Liberacion Automatica
        
        [Route("ReporteLiberacionAutomatica/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_10")]
        public async Task<LiberacionesResponseListadoDepositosLiberacionAutomatica> getListadoLiberacionAutomatica([FromBody] LiberacionesRequestDepositosLiberacionAutomatica item)
        {
            LiberacionesResponseListadoDepositosLiberacionAutomatica response = new LiberacionesResponseListadoDepositosLiberacionAutomatica();
            try
            {
                if (!string.IsNullOrEmpty(item.desde) && !string.IsNullOrEmpty(item.hasta))
                {
                    if (item.idCadena != 0)
                    {
                        item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy");
                        item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy");

                        string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                        string reqResponse = await request.doPost("Liberaciones/ReporteLiberacionAutomatica/listado", data, HttpContext.Session.GetString("token"));

                        if (request.codigoRespuesta == 0)
                        {
                            response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosLiberacionAutomatica>(reqResponse);
                        }
                        else
                        {
                            response.CodigoRespuesta = request.codigoRespuesta;
                            response.MensajeRespuesta = request.mensajeRespuesta;
                        }
                    }
                    else
                    {

                        response.CodigoRespuesta = -1;
                        response.MensajeRespuesta = "Debe proporcionar una cadena";
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Debe proporcionar un rango de fechas";
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

        #region Deposito Clientes
        
        [Route("DepositosClientes/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_01")]
        public async Task<LiberacionesResponseListadoDepositosCliente> getListadoDepositosCliente([FromBody] LiberacionesRequestDepositosCliente item)
        {
            LiberacionesResponseListadoDepositosCliente response = new LiberacionesResponseListadoDepositosCliente();
            try
            {
                if (!string.IsNullOrEmpty(item.desde) && !string.IsNullOrEmpty(item.hasta))
                {
                    
                    item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy") + " 00:00:00.000";
                    item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy") + " 23:59:59.999";

                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/DepositosClientes/listado", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosCliente>(reqResponse);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                   
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Debe proporcionar un rango de fechas";
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

        [Route("DepositosClientes/listadoCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_01")]
        public async Task<LiberacionesResponseDepositosClienteListadoCuenta> depositosClientesListadoCuentas()
        {
            LiberacionesResponseDepositosClienteListadoCuenta response = new LiberacionesResponseDepositosClienteListadoCuenta();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositosClientes/listadoCuentas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosClienteListadoCuenta>(reqResponse);
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

        [Route("DepositosClientes/listadoFormasPago")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_01")]
        public async Task<LiberacionesResponseDepositosClienteFormasPago> depositosClientesListadoFormasPago()
        {
            LiberacionesResponseDepositosClienteFormasPago response = new LiberacionesResponseDepositosClienteFormasPago();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositosClientes/listadoFormasPago", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosClienteFormasPago>(reqResponse);
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

        #region Deposito Banca
        
        [Route("DepositosBanca/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseListadoDepositosBanca> getListadoDepositosBanca([FromBody] LiberacionesRequestDepositoBanco item)
        {
            LiberacionesResponseListadoDepositosBanca response = new LiberacionesResponseListadoDepositosBanca();
            try
            {
                if (!string.IsNullOrEmpty(item.desde) && !string.IsNullOrEmpty(item.hasta))
                {

                    item.desde = DateTime.Parse(item.desde.Split("T")[0]).ToString("dd/MM/yyyy")/* + " 00:00:00.000"*/;
                    item.hasta = DateTime.Parse(item.hasta.Split("T")[0]).ToString("dd/MM/yyyy")/* + " 23:59:59.999"*/;

                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/DepositosBanca/listado", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDepositosBanca>(reqResponse);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }

                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Debe proporcionar un rango de fechas";
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

        [Route("DepositosBanca/listadoCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseDepositosClienteListadoCuenta> depositosBancaListadoCuentas()
        {
            LiberacionesResponseDepositosClienteListadoCuenta response = new LiberacionesResponseDepositosClienteListadoCuenta();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositosClientes/listadoCuentas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosClienteListadoCuenta>(reqResponse);
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

        [Route("DepositosBanca/listadoFormasPago")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseDepositosClienteFormasPago> depositosBancaListadoFormasPago()
        {
            LiberacionesResponseDepositosClienteFormasPago response = new LiberacionesResponseDepositosClienteFormasPago();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositosBanca/listadoFormasPago", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosClienteFormasPago>(reqResponse);
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

        [Route("DepositosBanca/tipoDepositos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseDepositosBancaListadoTipoDepositos> depositosBancaTipoDepositos()
        {
            LiberacionesResponseDepositosBancaListadoTipoDepositos response = new LiberacionesResponseDepositosBancaListadoTipoDepositos();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/DepositosNoRelacionados/tipoDepositos", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosBancaListadoTipoDepositos>(reqResponse);
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
        
        [Route("DepositosBanca/desrelacionar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseDepositosBancaDesRelacionarDeposito> depositosBancaDesrelacionar([FromBody] LiberacionesRequestDesRelacionarDeposito item)
        {
            LiberacionesResponseDepositosBancaDesRelacionarDeposito response = new LiberacionesResponseDepositosBancaDesRelacionarDeposito();
            try
            {
                
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosBanca/desrelacionar", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosBancaDesRelacionarDeposito>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "DepositosBanca/desrelacionar",
                        Detalles = "Deposito desrelacionado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        [Route("DepositosBanca/eliminar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseDepositosBancaEliminarDepositos> depositosBancaEliminar([FromBody] LiberacionesRequestEliminarDeposito item)
        {
            LiberacionesResponseDepositosBancaEliminarDepositos response = new LiberacionesResponseDepositosBancaEliminarDepositos();
            try
            {
                
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosBanca/eliminar", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosBancaEliminarDepositos>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "DepositosBanca/eliminar",
                        Detalles = "Deposito eliminar",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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
        
        [Route("DepositosBanca/eliminarConjunto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseDepositosBancaEliminarDepositos> depositosBancaEliminarConjunto([FromBody] LiberacionesRequestEliminaDepositosBanco item)
        {
            LiberacionesResponseDepositosBancaEliminarDepositos response = new LiberacionesResponseDepositosBancaEliminarDepositos();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosBanca/eliminarConjunto", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDepositosBancaEliminarDepositos>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "DepositosBanca/eliminarConjunto",
                        Detalles = "Deposito eliminar",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        [Route("DepositosBanca/conciliarNoRelacionado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseOperacionConciliarValidar> depositosBancaConciliarNoRelacionado([FromBody] LiberacionesRequestConciliaNoRelacionados item)
        {
            LiberacionesResponseOperacionConciliarValidar response = new LiberacionesResponseOperacionConciliarValidar();
            LiberacionesResponseDepositosBancaConciliacionDepositos responseConsl = new LiberacionesResponseDepositosBancaConciliacionDepositos();
            try
            {
                DateTime dumyDate = new DateTime();
                if (item.idCatalogoCuenta > 0 && !string.IsNullOrEmpty(item.desde) && DateTime.TryParse(item.desde,out dumyDate))
                {
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/DepositosBanca/conciliarNoRelacionado", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        responseConsl = JsonConvert.DeserializeObject<LiberacionesResponseDepositosBancaConciliacionDepositos>(reqResponse);
                        if (responseConsl.conciliado)
                        {
                            LiberacionesRequestValidaRelacionados requestVal = new LiberacionesRequestValidaRelacionados();
                            LiberacionesResponseDepositosBancaValidaReferenciados responseVal = new LiberacionesResponseDepositosBancaValidaReferenciados();
                            requestVal.desde = item.desde;
                            requestVal.idCatalogoCuenta = item.idCatalogoCuenta;
                            requestVal.idUsuario = item.idUsuario;
                            requestVal.ip = item.ip;

                            data = Seguridad.Encrypt(JsonConvert.SerializeObject(requestVal));
                            reqResponse = await request.doPost("Liberaciones/DepositosBanca/validaReferenciado", data, HttpContext.Session.GetString("token"));

                            if (request.codigoRespuesta == 0)
                            {

                                responseVal = JsonConvert.DeserializeObject<LiberacionesResponseDepositosBancaValidaReferenciados>(reqResponse);
                                response.CodigoRespuesta = responseVal.CodigoRespuesta;
                                response.MensajeRespuesta = responseVal.MensajeRespuesta;
                                response.realizado = responseVal.validado;

                                util.logActividadUsuario(new ActividadUsuario
                                {
                                    IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                                    Usuario = HttpContext.Session.GetString("user"),
                                    Seccion = "Liberaciones",
                                    Actividad = "DepositosBanca/conciliarNoRelacionado",
                                    Detalles = "Deposito conciliado",
                                    Objeto = JsonConvert.SerializeObject(item)
                                }, __appSettings.LoggerLiberaciones);
                            }
                            else
                            {
                                response.CodigoRespuesta = request.codigoRespuesta;
                                response.MensajeRespuesta = request.mensajeRespuesta;

                            }
                        }
                        else
                        {
                            response.CodigoRespuesta = request.codigoRespuesta;
                            response.MensajeRespuesta = "Ocurrió un error en el proceso de conciliacion no relacionados.";
                        }
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos de cuenta inválidos";
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

        [Route("DepositosBanca/nuevoDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseNuevoDepositoBanca> depositosBancaNuevoDeposito([FromBody] LiberacionesRequetNuevoDepositoBanco item)
        {
            LiberacionesResponseNuevoDepositoBanca response = new LiberacionesResponseNuevoDepositoBanca();
            try
            {
                item.fechaHoraAlta = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                item.horaDeposito = DateTime.Parse(item.horaDeposito).ToString("hh:mm:ss");
                item.idUsuarioVentas = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosBanca/nuevoDeposito", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseNuevoDepositoBanca>(reqResponse);

                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "DepositosBanca/nuevoDeposito",
                        Detalles = "Deposito registrado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        [Route("DepositosBanca/editaDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseEditaDeposito> depositosBancaEditaDeposito([FromBody] LiberacionesRequetEditDepositoBanco item)
        {
            LiberacionesResponseEditaDeposito response = new LiberacionesResponseEditaDeposito();
            try
            {
                item.hora = DateTime.Parse(item.hora).ToString("HH:mm:ss");
                item.idUsuarioVentas = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosBanca/editaDeposito", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseEditaDeposito>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "DepositosBanca/editaDeposito",
                        Detalles = "Deposito editado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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
        
        [Route("DepositosBanca/detalleDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_12_02")]
        public async Task<LiberacionesResponseDetalleDepositoBanca> detalleBancaDeposito([FromBody] LiberacionesRequestDetalleDeposito item)
        {
            LiberacionesResponseDetalleDepositoBanca response = new LiberacionesResponseDetalleDepositoBanca();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosBanca/detalleDeposito", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseDetalleDepositoBanca>(reqResponse);
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

        #region Fechas Feriadas

        [Route("FechasFeriadas/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_15")]
        public async Task<LiberacionesResponseListadoFechasFeriadas> getListadoFechasFeriadas()
        {
            LiberacionesResponseListadoFechasFeriadas response = new LiberacionesResponseListadoFechasFeriadas();
            try
            {
                
                string reqResponse = await request.doPost("Liberaciones/FechasFeriadas/listado", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoFechasFeriadas>(reqResponse);
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

        [Route("FechasFeriadas/listadoBancos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_15")]
        public async Task<LiberacionesResponseListadoBancos> fechasFeriadasListadoBancos()
        {
            LiberacionesResponseListadoBancos response = new LiberacionesResponseListadoBancos();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/FechasFeriadas/listadoBancos", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoBancos>(reqResponse);
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

        [Route("FechasFeriadas/nuevaFechaFeriada")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_15")]
        public async Task<LiberacionesResponseNuevaFechaFeriada> fechasFeriadasNuevaFecha([FromBody] LiberacionesRequestNuevaFechaFeriada item)
        {
            LiberacionesResponseNuevaFechaFeriada response = new LiberacionesResponseNuevaFechaFeriada();
            try
            {
                if (!string.IsNullOrEmpty(item.fechaInicio) && !string.IsNullOrEmpty(item.fechaFin) && !string.IsNullOrEmpty(item.fechaLaboralSiguiente))
                {

                    item.fechaInicio = DateTime.Parse(item.fechaInicio.Split("T")[0]).ToString("dd/MM/yyyy");
                    item.fechaFin = DateTime.Parse(item.fechaFin.Split("T")[0]).ToString("dd/MM/yyyy");
                    item.fechaLaboralSiguiente = DateTime.Parse(item.fechaLaboralSiguiente.Split("T")[0]).ToString("dd/MM/yyyy");
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/FechasFeriadas/nuevaFechaFeriada", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseNuevaFechaFeriada>(reqResponse);

                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "FechasFeriadas/nuevaFechaFeriada",
                            Detalles = "Fecha registrada",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos de fecha inválidos";
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

        [Route("FechasFeriadas/nuevaFechaBanco")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_15")]
        public async Task<LiberacionesResponseNuevaFechaBanco> fechasFeriadasnuevaFechaBanco([FromBody] LiberacionesRequestNuevaFechaBanco item)
        {
            LiberacionesResponseNuevaFechaBanco response = new LiberacionesResponseNuevaFechaBanco();
            try
            {
                if (item.idFechaFeriada > 0 && item.idBanco > 0)
                {
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/FechasFeriadas/nuevaFechaBanco", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseNuevaFechaBanco>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "FechasFeriadas/nuvaFechaBanco",
                            Detalles = "Fecha registrada",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos de fecha inválidos";
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

        #region Diferencia Horas
        
        [Route("DiferenciaHoras/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_16")]
        public async Task<LiberacionesResponseListadoDifrenciaHoras> getListadoDiferenciaHoras()
        {
            LiberacionesRequestListadoDiferenciaHoras item = new LiberacionesRequestListadoDiferenciaHoras();
            LiberacionesResponseListadoDifrenciaHoras response = new LiberacionesResponseListadoDifrenciaHoras();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DiferenciaHoras/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoDifrenciaHoras>(reqResponse);
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

        [Route("DiferenciaHoras/listadoEstados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_16")]
        public async Task<LiebracionResponseLsitadoEstadosZona> getListadoEstado()
        {
            LiberacionesRequestGestionaDiferenciaHoras item = new LiberacionesRequestGestionaDiferenciaHoras();
            LiebracionResponseLsitadoEstadosZona response = new LiebracionResponseLsitadoEstadosZona();
            try
            {

                item.menu = 0;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DiferenciaHoras/listadoEstados", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiebracionResponseLsitadoEstadosZona>(reqResponse);
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

        [Route("DiferenciaHoras/gestion/eliminar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_16")]
        public async Task<LiberacionesResponseGestionaDiferenciaHoras> gestionDiferenciaHorasEliminar([FromBody] LiberacionesRequestGestionaDiferenciaHoras item)
        {

            LiberacionesResponseGestionaDiferenciaHoras response = new LiberacionesResponseGestionaDiferenciaHoras();
            try
            {

                if (item.idEstado > 0)
                {
                    item.menu = 4;
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/DiferenciaHoras/gestion", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseGestionaDiferenciaHoras>(reqResponse);

                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "DiferenciaHoras/gestion/eliminar",
                            Detalles = "Eliminada",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos incorrectos";
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

        [Route("DiferenciaHoras/gestion/editar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_16")]
        public async Task<LiberacionesResponseGestionaDiferenciaHoras> gestionDiferenciaHorasEditar([FromBody] LiberacionesRequestGestionaDiferenciaHoras item)
        {

            LiberacionesResponseGestionaDiferenciaHoras response = new LiberacionesResponseGestionaDiferenciaHoras();
            try
            {

                if (item.idEstado > 0 )
                {
                    item.menu = 3;
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/DiferenciaHoras/gestion", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseGestionaDiferenciaHoras>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "DiferenciaHoras/gestion/editar",
                            Detalles = "Editada",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos incorrectos";
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

        [Route("DiferenciaHoras/gestion/agregar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_16")]
        public async Task<LiberacionesResponseGestionaDiferenciaHoras> gestionDiferenciaHorasAgregar([FromBody] LiberacionesRequestGestionaDiferenciaHoras item)
        {

            LiberacionesResponseGestionaDiferenciaHoras response = new LiberacionesResponseGestionaDiferenciaHoras();
            try
            {

                if (item.idEstado > 0 )
                {
                    item.menu = 2;
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/DiferenciaHoras/gestion", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseGestionaDiferenciaHoras>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "DiferenciaHoras/gestion/agregar",
                            Detalles = "Editada",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos incorrectos";
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

        #region Cadenas Referencidas
        
        [Route("CadenasReferenciadas/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_17")]
        public async Task<LiberacionesResponseListadoCadenasReferenciadas> getListadoCadenasReferenciadas([FromBody] LiberacionesRequestListadoCadenasReferenciadas item)
        {
            LiberacionesResponseListadoCadenasReferenciadas response = new LiberacionesResponseListadoCadenasReferenciadas();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/CadenasReferenciadas/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoCadenasReferenciadas>(reqResponse);
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

        [Route("CadenasReferenciadas/listadoLiberacionAutomatica")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_17")]
        public async Task<LiberacionesResponseCadenasReferenciadasLiberacionAutomatica> getListadoLiberacionAutomaticaReferenciadas()
        {
            LiberacionesResponseCadenasReferenciadasLiberacionAutomatica response = new LiberacionesResponseCadenasReferenciadasLiberacionAutomatica();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/CadenasReferenciadas/listadoLiberacionAutomatica", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseCadenasReferenciadasLiberacionAutomatica>(reqResponse);
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

        [Route("CadenasReferenciadas/agregar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_17")]
        public async Task<LiberacionesResponseNuevaCadenaReferenciada> nuevaCadenaReferenciada([FromBody] LiberacionesRequestNuevaCadenaReferenciada item)
        {
            LiberacionesResponseNuevaCadenaReferenciada response = new LiberacionesResponseNuevaCadenaReferenciada();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                if (item.idCadena > 0 && item.idCuenta > 0)
                {
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/CadenasReferenciadas/agregar", data, HttpContext.Session.GetString("token"));
                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseNuevaCadenaReferenciada>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "CadenasReferenciadas/agregar",
                            Detalles = "Agregado",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {

                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        [Route("CadenasReferenciadas/editar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_17")]
        public async Task<LiberacionesResponseEditaCadenaReferenciada> editarCadenaReferenciada([FromBody] LiberacionesRequestEditaCadenaReferenciada item)
        {
            LiberacionesResponseEditaCadenaReferenciada response = new LiberacionesResponseEditaCadenaReferenciada();
            try
            {

                if (item.idReferencia > 0 && item.idCuenta > 0)
                {
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/CadenasReferenciadas/editar", data, HttpContext.Session.GetString("token"));
                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseEditaCadenaReferenciada>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "CadenasReferenciadas/editar",
                            Detalles = "Editado",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        [Route("CadenasReferenciadas/eliminar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_17")]
        public async Task<LiberacionesResponseDeleteCadenaReferenciada> deleteCadenaReferenciada([FromBody] LiberacionesRequestDeleteCadenaReferenciada item)
        {
            LiberacionesResponseDeleteCadenaReferenciada response = new LiberacionesResponseDeleteCadenaReferenciada();
            try
            {

                if (item.idReferencia > 0)
                {
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/CadenasReferenciadas/eliminar", data, HttpContext.Session.GetString("token"));
                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesResponseDeleteCadenaReferenciada>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Liberaciones",
                            Actividad = "CadenasReferenciadas/eliminar",
                            Detalles = "Editado",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerLiberaciones);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Datos inválidos";
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

        #region Liberación Bancos
        [Route("LiberacionBancos/SubirArchivo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_20")]
        public async Task<LiberacionesResponseSubirDepositos> liberaBancos([FromBody] LiberacionesRequestSubirLiberacionBanco item)
        {
            LiberacionesResponseSubirDepositos response = new LiberacionesResponseSubirDepositos();
            try
            {
                DataTable dt = new DataTable();
                System.IO.StreamReader reader = null;
                //Se pasa el archivo a un DataTable
                if (!string.IsNullOrEmpty(item.archivo))
                {
                    if (item.ext == "xlsx" || item.ext == "xlsm" || item.ext == "csv" || item.ext == "xls" || item.ext == "exp")
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
                        else if (item.ext == "csv" || item.ext == "exp")
                        {
                            excelReader = ExcelReaderFactory.CreateCsvReader(reader.BaseStream);
                            dsResult = excelReader.AsDataSet();
                            dt = dsResult.Tables[0];
                        }
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
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item.idBanco));
                string reqResponse = await request.doPost("Liberaciones/LiberarBancos/obtieneElementosBancos", data, HttpContext.Session.GetString("token"));
                var csv = new StringBuilder();
                var csvPlantilla = new StringBuilder();
                var root = JsonConvert.DeserializeObject<listaElemento>(reqResponse);
                listaElemento lista = JsonConvert.DeserializeObject<listaElemento>(reqResponse);
                TimeSpan time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                String hora = time.ToString();
                elemento result = null;
                string fechaArchivo = "";

                //Banco azteca
                if (item.idBanco.idBanco == 20 || item.idBanco.idBanco == 42 || item.idBanco.idBanco == 47 || item.idBanco.idBanco == 74)
                {
                    fechaArchivo = "";
                    if (item.nombre.ToUpper().Contains("MOV"))
                    {
                        dt.Rows.RemoveAt(0);
                        DateTime tme2 = DateTime.Now.AddMinutes(5);
                        time = new TimeSpan(tme2.Hour, tme2.Minute, tme2.Second);
                        foreach (DataRow row in dt.Rows)
                        {
                            result = null;
                            //Aqui se separan por los dos puntos ":" para obtener el primer bloque de texto y comparar con la lista de conceptos
                            //Si no se tienen los dos puntos, se tomarán tal cual el texto y se compará con la lista
                            if (row[3].ToString().Trim().Contains("TRASPASO"))
                                result = lista.listaElementos.Find(x => x.FormaPago == row[3].ToString().Trim().Split(' ')[0]);
                            else if (row[3].ToString().Trim().Contains("TRASP "))
                                result = lista.listaElementos.Find(x => x.FormaPago.Contains("TRASP "));
                            else if (row[3].ToString().Trim().Contains("127017201899183"))
                                result = lista.listaElementos.Find(x => x.FormaPago.Contains("127017201899183"));
                            else if (row[3].ToString().Trim().Contains("DEP 017201899183042"))
                                result = lista.listaElementos.Find(x => x.FormaPago.Contains("DEP 017201899183042"));
                            else if (row[3].ToString().Trim().IsNumeric())
                                result = lista.listaElementos.Find(x => x.codigo == "B520");
                            else if (row[3].ToString().Trim().Contains("DEPOSITO DE EFECTIVO"))
                                result = lista.listaElementos.Find(x => x.FormaPago == "DEPOSITO DE EFECTIVO");
                            else if (row[3].ToString().Trim().Contains("012701720"))
                                result = lista.listaElementos.Find(x => x.codigo == "B601");
                            else if (row[3].ToString().Trim().Contains("Compra de saldo MTCenter"))
                                result = lista.listaElementos.Find(x => x.codigo == "MBD1");
                            else
                                result = lista.listaElementos.Find(x => x.FormaPago == row[3].ToString().Trim());

                            if (result == null)
                                result = lista.listaElementos.Find(x => x.FormaPago == "PAGO INTERBANCARIO");

                            if (result != null)
                            {
                                DateTime fecha = DateTime.Parse(row[1].ToString().Substring(8, 2) + "/" + row[1].ToString().Substring(5, 2) + "/" + row[1].ToString().Substring(0, 4));
                                if (fecha.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    fecha = fecha.AddDays(1);
                                }
                                string decimales = "";
                                if (row[4].ToString().Contains(".") && result.idCatalogo == 1)
                                {
                                    decimales = "0." + row[4].ToString().Split('.')[1];
                                    if (decimales.Length == 3)
                                        decimales = decimales + "0";
                                }
                                if (dt.Columns.Count == 8)
                                {
                                    DateTime tme = DateTime.Parse(row[7].ToString());
                                    hora = tme.Hour + ":" + tme.Minute + ":" + tme.Second;
                                }

                                fechaArchivo = fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0') + "-" + fecha.Year;
                                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                    fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year,//FECHA
                                    hora,//Hora
                                    0,//SUCURSAL
                                    row[6].ToString().Trim(),//MOVIMIENTO
                                    row[4].ToString().Trim(),//IMPORTE
                                    result.idCatalogo,//IDCATALOGO
                                    result.codigo,//CODIGO CONCEPTO
                                    (decimales == "") ? "NOREF" : decimales, //NOREF /// decimales,
                                    row[3].ToString().Trim(),//CONCEPTO
                                    item.idBanco.idBanco//ID CUENTA
                                    );
                                csv.AppendLine(newLine);
                            }
                        }
                    }
                    else if (item.nombre.ToUpper().Contains("CONCILIACION"))
                    {
                        //if (item.idBanco.idBanco == 47)
                        dt.Rows.Remove(dt.Rows[0]);
                        foreach (DataRow row in dt.Rows)
                        {
                            result = null;
                            int sucursal = 0;
                            if (item.idBanco.idBanco == 47 && row[6].ToString().Contains("SUCURSAL"))
                            {
                                int split_suc = 0;
                                if (int.TryParse(row[6].ToString().Split(' ')[4], out split_suc))
                                {
                                    sucursal = split_suc;
                                    string concepto = row[6].ToString().Replace(""+ row[6].ToString().Split(' ')[4], "");
                                    result = lista.listaElementos.Find(x => x.FormaPago == concepto.Trim());
                                }
                            }
                            else
                            {
                                sucursal = int.Parse(row[3].ToString().Trim());
                                result = lista.listaElementos.Find(x => x.codigo == row[10].ToString().Trim());
                            }
                            if (result == null)
                                result = lista.listaElementos.Find(x => x.FormaPago == row[8].ToString().Trim());
                            if (row[8].ToString().Trim().Contains("TRASP "))
                                result = lista.listaElementos.Find(x => x.FormaPago.Contains("TRASP "));
                            else if (row[8].ToString().Trim().Contains("127017201899183"))
                                result = lista.listaElementos.Find(x => x.FormaPago.Contains("127017201899183"));
                            else if (row[8].ToString().Trim().Contains("DEP 017201899183042"))
                                result = lista.listaElementos.Find(x => x.FormaPago.Contains("DEP 017201899183042"));
                            else if (row[8].ToString().Trim().Contains("Compra de saldo MTCenter"))
                                result = lista.listaElementos.Find(x => x.FormaPago.Contains("Compra de saldo MTCenter"));

                            if (result != null)
                            {
                                if (result.codigo == "")
                                {
                                    result.codigo = row[10].ToString().Trim();
                                }

                                DateTime fecha = DateTime.Parse(row[1].ToString().Substring(8, 2) + "/" + row[1].ToString().Substring(5, 2) + "/" + row[1].ToString().Substring(0, 4));

                                if (fecha.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    fecha = fecha.AddDays(1);
                                }

                                String importe = row[7].ToString().Remove(0, 1).Replace(",", "").Replace("$", "").Trim();
                                string decimales = "";
                                if (importe.Contains("."))
                                {
                                    decimales = "0." + importe.Split('.')[1];
                                    if (decimales.Length == 3)
                                        decimales = decimales + "0";
                                }
                                fechaArchivo = fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0') + "-" + fecha.Year;
                                
                                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                    fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year, //fecha
                                    DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, //hora
                                    sucursal, //sucursal
                                    row[4].ToString().Trim(), //referencia
                                    importe, //importe
                                    result.idCatalogo,//IDCATALOGO
                                    result.codigo,//CODIGO CONCEPTO
                                    (decimales == "0.00") ? "NOREF" : decimales, //concepto/ref Interbancaria /// decimales
                                    row[8].ToString().Trim(), //Descripcion -- catalogo
                                    item.idBanco.idBanco//ID CUENTA
                                    );
                                csv.AppendLine(newLine);
                            }
                        }
                    }

                }
                //Inbursa
                else if (item.idBanco.idBanco == 59)
                {
                    fechaArchivo = "";
                    string horaArchivo = DateTime.Now.Hour + "-" + DateTime.Now.Minute;
                    int autoInc = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        result = lista.listaElementos.Find(x => x.FormaPago == row[5].ToString().Trim());
                        if (result != null)
                        {
                            DateTime fecha = DateTime.Parse(row[0].ToString().Trim());//.Substring(4,2) +"/"+ row[1].ToString().Trim().Substring(6, 2)+"/"+ row[1].ToString().Trim().Substring(0, 4);
                            DateTime tme = DateTime.Parse(row[10].ToString().Trim());
                            hora = tme.Hour + ":" + tme.Minute + ":" + tme.Second;
                            string decimales = "";

                            if (row[7].ToString().Trim().Contains(".") || row[6].ToString().Trim().Contains("."))
                            {
                                if (result.idCatalogo == 11)
                                    decimales = "0." + row[6].ToString().Trim().Split('.')[1];
                                else
                                    decimales = "0." + row[7].ToString().Trim().Split('.')[1];

                                if (decimales.Length == 3)
                                    decimales = decimales + "0";
                            }
                            autoInc++;
                            fechaArchivo = fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0') + "-" + fecha.Year;
                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year,
                                hora,
                                0,
                                autoInc.ToString(),
                                (result.idCatalogo == 11) ? "-" + row[6].ToString().Trim() : row[7].ToString().Trim(),
                                result.idCatalogo,
                                result.codigo,
                                (decimales == "") ? "NOREF" : decimales,/////
                                row[5].ToString().Trim().Replace(",", " ") + " " + row[1].ToString().Trim().Replace(",", " ")
                                + " " + row[3].ToString().Trim().Replace(",", " ") + " " + row[4].ToString().Trim().Replace(",", " "),
                                item.idBanco.idBanco);
                            csv.AppendLine(newLine);

                        }
                    }
                }
                //////BANORTE
                else if (item.idBanco.idBanco == 45 || item.idBanco.idBanco == 62)
                {
                    fechaArchivo = "";
                    string horaArchivo = DateTime.Now.Hour + "-" + DateTime.Now.Minute;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row[6].ToString().Trim() == "4666")
                            result = lista.listaElementos.Find(x => x.FormaPago == "Cajero ATM");
                        else if (row[6].ToString().Trim() != "4666" && row[5].ToString().Trim() == "0")
                            result = lista.listaElementos.Find(x => x.FormaPago == "DEP.EFECTIVO 01374");
                        else
                            result = lista.listaElementos.Find(x => x.codigo == row[5].ToString().Trim());

                        if (row[6].ToString().Trim() == "4253")
                            result = lista.listaElementos.Find(x => x.nombre == "TELECOMM");
                        if (result != null)
                        {
                            DateTime fecha = DateTime.Parse(row[1].ToString().Trim()); 
                            hora = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;//tme.Hour + ":" + tme.Minute + ":" + tme.Second;

                            fechaArchivo = fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0') + "-" + fecha.Year;
                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year,//fecha
                                hora,//Hora
                                row[6].ToString().Trim(),//Sucursal
                                row[10].ToString().Trim().Replace(',', ' '),//movimiento - no Auto
                                (row[7].ToString().Trim().Equals("-")) ? "-" + row[8].ToString().Trim().Replace(",", "") : row[7].ToString().Trim().Replace(",", ""),//costo
                                result.idCatalogo,//idCatalogo
                                result.codigo,//idCodigo
                                row[3].ToString().Trim() + "/" + row[11].ToString().Trim().Replace(",", ""),/////Referencia
                                row[4].ToString().Trim().Replace(",", ""),//Descripción
                                item.idBanco.idBanco);
                            csv.AppendLine(newLine);

                        }
                    }
                }
                ///////BAJÍO
                else if (item.idBanco.idBanco == 25 || item.idBanco.idBanco == 52)
                {
                    fechaArchivo = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        result = null;
                        //Aqui se separan por los dos puntos ":" para obtener el primer bloque de texto y comparar con la lista de conceptos
                        //Si no se tienen los dos puntos, se tomarán tal cual el texto y se compará con la lista
                        String txtConcepto = (row[2].ToString() != null) ? row[2].ToString().Split(':')[0].Trim().ToUpper() : "";
                        for (int i = 0; i < lista.listaElementos.Count; i++)
                        {
                            string items = lista.listaElementos[i].FormaPago.ToUpper();
                            if (txtConcepto.Contains(items.ToString().ToUpper()))
                            {
                                result = lista.listaElementos[i];
                                break;
                            }
                        }
                        //Aqui se busca y se extrae la referencia
                        String[] listStri = row[2].ToString().Split(':');
                        string referencia = "";
                        int cont = 0;
                        foreach (string items in listStri)
                        {
                            if (items.Contains("Referencia"))
                                cont = 1;
                            if (items.Contains("Hora") && cont == 1)
                            {
                                referencia = items.Replace(" Hora", "");
                            }
                        }
                        if (result != null)
                        {
                            DateTime fecha = DateTime.Parse(row[0].ToString().Substring(0, 4) + "/" + row[0].ToString().Substring(4, 2) + "/" + row[0].ToString().Substring(6, 2));
                            hora = DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;//tme.Hour + ":" + tme.Minute + ":" + tme.Second;

                            fechaArchivo = fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0') + "-" + fecha.Year;
                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year,//fecha
                                hora,//Hora
                                0,//Sucursal
                                row[1].ToString().Trim().Replace(',', ' '),//movimiento - no Auto
                                row[5].ToString().Trim().Replace(",", ""),//costo
                                result.idCatalogo,//idCatalogo
                                result.codigo,//idCodigo
                                (referencia != "") ? referencia : "NOREF",/////Referencia
                                row[2].ToString().Trim().Replace(",", ""),//Descripción
                                item.idBanco.idBanco);
                            csv.AppendLine(newLine);

                        }
                    }
                }
                //Bancomer 
                else if (item.idBanco.idBanco == 39 || item.idBanco.idBanco == 16 || item.idBanco.idBanco == 27 || item.idBanco.idBanco == 18)
                {
                    fechaArchivo = "";
                    if (item.ext == "exp")
                    {
                        if (dt != null)
                        {
                            dt.Rows.RemoveAt(0);
                            dt = extraeCampos(dt);
                            foreach (DataRow row in dt.Rows)
                            {
                                result = null;
                                result = lista.listaElementos.Find(x => x.codigo == row[6].ToString().Trim());
                                
                                if (result != null)
                                {
                                    DateTime fecha = DateTime.Parse(row[11].ToString().Trim());
                                    DateTime tme = DateTime.Parse(row[10].ToString().Trim());
                                    hora = tme.Hour + ":" + tme.Minute + ":" + tme.Second;
                                    string referencia = "";
                                    referencia = row[0].ToString().Trim();
                                    switch (result.nombre)
                                    {
                                        case "EFECTIVO":
                                            referencia = row[0].ToString().Trim();
                                            break;
                                        case "CHEQUE":
                                            referencia = row[0].ToString().Trim();
                                            break;
                                        case "TRANSFERENCIA ELECTRONICA":
                                            referencia = row[1].ToString().Trim();
                                            break;
                                    }
                                    switch (result.codigo)
                                    {
                                        case "AA7":
                                            referencia = row[3].ToString().Trim().Remove(0, row[3].ToString().Trim().ToUpper().IndexOf("FOLIO") + 6);
                                            break;
                                        case "T20":
                                            referencia = row[3].ToString().Trim();
                                            break;
                                        case "W02":
                                            referencia = row[3].ToString().Trim();
                                            break;
                                    }


                                    fechaArchivo = fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0') + "-" + fecha.Year;
                                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                        fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year,//fecha
                                        hora,//Hora
                                        row[8].ToString().Trim(),//Sucursal
                                        row[0].ToString().Trim().Replace(',', ' '),//movimiento - no Auto
                                        (row[7].ToString().Trim().Equals("1")) ? "-" + double.Parse(row[4].ToString().Trim().Replace(",", "")).ToString() : double.Parse(row[4].ToString().Trim().Replace(",", "")).ToString(),
                                        //(result.codigo.Equals("11")) ? "-" + row[5].ToString().Trim().Replace(",", "") : row[5].ToString().Trim().Replace(",", ""),//costo
                                        result.idCatalogo,//idCatalogo
                                        result.codigo,//idCodigo
                                        referencia,/////Referencia
                                        row[2].ToString().Trim().Replace(",", "") + " " + row[1].ToString().Trim().Replace(",", "") + " " + row[3].ToString().Trim().Replace(",", ""),//Descripción
                                        item.idBanco.idBanco);
                                    csv.AppendLine(newLine);

                                }
                            }
                        }
                    }
                    else if (item.ext.Contains("xls"))
                    {
                        if (item.nombre.Contains("BBVA COM"))
                        {
                            dtBancomer = new DataTable();
                            fechaArchivo = DateTime.Now.ToShortDateString().ToString();
                            int noAuto = 0;
                            //Encuentra la ultima plantilla de la cuenta
                            //string url = "f:wwwroot/portal/backoffice/liberaciones/";
                            string url = Path.Combine(_hostingEnvironment.WebRootPath, "files", "");
                            ////string url = AppDomain.CurrentDomain.BaseDirectory + "/liberaciones/";
                            string[] files = Directory.GetFiles(url, "Bancomer.com-plantilla2-" + item.idBanco.idBanco.ToString() + ".csv", SearchOption.TopDirectoryOnly);
                            FileInfo[] file = new FileInfo[files.Length];
                            DateTime fechaCreacion = DateTime.Parse(fechaArchivo);
                            string nombreArchivoPos = "";
                            bool existe = false;
                            for (int i = 0; i < files.Length; i++)
                            {
                                file[i] = new FileInfo(files[i]);
                                string str = file[i].LastWriteTime.ToShortDateString();
                                if (fechaArchivo.Equals(str))
                                {
                                    fechaCreacion = file[i].LastWriteTime;
                                    nombreArchivoPos = file[i].Name;
                                    existe = true;
                                }
                                else
                                {
                                    existe = false;
                                    file[i].Delete();
                                    file[i].Refresh();
                                }
                            }

                            if (existe)
                            {
                                noAuto = obtenerAutoDeArchivo(url + "/"+ nombreArchivoPos);
                            }
                            else
                            {
                                data = Seguridad.Encrypt(JsonConvert.SerializeObject(item.idBanco));
                                reqResponse = await request.doPost("Liberaciones/LiberarBancos/getLastNoAutoBancomer", data, HttpContext.Session.GetString("token"));
                                cuenta respuestaCuenta = JsonConvert.DeserializeObject<cuenta>(reqResponse);
                                noAuto = respuestaCuenta.idBanco;
                                noAuto = noAuto + 1;
                            }
                            DataTable table = new DataTable();
                            dt.Rows.RemoveAt(0);
                            table = voltearFilas(dt);
                            if (dtBancomer.Rows.Count > 0)
                                table = comparaArchivos(dt);
                            if (table != null)
                            {
                                foreach (DataRow item1 in table.Rows)
                                {
                                    result = null;
                                    string[] formaP = item1[1].ToString().Split('/');
                                    result = lista.listaElementos.Find(x => x.FormaPago == formaP[0].Trim());
                                    if (result != null)
                                    {
                                        DateTime fecha = DateTime.Parse(item1[0].ToString().Trim());

                                        if (!string.IsNullOrEmpty(item1[5].ToString()))
                                            noAuto = int.Parse(item1[5].ToString());
                                        string referencia = noAuto.ToString();
                                        switch (result.nombre)
                                        {
                                            case "CAJERO ATM":
                                                referencia = formaP[1].Trim().Remove(0, formaP[1].Trim().IndexOf("FOLIO") + 6);
                                                break;
                                            case "CHEQUE":
                                                referencia = noAuto.ToString();
                                                if (result.FormaPago.Equals("DEP.CHEQUES DE OTRO BANCO"))
                                                    fecha = DateTime.Parse(fecha.AddDays(1).ToString());
                                                break;
                                            case "TRANSFERENCIA ELECTRONICA":
                                                if (result.FormaPago.Contains("PAGO CUENTA DE TERCERO"))
                                                    referencia = formaP[1].ToString().Trim().Substring(0, 10);
                                                else if (result.FormaPago.Contains("SPEI"))
                                                    referencia = (formaP[1].Equals("IXE")) ? formaP[2].ToString().Trim().Remove(0, 12) : formaP[1].ToString().Trim().Remove(0, 12);
                                                break;

                                        }
                                        hora = "00:00:00";

                                        var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                            fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year,//fecha
                                            hora,//Hora
                                            0,//Sucursal
                                            noAuto,//movimiento - no Auto
                                            (string.IsNullOrEmpty(item1[2].ToString())) ? item1[3] : "-" + item1[2],//costo
                                            result.idCatalogo,//idCatalogo
                                            result.idCatalogo,//idCodigo
                                            referencia,/////Referencia
                                            item1[1].ToString().Trim(),//Descripción
                                            item.idBanco.idBanco
                                            );
                                        csv.AppendLine(newLine);

                                        var newLinePlantilla = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                            fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year,//fecha
                                            hora,//Hora
                                            0,//Sucursal
                                            noAuto,//movimiento - no Auto
                                            (string.IsNullOrEmpty(item1[2].ToString())) ? item1[3] : "-" + item1[2],//costo
                                            result.idCatalogo,//idCatalogo
                                            result.idCatalogo,//idCodigo
                                            referencia,/////Referencia
                                            item1[1].ToString().Trim(),//Descripción
                                            item.idBanco.idBanco,
                                            item1[4].ToString().Trim() // SALDO SOLO PARA BANCOMER.COM
                                            );
                                        csvPlantilla.AppendLine(newLinePlantilla);
                                    }
                                    noAuto++;
                                }
                            }
                            byte[] archivoPlantilla = Encoding.UTF8.GetBytes(csvPlantilla.ToString());
                            System.IO.File.WriteAllBytes(@"" + url + "/Bancomer.com-plantilla2-" + item.idBanco.idBanco.ToString() + ".csv", archivoPlantilla);


                        }
                        else
                        {
                            if (dt != null)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    result = null;
                                    result = lista.listaElementos.Find(x => x.codigo == row[10].ToString().Trim());
                                    if (result != null)
                                    {
                                        DateTime fecha = DateTime.Parse(DateTime.Today.AddDays(-1).ToString());
                                        hora = "00:00:00";
                                        string referencia = "";
                                        referencia = row[9].ToString().Trim();
                                        switch (result.nombre)
                                        {
                                            case "EFECTIVO":
                                                referencia = row[9].ToString().Trim();
                                                break;
                                            case "CHEQUE":
                                                referencia = row[9].ToString().Trim();
                                                break;
                                            case "TRANSFERENCIA ELECTRONICA":
                                                referencia = row[15].ToString().Trim();
                                                break;
                                        }
                                        switch (result.codigo)
                                        {
                                            case "AA7":
                                                referencia = row[16].ToString().Trim().Remove(0, row[16].ToString().Trim().ToUpper().IndexOf("FOLIO") + 6);
                                                break;
                                            case "T20":
                                                referencia = row[16].ToString().Trim();
                                                break;
                                            case "W02":
                                                referencia = row[16].ToString().Trim();
                                                break;
                                        }


                                        fechaArchivo = fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0') + "-" + fecha.Year;
                                        var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                            fecha.Month.ToString().PadLeft(2, '0') + "/" + fecha.Day.ToString().PadLeft(2, '0') + "/" + fecha.Year,//fecha
                                            hora,//Hora
                                            "0",//Sucursal
                                            row[9].ToString().Trim(),//movimiento - no Auto
                                            (result.idCatalogo == 11) ? "-" + double.Parse(row[13].ToString().Trim()).ToString() : double.Parse(row[13].ToString().Trim()).ToString(),//costo
                                                                                                                                                                                 //(result.codigo.Equals("11")) ? "-" + row[5].ToString().Trim().Replace(",", "") : row[5].ToString().Trim().Replace(",", ""),//costo
                                            result.idCatalogo,//idCatalogo
                                            result.codigo,//idCodigo
                                            referencia,/////Referencia
                                            row[11].ToString().Trim().Replace(",", "") + " " + row[15].ToString().Trim().Replace(",", "") + " " + row[16].ToString().Trim().Replace(",", ""),//Descripción
                                            item.idBanco.idBanco);
                                        csv.AppendLine(newLine);

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        response.CodigoRespuesta = -5;
                        response.MensajeRespuesta = "Error: El tipo de archivo es diferente a los permitidos.";
                    }

                }
                //Santander
                else if (item.idBanco.idBanco == 15 || item.idBanco.idBanco == 41 || item.idBanco.idBanco == 43 || item.idBanco.idBanco == 37 || item.idBanco.idBanco == 50 || item.idBanco.idBanco == 49 || item.idBanco.idBanco == 85)
                {
                    fechaArchivo = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        result = lista.listaElementos.Find(x => x.FormaPago == row[4].ToString().Trim());
                        if (result != null)
                        {
                            var idCat = result.idCatalogo;

                            if (item.idBanco.idBanco == 15 && result.idCatalogo == 1 && row[8].ToString() == "00000000")
                            {
                                idCat = 4;
                            }


                            DateTime fecha = DateTime.Parse(row[1].ToString().Replace("'", "").Substring(0, 2) + "/" + row[1].ToString().Replace("'", "").Substring(2, 2) + "/" + row[1].ToString().Replace("'", "").Substring(4, 4));

                            if (row[4].ToString().Trim() == "DEP S B COBRO")
                                fecha.AddDays(1);

                            string decimales = "0.00";
                            if (row[6].ToString().Contains('.'))
                                decimales = "0." + row[6].ToString().Split('.')[1];

                            if (decimales.Length == 3)
                                decimales = decimales + "0";

                            fechaArchivo = fecha.Month.ToString().PadLeft(2, '0') + "-" + fecha.Day.ToString().PadLeft(2, '0') + "-" + fecha.Year;
                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                    fecha.Month + "/" + fecha.Day + "/" + fecha.Year,
                                    row[2].ToString().Trim(),
                                    int.Parse(row[3].ToString().Trim().Replace("'", "")),
                                    row[8].ToString().Trim().Replace("'", ""),
                                    ((row[5].ToString() == "+") ? row[6].ToString() : "-" + row[6].ToString()),
                                    idCat,
                                    idCat,
                                    ((row[9].ToString().Trim() == "") ? ((decimales == "0.00") ? "NOREF" : decimales.ToString()) : row[9].ToString().Trim()),
                                    row[4].ToString().Trim(),
                                    item.idBanco.idBanco
                                );
                            csv.AppendLine(newLine);
                        }
                    }
                }
                //STP
                else if (item.idBanco.idBanco == 72)
                {
                    csv = new StringBuilder();
                    dt.Rows.RemoveAt(0);
                    dt.Rows.RemoveAt(0);
                    fechaArchivo = "";
                    if (item.nombre.ToUpper().Contains("ENVIADAS"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (!row[14].ToString().Trim().ToUpper().Contains("CANCELADA"))
                            {
                                hora = row[9].ToString().Trim();
                                fechaArchivo = row[8].ToString().Trim().Substring(4, 2) + "/" + row[8].ToString().Trim().Substring(6, 2) + "/" + row[8].ToString().Trim().Substring(0, 4);
                                string cadena = "";
                                if (row[4].ToString().Length > 20)
                                    cadena = " - ID " + row[4].ToString().Remove(0, 22);

                                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                    fechaArchivo,//fecha
                                    hora.Remove(8),//Hora
                                    0,//Sucursal
                                    row[0].ToString().Trim().Replace(',', ' '),//movimiento - no Auto
                                    "-" + row[11].ToString().Trim().Replace(",", ""),//costo
                                    11,//idCatalogo
                                    "NULL",//idCodigo
                                    row[5].ToString().Trim().Replace(",", ""),/////Referencia

                                    row[1].ToString().Trim().Replace(",", "") + " " + row[3].ToString().Trim().Replace(",", "") + " " + row[4].ToString().Trim().Replace(",", "")
                                    + " " + row[5].ToString().Trim().Replace(",", "") + " " + row[10].ToString().Trim().Replace(",", "") + " " + row[16].ToString().Trim().Replace(",", "")
                                    + " " + row[18].ToString().Trim().Replace(",", "") + cadena, //Descripción
                                    item.idBanco.idBanco);
                                csv.AppendLine(newLine);
                            }
                        }
                        fechaArchivo = fechaArchivo.Replace("/", "-");
                        item.nombre = "STP-ENVIADAS-";
                    }
                    else if (item.nombre.ToUpper().Contains("RECIBIDAS"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            hora = row[9].ToString().Trim();
                            fechaArchivo = row[8].ToString().Trim().Substring(4, 2) + "/" + row[8].ToString().Trim().Substring(6, 2) + "/" + row[8].ToString().Trim().Substring(0, 4);
                            string cadena = "";
                            if (row[4].ToString().Length > 20)
                                cadena = " - ID " + row[4].ToString().Remove(0, 22);

                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                fechaArchivo,//fecha
                                hora.Remove(8),//Hora
                                0,//Sucursal
                                row[0].ToString().Trim().Replace(",", ""),//movimiento - no Auto
                                row[13].ToString().Trim().Replace(",", ""),//costo
                                3,//idCatalogo
                                "NULL",//idCodigo
                                row[5].ToString().Trim().Replace(",", ""),/////Referencia
                                row[1].ToString().Trim().Replace(',', ' ') + " " + row[3].ToString().Trim().Replace(',', ' ') + " " + row[4].ToString().Trim().Replace(',', ' ')
                                + " " + row[5].ToString().Trim().Replace(',', ' ') + " " + row[15].ToString().Trim().Replace(',', ' ') + " " + row[10].ToString().Trim().Replace(',', ' ')
                                + " " + row[11].ToString().Trim().Replace(',', ' ') + cadena, //Descripción
                                item.idBanco.idBanco);
                            csv.AppendLine(newLine);
                        }
                        fechaArchivo = fechaArchivo.Replace("/", "-");
                        item.nombre = "STP-RECIBIDAS-";
                    }
                }


                byte[] archivo = Encoding.UTF8.GetBytes(csv.ToString());
                System.IO.File.WriteAllBytes(@""+__appSettings.ArchivosBancosDir + item.nombre.ToUpper() + " " + fechaArchivo + ".csv", archivo);
                //System.IO.File.WriteAllBytes(@"C:\archivos_bancos\" + item.nombre.ToUpper() + " " + fechaArchivo + ".csv", archivo);

                BitacoraModel bitacora = new BitacoraModel();
                bitacora.idCatalogoSP = 75;
                bitacora.idCatalogoTabla = 60;
                bitacora.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                bitacora.ip = Seguridad.GetIPAddress();
                bitacora.llave = 0;
                bitacora.parametros = "Archivo guardado exitosamente.";

                string dataBitacora = Seguridad.Encrypt(JsonConvert.SerializeObject(bitacora));
                string reqResponseBitacora = await request.doPost("Bitacora/registraBitacora", dataBitacora, HttpContext.Session.GetString("token"));

                util.logActividadUsuario(new ActividadUsuario
                {
                    IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                    Usuario = HttpContext.Session.GetString("user"),
                    Seccion = "Liberaciones",
                    Actividad = "libera depositos de banco",
                    Detalles = "Se sube un archivo con los depositos de banco para liberarse automáticamente",
                    Objeto = JsonConvert.SerializeObject(item)
                }, __appSettings.LoggerLiberaciones);
                response.insertado = true;
                response.CodigoRespuesta = 0;
                response.MensajeRespuesta = "Archivo generado correctamente";
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("LiberacionBancos/setElementosBancos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_20")]
        public async Task<LiberacionesResponseSubirLiberacionDeposito> setElementoBanco([FromBody] LiberacionesRequestSetConceptoBancas item)
        {
            LiberacionesResponseSubirLiberacionDeposito response = new LiberacionesResponseSubirLiberacionDeposito();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/LiberarBancos/setElementosBancos", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<LiberacionesResponseSubirLiberacionDeposito>(reqResponse);
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

        [Route("LiberacionBancos/getConceptosBancas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_20")]
        public async Task<LiberacionesResponseConceptosBancos> getConceptosBancas()
        {
            LiberacionesResponseConceptosBancos response = new LiberacionesResponseConceptosBancos();
            try
            {

                string reqResponse = await request.doPost("Liberaciones/LiberarBancos/getConceptosBancos", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseConceptosBancos>(reqResponse);
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

        public int obtenerAutoDeArchivo(string file)
        {
            int result = 0;
            System.IO.File.SetAttributes(file, FileAttributes.Normal);
            //StreamReader sr = File.OpenText(file);
            StreamReader sr = new StreamReader(file);
            dtBancomer = csvBancomer(sr);
            result = int.Parse(dtBancomer.Rows[0][3].ToString());
            sr.Close();
            return result;
        }
        public static DataTable csvBancomer(StreamReader sr)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < 11; i++)
            {
                dt.Columns.Add("");
            }
            while (!sr.EndOfStream)
            {
                string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < 11; i++)
                {
                    dr[i] = rows[i].Replace("\"", "").Replace("\t", "");
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public DataTable voltearFilas(DataTable dt)
        {
            DataTable result = new DataTable();
            result.Columns.Add("Column0");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                result.Columns.Add(dt.Columns[i].Namespace.ToString());
            }
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dt.Rows[i];
                result.ImportRow(dr);
            }
            return result;
        }
        public DataTable comparaArchivos(DataTable dtNuevo)
        {
            DataTable result = new DataTable();
            List<int> posiciones = new List<int>();
            bool bandera = true;
            //Se crean las columnas de la tabla final
            result.Columns.Add("Column0");
            for (int i = 0; i < dtNuevo.Columns.Count; i++)
            {
                result.Columns.Add(dtNuevo.Columns[i].Namespace.ToString());
            }
            //los datos de la tabla guardada estan en dtBancomer

            foreach (DataRow item in dtBancomer.Rows)
            {
                for (int i = 0; i < dtNuevo.Rows.Count; i++)
                {
                    DataRow rowNuevo = dtNuevo.Rows[i];
                    if (
                        item[8].Equals(rowNuevo[1].ToString().Trim())
                        && (item[4].ToString().Replace("-", "").Equals(rowNuevo[2]) || item[4].ToString().Replace("-", "").Equals(rowNuevo[3]))
                        && (item[10].Equals(rowNuevo[4]))
                        )
                    {
                        if (string.IsNullOrEmpty(rowNuevo[5].ToString()))
                        {
                            if (bandera)
                            {
                                rowNuevo[5] = item[3];
                            }
                        }
                        else
                        {
                            //se cambia la bandera a falso para que ya no ponga los numeros anteriores ya que se encontró un desface
                            bandera = false;
                        }
                        result.ImportRow(rowNuevo);
                        posiciones.Add(i);
                        break;
                    }
                }
            }
            //Ordena de menor a mayor la lista
            posiciones.Sort();
            //Elimina uno por uno las filas que existen en la lista para no repetir depositos
            for (int i = posiciones.Count - 1; i >= 0; i--)
            {
                dtNuevo.Rows.RemoveAt(posiciones[i]);
            }
            //agrega los depositos nuevos a la tabla final
            for (int i = 0; i < dtNuevo.Rows.Count; i++)
            {
                result.ImportRow(dtNuevo.Rows[i]);
            }

            return result;
        }
        public DataTable extraeCampos(DataTable table)
        {
            DataTable dataTable = new DataTable();
            try
            {
                dataTable.Columns.Add("Movimiento");
                dataTable.Columns.Add("Referencia");
                dataTable.Columns.Add("Descripcion");
                dataTable.Columns.Add("Descripcion 2");
                dataTable.Columns.Add("Monto");
                dataTable.Columns.Add("Saldo");
                dataTable.Columns.Add("Codigo");
                dataTable.Columns.Add("cargo/abono");
                dataTable.Columns.Add("Sucursal");
                dataTable.Columns.Add("Fecha Operacion");
                dataTable.Columns.Add("Hora");
                dataTable.Columns.Add("Fecha Aplicacion");
                dataTable.Columns.Add("Fecha consulta");
                foreach (DataRow item in table.Rows)
                {

                    dataTable.Rows.Add(new Object[] {
                        item[0].ToString().Substring(4, 7),
                        item[0].ToString().Substring(11, 15),
                        item[0].ToString().Substring(26, 25),
                        item[0].ToString().Substring(51, 37),
                        item[0].ToString().Substring(89, 16),
                        item[0].ToString().Substring(105, 16),
                        item[0].ToString().Substring(121, 3),
                        item[0].ToString().Substring(88, 1),
                        item[0].ToString().Substring(124, 4),
                        item[0].ToString().Substring(128, 8),
                        item[0].ToString().Substring(136, 8),
                        item[0].ToString().Substring(144, 8),
                        item[0].ToString().Substring(152, 8) });
                }
            }
            catch (Exception ex)
            {

            }
            return dataTable;
        }

        [Route("LiberacionBancos/listadoCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_20")]
        public async Task<LiberacionesResponseListadoEstatusDepositosCuentas> LiberacionBancosListadoCuentas()
        {
            LiberacionesResponseListadoEstatusDepositosCuentas response = new LiberacionesResponseListadoEstatusDepositosCuentas();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/NuevoDeposito/listadoCuentas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoEstatusDepositosCuentas>(reqResponse);
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

        #region Coordinadores Activos

        [Route("CoordinadoresActivos/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_19")]
        public async Task<LiberacionesResponseListadoCoordinadores> getListadoCoordinadoresActivos()
        {
            LiberacionesResponseListadoCoordinadores response = new LiberacionesResponseListadoCoordinadores();
            try
            {               
                string reqResponse = await request.doPost("Liberaciones/CoordinadoresActivos/listado", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoCoordinadores>(reqResponse);
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


        [Route("CoordinadoresActivos/listadoUsuarios")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_19")]
        public async Task<LiberacionesResponseListadoCoordinadores> getListadoUsuariosCoordinadores()
        {
            LiberacionesResponseListadoCoordinadores response = new LiberacionesResponseListadoCoordinadores();
            try
            {
                string reqResponse = await request.doPost("Liberaciones/CoordinadoresActivos/listadoUsuarios", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponseListadoCoordinadores>(reqResponse);
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

        [Route("CoordinadoresActivos/setEstatusCoordinador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_19")]
        public async Task<LiberacionesResponEstatusCoordinador> setEstatusCoordinador([FromBody] LiberacionesRequestSetEstatusCoordinador item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {
               
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/CoordinadoresActivos/setEstatusCoordinador", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "CoordinadoresActivos/setEstatusCoordinador",
                        Detalles = "Editado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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


        [Route("CoordinadoresActivos/registraCoordinador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_19")]
        public async Task<LiberacionesResponEstatusCoordinador> registraCoordinador([FromBody] LiberacionesRequestSetEstatusCoordinador item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/CoordinadoresActivos/registraCoordinador", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "CoordinadoresActivos/registraCoordinador",
                        Detalles = "Registrado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        #region Control de cuentas


        [Route("ControlCuentas/listadoCadenasCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_21")]
        public async Task<LiberacionesResponseListadoCuentasBloqueo> getListaCadenaCuentas([FromBody] LiberacionesRequestCadenasCuentas item)
        {
            LiberacionesResponseListadoCuentasBloqueo response = new LiberacionesResponseListadoCuentasBloqueo();
            try
            {
                item.idCadena.Replace(" ","");
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/ControlCuentas/listadoCadenasCuentas", data, HttpContext.Session.GetString("token"));
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

        [Route("ControlCuentas/bloqueoLiberacionFull")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_21")]
        public async Task<LiberacionesResponEstatusCoordinador> bloqueoLiberacionFull([FromBody] LiberacionesRquestModificarLiberacion item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {
                item.opcion = 5;
                item.id = 0;
                item.ip = Seguridad.GetIPAddress();
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);

                    if (response.CodigoRespuesta == -2)
                    {
                        item.opcion = 7;
                        item.idBanco = 0;
                        data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                        reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                        response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    }

                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "ControlCuentas/listadoCadenasCuentas",
                        Detalles = "BloqueoLiberacionFUll",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        [Route("ControlCuentas/deleteBloqueoLiberacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_21")]
        public async Task<LiberacionesResponEstatusCoordinador> deleteBloqueoLiberacion([FromBody] LiberacionesRquestModificarLiberacion item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {
                item.opcion = 3;
                item.idCadena = 0;
                item.idBanco = 0;
                item.ip = Seguridad.GetIPAddress();
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "ControlCuentas/listadoCadenasCuentas",
                        Detalles = "deleteBloqueoLiberacion",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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


        [Route("ControlCuentas/deleteBloqueoExLiberacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_21")]
        public async Task<LiberacionesResponEstatusCoordinador> deleteBloqueoExLiberacion([FromBody] LiberacionesRquestModificarLiberacion item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {
                item.opcion = 4;
                item.id = 0;
                item.ip = Seguridad.GetIPAddress();
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "ControlCuentas/listadoCadenasCuentas",
                        Detalles = "Desbloqueo de cuentas",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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


        [Route("ControlCuentas/addBloqueoLiberacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_21")]
        public async Task<LiberacionesResponEstatusCoordinador> bloqueoLiberacion([FromBody] LiberacionesRquestModificarLiberacion item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {
                item.opcion = 2;
                item.id = 0;
                item.ip = Seguridad.GetIPAddress();
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "ControlCuentas/listadoCadenasCuentas",
                        Detalles = "Bloqueo de cuentas",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        [Route("ControlCuentas/borrarBloqueo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_21")]
        public async Task<LiberacionesResponEstatusCoordinador> borrarBloqueo([FromBody] LiberacionesRquestModificarLiberacion item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {
                item.opcion = 6;
                item.idBanco = 0;
                item.id = 0;
                item.ip = Seguridad.GetIPAddress();
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "ControlCuentas/listadoCadenasCuentas",
                        Detalles = "Bloqueo eliminado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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


        [Route("ControlCuentas/archivo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_21")]
        public async Task<LiberacionesResponseArchivoBloqueo> ControlCuentasArchivo([FromBody] LiberacionesRequestBloqueoCuentas item)
        {
            LiberacionesResponseArchivoBloqueo response = new LiberacionesResponseArchivoBloqueo();
            try
            {
                if (!string.IsNullOrEmpty(item.archivo))
                {
                    if (item.ext == "xls" || item.ext == "xlsx")
                    {

                        ExcelPackage workbook = new ExcelPackage();
                        var fileContent = File(Convert.FromBase64String(item.archivo), "application/vnd.ms-excel");
                        byte[] file = fileContent.FileContents;

                        using (MemoryStream ms = new MemoryStream(file))
                        using (ExcelPackage package = new ExcelPackage(ms))
                        {
                            if (package.Workbook.Worksheets.Count != 0)
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                                int rowCount = worksheet.Dimension.End.Row;     //get row count
                                for (int row = 2; row <= rowCount; row++)
                                {
                                    if (worksheet.Cells[row, 1].Value != null)
                                    {
                                        //if (int.Parse(worksheet.Cells[row, 9].Value.ToString().Trim()) == 1)
                                        if (worksheet.Cells[row, 9].Value != null)
                                        {
                                            if (int.Parse(worksheet.Cells[row, 9].Value.ToString()) == 1)
                                            {
                                                // si tiene todos los bancos
                                                await bloqueoLiberacionAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 1, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await bloqueoLiberacionAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 2, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await bloqueoLiberacionAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 4, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await bloqueoLiberacionAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 11, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await bloqueoLiberacionAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 12, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await bloqueoLiberacionAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 16, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await bloqueoLiberacionAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 25, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });

                                                response.registrado = true;
                                                response.CodigoRespuesta = 0;
                                                response.MensajeRespuesta = "Correcto validar cuentas";
                                            }
                                            else if (int.Parse(worksheet.Cells[row, 9].Value.ToString()) == 0)
                                            {
                                                // si tiene todos los bancos
                                                await deleteBloqueoExLiberacion(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 1, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await deleteBloqueoExLiberacion(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 2, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await deleteBloqueoExLiberacion(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 4, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await deleteBloqueoExLiberacion(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 11, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await deleteBloqueoExLiberacion(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 12, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await deleteBloqueoExLiberacion(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 16, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                await deleteBloqueoExLiberacion(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 25, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });

                                                response.registrado = true;
                                                response.CodigoRespuesta = 0;
                                                response.MensajeRespuesta = "Correcto validar cuentas";
                                            }
                                        }
                                        else
                                        {
                                            // no tiene todos
                                            for (int col = 1; col <= colCount; col++)
                                            {
                                                switch (col)
                                                {
                                                    case 2:

                                                        if (worksheet.Cells[row, col].Value != null)
                                                        {
                                                            if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 1)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 1, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            } else if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 0)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 1, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                        }

                                                        break;
                                                    case 3:
                                                        if (worksheet.Cells[row, col].Value != null)
                                                        {
                                                            //await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 5, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 2, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 1)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 2, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                            else if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 0)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 4, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                        }
                                                        break;
                                                    case 4:
                                                        if (worksheet.Cells[row, col].Value != null)
                                                        {
                                                            //await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 5, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 4, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 1)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 4, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                            else if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 0)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 4, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                        }
                                                        break;
                                                    case 5:
                                                        if (worksheet.Cells[row, col].Value != null)
                                                        {
                                                            //await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 5, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 11, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 1)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 11, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                            else if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 0)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 11, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                        }
                                                        break;
                                                    case 6:
                                                        if (worksheet.Cells[row, col].Value != null)
                                                        {
                                                            //await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 5, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 12, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 1)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 12, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                            else if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 0)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 12, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                        }
                                                        break;
                                                    case 7:
                                                        if (worksheet.Cells[row, col].Value != null)
                                                        {
                                                            //await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 5, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 16, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 1)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 16, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                            else if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 0)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 16, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                        }
                                                        break;
                                                    case 8:
                                                        if (worksheet.Cells[row, col].Value != null)
                                                        {
                                                            //await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 5, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 25, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 1)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 2, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 25, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                            else if (int.Parse(worksheet.Cells[row, col].Value.ToString()) == 0)
                                                            {
                                                                await bloqueoLiberacionFullAux(new LiberacionesRquestModificarLiberacion() { opcion = 4, id = 0, ip = Seguridad.GetIPAddress(), idBanco = 25, idCadena = int.Parse(worksheet.Cells[row, 1].Value.ToString()) });
                                                            }
                                                        }
                                                        break;
                                                }

                                            }

                                            response.registrado = true;
                                            response.CodigoRespuesta = 0;
                                            response.MensajeRespuesta = "Correcto validar cuentas";
                                        }
                                    }

                                   

                                }
                            }
                            else
                            {

                                response.registrado = false;
                                response.CodigoRespuesta = -1;
                                response.MensajeRespuesta = "El archivo no contiene hojas validas";
                            }

                            ms.Close();
                        }

                        
                    }
                    else
                    {
                        response.registrado = false;
                        response.CodigoRespuesta = -1;
                        response.MensajeRespuesta = "Archivo inválido";
                    }
                }
                else
                {
                    response.registrado = false;
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "Archivo inválido";
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

        public async Task<LiberacionesResponEstatusCoordinador> bloqueoLiberacionAux(LiberacionesRquestModificarLiberacion item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {
                item.opcion = 2;
                item.id = 0;
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "ControlCuentas/Archivo",
                        Detalles = "Bloqueo de cuentas",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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


        public async Task<LiberacionesResponEstatusCoordinador> bloqueoLiberacionFullAux([FromBody] LiberacionesRquestModificarLiberacion item)
        {
            LiberacionesResponEstatusCoordinador response = new LiberacionesResponEstatusCoordinador();
            try
            {
                item.opcion = item.opcion;
                item.id = 0;
                item.ip = Seguridad.GetIPAddress();
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);

                    if (response.CodigoRespuesta == -2)
                    {
                        item.opcion = 7;
                        item.idBanco = 0;
                        data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                        reqResponse = await request.doPost("Liberaciones/ControlCuentas/bloqueoLiberacion", data, HttpContext.Session.GetString("token"));
                        response = JsonConvert.DeserializeObject<LiberacionesResponEstatusCoordinador>(reqResponse);
                    }

                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Liberaciones",
                        Actividad = "ControlCuentas/Archivo",
                        Detalles = "BloqueoLiberacionFUll",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerLiberaciones);
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

        #region Depositos Rechazados 

        [Route("DepositosRechazados/listadoDepositosRechazados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_23")]
        public async Task<LiberacionesListaDepositoRechazadoResponse> getListaDepositosRechazados([FromBody] LiberacionesDepositosRechazadosRequest item)
        {
            LiberacionesListaDepositoRechazadoResponse response = new LiberacionesListaDepositoRechazadoResponse();
            try
            {
                item.idusuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.rol = int.Parse(HttpContext.Session.GetString("rol"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Liberaciones/DepositosRechazados/listadoDepositosRechazados", data, HttpContext.Session.GetString("token"));
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<LiberacionesListaDepositoRechazadoResponse>(reqResponse);
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

        [Route("DepositosRechazados/desbloquearDeposito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_04_23")]
        public async Task<LiberacionesDesbloquearDepositoResponse> desbloquearDeposito([FromBody] LiberacionesDesbloquearDepositoRequest item)
        {
            LiberacionesDesbloquearDepositoResponse response = new LiberacionesDesbloquearDepositoResponse();
            int[] roles = { 6, 21 };
            try
            {
                if (roles.Contains(int.Parse(HttpContext.Session.GetString("rol"))))
                //if (true)
                {
                    item.idusuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Liberaciones/DepositosRechazados/desbloquearDeposito", data, HttpContext.Session.GetString("token"));
                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<LiberacionesDesbloquearDepositoResponse>(reqResponse);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }
                else
                {
                    response.CodigoRespuesta = -401;
                    response.MensajeRespuesta = "El usuario no cuenta con los permisos necesarios";
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