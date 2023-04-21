using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class LiberacionesModel
    {
    }

    #region Listado Depositos

    #region Modelos

    public class Depositos
    {
        public string Id { get; set; }
        public string FormaDePago { get; set; }
        public string IdCatalogoCuenta { get; set; }
        public string Banco { get; set; }
        public string FechaDeposito { get; set; }
        public int IdCadena { get; set; }
        public int IdCatalogoEstatusDeposito { get; set; }
        public decimal Monto { get; set; }
        public string HoraDeposito { get; set; }
    }

    public class DetalleDeposito
    {
        public string Id { get; set; }
        public string RazonSocial { get; set; }
        public string Referencia { get; set; }
        public string Referencia2 { get; set; }
        public string Banco { get; set; }
        public string NumeroCuenta { get; set; }
        public string FormaDePago { get; set; }
        public double Monto { get; set; }
        public string FechaDeposito { get; set; }
        public string HoraDeposito { get; set; }
        public int IdCadena { get; set; }
        public string Observaciones { get; set; }
        public string IdCatalogoCuenta { get; set; }
        public string LbRefenrencia2 { get; set; }
        public int IdCatalogoEstatusDeposito { get; set; }
        public string Direccion { get; set; }
        public string Ficha { get; set; }
        public string Telefono1 { get; set; }
    }

    public class DepositosCuenta
    {
        public string id { get; set; }
        public string numeroAutorizacion { get; set; }
        public string referencia { get; set; }
        public string descripciones { get; set; }
        public double monto { get; set; }
        public string nombre { get; set; }
        public string fecha { get; set; }
        public string horaAlta { get; set; }
    }


    #endregion

    #region RequestModels


    public class LiberacionesRequestListadoDepositos
    {

        public int Usuario { get; set; }
        public int Rol { get; set; }
        public bool Bloqueados { get; set; }
        public string Desde { get; set; } = string.Empty;
        public string Hasta { get; set; } = string.Empty;
        public string Ejecutivo { get; set; } = string.Empty;
        public string Mtcid { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public string Banco { get; set; } = string.Empty;
    }

    public class LiberacionesRequestDetalleDepositos
    {
        public string IdDepositoStr { get; set; } = string.Empty;
        public int IdUsuario { get; set; } = 0;
        public int IdDeposito { get; set; }
    }

    public class LiberacionesRequestLiberacionAutomatica
    {
        public string idCuentaStr { get; set; }
        public int idCuenta { get; set; }
    }

    public class LiberacionesRequestDepositosCuenta
    {
        public string idDepositoStr { get; set; } = string.Empty;
        public int idDeposito { get; set; }
    }

    public class LiberacionesRequestAutorizaDeposito
    {
        public string idDepositoStr { get; set; } = string.Empty;
        public int idDeposito { get; set; }
        public int idUsuario { get; set; }
        public string observaciones { get; set; }
        public string idDepositoCuentaStr { get; set; } = string.Empty;
        public int idDepositoCuenta { get; set; }
    }

    public class LiberacionesRequestRechazaDeposito
    {
        public string idDepositoStr { get; set; }
        public int idDeposito { get; set; }
        public int idUsuario { get; set; }
        public string observaciones { get; set; }
    }


    #endregion

    #region ResponseModels

    public class LiberacionesResponseListadoDepositos : ResponseModel
    {
        public List<Depositos> listaDepositos { get; set; }
    }

    public class LiberacionesResponseDetalleDepositos : ResponseModel
    {
        public DetalleDeposito DetalleDeposito { get; set; }
        public int TotalDepositos { get; set; }
    }

    public class LiberacionesResponseLiberacionAutomatica : ResponseModel
    {
        public bool automatica { get; set; }
    }

    public class LiberacionesResponseDepositosCuenta : ResponseModel
    {
        public List<DepositosCuenta> listaDepositosCuenta { get; set; }
    }

    public class LiberacionesResponseAutorizaDeposito : ResponseModel
    {
        public bool autorizado { get; set; }
    }
    
    public class LiberacionesResponseRechazaDeposito : ResponseModel
    {
        public bool rechazado { get; set; }
    }

    #endregion

    #endregion

    #region Depositos Bloqueados

    #region Modelos

    public class DepositosBloqueados
    {
        public string id { get; set; }
        public string formaDePago { get; set; }
        public int idCatalogoCuenta { get; set; }
        public string banco { get; set; }
        public string fechaDeposito { get; set; }
        public int idCadena { get; set; }
        public int idCatalogoEstatusDeposito { get; set; }
        public decimal monto { get; set; }
        public string ejecutivo { get; set; } = "";
        public bool edit { get; set; } = false;
    }

    public class FormasPago
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class Cuentas
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    #endregion


    #region RequestModels


    public class LiberacionesRequestListadoDepositosBloqueados
    {

        public int usuario { get; set; }
        public int rol { get; set; }
        public bool bloqueados { get; set; }
        public string desde { get; set; }
        public string hasta { get; set; }
        public string ejecutivo { get; set; }
        public string mtcid { get; set; }
        public int formaPago { get; set; } = 0;
        public int banco { get; set; } = 0;
    }

    public class LiberacionesRequestListadoDepositosDesbloquea
    {
        public string idStr { get; set; }
        public int id { get; set; }
        public int rol { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestListadoDepositosEdita
    {
        public string idStr { get; set; }
        public int id { get; set; }
        public int idCatalogoCuenta { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }

    }


    #endregion


    #region ResponseModels


    public class LiberacionesResponseListadoDepositosBloqueados : ResponseModel
    {
        public List<DepositosBloqueados> listaDepositosBloqueados { get; set; }
    }

    public class LiberacionesResponseListadoDepositosBloqueadosFormasPago : ResponseModel
    {
        public List<FormasPago> listaFormasPago { get; set; }
    }

    public class LiberacionesResponseListadoDepositosBloqueadosCuentas : ResponseModel
    {
        public List<Cuentas> listadoCuentas { get; set; }
    }

    public class LiberacionesResponseListadoDepositosBloqueadosDesbloquea : ResponseModel
    {
        public bool desbloqueado { get; set; }
    }

    public class LiberacionesResponseListadoDepositosBloqueadosEdita : ResponseModel
    {
        public bool editado { get; set; }
    }


    #endregion


    #endregion

    #region Nuevo Depósito

    #region Modelos

    public class InformacionCadenaDeposito
    {
        public string label { get; set; }
        public decimal monto { get; set; }
    }

    public class DatosNotaFiscal
    {
        public string rfc { get; set; }
        public string razonSocial { get; set; }
        public string formaPago { get; set; }
        public string metodoPago { get; set; }
        public string usoCFDI { get; set; }
        public string cantidad { get; set; }
        public string claveUnidad { get; set; }
        public string claveProd { get; set; }
        public string descripcion { get; set; }
    }

    #endregion

    #region RequestModels

    public class LiberacionesRequestInformacionCadena
    {
        public int idCadena { get; set; }
        public int idUsuario { get; set; }
    }

    public class LiberacionesRequesrNuevoDepositoManual
    {
        public int idUsuario { get; set; } = 0;
        public int idCadena { get; set; }
        public int idCatalogoCuenta { get; set; }
        public DateTime fechaDeposito { get; set; }
        public int idCatalogoFormaPago { get; set; }
        public string referencia { get; set; }
        public string referencia2 { get; set; }
        public decimal monto { get; set; }
        public string observaciones { get; set; }
        public bool publico { get; set; }
        public bool comision { get; set; }
        public bool fiscal { get; set; }
        public DateTime horaDeposito { get; set; }
        public string folioFiscal { get; set; }
    }

    #endregion

    #region ResponseModels

    public class LiberacionesResponseInformacionCadenaDeposito : ResponseModel
    {
        public InformacionCadenaDeposito informacionCadena { get; set; }
    }

    public class LiberacionesResponseNuevoDeposito : ResponseModel
    {
        public bool registrado { get; set; }
        public List<DepositosEstatus> listadoEstatusDepositos { get; set; }
    }
    
    public class LiberacionesResponseDatosNotaFiscal : ResponseModel
    {
        public DatosNotaFiscal datos { get; set; }
    }

    #endregion

    #endregion

    #region Liberacion Bancos

    #region Modelos
    public class elemento
    {
        public string nombre { get; set; }
        public string codigo { get; set; } = "";
        public string FormaPago { get; set; }
        public int idCatalogo { get; set; }
    }
    public class cuenta:ResponseModel
    {
        public int idBanco { get; set; }
    }
    public class listaElemento
    {
        public List<elemento> listaElementos { get; set; }
    }
    public class conceptosBancas
    {
        public string nombre { get; set; }
    }
    #endregion


    #region RequestModels


    public class LiberacionesRequestSubirLiberacionBanco
    {
        public string nombre { get; set; }
        public string ext { get; set; }
        public string mimeType { get; set; }
        public string archivo { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
        public cuenta idBanco { get; set; } = new cuenta();
    }

    public class LiberacionesRequestSetConceptoBancas
    {
        public string nombre { get; set; }
        public string codigo { get; set; }
        public string formaPago { get; set; }
        public int idBanco { get; set; }
    }
    #endregion


    #region ResponseModels

    public class LiberacionesResponseSubirLiberacionDeposito : ResponseModel
    {
        public bool insertado { get; set; }
    }
    public class LiberacionesResponseConceptosBancos : ResponseModel
    {
        public List<conceptosBancas> listaConceptos { get; set; }
    }
    #endregion
    #endregion

    #region Subir Depositos


    #region Modelos

    #endregion


    #region RequestModels

    public class LiberacionesRequestSubirDepositos
    {
        public string nombre { get; set; }
        public string ext { get; set; }
        public string mimeType { get; set; }
        public string archivo { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }


    #endregion


    #region ResponseModels

    public class LiberacionesResponseSubirDepositos : ResponseModel
    {
        public bool insertado { get; set; }
    }

    #endregion

    #endregion

    #region Información de clientes

    #region Modelos

    public class InformacionCadena
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
    }

    public class InformacionClienteCadena
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

    public class InformacionClienteEstablecimiento
    {
        public int id { get; set; }
        public string label { get; set; }
        public bool activo { get; set; }
    }

    public class InformacionTerminales
    {
        public string terminal { get; set; }
        public string tipo { get; set; }
        public bool activo { get; set; }
        public string fechaAlta { get; set; }
    }

    public class InformacionClienteEstablecimientoDetalle
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string serieFolio { get; set; }
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
        public bool actualizado { get; set; }
        public int idFamiliaModulo { get; set; }
        public string corresponsalBNX { get; set; }
        public string direccion { get; set; }
        public string nombreComercial { get; set; }
        public int idCadena { get; set; }
    }


    #endregion

    #region RequestModels



    public class LiberacionesCadenaRequestInformacionCadena
    {
        public int idCadena { get; set; }
        public int tipoUsuario { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }


    public class InformacionCajeros
    {
        public string cajero { get; set; }
        public bool activo { get; set; }
    }
    
    public class LiberacionesCadenaRequestListadoCajeros
    {
        public int idEstablecimiento { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesCadenaRequestListadoTerminales
    {
        public int idEstablecimiento { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesCadenaRequestDetalleEstablecimiento
    {
        public int idEstablecimiento { get; set; }
    }


    #endregion


    #region ResponseModels 

    public class LiberacionesResponseInformacionCadena : ResponseModel
    {
        public InformacionCadena infoCadena { get; set; }
        public InformacionClienteCadena infoCadenaLabel { get; set; }
        public List<InformacionClienteEstablecimiento> listaEstablecimientos { get; set; }
    }

    public class LiberacionesResponseInformacionCadenaListadoCajeros : ResponseModel
    {
        public List<InformacionCajeros> listaCajeros { get; set; }
    }

    public class LiberacionesResponseInformacionCadenaListadoTerminales : ResponseModel
    {
        public List<InformacionTerminales> listaTerminales { get; set; }
    }

    public class LiberacionesResponseInformacionCadenaDetalleEstablecimiento : ResponseModel
    {
        public InformacionClienteEstablecimientoDetalle detalleEstablecimiento { get; set; }
    }
    #endregion


    #endregion
    
    #region Saldo a credito

    #region Modelos

    public class CadenasCredito
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class SaldoCadena
    {
        public int id { get; set; }
        public decimal monto { get; set; }
        public decimal credito { get; set; }
        public decimal limiteCredito { get; set; }
    }

    public class LiberacionesRequestTransfiereSaldoCredito
    {
        public int idCadena { get; set; }
        public float monto { get; set; }
        public int tipoUsuario { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    #endregion


    #region RequestModels


    public class LiberacionesRequestSaldoCadena
    {
        public int idCadena { get; set; }
    }

    #endregion


    #region ResponseModels


    public class LiberacionesResponseListadoCadenasCredito : ResponseModel
    {
        public List<CadenasCredito> listaCadenasCredito { get; set; }
    }


    public class LiberacionesResponseSaldoCadena : ResponseModel
    {
        public SaldoCadena saldoCadena { get; set; }
    }

    public class LiberacionesResponseTraspasoSaldoCredito : ResponseModel
    {
        public bool traspasado { get; set; }
    }

    #endregion

    #endregion

    #region Estatus Depositos

    #region Modelos

    public class DepositosEstatus
    {
        public string id { get; set; }
        public string formaDePago { get; set; }
        public int idEstablecimiento { get; set; }
        public string banco { get; set; }
        public string fechaDeposito { get; set; }
        public string horaDeposito { get; set; }
        public int idCadena { get; set; }
        public string status { get; set; }
        public double monto { get; set; }
        public double comision { get; set; }
        public string referencia { get; set; }
        public string referencia2 { get; set; }
        public string nombreUsuario { get; set; }
        public string horaLiberacion { get; set; }
        public string horaSolicitud { get; set; }
        public string observaciones { get; set; }
        public string ficha { get; set; }
        public int idCatalogoEstatusDeposito { get; set; }
        public bool reverso { get; set; }
    }

    public class EstatusDeposito
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class EstatusDetalleDeposito
    {
        public string id { get; set; }
        public int idCadena { get; set; }
        public int idCajero { get; set; }
        public int idTerminal { get; set; }
        public int idCatalogoCuenta { get; set; }
        public string cuenta { get; set; }
        public int idCatalogoFormaPago { get; set; }
        public string referencia { get; set; }
        public string referencia2 { get; set; }
        public string fechaDeposito { get; set; }
        public string horaDeposito { get; set; }
        public decimal monto { get; set; }
        public decimal comision { get; set; }
        public string observaciones { get; set; }
        public string horaSolicitud { get; set; }
        public string horaLiberacion { get; set; }
        public string usuario { get; set; }
        public string estatus { get; set; }
        public bool publicoGeneral { get; set; }
        public bool reverso { get; set; }
    }
    #endregion


    #region RequestModels

    public class LiberacionesRequestListadoEstatusDepositos
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string idCadena { get; set; }
        public int idEstatus { get; set; }
        public string usuario { get; set; }
        public string monto { get; set; }
        public string autorizacion { get; set; }
        public int idCatalogoCuenta { get; set; }
        public int idCatalogoFormaPago { get; set; }
        public int idUsuario { get; set; } = 0;
        public string ip { get; set; } = string.Empty;
    }

    public class LiberacionesRequestDetalleDeposito
    {
        public int idDeposito { get; set; }
        public string idDepositoStr { get; set; }
    }

    public class LiberacionesRequestEstatusEditaDeposito
    {
        public int idDeposito { get; set; }
        public string idDepositoStr { get; set; }
        public int idCatalogoCuenta { get; set; }
        public int idCatalogoFormaPago { get; set; }
        public string fechaDeposito { get; set; }
        public string horaDeposito { get; set; }
        public string referencia2 { get; set; }
        public string observaciones { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }
    #endregion


    #region ResponseModels


    public class LiberacionesResponseListadoEstatusDepositos : ResponseModel
    {
        public List<DepositosEstatus> listadoEstatusDepositos { get; set; }
    }

    public class LiberacionesResponseListadoEstatusDepositosFormasPago : ResponseModel
    {
        public List<FormasPago> listaFormasPago { get; set; }
    }

    public class LiberacionesResponseListadoEstatusDepositosCuentas : ResponseModel
    {
        public List<Cuentas> listadoCuentas { get; set; }
    }

    public class LiberacionesResponseListadoEstatus : ResponseModel
    {
        public List<EstatusDeposito> listadoEstatus { get; set; }
    }

    public class LiberacionesResponseDetalleDeposito : ResponseModel
    {
        public EstatusDetalleDeposito detalleDeposito { get; set; }
    }

    public class LiberacionesResponseEstatusEditDeposito : ResponseModel
    {
        public bool editado { get; set; }
    }

    public class LiberacionesResponseReporteEstatusDepositoExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }
    #endregion

    #endregion

    #region Bloquear Bancos

    #region Modelos

    public class BloqueaBancoLabel
    {
        public string id { get; set; }
        public string label { get; set; }
        public bool bloqueado { get; set; }
        public bool fichaDeposito { get; set; }
    }

    public class LiberacionAutomaticaLabel
    {
        public string id { get; set; }
        public string banco { get; set; }
        public string numeroCuenta { get; set; }
        public bool liberacionAutomatica { get; set; }
        public string label { get; set; }
    }

    #endregion


    #region RequestModels

    public class LiberacionesRequestBloqueaBanco
    {
        public int idDeposito { get; set; }
        public string idDepositoStr { get; set; }
        public bool bloqueado { get; set; }
    }

    public class LiberacionesRequestSolicitudFichaDeposito
    {
        public int idDeposito { get; set; }
        public string idDepositoStr { get; set; }
        public bool fichaDeposito { get; set; }
    }

    public class LiberacionesRequestBloqueaLiberacionAutomatica
    {
        public int idDeposito { get; set; }
        public string idDepositoStr { get; set; }
        public bool estatus { get; set; }
    }

    #endregion


    #region ResponseModels

    public class LiberacionesResponseListadoBancosLiberacion : ResponseModel
    {
        public List<BloqueaBancoLabel> listadoBancos { get; set; }
        public List<LiberacionAutomaticaLabel> listadoLiberaciones { get; set; }
    }

    public class LiberacionesResponseBloqueaBanco : ResponseModel
    {
        public bool realizado { get; set; }
    }

    public class LiberacionesResponsesolicitudFichaDeposito : ResponseModel
    {
        public bool realizado { get; set; }
    }

    public class LiberacionesResponsebloqueaLiberacionAutomatica : ResponseModel
    {
        public bool realizado { get; set; }
    }

    #endregion

    #endregion

    #region Relacionar Depositos

    #region Modelos

    public class DepositosError
    {
        public string id { get; set; }
        public string formaPago { get; set; }
        public string banco { get; set; }
        public string fechaDeposito { get; set; }
        public int idCadena { get; set; }
        public int idCatalogoEstatusDeposito { get; set; }
    }

    #endregion


    #region RequestModels


    public class LiberacionesRequestCuentaDepositoError
    {
        public int idCuenta { get; set; }
    }

    public class LiberacionesRequestRelacionaDeposito
    {
        public string idDepositoStr { get; set; }
        public int idDeposito { get; set; }
        public int idDepositoCuenta { get; set; }
    }

    public class LiberacionesRequestConciliaDepositosNoRelacionados
    {
        public int idUsuario { get; set; }
        public int idCuenta { get; set; }
        public string ip { get; set; }
    }
    #endregion


    #region ResponseModels


    public class LiberacionesResponseListadoDepositosError : ResponseModel
    {
        public List<DepositosError> listadoDepositosError { get; set; }
    }

    public class LiberacionesResponseListadoDepositosErrorCuentas : ResponseModel
    {
        public List<Cuentas> listadoCuentas { get; set; }
    }

    public class LiberacionesResponseRelacionaDeposito : ResponseModel
    {
        public bool relacionado { get; set; }
    }

    public class LiberacionesResponseRelacionaConciliaDepositosNoRelacionados : ResponseModel
    {
        public bool conciliados { get; set; }
    }
    #endregion

    #endregion

    #region Depositos no relacionados

    #region Modelos

    public class DepositosNoRelacionados
    {
        public string id { get; set; }
        public string banco { get; set; }
        public string cuenta { get; set; }
        public string numeroAutorizacion { get; set; }
        public string referencia { get; set; }
        public int sucursal { get; set; }
        public decimal monto { get; set; }
        public string fechaHoraAlta { get; set; }
        public string formaPago { get; set; }
        public string descripciones { get; set; }
        public string referenciaContabilidad { get; set; }
        public string descripcion { get; set; }
        public int idTipoDeposito { get; set; }
        public string facturaSAT { get; set; }
    }

    public class TipoDeposito
    {
        public int id { get; set; }
        public string label { get; set; }
    }
    #endregion


    #region RequestModels


    public class LiberacionesRequestDepositosNoRelacionados
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string tipoDeposito { get; set; }
        public string idCatalogoCuenta { get; set; }
        public string monto { get; set; }
        public string referencia2 { get; set; }
        public string referencia { get; set; }
        public string idCatalogoFormaPago { get; set; }
    }

    public class LiberacionesRequestSetContabilidadDepositoBanco
    {
        public int id { get; set; }
        public string idStr { get; set; }
        public int estatus { get; set; }
        public string observaciones { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestSetDepositoFolio
    {
        public int id { get; set; }
        public string idStr { get; set; }
        public string folio { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    #endregion


    #region ResponseModels

    public class LiberacionesResponseListadoDepositosNoRelacionados : ResponseModel
    {
        public List<DepositosNoRelacionados> listadoDepositosNoRelacionados { get; set; }
    }

    public class LiberacionesResponseListadoNoRelacionadosCuentas : ResponseModel
    {
        public List<Cuentas> listadoCuentas { get; set; }
    }

    public class LiberacionesResponseListadoTipoDepositos : ResponseModel
    {
        public List<TipoDeposito> listadoTipoDepositos { get; set; }
    }

    public class LiberacionesResponseListadoNoRelacionadosFormasPago : ResponseModel
    {
        public List<FormasPago> listaFormasPago { get; set; }
    }

    public class LiberacionesResponseReporteNoRelacionadosExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }

    public class LiberacionesResponseSetContabilidadDepositoBanco : ResponseModel
    {
        public bool editado { get; set; }
    }
    #endregion

    #endregion

    #region Reverso Deposito

    #region Modelos

    public class ReversoDeposito
    {
        public string id { get; set; }
        public string formaPago { get; set; }
        public string banco { get; set; }
        public string fechaDeposito { get; set; }
        public string horaDeposito { get; set; }
        public int idCadena { get; set; }
        public string status { get; set; }
        public decimal monto { get; set; }
        public decimal comision { get; set; }
        public string referencia { get; set; }
        public string referencia2 { get; set; }
        public string nombreUsuario { get; set; }
        public string horaLiberacion { get; set; }
        public string observaciones { get; set; }
        public int idCatalogoEstatusDeposito { get; set; }
        public bool reverso { get; set; }
    }

    #endregion


    #region RequestModels


    public class LiberacionesRequestReversoDeposito
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
        public int idEstatus { get; set; }
        public string usuario { get; set; }
        public string monto { get; set; }
        public string autorizacion { get; set; }
        public int idCatalogoCuenta { get; set; } = 0;
        public int idCatalogoFormaPago { get; set; } = 0;
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestReversar
    {
        public string idDepositoStr { get; set; }
        public int idDeposito { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }


    #endregion


    #region ResponseModels

    public class LiberacionesResponseListadoReversoDepositos : ResponseModel
    {
        public List<ReversoDeposito> listadoReversoDepositos { get; set; }
    }

    public class LiberacionesResponseListadoEstatusDeposito : ResponseModel
    {
        public List<EstatusDeposito> listadoEstatusDeposito { get; set; }
    }

    public class LiberacionesResponseReverso : ResponseModel
    {
        public bool reversado { get; set; }
    }
    #endregion

    #endregion

    #region Reporte Liberacion Automatica

    #region Modelos

    public class DepositosLiberacionAutomatica
    {
        public int id { get; set; }
        public string nombreComercial { get; set; }
        public string banco { get; set; }
        public decimal monto { get; set; }
        public decimal comision { get; set; }
        public string fecha { get; set; }
        public string numeroAutorizacion { get; set; }
        public string referencia { get; set; }
        public string referencia2 { get; set; }
        public string descripciones { get; set; }
        public string codigoDeposito { get; set; }
        public string sucursal { get; set; }
        public string formaPago { get; set; }
    }

    #endregion


    #region RequestModels


    public class LiberacionesRequestDepositosLiberacionAutomatica
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
        public string monto { get; set; }
        public string referencia2 { get; set; }
    }


    #endregion


    #region ResponseModels

    public class LiberacionesResponseListadoDepositosLiberacionAutomatica : ResponseModel
    {
        public List<DepositosLiberacionAutomatica> listadoDepositosLiberacionAutomatica { get; set; }
    }

    #endregion

    #endregion

    #region Depositos Clientes

    #region Modelos

    public class DepositoCliente
    {
        public string id { get; set; }
        public string nombreUsuario { get; set; }
        public int idCadena { get; set; }
        public string nombreComercial { get; set; }
        public string banco { get; set; }
        public decimal monto { get; set; }
        public decimal comision { get; set; }
        public string fechaDeposito { get; set; }
        public string horaDeposito { get; set; }
        public string horaLiberacion { get; set; }
        public string referencia2 { get; set; }
        public string formaPago { get; set; }
    }

    #endregion


    #region RequestModels


    public class LiberacionesRequestDepositosCliente
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
        public string monto { get; set; }
        public string referencia2 { get; set; }
        public int idCatalogoCuenta { get; set; }
    }


    #endregion


    #region ResponseModels

    public class LiberacionesResponseListadoDepositosCliente : ResponseModel
    {
        public List<DepositoCliente> listadoDepositosCliente { get; set; }
    }

    public class LiberacionesResponseDepositosClienteListadoCuenta : ResponseModel
    {
        public List<Cuentas> listadoCuentas { get; set; }
    }

    public class LiberacionesResponseDepositosClienteFormasPago : ResponseModel
    {
        public List<FormasPago> listaFormasPago { get; set; }
    }

    #endregion

    #endregion

    #region Depositos Banca

    #region Modelos

    public class DepositoBanca
    {
        public string id { get; set; }
        public string banco { get; set; }
        public string sucursal { get; set; }
        public decimal monto { get; set; }
        public string fecha { get; set; }
        public string fechaHora { get; set; }
        public string fechaHoraAlta { get; set; }
        public string fechaLiberacion { get; set; }
        public string numeroAutorizacion { get; set; }
        public string referencia { get; set; }
        public string descripciones { get; set; }
        public string codigoDeposito { get; set; }
        public string formaPago { get; set; }
        public string tipoDeposito { get; set; }
        public string referenciaContabilidad { get; set; }
    }

    public class DetalleDepositoBanca
    {
        public string id { get; set; }
        public int sucursal { get; set; }
        public string numeroAutorizacion { get; set; }
        public string referencia { get; set; }
        public string descripciones { get; set; }
        public decimal monto { get; set; }
        public string fecha { get; set; }
        public string fechaHoraAlta { get; set; }
        public string estatus { get; set; }
        public int idCatalogoFormaPago { get; set; }
        public string codigoDeposito { get; set; }
        public int idDeposito { get; set; }
        public int contabilidad { get; set; }
        public int idCatalogoCuenta { get; set; }
        public string cuenta { get; set; }
        public string hora { get; set; }
        public bool reverso { get; set; }

    }

    #endregion


    #region RequestModels

    public class LiberacionesRequestDepositoBanco
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string monto { get; set; }
        public string referencia2 { get; set; }
        public int idCatalogoCuenta { get; set; }
        public int idformapago { get; set; }
        public string sucursal { get; set; }
        public string referencia { get; set; }
        public int tipoDeposito { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestDesRelacionarDeposito
    {
        public string idDepositoStr { get; set; }
        public int idDeposito { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestEliminarDeposito
    {
        public string idDepositoStr { get; set; }
        public int idDeposito { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestEliminaDepositosBanco
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string monto { get; set; }
        public string referencia2 { get; set; }
        public int idCatalogoCuenta { get; set; }
        public int idformapago { get; set; }
        public string sucursal { get; set; }
        public string referencia { get; set; }
        public int tipoDeposito { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestConciliaNoRelacionados
    {
        public string desde { get; set; }
        public int idCatalogoCuenta { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestValidaRelacionados
    {
        public string desde { get; set; }
        public int idCatalogoCuenta { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequetNuevoDepositoBanco
    {
        public string sucursal { get; set; }
        public string noAuto { get; set; }
        public string referencia { get; set; }
        public string descripciones { get; set; }
        public float monto { get; set; }
        public string fecha { get; set; }
        public string fechaHoraAlta { get; set; } = "";
        public int idCatalogoEstatusDeposito { get; set; } = 1;
        public int idCatalogoFormaPago { get; set; }
        public string codigoDeposito { get; set; }
        public int idCatalogoCuenta { get; set; }
        public string horaDeposito { get; set; }
        public int idUsuarioVentas { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequetEditDepositoBanco
    {
        public int id { get; set; }
        public string idDepositoStr { get; set; }
        public string sucursal { get; set; }
        public string referencia { get; set; }
        public DateTime fecha { get; set; }
        public int idCatalogoFormaPago { get; set; }
        public int idCatalogoCuenta { get; set; }
        public string hora { get; set; }
        public int idUsuarioVentas { get; set; }
        public string ip { get; set; }
    }
    #endregion


    #region ResponseModels

    public class LiberacionesResponseListadoDepositosBanca : ResponseModel
    {
        public List<DepositoBanca> listadoDepositosBanca { get; set; }
    }

    public class LiberacionesResponseDepositosBancaListadoCuenta : ResponseModel
    {
        public List<Cuentas> listadoCuentas { get; set; }
    }

    public class LiberacionesResponseDepositosBancaFormasPago : ResponseModel
    {
        public List<FormasPago> listaFormasPago { get; set; }
    }

    public class LiberacionesResponseDepositosBancaListadoTipoDepositos : ResponseModel
    {
        public List<TipoDeposito> listadoTipoDepositos { get; set; }
    }

    public class LiberacionesResponseDepositosBancaDesRelacionarDeposito : ResponseModel
    {
        public bool desrelacionado { get; set; }
    }

    public class LiberacionesResponseDepositosBancaEliminarDepositos : ResponseModel
    {
        public bool eliminado { get; set; }
    }

    public class LiberacionesResponseDepositosBancaConciliacionDepositos : ResponseModel
    {
        public bool conciliado { get; set; }
    }

    public class LiberacionesResponseDepositosBancaValidaReferenciados : ResponseModel
    {
        public bool validado { get; set; }
    }

    public class LiberacionesResponseDetalleDepositoBanca : ResponseModel
    {
        public DetalleDepositoBanca detalleDeposito { get; set; }
    }

    public class LiberacionesResponseOperacionConciliarValidar : ResponseModel
    {
        public bool realizado { get; set; } = false;
    }

    public class LiberacionesResponseNuevoDepositoBanca : ResponseModel
    {
        public bool registrado { get; set; }
    }

    public class LiberacionesResponseEditaDeposito : ResponseModel
    {
        public bool editado { get; set; }
    }
    #endregion

    #endregion

    #region Fechas Feriadas

    #region Modelos

    public class FechasFeriadas
    {
        public int id { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public string fechaLaboralSiguiente { get; set; }
        public string bancos { get; set; }
    }

    public class Bancos
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    #endregion

    #region RequestModels

    public class LiberacionesRequestNuevaFechaFeriada
    {
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public string fechaLaboralSiguiente { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LiberacionesRequestNuevaFechaBanco
    {
        public int idFechaFeriada { get; set; }
        public int idBanco { get; set; }

    }

    #endregion

    #region ResponseModels

    public class LiberacionesResponseListadoFechasFeriadas : ResponseModel
    {
        public List<FechasFeriadas> listadoFechasFeriadas { get; set; }
    }

    public class LiberacionesResponseListadoBancos : ResponseModel
    {
        public List<Bancos> listadoBancos { get; set; }
    }

    public class LiberacionesResponseNuevaFechaFeriada : ResponseModel
    {
        public int idFecha { get; set; }
    }

    public class LiberacionesResponseNuevaFechaBanco : ResponseModel
    {
        public bool registrada { get; set; }
    }
    #endregion

    #endregion

    #region Diferencia Horas

    #region Modelos

    public class DiferenciaHoras
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int diferenciaHora { get; set; }
        public string horaReal { get; set; }
    }

    public class EstadosZonaHoraria
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }
    #endregion

    #region RequestModels

    public class LiberacionesRequestListadoDiferenciaHoras
    {
        public int idUsuario { get; set; }
        public string ip { get; set; }

    }


    public class LiberacionesRequestGestionaDiferenciaHoras
    {
        public int idEstado { get; set; }
        public int diferenciaHora { get; set; }
        public int menu { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }

    }

    #endregion

    #region ResponseModels



    public class LiberacionesResponseListadoDifrenciaHoras : ResponseModel
    {
        public List<DiferenciaHoras> listadoDiferenciaHoras { get; set; }
    }

    public class LiberacionesResponseGestionaDiferenciaHoras : ResponseModel
    {
        public bool realizado { get; set; }
    }


    public class LiebracionResponseLsitadoEstadosZona : ResponseModel
    {
        public List<EstadosZonaHoraria> listadoEstados { get; set; }
    }

    #endregion

    #endregion

    #region Cadenas Referenciadas

    #region Modelos

    public class CadenasReferenciadas
    {
        public int id { get; set; }
        public int idCadena { get; set; }
        public string nombreComercial { get; set; }
        public int idCuenta { get; set; }
        public string bancoCuenta { get; set; }
        public string referencia { get; set; }
    }

    public class LiberacionAutomaticaReferenciadas
    {
        public int id { get; set; }
        public string numeroCuenta { get; set; }

    }

    #endregion

    #region RequestModels

    public class LiberacionesRequestListadoCadenasReferenciadas
    {
        public int idCadena { get; set; }
        public string busqueda { get; set; }

    }

    public class LiberacionesRequestNuevaCadenaReferenciada
    {
        public int idCadena { get; set; }
        public int idCuenta { get; set; }
        public string referencia { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }

    }

    public class LiberacionesRequestEditaCadenaReferenciada
    {
        public int idReferencia { get; set; }
        public int idCuenta { get; set; }
        public string referencia { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }

    }

    public class LiberacionesRequestDeleteCadenaReferenciada
    {
        public int idReferencia { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }

    }
    #endregion

    #region ResponseModels

    public class LiberacionesResponseListadoCadenasReferenciadas : ResponseModel
    {
        public List<CadenasReferenciadas> listadoCadenasReferenciadas { get; set; }
    }

    public class LiberacionesResponseCadenasReferenciadasLiberacionAutomatica : ResponseModel
    {
        public List<LiberacionAutomaticaReferenciadas> listadoLiberacionAutomatica { get; set; }
    }

    public class LiberacionesResponseNuevaCadenaReferenciada : ResponseModel
    {
        public bool agregada { get; set; }
    }

    public class LiberacionesResponseEditaCadenaReferenciada : ResponseModel
    {
        public bool editada { get; set; }
    }

    public class LiberacionesResponseDeleteCadenaReferenciada : ResponseModel
    {
        public bool eliminada { get; set; }
    }
    #endregion

    #endregion

    #region Coordinadores Activos

    #region Modelos

    public class Coordinador
    {
        public int id { get; set; }
        public string coordinador { get; set; }
        public bool activo { get; set; }
    }

    #endregion

    #region RequestModels

    public class LiberacionesRequestSetEstatusCoordinador
    {
        public int idUsuario { get; set; }
        public bool estatus { get; set; }

    }

    #endregion

    #region ResponseModels

    public class LiberacionesResponseListadoCoordinadores : ResponseModel
    {
        public List<Coordinador> listadoCoordinadores { get; set; }
    }

    public class LiberacionesResponEstatusCoordinador : ResponseModel
    {
        public bool cambiado { get; set; }
    }

    #endregion

    #endregion

    #region Control de cuentas

    #region Models

    public class CadenaCuenta
    {
        public int idCadena { get; set; }
        public string nombre { get; set; }
        public bool bancomer { get; set; }
        public bool banamex { get; set; }
        public bool banorte { get; set; }
        public bool santander { get; set; }
        public bool bancoAzteca { get; set; }
        public bool bancoBajio { get; set; }
        public bool inbursa { get; set; }
        public bool bloqueoGeneral { get; set; }
    }
    #endregion

    #region RequestModels

    public class LiberacionesRequestCadenasCuentas
    {
        public string idCadena { get; set; }
    }

    public class LiberacionesRquestModificarLiberacion : UsuarioIp
    {
        public int opcion { get; set; } = 0;
        public int id { get; set; } = 0;
        public int idCadena { get; set; } = 0;
        public int idBanco { get; set; } = 0;

    }
    public class LiberacionesRequestBloqueoCuentas
    {
        public string nombre { get; set; }
        public string ext { get; set; }
        public string mimeType { get; set; }
        public string archivo { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    #endregion

    #region ResponseModels

    public class LiberacionesResponseListadoCuentasBloqueo : ResponseModel
    {
        public List<CadenaCuenta> listadoCadenaCuentas { get; set; }
    }

    public class LiberacionesResponseOperacionCuentaBloqueo : ResponseModel
    {
        public bool cambiado { get; set; } = false;
    }

    public class LiberacionesResponseArchivoBloqueo : ResponseModel
    {
        public bool registrado { get; set; } = false;
    }


    #endregion

    #endregion

    #region Depositos Rechazados

    #region model
    public class LiberacionesDepositosRechazadosModel
    {
        public int id { get; set; }
        public string formaPago { get; set; }
        public int idCatalogoCuenta { get; set; }
        public string banco { get; set; }
        public string fechaDeposito { get; set; }
        public string fechaRechazo { get; set; }
        public int idCadena { get; set; }
        public int idCatalogoEstatusDeposito { get; set; }
        public float monto { get; set; }
        public string nombreUsuario { get; set; }
    }

    #endregion

    #region RequestModel
    public class LiberacionesDepositosRechazadosRequest
    {
        public int idCadena { get; set; }
        public int idusuario { get; set; }
        public int rol { get; set; }
    }
    public class LiberacionesDesbloquearDepositoRequest
    {
        public int idDeposito { get; set; }
        public int idusuario { get; set; }
        public string ip { get; set; }
    }
        
    #endregion

    #region ResponseModel
    public class LiberacionesListaDepositoRechazadoResponse : ResponseModel
    {
        public List<LiberacionesDepositosRechazadosModel> listado { get; set; }
    }
    public class LiberacionesDesbloquearDepositoResponse : ResponseModel
    {
        public bool cambiado { get; set; }
    }
    #endregion


    #endregion
}
