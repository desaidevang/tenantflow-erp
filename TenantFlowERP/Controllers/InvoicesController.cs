using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantFlowERP.DTOs;
using TenantFlowERP.Interfaces;
using TenantFlowERP.Services;

namespace TenantFlowERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly InvoicePdfService _invoicePdfService;
        public InvoicesController(
       IInvoiceService invoiceService,
       InvoicePdfService invoicePdfService)
        {
            _invoiceService = invoiceService;
            _invoicePdfService = invoicePdfService;
        }

        // CREATE INVOICE
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<IActionResult> CreateInvoice(
            CreateInvoiceDto dto)
        {
            var result =
                await _invoiceService.CreateInvoice(dto);

            return Ok(result);
        }

        // GET ALL INVOICES
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetInvoices(
    int page = 1,
    int pageSize = 10,
    string? search = null)
        {
            var result =
              await _invoiceService.GetInvoices(
    page,
    pageSize,
    search);

            return Ok(result);
        }

        // GET INVOICE BY ID
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(Guid id)
        {
            var result =
                await _invoiceService.GetInvoiceById(id);

            return Ok(result);
        }


        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadInvoicePdf(Guid id)
        {
            var pdf =
                await _invoicePdfService.GenerateInvoicePdf(id);

            return File(
                pdf,
                "application/pdf",
                $"Invoice-{id}.pdf");
        }
    }
}