using System.Collections.Generic;
using System.Net.Mail;

namespace Sugary.WebApi.Models
{
    public class SendMailModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> To { get; set; }
        public Attachment Attachment { get; set; }
    }
}
