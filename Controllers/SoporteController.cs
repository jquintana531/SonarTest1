using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MTC.WebApp.BackOffice.DataManager;
using MTC.WebApp.BackOffice.Helpers;
using MTC.WebApp.BackOffice.Models;
using MTCenter.GRPC.GDBBO.DBProtos;
using Newtonsoft.Json;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Soporte")]
    [ApiController]
    public class SoporteController : Controller
    {
        private readonly AppSettings __appSettings;
        private readonly ConfigurationSettings __configurations;
        private RequestHelper request;
        private Utils util;
        private readonly SoporteManager _manager;

        public SoporteController(IOptions<AppSettings> appSetings, IOptions<ConfigurationSettings> configurations, DBData.DBDataClient client)
        {
            this.__appSettings = appSetings.Value;
            this.__configurations = configurations.Value;
            this.request = new RequestHelper(this.__appSettings);
            this._manager = new SoporteManager(client);
            util = new Utils();
        }



        #region Administrar Clientes

        [Route("AdministrarClientes/informacionCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseInformacionCadena> listadoDepositos([FromBody] SoporteRequestInformacionCadena item)
        {
            SoporteResponseInformacionCadena response = new SoporteResponseInformacionCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/informacionCadena", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseInformacionCadena>(reqResponse);
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

        [Route("AdministrarClientes/getUsoCFDI")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public Task<SoporteResponseUsoCFDI> getUsoCFDI()
        {
            SoporteResponseUsoCFDI response = new SoporteResponseUsoCFDI();
            DataRequestReply reply = new DataRequestReply();

            try
            {
                reply = _manager.getUsoCFDI();

                if (reply.CodigoRespuesta == 0)
                {
                    response.listado = JsonConvert.DeserializeObject<List<usoCFDI>>(reply.Data.ToStringUtf8());
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

        [Route("AdministrarClientes/cambiarEstatus")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseActivaDesactivaCadena> cambiaEstatusCadena([FromBody] SoporteRequestActivaDesactivaCadena item)
        {
            SoporteResponseActivaDesactivaCadena response = new SoporteResponseActivaDesactivaCadena();
            try
            {
                item.tipoUsuario = 2;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/cambiarEstatus", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseActivaDesactivaCadena>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/cambiarEstatus",
                        Detalles = "Estatus cambiado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);
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

        [Route("AdministrarClientes/correoBienvenida")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseEnviaCorreoBienvenida> correoBienvenida([FromBody] SoporteRequestEnviaCorreoBienvenida item)
        {
            SoporteResponseEnviaCorreoBienvenida response = new SoporteResponseEnviaCorreoBienvenida();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/correoBienvenida", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEnviaCorreoBienvenida>(reqResponse);


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

        [Route("AdministrarClientes/listadoTipoEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoTipoEstablecimiento> listadoTipoEstablecimientos()
        {
            SoporteResponseListadoTipoEstablecimiento response = new SoporteResponseListadoTipoEstablecimiento();
            try
            {
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoTipoEstablecimiento", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoTipoEstablecimiento>(reqResponse);
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


        [Route("AdministrarClientes/listadoFamiliaModulo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoFamiliaModulo> listadoFamiliaModulo()
        {
            SoporteResponseListadoFamiliaModulo response = new SoporteResponseListadoFamiliaModulo();
            try
            {
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoFamiliaModulo", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoFamiliaModulo>(reqResponse);
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


        [Route("AdministrarClientes/nuevoEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseNuevoEstablecimiento> nuevoEstablecimiento([FromBody] SoporteRequestNuevoEstablecimiento item)
        {
            SoporteResponseNuevoEstablecimiento response = new SoporteResponseNuevoEstablecimiento();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/nuevoEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseNuevoEstablecimiento>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/nuevoEstablecimiento",
                        Detalles = "Alta exitosa",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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

        [Route("AdministrarClientes/cambiarEstatusEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseSetEstausEstablecimiento> cambiarEstatusEstablecimiento([FromBody] SoporteRequestSetEstatusEstablecimiento item)
        {
            SoporteResponseSetEstausEstablecimiento response = new SoporteResponseSetEstausEstablecimiento();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.tipoUsuario = 2;
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/cambiarEstatusEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseSetEstausEstablecimiento>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/cambiarEstatusEstablecimiento",
                        Detalles = "E exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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

        [Route("AdministrarClientes/listadoTipoFacturacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoFacturacion> listadoTipoFacturacion()
        {
            SoporteRequestListadoFactura item = new SoporteRequestListadoFactura();
            SoporteResponseListadoFacturacion response = new SoporteResponseListadoFacturacion();
            try
            {
                item.id = 0;
                item.menu = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoTipoFacturacion", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoFacturacion>(reqResponse);
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

        [Route("AdministrarClientes/listadoMetodoDePago")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoMetodoPago> listadoMetodoDePago()
        {
            SoporteRequestListadoMetodoPago item = new SoporteRequestListadoMetodoPago();
            SoporteResponseListadoMetodoPago response = new SoporteResponseListadoMetodoPago();
            try
            {
                item.id = 0;
                item.menu = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoMetodoDePago", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoMetodoPago>(reqResponse);
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

        [Route("AdministrarClientes/listadoGrupos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
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

        [Route("AdministrarClientes/listadoTipoCredito")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoTipoCredito> listadoTipoCredito()
        {
            SoporteResponseListadoTipoCredito response = new SoporteResponseListadoTipoCredito();
            try
            {
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoTipoCredito", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoTipoCredito>(reqResponse);
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

        [Route("AdministrarClientes/getEstados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoEstado> getEstados()
        {
            SoporteResponseListadoEstado response = new SoporteResponseListadoEstado();
            try
            {
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/getEstados", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoEstado>(reqResponse);
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

        [Route("AdministrarClientes/listadoFamiliasComerciales")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
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

        [Route("AdministrarClientes/listadoBancos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoBanco> listadoBancos()
        {
            SoporteRequestListadoBancos item = new SoporteRequestListadoBancos();
            SoporteResponseListadoBanco response = new SoporteResponseListadoBanco();
            try
            {
                item.activo = 1;
                item.menu = 1;
                item.pos = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoBancos", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoBanco>(reqResponse);
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

        [Route("AdministrarClientes/getBancosBloqueoLiberacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseBancosBloqueoLiberacion> getBancosBloqueoLiberacion([FromBody] SoporteRequestBloqueoLiberacion item)
        {
            SoporteResponseBancosBloqueoLiberacion response = new SoporteResponseBancosBloqueoLiberacion();
            try
            {
                item.opcion = 1;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/getBancosBloqueoLiberacion", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseBancosBloqueoLiberacion>(reqResponse);
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

        [Route("AdministrarClientes/setBancosBloqueoLiberacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseSetBancoBloqueoLiberacion> setBancosBloqueoLiberacion([FromBody] SoporteRequestBloqueoLiberacion item)
        {

            SoporteResponseSetBancoBloqueoLiberacion response = new SoporteResponseSetBancoBloqueoLiberacion();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/setBancosBloqueoLiberacion", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseSetBancoBloqueoLiberacion>(reqResponse);
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

        [Route("AdministrarClientes/listadoCajeros")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoCajeros> listadoCajeros([FromBody] SoporteCadenaRequestListadoCajeros item)
        {
            SoporteResponseListadoCajeros response = new SoporteResponseListadoCajeros();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoCajeros", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoCajeros>(reqResponse);
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

        [Route("AdministrarClientes/listadoTerminales")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoTerminales> listadoTerminales([FromBody] SoporteCadenaRequestListadoTerminales item)
        {
            SoporteResponseListadoTerminales response = new SoporteResponseListadoTerminales();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/listadoTerminales", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoTerminales>(reqResponse);
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
        
        [Route("AdministrarClientes/detalleCajero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseDetalleCajero> detalleCajero([FromBody] SoporteRequestDetalleCajero item)
        {
            SoporteResponseDetalleCajero response = new SoporteResponseDetalleCajero();
            try
            {
                item.id = int.Parse(item.idStr);
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/detalleCajero", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseDetalleCajero>(reqResponse);
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
        
        [Route("AdministrarClientes/editarCajero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseEditaCajero> editarCajero([FromBody] SoporteRequestEditaCajero item)
        {
            SoporteResponseEditaCajero response = new SoporteResponseEditaCajero();
            try
            {

                int[] rolesCambios = { 9, 5 };
                int[] depPermitidos = { 1, 2, 14, 13 };

                //Roles de cambio
                if (!rolesCambios.Contains(int.Parse(HttpContext.Session.GetString("rol"))))
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "El usuario cuenta con restricciones para actualizar datos. Valida los datos actualizados.";
                    return response;
                }

                if (!depPermitidos.Contains(int.Parse(HttpContext.Session.GetString("departamento"))))
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "El usuario cuenta con restricciones para actualizar datos. Valida los datos actualizados.";
                    return response;
                }

                item.id = int.Parse(item.idStr);
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.tipoUsuario = 2;
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/editarCajero", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEditaCajero>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/editarCajero",
                        Detalles = "Editado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);
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



        [Route("AdministrarClientes/nuevoCajero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseNuevoCajero> nuevoCajero([FromBody] SoporteRequestNuevoCajero item)
        {
            SoporteResponseNuevoCajero response = new SoporteResponseNuevoCajero();
            try
            {
                
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                item.tipoUsuario = 2;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/nuevoCajero", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseNuevoCajero>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/nuevoCajero",
                        Detalles = "Creado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);
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

        [Route("AdministrarClientes/detalleEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseInformacionCadenaDetalleEstablecimiento> informacionDetalleEstablecimiento([FromBody]SoporteCadenaRequestDetalleEstablecimiento item)
        {
            SoporteResponseInformacionCadenaDetalleEstablecimiento response = new SoporteResponseInformacionCadenaDetalleEstablecimiento();
            try
            {

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));

                string reqResponse = await request.doPost("Soporte/AdministrarClientes/detalleEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {

                    response = JsonConvert.DeserializeObject<SoporteResponseInformacionCadenaDetalleEstablecimiento>(reqResponse);

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
        
        [Route("AdministrarClientes/setEstatusCajero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseSetEstatusCajero> setEstatusCajero([FromBody]SoporteRequestSetEstatusEstablecimiento item)
        {
            SoporteResponseSetEstatusCajero response = new SoporteResponseSetEstatusCajero();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.tipoUsuario = 2;
                item.id = int.Parse(item.idStr);
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/setEstatusCajero", data, HttpContext.Session.GetString("token"));
                
                if (request.codigoRespuesta == 0)                {

                    response = JsonConvert.DeserializeObject<SoporteResponseSetEstatusCajero>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/setEstatusCajero",
                        Detalles = "Estatus cambiado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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

        [Route("AdministrarClientes/setClaveDefinitivaCajero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseSetClaveDefinitivaCajero> setClaveDefinitivaCajero([FromBody]SoporteRequestSetClaveDefinitiva item)
        {
            SoporteResponseSetClaveDefinitivaCajero response = new SoporteResponseSetClaveDefinitivaCajero();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.tipoUsuario = 2;
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/setClaveDefinitivaCajero", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseSetClaveDefinitivaCajero>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/setClaveDefinitivaCajero",
                        Detalles = "Clave cambiada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);
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
        
        [Route("AdministrarClientes/setEstatusTerminal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseSetEstatusTerminal> setEstatusTerminal([FromBody]SoporteRequestSetEstatusTerminal item)
        {
            SoporteResponseSetEstatusTerminal response = new SoporteResponseSetEstatusTerminal();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.tipoUsuario = 2;
                item.id = int.Parse(item.idStr);
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/setEstatusTerminal", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseSetEstatusTerminal>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/setClaveDefinitivaCajero",
                        Detalles = "Clave cambiada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);
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
        
        [Route("AdministrarClientes/bajaTerminal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseBajaTerminal> bajaTerminal([FromBody]SoporteRequestBajaTerminal item)
        {
            SoporteResponseBajaTerminal response = new SoporteResponseBajaTerminal();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")); 
                item.ip = Seguridad.GetIPAddress();
                item.id = int.Parse(item.idStr);
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/bajaTerminal", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseBajaTerminal>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/bajaTerminal",
                        Detalles = "Termianl eliminada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);
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
        
        [Route("AdministrarClientes/nuevaTerminal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseNuevaTerminal> adminNuevaTerminal([FromBody] SoporteRequestNuevaTerminal item)
        {
            SoporteResponseNuevaTerminal response = new SoporteResponseNuevaTerminal();
            try
            {
                item.appMovil = false;
                item.tipoUsuario = 2;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/NuevaTerminal/nuevaTerminal", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseNuevaTerminal>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarClientes/nuevaTerminal",
                        Detalles = "Alta exitosa",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);


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


        [Route("AdministrarClientes/listadoEstablecimientos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoTerminalesCadena> adminlistadoEstablecimientos([FromBody] SoporteRequestTerminalesEstablecimiento item)
        {
            SoporteResponseListadoTerminalesCadena response = new SoporteResponseListadoTerminalesCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/NuevaTerminal/listadoEstablecimientos", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoTerminalesCadena>(reqResponse);


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

        [Route("AdministrarClientes/listadoTipoTerminal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseListadoTipoTerminales> adminlistadoTipoTerminal()
        {
            SoporteResponseListadoTipoTerminales response = new SoporteResponseListadoTipoTerminales();
            try
            {
                string reqResponse = await request.doPost("Soporte/NuevaTerminal/listadoTipoTerminal", "", HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoTipoTerminales>(reqResponse);


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


        [Route("AdministrarClientes/getCiudad")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
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


        [Route("AdministrarClientes/editarEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseEditarEstablecimiento> editarEstablecimiento([FromBody]SoporteRequestEditaEstablecimiento item)
        {
            SoporteResponseEditarEstablecimiento response = new SoporteResponseEditarEstablecimiento();
            try
            {

                int[] rolesCambios = { 9, 5 };
                int[] depPermitidos = { 1, 2, 14, 13 };

                //Roles de cambio
                if (!rolesCambios.Contains(int.Parse(HttpContext.Session.GetString("rol"))))
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "El usuario cuenta con restricciones para actualizar datos. Valida los datos actualizados.";
                    return response;
                }

                if (!depPermitidos.Contains(int.Parse(HttpContext.Session.GetString("departamento"))))
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "El usuario cuenta con restricciones para actualizar datos. Valida los datos actualizados.";
                    return response;
                }

                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/editarEstablecimiento", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEditarEstablecimiento>(reqResponse);
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


        [Route("AdministrarClientes/resetPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01,PERM_07_01_01")]
        public async Task<SoporteResponseResetClave> resetPassword([FromBody] SoporteRequestResetClave item)
        {
            SoporteResponseResetClave response = new SoporteResponseResetClave();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.id = int.Parse(item.idStr);
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarClientes/resetPassword", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseResetClave>(reqResponse);
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


        [Route("AdministrarClientes/editaCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_01")]
        public async Task<SoporteResponseEditaCadena> editaCadena([FromBody] SoporteRequestEditarCadena item)
        {
            SoporteResponseEditaCadena response = new SoporteResponseEditaCadena();
            try
            {

                item.nueva = 0;
                item.horaC = "23:00:00"; 
                item.tipoUsuario = 2;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                int[] rolesCambios = { 9, 5 };
                int[] depPermitidos = {1, 2, 14, 13};
                string depa = HttpContext.Session.GetString("departamento");


                if (item.tipoCredito == 7 || item.tipoCredito == 8 || item.tipoCredito == 10 || item.tipoCredito == 6)
                {
                    // primera vuelta
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Soporte/AdministrarClientes/editaCadena", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<SoporteResponseEditaCadena>(reqResponse);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }

                    // para los segundos periodos
                    if (item.tipoCredito == 8 || item.tipoCredito == 6)
                    {
                        if (response.CodigoRespuesta == 0)
                        {

                            item.inicio = item.inicio2;
                            item.hasta = item.hasta2;
                            item.corte = item.corte2;
                            item.factura = item.factura2;

                            data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                            reqResponse = await request.doPost("Soporte/AdministrarClientes/editaCadena", data, HttpContext.Session.GetString("token"));

                            if (request.codigoRespuesta == 0)
                            {
                                response = JsonConvert.DeserializeObject<SoporteResponseEditaCadena>(reqResponse);
                            }
                            else
                            {
                                response.CodigoRespuesta = -1;
                                response.MensajeRespuesta = "No se pudo guardar el segundo periodo";
                            }
                        }
                        else
                        {
                            response.CodigoRespuesta = -1;
                            response.MensajeRespuesta = "No se pudo realizar la operación";

                        }
                    }
                }
                else
                {
                    // creditos simples
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Soporte/AdministrarClientes/editaCadena", data, HttpContext.Session.GetString("token"));

                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<SoporteResponseEditaCadena>(reqResponse);
                    }
                    else
                    {
                        response.CodigoRespuesta = request.codigoRespuesta;
                        response.MensajeRespuesta = request.mensajeRespuesta;
                    }
                }


                //Roles de cambio
                if (!rolesCambios.Contains(int.Parse(HttpContext.Session.GetString("rol"))))
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "El usuario cuenta con restricciones para actualizar datos. Valida los datos actualizados.";
                    return response;
                }

                if (!depPermitidos.Contains(int.Parse(HttpContext.Session.GetString("departamento"))))
                {
                    response.CodigoRespuesta = -1;
                    response.MensajeRespuesta = "El usuario cuenta con restricciones para actualizar datos. Valida los datos actualizados.";
                    return response;
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

        #region Nueva Terminal


        [Route("NuevaTerminal/nuevaTerminal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_02")]
        public async Task<SoporteResponseNuevaTerminal> nuevaTerminal([FromBody] SoporteRequestNuevaTerminal item)
        {
            SoporteResponseNuevaTerminal response = new SoporteResponseNuevaTerminal();
            try
            {
                item.appMovil = false;
                item.tipoUsuario = 2;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/NuevaTerminal/nuevaTerminal", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseNuevaTerminal>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "NuevaTerminal/nuevaTerminal",
                        Detalles = "Alta exitosa",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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

        [Route("NuevaTerminal/listadoEstablecimientos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_02")]
        public async Task<SoporteResponseListadoTerminalesCadena> listadoEstablecimientos([FromBody] SoporteRequestTerminalesEstablecimiento item)
        {
            SoporteResponseListadoTerminalesCadena response = new SoporteResponseListadoTerminalesCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/NuevaTerminal/listadoEstablecimientos", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoTerminalesCadena>(reqResponse);


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

        [Route("NuevaTerminal/listadoTipoTerminal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_02")]
        public async Task<SoporteResponseListadoTipoTerminales> listadoTipoTerminal()
        {
            SoporteResponseListadoTipoTerminales response = new SoporteResponseListadoTipoTerminales();
            try
            {
                string reqResponse = await request.doPost("Soporte/NuevaTerminal/listadoTipoTerminal", "", HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoTipoTerminales>(reqResponse);


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
        
        #region Administrar IUSA
        
        [Route("AdministrarIUSA/listadoEstablecimientos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_03")]
        public async Task<SoporteResponseEstablecimientosIUSA> estableicmientosIUSA([FromBody] SoporteRequestGetEstablecimientosIUSA item)
        {
            SoporteResponseEstablecimientosIUSA response = new SoporteResponseEstablecimientosIUSA();
            try
            {
                
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarIUSA/listadoEstablecimientos", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEstablecimientosIUSA>(reqResponse);
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
        
        [Route("AdministrarIUSA/editarEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_03")]
        public async Task<SoporteResponseEditarEstablecimientoIUSA> editarEstablecimientoIUSA([FromBody] SoporteRequestEditaEstablecimientoIUSA item)
        {
            SoporteResponseEditarEstablecimientoIUSA response = new SoporteResponseEditarEstablecimientoIUSA();
            try
            {
                item.id = util.desencriptaId(item.idEstablecimientoStr);
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarIUSA/editarEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEditarEstablecimientoIUSA>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarIUSA/editarEstablecimiento",
                        Detalles = "Establecimiento editado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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
        
        [Route("AdministrarIUSA/nuevoEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_03")]
        public async Task<SoporteResponseNuevoEstablecimientoIUSA> nuevoEstablecimientoIUSA([FromBody] SoporteRequestNuevoEstablecimientoIUSA item)
        {
            SoporteResponseNuevoEstablecimientoIUSA response = new SoporteResponseNuevoEstablecimientoIUSA();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarIUSA/nuevoEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseNuevoEstablecimientoIUSA>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarIUSA/nuevoEstablecimiento",
                        Detalles = "Alta exitosa",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);


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
        
        [Route("AdministrarIUSA/listadoEstablecimientosCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_03")]
        public async Task<SoporteResponseListadoEstablecimientosCadena> establecimientosCadenaIUSA([FromBody] SoporteRequestEstablecimientos item)
        {
            SoporteResponseListadoEstablecimientosCadena response = new SoporteResponseListadoEstablecimientosCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarIUSA/listadoEstablecimientosCadena", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoEstablecimientosCadena>(reqResponse);


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

        #region Administrar WINPOS
        
        [Route("AdministrarWINPOS/listadoEstablecimientos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_04")]
        public async Task<SoporteResponseEstablecimientosWINPOS> establecimientosWINPOS([FromBody] SoporteRequestGetEstablecimientosWinPos item)
        {
            SoporteResponseEstablecimientosWINPOS response = new SoporteResponseEstablecimientosWINPOS();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarWINPOS/listadoEstablecimientos", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEstablecimientosWINPOS>(reqResponse);
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

        [Route("AdministrarWINPOS/editarEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_04")]
        public async Task<SoporteResponseEditarEstablecimientoWINPOS> editarEstablecimientoWINPOS([FromBody] SoporteRequestEditaEstablecimientoWinPos item)
        {
            SoporteResponseEditarEstablecimientoWINPOS response = new SoporteResponseEditarEstablecimientoWINPOS();
            try
            {
                item.idEstablecimiento = int.Parse(item.idEstablecimientoStr);
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarWINPOS/editarEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEditarEstablecimientoWINPOS>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarIUSA/editarEstablecimiento",
                        Detalles = "Establecimiento editado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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

        [Route("AdministrarWINPOS/nuevoEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_04")]
        public async Task<SoporteResponseNuevoEstablecimientoWINPOS> nuevoEstablecimientoWINPOS([FromBody] SoporteRequestNuevoEstablecimientoWinPos item)
        {
            SoporteResponseNuevoEstablecimientoWINPOS response = new SoporteResponseNuevoEstablecimientoWINPOS();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarWINPOS/nuevoEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseNuevoEstablecimientoWINPOS>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarIUSA/nuevoEstablecimiento",
                        Detalles = "Alta exitosa",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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

        [Route("AdministrarWINPOS/eliminarEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_04")]
        public async Task<SoporteResponseEliminarEstablecimientoWINPOS> editarEstablecimientoIUSA([FromBody] SoporteRequestEliminaEstablecimientoWinPos item)
        {
            SoporteResponseEliminarEstablecimientoWINPOS response = new SoporteResponseEliminarEstablecimientoWINPOS();
            try
            {
                item.idEstablecimiento = util.desencriptaId(item.idEstablecimientoStr);
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarWINPOS/eliminarEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEliminarEstablecimientoWINPOS>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "AdministrarIUSA/eliminarEstablecimiento",
                        Detalles = "Eliminado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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

        [Route("AdministrarWINPOS/listadoEstablecimientosCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_04")]
        public async Task<SoporteResponseListadoEstablecimientosCadena> establecimientosCadenaWINPOS([FromBody] SoporteRequestEstablecimientos item)
        {
            SoporteResponseListadoEstablecimientosCadena response = new SoporteResponseListadoEstablecimientosCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarWINPOS/listadoEstablecimientosCadena", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoEstablecimientosCadena>(reqResponse);


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

        [Route("AdministrarWINPOS/listadoAfiliacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_04")]
        public async Task<SoporteResponseListadoAfiliacion> listadoAfiliacionWINPOS([FromBody] SoporteRequestListadoAfiliacion item)
        {
            SoporteResponseListadoAfiliacion response = new SoporteResponseListadoAfiliacion();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarWINPOS/listadoAfiliacion", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoAfiliacion>(reqResponse);


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

        #region Remesas Error


        [Route("RemesasError/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_05")]
        public async Task<SoporteResponseListadoRemesasError> ipBloqueadas([FromBody]SoporteRequestGetRemesasError item)
        {
            SoporteResponseListadoRemesasError response = new SoporteResponseListadoRemesasError();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/RemesasError/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoRemesasError>(reqResponse);
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

        [Route("RemesasError/subirRemesa")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_05")]
        public async Task<SoporteResponseRemesaExitosa> insertarIfeRemesa([FromBody] SoporteRequestSubirIFERemesa item)
        {
            SoporteResponseRemesaExitosa response = new SoporteResponseRemesaExitosa();
            try
            {
                if (!string.IsNullOrEmpty(item.archivo))
                {
                    if (item.ext == "txt" || item.ext == "csv")
                    {
                        
                        
                        System.IO.File.WriteAllBytes(@"G:\\IFERemesaError\\" + item.idVentaError.ToString() + "." + item.ext, Convert.FromBase64String(item.archivo));


                        SoporteRequestRemesaExitosa itemRemesa = new SoporteRequestRemesaExitosa();
                        itemRemesa.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                        itemRemesa.ip = Seguridad.GetIPAddress();
                        string data = Seguridad.Encrypt(JsonConvert.SerializeObject(itemRemesa));
                        string reqResponse = await request.doPost("Soporte/RemesasError/subirRemesa", data, HttpContext.Session.GetString("token"));

                        if (request.codigoRespuesta == 0)
                        {

                            response = JsonConvert.DeserializeObject<SoporteResponseRemesaExitosa>(reqResponse);
                            response.MensajeRespuesta = "El archivo " + item.nombre + " " + item.ext + " se guardo con exito en la Base de Datos";
                            util.logActividadUsuario(new ActividadUsuario
                            {
                                IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                                Usuario = HttpContext.Session.GetString("user"),
                                Seccion = "Soporte",
                                Actividad = "RemesasError/subirRemesa",
                                Detalles = "Remesa cargada",
                                Objeto = JsonConvert.SerializeObject(item)
                            }, __appSettings.LoggerSoporte);
                        }
                        else
                        {
                            response.CodigoRespuesta = request.codigoRespuesta;
                            response.MensajeRespuesta = "No fue posible guardar el archivo " + item.nombre + " " + item.ext;
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

        #region IP Bloqueadas


        [Route("IpBloqueadas/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_06")]
        public async Task<SoporteResponseListadoIpBloqueadas> ipBloqueadas()
        {
            SoporteRequestGetIpBloqueadas item = new SoporteRequestGetIpBloqueadas();
            item.ip = string.Empty;
            SoporteResponseListadoIpBloqueadas response = new SoporteResponseListadoIpBloqueadas();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/IpBloqueadas/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoIpBloqueadas>(reqResponse);
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

        [Route("IpBloqueadas/eliminar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_06")]
        public async Task<SoporteResponseEliminarIpBloqueada> eliminarIpBloqueada([FromBody] SoporteRequestEliminarIpBloqueada item)
        {
            SoporteResponseEliminarIpBloqueada response = new SoporteResponseEliminarIpBloqueada();
            try
            {
                
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/IpBloqueadas/eliminar", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEliminarIpBloqueada>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "IpBloqueadas/eliminar",
                        Detalles = "Ip eliminada",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);

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

        #region Monitor Log


        [Route("MonitorLog/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_07")]
        public async Task<SoporteResponseListadoMonitorLog> listadoMonitorLogs([FromBody]SoporteRequestGetListadoMonitorLog item)
        {

            SoporteResponseListadoMonitorLog response = new SoporteResponseListadoMonitorLog();
            try
            {
                item.fechaInicio = DateTime.Parse(item.fechaInicio.ToString("dd/MM/yyyy"+" 00:00:00"));
                item.fechaFin = DateTime.Parse(item.fechaFin.ToString("dd/MM/yyyy" + " 00:00:00"));

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/MonitorLog/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoMonitorLog>(reqResponse);
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

        [Route("MonitorLog/listadoAplicaciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_07")]
        public async Task<SoporteResponseListadoAplicaciones> listadoAplicaciones()
        {
            SoporteResponseListadoAplicaciones response = new SoporteResponseListadoAplicaciones();
            try
            {
                
                string reqResponse = await request.doPost("Soporte/MonitorLog/listadoAplicaciones", "", HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoAplicaciones>(reqResponse);


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
        
        [Route("MonitorLog/listadoTipoOperacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_07")]
        public async Task<SoporteResponseListadoTipoOperacion> listadoTipoOperacion()
        {
            SoporteResponseListadoTipoOperacion response = new SoporteResponseListadoTipoOperacion();
            try
            {
                string reqResponse = await request.doPost("Soporte/MonitorLog/listadoTipoOperacion", "", HttpContext.Session.GetString("token"));
                
                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoTipoOperacion>(reqResponse);
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

        #region Configurar Proveedores
        
        [Route("ConfigurarProveedores/proveedoresPorRegion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_08")]
        public async Task<SoporteResponseProveedoresRegion> proveedoresPorRegion()
        {

            SoporteResponseProveedoresRegion response = new SoporteResponseProveedoresRegion();
            try
            {
                string reqResponse = await request.doPost("Soporte/ConfigurarProveedores/proveedoresPorRegion", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseProveedoresRegion>(reqResponse);
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

        [Route("ConfigurarProveedores/switchTelcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_08")]
        public async Task<SoporteResponseListadoSwitchTelcel> switchTelcel()
        {

            SoporteResponseListadoSwitchTelcel response = new SoporteResponseListadoSwitchTelcel();
            try
            {
                string reqResponse = await request.doPost("Soporte/ConfigurarProveedores/switchTelcel", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoSwitchTelcel>(reqResponse);
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

        [Route("ConfigurarProveedores/switchMovistar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_08")]
        public async Task<SoporteResponseListadoSwitchMovistar> switchMovistar()
        {

            SoporteResponseListadoSwitchMovistar response = new SoporteResponseListadoSwitchMovistar();
            try
            {
                string reqResponse = await request.doPost("Soporte/ConfigurarProveedores/switchMovistar", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoSwitchMovistar>(reqResponse);
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

        [Route("ConfigurarProveedores/switchIusaUnefon")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_08")]
        public async Task<SoporteResponseListadoSwitchIusaUnefon> switchIusaUnefon()
        {

            SoporteResponseListadoSwitchIusaUnefon response = new SoporteResponseListadoSwitchIusaUnefon();
            try
            {
                string reqResponse = await request.doPost("Soporte/ConfigurarProveedores/switchIusaUnefon", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoSwitchIusaUnefon>(reqResponse);
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

        #region Cajeros Push Money

        [Route("CajeroPushMoney/listado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_09")]
        public async Task<SoporteResponseListadoPushMoney> cajerosPushMoney([FromBody]SoporteRequestGetListadoPushMoney item)
        {

            SoporteResponseListadoPushMoney response = new SoporteResponseListadoPushMoney();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/CajeroPushMoney/listado", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoPushMoney>(reqResponse);
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
        
        [Route("CajeroPushMoney/editarCajero")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_09")]
        public async Task<SoporteResponseEditarEstablecimientoPushMoney> editarCajeroPushMoney([FromBody] SoporteRequestEditarEstablecimientoPushMoney item)
        {
            SoporteResponseEditarEstablecimientoPushMoney response = new SoporteResponseEditarEstablecimientoPushMoney();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/CajeroPushMoney/editarCajero", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseEditarEstablecimientoPushMoney>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Soporte",
                        Actividad = "CajeroPushMoney/editarCajero",
                        Detalles = "Cajero editado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerSoporte);
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


        [Route("CajeroPushMoney/listadoEstablecimientosCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_09")]
        public async Task<SoporteResponseListadoEstablecimientosCadena> establecimientosPushMoney([FromBody] SoporteRequestEstablecimientos item)
        {
            SoporteResponseListadoEstablecimientosCadena response = new SoporteResponseListadoEstablecimientosCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/CajeroPushMoney/listadoEstablecimientosCadena", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseListadoEstablecimientosCadena>(reqResponse);


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

        #region Administrar Establecimiento
        [Route("AdministrarEstablecimiento/informacionCadenaBnext")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_10")]
        public async Task<SoporteResponseInformacionCadenaBnext> informacionCadenaBnext([FromBody] SoporteRequestInformacionCadenaBnext item)
        {
            SoporteResponseInformacionCadenaBnext response = new SoporteResponseInformacionCadenaBnext();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/AdministrarEstablecimiento/informacionCadenaBnext", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseInformacionCadenaBnext>(reqResponse);
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

        #region Tickets Establecimiento

        [Route("Tickets/historialTickets")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_07,PERM_07_10")]
        public async Task<SoporteResponseHistorialTickets> historialTickets([FromBody] SoporteRequestHistorial item)
        {
            SoporteResponseHistorialTickets response = new SoporteResponseHistorialTickets();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Soporte/Tickets/historialTickets", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<SoporteResponseHistorialTickets>(reqResponse);
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

        #endregion
    }
}