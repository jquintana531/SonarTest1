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
using static MTC.WebApp.BackOffice.Models.OperacionesModel;

namespace MTC.WebApp.BackOffice.Controllers
{
    [Route("api/Operaciones")]
    [ApiController]
    public class OperacionesController : Controller
    {
        private readonly AppSettings __appSettings;
        private readonly ConfigurationSettings __configurations;
        private RequestHelper request;
        private Utils util;

        public OperacionesController(IOptions<AppSettings> appSetings, IOptions<ConfigurationSettings> configurations)
        {
            this.__appSettings = appSetings.Value;
            this.__configurations = configurations.Value;
            this.request = new RequestHelper(this.__appSettings);
            util = new Utils();
        }

        #region Conciliaciones

        [Route("Conciliaciones/ObtenerConciliacionProveedores")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_01")]
        public async Task<OperacionesResponseListaConciliacionesProveedores> obtenerConciliacionProveedores()
        {
            OperacionesResponseListaConciliacionesProveedores response = new OperacionesResponseListaConciliacionesProveedores();
            try
            {
                string reqResponse = await request.doPost("Operaciones/Conciliaciones/ObtenerConciliacionesProveedores", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListaConciliacionesProveedores>(reqResponse);
                    for (int i = 0; i < response.listaProveedores.Count; i++)
                    {
                        if (response.listaProveedores[i].estatus.Equals("0"))
                            response.listaProveedores[i].estatus = "";
                        else if (response.listaProveedores[i].estatus.Equals("1"))
                            response.listaProveedores[i].estatus = "./assets/app/media/img//misc/correcto.png";
                        else if (response.listaProveedores[i].estatus.Equals("2"))
                            response.listaProveedores[i].estatus = "./assets/app/media/img//error/bg7.png";
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

        [Route("Conciliaciones/RealizarConciliacionProveedores")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_01")]
        public async Task<ResponseModel> realizarConciliacionProveedores(OperacionesRequestRealizarConciliacionProveedor item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/Conciliaciones/RealizarConciliacionProveedores", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Conciliaciones/RealizarConciliacionProveedores",
                        Detalles = "Ejecuta las conciliaciones de los proveedores",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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


        [Route("Conciliaciones/AgregarNuevoProveedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_01")]
        public async Task<OperacionesResponseNuevaConciliacion> agregarNuevoProveedor(OperacionesRequestAgregarNuevoProveedor item)
        {
            OperacionesResponseNuevaConciliacion response = new OperacionesResponseNuevaConciliacion();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/Conciliaciones/AgregarNuevoProveedor", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseNuevaConciliacion>(reqResponse);
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

        #region ConciliacionMayoristas

        [Route("Conciliaciones/ObtenerConciliacionesMayoristas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_15")]
        public async Task<OperacionesResponseListaConciliacionesMayoristas> obtenerConciliacionMayoristas()
        {
            OperacionesResponseListaConciliacionesMayoristas response = new OperacionesResponseListaConciliacionesMayoristas();
            try
            {
                string reqResponse = await request.doPost("Operaciones/Conciliaciones/ObtenerConciliacionesMayoristas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListaConciliacionesMayoristas>(reqResponse);
                    for (int i = 0; i < response.listaMayoristas.Count; i++)
                    {
                        if (response.listaMayoristas[i].estatus.Equals("0"))
                            response.listaMayoristas[i].estatus = "";
                        else if (response.listaMayoristas[i].estatus.Equals("1"))
                            response.listaMayoristas[i].estatus = "./assets/app/media/img//misc/correcto.png";
                        else if (response.listaMayoristas[i].estatus.Equals("2"))
                            response.listaMayoristas[i].estatus = "./assets/app/media/img//error/bg7.png";
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


        [Route("Conciliaciones/RealizarConciliacionMayoristas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_15")]
        public async Task<ResponseModel> realizarConciliacionMayoristas(OperacionesRequestRealizarConciliacionMayorista item)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/Conciliaciones/RealizarConciliacionMayoristas", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<ResponseModel>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Conciliaciones/RealizarConciliacionMayoristas",
                        Detalles = "Ejecuta las conciliaciones de los mayoristas",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("Conciliaciones/AgregarNuevoMayorista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_01")]
        public async Task<OperacionesResponseNuevaConciliacion> agregarNuevoMayorista(OperacionesRequestAgregarNuevoMayorista item)
        {
            OperacionesResponseNuevaConciliacion response = new OperacionesResponseNuevaConciliacion();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/Conciliaciones/AgregarNuevoMayorista", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseNuevaConciliacion>(reqResponse);
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

        #region Administrar Compras

        [Route("AdministrarCompras/listadoCompras")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesResponseListadoEstatusCompras> listadoCompras(OperacionesRequestListadoEstatusCompras item)
        {
            OperacionesResponseListadoEstatusCompras response = new OperacionesResponseListadoEstatusCompras();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/AdministrarCompras/listadoCompras", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoEstatusCompras>(reqResponse);
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

        [Route("AdministrarCompras/setObservacionEstatusCompra")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesResponseEditaCompra> setObservacionEstatusCompra([FromBody] OperacionesRequestSetObervacionesCompra item)
        {
            OperacionesResponseEditaCompra response = new OperacionesResponseEditaCompra();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/AdministrarCompras/setObservacionEstatusCompra", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseEditaCompra>(reqResponse);

                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "AdministrarCompras/setObservacionEstatusCompra",
                        Detalles = "Compra editada",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("AdministrarCompras/setEstatusCompra")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesResponseEditaCompra> setEstatusCompra([FromBody] OperacionesRequestSetEstatusCompra item)
        {
            OperacionesResponseEditaCompra response = new OperacionesResponseEditaCompra();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/AdministrarCompras/setEstatusCompra", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseEditaCompra>(reqResponse);

                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "AdministrarCompras/setObservacionEstatusCompra",
                        Detalles = "Compra editada",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("AdministrarCompras/listadoEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesResponseListadoEstablecimientos> setEstatusCompra([FromBody] OperacionesRequestListadoEstablecimientos item)
        {
            OperacionesResponseListadoEstablecimientos response = new OperacionesResponseListadoEstablecimientos();
            try
            {
                item.activo = 2;
                item.menu = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/AdministrarCompras/listadoEstablecimiento", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoEstablecimientos>(reqResponse);
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

        [Route("AdministrarCompras/listadoProducto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesReponseListadoProductoCompra> listadoProductos()
        {
            OperacionesRequestProductoCompra item = new OperacionesRequestProductoCompra();
            OperacionesReponseListadoProductoCompra response = new OperacionesReponseListadoProductoCompra();
            try
            {
                item.id = 0;
                item.activo = 1;
                item.menu = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/AdministrarCompras/listadoProducto", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesReponseListadoProductoCompra>(reqResponse);
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

        [Route("AdministrarCompras/precioProducto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesResponsePrecioProducto> precioProducto([FromBody] OperacionesRequestProductoCompra item)
        {
            OperacionesResponsePrecioProducto response = new OperacionesResponsePrecioProducto();
            try
            {
                item.menu = 0;
                item.activo = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/AdministrarCompras/precioProducto", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponsePrecioProducto>(reqResponse);
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

        [Route("AdministrarCompras/detalleProducto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesResponseDetalleProducto> detalleProducto([FromBody] OperacionesRequestProductoCompra item)
        {
            OperacionesResponseDetalleProducto response = new OperacionesResponseDetalleProducto();
            try
            {
                item.menu = 0;
                item.activo = 1;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/AdministrarCompras/detalleProducto", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseDetalleProducto>(reqResponse);
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

        [Route("AdministrarCompras/insertarCompra")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesResponseAgregarCompra> insertarCompra([FromBody] OpereacionesRequestInsertaCompra item)
        {
            OperacionesResponseAgregarCompra response = new OperacionesResponseAgregarCompra();
            try
            {
                OperacionesResponseDetalleProducto detalleProducto = new OperacionesResponseDetalleProducto();
                detalleProducto = await this.detalleProducto(new OperacionesRequestProductoCompra { id = item.idProducto });

                if (detalleProducto != null && detalleProducto.CodigoRespuesta == 0)
                {
                    item.costo = float.Parse(detalleProducto.detalle.precioVenta.ToString());
                    item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                    item.ip = Seguridad.GetIPAddress();
                    string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                    string reqResponse = await request.doPost("Operaciones/AdministrarCompras/insertarCompra", data, HttpContext.Session.GetString("token"));


                    if (request.codigoRespuesta == 0)
                    {
                        response = JsonConvert.DeserializeObject<OperacionesResponseAgregarCompra>(reqResponse);
                        util.logActividadUsuario(new ActividadUsuario
                        {
                            IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                            Usuario = HttpContext.Session.GetString("user"),
                            Seccion = "Operaciones",
                            Actividad = "AdministrarCompras/insertarCompra",
                            Detalles = "Compra insertada",
                            Objeto = JsonConvert.SerializeObject(item)
                        }, __appSettings.LoggerOperaciones);
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
                    response.MensajeRespuesta = "Erro al obtener información de producto";
                }



            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("AdministrarCompras/comision")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_07")]
        public async Task<OperacionesResponseComisionCadena> listadoFamiliasComisiones([FromBody] OperacionesRequestListadoFamilias item)
        {
            OperacionesResponseComisionCadena response = new OperacionesResponseComisionCadena();
            try
            {
                item.activo = 2;
                item.menu = 0;
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/AdministrarCompras/comisionCadena", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseComisionCadena>(reqResponse);
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

        #region Familias comerciales

        [Route("FamiliasComerciales/listadoFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_08")]
        public async Task<OperacionesResponseListadoFamiliaComercial> listadoFamiliasComerciales()
        {
            OperacionesResponseListadoFamiliaComercial response = new OperacionesResponseListadoFamiliaComercial();
            try
            {
                string reqResponse = await request.doPost("Operaciones/FamiliasComerciales/listadoFamilia", "", HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoFamiliaComercial>(reqResponse);
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

        [Route("FamiliasComerciales/productosFamiliaComercial")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_08")]
        public async Task<OperacionesResponseListadoProductosFamiliaComercial> productosFamiliaComercial([FromBody] OperacionesRequestGetProductosFamiliaComercial item)
        {
            OperacionesResponseListadoProductosFamiliaComercial response = new OperacionesResponseListadoProductosFamiliaComercial();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/FamiliasComerciales/productosFamiliaComercial", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoProductosFamiliaComercial>(reqResponse);
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

        [Route("FamiliasComerciales/productosFamiliaComercialExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_06_01")]
        public ProductosFamiliaComercialExcel ReporteVentasExcel([FromBody] OperacionesRequestGetProductosFamiliaComercial item)
        {
            OperacionesResponseListadoProductosFamiliaComercial responseD = new OperacionesResponseListadoProductosFamiliaComercial();
            ProductosFamiliaComercialExcel responseD1 = new ProductosFamiliaComercialExcel();

            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), data);
                Task<string> taskResponse = request.doPost("Operaciones/FamiliasComerciales/productosFamiliaComercial", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoProductosFamiliaComercial>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;


                    Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = "Reporte de Familia Comerciales generado exitosamente";
                        responseD1.respuestaLista = responseD.listadoProductos;

                        ExcelPackage workbook = new ExcelPackage();
                        workbook.Workbook.Worksheets.Add("Familia Comerciales");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Nombre";
                        hojaActual.Cells["B1"].Value = "Proveedor";
                        hojaActual.Cells["C1"].Value = "Comisión Establecimiento";
                        hojaActual.Cells["D1"].Value = "Porcentaje";
                        hojaActual.Cells["E1"].Value = "Activo";
                        hojaActual.Cells["A1:E1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (ProductosFamiliaComercial carteraNew in responseD.listadoProductos)
                        {
                            if (carteraNew.activo)
                            {
                                //hojaActual.Cells["A" + linea].Value = carteraNew.p.ToString();
                                hojaActual.Cells["A" + linea].Value = carteraNew.nombre.ToString();
                                hojaActual.Cells["B" + linea].Value = carteraNew.proveedor.ToString();
                                hojaActual.Cells["C" + linea].Value = carteraNew.comisionEstablecimiento.ToString();
                                hojaActual.Cells["D" + linea].Value = carteraNew.porcentaje.ToString();
                                hojaActual.Cells["E" + linea].Value = carteraNew.activo.ToString();

                                linea++;
                            }
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

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Familias Comercial.xlsx";

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
            return responseD1;
        }


        [Route("FamiliasComerciales/agregarProductoFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_08")]
        public async Task<OperacionesResponseAgregaProductoFamilia> agregarProductoFamilia([FromBody] OperacionesRequestAgregarProductoFamilia item)
        {
            OperacionesResponseAgregaProductoFamilia response = new OperacionesResponseAgregaProductoFamilia();
            try
            {
                item.comisionEstablecimiento = 0;
                item.porcentaje = false;
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/FamiliasComerciales/agregarProductoFamilia", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseAgregaProductoFamilia>(reqResponse);

                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "FamiliasComerciales/agregarProductoFamilia",
                        Detalles = "Producto agregado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("FamiliasComerciales/setEstatusProductoFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_08")]
        public async Task<OperacionesResponseEstatusProductoFamilia> setEstatusProductoFamilia([FromBody] OperacionesRequestEstatusProductoFamilia item)
        {
            OperacionesResponseEstatusProductoFamilia response = new OperacionesResponseEstatusProductoFamilia();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/FamiliasComerciales/setEstatusProductoFamilia", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseEstatusProductoFamilia>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "FamiliasComerciales/setEstatusProductoFamilia",
                        Detalles = "Estatus cambiado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("FamiliasComerciales/setProductoFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_08")]
        public async Task<OperacionesResponseSetProductoFamilia> setProductoFamilia([FromBody] OperacionesRequestSetProductoFamilia item)
        {
            OperacionesResponseSetProductoFamilia response = new OperacionesResponseSetProductoFamilia();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/FamiliasComerciales/setProductoFamilia", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseSetProductoFamilia>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "FamiliasComerciales/setEstatusProductoFamilia",
                        Detalles = "Producto cambiado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("FamiliasComerciales/deleteProductoFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_08")]
        public async Task<OperacionesResponseEliminaProductoFamilia> deleteProductoFamilia([FromBody] OperacionesRequestDeleteProductoFamilia item)
        {
            OperacionesResponseEliminaProductoFamilia response = new OperacionesResponseEliminaProductoFamilia();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/FamiliasComerciales/deleteProductoFamilia", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseEliminaProductoFamilia>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "FamiliasComerciales/deleteProductoFamilia",
                        Detalles = "Eliminado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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
        [MTCAuthorizationHandler("PERM_08,PERM_08_12")]
        public async Task<SoporteResponseListadoRemesasError> remesasErrorListado([FromBody] SoporteRequestGetRemesasError item)
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
        [MTCAuthorizationHandler("PERM_08,PERM_08_12")]
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
                                Seccion = "Operaciones",
                                Actividad = "RemesasError/subirRemesa",
                                Detalles = "Remesa subida exitosamente",
                                Objeto = JsonConvert.SerializeObject(item)
                            }, __appSettings.LoggerOperaciones);
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

        #region Switch Proveedores

        [Route("SwitchProveedores/listadoSwitchProveedores")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_13")]
        public async Task<OperacionesResponseListadoSwitchProveedores> remesasErrorListado()
        {
            OperacionesResponseListadoSwitchProveedores response = new OperacionesResponseListadoSwitchProveedores();
            try
            {
                string reqResponse = await request.doPost("Operaciones/SwitchProveedores/listadoSwitchProveedores", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoSwitchProveedores>(reqResponse);
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

        [Route("SwitchProveedores/listadoCasosSwitch")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_13")]
        public async Task<OperacionesResponseListadoCasosSwitch> listadoCasosSwitch([FromBody] OperacionesRequestGetCasosSwitch item)
        {
            OperacionesResponseListadoCasosSwitch response = new OperacionesResponseListadoCasosSwitch();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/SwitchProveedores/listadoCasosSwitch", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoCasosSwitch>(reqResponse);
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

        [Route("SwitchProveedores/listadoConfiguracionCasos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_13")]
        public async Task<OperacionesResponseListadoConfiguracionCasos> listadoConfiguracionCasos([FromBody] OperacionesRequestGetCasosSwitch item)
        {
            OperacionesResponseListadoConfiguracionCasos response = new OperacionesResponseListadoConfiguracionCasos();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/SwitchProveedores/listadoConfiguracionCasos", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoConfiguracionCasos>(reqResponse);
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

        [Route("SwitchProveedores/getLabelCaso")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_13")]
        public async Task<OperacionesResponseLabelCaso> getLabelCaso([FromBody] OperacionesRequestGetCasosSwitch item)
        {
            OperacionesResponseLabelCaso response = new OperacionesResponseLabelCaso();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/SwitchProveedores/getLabelCaso", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseLabelCaso>(reqResponse);
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

        [Route("SwitchProveedores/realizaSwitch")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_13")]
        public async Task<OperacionesResponseRealizaSwitch> getLabelCaso([FromBody] OperacionesRequestSwitchCaso item)
        {
            OperacionesResponseRealizaSwitch response = new OperacionesResponseRealizaSwitch();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/SwitchProveedores/realizaSwitch", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseRealizaSwitch>(reqResponse);

                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "SwitchProveedores/realizaSwitch",
                        Detalles = "switcheo",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        #region Switch Familias

        [Route("SwitchFamilias/listadoProveedoresRegion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_14")]
        public async Task<OperacionesResponseListadoProveedoresRegion> listadoProveedoresRegion()
        {
            OperacionesResponseListadoProveedoresRegion response = new OperacionesResponseListadoProveedoresRegion();
            try
            {
                string reqResponse = await request.doPost("Operaciones/SwitchFamilias/listadoProveedoresRegion", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoProveedoresRegion>(reqResponse);
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

        [Route("SwitchFamilias/deleteProveedorRegion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_14")]
        public async Task<OperacionesResponseEliminaProveedorRegion> deleteProveedorRegion([FromBody] OperacionesRequestDeleteProveedoresRegion item)
        {
            OperacionesResponseEliminaProveedorRegion response = new OperacionesResponseEliminaProveedorRegion();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/SwitchFamilias/deleteProveedorRegion", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseEliminaProveedorRegion>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "SwitchFamilias/deleteProveedorRegion",
                        Detalles = "Proveedor eliminado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("SwitchFamilias/newProveedorRegion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_14")]
        public async Task<OperacionesResponseNuevoProveedorRegion> newProveedorRegion([FromBody] OperacionesRequestNuevoProveedorRegion item)
        {
            OperacionesResponseNuevoProveedorRegion response = new OperacionesResponseNuevoProveedorRegion();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/SwitchFamilias/newProveedorRegion", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseNuevoProveedorRegion>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "SwitchFamilias/newProveedorRegion",
                        Detalles = "Proveedor agregado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("SwitchFamilias/setProveedorRegion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_14")]
        public async Task<OperacionesResponseSetProveedorRegion> setProveedorRegion([FromBody] OperacionesRequestSetProveedoresRegion item)
        {
            OperacionesResponseSetProveedorRegion response = new OperacionesResponseSetProveedorRegion();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/SwitchFamilias/setProveedorRegion", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseSetProveedorRegion>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "SwitchFamilias/setProveedorRegion",
                        Detalles = "Proveedor seteado",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("SwitchFamilias/listadosNuevoProveedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_14")]
        public async Task<OperacionesResponseListadosNuevoProveedor> listadosNuevoProveedor()
        {
            OperacionesResponseListadosNuevoProveedor response = new OperacionesResponseListadosNuevoProveedor();
            try
            {
                string reqResponse = await request.doPost("Operaciones/SwitchFamilias/listadosNuevoProveedor", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadosNuevoProveedor>(reqResponse);
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

        [Route("SwitchFamilias/listadoProveedores")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_14")]
        public async Task<OperacionesResponseListadoProveedores> listadoProveedores([FromBody] OperacionesRequestListadoProveedores item)
        {
            OperacionesResponseListadoProveedores response = new OperacionesResponseListadoProveedores();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/SwitchFamilias/listadoProveedores", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoProveedores>(reqResponse);
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

        #region Administrar Familias

        [Route("AdministrarFamilias/listadoFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_06")]
        public OperacionesResponseListadoFamilias listadoFamilias()
        {
            OperacionesResponseListadoFamilias responseD = new OperacionesResponseListadoFamilias();
            try
            {
                Task<string> taskResponse = request.doPost("Operaciones/AdministrarFamilias/listadoFamilia", "", HttpContext.Session.GetString("token"));
                Task.WhenAll(taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoFamilias>(response);
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

        [Route("AdministrarFamilias/AgregarFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_06")]
        public ResponseModel AgregarFamilia(OperacionesRequestAgregarFamilia item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/AdministrarFamilias/AgregarFamilia", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "AdministrarFamilias/AgregarFamilia",
                        Detalles = "Familia agregada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("AdministrarFamilias/EditarFamilia")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_06")]
        public ResponseModel EditarFamilia(OperacionesRequestEditarFamilia item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/AdministrarFamilias/EditarFamilia", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "AdministrarFamilias/EditarFamilia",
                        Detalles = "Familia editada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        #region Productos

        [Route("Productos/listadoCatalogosGenericos")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_05")]
        public OperacionesResponseCatalogosListadosGenericos listadoCatalogosGenericos()
        {

            OperacionesResponseCatalogosListadosGenericos responseD = new OperacionesResponseCatalogosListadosGenericos();
            try
            {
                RequestHelper request_proveedores = new RequestHelper(this.__appSettings);
                RequestHelper request_cproductos = new RequestHelper(this.__appSettings);
                RequestHelper request_intermediarios = new RequestHelper(this.__appSettings);
                RequestHelper request_fproductos = new RequestHelper(this.__appSettings);


                Task<string> taskResponse_proveedores = request_proveedores.doPost("Operaciones/Productos/ListadoProveedor", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestCatalogo { id = 0, menu = 1 })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_cproductos = request_cproductos.doPost("Operaciones/Productos/ListadoClasificacionProducto", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestCatalogo { id = 0, menu = 1 })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_intermediarios = request_intermediarios.doPost("Operaciones/Productos/ListadoIntermediario", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestCatalogo { id = 0, menu = 1 })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_fproductos = request_fproductos.doPost("Operaciones/Productos/ListadoFamiliaProducto", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestCatalogo { id = 0, menu = 1 })), HttpContext.Session.GetString("token"));

                Task.WhenAll(taskResponse_proveedores, taskResponse_cproductos, taskResponse_intermediarios, taskResponse_fproductos);
                string response_proveedores = taskResponse_proveedores.Result;
                string response_cproductos = taskResponse_cproductos.Result;
                string response_intermediarios = taskResponse_intermediarios.Result;
                string response_fproductos = taskResponse_fproductos.Result;


                if (request_proveedores.codigoRespuesta == 0 && request_cproductos.codigoRespuesta == 0 && request_intermediarios.codigoRespuesta == 0 && request_fproductos.codigoRespuesta == 0)
                {
                    responseD.proveedores = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_proveedores);
                    responseD.cproductos = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_cproductos);
                    responseD.intermediarios = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_intermediarios);
                    responseD.fproductos = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_fproductos);

                    if (responseD.proveedores.CodigoRespuesta == 0 && responseD.cproductos.CodigoRespuesta == 0 && responseD.intermediarios.CodigoRespuesta == 0 && responseD.fproductos.CodigoRespuesta == 0)
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

        [Route("Productos/getCatalogosProductosAE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_05")]
        public OperacionesResponseListadoProductos getCatalogosProductosAE()
        {
            OperacionesResponseListadoProductos responseD = new OperacionesResponseListadoProductos();
            try
            {
                Task<string> taskResponse = request.doPost("Operaciones/Productos/listadoProductos", "", HttpContext.Session.GetString("token"));
                Task.WhenAll(taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoProductos>(response);
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

        [Route("Productos/AgregarProducto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_05")]
        public ResponseModel AgregarProducto(OperacionesRequestAgregarProducto item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Productos/AgregarProducto", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Productos/AgregarProducto",
                        Detalles = "Producto agregado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("Productos/EditarProducto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_05")]
        public ResponseModel EditarProducto(OperacionesRequestEditarProducto item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Productos/EditarProducto", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Productos/EditarProducto",
                        Detalles = "Producto editado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("Productos/ToggleProducto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_05")]
        public ResponseModel ToggleProducto(OperacionesRequestToggleProducto item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Productos/ToggleProducto", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
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

        #region Comunicados

        [Route("Comunicados/getCatalogosComunicados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_02")]
        public OperacionesResponseListadoComunicados getCatalogosComunicados(OperacionesRequestListadoComunicados item)
        {
            OperacionesResponseListadoComunicados responseD = new OperacionesResponseListadoComunicados();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Comunicados/getCatalogosComunicados", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoComunicados>(response);
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

        [Route("Comunicados/listadoCadenasComunicados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_02")]
        public OperacionesResponseListadoGenerico listadoCadenasComunicados(OperacionesRequestGetCatalogoCadena item)
        {
            OperacionesResponseListadoGenerico responseD = new OperacionesResponseListadoGenerico();
            item.id = 0;
            item.activo = 2;
            item.menu = 1;
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Comunicados/ListadoCadenasComunicados", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response);
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

        [Route("Comunicados/ListadoEstablecimientosComunicados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_02")]
        public OperacionesResponseListadoGenerico ListadoEstablecimientosComunicados(OperacionesRequestGetCatalogoEstablecimiento item)
        {
            OperacionesResponseListadoGenerico responseD = new OperacionesResponseListadoGenerico();

            item.activo = 2;
            item.menu = 1;
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Comunicados/ListadoEstablecimientosComunicados", data, HttpContext.Session.GetString("token"));
                Task.WhenAll(taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response);
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

        [Route("Comunicados/listadoCatalogosGenericosComunicados")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_02")]
        public OperacionesResponseCatalogosListadosGenericosCom listadoCatalogosGenericosComunicados()
        {

            OperacionesResponseCatalogosListadosGenericosCom responseD = new OperacionesResponseCatalogosListadosGenericosCom();
            try
            {
                RequestHelper request_grupos = new RequestHelper(this.__appSettings);
                RequestHelper request_contactos = new RequestHelper(this.__appSettings);


                Task<string> taskResponse_grupos = request_grupos.doPost("Operaciones/Comunicados/ListadoGruposComunicados", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestGetCatalogo { id = 0, menu = 1, activo = 2 })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_contactos = request_contactos.doPost("Operaciones/Comunicados/ListadoContactosComunicados", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestGetCatalogo { activo = 2, menu = 1 })), HttpContext.Session.GetString("token"));

                Task.WhenAll(taskResponse_grupos, taskResponse_contactos);
                string response_grupos = taskResponse_grupos.Result;
                string response_contactos = taskResponse_contactos.Result;


                if (request_grupos.codigoRespuesta == 0 && request_contactos.codigoRespuesta == 0)
                {
                    responseD.grupos = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_grupos);
                    responseD.contactos = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_contactos);

                    if (responseD.grupos.CodigoRespuesta == 0 && responseD.contactos.CodigoRespuesta == 0)
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

        [Route("Comunicados/AgregarComunicado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_02")]
        public ResponseModel AgregarComunicado(OperacionesRequestComunicadoAdd item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Comunicados/AgregarComunicado", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Comunicados/AgregarComunicado",
                        Detalles = "Comunicado agregado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("Comunicados/EditarComunicado")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_02")]
        public ResponseModel EditarComunicado(OperacionesRequestComunicadoEdit item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Comunicados/EditarComunicado", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Comunicados/EditarComunicado",
                        Detalles = "Comunicado editado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("Comunicados/OperacionesRequestGetCatalogoCadenaEstablecimiento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_02")]
        public OperacionesResponseCadenasEstablecimientos OperacionesRequestGetCatalogoCadenaEstablecimiento(OperacionesRequestGetCatalogoCadenaEstablecimiento item)
        {

            OperacionesResponseCadenasEstablecimientos responseD = new OperacionesResponseCadenasEstablecimientos();
            try
            {
                item.cadenas.id = 0;
                item.cadenas.activo = 2;
                item.cadenas.menu = 1;

                item.establecimientos.activo = 2;
                item.establecimientos.menu = 1;

                RequestHelper request_cadenas = new RequestHelper(this.__appSettings);
                RequestHelper request_establecimentos = new RequestHelper(this.__appSettings);


                Task<string> taskResponse_cadenas = request_cadenas.doPost("Operaciones/Comunicados/ListadoCadenasComunicados", Seguridad.Encrypt(JsonConvert.SerializeObject(item.cadenas)), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_establecimientos = request_establecimentos.doPost("Operaciones/Comunicados/ListadoEstablecimientosComunicados", Seguridad.Encrypt(JsonConvert.SerializeObject(item.establecimientos)), HttpContext.Session.GetString("token"));

                Task.WhenAll(taskResponse_cadenas, taskResponse_establecimientos);
                string response_cadenas = taskResponse_cadenas.Result;
                string response_establecimientos = taskResponse_establecimientos.Result;


                if (request_cadenas.codigoRespuesta == 0 && request_establecimentos.codigoRespuesta == 0)
                {
                    responseD.cadenas = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_cadenas);
                    responseD.establecimientos = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_establecimientos);

                    if (responseD.cadenas.CodigoRespuesta == 0 && responseD.establecimientos.CodigoRespuesta == 0)
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

        #endregion

        #region Reveros y Cancelaciones


        [Route("Reversos/ObtenerCRC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_03")]
        public OperacionesResponseObtenerCRC ObtenerCRC()
        {

            OperacionesResponseObtenerCRC responseD = new OperacionesResponseObtenerCRC();
            try
            {
                RequestHelper request_clasificacion_prod = new RequestHelper(this.__appSettings);
                RequestHelper request_productos = new RequestHelper(this.__appSettings);
                RequestHelper request_proveedores = new RequestHelper(this.__appSettings);


                Task<string> taskResponse_clasificacion_prod = request_clasificacion_prod.doPost("Reportes/CatalogoClasificacionProducto", Seguridad.Encrypt(JsonConvert.SerializeObject(new ReporteRequestCatalogoClasificacionProductoModel { id = 0, menu = true })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_productos = request_clasificacion_prod.doPost("Operaciones/Reversos/ObtenerProductosPorClasificacion", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestGetCatalogo { id = 0, menu = 1, activo = 2 })), HttpContext.Session.GetString("token"));
                Task<string> taskResponse_proveedores = request_proveedores.doPost("Operaciones/SwitchFamilias/listadoProveedores", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestListadoProveedores { idCatalogoIntermediario = 5 })), HttpContext.Session.GetString("token"));



                Task.WhenAll(taskResponse_clasificacion_prod, taskResponse_productos, taskResponse_proveedores);
                string response_clasificacion_prod = taskResponse_clasificacion_prod.Result;
                string response_productos = taskResponse_productos.Result;
                string response_proveedores = taskResponse_proveedores.Result;

                if (request_clasificacion_prod.codigoRespuesta == 0 && request_productos.codigoRespuesta == 0)
                {
                    responseD.clasificaciones = JsonConvert.DeserializeObject<OperacionesResponseListadoGenericoRC>(response_clasificacion_prod);
                    responseD.productos = JsonConvert.DeserializeObject<OperacionesResponseListadoPC>(response_productos);
                    responseD.proveedores = JsonConvert.DeserializeObject<OperacionesResponseListadoProveedores>(response_proveedores);

                    if (responseD.clasificaciones.CodigoRespuesta == 0 && responseD.productos.CodigoRespuesta == 0 && responseD.proveedores.CodigoRespuesta == 0)
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

        [Route("Reversos/ObtenerListadoCV")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_03")]
        public OperacionesResponseListadoCV ObtenerListadoCV(OperacionesRequestListadoVentasCancelar item)
        {
            OperacionesResponseListadoCV responseD = new OperacionesResponseListadoCV();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Reversos/ObtenerListadoCV", data, HttpContext.Session.GetString("token"));
                taskResponse.Wait();
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoCV>(response);
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

        [Route("Reversos/ObtenerListadoCVExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_03")]
        public OperacionesResponseListadoCVExcel ObtenerListadoCVExcel(OperacionesRequestListadoVentasCancelar item)
        {
            OperacionesResponseListadoCV responseD = new OperacionesResponseListadoCV();
            OperacionesResponseListadoCVExcel responseD1 = new OperacionesResponseListadoCVExcel();

            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Reversos/ObtenerListadoCV", data, HttpContext.Session.GetString("token"));
                taskResponse.Wait();
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoCV>(response);


                    responseD1.CodigoRespuesta = responseD.CodigoRespuesta;

                    if (responseD1.CodigoRespuesta == 0)
                    {
                        responseD.MensajeRespuesta = responseD.MensajeRespuesta;
                        responseD1.listado = responseD.listado;

                        ExcelPackage workbook = new ExcelPackage();
                        workbook.Workbook.Worksheets.Add("Reporte Reversos y Cancelaciones");
                        var hojaActual = workbook.Workbook.Worksheets["Reporte"];
                        hojaActual = workbook.Workbook.Worksheets[0];

                        hojaActual.Cells["A1"].Value = "Fecha Hora";
                        hojaActual.Cells["B1"].Value = "MTCid";
                        hojaActual.Cells["C1"].Value = "Establecimiento";
                        hojaActual.Cells["D1"].Value = "Producto";
                        hojaActual.Cells["E1"].Value = "Referencia";
                        hojaActual.Cells["F1"].Value = "Referencia 2";
                        hojaActual.Cells["G1"].Value = "Entrada";
                        hojaActual.Cells["H1"].Value = "Salida";
                        hojaActual.Cells["I1"].Value = "No. Autorización";

                        hojaActual.Cells["A1:I1"].Style.Font.Bold = true;
                        int linea = 2;

                        foreach (OperacionesCatalogoCVentaModel compra in responseD.listado)
                        {

                            hojaActual.Cells["A" + linea].Value = compra.fechaHora.ToString();
                            hojaActual.Cells["B" + linea].Value = compra.MTCid.ToString();
                            hojaActual.Cells["C" + linea].Value = compra.tienda;
                            hojaActual.Cells["D" + linea].Value = compra.producto;
                            hojaActual.Cells["E" + linea].Value = compra.referencia1;
                            hojaActual.Cells["F" + linea].Value = compra.referencia2;
                            hojaActual.Cells["G" + linea].Value = compra.entrada.ToString();
                            hojaActual.Cells["H" + linea].Value = compra.salida.ToString();
                            hojaActual.Cells["I" + linea].Value = compra.autorizacion.ToString();

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

                        }
                        responseD1.respuesta = base64Excel;
                        responseD1.reporteNombre = "Reporte Reversos y Cancelaciones.xlsx";

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
            return responseD1;
        }

        [Route("Reversos/CancelarTransaccion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_03")]
        public ResponseModel cancelarTransaccion(OperacionesRequestCancelarTransaccion item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Reversos/CancelarTransaccion", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Reversos/CancelarTransaccion",
                        Detalles = "Transacción cancelada exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
                Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

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

        #region Usuarios BackOffice

        [Route("Usuarios/listadoCatalogosGenericosUB")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08_04")]
        public OperacionesResponseCatalogosListadosGenericosUB listadoCatalogosGenericosUB()
        {

            OperacionesResponseCatalogosListadosGenericosUB responseD = new OperacionesResponseCatalogosListadosGenericosUB();
            try
            {
                RequestHelper request_roles = new RequestHelper(this.__appSettings);
                RequestHelper request_departamentos = new RequestHelper(this.__appSettings);


                Task<string> taskResponse_roles = request_roles.doPost("Operaciones/Usuarios/ListadoRoles", "", HttpContext.Session.GetString("token"));
                Task<string> taskResponse_departamentos = request_departamentos.doPost("Operaciones/Usuarios/ListadoDepartamentos", "", HttpContext.Session.GetString("token"));

                Task.WhenAll(taskResponse_roles, taskResponse_departamentos);
                string response_roles = taskResponse_roles.Result;
                string response_departamentos = taskResponse_departamentos.Result;


                if (request_roles.codigoRespuesta == 0 && request_departamentos.codigoRespuesta == 0)
                {
                    responseD.roles = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_roles);
                    responseD.departamentos = JsonConvert.DeserializeObject<OperacionesResponseListadoGenerico>(response_departamentos);

                    if (responseD.roles.CodigoRespuesta == 0 && responseD.departamentos.CodigoRespuesta == 0)
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

        [Route("Usuarios/NuevoUsuarioBO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_04")]
        public ResponseModel NuevoUsuarioBO(OperacionesRequestNuevoUsuario item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Usuarios/NuevoUsuarioBO", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Usuarios/NuevoUsuarioBO",
                        Detalles = "Usuario creado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("Usuarios/ListadoUsuariosCatalogo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_04")]
        public OperacionesResponseListadoUsuarios ListadoUsuariosCatalogo()
        {
            OperacionesResponseListadoUsuarios responseD = new OperacionesResponseListadoUsuarios();
            try
            {

                Task<string> taskResponse = request.doPost("Operaciones/Usuarios/ListadoUsuariosCatalogo", Seguridad.Encrypt(JsonConvert.SerializeObject(new OperacionesRequestUsuarioLista { idDepartamento = 0, activo = 1, idCatalogoRol = 0, menu = false })), HttpContext.Session.GetString("token"));
                taskResponse.Wait();
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseListadoUsuarios>(response);
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

        [Route("Usuarios/ResetClave")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_04")]
        public OperacionesResponseDefinirClave ResetClave(OperacionesRequestSetClave item)
        {
            OperacionesResponseDefinirClave responseD = new OperacionesResponseDefinirClave();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                item.clave = "";
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Usuarios/DefinirClaveUsuario", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<OperacionesResponseDefinirClave>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Usuarios/ResetClave",
                        Detalles = "Clave restablecida exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
                Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = ex.Message;
            }
            return responseD;
        }

        [Route("Usuarios/toogleUsuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_04")]
        public ResponseModel toogleUsuario(OperacionesRequestToggleUsuario item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Usuarios/toogleUsuario", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Usuarios/toogleUsuario",
                        Detalles = "Usuario estatus cambiado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
                }
                else
                {
                    responseD.CodigoRespuesta = request.codigoRespuesta;
                    responseD.MensajeRespuesta = request.mensajeRespuesta;
                }
                Logger.DoLog(MethodBase.GetCurrentMethod(), responseD);

            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), ex);
                responseD.CodigoRespuesta = -9;
                responseD.MensajeRespuesta = ex.Message;
            }
            return responseD;
        }

        [Route("Usuarios/EditarUsuarioBO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_04")]
        public ResponseModel EditarUsuarioBO(OperacionesRequestEditarUsuario item)
        {
            ResponseModel responseD = new ResponseModel();
            try
            {
                item.ip = Seguridad.GetIPAddress();
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                Task<string> taskResponse = request.doPost("Operaciones/Usuarios/EditarUsuarioBO", data, HttpContext.Session.GetString("token"));
                Task<Boolean> taskLog = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), item);
                Task.WhenAll(taskLog, taskResponse);
                string response = taskResponse.Result;

                if (request.codigoRespuesta == 0)
                {
                    responseD = JsonConvert.DeserializeObject<ResponseModel>(response);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "Usuarios/EditarUsuarioBO",
                        Detalles = "Usuario editado exitosamente",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        #region Dispositivos Billpocket


        [Route("dispositivosBP/listadoCadenas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_17")]
        public async Task<OperacionesResponseListadoCadenasTPV> listaCadenasTPV()
        {
            OperacionesResponseListadoCadenasTPV response = new OperacionesResponseListadoCadenasTPV();
            try
            {

                string reqResponse = await request.doPost("Operaciones/dispositivosBP/listadoCadenas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoCadenasTPV>(reqResponse);
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

        [Route("dispositivosBP/altaToken")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_09,PERM_08_17")]
        public async Task<OperacionesReponseRegistraTokenBP> registroUsuarioTPV([FromBody] OperacionesRequestNuevoUsuarioBillpocket item)
        {
            OperacionesReponseRegistraTokenBP response = new OperacionesReponseRegistraTokenBP();
            try
            {
                if (item.tipo_persona.CompareTo("1") == 0)
                {
                    item.razon_social = string.Empty;
                }

                item.email = item.idCadena + "@mtcenter.com.mx";
                item.password = util.encripta(item.idCadena+item.nombre.Substring(0,1));

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/dispositivosBP/altaToken", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesReponseRegistraTokenBP>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "dispositivosBP/altaToken",
                        Detalles = "Alta exitosa",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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

        [Route("dispositivosBP/estatus")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_17")]
        public async Task<OperacionesReponseEstatusTokenBP> estatusTPV([FromBody] OperacionesRequestEstatusTokenBP item)
        {
            OperacionesReponseEstatusTokenBP response = new OperacionesReponseEstatusTokenBP();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/dispositivosBP/estatusToken", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesReponseEstatusTokenBP>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "dispositivosBP/estatusToken",
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

        [Route("dispositivosBP/catalogosBP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_17")]
        public async Task<OperacionesReponseCatalogosBP> catalogosBP([FromBody] OperacionesRequestCatalogoBP item)
        {
            OperacionesReponseCatalogosBP response = new OperacionesReponseCatalogosBP();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/dispositivosBP/catalogosBP", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesReponseCatalogosBP>(reqResponse);
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

        [Route("dispositivosBP/infoCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_17")]
        public async Task<OperacionesResponseInfoCadenaTPV> infoBasicaCadena([FromBody] OperacionesInformacionCadena item)
        {
            OperacionesResponseInfoCadenaTPV response = new OperacionesResponseInfoCadenaTPV();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/dispositivosBP/infoCadena", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseInfoCadenaTPV>(reqResponse);

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


        [Route("dispositivosBP/registraLector")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_17")]
        public async Task<OpereacionesResponseAltaLector> registraLector([FromBody] OpereacionesRequestAltaLector item)
        {
            OpereacionesResponseAltaLector response = new OpereacionesResponseAltaLector();
            try
            {
                

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/dispositivosBP/registraLector", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OpereacionesResponseAltaLector>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "dispositivosBP/registraLector",
                        Detalles = "Alta exitosa",
                        Objeto = JsonConvert.SerializeObject(item)
                    }, __appSettings.LoggerOperaciones);
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


        [Route("dispositivosBP/lectoresCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_17")]
        public async Task<OpereacionesResponseLectores> lectoresCadena([FromBody] OpereacionesRequestLectores item)
        {
            OpereacionesResponseLectores response = new OpereacionesResponseLectores();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/dispositivosBP/lectoresCadena", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OpereacionesResponseLectores>(reqResponse);
                    
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

        #region Saldo con tarjeta

        [Route("saldoTarjeta/listadoCadenas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_16")]
        public async Task<OperacionesResponseListadoCadenasSaldoTarjeta> listadoCadenasSaldoTarjeta()
        {
            OperacionesResponseListadoCadenasSaldoTarjeta response = new OperacionesResponseListadoCadenasSaldoTarjeta();
            try
            {
               
                string reqResponse = await request.doPost("Operaciones/saldoTarjeta/listadoCadenas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoCadenasSaldoTarjeta>(reqResponse);
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

        [Route("saldoTarjeta/estatus")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_16")]
        public async Task<OperacionesReponseEstatusSaldoTarjeta> cambiaEstatusSaldoTarjeta([FromBody] OperacionesRequestEstatusSaldoTarjeta item)
        {
            OperacionesReponseEstatusSaldoTarjeta response = new OperacionesReponseEstatusSaldoTarjeta();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/saldoTarjeta/estatus", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesReponseEstatusSaldoTarjeta>(reqResponse);
                    util.logActividadUsuario(new ActividadUsuario
                    {
                        IdUsuario = int.Parse(HttpContext.Session.GetString("idUsuario")),
                        Usuario = HttpContext.Session.GetString("user"),
                        Seccion = "Operaciones",
                        Actividad = "saldoTarjeta/estatus",
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

        [Route("saldoTarjeta/infoCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_16")]
        public async Task<OperacionesResponseInformacionBasicaCadena> infoCadenaTPV([FromBody] OperacionesInformacionCadena item)
        {
            OperacionesResponseInformacionBasicaCadena response = new OperacionesResponseInformacionBasicaCadena();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/saldoTarjeta/infoCadena", data, HttpContext.Session.GetString("token"));


                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseInformacionBasicaCadena>(reqResponse);
               
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

        #region Cuentas STP

        [Route("cuentasSTP/getCuentasUsuarioSTP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_18")]
        public async Task<OperacionesResponseCuentasSTP> getCuentasSTP([FromBody] OperacionesRequestCuentasSTP item)
        {
            OperacionesResponseCuentasSTP response = new OperacionesResponseCuentasSTP();
            try
            {
                //if (!string.IsNullOrEmpty(item.idStr))
                //{
                //    item.idCajero = util.desencriptaId(item.idStr);
                //}
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/cuentasSTP/getCuentasUsuarioSTP", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseCuentasSTP>(reqResponse);
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

        [Route("cuentasSTP/resetCuentaSTP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_18")]
        public async Task<OperacionesResponseResetCuenta> resetCuentaSTP([FromBody] OperacionesRequestCuentasSTP item)
        {
            OperacionesResponseResetCuenta response = new OperacionesResponseResetCuenta();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/cuentasSTP/resetCuentaSTP", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseResetCuenta>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }
                string reqResponse2 = await request.doPost("Operaciones/cuentasSTP/getCuentasUsuarioSTP", data, HttpContext.Session.GetString("token"));
                response.cuentasSTP = JsonConvert.DeserializeObject<OperacionesResponseCuentasSTP>(reqResponse2);
            }
            catch (Exception ex)
            {
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }

        [Route("cuentasSTP/activaCuentaSTP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_18")]
        public async Task<OperacionesResponseResetCuenta> activaCuentaSTP([FromBody] OperacionesRequestActivaCuentasSTP item)
        {
            OperacionesResponseResetCuenta response = new OperacionesResponseResetCuenta();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/cuentasSTP/activaCuentaSTP", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseResetCuenta>(reqResponse);
                }
                else
                {
                    response.CodigoRespuesta = request.codigoRespuesta;
                    response.MensajeRespuesta = request.mensajeRespuesta;
                }
                string reqResponse2 = await request.doPost("Operaciones/cuentasSTP/getCuentasUsuarioSTP", data, HttpContext.Session.GetString("token"));
                response.cuentasSTP = JsonConvert.DeserializeObject<OperacionesResponseCuentasSTP>(reqResponse2);
            }
            catch (Exception ex)
            {
                response.CodigoRespuesta = -9;
                response.MensajeRespuesta = ex.Message;
            }
            return response;
        }


        [Route("AdministrarClientes/informacionCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_18")]
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


        [Route("AdministrarClientes/listadoCajeros")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_18")]
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

        #endregion

        #region Nueva cuenta cadena


        [Route("CuentaCadena/getCuentas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_19")]
        public async Task<OperacionesResponseCuentaCadenaLista> getCuentasCuentaCadena()
        {
            OperacionesResponseCuentaCadenaLista response = new OperacionesResponseCuentaCadenaLista();
            try
            {
                
                string reqResponse = await request.doPost("Operaciones/CuentaCadena/getCuentas", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseCuentaCadenaLista>(reqResponse);
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


        [Route("CuentaCadena/getCuentaCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_19")]
        public async Task<OperacionesResponseCuentasNuevaCuenta> getCuentas([FromBody] OperacionesRequestListadoCuentasCadena item)
        {
            OperacionesResponseCuentasNuevaCuenta response = new OperacionesResponseCuentasNuevaCuenta();
            try
            {

                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/CuentaCadena/getCuentaCadena", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseCuentasNuevaCuenta>(reqResponse);
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


        [Route("CuentaCadena/nuevaCuenta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_19")]
        public async Task<OperacionesResponseAltaNuevaCuenta> nuevaCuenta([FromBody] OperacionesRequestNuevaCuentaCadena item)
        {
            OperacionesResponseAltaNuevaCuenta response = new OperacionesResponseAltaNuevaCuenta();
            try
            {
                item.idCatalogoCuenta = 20; 
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/CuentaCadena/nuevaCuenta", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseAltaNuevaCuenta>(reqResponse);
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

        #endregion Nueva cuenta cadena

        #region Activación POSWEB
        [Route("activacionPOSWEB/getBloqueosAcceso")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_21")]
        public async Task<OperacionesResponseAccesopIP> getAccesoIP([FromBody] OperacionesRequestAccesosIP item)
        {
            OperacionesResponseAccesopIP response = new OperacionesResponseAccesopIP();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/ActivacionPOSWEB/getBloqueosAcceso", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseAccesopIP>(reqResponse);
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

        [Route("activacionPOSWEB/newAccesoPW")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_21")]
        public async Task<OperacionesResponseAccesopIP> newAccesoPW([FromBody] OperacionesRequestAccesosIP item)
        {
            OperacionesResponseAccesopIP response = new OperacionesResponseAccesopIP();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/ActivacionPOSWEB/newAccesoPW", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseAccesopIP>(reqResponse);
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

        [Route("activacionesPOSWEB/desbloqueaCadena")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_21")]
        public async Task<OperacionesResponseAccesopIP> desbloqueaCadena([FromBody] OperacionesRequestAccesosIP item)
        {
            OperacionesResponseAccesopIP response = new OperacionesResponseAccesopIP();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/ActivacionPOSWEB/desbloqueaCadena", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseAccesopIP>(reqResponse);
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

        [Route("activacionesPOSWEB/desbloqueaIP")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_21")]
        public async Task<OperacionesResponseAccesopIP> desbloqueaIP([FromBody] OperacionesRequestAccesosIP item)
        {
            OperacionesResponseAccesopIP response = new OperacionesResponseAccesopIP();
            try
            {
                item.idUsuario = int.Parse(HttpContext.Session.GetString("idUsuario"));
                item.ip = Seguridad.GetIPAddress();
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/ActivacionPOSWEB/desbloqueaIP", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseAccesopIP>(reqResponse);
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


        #region Codigos QR


        [Route("CodigosQR/getClientes")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_20")]
        public async Task<OperacionesResponseListadoClientesQR> clientesQR()
        {
            OperacionesResponseListadoClientesQR response = new OperacionesResponseListadoClientesQR();
            try
            {

                string reqResponse = await request.doPost("Operaciones/CodigosQR/getClientes", "", HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoClientesQR>(reqResponse);
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

        [Route("CodigosQR/editaTerminalQR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_20")]
        public async Task<OperacionesResponseEditaTerminal> editaTerminalQR([FromBody] OperacionesRequestEditaTerminalCodigoQR item)
        {
            OperacionesResponseEditaTerminal response = new OperacionesResponseEditaTerminal();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/CodigosQR/editaTerminalQR", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseEditaTerminal>(reqResponse);
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

        [Route("CodigosQR/registraShortCodeQR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_20")]
        public async Task<OperacionesResponseRegistraShortCode> registraShortCodeQR([FromBody] OperacionesRequestRegistraShortCode item)
        {
            OperacionesResponseRegistraShortCode response = new OperacionesResponseRegistraShortCode();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/CodigosQR/registraShortCodeQR", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseRegistraShortCode>(reqResponse);
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

        [Route("CodigosQR/editaEstatusClienteQR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_20")]
        public async Task<OperacionesResponseEditaEstatusClienteQR> editaEstatusClienteQR([FromBody] OperacionesRequestCambiaEstatusClienteQR item)
        {
            OperacionesResponseEditaEstatusClienteQR response = new OperacionesResponseEditaEstatusClienteQR();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/CodigosQR/editaEstatusClienteQR", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseEditaEstatusClienteQR>(reqResponse);
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

        [Route("CodigosQR/terminalesClientesQR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MTCAuthorizationHandler("PERM_08,PERM_08_20")]
        public async Task<OperacionesResponseListadoTerminalesEstablecimientoQR> terminalesClientesQR([FromBody] OperacionesRequestEstablecimientoTerminal item)
        {
            OperacionesResponseListadoTerminalesEstablecimientoQR response = new OperacionesResponseListadoTerminalesEstablecimientoQR();
            try
            {
                string data = Seguridad.Encrypt(JsonConvert.SerializeObject(item));
                string reqResponse = await request.doPost("Operaciones/CodigosQR/terminalesClientesQR", data, HttpContext.Session.GetString("token"));

                if (request.codigoRespuesta == 0)
                {
                    response = JsonConvert.DeserializeObject<OperacionesResponseListadoTerminalesEstablecimientoQR>(reqResponse);
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