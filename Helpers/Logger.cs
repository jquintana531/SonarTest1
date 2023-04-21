using log4net;
using log4net.Config;
using MTC.WebApp.BackOffice.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Helpers
{
    public static class Logger
    {

        public static void LogActividad(ActividadUsuario objeto, bool error = false, string file = "")
        {
           
            ILog logger = string.IsNullOrEmpty(file) ? LogManager.GetLogger(typeof(Program)) : LogManager.GetLogger(Assembly.GetCallingAssembly(),file);
            String xml;
            System.IO.StringWriter strWriter = new System.IO.StringWriter();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(objeto.GetType());
            serializer.Serialize(strWriter, objeto);
            xml = strWriter.ToString();
            xml = "<Fecha_Registro>" + System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:mmm") + "</Fecha_Registro>\n" + xml;
            strWriter.Close();

            if (error)
            {
                logger.Error(xml);
            }
            else
            {
                logger.Info(xml);
            }
        }


        public static async Task<bool> DoLogAsync(MethodBase meta, Object detalles, bool error = false)
        {
            return await Task.Run(() =>
            {
                ActividadUsuario logAct = new ActividadUsuario();

                logAct.Seccion = meta.Name;
                logAct.Actividad = meta.ReflectedType.Name;
                logAct.Detalles = JsonConvert.SerializeObject(detalles);
                if (error)
                {
                    Logger.LogActividad(logAct, true);
                }
                else
                {
                    Logger.LogActividad(logAct);
                }
                return true;
            });
        }


        public static void RegistraLog(Object objeto, string mensaje)
        {
            try
            {
                string file = "LogLeads";

                ILog logger = string.IsNullOrEmpty(file) ? LogManager.GetLogger(typeof(Program)) : LogManager.GetLogger(Assembly.GetCallingAssembly(), file);
                String xml;
                System.IO.StringWriter strWriter = new System.IO.StringWriter();
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(mensaje.GetType());
                serializer.Serialize(strWriter, mensaje);
                xml = strWriter.ToString();
                strWriter.Close();

            }
            catch (Exception xm)
            {
                int i = 0;
            }
        }

        public static void DoLog(MethodBase meta, Object detalles, bool error = false, string file = "")
        {
            ActividadUsuario logAct = new ActividadUsuario();

            logAct.Seccion = meta.Name;
            logAct.Actividad = meta.ReflectedType.Name;
            logAct.Detalles = JsonConvert.SerializeObject(detalles);
            if (error)
            {
                Logger.LogActividad(logAct, true, file);
            }
            else
            {
                Logger.LogActividad(logAct,false,file);
            }
        }
    }


}
