using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }
        public int IdCatalogoDepartamento { get; set; }
        public int IdGrupoUsuario { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Clave { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Extension { get; set; }
        public bool Activo { get; set; }
        public bool JefeDepartamento { get; set; }
        public bool Visible { get; set; }
        public int IdFirma { get; set; }
        public string ResetClave { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Usuario_name { get; set; }
        public bool StatusClave { get; set; }
        public int IdCatalogoRol { get; set; }
        public string Temp { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public string Ip { get; set; }
        public int TipoAplicacion { get; set; }

    }

    public class UsuarioDataAccessLayer
    {
        public UsuarioModel GetUsuarioData(int id)
        {
            try
            {
                UsuarioModel user = new UsuarioModel();
                return user;
            }
            catch
            {
                throw;
            }
        }
    }

    //Sub Clases derivadas
    public class Credential : ModelBase
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class ActividadUsuario
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Seccion { get; set; }
        public string Actividad { get; set; }
        public string Detalles { get; set; }
        public string Objeto { get; set; } = string.Empty;

    }


    public class UsuarioResponseModel
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public int IdRol { get; set; }
        public bool StatusClave { get; set; }
        public int IdGrupoUsuario { get; set; }
        public int IdDepartamento { get; set; }
        public List<string> Permisos { get; set; } = new List<string>();
    }

    public class LoginResponse : ResponseModel
    {
        public AuthorizationToken authorization { get; set; }
        public UsuarioResponseModel usuario { get; set; }
    }

    public class UserResponse : ResponseModel
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public JToken Roles { get; set; }
    }

    public class UsuarioIp
    {
        public int idUsuario { get; set; }
        public string ip { get; set; }
    }

    public class NewPasswordModel
    {
        public int Usuario { get; set; }
        public string PassAntigua { get; set; }
        public string PassNueva { get; set; }
        public string Ip { get; set; } = "";
    }
}
