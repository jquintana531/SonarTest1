

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MTC.WebApp.BackOffice.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Helpers
{

    public  class MTCAuthorizationHandler : TypeFilterAttribute
    {
        public MTCAuthorizationHandler(string permisos ) : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { permisos };
        }
    }

    public class AuthorizeActionFilter : IAsyncActionFilter
    {
        private readonly string _permisos;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public AuthorizeActionFilter(string permisos, IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
            _permisos = permisos;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                bool isAuthorized = existePermiso(context.HttpContext.User, _permisos);

                if (!isAuthorized)
                {
                    Logger.DoLog(MethodBase.GetCurrentMethod(), new { actividad = context.RouteData.Values.ElementAt(1) + "-" + context.RouteData.Values.ElementAt(0), mensaje = "No cuenta con los permisos necesarios", idUsuario = int.Parse(_session.GetString("idUsuario")) });
                    context.Result = new ObjectResult(new ResponseModel { CodigoRespuesta = 401, MensajeRespuesta = "No autorizado" })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
                else
                {
                    await next();
                }
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), new { actividad = "OnActionExecutionAsync", mensaje = "Ex:" + ex.Message, idUsuario = 0 });
                context.Result = new ObjectResult(new ResponseModel { CodigoRespuesta = 401, MensajeRespuesta = "No autorizado" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            
        }


        public Boolean existePermiso(ClaimsPrincipal user, string permisos)
        {
            try
            {
                Boolean valido = false;
                List<string> requeridos = permisos.Split(new char[] { ',' }).ToList();
                List<string> permisosCargados = JsonConvert.DeserializeObject<List<string>>(_session.GetString("roles"));


                foreach (string permiso in requeridos)
                {
                    if (permisosCargados.Contains(permiso))
                    {
                        valido = true;
                    }
                    else
                    {
                        valido = false;
                        break;
                    }
                }


                return valido;
            }
            catch (Exception ex)
            {
                Logger.DoLog(MethodBase.GetCurrentMethod(), new { actividad = "Existe permiso", mensaje = "Ex:"+ex.Message, idUsuario = 0 });
                return false;

            }

            
        }
    }

}
