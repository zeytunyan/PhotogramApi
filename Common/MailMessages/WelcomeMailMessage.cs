using System.Net.Mail;

namespace Common.MailMessages
{
    public class WelcomeMailMessage : PersonalMailMessage
    {
        public WelcomeMailMessage(string addressee) : base(addressee)
        {
            SetSubjectAndBody();
        }

        public WelcomeMailMessage(string addressee, string from, string to) :
            base(addressee, from, to)
        {
            SetSubjectAndBody();
        }

        public WelcomeMailMessage(MailAddress from, MailAddress to) :
            base(from, to)
        {
            SetSubjectAndBody();
        }

        private void SetSubjectAndBody()
        {
            Subject = "Welcome to Photogram";
            Body = @"Hello, {Adressee}! Thank you for registering on our service.

We are happy to inform you that your personal account has been created, and you can start sharing your photos right now!

You received this message because your email address was registered on the Photogram service.
If this letter has come to you by mistake, please ignore it and accept our apologies.

With respect,
Photogram command.";
        }
    }
}
