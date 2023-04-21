using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Models
{
    public class AdministracionPermisosModel : ModelBase
    {
        public string usuario { set; get; }
    }

    public class AdministracionRequestBUsuariosModel : ModelBase
    {
        public int idUsuario { set; get; }
        public string nombre { set; get; }
    }

    public class AdministracionRequestPermisosRol
    {
        public int rol { get; set; }
    }


    public class AdministracionPermisosResponse : ResponseModel
    {
        public List<AdministracionUsuarioModel> usuarios { set; get; }
        public List<PermisosModel> permisosGenerales { set; get; }
        public List<DashboardModel> dashboards { set; get; }
        public string permisosJson { set; get; }
    }

    public class AdministracionUsuarioModel
    {
        public int idUsuario { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public string usuario { get; set; }
        public List<string> permisos { get; set; }
        public List<string> dashboards { get; set; }
    }
    public class AdministracionPermisosRolResponse : ResponseModel
    {
        public List<string> permisos { get; set; }
    }

    public class AdministracionBUsuariosResponse : ResponseModel
    {
        public List<AdministracionUsuarioModel> respuesta { get; set; }
    }


    public class PermisosModel
    {
        public int Id_permiso { set; get; }
        public string descripcion { set; get; }
        public string codigo { set; get; }
        public int FK_Id_permiso { set; get; }
    }

    public class AdministracionBPermisosResponse : ResponseModel
    {
        public List<PermisosModel> respuesta { get; set; }
    }


    public class AdministracionRequestSetPermisosModel : ModelBase
    {
        public int idUsuarioIn { set; get; }
        public int idUsuario { set; get; }
        public int[] permisos { set; get; }
    }

    public class DashboardModel
    {
        public int id { set; get; }
        public string nombre { set; get; }
        public Boolean activo { set; get; }
    }

    public class AdministracionDashboardsResponse : ResponseModel
    {
        public List<DashboardModel> respuesta { get; set; }
    }


    public class AdministracionRequestSetDashboardModel : ModelBase
    {
        public int idUsuarioIn { set; get; }
        public int idUsuario { set; get; }
        public int[] dashboards { set; get; }
    }


    public class AdministracionRequestInfoDashboardModel : ModelBase
    {
        public int idUsuario { set; get; }
        public int opcion { set; get; }
    }


    //info usuario dashboard
    public class DashboardInfoUsuario
    {
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string email { get; set; }
        public string usuario { get; set; }
        public string rol { get; set; }
        public string departamento { get; set; }
    }


    public class AdministracionDashboardInfoResponse : ResponseModel
    {
        public DataTable respuesta { get; set; }
    }
}
