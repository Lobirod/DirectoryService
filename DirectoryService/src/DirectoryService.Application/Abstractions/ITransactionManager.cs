using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Abstractions;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransactionAsync(CancellationToken cancellationToken);
    
    Task<UnitResult<Error>> SaveChangeAsync(CancellationToken cancellationToken);
}