using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Encrypt
{
    public class clsEncDnc
    {
        internal const string Inputkey = "560A18CD-6346-4CF0-A2E8-671F9B6B9EA9";
        internal const string Inputky6 = "RmluZ2VyUHJpbnRzIENvbnRyb2wgQXNpc3RlbmNpYSB5IExhYm9yYSBDcmVhdGVnYSA=";
        //"Q29udHJvbCBBc2lzdGVuY2lhIHkgTGFib3JhIENyZWF0ZWdhIA==
        //"RmluZ2VyUHJpbnRzIENvbnRyb2wgQXNpc3RlbmNpYSB5IExhYm9yYSBDcmVhdGVnYSA=
        //"92bc0415eaa3052467cd0c3ac5388ee0cfa1c6960e2b217086171d7ce781217c
        string keyEnc = "AssCtrl2024Lbr";

        public string encPss(string entPss)
        {
            byte[] Salt = Encoding.ASCII.GetBytes(keyEnc.Length.ToString());
            string salt = Salt.ToString();

            if (string.IsNullOrEmpty(entPss))
                throw new ArgumentNullException("text");

            var aesAlg = NewRijndaelManaged(salt);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(entPss);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public string dncPss(string entPss)
        {

            //entPss = "IlFZAwSx1PvEhTrddDz3lA==";

            byte[] Salt = Encoding.ASCII.GetBytes(keyEnc.Length.ToString());
            string salt = Salt.ToString();

            if (string.IsNullOrEmpty(entPss))
                throw new ArgumentNullException("cipherText");

            if (!IsBase64String(entPss))
                throw new Exception("The cipherText input parameter is not base64 encoded");

            string text;

            var aesAlg = NewRijndaelManaged(salt);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            var cipher = Convert.FromBase64String(entPss);

            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        text = srDecrypt.ReadToEnd();
                    }
                }
            }
            return text;
        }

        public static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) &&
                   Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }

        private static RijndaelManaged NewRijndaelManaged(string salt)
        {
            if (salt == null) throw new ArgumentNullException("salt");
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes(Inputkey, saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }

        public string dcrypt(string cadena, string clave = Inputky6)
        {
            byte[] arreglo = Convert.FromBase64String(cadena);
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] llave = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(clave));
            mD5CryptoServiceProvider.Clear();
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider()
            {
                Key = llave,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] resultado = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(arreglo, 0, (int)arreglo.Length);
            tripleDESCryptoServiceProvider.Clear();
            return Encoding.UTF8.GetString(resultado);
        }

        public Task<string> dcryptAsync(string cadena, string clave = Inputky6)
        {
            byte[] arreglo = Convert.FromBase64String(cadena);
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] llave = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(clave));
            mD5CryptoServiceProvider.Clear();
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider()
            {
                Key = llave,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] resultado = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(arreglo, 0, (int)arreglo.Length);
            tripleDESCryptoServiceProvider.Clear();
            string str = Encoding.UTF8.GetString(resultado);
            return Task.Run<string>(() => str);
        }

        public string encryp(string cadena, string clave = Inputky6)
        {
            byte[] arreglo = Encoding.UTF8.GetBytes(cadena);
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] llave = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(clave));
            mD5CryptoServiceProvider.Clear();
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider()
            {
                Key = llave,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] resultado = tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(arreglo, 0, (int)arreglo.Length);
            tripleDESCryptoServiceProvider.Clear();
            return Convert.ToBase64String(resultado, 0, (int)resultado.Length);
        }

        public Task<string> encrypAsync(string cadena, string clave = Inputky6)
        {
            byte[] arreglo = Encoding.UTF8.GetBytes(cadena);
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] llave = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(clave));
            mD5CryptoServiceProvider.Clear();
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider()
            {
                Key = llave,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform convertir = tripleDESCryptoServiceProvider.CreateEncryptor();
            byte[] numArray = convertir.TransformFinalBlock(arreglo, 0, (int)arreglo.Length);
            tripleDESCryptoServiceProvider.Clear();
            return Task.Run<string>(() => Convert.ToBase64String(numArray, 0, (int)numArray.Length));
        }

        private byte[] encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }
        public string encrypt(string Data, string Password, int Bits)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(Data);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x0, 0x1, 0x2, 0x1C, 0x1D, 0x1E, 0x3, 0x4, 0x5, 0xF, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
            if (Bits == 128)
            {
                byte[] encryptedData = encrypt(clearBytes, pdb.GetBytes(16), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else if (Bits == 192)
            {
                byte[] encryptedData = encrypt(clearBytes, pdb.GetBytes(24), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else if (Bits == 256)
            {
                byte[] encryptedData = encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else
            {
                return String.Concat(Bits);
            }

        }
        private byte[] decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }
        public string decrypt(string Data, string Password, int Bits)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(Data);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x0, 0x1, 0x2, 0x1C, 0x1D, 0x1E, 0x3, 0x4, 0x5, 0xF, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
                if (Bits == 128)
                {
                    byte[] decryptedData = decrypt(cipherBytes, pdb.GetBytes(16), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else if (Bits == 192)
                {
                    byte[] decryptedData = decrypt(cipherBytes, pdb.GetBytes(24), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else if (Bits == 256)
                {
                    byte[] decryptedData = decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else
                {
                    return String.Concat(Bits);
                }
            }
            catch (Exception ex)
            {
                return String.Concat(Bits);
            }
        }

    }
}
