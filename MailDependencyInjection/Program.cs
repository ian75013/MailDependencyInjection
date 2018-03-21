using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailDependencyInjection
{
    interface IFAI
    {

        void SetFAI(ref SmtpClient client,string email,string password);
    }

    interface ILogger
    {
        void Log(string message);
    }

    class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("{0} : {1}",DateTime.Now,message);
        }
    }

    class GoogleFAI : IFAI
    {
        public void SetFAI(ref SmtpClient client,string email,string password)
        {
            client.EnableSsl = true;
            client.Port = 587;
            client.Host = "smtp.googlemail.com";
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(email, password);
           

        }
    }

    class EventLogger : ILogger
    {
        public void Log(string message)
        {
            if (!EventLog.SourceExists("MySource"))
            {
                //An event log source should not be created and immediately used.
                //There is a latency time to enable the source, it should be created
                //prior to executing the application that uses the source.
                //Execute this sample a second time to use the new source.
                EventLog.CreateEventSource("MySource", "MyNewLog");
                Console.WriteLine("CreatedEventSource");
                Console.WriteLine("Exiting, execute the application a second time to use the source.");
                // The source is created.  Exit the application to allow it to be registered.
                return;
            }

            // Create an EventLog instance and assign its source.
            EventLog myLog = new EventLog();
            myLog.Source = "MySource";
            myLog.WriteEntry(message);
        }
    }

    class MailService
    {
        private ILogger logger;
        private IFAI fai;
        private SmtpClient client;

        public MailService(ILogger logger,IFAI fai,string email,string password)
        {
            this.logger = logger;
            this.fai = fai;
            client = new SmtpClient();
            fai.SetFAI(ref client,email, password);
        }

        public int SendEmail(string adressTo,string adressFrom, string subject, string body)
        {
            logger.Log("Creating mail message ....");
            MailMessage objeto_mail = new MailMessage();
            objeto_mail.From = new MailAddress(adressFrom);
            objeto_mail.To.Add(new MailAddress(adressTo));
            objeto_mail.Subject = subject;
            objeto_mail.Body = body;

            logger.Log("Sending message ....");
            client.Send(objeto_mail);
            logger.Log("Send message succesfully");
            return 0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var mailService = new MailService(new EventLogger(), new GoogleFAI(),"smatti.sofian@gmail.com","69ay9vtmarcel@antihack");
            mailService.SendEmail("smatti.sofian@free.fr","smatti.sofian@gmail.com", "Essai", "Sucessful");
        }
    }
}
