namespace ExcelDataImport.Models
{
    public class CargoType
    {
        public Guid Id { get; set; }
        public string? Chapter { get; set; }

        public string? ChapterName { get; set; }
        public string? Heading { get; set; } 
        public string? HeadingName { get; set; } 
        public string? HSCode { get; set; }
        public string? Description { get; set; }


    }
}
