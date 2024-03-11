using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using HelloDocAdmin.Entity.Data;
using HelloDocAdmin.Entity.Models;
using HelloDocAdmin.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;


namespace HelloDocAdmin.Controllers.AdminSite
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IDashboardRepository _dashboardrepo;
        private readonly INotyfService _notyf;

        public ReportController(ApplicationDbContext context, IDashboardRepository dashboardrepo, INotyfService notyf)
        {
            _context = context;
            _dashboardrepo = dashboardrepo;
            _notyf = notyf;

        }

        public IActionResult Index()
        {

            return View();
        }
        public IActionResult DownloadExcel(string status)
        {
            try
            {
               
                var data = _dashboardrepo.GetRequests(status);
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Data");

                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Date Of Birth";
                worksheet.Cell(1, 3).Value = "Requestor";
                worksheet.Cell(1, 4).Value = "Physician Name";
                worksheet.Cell(1, 5).Value = "Date of Service";
                worksheet.Cell(1, 6).Value = "Requested Date";
                worksheet.Cell(1, 7).Value = "Phone Number";
                worksheet.Cell(1, 8).Value = "Address";
                worksheet.Cell(1, 9).Value = "Notes";

                int row = 2;
                foreach (var item in data)
                {

                    worksheet.Cell(row, 1).Value = item.PatientName;
               
                    worksheet.Cell(row, 2).Value = item.Dob.ToString();
                    worksheet.Cell(row, 3).Value = item.Requestor;

                    worksheet.Cell(row, 4).Value = item.ProviderName;

                    worksheet.Cell(row, 5).Value = "123";
                    worksheet.Cell(row, 6).Value = item.RequestedDate;
                    worksheet.Cell(row, 7).Value = item.PhoneNumber;
                    worksheet.Cell(row, 8).Value = item.Address;
                    worksheet.Cell(row, 9).Value = item.Notes;
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                _notyf.Success("data.xlsx file downloaded ...");
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "data.xlsx");
            }
            catch (Exception ex)
            {
                _notyf.Warning( ex.Message);
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

    }
}
