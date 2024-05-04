namespace AC_Jobs_API.DTos
{
    public class EmailDto
    {
        public string fromAddress { get; set; }
        public string toAddress { get; set; }
        public string ccAddress { get; set; }
        public string bccAddress { get; set; }
        public string subject { get; set; }
        public bool isHtmlBody { get; set; }
        public string body { get; set; }
    }


}
