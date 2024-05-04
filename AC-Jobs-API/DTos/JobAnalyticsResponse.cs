using System.ComponentModel.DataAnnotations;

namespace AC_Jobs_API.DTos
{
    public class JobAnalyticsResponseDto
    {
        public int TotalJobs { get; set; }
        public int NewJobs { get; set; }
        public int SourceWiseCount { get; set; }
        public double PercentageIncrease { get; set; }
        public int JobsCompleted { get; set; }
        public List<YearWiseJobsResponseDto> YearWiseJobsCount { get; set; }
        public List<SourceWiseJobsResponseDto> SourceWiseResponse { get; set; }
    }

    public class YearWiseJobsResponseDto
    {
        public int Year { get; set; }
        public int Count { get; set; }
        public List<MonthWiseJobsResponseDto> MonthWiseJobsCount { get; set; }
    }

    public class MonthWiseJobsResponseDto
    {
        public string? Month { get; set; }
        public int? Count { get; set; }
    }

    public class SourceWiseJobsResponseDto
    {
        public long? SourceId { get; set; }
        public int? Count { get; set; }
        public string? SourceName { get; set; }
    }
}
