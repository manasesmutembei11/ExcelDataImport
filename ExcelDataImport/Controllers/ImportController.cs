using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;
using ExcelDataImport.Models;

namespace ExcelDataImport.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ImportController(ApplicationDbContext context)
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
            var results = new List<ExpaqClaim>();
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
                await _context.Claims.AddRangeAsync(results);
                await _context.SaveChangesAsync();
            }

            return Ok(new { Success = results, Errors = errorRecords });
        }

        private List<ExpaqClaim> ParseExcelData(MemoryStream stream)
        {
            using (var excelEngine = new ExcelEngine())
            {
                var application = excelEngine.Excel;

                var workbook = application.Workbooks.Open(stream);

                var worksheet = workbook.Worksheets[0];

                var data = new List<ExpaqClaim>();

                for (int row = 2; row <= worksheet.Rows.Length; row++)
                {
                    var claim = new ExpaqClaim
                    {
                        DocumentType = worksheet[row, 1].Text,
                        DateFiled = ParseDate(worksheet[row, 2].Text),
                        DateReceived = ParseDate(worksheet[row, 3].Text),
                        CourtStation = worksheet[row, 4].Text,
                        Rank = worksheet[row, 5].Text,
                        CaseNo = worksheet[row, 6].Text,
                        Year = worksheet[row, 7].Text,
                        Plaintiff = worksheet[row, 8].Text,
                        Defendant = worksheet[row, 9].Text,
                        ThirdPartyAdvocate = worksheet[row, 10].Text,
                        InjuryType = worksheet[row, 11].Text,
                        LossDate = ParseDate(worksheet[row, 12].Text),
                        InsuredMV = worksheet[row, 13].Text,
                        ClaimNo = worksheet[row, 14].Text,
                        Region = worksheet[row, 15].Text,
                        OurAdvocate = worksheet[row, 16].Text
                    };
                    data.Add(claim);
                }
                return data;
            }
        }

        private (bool IsValid, string ErrorMessage) ValidateRow(ExpaqClaim claim)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(claim.DocumentType))
                errorMessages.Add("DocumentType is required.");
            if (!claim.DateFiled.HasValue)
                errorMessages.Add("DateFiled is required.");
            if (!claim.DateReceived.HasValue)
                errorMessages.Add("DateReceived is required.");
            if (string.IsNullOrWhiteSpace(claim.CourtStation))
                errorMessages.Add("CourtStation is required.");
            if (string.IsNullOrWhiteSpace(claim.CaseNo))
                errorMessages.Add("CaseNo is required.");
            if (string.IsNullOrWhiteSpace(claim.Year))
                errorMessages.Add("Year is required.");
            if (claim.LossDate.HasValue && claim.LossDate > DateTime.Now)
                errorMessages.Add("LossDate cannot be in the future.");

            return (errorMessages.Count == 0, string.Join("; ", errorMessages));
        }

        private DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            DateTime parsedDate;
            return DateTime.TryParse(dateString, out parsedDate) ? parsedDate : (DateTime?)null;
        }
    }
}
