using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    
    public class FinanzasBCFDIModel
    {
        public int id { get; set; }
        public int idLoteBE { get; set; }
        public DateTime fechaEmision { get; set; }
        public String RFC { get; set; }
        public String razonSocial { get; set; }
        public decimal subTotal { get; set; }
        public decimal iva { get; set; }
        public decimal total { get; set; }
        public String tipoComprobante { get; set; }
        public String folio_serie { get; set; }
        public String uuid { get; set; }
        public String concepto { get; set; }
        public decimal tasaIVA { get; set; }
        public String cancelado { get; set; }
        public String observacionesGrales { get; set; }
        public String MetodoDePago { get; set; }
        public String documentoRelacionado { get; set; }
    }

    public class FinanzasRequestBCFDIModel : ModelBase
    {
        public string rfc { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public string tipoComprobante { get; set; }
        public string folio { get; set; }
    }
    public class FinanzasBCFDIResponse : ResponseModel
    {
        public List<FinanzasBCFDIModel> finanzas { get; set; }

    }
    public class FinanzasBCFDIResponseExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<FinanzasBCFDIModel> finanzas { get; set; }

    }
    public class FinanzasRequestCancelarFactura : ModelBase
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
    }
    public class FinanzasCancelarFacturaResponse : ResponseModel
    {
        public Boolean respuesta { get; set; }
    }
    public class FinanzasRequestMostarXSLSDocModel : ModelBase
    {
        public int id { get; set; }
    }
    public class FinanzastMostarXSLSDocDResponse : ResponseModel
    {
        public string respuesta { get; set; }
        public FinanzastDocDResponse respuestaXML { get; set; }
        public FinanzastDocDResponse respuestaPDF { get; set; }
    }
    public class FinanzastDocDResponse 
    {
        public string nombre { get; set; }
        public string baseURI { get; set; }
        public string base64 { get; set; }
    }
    public class FinanzasBCFDIGModel
    {
        public int id { get; set; }
        public String razonSocial { get; set; }
        public String tipoComprobante { get; set; }
        public String observaciones { get; set; }
        public String archivoOriginal { get; set; }
        public DateTime fechaModificacion { get; set; }
    }
    public class FinanzasRequestBCFDIGModel : ModelBase
    {
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public string folio { get; set; }
    }
    public class FinanzasBCFDIGResponseModel : ResponseModel
    {
        public List<FinanzasBCFDIGModel> lista { get; set; }

    }
    public class FinanzasRequestCFDIPrevisualizarModel : ModelBase
    {
        public FinanzasGenericPrevisualizar cliente { get; set; }
        public FinanzasGenericPrevisualizar documento { get; set; }
        public FinanzasGenericPrevisualizar sucursal { get; set; }
        public FinanzasGenericPrevisualizar usoCFDI { get; set; }
        public FinanzasGenericPrevisualizar metodoPago { get; set; }
        public FinanzasGenericPrevisualizar formaPago { get; set; }
        public string obervaciones { get; set; }
        public List<FinanzasProductoCFDI_P> productos { get; set; }
    }
    public class FinanzasGenericPrevisualizar
    {
        public int id { get; set; }
        public string label { get; set; }
        public string label_opt { get; set; }
    }
    public class FinanzasProductoCFDI_P
    {
        public string descProducto { get; set; }
        public string descUnidad { get; set; }
        public int cantidad { get; set; }
        public float total { get; set; }
        public float importe { get; set; }
        public string descClavePS { get; set; }
        public string descClaveU { get; set; }
    }
    public class FinanzastCFDIPrevizualisarDResponse : ResponseModel
    {
        public FinanzastDocDResponse respuestaPDF { get; set; }
    }
    public class FinanzasRequestCFDIGenericoFacturaModel : ModelBase
    {
        public int idUsuario { get; set; }
        public FinanzasGenericPrevisualizar cliente { get; set; }
        public FinanzasGenericPrevisualizar documento { get; set; }
        public FinanzasGenericPrevisualizar sucursal { get; set; }
        public FinanzasGenericPrevisualizar usoCFDI { get; set; }
        public FinanzasGenericPrevisualizar metodoPago { get; set; }
        public FinanzasGenericPrevisualizar formaPago { get; set; }
        public string obervaciones { get; set; }
        public string uuid { get; set; }
        public List<FinanzasProductoCFDI> productos { get; set; }
    }
    public class FinanzasRequestRefacturarModel : ModelBase
    {
        public int idUsuario { get; set; }
        public int idCliente { get; set; }
        public int idFactura { get; set; }
    }
    public class FinanzasProductoCFDI
    {
        public int idProducto { get; set; }
        public string descProducto { get; set; }
        public int idUnidad { get; set; }
        public string descUnidad { get; set; }
        public int idClavePS { get; set; }
        public string descClavePS { get; set; }
        public int idClaveU { get; set; }
        public string descClaveU { get; set; }
        public int cantidad { get; set; }
        public float total { get; set; }
        public float importe { get; set; }
    }
    public class FinanzastCFDIGenericoFacturaResponse : ResponseModel
    {
        public Boolean respuesta { get; set; }
    }

    //Catalogos
    public class FinanzasGTDResponseModel : ResponseModel
    {
        public DataTable lista { get; set; }
    }

    //Catalogo Response Docs
    public class FinanzasRDocsResponseModel : ResponseModel
    {
        public DataTable objTiposDocumento { get; set; }
        public DataTable objSucursales { get; set; }
        public DataTable objUsoCFDI { get; set; }
        public DataTable objMetodosPago { get; set; }
        public DataTable objFormasPago { get; set; }
        public DataTable objClientes { get; set; }
        public DataTable objProductos { get; set; }
        public DataTable objClaveUnidad { get; set; }
        public DataTable objUnidadades { get; set; }
        public DataTable objCCProdServ { get; set; }
    }

    public class FinanzasRequestCClientesModel : ModelBase
    {
        public string busqueda { get; set; }
        public int opcion { get; set; }
    }


    #region CFDI Montos
    public class FinanzasRequestCFDICadenaMontos : ModelBase
    {
        public int idCadena { get; set; }
    }

    public class FinanzasCadenaResponse : ResponseModel
    {
        public FinanzasCadenaCFDIMontos cadena { get; set; }
    }

    public class FinanzasCadenaCFDIMontos
    {
        public string nombreComercial { get; set; }
            
    }



    public class FinanzasBCFDIMontosModel
    {
        public int id { get; set; }
        public int idCadena { get; set; }
        public String nombreComercial { get; set; }
        public decimal monto { get; set; }
        public DateTime periodo { get; set; }
    }

    public class FinanzasBCFDIMontosResponse : ResponseModel
    {
        public List<FinanzasBCFDIMontosModel> lista { get; set; }
    }


    public class FinanzasRequestCFDIMontosAdd : ModelBase
    {
        public int idCadena { get; set; }
        public decimal monto { get; set; }
        public DateTime periodo { get; set; }
        public int idUsuario { get; set; }
    }

    public class FinanzasCFDIMontosAddResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }

    public class FinanzasRequestCFDIMontosEdit : ModelBase
    {
        public int id { get; set; }
        public decimal monto { get; set; }
        public DateTime periodo { get; set; }
        public int idUsuario { get; set; }
    }

    public class FinanzasCFDIMontosEditResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }

    public class FinanzasRequestCFDIMontosDelete : ModelBase
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
    }

    public class FinanzasCFDIMontosDeleteResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }
    #endregion

    #region Clientes
    public class FinanzasRequestCFDIBClientes : ModelBase
    {
        public int opcion { get; set; }
        public String busqueda { get; set; }
    }

    public class FinanzasBCFDIBClientesModel
    {
        public int id { get; set; }
        public string razonSocial { get; set; }
        public string rfc { get; set; }
        public string municipio { get; set; }
        public string estado { get; set; } 
        public string calle { get; set; } 
        public string noExterior { get; set; } 
        public string noInterior { get; set; } 
        public string colonia { get; set; } 
        public int idEstado { get; set; } 
        public int idMunicipio { get; set; } 
        public string codigoPostal { get; set; } 
        public string pais { get; set; } 
        public string persona { get; set; } 
        public int iva { get; set; }
        public string metodoPago { get; set; }
        public int idMetodoPago { get; set; }
        public string noCuenta { get; set; }
    }

    public class FinanzasBCFDIBClientesResponse : ResponseModel
    {
        public List<FinanzasBCFDIBClientesModel> lista { get; set; }
    }

    public class FinanzasCFDICEstadoModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class FinanzasCFDICEstadosResponse : ResponseModel
    {
        public List<FinanzasCFDICEstadoModel> lista { get; set; }
    }

    public class FinanzasRequestCFDIBCiudades : ModelBase
    {
        public int idEstado { get; set; }
        public string codigoPostal { get; set; }
    }

    public class FinanzasCFDICiudadesModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
    }

    public class FinanzasCFDICiudadesResponse : ResponseModel
    {
        public List<FinanzasCFDICiudadesModel> lista { get; set; }
    }


    public class FinanzasRequestCFDIClientesAdd : ModelBase
    {
        public string razonSocial { get; set; }
        public string rfc { get; set; }
        public string municipio { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public int idMunicipio { get; set; }
        public string codigoPostal { get; set; }
        public string pais { get; set; }
        public string persona { get; set; }
        public int iva { get; set; }
        public int idMetodoPago { get; set; }
        public string noCuent { get; set; }

        public int idUsuario { get; set; }
    }


    public class FinanzasCFDIClienteAddResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }



    public class FinanzasRequestCFDIClientesEdit : ModelBase
    {
        public int idCliente { get; set; }
        public string razonSocial { get; set; }
        public string rfc { get; set; }
        public string municipio { get; set; }
        public string calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string colonia { get; set; }
        public int idMunicipio { get; set; }
        public string codigoPostal { get; set; }
        public string pais { get; set; }
        public string persona { get; set; }
        public int iva { get; set; }
        public int idMetodoPago { get; set; }
        public string noCuent { get; set; }

        public int idUsuario { get; set; }
    }


    public class FinanzasCFDIClienteEditResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }




    public class FinanzasRequestCFDIClientesDell : ModelBase
    {
        public int idClienteCFDI { get; set; }
        public int idUsuario { get; set; }
    }


    public class FinanzasCFDIClienteDellResponse : ResponseModel
    {
        public string respuesta { get; set; }
    }



    #endregion

    #region Productos
    public class FinanzasCFDIProductosModel
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public decimal precioUnitario { get; set; }
        public string unidad { get; set; }
        public int unidad_id { get; set; }
    }

    public class FinanzasCFDIProductosResponse : ResponseModel
    {
        public List<FinanzasCFDIProductosModel> lista { get; set; }
    }


    public class FinanzasRequestCFDIProductoDell : ModelBase
    {
        public int idProductoCFDI { get; set; }
        public int idUsuario { get; set; }
    }


    public class FinanzasCFDIProductoDellResponse : ResponseModel
    {
        public string respuesta { get; set; }
    }



    public class FinanzasCFDIUnidadesModel
    {
        public int idCfdiUnidad { get; set; }
        public string descripcion { get; set; }
    }

    public class FinanzasCFDIUnidadesResponse : ResponseModel
    {
        public List<FinanzasCFDIUnidadesModel> lista { get; set; }
    }



    public class FinanzasRequestCFDIProductoAdd : ModelBase
    {
        public int idCfdiUnidad { get; set; }
        public decimal precioUnitario { get; set; }
        public string descripcion { get; set; }
        public int idUsuario { get; set; }
    }


    public class FinanzasCFDIProductoAddResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }


    public class FinanzasRequestCFDIProductoEdit : ModelBase
    {
        public int id { get; set; }
        public int idCfdiUnidad { get; set; }
        public decimal precioUnitario { get; set; }
        public string descripcion { get; set; }
        public int idUsuario { get; set; }
    }


    public class FinanzasCFDIProductoEditResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }


    #endregion

    #region Cancelar CFDI


    public class FinanzasRequestCancelarCFDIModel : ModelBase
    {
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }
        public string folio { get; set; }
        public int idUsuario { get; set; }
        public int idRol { get; set; }
    }

    public class FinanzasCancelarCFDIModel
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public int idLoteBE { get; set; }
        public DateTime fechaEmision { get; set; }
        public String RFC { get; set; }
        public String razonSocial { get; set; }
        public String tipoComprobante { get; set; }
        public String concepto { get; set; }
        public decimal tasaIVA { get; set; }
        public String observacionesGrales { get; set; }
        public String uuid { get; set; }
    }
    public class FinanzasCancelarCFDIResponseModel : ResponseModel
    {
        public List<FinanzasCancelarCFDIModel> lista { get; set; }

    }



    public class FinanzasRequestCFDIBorrarFacturaPPC : ModelBase
    {
        public int idCFDIDocmento { get; set; }
        public int idUsuario { get; set; }
    }
    public class FinanzasCFDIBorrarFacturaPPCResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }



    public class FinanzasRequestCFDICancelarFactura : ModelBase
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public int idUsuario { get; set; }
        public string sustitucion { get; set; }
        public string motivo { get; set; }
    }


    public class MotivosCFDIModel
    {
        public int id { get; set; }
        public string clave { get; set; }
        public string label { get; set; }
    }
    public class FinanzasCFDICancelarFacturaResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }
    public class MotivosCFDIResponseModel : ResponseModel
    {
        public List<MotivosCFDIModel> motivos { get; set; }
    }


    public class FinanzasResponseCFDIGenericoFacturaModel : ResponseModel
    {
        public FinanzasRequestCFDIGenericoFacturaModel respuesta { get; set; }
    }

    #endregion

    #region Facturación POS

    #region Modelo
    public class CadenaCFDIPOS
    {
        public int idCadena { get; set; }
        public string nombreComercial { get; set; }
        public string razonSocial { get; set; }
        public string direccion { get; set; }
        public string rfc { get; set; }
        public string contacto { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public string email { get; set; }
        public string manifiesto { get; set; }
    }



    #endregion

    #region Request
    public class FinanzasRequestBuscarCadenaCFDIModel : ModelBase
    {
        public int idCadena { get; set; }
    }
    #endregion


    #region Response
    public class FinanzasBuscarCadenaCFDIResponse : ResponseModel
    {
        public CadenaCFDIPOS cadena { get; set; }
    }
    #endregion

    #endregion

    #region Subir Facturas


    #region Models



    #endregion

    #region Request Models

    public class FinanzasRequestSubirFacturas
    {
        public string nombre { get; set; }
        public string ext { get; set; }
        public string mimeType { get; set; }
        public string archivo { get; set; }
    }


    #endregion

    #region Response Models

    public class FinanzasResponseSubirFacturas : ResponseModel
    {
        public bool insertado { get; set; }
    }

    #endregion


    #endregion


    #region Visor Facturas

    #region Models

    public class TipoComprobante
    {
        public int id { get; set; }
        public string descripcion { get; set; }
    }

    public class DocumentoSAT
    {
        public int id { get; set; }
        public string idLoteBE { get; set; }
        public string fechaEmision { get; set; }
        public string rfc { get; set; }
        public string razonSocial { get; set; }
        public decimal subTotal { get; set; }
        public decimal iva { get; set; }
        public decimal total { get; set; }
        public string tipoComprobante { get; set; }
        public string folioSerie { get; set; }
        public string uuid { get; set; }
        public string concepto { get; set; }
        public decimal tasaIVA { get; set; }
        public string observacionesGenerales { get; set; }
        public string cancelado { get; set; }
        public string metodoPago { get; set; }
        public string documentoRelacionado { get; set; }
        public string idDeposito { get; set; }
        public string fechaDeposito { get; set; }
    }

    #endregion

    #region RequestModels

    public class FinanzasRequestDocumentoSAT
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public string rfc { get; set; }
        public string tipoComprobante { get; set; }
        public string folio { get; set; }
    }
    public class FinanzasRequestOperacionFactura : UsuarioIp
    {
        public int idCfdiDocumento { get; set; }
    }
    public class FinanzasRequestGetXMLFactura
    {
        public int id { get; set; }
    }
    #endregion

    #region ResponseModels

    public class FinanzasResponseTipoComprobantes : ResponseModel
    {
        public List<TipoComprobante> listadoComprobantes { get; set; }
    }

    public class FinanzasResponseDocumentoSAT : ResponseModel
    {
        public List<DocumentoSAT> listadoDocumentos { get; set; }
    }

    public class FinanzasResponseDocumentoSATExcel : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
        public List<DocumentoSAT> listadoDocumentos { get; set; }

    }

    public class FinanzasResponseOperacionFactura : ResponseModel
    {
        public bool realizado { get; set; }
    }
    public class FinanzasResponseReporteDocumentoSATExcel : ResponseModel
    {
        public string reporteExcel { get; set; }
        public string reporteNombre { get; set; }
    }
    public class FinanzasResponseFactua : ResponseModel
    {
        public string xml { get; set; }
        public string ruta { get; set; }
    }
    #endregion

    #endregion
}
