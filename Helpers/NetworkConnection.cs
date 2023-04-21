using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Principal;
using System.IO;
using MTC.WebApp.BackOffice.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.Threading.Tasks;

namespace MTC.WebApp.BackOffice.Helpers
{
    public class NetworkConnection : IDisposable
    {
        string _networkName;

        public NetworkConnection(string networkName,
            NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource()
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = networkName
            };

            var userName = string.IsNullOrEmpty(credentials.Domain)
                ? credentials.UserName
                : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            var result = WNetAddConnection2(
                netResource,
                credentials.Password,
                userName,
                0);

            if (result != 0)
            {
                throw new Win32Exception(result);
            }
        }

        ~NetworkConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            WNetCancelConnection2(_networkName, 0, true);
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource,
            string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags,
            bool force);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NetResource
    {
        public ResourceScope Scope;
        public ResourceType ResourceType;
        public ResourceDisplaytype DisplayType;
        public int Usage;
        public string LocalName;
        public string RemoteName;
        public string Comment;
        public string Provider;
    }

    public enum ResourceScope : int
    {
        Connected = 1,
        GlobalNetwork,
        Remembered,
        Recent,
        Context
    };

    public enum ResourceType : int
    {
        Any = 0,
        Disk = 1,
        Print = 2,
        Reserved = 8,
    }

    public enum ResourceDisplaytype : int
    {
        Generic = 0x0,
        Domain = 0x01,
        Server = 0x02,
        Share = 0x03,
        File = 0x04,
        Group = 0x05,
        Network = 0x06,
        Root = 0x07,
        Shareadmin = 0x08,
        Directory = 0x09,
        Tree = 0x0a,
        Ndscontainer = 0x0b
    }


    public class Impersonation
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);
        public static void LogOn(string rutaLocal, string rutaDestino)
        {
            SafeTokenHandle safeTokenHandle;
            try
            {

                Task<Boolean> taskLogLocaljp1 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), "En try");
                taskLogLocaljp1.Wait();

                AppSettings appSettings = new AppSettings();
                string userName = appSettings.user;
                string domainName = appSettings.domain;
                string password = appSettings.password;
                // \\192.168.5.8
                // TecnologiaMTC
                // alanTI2022

                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token.
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token.
                bool returnValue = LogonUser(userName, domainName, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeTokenHandle);

                Console.WriteLine("LogonUser called.");

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    Console.WriteLine("LogonUser failed with error code : {0}", ret);


                    Task<Boolean> taskLogLocaljp3 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), "LogonUser failed with error code :" +ret);
                    taskLogLocaljp3.Wait();

                    throw new System.ComponentModel.Win32Exception(ret);
                }
                using (safeTokenHandle)
                {

                    Task<Boolean> taskLogLocaljp2 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), "En using");
                    taskLogLocaljp2.Wait();

                    //File.Copy("C:\\Logger\\BuroCredito\\apiBuro-2023-03-28.xml", "S:\\archivos_bancos\\apiBuro-2023-03-28.xml");
                    File.Copy(rutaLocal, rutaDestino);


                    Task<Boolean> taskLogLocaljp4 = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), "Copiado");
                    taskLogLocaljp4.Wait();

                    /*
                    if (File.Exists(rutaLocal))
                    {
                        File.Delete(rutaLocal);
                    }*/

                    Console.WriteLine("exito");
                }
            }
            catch (Exception ex)
            {
                Task<Boolean> taskLogLocaljp = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), ex.Message);
                taskLogLocaljp.Wait();
            }
        }

    }
    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
}
