using System.ComponentModel.DataAnnotations;

namespace VideoTest
{
    public class ApiOptions
    {
        public const string SectionName = "ApiData";

        [Required]
        public string Url { get; set; } = string.Empty;
        [Required]
        public string Login { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        public int StreamsCount { get; set; } = 2;
    }
}
