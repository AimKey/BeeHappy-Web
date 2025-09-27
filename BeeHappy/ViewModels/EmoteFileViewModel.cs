namespace BeeHappy.ViewModels
{
    public class EmoteFileViewModel
    {
        public string? Format { get; set; }
        public string? Url { get; set; }
        public int Size { get; set; }

        // thêm cho upload
        public IFormFile File { get; set; }
    }
}
