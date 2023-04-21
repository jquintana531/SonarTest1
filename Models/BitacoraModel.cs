using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class BitacoraModel
    {
        public int idCatalogoTabla { get; set; }
        public int idCatalogoSP { get; set; }
        public int idUsuario { get; set; }
        public int llave { get; set; }
        public string parametros { get; set; }
        public string ip { get; set; }
    }
}
