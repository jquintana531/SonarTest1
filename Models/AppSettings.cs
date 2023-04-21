using MTC.WebApp.BackOffice.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class AppSettings
    {
        public string Evironment { set; get; }
        public string BaseUrlApi { set; get; }
        public string AWSApi { set; get; }
        public string TicketDir { set; get; }
        public string TicketVisor { set; get; }
        public string FacturasSatDir { set; get; }
        public string ArchivosBancosDir { set; get; }
        public string LoggerLeads { set; get; }
        public string LoggerFacturacion { set; get; }
        public string LoggerLiberaciones { set; get; }
        public string LoggerContabilidad { set; get; }
        public string LoggerSoporte { set; get; }
        public string LoggerOperaciones { set; get; }
        public string LoggerRecursos { set; get; }
        public string LoggerCalidad { set; get; }
        public string DBClient { set; get; } = string.Empty;
        public string PathWebFichaDeposito { set; get; } = string.Empty;
        public string PathLocalFichaDeposito { set; get; } = string.Empty;
        public string domain { set; get; }
        public string user { set; get; }
        public string password { set; get; }
    }

    public class ConfigurationSettings
    {
        public int cargaDepositosBD { set; get; }
        public float montoMaximoDepositos { set; get; }
    }

    public class ModelBase
    {
        public string IdentificadorDispositivo { get; set; }
        public string Ip { get; set; }
        public ModelBase()
        {
            this.IdentificadorDispositivo = "BackOffice";
            this.Ip = Seguridad.GetIPAddress();
        }
    }

    public class Is3_3
    {
        public bool is3_3 { get; set; } = false;
        public bool isComplemento { get; set; } = false;
    }
}
