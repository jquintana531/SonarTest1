using MTC.WebApp.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Helpers
{
    public class Utils
    {
        private bool is3_3;

        public int desencriptaId(string id)
        {
            Encripta encriptador = new Encripta(System.Text.Encoding.UTF8);
            return int.Parse(encriptador.Decrypt(id));
        }

        public string encripta(string texto)
        {

            Encripta encriptador = new Encripta(System.Text.Encoding.UTF8);
            return encriptador.Encrypt(texto);
        }

        public void logActividadUsuario(ActividadUsuario actividad)
        {
            Logger.LogActividad(actividad);
        }


        public void logActividadUsuario(ActividadUsuario actividad, string file)
        {
            Logger.LogActividad(actividad,false, file);
        }

        public string FichaDeposito(AppSettings settings, string idDeposito)
        {

            //string pathLocal = "T:/wwwroot/images/";
            string pathUrl = "https://www.mtcpos.com.mx/imgDepositos/";


            //string pathUrl = settings.PathWebFichaDeposito;



            try
            {
                /*
                if (System.IO.File.Exists(settings.PathLocalFichaDeposito + idDeposito + ".jpg"))
                {
                    pathUrl += idDeposito + ".jpg";
                }
                else if (System.IO.File.Exists(settings.PathLocalFichaDeposito + idDeposito + ".jpeg"))
                {
                    pathUrl += idDeposito + ".jpeg";
                }
                else if (System.IO.File.Exists(settings.PathLocalFichaDeposito + idDeposito + ".png"))
                {
                    pathUrl += idDeposito + ".png";
                }
                else if (System.IO.File.Exists(settings.PathLocalFichaDeposito + idDeposito + ".pdf"))
                {
                    //rutaImagen = idDeposito + ".pdf";
                    pathUrl += idDeposito + ".pdf";
                    //pathUrl += "~/img/verPDF.png";
                }
                else
                {
                    //pathUrl += "blank.png";
                    //pathUrl += idDeposito + ".jpg";
                    //pathUrl = settings.PathWebFichaDeposito + idDeposito+ ".jpg";
                    pathUrl = "";
                }*/

                
                if (ImgDepExist(Path.Combine(pathUrl, idDeposito+".jpg")))
                {
                    return Path.Combine(pathUrl, idDeposito+".jpg");
                }
                else if (ImgDepExist(Path.Combine(pathUrl, idDeposito+".jpeg")))
                {
                    return Path.Combine(pathUrl, idDeposito+".jpg");
                }
                else if (ImgDepExist(Path.Combine(pathUrl, idDeposito+".png")))
                {
                    return Path.Combine(pathUrl, idDeposito + ".png");
                }
                else if (ImgDepExist(Path.Combine(pathUrl, idDeposito + ".pdf")))
                {
                    return Path.Combine(pathUrl, idDeposito + ".pdf");
                }
                else
                {
                    //pathUrl += "blank.png";
                    //pathUrl += idDeposito + ".jpg";
                    //pathUrl = settings.PathWebFichaDeposito + idDeposito+ ".jpg";
                    pathUrl = "";
                }

                return pathUrl;

            }
            catch
            {
                return pathUrl += "blank.png";
            }
        }

        public bool ImgDepExist(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "HEAD";

            bool exists;
            try
            {
                request.GetResponse();
                exists = true;
            }
            catch
            {
                exists = false;
            }

            return exists;

        }

        public string FormatFechaT(string fecha)
        {
            try
            {

                DateTime fechaFormated = DateTime.Parse(fecha);

                return fechaFormated.ToString("dd/MM/yyyy");
            }
            catch
            {
                return "";
            }
        }

        public Is3_3 try3_3(string archivo)
        {
            Is3_3 is3 = new Is3_3();
            is3.is3_3 = false;
            StreamReader sr = null;
            string info = string.Empty;
            try
            {
                sr = new StreamReader(archivo);
                info = sr.ReadToEnd();
                if (info.Contains("Version=\"3.3\""))
                {
                    is3.is3_3 = true;
                    if (info.Contains("pago10:Pago"))
                        is3.isComplemento = true;
                }
            }
            catch { }
            finally
            {
                if (sr != null) {
                    sr.Close();
                }
                    
            }

            return is3;
        }

        public string getMes(int mesNumero)
        {
            string mes = string.Empty;

            switch (mesNumero)
            {
                case 1:
                    mes = "Enero";
                    break;
                case 2:
                    mes = "Febrero";
                    break;
                case 3:
                    mes = "Marzo";
                    break;
                case 4:
                    mes = "Abril";
                    break;
                case 5:
                    mes = "Mayo";
                    break;
                case 6:
                    mes = "Junio";
                    break;
                case 7:
                    mes = "Julio";
                    break;
                case 8:
                    mes = "Agosto";
                    break;
                case 9:
                    mes = "Septiembre";
                    break;
                case 10:
                    mes = "Octubre";
                    break;
                case 11:
                    mes = "Noviembre";
                    break;
                case 12:
                    mes = "Diciembre";
                    break;
            }

            return mes;
        }
    }
}
