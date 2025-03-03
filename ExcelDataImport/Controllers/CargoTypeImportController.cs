using ExcelDataImport.Models;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;

namespace ExcelDataImport.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CargoTypeImportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CargoTypeImportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not provided.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            var data = ParseExcelData(stream);
            var results = new List<CargoType>();
            var errorRecords = new List<object>();

            foreach (var row in data)
            {
                var (isValid, errorMessage) = ValidateRow(row);
                if (isValid)
                {
                    results.Add(row);
                }
                else
                {
                    errorRecords.Add(new { Row = row, Error = errorMessage });
                }
            }

            if (results.Any())
            {
                await _context.CargoTypes.AddRangeAsync(results);
                await _context.SaveChangesAsync();
            }

            return Ok(new { Success = results, Errors = errorRecords });
        }

        private List<CargoType> ParseExcelData(MemoryStream stream)
        {
            using (var excelEngine = new ExcelEngine())
            {
              
                var application = excelEngine.Excel;

                var workbook = application.Workbooks.Open(stream);

                var worksheet = workbook.Worksheets[0];

                var data = new List<CargoType>();

                for (int row = 2; row <= worksheet.Rows.Length; row++)
                {
                    var cargoType = new CargoType
                    {
                        Chapter = worksheet[row, 1].Text,
                        ChapterName = worksheet[row, 2].Text,
                        Heading = worksheet[row, 3].Text,
                        HeadingName = worksheet[row, 4].Text,
                        HSCode = worksheet[row, 5].Text,
                        Description = worksheet[row, 6].Text
                    };
                    data.Add(cargoType);
                }
                return data;
            }
        }

        private (bool IsValid, string ErrorMessage) ValidateRow(CargoType cargoType)
        {
            var errorMessages = new List<string>();
            if (string.IsNullOrWhiteSpace(cargoType.ChapterName))
                errorMessages.Add("Chapter name is required.");
            if (string.IsNullOrWhiteSpace(cargoType.Heading))
                errorMessages.Add("Heading is required.");
            if (string.IsNullOrWhiteSpace(cargoType.HeadingName))
                errorMessages.Add("Heading name is required.");
            if (string.IsNullOrWhiteSpace(cargoType.HSCode))
                errorMessages.Add("HS Code is required.");
            if (string.IsNullOrWhiteSpace(cargoType.Description))
                errorMessages.Add("Description is required");


            return (errorMessages.Count == 0, string.Join("; ", errorMessages));
        }
    }
}
