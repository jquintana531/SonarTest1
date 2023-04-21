using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class CalidadModel
    {
    }


    #region Lista Seguimiento

    #region Modelos

    public class CalidadSeguimiento
    {
        public int id { get; set; }
        public int mtcid { get; set; }
        public int idEstatus { get; set; }
        public int idRazones { get; set; }
        public string razon { get; set; }
        public decimal comisionRecarga { get; set; }
        public string grupo { get; set; }
        public string fechaAlta { get; set; }
        public string fechaInclusion { get; set; }
        public string contacto { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public string activo { get; set; }
        public string motivo { get; set; }
        public string oficina { get; set; }
        public string promotor { get; set; }
        public string ventasUltimos3Meses { get; set; }
        public string estatus { get; set; }
        public string observaciones { get; set; }
        public string color { get; set; }
        public string fechaTarea { get; set; }
        public string observacionesTarea { get; set; }
        public string asignado { get; set; }
        public int incremento { get; set; }
        public int idMotivoSeguimiento { get; set; }

    }
    public class CalidadHistorialListaSeguimiento
    {
        public string fechaHora { get; set; }
        public int idEstatus { get; set; }
        public string estatus { get; set; }
        public string motivo { get; set; }
        public string razon { get; set; }
    }
    public class CalidadListadoTareasSeguimiento
    {
        public int id { get; set; }
        public string tarea { get; set; }
        public string fechaRecordatorio { get; set; }
        public bool estatusTarea { get; set; }
        public string observaciones { get; set; }
    }
    public class CalidadHistorialVenta
    {
        public string mes { get; set; }
        public string monto { get; set; }
        public int num { get; set; }
    }
    public class CalidadModelReporte
    {
        public string label { get; set; }
        public int contador { get; set; }
    }

    #endregion Modelos

    #region RequestModels

    public class CalidadRequestListaSeguimiento
    {
        public int idCadena { get; set; }
        public int idEstatus { get; set; }
        public string correo { get; set; }
        public string nombre { get; set; }
        public string fechaIni { get; set; }
        public string fechaFin { get; set; }
        public int orden { get; set; }
        public int activo { get; set; }
        public int idMotivo { get; set; }
        public int incremento { get; set; }
        public bool firstTime { get; set; }

    }
    public class CalidadRequestCambioEstatusComision
    {
        public int idListaSeguimiento { get; set; }
        public int idEstatus { get; set; }
        public int idUsuario { get; set; }
    }
    public class CalidadRequestCambioEstatus
    {
        public int idListaSeguimiento { get; set; }
        public int idEstatus { get; set; }
        public int idRazon { get; set; }
        public int idUsuario { get; set; }
    }
    public class CalidadRequestHistorialListaSeguimiento
    {
        public int idCadena { get; set; }
    }
    public class CalidadRequestEnviaCorreo
    {
        public int idListaSeguimiento { get; set; }
        public int tipoCorreo { get; set; }
        public int idUsuario { get; set; }
    }
    public class CalidadRequestListadoTareas
    {
        public int idListaSeguimiento { get; set; }
    }
    public class CalidadRequestActualizaTarea
    {
        public int idtareasListaSeguimiento { get; set; }
        public int idEstatusTarea { get; set; }
        public int idUsuario { get; set; }
        public string observaciones { get; set; }
    }
    public class CalidadRequestNuevaTarea
    {
        public int idListaSeguimiento { get; set; }
        public int idUsuario { get; set; }
        public int idTarea { get; set; }
        public DateTime fechaRecordatorio { get; set; }
        public string observaciones { get; set; }

    }
    public class CalidadRequestHistorialVenta
    {
        public int idCadena { get; set; }
    }
    public class CalidadRequestAgregarCadena
    {
        public int idCadena { get; set; }
        public int idMotivo { get; set; }
        public int idUsuario { get; set; }
    }
    public class CalidadRequestReporteSeguimiento
    {
        public int idReporte { get; set; }
        public int idMotivo { get; set; }
        public string fechaIni { get; set; }
        public string fechaFin { get; set; }
    }
    public class CalidadSeguimientoExport
    {
        public int mtcid { get; set; }
        public string razon { get; set; }
        public decimal comisionRecarga { get; set; }
        public string grupo { get; set; }
        public string fechaAlta { get; set; }
        public string fechaInclusion { get; set; }
        public string contacto { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public string activo { get; set; }
        public string motivo { get; set; }
        public string oficina { get; set; }
        public string promotor { get; set; }
        public string estatus { get; set; }
        public string observaciones { get; set; }
        public string fechaTarea { get; set; }
        public string observacionesTarea { get; set; }
        public string asignado { get; set; }
        public int incremento { get; set; }
        public decimal mes1 { get; set; }
        public decimal mes2 { get; set; }
        public decimal mes3 { get; set; }
        public decimal mes4 { get; set; }

    }


    #endregion RequestModels

    #region ResponseModels

    public class CalidadResponseListadoEstatusSeguimiento : ResponseModel
    {
        public List<ListadoLabel> listadoEstatusSeguimiento { get; set; }
    }
    public class CalidadResponseListadoMotivoSeguimiento : ResponseModel
    {
        public List<ListadoLabel> listadoMotivoSeguimiento { get; set; }
    }
    public class CalidadRespponseListaSeguimiento : ResponseModel
    {
        public List<CalidadSeguimiento> listadoSeguimiento { get; set; }
    }
    public class CalidadResponseListadoPerdidaSeguimiento : ResponseModel
    {
        public List<ListadoLabel> listadoPerdida { get; set; }
    }
    public class CalidadResponseEstatusIncremento : ResponseModel
    {
        public bool cambiado { get; set; }
    }
    public class CalidadResponseCambioEstatus : ResponseModel
    {
        public bool cambiado { get; set; }
    }
    public class CalidadResponseListadoHistorialSeguimiento : ResponseModel
    {
        public List<CalidadHistorialListaSeguimiento> listadoHistorial { get; set; }
    }
    public class CalidadResponseListadoEnvioCorreo : ResponseModel
    {
        public List<ListadoLabel> listadoEnvioCorreo { get; set; }
    }
    public class CalidadResponseEnvioCorreo : ResponseModel
    {
        public bool enviado { get; set; }
    }
    public class CalidadResponseListadoTareasSeguimiento : ResponseModel
    {
        public List<CalidadListadoTareasSeguimiento> listadoTareasSeguimiento { get; set; }
    }
    public class CalidadResponseActualizaEstadoTarea : ResponseModel
    {
        public bool actualizado { get; set; }
    }
    public class CalidadResponseNuevaTarea : ResponseModel
    {
        public bool agregada { get; set; }
    }
    public class CalidadResponseListadoFasesTarea : ResponseModel
    {
        public List<ListadoLabel> listadoFasesTarea { get; set; }
    }
    public class CalidadResponseHistorialVenta : ResponseModel
    {
        public List<CalidadHistorialVenta> listadoHistorialVenta { get; set; }
    }
    public class CalidadResponseAgregaCadena : ResponseModel
    {
        public bool agregada { get; set; }
    }
    public class CalidadResponseReporte : ResponseModel
    {
        public List<CalidadModelReporte> reporte { get; set; }
    }
    public class CalidadResponseListaSeguimientoExport : ResponseModel
    {
        public List<CalidadSeguimientoExport> listadoSeguimiento { get; set; }
    }
    public class CalidadResponseSeguimientoExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }

    #endregion ResponseModels

    #endregion Lista Seguimiento

}
