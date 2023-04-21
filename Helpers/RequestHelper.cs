using Microsoft.Extensions.Configuration;
using MTC.WebApp.BackOffice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Helpers
{
    public class RequestHelper
    {

        public string mensajeRespuesta = "";
        public int codigoRespuesta = 0;
        string WebApi = "";
        string AWSApi = "";
        public RequestHelper(AppSettings settings)
        {
            this.WebApi = settings.BaseUrlApi;
            this.AWSApi = settings.AWSApi;
        }


        public async Task<string> doPost(string target, string postData, string token)
        {
            return await Task.Run(() =>
            {
                try
                {
                    HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(WebApi + target);
                    Encoding encoding = new UTF8Encoding();
                    string responseString = "";
                    httpWReq.Timeout = 300000; // minutos
                    string value = "value=" + postData;
                    byte[] data = encoding.GetBytes(value);

                    httpWReq.Method = "POST";
                    httpWReq.ContentType = "application/x-www-form-urlencoded";
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpWReq.Headers.Add("Authorization", "Bearer " + token);
                    }
                    httpWReq.ContentLength = data.Length;

                    Stream stream = httpWReq.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    stream.Close();

                    HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                    if (!string.IsNullOrEmpty(response.ToString()))
                    {
                        string s = response.ToString();
                        responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                        codigoRespuesta = 0;
                        mensajeRespuesta = "";
                    }
                    else
                    {
                        codigoRespuesta = -1;
                        mensajeRespuesta = "Sin Contenido.";
                    }

                    return responseString;

                }
                catch(TimeoutException tx)
                {
                    codigoRespuesta = -1;
                    mensajeRespuesta = "WE:" + codigoRespuesta + " Timeout " + tx.Message;
                    //mensajeRespuesta = "WE: " + ex.ToString();
                    return "";
                }
                catch (Exception ex)
                {
                    codigoRespuesta = -1;
                    mensajeRespuesta = "WE:"+ codigoRespuesta + " Error interno " + ex.Message;
                    //mensajeRespuesta = "WE: " + ex.ToString();
                    return "";
                }
            });
        }
        public async Task<string> doPostAWS(string target, string postData, string token)
        {
            return await Task.Run(() =>
            {
                try
                {
                    HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(AWSApi + target);
                    Encoding encoding = new UTF8Encoding();
                    string responseString = "";
                    httpWReq.Timeout = 300000; // minutos
                    string value = "value=" + postData;
                    byte[] data = encoding.GetBytes(value);

                    httpWReq.Method = "POST";
                    httpWReq.ContentType = "application/x-www-form-urlencoded";
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpWReq.Headers.Add("Authorization", "Bearer " + token);
                    }
                    httpWReq.ContentLength = data.Length;

                    Stream stream = httpWReq.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    stream.Close();

                    HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                    if (!string.IsNullOrEmpty(response.ToString()))
                    {
                        string s = response.ToString();
                        responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                        codigoRespuesta = 0;
                        mensajeRespuesta = "";
                    }
                    else
                    {
                        codigoRespuesta = -1;
                        mensajeRespuesta = "Sin Contenido.";
                    }

                    return responseString;

                }
                catch(TimeoutException tx)
                {
                    codigoRespuesta = -1;
                    mensajeRespuesta = "WE:" + codigoRespuesta + " Timeout " + tx.Message;
                    //mensajeRespuesta = "WE: " + ex.ToString();
                    return "";
                }
                catch (Exception ex)
                {
                    codigoRespuesta = -1;
                    mensajeRespuesta = "WE:"+ codigoRespuesta + " Error interno " + ex.Message;
                    //mensajeRespuesta = "WE: " + ex.ToString();
                    return "";
                }
            });
        }

    }
}
