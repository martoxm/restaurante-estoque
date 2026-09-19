using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Commands.CreateSupplier;

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
