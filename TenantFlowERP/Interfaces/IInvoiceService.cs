using TenantFlowERP.DTOs;

namespace TenantFlowERP.Interfaces
{
    public interface IInvoiceService
    {
        Task<object> CreateInvoice(CreateInvoiceDto dto);

        Task<object> GetInvoices(
     int page,
     int pageSize,
     string? search);

        Task<object> GetInvoiceById(Guid id);

    }
}