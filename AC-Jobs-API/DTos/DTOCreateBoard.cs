namespace AC_Jobs_API.DTos
{
    public class DTOCreateBoard
    {
        public long? Id { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectType { get; set; }
        public string? ProjectColor { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public List<long>? AccessUsers { get; set; }
        public string? CardTitle { get; set; }
        public long? FolderId { get; set; }
        public List<DTOBoardStatus>? Statuses { get; set; }


    }



}
