using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    #region Ajuste de saldo
    public class ContabilidadCuentas
    {
        public int id { get; set; }
        public string label { get; set; }
    }

    public class ContabilidadListadoCuentasResponse : ResponseModel
    {
        public List<ContabilidadCuentas> listadoCuentas { get; set; }
    }

    public class ContabilidadFormasPagoResponse : ResponseModel
    {
        public List<FormasPago> listadoFormasPago { get; set; }
    }

    public class ContabilidadAjusteSaldoGuardar : ModelBase
    {
        public int idUsuario { set; get; }
        public int idCadena { set; get; }
        public int idCatalogoCuenta { set; get; }
        public DateTime fechaDeposito { set; get; }
        public int idCatalogoFormaPago { set; get; }
        public string referencia { set; get; }
        public string referencia2 { set; get; }
        public decimal monto { set; get; }
        public string observaciones { set; get; }
        public Boolean publico { set; get; }
        public Boolean comision { set; get; }
        public DateTime horaDeposito { set; get; }
    }

    public class ContabilidadAjusteSaldoGuardarResponse : ResponseModel
    {
        public int respuesta { get; set; }
    }

    public class ContabilidadResponseInformacionCadena : ResponseModel
    {
        public InformacionClienteCadena infoCadena { get; set; }
    }
    #endregion

    #region Cartera de Clientes
    public class ContabilidadGenerarCartera : ModelBase
    {
        public int anio { set; get; }
        public int mes { set; get; }
    }

    public class ContabilidadCarteraData : ModelBase
    {
        public string id { set; get; }
        public string nombreComercial { set; get; }
        public string tasaIVA { set; get; }
        public string CondicionComercial { set; get; }
        public string SaldoContable { set; get; }
        public string Recargas { set; get; }
        public string recargasInternacionales { set; get; }
        public string iusaCfeTarjeta { set; get; }
        public string Zuggy { set; get; }
        public string TiendaVirtual { set; get; }
        public string CelularesPayandGo { set; get; }
        public string ComisionTraspOPE { set; get; }
        public string NotadeCredito { set; get; }
        public string ServiciosDatalogic { set; get; }
        public string Elektra { set; get; }
        public string Regalii { set; get; }
        public string TraspasosOperadora { set; get; }
        public string TraspasoCargo { set; get; }
        public string Profarmex { set; get; }
        public string TimbradoFP { set; get; }
        public string iusaCfeRecarga { set; get; }
        public string Televia { set; get; }
        public string IAVE { set; get; }
        public string ViaPass { set; get; }
        public string BANAMEX2376 { set; get; }
        public string BANCOMER989 { set; get; }
        public string BANCOMER1466 { set; get; }
        public string SANTANDER { set; get; }
        public string BANORTE { set; get; }
        public string AZTECA { set; get; }
        public string BBVA1131 { set; get; }
        public string BBVA0728FARMA { set; get; }
        public string BNX5907FARMA { set; get; }
        public string BANBAJIO { set; get; }
        public string SANTANDERFARMA { set; get; }
        public string BANORTEFARMA { set; get; }
        public string ciBanco { set; get; }
        public string Intermex { set; get; }
        public string SantanderTPV { set; get; }
        public string BancomerTPV { set; get; }
        public string Lealtad { set; get; }
        public string DecrementoRecarga { set; get; }
        public string DecrementoServicio { set; get; }
        public string transpasoComRecarga { set; get; }
        public string compraComRecarga { set; get; }
        public string zuggyComRecarga { set; get; }
        public string IncrementoRecarga { set; get; }
    }

    public class ContabilidaGenerarCarteraResponse : ResponseModel
    {
        public List<ContabilidadCarteraData> respuesta { get; set; }
    }

    public class ContabilidaGenerarCarteraResponse2 : ResponseModel
    {
        public string respuesta { get; set; }
        public string reporteNombre { get; set; }
    }
    #endregion
}
