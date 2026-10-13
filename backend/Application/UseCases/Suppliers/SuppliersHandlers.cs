using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Suppliers;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Suppliers;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, Guid>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IClock _clock;

    public CreateSupplierHandler(
        ISupplierRepository supplierRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IClock clock)
    {
        _supplierRepository = supplierRepository;
        _context = context;
        _currentUserService = currentUserService;
        _clock = clock;
    }

    public async Task<Guid> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = Supplier.Create(request.Name, _clock.UtcNow, request.Phone, request.Email, _currentUserService.RestaurantId);

        _supplierRepository.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}

public class DeleteSupplierHandler : IRequestHandler<DeleteSupplierCommand>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IApplicationDbContext _context;

    public DeleteSupplierHandler(ISupplierRepository supplierRepository, IApplicationDbContext context)
    {
        _supplierRepository = supplierRepository;
        _context = context;
    }

    public async Task Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier is null)
            throw new NotFoundException($"Fornecedor com Id '{request.Id}' não encontrado.");

        _supplierRepository.Remove(supplier);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class UpdateSupplierHandler : IRequestHandler<UpdateSupplierCommand>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IApplicationDbContext _context;

    public UpdateSupplierHandler(ISupplierRepository supplierRepository, IApplicationDbContext context)
    {
        _supplierRepository = supplierRepository;
        _context = context;
    }

    public async Task Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier is null)
            throw new NotFoundException($"Fornecedor com Id '{request.Id}' não encontrado.");

        supplier.Update(request.Name, request.Phone, request.Email);

        _supplierRepository.Update(supplier);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDetailsDto?>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierByIdHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDetailsDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier is null)
            return null;

        return new SupplierDetailsDto(
            supplier.Id,
            supplier.Name,
            supplier.Phone,
            supplier.Email,
            supplier.CreatedAt);
    }
}

public class GetSuppliersHandler : IRequestHandler<GetSuppliersQuery, PagedResult<SupplierListItemDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSuppliersHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<PagedResult<SupplierListItemDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var (suppliers, totalCount) = await _supplierRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Name,
            request.SortDirection,
            cancellationToken);

        var items = suppliers
            .Select(supplier => new SupplierListItemDto(
                supplier.Id,
                supplier.Name,
                supplier.Phone,
                supplier.Email,
                supplier.CreatedAt))
            .ToList();

        return new PagedResult<SupplierListItemDto>(items, request.Page, request.PageSize, totalCount);
    }
}
