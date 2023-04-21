using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class LeadsModel
    {
    }

    #region Modelos

    public class UsuarioVentas
    {
        public int id { get; set; }
        public string usuario { get; set; }
    }

    public class CatalogoLandingPage
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class CatalogoCanalLead
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class CatalogoObservacion
    {
        public int id { get; set; }
        public string observacion { get; set; }
    }

    public class CatalogoFaseLead
    {
        public int id { get; set; }
        public string fase { get; set; }
    }

    public class CatalogoTarea
    {
        public int id { get; set; }
        public string tarea { get; set; }
    }

    public class ListadoLeads
    {
        public int Id { get; set; }
        public int Vencida { get; set; }
        public int IdFase { get; set; }
        public string Fase { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string FechaAlta { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Tarea { get; set; }
        public string Observaciones { get; set; }
        public string EstatusTarea { get; set; }
        public string FechaUltimoCambio { get; set; }
        public string TipoContacto { get; set; }
        public string HorarioContacto { get; set; }
        public int TotalRows { get; set; }

    }

    public class HistorialTareasLead
    {
        public int id { get; set; }
        public string tarea { get; set; }
        public string fechaRecordatorio { get; set; }
        public bool estatusTarea { get; set; }
        public string observaciones { get; set; }
    }

    public class ListadoLeadsEjecutivo
    {
        public int id { get; set; }
        public int vencida { get; set; }
        public int idEstatusFase { get; set; }
        public string fase { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string fechaAlta { get; set; }
        public string pagina { get; set; }
        public string canal { get; set; }
        public int idUsuario { get; set; }
        public string mensaje { get; set; }
        public string observaciones { get; set; }
        public string tarea { get; set; }
        public string tareaFechaProgramada { get; set; }
        public string estatusTarea { get; set; }
        public int idCatalogoObservacion { get; set; }
        public string observacion { get; set; }
        public int idCadena { get; set; }
        public string giroComercial { get; set; }
        public string tipoContacto { get; set; }
        public string HorarioContacto { get; set; }
    }

    public class ListaCorreos
    {
        public int id { get; set; }
        public string tipoCorreo { get; set; }
    }

    public class Estado
    {
        public int id { get; set; } = 0;
        public string nombre { get; set; } = string.Empty;
    }

    public class Ciudad
    {
        public int idEstado { get; set; } = 0;
        public int idCiudad { get; set; } = 0;
        public string nombreCiudad { get; set; } = string.Empty;
    }

    public class CodigoPostal
    {
        public string codigoPostal { get; set; }
    }

    public class DatosPrecadena
    {
        public string nombreComercial { get; set; }
        public string razonSocial { get; set; }
        public string rfc { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoEstado { get; set; }
        public int idCatalogoCiudad { get; set; }
    }

    public class DatosCadena
    {
        public string nombreComercial { get; set; }
        public string telefono1 { get; set; }
        public string email { get; set; }
        public string colonia { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoEstado { get; set; }
        public int idCatalogoCiudad { get; set; }
    }
    
    public class DatosEstablecimiento
    {
        public string nombre { get; set; }
        public string telefono1 { get; set; }
        public string email { get; set; }
        public string colonia { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string codigoPostal { get; set; }
        public int idCatalogoCiudad { get; set; }
        public string encargado { get; set; }
    }

    public class ListaCajerosEstablecimiento
    {
        public string num { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string resetClave { get; set; }
        public string id { get; set; }
        public string administrador { get; set; }
    }

    public class TipoTerminales
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class DatosLeadsPorFase
    {
        public string fase { get; set; }
        public int leads { get; set; }
    }

    public class DatosLeadsConversion
    {
        public string titulo { get; set; }
        public int leads { get; set; }
    }

    public class DatosLeadsCancelados
    {
        public string observacion { get; set; }
        public int leads { get; set; }
    }

    public class DatosLeadsAltas
    {
        public string idCadena { get; set; }
        public string ejecutivo { get; set; }
        public string fechaAlta { get; set; }
        public string primerDeposito { get; set; }
        public string venta { get; set; }
        public string saldo { get; set; }
    }

    public class DatosLeadsEjecutivos
    {
        public string ejecutivo { get; set; }
        public string totalClientesActivos { get; set; }
        public string totalClientesVentaMayor500 { get; set; }
        public string sumaVentaClientes { get; set; }
    }

    public class DatosLeadsCanal
    {
        public string canal { get; set; }
        public int leads { get; set; }
    }

    public class DatosLeadsVentasMensuales
    {
        public string mes { get; set; }
        public string leads { get; set; }
    }

    public class DatosLeadsVentasAcumulado
    {
        public string mes { get; set; }
        public string leads { get; set; }
    }

    public class ListadoReporteConcentrado
    {
        public string idLead { get; set; }
        public string idCadena { get; set; }
        public string idRecomendado { get; set; }
        public string canal { get; set; }
        public string fase { get; set; }
        public string cliente { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string fechaRegistro { get; set; }
        public string ejecutivo { get; set; }
        public string observaciones { get; set; }
        public string mensaje { get; set; }
        public string comentario { get; set; }
        public string tarea { get; set; }
        public string estatusTarea { get; set; }
        public string descripcion { get; set; }
        public string fechaConcluido { get; set; }
        public string fechaUltimoCambio { get; set; }
        public string giro { get; set; }
        public string tipoContacto { get; set; }
        public string horarioContacto { get; set; }

    }

    public class TipoEstablecimiento
    {
        public int id { get; set; } = 0;
        public string label { get; set; } = string.Empty;
    }

    public class UsuarioRolModel
    {
        public int id { get; set; } = 0;
        public string nombre { get; set; } = string.Empty;
        public string usuario { get; set; } = string.Empty;
    }
    public class TiposContactos
    {
        public int id { get; set; }
        public string concepto { get; set; }
    }
    public class ConceptoTiposContactos
    {
        public int id { get; set; }
        public string concepto { get; set; }
        public int idTipoContacto { get; set; }
    }

    #endregion Modelos

    #region RequestModels

    public class LeadsRequestListadoLeads
    {

        public int IdLead { get; set; } = 0;
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int IdFase { get; set; } = 0;
        public int IdUsuario { get; set; } = 0;
        public string FechaInicio { get; set; } = string.Empty;
        public string FechaFin { get; set; } = string.Empty;
        public int IdCadena { get; set; } = 0;
        public int FiltroTarea { get; set; } = 0;
        public bool PrimeraCarga { get; set; } = false;
        public int IdCanal { get; set; } = 0;
        public int Pagina { get; set; } = 1;
    }

    public class LeadsRequestHistorialTareas
    {
        public int idLead { get; set; } = 0;
    }

    public class LeadsRequestActualizaUsuarioLead
    {
        public int idLead { get; set; } = 0;
        public int idUsuario { get; set; } = 0;
    }

    public class LeadsRequestCrearLead
    {
        public string nombre { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;
        public int idUsuario { get; set; } = 0;
        public int idCatalogoLandingPage { get; set; } = 0;
        public int idCatalogoCanalLead { get; set; } = 0;
        public string mensaje { get; set; } = string.Empty;
        public int mtcid { get; set; } = 0;
    }

    public class LeadsRequestExisteLead
    {
        public string telefono { get; set; }
        public string correo { get; set; }
    }

    public class LeadsRequestEnviaCorreo
    {
        public int idMenu { get; set; } = 0;
        public int idLead { get; set; } = 0;
    }

    public class LeadsRequestAgregaTarea
    {
        public int idLead { get; set; } = 0;
        public int idUsuario { get; set; } = 0;
        public int idTarea { get; set; } = 0;
        public string fechaRecordatorio { get; set; } = string.Empty;
        public string horaRecordatorio { get; set; } = string.Empty;
        public string observaciones { get; set; } = string.Empty;
    }

    public class LeadsRequestActualizaTarea
    {
        public int idSeguimientoLead { get; set; } = 0;
        public int idLead { get; set; } = 0;
        public int statusTarea { get; set; } = 0;
        public string observaciones { get; set; } = string.Empty;
    }

    public class LeadsRequestCancelaLead
    {
        public int idFase { get; set; } = 6;
        public int idLead { get; set; } = 0;
        public int idCatalogoObservacion { get; set; } = 0;
    }

    public class LeadsRequestActualizaFase
    {
        public int idEstatusFase { get; set; }
        public int idLead { get; set; }
    }

    public class LeadsRequestReactivarLead
    {
        public int idLead { get; set; } = 0;
        public string correo { get; set; } = string.Empty;
    }

    public class LeadsRequestProcesoAltaSistema
    {
        public int idLead { get; set; }
        public string correo { get; set; }
    }

    public class LeadsRequestCrearPrecadena
    {
        public int idLead { set; get; }
        public string contacto { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
    }

    public class LeadsRequestGetCiudad
    {
        public string codigoPostal { get; set; }
    }

    public class LeadsRequestDatosPrecadena
    {
        public string correo { get; set; }
    }

    public class LeadsRequestDatosCadena
    {
        public int idCadena { get; set; }
    }

    public class LeadsRequestActualizaPrecadena
    {
        public string idCorreo { get; set; }
        public string nombreComercial { get; set; }
        public string razonSocial { get; set; }
        public string rfc { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public int idCiudad { get; set; }
        public string telefono { get; set; } = string.Empty;
    }

    public class LeadsRequestCrearCadena
    {
        public string email { get; set; }
        public int idLead { get; set; }
    }

    public class LeadsRequestCrearEstablecimiento
    {
        public string idCadena { get; set; }
        public string nombre { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public string codigoPostal { get; set; }
        public string idCatalogoCiudad { get; set; }
        public string telefono { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public string encargado { get; set; }
        public int giroEstablecimiento { get; set; }
    }

    public class LeadsRequestDatosEstablecimiento
    {
        public int idCadena { get; set; }
    }

    public class LeadsRequestCajerosLista
    {
        public int idEstablecimiento { get; set; }
    }

    public class LeadsRequestCrearCajero
    {
        public int idEstablecimiento { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellidoPaterno { get; set; } = string.Empty;
        public string apellidoMaterno { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string telefono1 { get; set; } = string.Empty;
        public bool esAdministrador { get; set; } = false;
    }

    public class LeadsRequestIdEstablecimiento
    {
        public int idCadena { get; set; }
    }

    public class LeadsRequestEliminarCajero
    {
        public int idCajero { get; set; }
    }

    public class LeadsRequestNuevaTerminal
    {
        public int idEstablecimiento { get; set; } = 0;
        public long numero { get; set; } = 0;
        public bool appMovil { get; set; } = false;
        public int tipoUsuario { get; set; } = 2;
        public int idUsuario { get; set; } = 0;
        public string ip { get; set; } = string.Empty;
        public int idTipoTerminal { get; set; } = 0;
        public string version { get; set; } = string.Empty;
    }

    public class LeadsRequestDatosReportes
    {
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public int menu { get; set; }
        public int idUsuario { get; set; }
    }

    public class LeadsRequestMigrarLeads
    {
        public int idUsuarioOrigen { get; set; }
        public int idUsuarioDestino { get; set; }
    }

    public class LeadsRequestReporteConcentrado
    {
        public string fechaDesde { get; set; }
        public string fechaHasta { get; set; }
    }
    public class LeadsRequestEstatusFase
    {
        public int idEstatusFase { get; set; }
    }

    public class LeadsRequestAsignarLeads: ResponseModel
    {
        public string nombre { get; set; }
        public string ext { get; set; }
        public string mimeType { get; set; }
        public string archivo { get; set; }
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class LeadsRequestListaTipoRol
    {
        public int tipo { get; set; }
    }

    public class LeadsRequestGruposTrabajo
    {
        public int idLider { get; set; }
        public string idsEjecutivos { get; set; }
    }

    public class LeadsRequestGruposTrabajoCoordinador
    {
        public int idCoordinador { get; set; }
    }

    public class LeadsRequestConceptosTiposContacto: ResponseModel
    {
        public int idTipoContacto { get; set; }
    }
    public class LeadsRequestTipoContacto
    {
        public int id { get; set; }
        public int idTipoContacto { get; set; }
        public int idConceptoContacto { get; set; }
    }
    #endregion

    #region ResponseModels

    public class LeadsResponseListaUsuarios : ResponseModel
    {
        public List<UsuarioVentas> listaUsuarioVentas { get; set; }
    }

    public class LeadsResponseCatalogoLandingPage : ResponseModel
    {
        public List<CatalogoLandingPage> listaCatalogoLandingPage { get; set; }
    }

    public class LeadsResponseCatalogoCanalLead : ResponseModel
    {
        public List<CatalogoCanalLead> listaCatalogoCanalLead { get; set; }
    }

    public class LeadsResponseCatalogoObservacion : ResponseModel
    {
        public List<CatalogoObservacion> listaCatalogoObservacion { get; set; }
    }

    public class LeadsResponseCatalogoFaseLead : ResponseModel
    {
        public List<CatalogoFaseLead> listaCatalogoFaseLead { get; set; }
    }

    public class LeadsResponseCatalogoTareas : ResponseModel
    {
        public List<CatalogoTarea> listaCatalogoTareas { get; set; }
    }

    public class LeadsResponseListadoLeads : ResponseModel
    {
        public List<ListadoLeads> listaLeads { get; set; }
    }

    public class LeadsResponseHistorialTareas : ResponseModel
    {
        public List<HistorialTareasLead> listaHistorialTareas { get; set; }
    }

    public class LeadsResponseListadoLeadsEjecutivo : ResponseModel
    {
        public List<ListadoLeadsEjecutivo> listaLeads { get; set; }
    }

    public class LeadsResponseListadoCorreos : ResponseModel
    {
        public List<ListaCorreos> listaCorreos { get; set; }
    }

    public class LeadsResponseAgregaTarea : ResponseModel
    {
        public int idTarea { get; set; }
    }

    public class LeadsResponseProcesoAltaSistema : ResponseModel
    {
        public int paso { get; set; }
    }

    public class LeadsResponseCrearPrecadena : ResponseModel
    {
        public int verificacion { get; set; }
    }

    public class LeadsResponseProcesoGetEstado : ResponseModel
    {
        public List<Estado> listaEstados { get; set; }
    }

    public class LeadsResponseProcesoGetCiudad : ResponseModel
    {
        public Ciudad ciudad { get; set; }
    }

    public class LeadsResponseProcesoDatosPrecadena : ResponseModel
    {
        public DatosPrecadena datosPrecadena { get; set; }
    }

    public class LeadsResponseProcesoDatosCadena : ResponseModel
    {
        public DatosCadena datosCadena { get; set; }
    }

    public class LeadsResponseCrearCadena : ResponseModel
    {
        public int idCadena { get; set; }
    }

    public class LeadsResponseCrearEstablecimiento : ResponseModel
    {
        public int idEstablecimiento { get; set; }
    }

    public class LeadsResponseDatosEstablecimiento : ResponseModel
    {
        public DatosEstablecimiento datosEstablecimiento { get; set; }
    }

    public class LeadsResponseListaCajerosEstablecimiento : ResponseModel
    {
        public List<ListaCajerosEstablecimiento> listaCajeros { get; set; }
    }

    public class LeadsResponseCrearCajero : ResponseModel
    {
        public int creado { get; set; }
    }

    public class LeadsResponseIdEstablecimiento : ResponseModel
    {
        public int idEstablecimiento { get; set; }
    }

    public class LeadsResponseEliminarCajero : ResponseModel
    {
        public int eliminado { get; set; }
    }

    public class LeadsResponseExisteLead : ResponseModel
    {
        public string existeLead { get; set; }
    }

    public class LeadsResponseAltaTerminal : ResponseModel
    {
        public int idTerminal { get; set; }
    }

    public class LeadsResponseListaTerminales : ResponseModel
    {
        public List<TipoTerminales> listaTerminales { get; set; }
    }

    public class LeadsResponseReporteLeadsFase : ResponseModel
    {
        public List<DatosLeadsPorFase> leadsFase { get; set; }
    }

    public class LeadsResponseReporteConversion : ResponseModel
    {
        public List<DatosLeadsConversion> reporteConversion { get; set; }
    }

    public class LeadsResponseReporteCancelados : ResponseModel
    {
        public List<DatosLeadsCancelados> reporteCancelados { get; set; }
    }

    public class LeadsResponseReporteLeadsAlta : ResponseModel
    {
        public List<DatosLeadsAltas> reporteLeadsAltas { get; set; }
    }

    public class LeadsResponseReporteLeadsEjecutivo : ResponseModel
    {
        public List<DatosLeadsEjecutivos> reporteLeadsEjecutivo { get; set; }
    }

    public class LeadsResponseReporteCanales : ResponseModel
    {
        public List<DatosLeadsCanal> reporteCanales { get; set; }
    }

    public class LeadsResponseReporteLeadsVentasMensuales : ResponseModel
    {
        public List<DatosLeadsVentasMensuales> reporteLeadsVentasMensuales { get; set; }
    }

    public class LeadsResponseReporteLeadsVentasAcumulado : ResponseModel
    {
        public List<DatosLeadsVentasAcumulado> getReporteLeadsVentasAcumulado { get; set; }
    }

    public class LeadsResponseReporteLeadExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }

    public class LeadsResponseMigrarLeads : ResponseModel
    {
        public int migrado { get; set; }
    }

    public class LeadsResponseReporteConcentrado : ResponseModel
    {
        public List<ListadoReporteConcentrado> reporteConcentrado { get; set; }
    }

    public class LeadsResponseCrearLead : ResponseModel
    {
        public int idUsuario { get; set; }
    }
    public class ResponseTipoEstablecimientos : ResponseModel
    {
        public List<TipoEstablecimiento> tipos { get; set; }
    }
    public class ResponseUsuarioRolModel : ResponseModel
    {
        public List<UsuarioRolModel> respuesta { get; set; }
    }
    public class ResponseFaseAnteriorLead : ResponseModel
    {
        public int fase { get; set; }
    }
    public class ResponseTiposContactos : ResponseModel
    {
        public List<TiposContactos> respuesta { get; set; }
    }
    public class ResponseConceptoTiposContactos : ResponseModel
    {
        public List<ConceptoTiposContactos> respuesta { get; set; }
    }
    #endregion ResponseModels
}
