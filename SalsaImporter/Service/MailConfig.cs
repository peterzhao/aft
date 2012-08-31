namespace SalsaImporter.Service
{
    public class MailConfig
    {
        private string _sender;
        private string _recipient;
        private string _subject;
        private string _host;
        private int _port;

        public MailConfig(string sender, string recipient, string subject, string host, int port)
        {
            _sender = sender;
            _recipient = recipient;
            _subject = subject;
            _host = host;
            _port = port;
        }

        public string Sender
        {
            get { return _sender; }
        }

        public string Recipient
        {
            get { return _recipient; }
        }

        public string Subject
        {
            get { return _subject; }
        }

        public string Host
        {
            get { return _host; }
        }

        public int Port
        {
            get { return _port; }
        }
    }
}