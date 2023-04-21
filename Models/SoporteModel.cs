using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class SoporteModel
    {

    }


    #region Administrar Clientes

    #region Modelos

    public class AdministrarInformacionCadena
    {
        public int inicioCredito { get; set; }
        public int finCredito { get; set; }
        public string fechaMod { get; set; }
        public int corte { get; set; }
        public int factura { get; set; }
        public string horaCorte { get; set; }
        public bool comisionDescuento { get; set; }
        public int idGrupo { get; set; }
        public string nombreComercial { get; set; }
        public string rfc { get; set; }
        public string razonSocial { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoCiudad { get; set; }
        public string contacto { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public int idCatalogoFactura { get; set; }
        public string tasaIva { get; set; }
        public int idFamilia { get; set; }
        public string direccion { get; set; }
        public bool comisionista { get; set; }
        public int idMetodoPago { get; set; }
        public string cuentaPago { get; set; }
        public decimal limiteCredito { get; set; }
        public bool depositoTarjeta { get; set; }
        public bool corresponsal { get; set; }
        public bool verSaldoPos { get; set; }
        public int credito { get; set; }
        public string tipoCredito { get; set; }
        public string activoCadena { get; set; }
        public int mayorista { get; set; }
        public string cuentaAzteca { get; set; }
        public int lector { get; set; }
        public int cuentaSTP { get; set; }
        public string usoCFDI { get; set; }
    }

    public class AdministrarInformacionClienteCadena
    {
        public int id { get; set; }
        public string label { get; set; }
        public bool activo { get; set; }
        public decimal monto { get; set; }
        public decimal IDE { get; set; }
        public decimal saldoSTP { get; set; }
        public decimal comisionRecarga { get; set; }
        public string facturacion { get; set; }
        public decimal credito { get; set; }
        public string familia { get; set; }
        public string grupo { get; set; }
        public bool depositoTarjeta { get; set; }
        public string direccion { get; set; }
        public string lugar { get; set; }
        public string rfc { get; set; }
        public string periodoCredito { get; set; } = "";
    }

    public class AdministrarInformacionClienteEstablecimiento
    {
        public int id { get; set; }
        public string label { get; set; }
        public bool activo { get; set; }
    }

    public class ListadoLabel
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class AdministrarInformacionCajeros
    {
        public string id { get; set; }
        public bool administrador { get; set; }
        public string cajero { get; set; }
        public bool activo { get; set; }
        public bool statusClave { get; set; }
        public string resetClave { get; set; }
        public string usuario { get; set; }
        public string telefono { get; set; }
    }

    public class AdministrarInformacionTerminales
    {
        public string id { get; set; }
        public string terminal { get; set; }
        public string tipo { get; set; }
        public bool activo { get; set; }
        public string fechaAlta { get; set; }
    }

    public class DatosCajero
    {
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string usuario { get; set; }
        public bool administrador { get; set; }
    }
    public class usoCFDI
    {
        public int id { get; set; }
        public string clave { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
    }


    #endregion

    #region RequestModels

    public class SoporteRequestInformacionCadena : UsuarioIp
    {
        public int idCadena { get; set; }
    }

    public class SoporteRequestActivaDesactivaCadena : UsuarioIp
    {
        public int id { get; set; }
        public int activo { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestEnviaCorreoBienvenida : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
        public int idCadena { get; set; }
    }

    public class SoporteRequestNuevoEstablecimiento : UsuarioIp
    {
        public int idCadena { get; set; }
        public string nombre { get; set; }
        public int idCatalogoTipoEstablecimiento { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoCiudad { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public string encargado { get; set; }
        public int id { get; set; }
        public int tipoUsuario { get; set; }
        public string serieFolio { get; set; }
    }

    public class SoporteRequestSetEstatusEstablecimiento : UsuarioIp
    {
        public int id { get; set; }
        public string idStr { get; set; }
        public int activo { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestListadoFactura
    {
        public int id { get; set; }
        public int menu { get; set; }
    }

    public class SoporteRequestListadoMetodoPago
    {
        public int id { get; set; }
        public int menu { get; set; }
    }

    public class SoporteRequestListadoGrupos
    {
        public int id { get; set; }
        public int activo { get; set; }
        public int menu { get; set; }
    }

    public class SoporteRequestCiudad
    {
        public string codigoPostal { get; set; }
    }

    public class SoporteRequestFamiliaComercial
    {
        public int id { get; set; }
        public int menu { get; set; }
    }

    public class SoporteRequestListadoBancos
    {
        public int activo { get; set; }
        public int menu { get; set; }
        public int pos { get; set; }
    }

    public class SoporteCadenaRequestListadoCajeros : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
    }

    public class SoporteCadenaRequestListadoTerminales : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
    }
    
    public class SoporteRequestSetEstatusCajero : UsuarioIp
    {
        public int id { get; set; }
        public int activo { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestSetClaveDefinitiva : UsuarioIp
    {
        public int id { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestDetalleCajero : UsuarioIp
    {
        public int id { get; set; }
        public string idStr { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestEditaCajero : UsuarioIp
    {

        public int id { get; set; }
        public string idStr { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public bool administrador { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestNuevoCajero : UsuarioIp
    {

        public int idEstablecimiento { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string usuario { get; set; }
        public bool administrador { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteCadenaRequestDetalleEstablecimiento : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
    }
    
    public class SoporteRequestSetEstatusTerminal : UsuarioIp
    {
        public string idStr { get; set; }
        public int id { get; set; }
        public int activo { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestBajaTerminal : UsuarioIp
    {
        public int id { get; set; }
        public string idStr { get; set; }
    }

    public class SoporteRequestEditaEstablecimiento : UsuarioIp
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int idCatalogoTipoEstablecimiento { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoCiudad { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public string encargado { get; set; }
        public int actualizado { get; set; }
        public int idFamiliaModulo { get; set; }
        public int tipoUsuario { get; set; }
        public string serieFolio { get; set; }
    }

    public class SoporteRequestResetClave : UsuarioIp
    {
        public int id { get; set; }
        public string idStr { get; set; }
        public string clave { get; set; } = string.Empty;
    }

    public class SoporteRequestBloqueoLiberacion : UsuarioIp
    {
        public int opcion { get; set; }
        public int id { get; set; }
        public int idCadena { get; set; }
        public int idBanco { get; set; }
    }

    public class SoporteRequestEditarCadena : UsuarioIp
    {
        public int id { get; set; }
        public int idGrupo { get; set; }
        public string nombreComercial { get; set; }
        public string rfc { get; set; }
        public string razonSocial { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoCiudad { get; set; }
        public string contacto { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public int idCatalogoTipoFactura { get; set; }

        public int idFamilia { get; set; }
        public int tasaIVA { get; set; }
        public int comisionista { get; set; }
        public int nueva { get; set; }
        public int idMetodoPago { get; set; }
        public string cuentaPago { get; set; }
        public float limiteCredito { get; set; }
        public int depositoTarjeta { get; set; }
        public int corresponsal { get; set; }
        public int verSaldoPOS { get; set; }
        public int tipoCredito { get; set; }

        public int tipoUsuario { get; set; }
        // usuario 
        // ip

        public int inicio { get; set; } = 0;
        public int hasta { get; set; } = 0;
        public int corte { get; set; } = 0;

        public int factura { get; set; } = 0;
        public string horaC { get; set; }

        public int comisionDescuento { get; set; }
        public int mayorista { get; set; }

        // Para 8 y 6

        public int inicio2 { get; set; } = 0;
        public int hasta2 { get; set; } = 0;
        public int corte2 { get; set; } = 0;

        public int factura2 { get; set; } = 0;

        public int usoCFDI { get; set; }

    }

    #endregion

    #region ResponseModels

    public class SoporteResponseInformacionCadena : ResponseModel
    {
        public AdministrarInformacionCadena infoCadena { get; set; }
        public AdministrarInformacionCadena infoCadena2 { get; set; }
        public AdministrarInformacionClienteCadena infoCadenaLabel { get; set; }
        public List<AdministrarInformacionClienteEstablecimiento> listaEstablecimientos { get; set; }
    }

    public class SoporteResponseActivaDesactivaCadena : ResponseModel
    {
        public bool realizado { get; set; }
    }

    public class SoporteResponseEnviaCorreoBienvenida : ResponseModel
    {
        public bool enviado { get; set; }
    }

    public class SoporteResponseNuevoEstablecimiento : ResponseModel
    {
        public bool agregado { get; set; }
    }

    public class SoporteResponseSetEstausEstablecimiento : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseListadoFacturacion : ResponseModel
    {
        public List<ListadoLabel> listadoTipoFacturacion { get; set; }
    }

    public class SoporteResponseListadoMetodoPago : ResponseModel
    {
        public List<ListadoLabel> listadoMetodoPago { get; set; }
    }

    public class SoporteResponseListadoGrupos : ResponseModel
    {
        public List<ListadoLabel> listadoGrupo { get; set; }
    }

    public class SoporteResponseListadoTipoCredito : ResponseModel
    {
        public List<ListadoLabel> listadoTipoCredito { get; set; }
    }

    public class SoporteResponseListadoEstado : ResponseModel
    {
        public List<Estado> listadoEstados { get; set; }
    }

    public class SoporteResponseCiudad : ResponseModel
    {
        public Ciudad ciudad { get; set; }
    }

    public class SoporteResponseFamiliasComerciales : ResponseModel
    {
        public List<ListadoLabel> listadoFamilias { get; set; }
    }

    public class SoporteResponseListadoBanco : ResponseModel
    {
        public List<ListadoLabel> listadoBancos { get; set; }
    }

    public class SoporteResponseListadoCajeros : ResponseModel
    {
        public List<AdministrarInformacionCajeros> listadoCajeros { get; set; }
    }

    public class SoporteResponseListadoTerminales : ResponseModel
    {
        public List<AdministrarInformacionTerminales> listadoTerminales { get; set; }
    }

    public class SoporteResponseSetEstausCajero : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseDetalleCajero : ResponseModel
    {
        public DatosCajero cajero { get; set; }
    }

    public class SoporteResponseEditaCajero : ResponseModel
    {
        public bool editado { get; set; }
    }

    public class SoporteResponseNuevoCajero : ResponseModel
    {
        public bool creado { get; set; }
    }


    public class SoporteResponseInformacionCadenaDetalleEstablecimiento : ResponseModel
    {
        public InformacionClienteEstablecimientoDetalle detalleEstablecimiento { get; set; }
    }
    
    public class SoporteResponseListadoTipoEstablecimiento : ResponseModel
    {
        public List<ListadoLabel> listadoTipoEstablecimiento { get; set; }
    }

    public class SoporteResponseListadoFamiliaModulo : ResponseModel
    {
        public List<ListadoLabel> listadoFamiliaModulo { get; set; }
    }

    public class SoporteResponseSetEstatusCajero : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseSetClaveDefinitivaCajero : ResponseModel
    {
        public bool cambiado { get; set; }
    }
    
    public class SoporteResponseSetEstatusTerminal : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseBajaTerminal : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseEditarEstablecimiento : ResponseModel
    {
        public bool editado { get; set; }
    }

    public class SoporteResponseResetClave : ResponseModel
    {
        public string claveTemp { get; set; }
    }

    public class SoporteResponseBancosBloqueoLiberacion : ResponseModel
    {
        public List<ListadoLabel> listado { get; set; }
    }
    public class SoporteResponseSetBancoBloqueoLiberacion : ResponseModel
    {
        public bool realizado { get; set; }
    }
    public class SoporteResponseEditaCadena : ResponseModel
    {
        public bool editado { get; set; }
    }
    public class SoporteResponseUsoCFDI : ResponseModel
    {
        public List<usoCFDI> listado { get; set; }
    }
    #endregion

    #endregion

    #region Agregar Terminal

    #region Models

    #endregion


    #region Request Models

    public class SoporteRequestNuevaTerminal : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
        public long numero { get; set; }
        public bool appMovil { get; set; }
        public int idTipoTerminal { get; set; }
        public string version { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestTerminalesEstablecimiento : UsuarioIp
    {
        public int idCadena { get; set; }
    }

    #endregion


    #region Response Models

    public class SoporteResponseNuevaTerminal : ResponseModel
    {
        public bool agregada { get; set; }
    }

    public class SoporteResponseListadoTerminalesCadena : ResponseModel
    {
        public List<AdministrarInformacionClienteEstablecimiento> listadoEstablecimiento { get; set; }
    }


    public class SoporteResponseListadoTipoTerminales : ResponseModel
    {
        public List<ListadoLabel> listadoTipoTerminales { get; set; }
    }
    #endregion

    #endregion
    
    #region Administrar IUSA

    #region Models

    public class EstablecimientosIUSA
    {
        public int idCadena { get; set; }
        public string idEstablecimiento { get; set; }
        public string nombre { get; set; }
        public string iusa_codigoSucursal { get; set; }
        public string iusa_usuario { get; set; }
        public string iusa_clave { get; set; }
    }


    #endregion


    #region RequestModels


    public class SoporteRequestGetEstablecimientosIUSA
    {
        public int idEstablecimiento { get; set; } = 0;
        public string busqueda { get; set; }
    }

    public class SoporteRequestEditaEstablecimientoIUSA : UsuarioIp
    {
        public int id { get; set; } = 0;
        public string idEstablecimientoStr { get; set; }
        public string codigoSucursal { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
    }

    public class SoporteRequestNuevoEstablecimientoIUSA : UsuarioIp
    {
        public int idEstablecimiento { get; set; } = 0;
        public string codigoSucursal { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
    }

    public class SoporteRequestEstablecimientos : UsuarioIp
    {
        public int idCadena { get; set; }
    }   
    #endregion


    #region ResponseModels



    public class SoporteResponseEstablecimientosIUSA : ResponseModel
    {
        public List<EstablecimientosIUSA> listaEstablecimientosIUSA { get; set; }
    }

    public class SoporteResponseEditarEstablecimientoIUSA : ResponseModel
    {
        public bool editado { get; set; }
    }

    public class SoporteResponseNuevoEstablecimientoIUSA : ResponseModel
    {
        public bool creado { get; set; }
    }

    public class SoporteResponseListadoEstablecimientosCadena : ResponseModel
    {
        public List<AdministrarInformacionClienteEstablecimiento> listadoEstablecimiento { get; set; }
    }
    #endregion


    #endregion

    #region Administrar WINPOS


    #region Models

    public class EstablecimientosWinPos
    {
        public int idCadena { get; set; }
        public string idEstablecimiento { get; set; }
        public string nombre { get; set; }
        public bool iusa_activo { get; set; }
        public string iusa_codigoSucursal { get; set; }
        public string iusa_usuario { get; set; }
        public string iusa_clave { get; set; }
        public bool bancomer_activo { get; set; }
        public int bancomer_idAfiliacion { get; set; }
        public string bancomer_afiliacion { get; set; }
        public bool mit_activo { get; set; }
        public string mit_usuario { get; set; }
        public string mit_clave { get; set; }
        public int mit_idAfiliacion { get; set; }
        public string mit_merchant { get; set; }
    }


    #endregion


    #region RequestModels


    public class SoporteRequestGetEstablecimientosWinPos
    {
        public int idEstablecimiento { get; set; } = 0;
        public string busqueda { get; set; }
    }

    public class SoporteRequestNuevoEstablecimientoWinPos : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
        public bool iusa_activo { get; set; }
        public string iusa_codigoSucursal { get; set; }
        public string iusa_usuario { get; set; }
        public string iusa_clave { get; set; }
        public bool bancomer_activo { get; set; }
        public int bancomer_idAfiliacion { get; set; }
        public bool mit_activo { get; set; }
        public string mit_usuario { get; set; }
        public string mit_clave { get; set; }
        public int mit_idAfiliacion { get; set; }
    }

    public class SoporteRequestEliminaEstablecimientoWinPos : UsuarioIp
    {
        public string idEstablecimientoStr { get; set; }
        public int idEstablecimiento { get; set; }
    }

    public class SoporteRequestEditaEstablecimientoWinPos : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
        public string idEstablecimientoStr { get; set; }
        public bool iusa_activo { get; set; }
        public string iusa_codigoSucursal { get; set; }
        public string iusa_usuario { get; set; }
        public string iusa_clave { get; set; }
        public bool bancomer_activo { get; set; }
        public int bancomer_idAfiliacion { get; set; }
        public bool mit_activo { get; set; }
        public string mit_usuario { get; set; }
        public string mit_clave { get; set; }
        public int mit_idAfiliacion { get; set; }
    }

    public class SoporteRequestListadoAfiliacion
    {
        public int idCatalogoProveedor { get; set; }
        public int menu { get; set; }
    }
    #endregion


    #region ResponseModels


    public class SoporteResponseEstablecimientosWINPOS : ResponseModel
    {
        public List<EstablecimientosWinPos> listaEstablecimientosWINPOS { get; set; }
    }

    public class SoporteResponseNuevoEstablecimientoWINPOS : ResponseModel
    {
        public bool creado { get; set; }
    }

    public class SoporteResponseEditarEstablecimientoWINPOS : ResponseModel
    {
        public bool editado { get; set; }
    }

    public class SoporteResponseEliminarEstablecimientoWINPOS : ResponseModel
    {
        public bool eliminado { get; set; }
    }

    public class SoporteResponseListadoAfiliacion : ResponseModel
    {
        public List<ListadoLabel> listadoAfiliacion { get; set; }
    }
    #endregion


    #endregion

    #region Remesas Error

    #region Models

    public class RemesasError
    {
        public int idVentaError { get; set; }
        public int idCadena { get; set; }
        public string referencia1 { get; set; }
        public string referencia2 { get; set; }
        public decimal salida { get; set; }
        public string fechaHora { get; set; }
        public int codigoRespuesta { get; set; }
    }

    #endregion


    #region RequestModels

    public class SoporteRequestGetRemesasError
    {
        public int idCadena { get; set; }
        public string desde { get; set; }
        public string hasta { get; set; }
        public string referencia { get; set; }
        public float monto { get; set; }
    }

    public class SoporteRequestSubirIFERemesa
    {
        public string nombre { get; set; }
        public string ext { get; set; }
        public string mimeType { get; set; }
        public string archivo { get; set; }
        public int idVentaError { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class RemesaExitosaDetalle
    {
        public decimal saldoAnterior { get; set; }
        public decimal saldoActual { get; set; }
    }

    public class SoporteRequestRemesaExitosa : UsuarioIp
    {
        public int idRemesaError { get; set; }
    }
    public class SoporteResponseRemesaExitosa : ResponseModel
    {
        public RemesaExitosaDetalle detalle { get; set; }
    }
    #endregion


    #region ResponseModels


    public class SoporteResponseListadoRemesasError : ResponseModel
    {
        public List<RemesasError> listadoRemesasError { get; set; }
    }

    #endregion

    #endregion

    #region IP Bloqueada


    #region Models

    public class IpBloqueadas
    {
        public string ip { get; set; }
    }

    #endregion


    #region RequestModels

    public class SoporteRequestGetIpBloqueadas
    {
        public string ip { get; set; }
    }

    public class SoporteRequestEliminarIpBloqueada : UsuarioIp
    {
        public string ipEliminar { get; set; }
    }
    #endregion


    #region ResponseModels


    public class SoporteResponseListadoIpBloqueadas : ResponseModel
    {
        public List<IpBloqueadas> listadoIpBloqueadas { get; set; }
    }

    public class SoporteResponseEliminarIpBloqueada : ResponseModel
    {
        public bool eliminada { get; set; }
    }

    #endregion


    #endregion

    #region Monitor Log


    #region Models

    public class MonitorLog
    {
        public int idCadena { get; set; }
        public int idEstablecimiento { get; set; }
        public string producto { get; set; }
        public double monto { get; set; }
        public string referencia { get; set; }
        public string tipoOperacion { get; set; }
        public string aplicacion { get; set; }
        public string fechaHora { get; set; }
    }

    #endregion


    #region RequestModels

    public class SoporteRequestGetListadoMonitorLog
    {
        public int aplicacion { get; set; }
        public int tipoOperacion { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
    }

    #endregion


    #region ResponseModels


    public class SoporteResponseListadoMonitorLog : ResponseModel
    {
        public List<MonitorLog> listadoLog { get; set; }
    }

    public class SoporteResponseListadoAplicaciones : ResponseModel
    {
        public List<ListadoLabel> listadoAplicaciones { get; set; }
    }

    public class SoporteResponseListadoTipoOperacion : ResponseModel
    {
        public List<ListadoLabel> listadoTipoOperacion { get; set; }
    }
    #endregion


    #endregion

    #region Configurar Proveedores


    #region Models

    public class ProveedoresPorRegion
    {
        public string intermediario { get; set; }
        public string nombreComercial { get; set; }
        public string regiones { get; set; }
        public string grupo { get; set; }
        public string familia { get; set; }
    }

    public class SwitchTelcel
    {
        public int idFamilia { get; set; }
        public string familia { get; set; }
        public bool tn { get; set; }
        public bool celex { get; set; }
        public bool apamate { get; set; }
        public bool cencel { get; set; }
        public bool moviTelcel { get; set; }
    }

    public class SwitchMovistar
    {
        public int idFamilia { get; set; }
        public string familia { get; set; }
        public bool movilway { get; set; }
        public bool tf { get; set; }
        public bool emi { get; set; }
    }

    public class SwitchIusaUnefon
    {
        public int idFamilia { get; set; }
        public string familia { get; set; }
        public bool iusacell { get; set; }
        public bool unefon { get; set; }
        public bool emi { get; set; }
    }

    #endregion


    #region RequestModels

    #endregion


    #region ResponseModels


    public class SoporteResponseProveedoresRegion : ResponseModel
    {
        public List<ProveedoresPorRegion> listadoProveedores { get; set; }
    }

    public class SoporteResponseListadoSwitchTelcel : ResponseModel
    {
        public List<SwitchTelcel> listadoSwitchTelcel { get; set; }
    }

    public class SoporteResponseListadoSwitchMovistar : ResponseModel
    {
        public List<SwitchMovistar> listadoSwitchMovistar { get; set; }
    }

    public class SoporteResponseListadoSwitchIusaUnefon : ResponseModel
    {
        public List<SwitchIusaUnefon> listadoSwitchIusaUnefon { get; set; }
    }
    #endregion


    #endregion

    #region Cajeros Push Money


    #region Models

    public class CajerosPushMoney
    {
        public string cajero { get; set; }
        public string rfc { get; set; }
        public string idIntelisis { get; set; }
        public string llave { get; set; }
        public int idCadena { get; set; }
        public int idEstablecimiento { get; set; }
        public string establecimiento { get; set; }
        public string fechaAlta { get; set; }
    }

    #endregion


    #region RequestModels

    public class SoporteRequestGetListadoPushMoney
    {
        public string nombre { get; set; }
        public string rfc { get; set; }
        public string idIntelisis { get; set; }
    }


    public class SoporteRequestEditarEstablecimientoPushMoney : UsuarioIp
    {
        public string llave { get; set; }
        public int idEstablecimiento { get; set; }
    }


    #endregion


    #region ResponseModels

    public class SoporteResponseListadoPushMoney : ResponseModel
    {
        public List<CajerosPushMoney> listadoPushMoney { get; set; }
    }

    public class SoporteResponseEditarEstablecimientoPushMoney : ResponseModel
    {
        public bool editado { get; set; }
    }

    #endregion


    #endregion

    #region Administrar Establecimiento
    #region Modelos

    public class AdministrarInformacionCadenaBnext
    {
        public int inicioCredito { get; set; }
        public int finCredito { get; set; }
        public string fechaMod { get; set; }
        public int corte { get; set; }
        public int factura { get; set; }
        public string horaCorte { get; set; }
        public bool comisionDescuento { get; set; }
        public int idGrupo { get; set; }
        public string nombreComercial { get; set; }
        public string rfc { get; set; }
        public string razonSocial { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoCiudad { get; set; }
        public string contacto { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public int idCatalogoFactura { get; set; }
        public string tasaIva { get; set; }
        public int idFamilia { get; set; }
        public string direccion { get; set; }
        public bool comisionista { get; set; }
        public int idMetodoPago { get; set; }
        public string cuentaPago { get; set; }
        public decimal limiteCredito { get; set; }
        public bool depositoTarjeta { get; set; }
        public bool corresponsal { get; set; }
        public bool verSaldoPos { get; set; }
        public int credito { get; set; }
        public string tipoCredito { get; set; }
        public string activoCadena { get; set; }
        public int mayorista { get; set; }
    }

    public class AdministrarInformacionClienteCadenaBnext
    {
        public int id { get; set; }
        public string label { get; set; }
        public bool activo { get; set; }
        public decimal monto { get; set; }
        public decimal IDE { get; set; }
        public decimal comisionRecarga { get; set; }
        public string facturacion { get; set; }
        public decimal credito { get; set; }
        public string familia { get; set; }
        public string grupo { get; set; }
        public bool depositoTarjeta { get; set; }
        public string direccion { get; set; }
        public string lugar { get; set; }
        public string rfc { get; set; }
    }

    public class AdministrarInformacionClienteEstablecimientoBnext
    {
        public int id { get; set; }
        public string label { get; set; }
        public string monto { get; set; }
        public bool activo { get; set; }
    }

    public class ListadoLabelBnext
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class AdministrarInformacionCajerosBnext
    {
        public string id { get; set; }
        public bool administrador { get; set; }
        public string cajero { get; set; }
        public bool activo { get; set; }
        public bool statusClave { get; set; }
        public string resetClave { get; set; }
        public string usuario { get; set; }
        public string telefono { get; set; }
    }

    public class AdministrarInformacionTerminalesBnext
    {
        public string id { get; set; }
        public string terminal { get; set; }
        public string tipo { get; set; }
        public bool activo { get; set; }
        public string fechaAlta { get; set; }
    }

    public class ClaveDefinitivaCajeroBnext
    {
        public int codigo { get; set; }
        public string respuesta { get; set; }
        public string telefono { get; set; }
    }

    public class DatosCajeroBnext
    {
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string usuario { get; set; }
        public bool administrador { get; set; }
    }

    #endregion

    #region RequestModels

    public class SoporteRequestInformacionCadenaBnext : UsuarioIp
    {
        public string idEstablecimiento { get; set; }
    }

    public class SoporteRequestActivaDesactivaCadenaBnext : UsuarioIp
    {
        public int id { get; set; }
        public int activo { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestEnviaCorreoBienvenidaBnext : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
        public int idCadena { get; set; }
    }

    public class SoporteRequestNuevoEstablecimientoBnext : UsuarioIp
    {
        public int idCadena { get; set; }
        public string nombre { get; set; }
        public int idCatalogoTipoEstablecimiento { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoCiudad { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public string encargado { get; set; }
        public int id { get; set; }
        public int tipoUsuario { get; set; }
        public string serieFolio { get; set; }
    }

    public class SoporteRequestSetEstatusEstablecimientoBnext : UsuarioIp
    {
        public int id { get; set; }
        public int activo { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestListadoFacturaBnext
    {
        public int id { get; set; }
        public int menu { get; set; }
    }

    public class SoporteRequestListadoMetodoPagoBnext
    {
        public int id { get; set; }
        public int menu { get; set; }
    }

    public class SoporteRequestListadoGruposBnext
    {
        public int id { get; set; }
        public int activo { get; set; }
        public int menu { get; set; }
    }

    public class SoporteRequestCiudadBnext
    {
        public string codigoPostal { get; set; }
    }

    public class SoporteRequestFamiliaComercialBnext
    {
        public int id { get; set; }
        public int menu { get; set; }
    }

    public class SoporteRequestListadoBancosBnext
    {
        public int activo { get; set; }
        public int menu { get; set; }
        public int pos { get; set; }
    }

    public class SoporteCadenaRequestListadoCajerosBnext : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
    }

    public class SoporteCadenaRequestListadoTerminalesBnext : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
    }

    public class SoporteCadenaRequestDetalleEstablecimientoBnext : UsuarioIp
    {
        public int idEstablecimiento { get; set; }
    }

    public class SoporteRequestSetEstatusCajeroBnext : UsuarioIp
    {
        public int id { get; set; }
        public int activo { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestSetClaveDefinitivaBnext : UsuarioIp
    {
        public int id { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestDetalleCajeroBnext : UsuarioIp
    {
        public int id { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestEditaCajeroBnext : UsuarioIp
    {

        public int id { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public bool administrador { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestSetEstatusTerminalBnext : UsuarioIp
    {
        public int id { get; set; }
        public int activo { get; set; }
        public int tipoUsuario { get; set; }
    }

    public class SoporteRequestBajaTerminalBnext : UsuarioIp
    {
        public int id { get; set; }
    }

    public class SoporteRequestEditaEstablecimientoBnext : UsuarioIp
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int idCatalogoTipoEstablecimiento { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoCiudad { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public string encargado { get; set; }
        public int actualizado { get; set; }
        public int idFamiliaModulo { get; set; }
        public int tipoUsuario { get; set; }
        public string serieFolio { get; set; }
    }

    #endregion

    #region ResponseModels

    public class SoporteResponseInformacionCadenaBnext : ResponseModel
    {
        public AdministrarInformacionCadenaBnext infoCadena { get; set; }
        public AdministrarInformacionClienteCadenaBnext infoCadenaLabel { get; set; }
        public List<AdministrarInformacionClienteEstablecimientoBnext> listaEstablecimientos { get; set; }
    }

    public class SoporteResponseActivaDesactivaCadenaBnext : ResponseModel
    {
        public bool realizado { get; set; }
    }

    public class SoporteResponseEnviaCorreoBienvenidaBnext : ResponseModel
    {
        public bool enviado { get; set; }
    }

    public class SoporteResponseListadoTipoEstablecimientoBnext : ResponseModel
    {
        public List<ListadoLabelBnext> listadoTipoEstablecimiento { get; set; }
    }

    public class SoporteResponseListadoFamiliaModuloBnext : ResponseModel
    {
        public List<ListadoLabelBnext> listadoFamiliaModulo { get; set; }
    }

    public class SoporteResponseNuevoEstablecimientoBnext : ResponseModel
    {
        public bool agregado { get; set; }
    }

    public class SoporteResponseSetEstausEstablecimientoBnext : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseListadoFacturacionBnext : ResponseModel
    {
        public List<ListadoLabelBnext> listadoTipoFacturacion { get; set; }
    }

    public class SoporteResponseListadoMetodoPagoBnext : ResponseModel
    {
        public List<ListadoLabelBnext> listadoMetodoPago { get; set; }
    }

    public class SoporteResponseListadoGruposBnext : ResponseModel
    {
        public List<ListadoLabelBnext> listadoGrupo { get; set; }
    }

    public class SoporteResponseListadoTipoCreditoBnext : ResponseModel
    {
        public List<ListadoLabelBnext> listadoTipoCredito { get; set; }
    }

    public class SoporteResponseListadoEstadoBnext : ResponseModel
    {
        public List<Estado> listadoEstados { get; set; }
    }

    public class SoporteResponseCiudadBnext : ResponseModel
    {
        public Ciudad ciudad { get; set; }
    }

    public class SoporteResponseFamiliasComercialesBnext : ResponseModel
    {
        public List<ListadoLabelBnext> listadoFamilias { get; set; }
    }

    public class SoporteResponseListadoBancoBnext : ResponseModel
    {
        public List<ListadoLabelBnext> listadoBancos { get; set; }
    }

    public class SoporteResponseListadoCajerosBnext : ResponseModel
    {
        public List<AdministrarInformacionCajerosBnext> listadoCajeros { get; set; }
    }

    public class SoporteResponseListadoTerminalesBnext : ResponseModel
    {
        public List<AdministrarInformacionTerminalesBnext> listadoTerminales { get; set; }
    }

    public class SoporteResponseSetEstausCajeroBnext : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseDetalleCajeroBnext : ResponseModel
    {
        public DatosCajeroBnext cajero { get; set; }
    }

    public class SoporteResponseEditaCajeroBnext : ResponseModel
    {
        public bool editado { get; set; }
    }

    public class SoporteResponseInformacionCadenaDetalleEstablecimientoBnext : ResponseModel
    {
        public InformacionClienteEstablecimientoDetalle detalleEstablecimiento { get; set; }
    }

    public class SoporteResponseSetEstatusCajeroBnext : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseSetClaveDefinitivaCajeroBnext : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseSetEstatusTerminalBnext : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    public class SoporteResponseBajaTerminalBnext : ResponseModel
    {
        public bool cambiado { get; set; }
    }
    public class SoporteResponseEditarEstablecimientoBnext : ResponseModel
    {
        public bool editado { get; set; }
    }
    #endregion
    #endregion

    #region Ticket Establecimiento

    #region model
    public class HistorialTickets
    {
        public string ticketID { get; set; }
        public string tipoTicket { get; set; }
        public string asunto { get; set; }
        public string estatus { get; set; }
        public string medio { get; set; }
        public string atendio { get; set; }
        public string fechaAlta { get; set; }
        public string fechaVencimiento { get; set; }
        public string tema { get; set; }
        public string nombre { get; set; }

    }

    #endregion

    #region RequestModels
    public class SoporteRequestHistorial
    {
        public string idCadena { get; set; }
        public string idEstablecimiento { get; set; }
    }
    #endregion

    #region ResponseModels
    public class SoporteResponseHistorialTickets : ResponseModel
    {
        public List<HistorialTickets> listadoHistorial { get; set; }
    }
    #endregion

    #endregion
}
