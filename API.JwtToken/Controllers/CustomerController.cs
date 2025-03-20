using API.JwtToken.Modal;
using API.JwtToken.Service;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;

namespace API.JwtToken.Controllers
{
    [Authorize]
    //[DisableCors]
    [EnableRateLimiting("fixedwindow")] //use RateLimiting here
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService service;

        public CustomerController(ICustomerService service)
        {
            this.service = service;
        }

        //[EnableCors("")]
        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task< IActionResult> GetAll()
        {
            var data = await service.GetAll();
            if (data ==null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [DisableRateLimiting] //use this if we dont want to use RateLimiting here
        [HttpGet("GetByCode")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var data = await service.GetByCode(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CustomerModal customer)
        {
            var data = await service.Create(customer);
            return Ok(data);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(CustomerModal customer, string code )
        {
            var data = await service.Update(customer, code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult>Remove(string code)
        {
            var data = await service.Remove(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [AllowAnonymous]
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                //create a DataTable
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Code", typeof(string));
                dataTable.Columns.Add("Name", typeof(string));
                dataTable.Columns.Add("Email", typeof(string));
                dataTable.Columns.Add("Phone", typeof(string));
                dataTable.Columns.Add("Creditlimit", typeof(string));

                //get Data Service
                var data = await this.service.GetAll();

                //Add data rows into DataTable
                if (data != null && data.Count > 0)
                {
                    data.ForEach(item =>
                    {
                        dataTable.Rows.Add(item.Code, item.Name, item.Email, item.Phone, item.Creditlimit);
                    });
                }
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.AddWorksheet(dataTable, "Customer Infor");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customer.xlsx");
                    }
                }

                //Add Data into XLWorkbook
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
