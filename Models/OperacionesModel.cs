using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class OperacionesModel
    {

        #region Conciliaciones

        #region Modelos
        public class listaConciliacionProveedores
        {
            public int id { get; set; }
            public string proveedor { get; set; }
            public string fecha { get; set; }
            public string estatus { get; set; }
            public bool check { get; set; } = false;
        }
        public class listaConciliacionMayoritas
        {
            public int id { get; set; }
            public int idCliente { get; set; }
            public string idEstablecimiento { get; set; }
            public string nombreComercial { get; set; }
            public string fecha { get; set; }
            public string estatus { get; set; }
            public bool check { get; set; } = false;
        }
        #endregion
        #region RequestModel

        public class OperacionesRequestRealizarConciliacionProveedor
        {
            public string fecha { get; set; }
            public string idProveedores { get; set; }
        }

        public class OperacionesRequestAgregarNuevoProveedor
        {
            public string nombreProv { get; set; }
            public string sp { get; set; }
        }

        public class OperacionesRequestRealizarConciliacionMayorista
        {
            public string fecha { get; set; }
            public string id { get; set; }
        }

        public class OperacionesRequestAgregarNuevoMayorista
        {
            public int idCliente { get; set; }
            public int idEstablecimiento { get; set; }
            public string nombreComercial { get; set; }
            public string carpeta { get; set; }
            public string sp { get; set; }
        }

        #endregion
        #region ResponseModel
        public class OperacionesResponseListaConciliacionesProveedores : ResponseModel
        {
            public List<listaConciliacionProveedores> listaProveedores { get; set; }
        }

        public class OperacionesResponseNuevaConciliacion: ResponseModel
        {
            public bool agregado { get; set; }
        }

        public class OperacionesResponseListaConciliacionesMayoristas : ResponseModel
        {
            public List<listaConciliacionMayoritas> listaMayoristas { get; set; } = new List<listaConciliacionMayoritas>();
        }

        
        #endregion
        #endregion

        #region Administrar Compras

        #region Modelos

        public class EstatusCompras
        {
            public int orden { get; set; }
            public int idCadena { get; set; }
            public string fechaHora { get; set; }
            public string nombre { get; set; }
            public int cantidad { get; set; }
            public decimal costo { get; set; }
            public decimal comisionRecarga { get; set; }
            public string idCatalogoEstatusCompras { get; set; }
            public string estatus { get; set; }
            public string observaciones { get; set; }
        }

        public class DetalleProducto
        {
            public string nombre { get; set; }
            public string detalle { get; set; }
            public decimal precioVenta { get; set; }
            public string ligaImagen { get; set; }

        }

        #endregion

        #region RequestModels

        public class OperacionesRequestListadoEstatusCompras
        {
            public int idEstatus { get; set; }
            public int idCadena { get; set; }
            public string fechaIncio { get; set; }
            public string fechaFin { get; set; }
        }

        public class OperacionesRequestSetObervacionesCompra
        {
            public int idCompraExitosa { get; set; }
            public string observaciones { get; set; }
        }

        public class OperacionesRequestSetEstatusCompra
        {
            public int id { get; set; }
            public int idCatalogoEstatusCompras { get; set; }
            public int idUsuario { get; set; }
            public string ip { get; set; }
        }

        public class OperacionesRequestProductoCompra
        {
            public int id { get; set; }
            public int activo { get; set; }
            public int menu { get; set; }
        }
        
        public class OperacionesRequestListadoEstablecimientos
        {
            public int idCadena { get; set; }
            public int activo { get; set; }
            public int menu { get; set; }
        }

        public class OpereacionesRequestInsertaCompra : UsuarioIp
        {
            public int idCadena { get; set; }
            public int idTienda { get; set; }
            public int cantidad { get; set; }
            public int idProducto { get; set; }
            public float costo { get; set; }
            public bool saldoIDE { get; set; }
        }

        public class OperacionesRequestListadoFamilias
        {
            public int id { get; set; }
            public int activo { get; set; }
            public int menu { get; set; }
        }

        #endregion

        #region ResponseModels

        public class OperacionesResponseListadoEstatusCompras : ResponseModel
        {
            public List<EstatusCompras> listadoCompras { get; set; }
        }

        public class OperacionesReponseListadoProductoCompra : ResponseModel
        {
            public List<ListadoLabel> listadoProductos { get; set; }
        }

        public class OperacionesResponseEditaCompra : ResponseModel
        {
            public bool editado { get; set; }
        }

        public class OperacionesResponseListadoEstablecimientos : ResponseModel
        {
            public List<ListadoLabel> listadoEstablecimientos { get; set; }
        }

        public class OperacionesResponsePrecioProducto : ResponseModel
        {
            public decimal precio { get; set; }
        }

        public class OperacionesResponseDetalleProducto : ResponseModel
        {
            public DetalleProducto detalle { get; set; }
        }

        public class OperacionesResponseAgregarCompra : ResponseModel
        {
            public bool agregada { get; set; }
        }

        public class OperacionesResponseComisionCadena : ResponseModel
        {
            public decimal comision { get; set; }
        }
        #endregion

        #endregion

        #region Administrar Familia Comerciales

        #region Modelos

        public class ListadoLabel
        {
            public int id { get; set; }
            public string label { get; set; }
        }

        public class ProductosFamiliaComercial
        {
            public int id { get; set; }
            public string nombre { get; set; }
            public string proveedor { get; set; }
            public float comisionEstablecimiento { get; set; }
            public bool porcentaje { get; set; }
            public bool activo { get; set; }
        }

        public class ProductosFamiliaComercialExcel : ResponseModel
        {
            public string respuesta { get; set; }
            public string reporteNombre { get; set; }
            public List<ProductosFamiliaComercial> respuestaLista { get; set; }
        }
        
        #endregion

        #region RequestModels

        public class OperacionesRequestGetProductosFamiliaComercial
        {
            public int idFamilia { get; set; }
            public bool existe { get; set; }
        }

        public class OperacionesRequestAgregarProductoFamilia : UsuarioIp
        {
            public int idFamilia { get; set; }
            public int idCatalogoProducto { get; set; }
            public float comisionEstablecimiento { get; set; }
            public bool porcentaje { get; set; }
        }

        public class OperacionesRequestEstatusProductoFamilia : UsuarioIp
        {
            public int idFamilia { get; set; }
            public int idCatalogoProducto { get; set; }
            public bool activo { get; set; }
        }

        public class OperacionesRequestSetProductoFamilia : UsuarioIp
        {
            public int idFamilia { get; set; }
            public int idCatalogoProducto { get; set; }
            public float comisionEstablecimiento { get; set; }
            public bool porcentaje { get; set; }
        }

        public class OperacionesRequestDeleteProductoFamilia : UsuarioIp
        {
            public int idFamilia { get; set; }
            public int idCatalogoProducto { get; set; }
        }
        #endregion

        #region ResponseModels

        public class OperacionesResponseListadoFamiliaComercial : ResponseModel
        {
            public List<ListadoLabel> listadoFamiliaComercial { get; set; }
        }

        public class OperacionesResponseListadoProductosFamiliaComercial : ResponseModel
        {
            public List<ProductosFamiliaComercial> listadoProductos { get; set; }
        }

        public class OperacionesResponseAgregaProductoFamilia : ResponseModel
        {
            public bool agregado { get; set; }
        }

        public class OperacionesResponseEstatusProductoFamilia : ResponseModel
        {
            public bool cambiado { get; set; }
        }

        public class OperacionesResponseSetProductoFamilia : ResponseModel
        {
            public bool cambiado { get; set; }
        }

        public class OperacionesResponseEliminaProductoFamilia : ResponseModel
        {
            public bool eliminado { get; set; }
        }


        #endregion

        #endregion

        #region Switch Proveedores

        #region Modelos

        public class ConfiguracionCasos
        {
            public string proveedor { get; set; }
            public string regiones { get; set; }
            public string grupos { get; set; }
            public string familias { get; set; }
        }



        #endregion


        #region RequestModels

        public class OperacionesRequestGetCasosSwitch
        {
            public int idIntermediario { get; set; }
        }

        public class OperacionesRequestLabelCaso
        {
            public int idIntermediario { get; set; }
        }

        public class OperacionesRequestSwitchCaso
        {
            public int idIntermediario { get; set; }
            public int caso { get; set; }
        }
        #endregion



        #region ResponseModels



        #endregion

        public class OperacionesResponseListadoSwitchProveedores : ResponseModel
        {
            public List<ListadoLabel> listadoProveedores { get; set; }
        }

        public class OperacionesResponseListadoCasosSwitch : ResponseModel
        {
            public List<ListadoLabel> listadoCasosSwitch { get; set; }
        }

        public class OperacionesResponseListadoConfiguracionCasos : ResponseModel
        {
            public List<ConfiguracionCasos> listadoConfiguracionCasos { get; set; }
        }

        public class OperacionesResponseLabelCaso : ResponseModel
        {
            public string nombre { get; set; }
        }

        public class OperacionesResponseRealizaSwitch : ResponseModel
        {
            public bool realizado { get; set; }
        }
        #endregion

        #region Switch Familias

        #region Modelos

        public class ProveedoresRegion
        {
            public string id { get; set; }
            public string intermediario { get; set; }
            public string proveedor { get; set; }
            public string region { get; set; }
            public string grupo { get; set; }
            public string familia { get; set; }
            public bool status { get; set; }
        }

        #endregion

        #region Request Models

        public class OperacionesRequestProveedoresRegion
        {
            public int idProveedor { get; set; }
            public int idRegion { get; set; }
        }

        public class OperacionesRequestDeleteProveedoresRegion : UsuarioIp
        {
            public string idStr { get; set; }
            public int id { get; set; }
        }

        public class OperacionesRequestNuevoProveedorRegion : UsuarioIp
        {
            public int idCatalogoProveedor { get; set; }
            public int idCatalogoRegion { get; set; }
            public int idGrupo { get; set; }
            public int idFamilia { get; set; }
            public int idIntermediario { get; set; }
            public bool estatus { get; set; }

        }

        public class OperacionesRequestSetProveedoresRegion : UsuarioIp
        {
            public string idStr { get; set; }
            public int id { get; set; } = 0;
            public bool estatus { get; set; }
        }

        public class OperacionesRequestListadoProveedores 
        {
            public int idCatalogoIntermediario { get; set; }

        }
        #endregion


        #region ResponseModels


        public class OperacionesResponseListadoProveedoresRegion : ResponseModel
        {
            public List<ProveedoresRegion> listadoProveedoresRegion { get; set; }
        }

        public class OperacionesResponseEliminaProveedorRegion : ResponseModel
        {
            public bool eliminado { get; set; }
        }

        public class OperacionesResponseNuevoProveedorRegion : ResponseModel
        {
            public bool agregado { get; set; }
        }

        public class OperacionesResponseSetProveedorRegion : ResponseModel
        {
            public bool cambiado { get; set; }
        }

        public class OperacionesResponseListadosNuevoProveedor : ResponseModel
        {
            public List<ListadoLabel> listadoIntermediario { get; set; }
            public List<ListadoLabel> listadoRegiones { get; set; }
            public List<ListadoLabel> listadoGrupo { get; set; }
            public List<ListadoLabel> listadoFamilia { get; set; }

        }

        public class OperacionesResponseListadoProveedores : ResponseModel
        {
            public List<ListadoLabel> listadoProveedores { get; set; }

        }

        #endregion


        #endregion

        #region Switch Administrar Familias

        #region Modelos

        public class ListadoLabelComision : ListadoLabel
        {
            public float comisionRecarga { get; set; }
        }

        public class OperacionesRequestAgregarFamilia : UsuarioIp
        {
            public string nombre { get; set; }
            public float comision { get; set; }
        }

        public class OperacionesRequestEditarFamilia : OperacionesRequestAgregarFamilia
        {
            public string id { get; set; }
        }

        #endregion

        #region Request Models

        #endregion

        #region Response Models

        public class OperacionesResponseListadoFamilias : ResponseModel
        {
            public List<ListadoLabelComision> listadoFamilias { get; set; }
        }

        #endregion



        #endregion

        #region Switch Productos

        #region Models


        public class ListadoLabelProductos
        {
            public int id { get; set; }
            public int idCatalogoProveedor { get; set; }
            public string claveProducto { get; set; }
            public string nombre { get; set; }
            public string descripcionTicket { get; set; }
            public int idCatalogoClasificacionProducto { get; set; }
            public float costo { get; set; }
            public float maxImporte { get; set; }
            public float comisionMTC { get; set; }
            public int idCatalogoIntermediario { get; set; }
            public int comisionIntermediario { get; set; }
            public int comisionTotal { get; set; }
            public int idFamiliaProducto { get; set; }
            public string mensaje1 { get; set; }
            public string mensaje2 { get; set; }
            public string fechaAlta { get; set; }
            public int activo { get; set; }
        }

        public class OperacionesRequestGetProductos
        {
            public int id { get; set; }
            public int menu { get; set; }
            public int activo { get; set; }
        }

        public class OperacionesRequestCatalogo
        {
            public int id { get; set; }
            public int menu { get; set; }
        }


        #endregion

        #region RequestModels
        public class OperacionesRequestAgregarProducto : UsuarioIp{
            public int idCatalogoProveedor { get; set; }
            public string claveProducto { get; set; }
            public string nombre { get; set; }
            public string descripcionTicket { get; set; }
            public int idCatalogoClasificacionProducto { get; set; }
            public float costo { get; set; }
            public float maxImporte { get; set; }
            public float comisionMTC { get; set; }
            public int idCatalogoIntermediario { get; set; }
            public float comisionIntermediario { get; set; }
            public float comisionTotal { get; set; }
            public int idFamiliaProducto { get; set; }
            public string mensaje1 { get; set; }
            public string mensaje2 { get; set; }
        }

        public class OperacionesRequestEditarProducto : OperacionesRequestAgregarProducto
        {
            public int id { get; set; }
        }

        public class OperacionesRequestToggleProducto : UsuarioIp
        {
            public int id { get; set; }
            public Boolean activo { get; set; }
        }
        #endregion

        #region ResponseModels

        public class OperacionesResponseListadoProductos : ResponseModel
        {
            public List<ListadoLabelProductos> listadoProductos { get; set; }
        }

        public class OperacionesResponseListadoGenerico : ResponseModel
        {
            public List<ListadoLabel> listado { get; set; }
        }

        public class OperacionesResponseCatalogosListadosGenericos : ResponseModel
        {
            public OperacionesResponseListadoGenerico proveedores { get; set; }
            public OperacionesResponseListadoGenerico cproductos  { get; set; }
            public OperacionesResponseListadoGenerico intermediarios { get; set; }
            public OperacionesResponseListadoGenerico fproductos { get; set; }
        }


        #endregion

        #endregion

        #region Comunicados

        #region Models

        public class ElementoComunicado
        {
            public int id { get; set; }
            public int idGrupo { get; set; }
            public int idCadena { get; set; }
            public int idEstablecimiento { get; set; }
            public string titulo { get; set; }
            public string mensaje { get; set; }
            public DateTime fechaInicio { get; set; }
            public DateTime fechaFin { get; set; }
            public string contacto { get; set; }
            public string dirigido { get; set; }
            public int idContacto { get; set; }
            public Boolean general { get; set; }
        }

        #endregion

        #region RequestModels

        public class OperacionesRequestListadoComunicados
        {
            public int id { get; set; }
        }

        public class OperacionesRequestGetCatalogo : OperacionesRequestCatalogo
        {
            public int activo { get; set; }
        }

        public class OperacionesRequestGetCatalogoCadena : OperacionesRequestGetCatalogo
        {
            public int idGrupo { get; set; }
        }

        public class OperacionesRequestGetCatalogoCadenaEstablecimiento
        {
            public OperacionesRequestGetCatalogoCadena cadenas { get; set; }
            public OperacionesRequestGetCatalogoEstablecimiento establecimientos { get; set; }
        }


        public class OperacionesRequestGetCatalogoEstablecimiento : OperacionesRequestGetCatalogo
        {
            public int idCadena { get; set; }
        }

        public class OperacionesRequestComunicadoAdd : UsuarioIp
        {
            public string general { get; set; }
            public int idGrupo { get; set; }
            public int idCadena { get; set; }
            public int idEstablecimiento { get; set; }
            public string titulo { get; set; }
            public string mensaje { get; set; }
            public DateTime fechaInicio { get; set; }
            public DateTime fechaFin { get; set; }
            public int idContacto { get; set; }
        }

        public class OperacionesRequestComunicadoEdit : OperacionesRequestComunicadoAdd
        {
            public int id { get; set; }
        }

        #endregion

        #region ResponseModels

        public class OperacionesResponseListadoComunicados : ResponseModel
        {
            public List<ElementoComunicado> listado { get; set; }
        }

        public class OperacionesResponseCatalogosListadosGenericosCom : ResponseModel
        {
            public OperacionesResponseListadoGenerico grupos { get; set; }
            public OperacionesResponseListadoGenerico contactos { get; set; }
        }


        public class OperacionesResponseCadenasEstablecimientos : ResponseModel
        {
            public OperacionesResponseListadoGenerico cadenas { get; set; }
            public OperacionesResponseListadoGenerico establecimientos { get; set; }
        }

        #endregion

        #endregion

        #region Reversos y Cancelaciones

        #region Models

        public class OperacionesCatalogoCVentaModel
        {
            public int id { get; set; }
            public DateTime fechaHora { get; set; }
            public int MTCid { get; set; }
            public int idTienda { get; set; }
            public string tienda { get; set; }
            public string cajero { get; set; }
            public string producto { get; set; }
            public string proveedor { get; set; }
            public string referencia1 { get; set; }
            public string referencia2 { get; set; }
            public float entrada { get; set; }
            public float salida { get; set; }
            public float comisionRecarga { get; set; }
            public float comisionEstablecimiento { get; set; }
            public int autorizacion { get; set; }
            public string tipoTerminal { get; set; }
            public string estatusCancelada { get; set; }
            public int idCatalogoEstatusVenta { get; set; }
            public string estatusCancelaciones { get; set; }
            public string observaciones { get; set; }
        }

        #endregion

        #region RequestModels
        public class OperacionesRequestListadoVentasCancelar
        {
            public DateTime desde { get; set; }
            public DateTime hasta { get; set; }
            public string idCadena { get; set; }
            public int idTienda { get; set; }
            public int clasificacion { get; set; }
            public int idProducto { get; set; }
            public string monto { get; set; }
            public string referencia { get; set; }
            public int idProveedor { get; set; }
            public int idCatalogoEstatusVenta { get; set; }
        }

        public class OperacionesRequestCancelarTransaccion : UsuarioIp
        {
            public int idVenta { get; set; }
            public string observaciones { get; set; }
        }

        #endregion

        #region ResponseModels

        public class OperacionesResponseListadoGenericoRC : ResponseModel
        {
            public List<ListadoLabel> respuesta { get; set; }
        }

        public class OperacionesResponseListadoPC : ResponseModel
        {
            public List<ReporteCatalogoProductoModel> listado { get; set; }
        }

        public class OperacionesResponseObtenerCRC : ResponseModel
        {
            public OperacionesResponseListadoGenericoRC clasificaciones { get; set; }
            public OperacionesResponseListadoPC productos { get; set; }
            public OperacionesResponseListadoProveedores proveedores { get; set; }

        }


        public class OperacionesResponseListadoCV : ResponseModel
        {
            public List<OperacionesCatalogoCVentaModel> listado { get; set; }
        }

        public class OperacionesResponseListadoCVExcel : ResponseModel
        {
            public string respuesta { get; set; }
            public string reporteNombre { get; set; }
            public List<OperacionesCatalogoCVentaModel> listado { get; set; }
        }
        #endregion

        #endregion

        #region Usuarios BackOffice

        #region Models
        public class OperacionesUsuarioFListado
        {
            public int id { get; set; }
            public string label { get; set; }
            public string nombreDepartamento { get; set; }
            public string nombreRol { get; set; }
            public int statusClave { get; set; }
            public string resetClave { get; set; }
            public string usuario { get; set; }
            public Boolean activo { get; set; }
            public int idCatalogoRol { get; set; }
            public int idCatalogoDepartamento { get; set; }
            public string email { get; set; }
            public int jefeDepartamento { get; set; }
        }

        #endregion

        #region RequestModels
        public class OperacionesRequestNuevoUsuario : UsuarioIp
        {
            public string nombre { get; set; }
            public string apellidoPaterno { get; set; }
            public string apellidoMaterno { get; set; }
            public string email { get; set; }
            public string usuario { get; set; }
            public int idCatalogoRol { get; set; }
            public int idCatalogoDepartamento { get; set; }
            public int jefeDepartamento { get; set; }
        }

        public class OperacionesRequestUsuarioLista
        {
            public int idCatalogoRol { get; set; }
            public int idDepartamento { get; set; }
            public int activo { get; set; }
            public Boolean menu { get; set; }
        }


        public class OperacionesRequestSetClave : UsuarioIp
        {
            public int id { get; set; }
            public string clave { get; set; }
        }

        public class OperacionesRequestToggleUsuario : UsuarioIp
        {
            public int id { get; set; }
            public int activo { get; set; }
        }

        public class OperacionesRequestEditarUsuario : UsuarioIp
        {
            public int id { get; set; }
            public string email { get; set; }
            public int idCatalogoRol { get; set; }
            public int idCatalogoDepartamento { get; set; }
            public int jefeDepartamento { get; set; }
            public int activo { get; set; }
        }

        #endregion

        #region ResponseModels
        public class OperacionesResponseCatalogosListadosGenericosUB : ResponseModel
        {
            public OperacionesResponseListadoGenerico roles { get; set; }
            public OperacionesResponseListadoGenerico departamentos { get; set; }
        }


        public class OperacionesResponseListadoUsuarios : ResponseModel
        {
            public List<OperacionesUsuarioFListado> listado { get; set; }
        }

        public class OperacionesResponseDefinirClave : ResponseModel
        {
            public string clave { get; set; }
        }

        #endregion

        #endregion

        #region Dispositivos Billpocket

        #region Models

        public class CadenasDispositivosTPV
        {
            public int idCadena { get; set; }
            public string razonSocial { get; set; }
            public string contacto { get; set; }
            public string telefono1 { get; set; }
            public bool activo { get; set; }
        }

        public class GenericBPTPV
        {
            public int id { get; set; } = 0;
            public string desc { get; set; } = string.Empty;
            public string abrv { get; set; } = string.Empty;

        }

        public class InformacionCadena
        {
            public string contacto { get; set; }
            public string email { get; set; }
            public string telefono1 { get; set; }
            public string rfc { get; set; }
            public string calle { get; set; }
            public string noExterior { get; set; }
            public string noInterior { get; set; }
            public string codigoPostal { get; set; }
            public string colonia { get; set; }
            public string razonSocial { get; set; }

        }

        public class LectorCadena
        {
            public int idCadena { get; set; }
            public string sn { get; set; }
            public bool registrado { get; set; }
        }

        public class Lector
        {
            public int id { get; set; }
            public string sn { get; set; }
            public string fechaHora { get; set; }
        }


        #endregion

        #region RequestModels

        public class OperacionesRequestAltaTokenBP
        {
            public int idCadena { get; set; }
            public string token { get; set; }
        }

        public class OperacionesRequestEstatusTokenBP
        {
            public int idCadena { get; set; }
            public int estatus { get; set; }
        }

        public class OperacionesRequestNuevoUsuarioBillpocket
        {
            public string email { get; set; } = string.Empty;
            public string password { get; set; } = string.Empty;
            public string telefono { get; set; } = string.Empty;
            public string nombre { get; set; } = string.Empty;
            public string apellido_paterno { get; set; } = string.Empty;
            public string apellido_materno { get; set; } = string.Empty;
            public string tipo_persona { get; set; } = string.Empty;
            public string actividad_comercial { get; set; } = string.Empty;
            public string curp { get; set; } = string.Empty;
            public string rfc { get; set; } = string.Empty;
            public string calle { get; set; } = string.Empty;
            public string exterior { get; set; } = string.Empty;
            public string interior { get; set; } = string.Empty;
            public string colonia { get; set; } = string.Empty;
            public string codigo_postal { get; set; } = string.Empty;
            public string estado { get; set; } = string.Empty;
            public string municipio { get; set; } = string.Empty;
            public string razon_social { get; set; } = string.Empty;
            public string tipo_sociedad { get; set; } = string.Empty;
            public string fecha_constitucion { get; set; } = string.Empty;
            public string representante_legal { get; set; } = string.Empty;
            public int idCadena { get; set; } = 0;
        }

        public class OperacionesRequestCatalogoBP
        {
            public int tipo { get; set; }
            public int idEstado { get; set; }
        }

        public class OpereacionesRequestAltaLector
        {
            public List<LectorCadena> lectores { get; set; }
        }

        public class OpereacionesRequestLectores
        {
            public int idCadena { get; set; }
        }


        #endregion

        #region ResponseModels

        public class OperacionesResponseListadoCadenasTPV : ResponseModel
        {
            public List<CadenasDispositivosTPV> listadoCadenas { get; set; }
        }

        public class OperacionesReponseRegistraTokenBP : ResponseModel
        {
            public bool response { get; set; }
        }

        public class OperacionesReponseEstatusTokenBP : ResponseModel
        {
            public bool response { get; set; }
        }

        public class OperacionesReponseCatalogosBP : ResponseModel
        {
            public List<GenericBPTPV> listado { get; set; } = new List<GenericBPTPV>();
        }
        public class OperacionesResponseInfoCadenaTPV : ResponseModel
        {
            public InformacionCadena informacion { get; set; }
        }
        public class OpereacionesResponseAltaLector : ResponseModel
        {
            public List<LectorCadena> lectores { get; set; }
        }
        public class OpereacionesResponseLectores : ResponseModel
        {
            public List<Lector> listado { get; set; }
        }

        #endregion

        #endregion

        #region Saldo con tarjeta

        #region Models

        public class CadenaSaldoTarjeta
        {
            public int idCadena { get; set; }
            public string nombreComercial { get; set; }
            public string rfc { get; set; }
            public string contacto { get; set; }
            public string telefono { get; set; }
            public bool depositoTarjeta { get; set; }
        }

        public class DatosBasicosCadena
        {

            public string nombreComercial { get; set; }
            public string rfc { get; set; }
            public string razonSocial { get; set; }
            public string contacto { get; set; }
        }

        #endregion

        #region RequestModels


        public class OperacionesRequestEstatusSaldoTarjeta
        {
            public int idCadena { get; set; }
            public int estatus { get; set; }
        }

        public class OperacionesInformacionCadena : UsuarioIp
        {
            public int idCadena { get; set; }
        }


        #endregion

        #region ResponseModels

        public class OperacionesResponseListadoCadenasSaldoTarjeta : ResponseModel
        {
            public List<CadenaSaldoTarjeta> listadoCadenas { get; set; }
        }

        public class OperacionesReponseEstatusSaldoTarjeta : ResponseModel
        {
            public bool response { get; set; }
        }

        public class OperacionesResponseInformacionBasicaCadena : ResponseModel
        {
            public DatosBasicosCadena infoCadena { get; set; }
        }

        #endregion

        #endregion

        #region Reset Cuentas STP
        #region Models

        public class cuentasClienteSTP
        {
            public int idCajero { get; set; }
            public int idCadena { get; set; }
            public int idEstablecimiento { get; set; }
            public string cuenta { get; set; }
            public int idBanco { get; set; }
            public string bancoLabel { get; set; }
            public int tipoCuenta { get; set; }
            public string tipoCuentaLabel { get; set; }
            public int activa { get; set; }
            public int id { get; set; }
        }

        #endregion

        #region RequestModels
        public class OperacionesRequestCuentasSTP
        {
            public int idCadena { get; set; } = 0;
            public string cuenta { get; set; } = "";
            public string idStr { get; set; }
        }
        public class OperacionesRequestActivaCuentasSTP
        {
            public int idCajero { get; set; } = 0;
            public int idCadena { get; set; } = 0;
            public string cuenta { get; set; } = "";
            public int activa { get; set; }
            public int id { get; set; }
        }

        #endregion

        #region ResponseModels
        public class OperacionesResponseCuentasSTP : ResponseModel
        {
            public List<cuentasClienteSTP> cuentasSTP { get; set; }
        }
        public class OperacionesResponseResetCuenta : ResponseModel
        {
            public int response { get; set; }
            public OperacionesResponseCuentasSTP cuentasSTP { get; set; }
        }

        #endregion
        #endregion

        #region Nueva cuenta cadena

        #region Models

        public class Cuentas
        {
            public int id { get; set; }
            public string label { get; set; }
        }

        public class CuentaCadena
        {
            public int idCadena { get; set; }
            public string razonSocial { get; set; }
            public string banco { get; set; }
            public string numeroCuenta { get; set; }
        }

        #endregion Models

        #region RequestModels

        public class OperacionesRequestListadoCuentasCadena
        {
            public int idCadena { get; set; }
        }
        public class OperacionesRequestNuevaCuentaCadena : UsuarioIp
        {
            public int idCadena { get; set; }
            public int idCatalogoCuenta { get; set; }
        }

        #endregion RequestModels

        #region ResponseModels
        public class OperacionesResponseCuentaCadenaLista : ResponseModel
        {
            public List<Cuentas> cuentas { get; set; }
        }
        public class OperacionesResponseCuentasNuevaCuenta : ResponseModel
        {
            public List<CuentaCadena> cuentaCadena { get; set; }
        }

        public class OperacionesResponseAltaNuevaCuenta : ResponseModel
        {
            public bool registrada { get; set; }
        }

        #endregion ResponseModels

        #endregion Nueva cuenta cadena

        #region Codigos QR

        #region Models

        public class ClientesQR
        {
            public int id { get; set; }
            public int idCadena { get; set; }
            public int idEstablecimiento { get; set; }
            public string razonSocial { get; set; }
            public string nombre { get; set; }
            public string numero { get; set; }
            public string shortCode { get; set; }
            public string url { get; set; }
            public int activo { get; set; }
        }

        public class TerminalesClientesQR
        {
            public int idTerminal { get; set; }
            public string numero { get; set; }
            public string nombre { get; set; }
        }

        #endregion Models

        #region RequestModels

        public class OperacionesRequestEditaTerminalCodigoQR
        {
            public int idTerminal { get; set; }
            public int idClienteCodigo { get; set; }
        }
        public class OperacionesRequestEstablecimientoTerminal
        {
            public int idEstablecimiento { get; set; }
        }
        public class OperacionesRequestRegistraShortCode
        {
            public string shortCode { get; set; }
            public string url { get; set; }
            public int idClienteCodigo { get; set; }
        }
        public class OperacionesRequestCambiaEstatusClienteQR
        {
            public int activo { get; set; }
            public int idClienteCodigo { get; set; }
        }



        #endregion RequestModels

        #region ResponseModels

        public class OperacionesResponseListadoClientesQR : ResponseModel
        {
            public List<ClientesQR> listado { get; set; }
        }

        public class OperacionesResponseEditaTerminal : ResponseModel
        {
            public bool editado { get; set; }
        }

        public class OperacionesResponseRegistraShortCode : ResponseModel
        {
            public bool registrado { get; set; }
        }

        public class OperacionesResponseEditaEstatusClienteQR : ResponseModel
        {
            public bool editado { get; set; }
        }

        public class OperacionesResponseListadoTerminalesEstablecimientoQR : ResponseModel
        {
        
            public List<TerminalesClientesQR> listado { get; set; }
        }


        #endregion ResponseModels

        #endregion

        #region Activación POSWEB

        #region Models
        public class AccesosIP
        {
            public int id { get; set; }
            public int idCadena { get; set; }
            public string ip { get; set; }
            public string descripciones { get; set; }
            public string fechaHora { get; set; }
            public int cadenaActiva { get; set; }
            public string ipActiva { get; set; }
            public int cadenaBloqueda { get; set; }
            public string ipBloqueada { get; set; }
        }

        #endregion
        #region RequestModels

        public class OperacionesRequestAccesosIP
        {
            public int idCadena { get; set; }
            public string ip { get; set; } = "";
            public int idUsuario { get; set; } = 0;
            public string ipAlta { get; set; } = "";
        }
        #endregion
        #region ResponeModel

        public class OperacionesResponseAccesopIP : ResponseModel
        {
            public List<AccesosIP> info { get; set; }
        }
        #endregion

        #endregion

    }


}
