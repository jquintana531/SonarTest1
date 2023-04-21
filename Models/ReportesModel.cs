using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    #region Reporte Ventas
    public class ReporteRequestReporteVentasModel
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
        public int idTienda { get; set; }
        public int clasificacion { get; set; }
        public int idProveedor { get; set; }
        public int idProducto { get; set; }
        public string monto { get; set; }
        public string referencia { get; set; }
        public int estatus { get; set; }
    }

    public class ReporteReporteVentasModel
    {
        public DateTime fechaHora { get; set; }
        public string MTCid { get; set; }
        public string tienda { get; set; }
        public string cajero { get; set; }
        public string clasificacion { get; set; }
        public string producto { get; set; }
        public string proveedor { get; set; }
        public string referencia1 { get; set; }
        public string referencia2 { get; set; }
        public string entrada { get; set; }
        public string salida { get; set; }
        public string comisionRecarga { get; set; }
        public string comisionEstablecimiento { get; set; }
        public string autorizacion { get; set; }
        public string tipoTerminal { get; set; }
        public string estatusCancelada { get; set; }
    }


    public class ReporteResponseReporteVentas : ResponseModel
    {
        public List<ReporteReporteVentasModel> respuesta { get; set; }
    }

    public class ReporteResponseReporteVentasExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteVentasModel> respuestaLista { get; set; }
    }



    public class ReporteRequestGetEstablecimientosModel
    {
        public int idCadena { get; set; }
        public int activo { get; set; }
        public bool menu { get; set; }
    }

    public class ReporteEstablecimientoModel
    {
        public int id { get; set; }
        public string label { get; set; }
        public bool activo { get; set; }
    }

    public class ReporteResponseEstablecimientos : ResponseModel
    {
        public List<ReporteEstablecimientoModel> respuesta { get; set; }
    }

    #endregion

    #region Reporte Compras
    public class ReporteRequestReporteComprasModel
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
    }

    public class ReporteReporteComprasModel
    {
        public string orden { get; set; }
        public int idCadena { get; set; }
        public DateTime fechaHora { get; set; }
        public string nombre { get; set; }
        public string cantidad { get; set; }
        public string costo { get; set; }
        public string importeComision { get; set; }
        public string comisionRecarga { get; set; }
        public string estatus { get; set; }
        public string total { get; set; }
        public string observaciones { get; set; }
        public string cajero { get; set; }
    }


    public class ReporteResponseReporteCompras : ResponseModel
    {
        public List<ReporteReporteComprasModel> respuesta { get; set; }
    }

    public class ReporteResponseReporteComprasExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteComprasModel> respuestaLista { get; set; }
    }


    #endregion

    #region Reporte Depositos

    public class ReporteRequestReporteDepositosModel
    {
        internal int idUsuario;

        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
        public string monto { get; set; }
        public string referencia2 { get; set; }
        public int idCatalogoCuenta { get; set; }
    }



    public class ReporteReporteDepositosModel
    {
        public int idDeposito { get; set; }
        public string nombreUsuario { get; set; }
        public int id { get; set; }
        public string nombreComercial { get; set; }
        public string banco { get; set; }
        public string monto { get; set; }
        public string comision { get; set; }
        public DateTime fechaDeposito { get; set; }
        public DateTime horaDeposito { get; set; }
        public DateTime horaLiberacion { get; set; }
        public string referencia2 { get; set; }
        public string formaPago { get; set; }
    }

    public class ReporteResponseReporteDepositos : ResponseModel
    {
        public List<ReporteReporteDepositosModel> respuesta { get; set; }
    }

    public class ReporteResponseReporteDepositosExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteDepositosModel> respuestaLista { get; set; }
    }

    #endregion

    #region Reporte de Depositos Banca

    public class ReporteRequestReporteDepositosBancaModel : ModelBase
    {
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public string monto { get; set; }
        public string autorizacion { get; set; }
        public string sucursal { get; set; }
        public string referencia { get; set; }
        public int idDeposito { get; set; }
        public int idFormaP { get; set; }
        public string idCatalogoCuenta { get; set; }
        public int idUsuario { set; get; }
        public int idCadena { get; set; }
    }



    public class ReporteReporteDepositosBancaModel
    {
        public string tipoDeposito { set; get; }
        public string numeroAutorizacion { set; get; }
        public string formaPago { set; get; }
        public string monto { set; get; }
        public string descripciones { set; get; }
        public string referenciaContabilidad { set; get; }
        public string sucursal { set; get; }
        public string codigoDeposito { set; get; }
        public string referencia { set; get; }
        public DateTime fechaHora { set; get; }
        public DateTime fechaHoraAlta { set; get; }
        public string fechaLiberacion { set; get; }
        public string banco { set; get; }
        public int idDeposito { get; set; }
    }



    public class ReporteResponseReporteDepositosBanca : ResponseModel
    {
        public List<ReporteReporteDepositosBancaModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteDepositosBancaExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteDepositosBancaModel> respuestaLista { get; set; }
    }

    public class ReporteCatalogosDepositosBancaResponse : ResponseModel
    {
        public List<ContabilidadCuentas> listadoCuentas { get; set; }
        public List<FormasPago> listadoFormasPago { get; set; }
        public List<TipoDeposito> listadoTipoDepositos { get; set; }
    }

    #endregion

    #region Reporte Balance General

    public class ReporteRequestReporteBalanceGeneralModel : ModelBase
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
    }



    public class ReporteReporteBalanceGeneralModel
    {
        public DateTime fechaInicial { get; set; }
        public decimal saldoInicial { get; set; }
        public decimal depositos { get; set; }
        public decimal comisionDepositos { get; set; }
        public decimal entradas { get; set; }
        public decimal salidas { get; set; }
        public decimal compras { get; set; }
        public decimal comisionEstablecimiento { get; set; }
        public decimal comisionRecarga { get; set; }
        public decimal credito { get; set; }
        public decimal saldoFinal { get; set; }
        public decimal saldoCalculado { get; set; }
    }



    public class ReporteResponseReporteBalanceGeneral : ResponseModel
    {
        public List<ReporteReporteBalanceGeneralModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteBalanceGeneralExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteBalanceGeneralModel> respuestaLista { get; set; }
    }


    #endregion

    #region Reporte Monedero

    public class ReporteRequestReporteMonederoModel : ModelBase
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idMonedero { get; set; }
    }



    public class ReporteReporteMonederoModel
    {
        public string Autorizacion { get; set; }
        public int idMonedero { get; set; }
        public string nombre { get; set; }
        public DateTime fechaHora { get; set; }
        public string descripcion { get; set; }
        public decimal monto { get; set; }
        public decimal saldoMonedero { get; set; }
    }



    public class ReporteResponseReporteMonedero : ResponseModel
    {
        public List<ReporteReporteMonederoModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteMonederoExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteMonederoModel> respuestaLista { get; set; }
    }


    #endregion

    #region Reporte Credito Cadena

    public class ReporteRequestReporteCreditoCadenaModel : ModelBase
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
    }



    public class ReporteReporteCreditoCadenaModel
    {
        public string Clasificacion { get; set; }
        public int idCadena { get; set; }
        public string TRX { get; set; }
        public string entrada { get; set; }
        public string salida { get; set; }
        public string comisionRecarga { get; set; }
        public string comisionEstablecimiento { get; set; }
    }



    public class ReporteResponseReporteCreditoCadena : ResponseModel
    {
        public List<ReporteReporteCreditoCadenaModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteCreditoCadenaExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteCreditoCadenaModel> respuestaLista { get; set; }
    }


    #endregion

    #region Reporte Pago Tarjeta

    public class ReporteRequestReportePagoTarjetaModel : ModelBase
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string autorizacion { get; set; }
        public int idProveedor { get; set; }
    }



    public class ReporteReportePagoTarjetaModel
    {
        public string fechaHora { get; set; }
        public int MTCid { get; set; }
        public string tienda { get; set; }
        public string tarjeta { get; set; }
        public string afiliacion { get; set; }
        public decimal monto { get; set; }
        public string autorizacion { get; set; }
        public string tipo { get; set; }
        public string proveedor { get; set; }
        public decimal comisionMTC { get; set; }
        public decimal comisionEstablecimiento { get; set; }
    }



    public class ReporteResponseReportePagoTarjeta : ResponseModel
    {
        public List<ReporteReportePagoTarjetaModel> respuesta { get; set; }
    }
    public class ReporteResponseReportePagoTarjetaExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReportePagoTarjetaModel> respuestaLista { get; set; }
    }


    #endregion

    #region Reporte Ventas (Recomendados)

    public class ReporteRequestReporteVentasRecomendadosModel : ModelBase
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string nombreRecomendado { get; set; }
        public string nombreBeneficiario { get; set; }
        public string cadenaRecomendado { get; set; }
        public string cadenaBeneficiario { get; set; }
    }



    public class ReporteReporteVentasRecomendadosModel
    {
        public DateTime FechaRecomendacion { get; set; }
        public int MTCidBeneficiario { get; set; }
        public string NombreComercialBeneficiario { get; set; } 
        public int MTCidRecomendado { get; set; }
        public string ContactoRecomendado { get; set; }
        public string EmailRecomendado { get; set; }
        public string Tel1Recomendado { get; set; }
        public string ejecutivo { get; set; }
        public string tarea { get; set; }
        public string comentario { get; set; }
        public string activo { get; set; }
        public int id { get; set; }
    }



    public class ReporteResponseReporteVentasRecomendados : ResponseModel
    {
        public List<ReporteReporteVentasRecomendadosModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteVentasRecomendadosExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteVentasRecomendadosModel> respuestaLista { get; set; }
    }

    #endregion

    #region Reporte Cliente Factura

    public class ReporteRequestReporteClienteFacturaModel : ModelBase
    {
        public int idCadena { get; set; }
        public int idCatalogoTipofactura { get; set; }
    }



    public class ReporteReporteClienteFacturaModel
    {
        public int idCadena { get; set; }
        public string rfc { get; set; }
        public string tipoFactura { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
    }



    public class ReporteResponseReporteClienteFactura : ResponseModel
    {
        public List<ReporteReporteClienteFacturaModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteClienteFacturaExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteClienteFacturaModel> respuestaLista { get; set; }
    }





    public class ReporteRequestReporteTipoFacturaModel : ModelBase
    {
        public int id { get; set; }
        public Boolean menu { get; set; }
    }

    public class ReporteReporteTipoFacturaModel
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class ReporteResponseReporteTipoFactura : ResponseModel
    {
        public List<ReporteReporteTipoFacturaModel> respuesta { get; set; }
    }


    #endregion

    #region Reporte Cash Out

    public class ReporteRequestReporteCashOutModel : ModelBase
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
    }



    public class ReporteReporteCashOutModel
    {
        public string nombre { get; set; }
        public int mtcid { get; set; }
        public string nombreComercial { get; set; }
        public string cuenta { get; set; }
        public decimal monto { get; set; }
        public DateTime horaLiberacion { get; set; }
    }



    public class ReporteResponseReporteCashOut : ResponseModel
    {
        public List<ReporteReporteCashOutModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteCashOutExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteCashOutModel> respuestaLista { get; set; }
    }

    #endregion

    #region Reporte Credito

    public class ReporteRequestReporteCreditoModel : ModelBase
    {
        public string tbDesde { get; set; }
        public string tbHasta { get; set; }
        public int idCadena { get; set; }
        public int tipoReporte { get; set; }
    }

    public class ReporteReporteCreditoModel
    {
        public DateTime fecha { get; set; }
        public string ejecutivo { get; set; }
        public int cadena { get; set; }
        public string cadenaMonto { get; set; }
    }

    public class ReporteReporteCredito2Model
    {
        public int idCadena { get; set; }
        public decimal credito { get; set; }
    }

    public class ReporteReporteCredito3Model
    {
        public int idCadena { get; set; }
        public decimal credito { get; set; }
        public string modalidad { get; set; }
    }

    public class ReporteResponseReporteCredito : ResponseModel
    {
        public Object respuesta { get; set; }
    }
    public class ReporteResponseReporteCreditoExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public Object respuestaLista { get; set; }
    }

    #endregion

    #region Reporte Visor Notificaciones 

    public class ReporteRequestReporteVisorNotificacionesModel : ModelBase
    {
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public int idCadena { get; set; }
    }



    public class ReporteReporteVisorNotificacionesModel
    {
        public string mensaje { get; set; }
        public string actividad { get; set; }
        public DateTime fecha { get; set; }
    }



    public class ReporteResponseReporteVisorNotificaciones : ResponseModel
    {
        public List<ReporteReporteVisorNotificacionesModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteVisorNotificacionesExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteVisorNotificacionesModel> respuestaLista { get; set; }
    }

    #endregion

    #region Reporte Resumen Transacciones

    public class ReporteRequestReporteResumenTransaccionesModel : ModelBase
    {
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public int idCadena { get; set; }
    }




    public class ReporteReporteResumenTransaccionesModel
    {
        public int mtcid { get; set; }
        public string sucursal { get; set; }
        public string referencia1 { get; set; }
        public string numeroAutorizacion { get; set; }
        public decimal entrada { get; set; }
        public DateTime fechaHoraResponse { get; set; }
        public string empresa { get; set; }
        public string producto { get; set; }
    }



    public class ReporteResponseReporteResumenTransacciones : ResponseModel
    {
        public List<ReporteReporteResumenTransaccionesModel> respuesta { get; set; }
    }
    public class ReporteResponseReporteResumenTransaccionesExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteResumenTransaccionesModel> respuestaLista { get; set; }
    }

    #endregion
    
    #region Reporte Tickets

    public class ReporteRequestReporteTicketsModel : ModelBase
    {
        public string busqueda { get; set; }
        public int opcion { get; set; }
        public DateTime? desde { get; set; } = null;
        public DateTime? hasta { get; set; } = null;
        public int idUsuario { get; set; }
    }


    public class ReporteReporteTicketsModel
    {
        public int id { get; set; }
        public int ticketID { get; set; }
        public DateTime fechaAlta { get; set; }
        public string tipoTicket { get; set; }
        public string medio { get; set; }
        public string asunto { get; set; }
        public string estatus { get; set; }
        public string prioridad { get; set; }
        public string colorPrioridad { get; set; }
        public int vencido { get; set; }
        public string primerContacto { get; set; }
        public string ultimoContacto { get; set; }
        public string seguimiento { get; set; }
        public DateTime fechaVencimiento { get; set; }
    }

    public class ReporteResponseReporteTickets : ResponseModel
    {
        public List<ReporteReporteTicketsModel> respuesta { get; set; }
    }

    public class ReporteResponseReporteTicketsExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteTicketsModel> respuestaLista { get; set; }
    }




    public class ReporteRequestObtenerTicketModel : ModelBase
    {
        public int ticketID { get; set; }
    }


    public class ReporteObtenerTicketModel
    {
        public int ticketID { get; set; }
        public string nombreAltaTicket { get; set; }
        public string correoAltaTicket { get; set; }
        public string telefonoAltaTicket { get; set; }
        public string asunto { get; set; }
        public string tema { get; set; }
        public string departamento { get; set; }
        public string estatus { get; set; }
        public string prioridad { get; set; }
        public DateTime fechaAlta { get; set; }
        public DateTime fechaActualizado { get; set; }
        public DateTime fechaUltimoMensaje { get; set; }
        public int contestado { get; set; }
        public DateTime fechaUltimaRespuesta { get; set; }
        public DateTime fechaCierre { get; set; }
        public string usuarioAsignado { get; set; }
        public int idUsuarioAsignado { get; set; }
        public string vencimiento { get; set; }
        public string origen { get; set; }
        public string ip { get; set; }
    }

    public class ReporteResponseObtenerTicket : ResponseModel
    {
        public ReporteObtenerTicketModel respuesta { get; set; }
    }


    public class ReporteObtenerTicketConversacionModel
    {
        public int rowNumber { set; get; }
        public int idConversacion { set; get; }
        public string usuario { set; get; }
        public int idTicket { set; get; }
        public string mensaje { set; get; }
        public DateTime fechaHora { set; get; }
        public string tipo { set; get; }
        public int adjunto { set; get; }
        public string nombreArchivo { set; get; }
        public string hashName { set; get; }
        public string extension { set; get; }
        public string base64 { set; get; }
    }

    public class ReporteResponseObtenerTicketConversacion : ResponseModel
    {
        public List<ReporteObtenerTicketConversacionModel> respuesta { get; set; }
    }

    public class ReporteResponseTicket : ResponseModel
    {
        public ReporteObtenerTicketModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> respuesta2 { get; set; }
        public List<ReportePrioridadCatalogoModel> respuesta3 { get; set; }
        public List<ReporteUsuarioDepartamentoModel> respuesta4 { get; set; }
        public List<ReporteCatalogoDepartamentoModel> respuesta5 { get; set; }
    }


    public class ReporteRequestPrioridadCatalogoModel : ModelBase
    {
        public int id { get; set; }
        public Boolean menu { get; set; }
    }


    public class ReportePrioridadCatalogoModel
    {
        public int id { set; get; }
        public string label { set; get; }
    }

    public class ReporteResponsePrioridadCatalogo : ResponseModel
    {
        public List<ReportePrioridadCatalogoModel> respuesta { get; set; }
    }




    public class ReporteRequestUsuarioDepartamentoModel : ModelBase
    {
        public int idUsuario { get; set; }
    }


    public class ReporteUsuarioDepartamentoModel
    {
        public int id { set; get; }
        public string label { set; get; }
    }

    public class ReporteResponseUsuarioDepartamento : ResponseModel
    {
        public List<ReporteUsuarioDepartamentoModel> respuesta { get; set; }
    }


    public class ReporteCatalogoDepartamentoModel
    {
        public int id { set; get; }
        public string label { set; get; }
    }

    public class ReporteResponseCatalogoDepartamento : ResponseModel
    {
        public List<ReporteCatalogoDepartamentoModel> respuesta { get; set; }
    }



    public class ReporteRequestCambioPrioridadModel : ModelBase
    {
        public int ticketID { get; set; }
        public int idPrioridad { get; set; }
        public int idUsuario { get; set; }
    }


    public class ReporteCambioPrioridadModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseCambioPrioridad : ResponseModel
    {
        public ReporteCambioPrioridadModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> lista { get; set; }
    }






    public class ReporteRequestCambioVencidoModel : ModelBase
    {
        public int ticketID { get; set; }
        public Boolean vencido { get; set; }
        public int idUsuario { get; set; }
    }
    public class ReporteCambioVencidoModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseCambioVencido : ResponseModel
    {
        public ReporteCambioVencidoModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> lista { get; set; }
    }



    public class ReporteRequestLiberarTicketModel : ModelBase
    {
        public int ticketID { get; set; }
        public int idUsuarioAsignado { get; set; }
        public int idUsuario { get; set; }
    }
    public class ReporteLiberarTicketModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseLiberarTicket : ResponseModel
    {
        public ReporteLiberarTicketModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> lista { get; set; }
    }








    public class ReporteRequestAsignarTicketModel : ModelBase
    {
        public int ticketID { get; set; }
        public int idUsuarioAsignado { get; set; }
        public int idUsuario { get; set; }
    }
    public class ReporteAsignarTicketModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseAsignarTicket : ResponseModel
    {
        public ReporteAsignarTicketModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> lista { get; set; }
    }



    public class ReporteRequestTransferirTicketModel : ModelBase
    {
        public int ticketID { get; set; }
        public int idDepartamento { get; set; }
        public int idUsuario { get; set; }
    }
    public class ReporteTransferirTicketModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseTransferirTicket : ResponseModel
    {
        public ReporteTransferirTicketModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> lista { get; set; }
    }



    public class ReporteRequestCerrarTicketModel : ModelBase
    {
        public int ticketID { get; set; }
        public int idCatalogoStatusTicket { get; set; }
        public int idUsuario { get; set; }
    }
    public class ReporteCerrarTicketModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseCerrarTicket : ResponseModel
    {
        public ReporteCerrarTicketModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> lista { get; set; }
    }






    public class ReporteRequestNuevaRespuestaModel : ModelBase
    {
        public int ticketID { get; set; }
        public string mensaje { get; set; }
        public int idUsuario { get; set; }
        public IFormFile file { get; set; }
    }
    public class ReporteNuevaRespuestaModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseNuevaRespuesta : ResponseModel
    {
        public ReporteNuevaRespuestaModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> lista { get; set; }
    }



    public class ReporteRequestNuevaNotaModel : ModelBase
    {
        public int ticketID { get; set; }
        public string mensaje { get; set; }
        public string titulo { get; set; }
        public int idUsuario { get; set; }
        public IFormFile file { get; set; }
    }
    public class ReporteNuevaNotaModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseNuevaNota : ResponseModel
    {
        public ReporteNuevaNotaModel respuesta { get; set; }
        public List<ReporteObtenerTicketConversacionModel> lista { get; set; }
    }

    public class ReporteRequestUploadTicketDocModel : ModelBase
    {
        public int ticketID { get; set; }
        
    }
    public class ReporteUploadTicketDocModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseUploadTicketDoc : ResponseModel
    {
        public ReporteUploadTicketDocModel respuesta { get; set; }
    }



    public class ReporteRequestNuevoTicketAdjuntoModel : ModelBase
    {
        public int ticketID { get; set; }
        public int idConversacion { get; set; }
        public string tipoConversacion { get; set; }
        public string tipoArchivo { get; set; }
        public string tamano { get; set; }
        public string nombre { get; set; }
        public string hashName { get; set; }
        public string ip { get; set; }
        public string base64 { get; set; }
    }

    public class ReporteNuevoTicketAdjuntoModel
    {
        public int resultado { get; set; }
    }

    public class ReporteResponseNuevoTicketAdjunto : ResponseModel
    {
        public ReporteNuevoTicketAdjuntoModel respuesta { get; set; }
    }

    public class ListadoLabelReporte : ListadoLabel
    {
        public int idDepartamento { get; set; }
    }
    public class ReporteResponseListadoTemas : ResponseModel
    {
        public List<ListadoLabelReporte> listado { get; set; }
    }
    public class ReporteResponseListadoGenerico : ResponseModel
    {
        public List<ListadoLabel> listado { get; set; }
    }

    public class OperacionesResponselistadoCatalogosAgregarTicket : ResponseModel
    {
        public ReporteResponseListadoTemas temas { get; set; }
        public ReporteResponseListadoGenerico asuntos { get; set; }
        public ReporteResponseListadoGenerico departamentos { get; set; }
        public ReporteResponseListadoGenerico vias { get; set; }
        public ReporteResponseListadoGenerico prioridades { get; set; }
    }
    public class ReporteRequestCatalogo
    {
        public int id { get; set; }
        public int menu { get; set; }
    }

    public class ReporteRequestCatalogoAsunto : ReporteRequestCatalogo
    {
        public int idTema { get; set; }
    }



    
    public class ReporteRequestNuevoTicketModel : ModelBase
    {
        public int idTema { get; set; }
        public int idAsunto { get; set; }
        public int idDepartamento { get; set; }
        public int idCadena { get; set; }
        public int idEstablecimiento { get; set; }
        public int idCajero { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public int idOrigen { get; set; }
        public int idViaContacto { get; set; }
        public int idPrioridad { get; set; }
        public string mensaje { get; set; }
        public int idUsuario { get; set; }
    }

    public class ReporteResponseNuevoTicket : ResponseModel
    {
        public int idTicket { get; set; }
    }




    #endregion

    #region Reporte de Errores

    public class ReporteRequestObtenerCadenaErrorModel : ModelBase
    {
        public int idCadena { get; set; }
    }

    public class ReporteRequest2ObtenerCadenaErrorModel : ModelBase
    {
        public int idCadena { get; set; }
        public int activo { get; set; }
        public Boolean menu { get; set; }
    }


    public class ReporteResponseObtenerCadenaError : ResponseModel
    {
        public List<ReporteEstablecimientoModel> respuesta { get; set; }
        public List<ReporteCatalogoClasificacionProductoModel> respuesta2 { get; set; }
    }



    public class ReporteRequestCatalogoClasificacionProductoModel : ModelBase
    {
        public int id { get; set; }
        public Boolean menu { get; set; }
    }

    public class ReporteCatalogoClasificacionProductoModel
    {
        public int id { set; get; }
        public string label { set; get; }
    }

    public class ReporteResponseCatalogoClasificacionProducto : ResponseModel
    {
        public List<ReporteCatalogoClasificacionProductoModel> respuesta { get; set; }
    }


    public class ReporteRequestCatalogoProductoModel : ModelBase
    {
        public int id { get; set; }
        public Boolean menu { get; set; }
        public int activo { get; set; }
    }

    public class ReporteCatalogoProductoModel
    {
        public int id { set; get; }
        public string label { set; get; }
        public int idCatalogoClasificacionProducto { set; get; }
        public int idCatalogoProveedor { set; get; }
    }

    public class ReporteResponseCatalogoProducto : ResponseModel
    {
        public List<ReporteCatalogoProductoModel> respuesta { get; set; }
    }




    public class ReporteRequestCatalogoProveedoresModel : ModelBase
    {
        public int id { get; set; }
        public Boolean menu { get; set; }
        public int idClasificacionProducto { get; set; }
        public int idCatalogoIntermediario { get; set; }
    }

    public class ReporteCatalogoProveedoresModel
    {
        public int id { set; get; }
        public string label { set; get; }
    }

    public class ReporteResponseCatalogoProveedores : ResponseModel
    {
        public List<ReporteCatalogoProveedoresModel> respuesta { get; set; }
    }

    public class ReporteRequestObtenerCatalogosErrorModel : ModelBase
    {
        public int idClasificacion { get; set; }
    }

    public class ReporteResponseObtenerCatalogosError : ResponseModel
    {
        public List<ReporteCatalogoProductoModel> respuesta3 { get; set; }
        public List<ReporteCatalogoProveedoresModel> respuesta4 { get; set; }
    }



    public class ReporteRequestReporteVentaErrorModel : ModelBase
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idCadena { get; set; }
        public int idTienda { get; set; }
        public int clasificacion { get; set; }
        public int idProveedor { get; set; }
        public int idProducto { get; set; }
    }


    public class ReporteReporteVentaErrorModel
    {
        public string clasificacion { get; set; }
        public string proveedor { get; set; }
        public string codigoRespuesta { get; set; }
        public string cantidad { get; set; }
        public string descripcionError { get; set; }
        public string fh { get; set; }
    }


    public class ReporteResponseReporteError : ResponseModel
    {
        public List<ReporteReporteVentaErrorModel> respuesta3 { get; set; }
        public List<ReporteReporteVentaErrorRefModel> respuesta4 { get; set; }
    }


    public class ReporteResponseReporteVentaError : ResponseModel
    {
        public List<ReporteReporteVentaErrorModel> respuesta { get; set; }
    }



    public class ReporteReporteVentaErrorRefModel
    {
        public string fechaHora { get; set; }
        public int MTCid { get; set; }
        public string cadena { get; set; }
        public string tienda { get; set; }
        public string producto { get; set; }
        public string referencia1 { get; set; }
        public string referencia2 { get; set; }
        public string entrada { get; set; }
        public string salida { get; set; }
        public string codigoRespuesta { get; set; }
        public string descripcionError { get; set; }
    }

    public class ReporteResponseReporteVentaErrorRef : ResponseModel
    {
        public List<ReporteReporteVentaErrorRefModel> respuesta { get; set; }
    }


    public class ReporteResponseReporteErrorExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteReporteVentaErrorRefModel> respuesta4 { get; set; }
    }



    #endregion

    #region Reporte Venta Error Six

    #region Models

    public class ReporteVentaErrorResumenSix
    {
        public string clasificacion { get; set; }
        public string proveedor { get; set; }
        public string codigoRespuesta { get; set; }
        public string cantidad { get; set; }
        public string descripcionError { get; set; }
        public string fh { get; set; }
    }

    public class ReporteVentaErrorSix
    {
        public string fechaHora { get; set; }
        public string MTCid { get; set; }
        public string cadena { get; set; }
        public string tienda { get; set; }
        public string cajero { get; set; }
        public string clasificacion { get; set; }
        public string producto { get; set; }
        public string proveedor { get; set; }
        public string referencia1 { get; set; }
        public string referencia2 { get; set; }
        public string entrada { get; set; }
        public string salida { get; set; }
        public string codigoRespuesta { get; set; }
        public string descripcionError { get; set; }
    }


    #endregion

    #region RequestModels

    public class ReportesRequestListadoProveedores
    {
        public int id { get; set; }
        public int idClasificacionProducto { get; set; }
    }
    public class ReportesRequestCatalogoProducto
    {
        public int idClasificacion { get; set; }
        public int idProveedor { get; set; }
    }
    public class ReportesRequestReporteVentaErrorSix
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string tienda { get; set; }
        public int clasificacion { get; set; }
        public int idProveedor { get; set; }
        public int idProducto { get; set; }
    }
    #endregion

    #region ResponseModels

    public class ReportesResponseVentaErrorResumenSix : ResponseModel
    {
        public List<ReporteVentaErrorResumenSix> listaResumenError { get; set; }
    }
    public class ReportesResponseVentaErrorSix : ResponseModel
    {
        public List<ReporteVentaErrorSix> listaVentaError { get; set; }
    }
    public class ReporteResponseClasificacionProducto : ResponseModel
    {
        public List<ListadoLabel> listadoClasificacionProducto { get; set; }
    }
    public class ReporteResponseProveedores : ResponseModel
    {
        public List<ListadoLabel> listadoProveedores { get; set; }
    }
    public class ReporteResponseProductos : ResponseModel
    {
        public List<ListadoLabel> listadoProducto { get; set; }
    }

    public class ReportesResponseVentaErrorSixExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }

    #endregion

    #endregion

    #region Reporte Venta Exitosa Six

    #region Models



    public class ReporteVentaSix
    {
        public string fechaHora { get; set; }
        public string MTCid { get; set; }
        public string cadena { get; set; }
        public string tienda { get; set; }
        public string cajero { get; set; }
        public string producto { get; set; }
        public string proveedor { get; set; }
        public string referencia1 { get; set; }
        public string referencia2 { get; set; }
        public string entrada { get; set; }
        public string salida { get; set; }
        public string comisionRecarga { get; set; }
        public string comisionEstablecimiento { get; set; }
        public string autorizacion { get; set; }
        public string tipoTerminal { get; set; }
    }


    #endregion

    #region RequestModels


    public class ReportesRequestReporteVentaSix
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string tienda { get; set; }
        public int clasificacion { get; set; }
        public int idProveedor { get; set; }
        public int idProducto { get; set; }
        public string monto { get; set; }
        public string referencia { get; set; }
    }
    #endregion

    #region ResponseModels

    public class ReportesResponseVentaSix : ResponseModel
    {
        public List<ReporteVentaSix> listaVentaExitosa { get; set; }
    }

    public class ReportesResponseVentaSixExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }
    #endregion

    #endregion

    #region Reporte Balance SIX

    #region Models

    public class BalanceSix
    {
        public string fechaInicial { get; set; }
        public string saldoInicial { get; set; }
        public string depositos { get; set; }
        public string comisionDepositos { get; set; }
        public string entradas { get; set; }
        public string salidas { get; set; }
        public string compras { get; set; }
        public string comisionEstablecimiento { get; set; }
        public string comisionRecarga { get; set; }
        public string credito { get; set; }
        public string saldoFinal { get; set; }
        public string saldoCalculado { get; set; }
    }

    #endregion

    #region RequestModels

    public class ReportesRequestBalanceSix
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string establecimiento { get; set; }
    }

    #endregion

    #region ResponseModels

    public class ReportesResponseBalanceSix : ResponseModel
    {
        public List<BalanceSix> listadoBalance { get; set; }
    }

    public class ReportesResponseBalanceSixExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }

    #endregion

    #endregion

    #region Reporte Liberaciones Ejecutivos

    #region Models

    public class DepositosEjecutivo
    {
        public int idDeposito { get; set; }
        public int idCadena { get; set; }
        public decimal monto { get; set; }
        public string banco { get; set; }
        public string formaPago { get; set; }
        public string referencia { get; set; }
        public string horaSolicitud { get; set; }
        public string horaLiberacion { get; set; }
        public string idCatalogoEstatusDeposito { get; set; }
        public string usuario { get; set; }
    }
    public class LiberadosDetalles
    {
        public int pendiente { get; set; }
        public int bloqueado { get; set; }
        public int autorizado { get; set; }
        public int rechazado { get; set; }
    }
    #endregion

    #region RequestModels

    public class ReportesRequestDepositosEjecutivo
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int idEstatus { get; set; } = 0;
        public int idUsuario { get; set; } = 0;
    }

    #endregion

    #region ResponseModels

    public class ReportesResponseDepositosEjecutivo : ResponseModel
    {
        public List<DepositosEjecutivo> listadoLiberaciones { get; set; }
        public LiberadosDetalles detalles { get; set; }
    }

    public class ReportesResponseDepositosEjecutivoExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }

    #endregion

    #endregion

    #region Reporte Conciliaciones

    #region models
    public class ReporteConciliacionesModel
    {
        public DateTime fechaHora { get; set; }
        public int MTCid { get; set; }
        public int idTienda { get; set; }
        public string razonSocial { get; set; }
        public string producto { get; set; }
        public string referencia1 { get; set; }
        public int numeroTransaccion { get; set; }
        public float entrada { get; set; }
        public int autorizacion { get; set; }
    }
    #endregion

    #region RequestModel
    public class ReporteConciliacionesRequest
    {
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public int idCadena { get; set; }
        public int idTienda { get; set; }
        public int clasificacion { get; set; }
        public string referencia { get; set; }

    }
    #endregion

    #region ResponseModel
    public class ReporteConciliacionesResponse : ResponseModel
    {
        public List<ReporteConciliacionesModel> lista { get; set; }
    }
    public class ReporteConciliacionesExcelResponse : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<ReporteConciliacionesModel> respuestaLista { get; set; }
    }
    #endregion

    #endregion

    #region Reporte Clientes Activos

    #region Model
    public class clientesActivos
    {
        public string coordinador { get; set; }
        public string ejecutivo { get; set; }
        public string razonSocial { get; set; }
        public DateTime fechaAlta { get; set; }
        public int id { get; set; }
        public DateTime fechaDeposito { get; set; }
        public float monto { get; set; }
        public float ventaTae { get; set; }
        public float saldoCadena { get; set; }
    }
    public class AltasEjecutivo
    {
        public int id { get; set; }
        public string coordinador { get; set; }
        public string ejecutivo { get; set; }
        public string razonSocial { get; set; }
        public DateTime fechaAlta { get; set; }
    }

    public class medicionDesempeno
    {
        public string coordinador { get; set; }
        public string ejecutivo { get; set; }
        public int leadsAsignados { get; set; }
        public int clientesConDep { get; set; }
        public int clientesSinVentas { get; set; }
        public int clientesConVentas { get; set; }
        public int meta { get; set; }
        public int avanceMeta { get; set; }
        public int conversionConDep { get; set; }
        public int conversionConVenta { get; set; }
        public string estatus { get; set; }
    }

    public class MetasMensualesModel
    {
        public string mes { get; set; }
        public int anio { get; set; }
        public int leadsMeta { get; set; }
        public int metaClientes { get; set; }
        public int noEjecutivos { get; set; }
        public float porcentajeConversion { get; set; }
    }

    public class CanalMensualesModel
    {
        public string canal { get; set; }
        public int dia1 { get; set; }
        public int dia2 { get; set; }
        public int dia3 { get; set; }
        public int dia4 { get; set; }
        public int dia5 { get; set; }
        public int dia6 { get; set; }
        public int dia7 { get; set; }
        public int dia8 { get; set; }
        public int dia9 { get; set; }
        public int dia10 { get; set; }
        public int dia11 { get; set; }
        public int dia12 { get; set; }
        public int dia13 { get; set; }
        public int dia14 { get; set; }
        public int dia15 { get; set; }
        public int dia16 { get; set; }
        public int dia17 { get; set; }
        public int dia18 { get; set; }
        public int dia19 { get; set; }
        public int dia20 { get; set; }
        public int dia21 { get; set; }
        public int dia22 { get; set; }
        public int dia23 { get; set; }
        public int dia24 { get; set; }
        public int dia25 { get; set; }
        public int dia26 { get; set; }
        public int dia27 { get; set; }
        public int dia28 { get; set; }
        public int dia29 { get; set; }
        public int dia30 { get; set; }
        public int dia31 { get; set; }
    }
    #endregion

    #region Request Model
    public class ReporteClientesActivosRequest
    {
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public int idCoordinador { get; set; }
        public int idEjecutivo { get; set; }
    }
    public class MetasMensualesRequest
    {
        public int mes { get; set; }
        public int anio { get; set; }
    }
    public class CanalMensualesRequest
    {
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }

    }
    #endregion

    #region Response Model
    public class ReporteClientesActivosResponse : ResponseModel
    {
        public List<clientesActivos> lista { get; set; }
    }
    public class ReporteAltasEjecutivoResponse : ResponseModel
    {
        public List<AltasEjecutivo> lista { get; set; }
    }

    public class ReporteClientesActivosExcelResponse : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<clientesActivos> respuestaLista { get; set; }
    }
    public class ReporteAltasEjecutivoExcelResponse : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<AltasEjecutivo> respuestaLista { get; set; }
    }
    public class ReporteMedicionDesempenoResponse : ResponseModel
    {
        public List<medicionDesempeno> lista { get; set; }
    }
    public class ReporteMedicionDesempenoExcelResponse : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<medicionDesempeno> respuestaLista { get; set; }
    }
    public class MetasMensualesResponse : ResponseModel
    {
        public MetasMensualesModel lista { get; set; }
    }
    public class CanalesMensualesResponse : ResponseModel
    {
        public List<CanalMensualesModel> lista { get; set; }
    }
    public class ReporteCanalesMensualesExcelResponse : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<CanalMensualesModel> respuestaLista { get; set; }
    }
    #endregion

    #endregion

    #region Reporte clientes Familias/Grupos
    #region Model
    public class clientesBusqueda
    {
        public int idCadena { get; set; }
        public string nombreComercial { get; set; }
        public int idEstablecimiento { get; set; }
        public string nombreEstablecimiento { get; set; }
    }

    #endregion

    #region Request Model
    public class ReporteClientesBusquedaRequest
    {
        public int busqueda { get; set; }
    }
    #endregion

    #region Response Model
    public class ReporteClientesBusquedaResponse : ResponseModel
    {
        public List<clientesBusqueda> lista { get; set; }
    }
    public class ReporteClientesBusquedaExcelResponse : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<clientesBusqueda> respuestaLista { get; set; }
    }

    #endregion
    #endregion

    #region Reporte Creditos MO

    #region model

    public class ReportesCreditoModal
    {
        public byte[] FileContents { get; set; }
        public string fileName { get; set; }
    }

    #endregion

    #region RequestModel

    public class ReportesCreditoRequestModel
    {
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public int idCadena { get; set; }
        public int tipoReporte { get; set; }
    }
    public class ReportesDocumentosPMRequestModel
    {
        public string nombre { get; set; }
        public string ext { get; set; }
        public string mimeType { get; set; }
        public string archivo { get; set; }
        public int idCadena { get; set; }
        public int tipoDocumento { get; set; }
    }
    public class UploadFilesRequestModel
    {
        public string clave { get; set; }
        public int tipoDocumento { get; set; }
        public byte[] filesBytes { get; set; }
        public int idCadena { get; set; }
        public string archivo { get; set; } = null;
    }
    #endregion

    #region ResponseModel
    public class ReportesCreditoResponseModel
    {
        public ReportesCreditoModal respuesta { get; set; }
    }
    public class ReportesCreditoResponseModelFile : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
    }
    #endregion

    #endregion
}
