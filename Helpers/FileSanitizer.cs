using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MTC.WebApp.BackOffice.Helpers
{
    public class FileSanitizer
    {


        public static bool ArchivoValido(byte[] file, string fileExt)
        {
            fileExt = fileExt.ToUpper();
            byte[] header;
            try
            {
                Dictionary<string, byte[]> fileHeader = new Dictionary<string, byte[]>();

                fileHeader.Add("JPG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
                fileHeader.Add("JPEG", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });
                fileHeader.Add("PNG", new byte[] { 0x89, 0x50, 0x4E, 0x47 });
                fileHeader.Add("TIF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
                fileHeader.Add("TIFF", new byte[] { 0x49, 0x49, 0x2A, 0x00 });
                fileHeader.Add("GIF", new byte[] { 0x47, 0x49, 0x46, 0x38 });
                fileHeader.Add("BMP", new byte[] { 0x42, 0x4D });
                fileHeader.Add("ICO", new byte[] { 0x00, 0x00, 0x01, 0x00 });
                fileHeader.Add("PDF", new byte[] { 0x25, 0x50, 0x44, 0x46 });

                byte[] tmp = fileHeader[fileExt];
                header = new byte[tmp.Length];



                Array.Copy(file, 0, header, 0, header.Length);

                if (compareArray(tmp, header))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }
        }



        private static bool compareArray(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }


        public static bool ValidarParametros(IFormFile file)
        {
            List<string> allowExt = new List<string> { ".doc", ".docx", ".DOC", ".DOCX", ".jpg", ".JPG", ".jpeg", ".JPEG", ".png", ".PNG", ".pdf", ".PDF", ".xls", ".xlsx" };
            string ext = Path.GetExtension(file.FileName);
            if (allowExt.Contains(ext))
            {
                var len = (file.Length * 0.000001);
                if ((file.Length * 0.000001) <= 2)
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<Boolean> uploadFile(IFormFile file,string pathDir,string newName ="")
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (file != null || file.Length > 0)
                    {
                        if (FileSanitizer.ValidarParametros(file))
                        {
                            string filename = file.FileName;
                            if (!Directory.Exists(pathDir))
                            {
                                Directory.CreateDirectory(pathDir);
                            }
                            if (newName.Length > 0)
                            {
                                filename = newName+ Path.GetExtension(file.FileName);
                            }

                            var path = Path.Combine(pathDir, filename);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            return true;
                        }
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        public static bool uploadFile64(string file64, string fileName,string ext,string pathDir, string newName = "", string rootPath = "")
        {
            try
            {

                if (file64 != null || file64.Length > 0)
                {

                    if (FileSanitizer.ValidarExt(ext))
                    {
                        if (!Directory.Exists(pathDir))
                        {
                            Directory.CreateDirectory(pathDir);
                        }
                        if (newName.Length > 0)
                        {
                            fileName = newName +"."+ ext;
                        }

                        var path = Path.Combine(pathDir, fileName);

                        //var pathLocal = @"F:\\wwwroot\\images\\helpDesk\\" + fileName;


                        File.WriteAllBytes(path, Convert.FromBase64String(file64));

                   

                      
                        return true;
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                Task<Boolean> taskLogLocaljp = Logger.DoLogAsync(MethodBase.GetCurrentMethod(), ex.Message);
                taskLogLocaljp.Wait();
                return false;
            }
        }
        public static bool ValidarExt(string ext)
        {
            List<string> allowExt = new List<string> { "doc", "docx", "DOC", "DOCX", "jpg", "JPG", "jpeg", "JPEG", "png", "PNG", "pdf", "PDF", "xls", "xlsx" };
            
            if (allowExt.Contains(ext))
            {
               
                    return true;
                
            }
            return false;
        }


    }
}
