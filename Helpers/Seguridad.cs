using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MTC.WebApp.BackOffice.Helpers
{
    public static class Seguridad
    {
        private const string privateKey = "a^-;a97|.mT=B!.wx5Z%6;!+5939S;:Dj%tE7;DzTwe.N%;%rc275+v8y9436f2=%E|^w;7D~c^6a4.^GC|3x9^^pN2%62%3H-+8%+:USP6XJ=p|~*:^23!3:wX^:sU2";

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.ToArray<byte>(Enumerable.Select<int, byte>(Enumerable.Where<int>(Enumerable.Range(0, hex.Length), (Func<int, bool>)(x => x % 2 == 0)), (Func<int, byte>)(x => Convert.ToByte(hex.Substring(x, 2), 16))));
        }

        private static string resta(string parametro1, string parametro2)
        {
            string str1 = parametro1;
            string str2 = new string('0', str1.Length - parametro2.Length) + parametro2;
            string str3 = string.Empty;
            int num1 = 0;
            for (int index = 1; index <= str1.Length; ++index)
            {
                int num2 = Convert.ToInt32("0x0" + str1.Substring(str1.Length - index, 1), 16);
                int num3 = Convert.ToInt32("0x0" + (str2.Length - index >= 0 ? str2.Substring(str2.Length - index, 1) : "0"), 16);
                int num4 = num2 - num1 - num3;
                if (num4 < 0)
                {
                    num4 += 16;
                    num1 = 1;
                }
                else
                    num1 = 0;
                str3 = string.Format("{0:x}", (object)num4).Substring(0, 1) + str3;
            }
            return str3;
        }

        public static string Encrypt(string param)
        {
            //se calcula el largo del mensaje
            param = toUtf8(param);

            int len = param.Length;

            //se calcula el key que se va a utilizar de base

            int number = (len / privateKey.Length) + 1;

            float otro = (len / privateKey.Length) + 1;

            string key = String.Concat(Enumerable.Repeat(privateKey, (len / privateKey.Length) + 1));
            key = key.Substring(((key.Length - len) / 2), len);

            //Se convierten los valores a Hex
            param = StringToHex(param);
            key = StringToHex(key);

            //se agrega la longitud al final de param
            param = param + len.ToString("X3");

            //se agrega ceros al key
            key = key + "000";

            //se suman los valores
            string sumaHex = add(param, key);

            //se convierten a base64
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(sumaHex);
            string encodedString = System.Convert.ToBase64String(data);

            return encodedString;
        }

        private static string add(string parametro1, string parametro2)
        {
            string _long = parametro1.Length >= parametro2.Length ? parametro1 : parametro2;
            string _shor = new string('0', _long.Length - (parametro1.Length >= parametro2.Length ? parametro2 : parametro1).Length) + (parametro1.Length >= parametro2.Length ? parametro2 : parametro1);

            string suma = string.Empty;
            int c_lg = 0x00;
            int c_sh = 0x00;

            int accr = 0x00;
            int oper = 0;

            for (int intFlag = 1; intFlag <= _long.Length; intFlag++)
            {
                c_lg = Convert.ToInt32("0x0" + _long.Substring(_long.Length - intFlag, 1), 16);
                c_sh = Convert.ToInt32("0x0" + (_shor.Length - intFlag >= 0 ? _shor.Substring(_shor.Length - intFlag, 1) : "0"), 16);

                oper = c_lg + c_sh + accr;

                if (oper > 0x0F)
                {
                    oper -= 16;
                    accr = 0x01;
                }
                else
                {
                    accr = 0x00;
                }

                suma = string.Format("{0:x}", oper).Substring(0, 1) + suma;
            }

            return suma;
        }

        private static string StringToHex(string text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in text.ToCharArray())
            {
                stringBuilder.Append(((Int16)c).ToString("x"));
            }

            return stringBuilder.ToString();
        }


        public static string toUtf8(String texto)
        {
            string destinationURL = texto;
            string vari = HttpUtility.UrlEncode(destinationURL);
            return vari;

        }

        public static string GetIPAddress()
        {
            string IPAddress = "";

            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;

        }

        public static Boolean ValidarRFC(string cadena)
        {
            int i = 0;
            bool confirmacion = true;
            if (cadena.Length > 11 && cadena.Length < 14)
            {
                if (cadena.Length == 12)
                {
                    cadena = "-" + cadena;
                    i = 1;
                }
                for (int j = i; j <= 3; j++)
                {
                    if (!char.IsLetter(cadena[j]))
                        confirmacion = false;
                }
                for (int j = 4; j <= 9; j++)
                {
                    if (!char.IsDigit(cadena[j]))
                        confirmacion = false;
                }
                for (int j = 9; j < 13; j++)
                {
                    if (!char.IsLetterOrDigit(cadena[j]))
                        confirmacion = false;
                }
                if (!confirmacion)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            if (confirmacion)
                return true;
            else
                return false;
        }


    }
}
