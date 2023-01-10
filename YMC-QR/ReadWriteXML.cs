﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;


namespace VELOCIS_EINV
{
    class ReadWriteXML
    {
        protected internal string SERVERNAME = null;
        protected internal string USERNAME = null;
        protected internal string PASSWORD = null;
        protected internal string SAGEDB = null;
        protected internal string SAA = null;
        protected internal string SAPSS = null;
        protected internal string SGSTIN = null;
        protected internal string BGSTIN = null;
        protected internal string HSNCODE1 = null;
        protected internal string HSNCODE2 = null;
        protected internal string STATE = null;
        protected internal string PINCODE = null;
        protected internal string INVNUM = null;
        protected internal string APIURL = null;
        //  private static string configPassword = "SecretKey";
        //private static string configPassword = "AcrossDomain";
        private static byte[] _salt = Encoding.ASCII.GetBytes("0123456789abcdef");
        XmlDocument xmldoc = new XmlDocument();

        protected internal bool ReadXML()
        {
            bool result = true;
           try
            {
               bool checkRes= File.Exists(@"YMCDetCRD.xml");
                if (checkRes == true)
                {
                    string xmlFile = File.ReadAllText(@"YMCDetCRD.xml");
                    xmldoc.LoadXml(xmlFile);
                    XmlNodeList dblist = xmldoc.SelectNodes("dbconfig");
                    foreach (XmlNode xn in dblist)
                    {
                        SERVERNAME = xn["SERVERNAME"].InnerText;
                        USERNAME = xn["USERNAME"].InnerText;
                        PASSWORD = xn["PASSWORD"].InnerText;

                        SAGEDB = xn["SAGEDB"].InnerText;
                        SAA = xn["SAA"].InnerText;
                        SAPSS = xn["SAPSS"].InnerText;

                        SGSTIN = xn["SGSTIN"].InnerText;
                        BGSTIN = xn["BGSTIN"].InnerText;
                        HSNCODE1 = xn["HSNCODE1"].InnerText;

                        HSNCODE2 = xn["HSNCODE2"].InnerText;
                        STATE = xn["STATE"].InnerText;
                        PINCODE = xn["PINCODE"].InnerText;
                        INVNUM= xn["INVNUM"].InnerText;
                        APIURL = xn["APIURL"].InnerText;
                    }
                }
                else
                {
                    result = checkRes;
                }
            }
            catch (Exception )
            {
                result = false;
            }
            return result;
        }

        //protected internal void SaveXML()
        //{
        //    XmlNodeList dblist = xmldoc.SelectNodes("dbconfig");
        //    foreach (XmlNode xn in dblist)
        //    {
        //        xn["servername"].InnerText = servername;
        //        xn["sapassword"].InnerText = EncryptString(sapassword, configPassword);
        //        xn["gstdb"].InnerText = gstdb;
        //        xn["storedprocname"].InnerText = storedprocname;
        //        xmldoc.Save(@"DBConfig.xml");
        //    }
        //}

        public string EncryptString(string plainText, string sharedSecret)
        {
            string result = null;
            RijndaelManaged aesAlg = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    result = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return result;
        }

        public  string DecryptString(string cipherText, string sharedSecret)
        {
            RijndaelManaged aesAlg = null;
            string result = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return result;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
