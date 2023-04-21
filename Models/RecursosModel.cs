using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class RecursosModel
    {

        #region Buscador Universal

        #region Modelos

        public class RecursosBuscadorUniversalCadena
        {
            public int id { get; set; }
            public string label { get; set; }
        }
        public class RecursosBuscadorUniversalEstablecimiento
        {
            public int idCadena { get; set; }
            public int idEstablecimiento { get; set; }
            public string establecimiento { get; set; }
        }
        public class RecursosBuscadorUniversalCajero
        {
            public int idCadena { get; set; }
            public int idEstablecimiento { get; set; }
            public int idCajero { get; set; }
            public string cajero { get; set; }
            public string btnReset { get; set; }
        }
        public class RecursosBuscadorUniversalTerminal
        {
            public int idCadena { get; set; }
            public int idEstablecimiento { get; set; }
            public string numero { get; set; }
            public DateTime fechaAlta { get; set; }
        }

        public class RecursosBuscadorUniversalRFC
        {
            public int id { get; set; }
            public string nombreComercial { get; set; }
        }

        #endregion

        #region RequestModels
        public class RecursosRequestBuscador : ModelBase
        {
            public int tipo { get; set; }
            public string consulta { get; set; }
        }

        #endregion

        #region ResponseModels
        public class RecursosResponseBuscadorUniversal : ResponseModel
        {
            public Object respuesta { get; set; }
        }

        public class RecursosResponseBuscadorUniversalExcel : ResponseModel
        {
            public string respuesta { get; set; }
            public string reporteNombre { get; set; }
            public Object respuestaLista { get; set; }
        }

        #endregion

        #endregion

        #region Listado Tickets


        #region Modelos
        public class Ticket
        {
            public string id { get; set; }
            public string ticketId { get; set; }
            public string fechaAlta { get; set; }
            public string tipoTicket { get; set; }
            public string medio { get; set; }
            public string asunto { get; set; }
            public string estatus { get; set; }
            public string prioridad { get; set; }
            public string colorPriodidad { get; set; }
            public bool vencido { get; set; }
            public string primerContacto { get; set; }
            public string ultimoContacto { get; set; }
            public string seguimiento { get; set; }
            public string fechaVencimiento { get; set; }

        }
        public class ListadoLabel
        {
            public int id { get; set; }
            public string label { get; set; }
        }        
        public class Cajeros
        {
            public int id { get; set; }
            public string cajero { get; set; }
            public bool activo { get; set; } = false;
        }
        public class DetalleTicket
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
            public string fechaAlta { get; set; }
            public string fechaActualizado { get; set; }
            public string fechaUltimoMensaje { get; set; }
            public int contestado { get; set; }
            public string fechaUltimaRespuesta { get; set; }
            public string fechaCierre { get; set; }
            public string usuarioAsignado { get; set; }
            public int idUsuarioAsignado { get; set; }
            public string vencimiento { get; set; }
            public string origen { get; set; }
            public string ip { get; set; }
            public string medio { get; set; }
        }
        public class TicketConversacion
        {
            public int rowNumber { set; get; }
            public int idConversacion { set; get; }
            public string usuario { set; get; }
            public int idTicket { set; get; }
            public string mensaje { set; get; }
            public string fechaHora { set; get; }
            public string tipo { set; get; }
            public int adjunto { set; get; }
            public string nombreArchivo { set; get; }
            public string hashName { set; get; }
            public string extension { set; get; }
        }
        public class MisTickets
        {
            public string ticketID { get; set; }
            public string tipoTicket { get; set; }
            public string asunto { get; set; }
            public string estatus { get; set; }
            public string medio { get; set; }
            public string atendio { get; set; }
            public string fechaAlta { get; set; }
            public string fechaVencimiento { get; set; }
        }
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
        public class TicketsUsuario
        {
            public string medio { get; set; }
            public string total { get; set; }
        }

        #endregion


        #region RequestModels

        public class RecursosRequestListadoTickets
        {
            public int idUsuario { get; set; }
            public int opcion { get; set; }
            public string busqueda { get; set; }
            public string desde { get; set; }
            public string hasta { get; set; }
            public bool firstTime { get; set; } = false;
        }
        public class RequestCatalogo
        {
            public int id { get; set; }
            public int menu { get; set; }
        }
        public class RequestCatalogoAsunto : RequestCatalogo
        {
            public int idTema { get; set; }
        }
        public class RecursosRequestListadoEstablecimiento
        {
            public int idCadena { get; set; }
            public int activo { get; set; }
            public int menu { get; set; }
        }
        public class RecursosRequestListadoCajeros
        {
            public int idEstablecimiento { get; set; }
            public int idUsuario { get; set; }
            public string ip { get; set; }
        }
        public class RequestNuevoTicket : ModelBase
        {
            public int idTema { get; set; }
            public int idAsunto { get; set; }
            public int idDepartamento { get; set; }
            public int idCadena { get; set; } = 0;
            public int idEstablecimiento { get; set; } = 0;
            public int idCajero { get; set; } = 0;
            public string nombre { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
            public string telefono { get; set; } = string.Empty;
            public int idOrigen { get; set; } = 2;
            public int idViaContacto { get; set; }
            public int idPrioridad { get; set; }
            public string mensaje { get; set; }
            public int idUsuario { get; set; } = 0;
            public int tipoTicket { get; set; } = 0;
            public bool cerrarTicket { get; set; } = false;
            public int idUsuarioAux { get; set; } = 0;
        }
        public class RequestAsignarTicket : ModelBase
        {
            public string ticketID { get; set; }
            public int idUsuarioAsignado { get; set; }
            public int idUsuario { get; set; }
        }
        public class RequestCambioEstatusTicket : ModelBase
        {
            public long ticketID { get; set; }
            public int idCatalogoStatusTicket { get; set; }
            public int idUsuario { get; set; }
        }
        public class RequestDetalleTicket : ModelBase
        {
            public int ticketID { get; set; }
        }
        public class RequestCambioPrioridadTicket : ModelBase
        {
            public int ticketID { get; set; }
            public int idPrioridad { get; set; }
            public int idUsuario { get; set; }
        }
        public class RequestCambioVencido : ModelBase
        {
            public int ticketID { get; set; }
            public bool vencido { get; set; }
            public int idUsuario { get; set; }
        }
        public class RequestLiberarTicket : ModelBase
        {
            public int ticketID { get; set; }
            public int idUsuarioAsignado { get; set; }
            public int idUsuario { get; set; }
        }
        public class RequestUsuarioDepartamento : ModelBase
        {
            public int idUsuario { get; set; }
        }
        public class RequestTransferirTicket : ModelBase
        {
            public int ticketID { get; set; }
            public int idDepartamento { get; set; }
            public int idUsuario { get; set; }
        }
        public class RequestNuevaRespuestaAux : ModelBase
        {
            public int ticketID { get; set; }
            public string mensaje { get; set; }
            public string titulo { get; set; }
            public int tipo { get; set; }
            public int idUsuario { get; set; }

        }
        public class RequestNuevaRespuesta : ModelBase
        {
            public int ticketID { get; set; }
            public string mensaje { get; set; }
            public string titulo { get; set; }
            public int tipo { get; set; }
            public int idUsuario { get; set; }

            public Archivo file { get; set; }

            public class Archivo
            {
                public string nombre { get; set; }
                public string ext { get; set; }
                public string mimeType { get; set; }
                public string archivo { get; set; }
                public string size { get; set; } 
            }

        }
        public class RequestNuevaNotaTicket : ModelBase
        {
            public int ticketID { get; set; }
            public string mensaje { get; set; }
            public string titulo { get; set; }
            public int idUsuario { get; set; }
        }
        public class RequestNuevaRespuestaTicket : ModelBase
        {
            public int ticketID { get; set; }
            public string mensaje { get; set; }
            public int idUsuario { get; set; }
        }
        public class RequestNuevoTicketAdjunto : ModelBase
        {
            public int ticketID { get; set; }
            public int idConversacion { get; set; }
            public string tipoConversacion { get; set; }
            public string tipoArchivo { get; set; }
            public string tamano { get; set; }
            public string nombre { get; set; }
            public string hashName { get; set; }
        }
        public class RequestMisTickets
        {
            public int idUsuario { get; set; }
            public string desde { get; set; }
            public string hasta { get; set; }
            public int status { get; set; }
        }
        public class RequestHistorial
        {
            public int idUsuario { get; set; }
            public string desde { get; set; }
            public string hasta { get; set; }
            public int status { get; set; }
            public string idCadena { get; set; }
            public string ticket { get; set; } = string.Empty;
        }
        public class RequestTicketUsuario
        {
            public int idUsuario { get; set; }
            public string desde { get; set; }
            public string hasta { get; set; }
            public int idUsuarioAdministrador { get; set; }
        }
        #endregion|


        #region ResponseModels

        public class RecursosResponseListadoTickets : ResponseModel
        {
            public List<Ticket> listadoTickets { get; set; }
        }

        public class RecursosResponseListadoTemas : ResponseModel
        {
            public List<ListadoLabel> listadoTemas { get; set; }
        }
        public class RecursosResponseListadoAsuntos : ResponseModel
        {
            public List<ListadoLabel> listadoAsuntos { get; set; }
        }
        public class RecursosResponseListadoDepartamentos : ResponseModel
        {
            public List<ListadoLabel> listadoDepartamentos { get; set; }
        }
        public class RecursosResponseListadoContacto : ResponseModel
        {
            public List<ListadoLabel> listadoContacto { get; set; }
        }
        public class RecursosResponseListadoPrioridades : ResponseModel
        {
            public List<ListadoLabel> listadoPrioridades { get; set; }
        }
        public class RecursosResponseAgregarTicket : ResponseModel
        {
            public long idTicket { get; set; }
        }
        public class RecursosResponseListadoEstablecimientos : ResponseModel
        {
            public List<ListadoLabel> listadoEstablecimientos { get; set; }
        }
        public class RecursosResponseListadoCajeros : ResponseModel
        {
            public List<Cajeros> listadoCajeros { get; set; }
        }
        public class RecursosResponseCambioEstatusTicket : ResponseModel
        {
            public int resultado { get; set; }
        }
        public class RecursosReponseDetalleTicket : ResponseModel
        {
            public DetalleTicket ticket { get; set; }
        }
        public class RecursosResponseConversacionTicket : ResponseModel
        {
            public List<TicketConversacion> listaConversacion { get; set; }
        }
        public class RecursosResponseCatalogoPrioridad : ResponseModel
        {
            public List<ListadoLabel> listadoPrioridad { get; set; }
        }
        public class RecursosResponseCambioPrioridadTicket: ResponseModel
        {
            public int resultado { get; set; }
        }
        public class RecursosResponseCambioVencido : ResponseModel
        {
            public int resultado { get; set; }
        }
        public class RecursosResponseLiberarTicketModel:ResponseModel
        {
            public int resultado { get; set; }
        }
        public class RecursosResponseListaUsuarios : ResponseModel
        {
            public List<ListadoLabel> listaUsuarios { get; set; }
        }
        public class RecursosResponseAsignarTicket : ResponseModel
        {
            public int resultado { get; set; }
        }
        public class RecursosResponseTransferirTicket : ResponseModel
        {
            public int resultado { get; set; }
        }
        public class RecursosResponseRespuestaTicket : ResponseModel
        {
            public int resultado { get; set; }
        }
        public class RecursosResponseTicketAdjunto : ResponseModel
        {
            public int resultado { get; set; }
        }
        public class RecursossResponseMisTickets : ResponseModel
        {
            public List<MisTickets> listadoMisTickets { get; set; }
        }
        public class RecursosResponseListadoEstatusTickets : ResponseModel
        {
            public List<ListadoLabel> listadoEstatusTickets { get; set; }
        }
        public class RecursosResponseReporteMisTicketsExcel : ResponseModel
        {
            public string reporteExcel { get; set; }
            public string reporteNombre { get; set; }
        }
        public class RecursosResponseHistorialTickets : ResponseModel
        {
            public List<HistorialTickets> listadoHistorial { get; set; }
        }
        public class RecursosResponseReporteHistorialExcel : ResponseModel
        {
            public string reporteExcel { get; set; }
            public string reporteNombre { get; set; }
        }
        public class RecursosResponseTicketsUsuario : ResponseModel
        {
            public List<TicketsUsuario> listadoTicketsUsuario { get; set; }
        }
        public class RecursosResponseReporteTicketsUsuarioExcel : ResponseModel
        {
            public string reporteExcel { get; set; }
            public string reporteNombre { get; set; }
        }

        public class RecursosResponseTicketVisor
        {
            public string dir { get; set; }
        }

        #endregion


        #endregion
    }
}
