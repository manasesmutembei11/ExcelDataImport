using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using ExcelDataImport.Models;

namespace ExcelDataImport.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericImportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenericImportController(ApplicationDbContext context)
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

            var (tableName, data) = ParseExcelData(stream);

            if (!data.Any())
                return BadRequest("No valid data found in the Excel file.");

            await CreateOrUpdateTable(tableName, data);
            await InsertData(tableName, data);

            return Ok(new { Message = "Data imported successfully.", Table = tableName });
        }

        private (string TableName, List<Dictionary<string, object>>) ParseExcelData(MemoryStream stream)
        {
            using var excelEngine = new ExcelEngine();
            var application = excelEngine.Excel;
            var workbook = application.Workbooks.Open(stream);
            var worksheet = workbook.Worksheets[0];

            var data = new List<Dictionary<string, object>>();
            string tableName = worksheet.Name.Replace(" ", "_");

            var headers = new List<string>();

            // Read headers (First row)
            for (int col = 1; col <= worksheet.Columns.Length; col++)
            {
                headers.Add(worksheet[1, col].Text.Trim().Replace(" ", "_"));
            }

            // Read Data Rows
            for (int row = 2; row <= worksheet.Rows.Length; row++)
            {
                var rowData = new Dictionary<string, object>();

                for (int col = 1; col <= headers.Count; col++)
                {
                    rowData[headers[col - 1]] = worksheet[row, col].Text;
                }

                data.Add(rowData);
            }

            return (tableName, data);
        }

        private async Task CreateOrUpdateTable(string tableName, List<Dictionary<string, object>> data)
        {
            if (!data.Any()) return;

            var firstRow = data.First();
            var columns = firstRow.Keys.Select(col => $"[{col}] NVARCHAR(MAX)").ToList();

            string createTableQuery = $@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}')
        BEGIN
            CREATE TABLE {tableName} (
                Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY, 
                {string.Join(", ", columns)}
            );
        END";

            await _context.Database.ExecuteSqlRawAsync(createTableQuery);
        }


        private async Task InsertData(string tableName, List<Dictionary<string, object>> data)
        {
            foreach (var row in data)
            {
                if (!row.ContainsKey("Id"))
                {
                    row["Id"] = Guid.NewGuid(); 
                }

                var columns = string.Join(", ", row.Keys);
                var values = string.Join(", ", row.Values.Select(v => v != null ? $"'{v.ToString().Replace("'", "''")}'" : "NULL"));

                string insertQuery = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                await _context.Database.ExecuteSqlRawAsync(insertQuery);
            }
        }


    }
}
