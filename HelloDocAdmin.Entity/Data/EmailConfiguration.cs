using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace HelloDocAdmin.Entity.Data
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


        public string tophone { get; set; }
        public string fromphone { get; set; }
        public string msgbody { get; set; }
        #region SendMail
        public Boolean SendMail(string To, string Subject, string Body)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;


            MailMessage message = new MailMessage();
            message.From = new MailAddress(From);
            message.Subject = Subject;
            message.To.Add(new MailAddress(To));
            message.Body = Body;
            message.IsBodyHtml = true;
            using (var smtpClient = new SmtpClient(SmtpServer))
            {
                smtpClient.Port = Port;
                smtpClient.Credentials = new NetworkCredential(UserName, Password);
                smtpClient.EnableSsl = true;

                smtpClient.Send(message);
            }
            return true;
        }
        #endregion


        public bool sendsms()
        {
            try
            {
                var accountSid = "ACac095573552e7e6fbe9a93fe96518063";
                var authToken = "184384da7e93841bf814c33c6d93f1dc";
                TwilioClient.Init(accountSid, authToken);

                var to = new PhoneNumber("+91" + tophone);
                var message = MessageResource.Create(
                    to,
                    from: new PhoneNumber("+12563716553"), //  From number, must be an SMS-enabled Twilio number ( This will send sms from ur "To" numbers ).  
                    body: msgbody);



                Console.WriteLine(message.Body);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }



        #region Encode_Decode
        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }
        public string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
        #endregion
    }

}