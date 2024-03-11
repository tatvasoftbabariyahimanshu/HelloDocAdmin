using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocAdmin.Repositories
{
    public class ENC
    {
        public string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }

        public string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
        public string EncryptDate(DateTime date)
        {
            ENC data = new ENC();
            string dateString = date.ToString("yyyy-MM-ddTHH:mm:ss");
            return data.EnryptString(dateString);
        }

        public DateTime DecryptDate(string encryptedDate)
        {
            ENC data = new ENC();
            string decryptedString = data.DecryptString(encryptedDate);
            return DateTime.ParseExact(decryptedString, "yyyy-MM-ddTHH:mm:ss", null);
        }
    }
}
