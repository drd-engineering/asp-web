using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Web;
using System.Threading;

namespace System.Based.Core
{
    public class XFEncryptionHelper
    {
        //  Call this function to remove the key from memory after use for security
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);

        /// <summary>
        /// Creates a random salt that will be used to encrypt your file. This method is required on FileEncrypt.
        /// </summary>
        /// <returns></returns>
        private static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    // Fille the buffer with the generated data
                    rng.GetBytes(data);
                }
            }

            return data;
        }

        private static byte[] StringToSalt()
        {
            return Encoding.UTF8.GetBytes(ConfigConstant.ENCRYPT_DECRYPT_SALT);
        }

        /// <summary>
        /// Encrypts a file from its path and a plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="password"></param>
        private string FileEncrypt(string inputFile, string password, string salt)
        {
            //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

            //generate random salt
            //byte[] salt = StringToSalt();// GenerateRandomSalt();

            //create output file name
            FileStream fsCrypt = new FileStream(inputFile + ".drd", FileMode.Create);

            //convert password string to byte arrray
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = System.Text.Encoding.UTF8.GetBytes(salt);

            //Set Rijndael symmetric encryption algorithm
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
            AES.Mode = CipherMode.CFB;

            // write salt to the begining of the output file, so in this case can be random every time
            //fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(inputFile, FileMode.Open);

            //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
            byte[] buffer = new byte[1048576];
            int read;
            string result = "OK";
            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents(); // -> for responsive GUI, using Task will be better!
                    cs.Write(buffer, 0, read);

                }
                cs.FlushFinalBlock();
                // Close up
                fsIn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                result = ex.Message;
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }

            return result;
        }

        /// <summary>
        /// Encrypts a file from its path and a plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="password"></param>
        private string FileEncryptRequest(HttpRequestBase request, string inputFile, string password, string salt)
        {

            //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

            //generate random salt
            //byte[] salt = StringToSalt();// GenerateRandomSalt();

            //create output file name
            FileStream fsCrypt = new FileStream(inputFile + ".drd", FileMode.Create);

            //convert password string to byte arrray
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = System.Text.Encoding.UTF8.GetBytes(salt);

            //Set Rijndael symmetric encryption algorithm
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
            AES.Mode = CipherMode.CFB;

            // write salt to the begining of the output file, so in this case can be random every time
            //fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
            byte[] buffer = new byte[1048576];
            int read;
            string result = "OK";
            try
            {
                var binaryReader = new BinaryReader(request.Files[0].InputStream);
                while ((read = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents(); // -> for responsive GUI, using Task will be better!
                    cs.Write(buffer, 0, read);
                }
                cs.FlushFinalBlock();
                binaryReader.Close();
                binaryReader.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                result = ex.Message;
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }

            return result;
        }

        /// <summary>
        /// Decrypts an encrypted file with the FileEncrypt method through its path and the plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="password"></param>
        private string FileDecrypt(string inputFile, string outputFile, string password, string salt)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = System.Text.Encoding.UTF8.GetBytes(salt);
            //byte[] salt = StringToSalt(); //new byte[32];

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            //fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            string result = "OK";
            int read;
            byte[] buffer = new byte[1048576];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents();
                    fsOut.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
                result = ex_CryptographicException.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                result = ex.Message;
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
                result = ex.Message;
            }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private string FileDecryptRequest(ref byte[] dataBytes, string inputFile, string outputFile, string password, string salt)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = System.Text.Encoding.UTF8.GetBytes(salt);
            //byte[] salt = StringToSalt(); //new byte[32];

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            //fsCrypt.Read(salt, 0, salt.Length);

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CFB;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            //FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            string result = "OK";
            int read;
            byte[] buffer = new byte[1048576];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Application.DoEvents();
                    //fsOut.Write(buffer, 0, read);
                    int base_size = dataBytes.Length;

                    Array.Resize(ref dataBytes, base_size + read);
                    Buffer.BlockCopy(buffer, 0, dataBytes, base_size - (base_size == 0 ? 0 : 1), read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
                result = ex_CryptographicException.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                result = ex.Message;
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
                result = ex.Message;
            }
            finally
            {
                //fsOut.Close();
                fsCrypt.Close();
            }

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public string FileEncryptRequest(HttpRequestBase request, string inputFile)
        {
            string password = ConfigConstant.ENCRYPT_DECRYPT_PWD;
            string salt = ConfigConstant.ENCRYPT_DECRYPT_SALT;

            // For additional security Pin the password of your files
            //GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            // Encrypt the file
            var result = FileEncryptRequest(request, inputFile, password, salt);

            // To increase the security of the encryption, delete the given password from the memory !
            //ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            //gch.Free();

            // You can verify it by displaying its value later on the console (the password won't appear)
            //Console.WriteLine("The given password is surely nothing: " + password);

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public string FileEncrypt(string inputFile)
        {
            string password = ConfigConstant.ENCRYPT_DECRYPT_PWD;
            string salt = ConfigConstant.ENCRYPT_DECRYPT_SALT;
            // For additional security Pin the password of your files
            //GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            // Encrypt the file
            var result = FileEncrypt(inputFile, password, salt);

            // To increase the security of the encryption, delete the given password from the memory !
            //ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            //gch.Free();

            // You can verify it by displaying its value later on the console (the password won't appear)
            //Console.WriteLine("The given password is surely nothing: " + password);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public string FileDecrypt(string inputFile)
        {
            string password = ConfigConstant.ENCRYPT_DECRYPT_PWD;
            string salt = ConfigConstant.ENCRYPT_DECRYPT_SALT;
            // For additional security Pin the password of your files
            //GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            // Decrypt the file
            var result = FileDecrypt(inputFile + ".drd", inputFile, password, salt);

            // To increase the security of the decryption, delete the used password from the memory !
            //ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            //gch.Free();

            // You can verify it by displaying its value later on the console (the password won't appear)
            //Console.WriteLine("The given password is surely nothing: " + password);

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public string FileDecryptRequest(ref byte[] dataBytes, string inputFile)
        {
            string password = ConfigConstant.ENCRYPT_DECRYPT_PWD;
            string salt = ConfigConstant.ENCRYPT_DECRYPT_SALT;

            // For additional security Pin the password of your files
            //GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);

            // Decrypt the file
            var result = FileDecryptRequest(ref dataBytes, inputFile + ".drd", inputFile, password, salt);

            // To increase the security of the decryption, delete the used password from the memory !
            //ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            //gch.Free();
            // You can verify it by displaying its value later on the console (the password won't appear)
            //Console.WriteLine("The given password is surely nothing: " + password);

            return result;
        }

    }

}