namespace VideoTest.Models
{
    public class Channel
    {
        public Guid Id { get; set; }
        // В коде не используется, но если понадобится узнать название камеры
        public string Name { get; set; } = string.Empty;
        public bool IsDisabled { get; set; }
    }
}
