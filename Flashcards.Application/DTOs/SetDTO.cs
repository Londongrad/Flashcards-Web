namespace Flashcards.Application.DTOs
{
    public class SetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool HasImages { get; set; }
        public int WordCount { get; set; }
    }
}
