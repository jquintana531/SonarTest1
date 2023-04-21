using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class ResponseModel
    {
        public int CodigoRespuesta { get; set; }
        public string MensajeRespuesta { get; set; }
    }
    public class AuthorizationToken
    {
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
    }

    


    


}
