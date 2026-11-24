using System.ComponentModel.DataAnnotations;

namespace VideoTest
{
    public class ApiOptions
    {
        public const string SectionName = "ApiData";

        public string Url { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int StreamsCount { get; set; } = 2;
    }
}
