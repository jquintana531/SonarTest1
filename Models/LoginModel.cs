using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class LoginModel
    {
    }


    public class LoginData
    {
        [JsonProperty(PropertyName = "codigoRespuesta")]
        public int CodigoRespuesta { get; set; }

        [JsonProperty(PropertyName = "mensajeRespuesta")]
        public string MensajeRespuesta { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "claim_jti")]
        public int Claim_jti { get; set; }

        [JsonProperty(PropertyName = "claim_idUsuario")]
        public int Claim_idUsuario { get; set; }

        [JsonProperty(PropertyName = "claim_nombre")]
        public string Claim_nombre { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "claim_aPaterno")]
        public string Claim_aPaterno { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "claim_aMaterno")]
        public string Claim_aMaterno { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "claim_idRol")]
        public int Claim_idRol { get; set; }

        [JsonProperty(PropertyName = "statusClave")]
        public bool StatusClave { get; set; }

        [JsonProperty(PropertyName = "claim_idGrupoUsuario")]
        public int Claim_idGrupoUsuario { get; set; }

        [JsonProperty(PropertyName = "claim_idDepartamento")]
        public int Claim_idDepartamento { get; set; }

        [JsonProperty(PropertyName = "tipoAplicacion")]
        public int TipoAplicacion { get; set; }

        [JsonProperty(PropertyName = "permisos")]
        public string Permisos { get; set; } = string.Empty;
    }
}
