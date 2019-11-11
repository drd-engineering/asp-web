using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Security.Cryptography;

namespace DRD.Service
{
    public class FaspayDataService
    {
        //public class TrxContent
        //{
        //    public decimal Id;
        //    public string Nomor;
        //    public string Type;

        //};

        public FaspayData GetData(string bank)
        {
            FaspayData data = new FaspayData();
            //data.UserId = "bot31468";
            //data.Password = "Fj2KXCAJ";
            //data.MerchantId = "31468";

            data.IsProduction = true; ///// PENTING
            if (data.IsProduction)
            {
                // production
                if (bank.Equals("Credit"))
                {
                    data.Password = "eymtz"; 
                    data.MerchantId = "aggregator_drdaccess";
                } 
                else {
                    data.UserId = "bot32547";
                    data.Password = "jmxhuE4y";
                    data.MerchantId = "32547";
                }
            }
            else {
                // development
                if (bank.Equals("Credit"))
                {
                    data.Password = "abcde";
                    data.MerchantId = "tes_auto";
                }
                else {
                    data.UserId = "bot32547";
                    data.Password = "p@ssw0rd";
                    data.MerchantId = "32547";
                }
            }
            

            //if (bank.Equals("bca"))
            //{
            //    data.BCAKlikPayCode = "UATYUK";
            //    data.BCAClearKey = genKeyId("KlikPayYukTraDev");
            //}

            return data;
        }

        //public TrxContent SplitTrxId(string value)
        //{
        //    TrxContent trx = new TrxContent();

        //    int pos = value.IndexOf("-TX");
        //    int pos2 = value.IndexOf("-", pos + 1);
        //    trx.Nomor = value.Substring(0, pos);
        //    trx.Type = value.Substring(pos + 3, 2);
        //    trx.Id = decimal.Parse(value.Substring(pos + 5, pos2 - (pos + 5)));

        //    return trx;
        //}

        public string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
        public string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }


        private string pack(string input)
        {
            //only for H32 & H*
            return Encoding.Default.GetString(FromHex(input));
        }
        public byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[(hex.Length / 2)];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        static string hex2bin(string hexdata)
        {
            if (hexdata == null)
                throw new ArgumentNullException("hexdata");
            if (hexdata.Length % 2 != 0)
                throw new ArgumentException("hexdata should have even length");

            byte[] bytes = new byte[hexdata.Length / 2];
            for (int i = 0; i < hexdata.Length; i += 2)
                bytes[i / 2] = (byte)(HexValue(hexdata[i]) * 0x10
                + HexValue(hexdata[i + 1]));
            return Encoding.GetEncoding(1252).GetString(bytes);
        }

        static int HexValue(char c)
        {
            int ch = (int)c;
            if (ch >= (int)'0' && ch <= (int)'9')
                return ch - (int)'0';
            if (ch >= (int)'a' && ch <= (int)'f')
                return ch - (int)'a' + 10;
            if (ch >= (int)'A' && ch <= (int)'F')
                return ch - (int)'A' + 10;
            throw new ArgumentException("Not a hexadecimal digit.");
        }

        private string str2bin(string data)
        {
            //var len = data.Length;
            //return pack(data);
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }
        private string bin2hex(string data)
        {
            //var corrected = ereg_replace("[^0-9a-fA-F]", "", data);
            //var corrected = Regex.Replace("[^0-9a-fA-F-[]", "", data);
            var corrected = BinToHex(data);
            return corrected;
        }

        static string BinToHex(string bin)
        {
            StringBuilder binary = new StringBuilder(bin);
            bool isNegative = false;
            if (binary[0] == '-')
            {
                isNegative = true;
                binary.Remove(0, 1);
            }

            for (int i = 0, length = binary.Length; i < (4 - length % 4) % 4; i++) //padding leading zeros
            {
                binary.Insert(0, '0');
            }

            StringBuilder hexadecimal = new StringBuilder();
            StringBuilder word = new StringBuilder("0000");
            for (int i = 0; i < binary.Length; i += 4)
            {
                word = new StringBuilder("0000");
                for (int j = i; j < i + 4; j++)
                {
                    word[j % 4] = binary[j];
                }

                switch (word.ToString())
                {
                    case "0000": hexadecimal.Append('0'); break;
                    case "0001": hexadecimal.Append('1'); break;
                    case "0010": hexadecimal.Append('2'); break;
                    case "0011": hexadecimal.Append('3'); break;
                    case "0100": hexadecimal.Append('4'); break;
                    case "0101": hexadecimal.Append('5'); break;
                    case "0110": hexadecimal.Append('6'); break;
                    case "0111": hexadecimal.Append('7'); break;
                    case "1000": hexadecimal.Append('8'); break;
                    case "1001": hexadecimal.Append('9'); break;
                    case "1010": hexadecimal.Append('A'); break;
                    case "1011": hexadecimal.Append('B'); break;
                    case "1100": hexadecimal.Append('C'); break;
                    case "1101": hexadecimal.Append('D'); break;
                    case "1110": hexadecimal.Append('E'); break;
                    case "1111": hexadecimal.Append('F'); break;
                    default:
                        return "Invalid number";
                }
            }

            if (isNegative)
            {
                hexadecimal.Insert(0, '-');
            }

            return hexadecimal.ToString();
        }

        private long intval32bits(long value)
        {
            if (value > 2147483647)
                value = (value - 4294967296);
            else if (value < -2147483648)
                value = (value + 4294967296);
            return value;
        }
        private long getHash(string value)
        {
            long h = 0;
            for (int i = 0; i < value.Length; i++)
            {
                h = intval32bits(add31T(h) + value[i]);
            }
            return h;
        }
        private long add31T(long value)
        {
            long result = 0;
            for (int i = 1; i <= 31; i++)
            {
                result = intval32bits(result + value);
            }
            return result;
        }

        //----
        private string genKeyId(string clearKey)
        {
            return bin2hex(str2bin(clearKey)).ToUpper();
        }

        public string genSignature(string klikPayCode, DateTime transactionDate, string transactionNo,
            long amount, string currency, string keyId)
        {
            /*
            * Signature Step 1
            */
            var tempKey1 = klikPayCode + transactionNo + currency + keyId;
            var hashKey1 = getHash(tempKey1);
            //echo "tempKey1 : " . tempKey1;
            //echo " hasKey1 : " . hashKey1 . "<br>";
            /*
            * Signature Step 2
            */
            var expDate = transactionDate.ToString("ddMMyyyy");
            var strDate = intval32bits(long.Parse(expDate));
            var amt = intval32bits(amount);
            var tempKey2 = strDate + amt;
            var hashKey2 = getHash(tempKey2.ToString());
            //echo "tempKey2 : " . tempKey2;
            //echo " hashKey2 : " . hashKey2 . "<br>";
            /*
            * Generate Key Step 3
            */
            var signature = hashKey1 + hashKey2;
            return Math.Abs(signature).ToString();
        }
    }
}
