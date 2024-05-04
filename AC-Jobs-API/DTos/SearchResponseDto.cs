using System.ComponentModel.DataAnnotations;

namespace AC_Jobs_API.DTos
{
    public class SearchResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IsJob { get; set; } = "false";
        public string IsContact { get; set; } = "false";
        public string IsWorkOrder { get; set; } = "false";
    }

    public class SearchContactsResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IsContact { get; set; }
    }
}
