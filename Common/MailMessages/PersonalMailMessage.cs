using System.Net.Mail;

namespace Common.MailMessages
{
    public class PersonalMailMessage : MailMessage
    {
        public string Addressee { get; set; }

        public PersonalMailMessage(string addressee) : base() 
        {
            Addressee = addressee;
        }

        public PersonalMailMessage(string addressee, string from, string to) :
            this(new MailAddress(from), new MailAddress(to, addressee))
        { }

        public PersonalMailMessage(string addressee, string from, string to, string? subject, string? body) : 
            this(addressee, from, to)
        {
            Subject = subject;
            Body = body;
        }

        public PersonalMailMessage(MailAddress from, MailAddress to) :
            this(to.DisplayName)
        {
            From = from;
            To.Add(to);
        }
    }
}
