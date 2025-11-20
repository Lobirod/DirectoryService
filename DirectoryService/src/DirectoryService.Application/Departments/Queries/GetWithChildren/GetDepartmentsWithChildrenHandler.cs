using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments.Response;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetWithChildren;

public class GetDepartmentsWithChildrenHandler
    : IQueryHandler<Result<GetDepartmentsWithChildrenResponse, Errors>, GetDepartmentsWithChildrenQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetDepartmentsWithChildrenQuery> _validator;

    public GetDepartmentsWithChildrenHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetDepartmentsWithChildrenQuery> validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<Result<GetDepartmentsWithChildrenResponse, Errors>> Handle(
        GetDepartmentsWithChildrenQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();
        
        int pageSize = query.Request.PageSize;
        
        int offset = (query.Request.Page - 1) * pageSize;

        int prefetch = query.Request.Prefetch;
        
        using var connection = _connectionFactory.GetDbConnection();

        var departmentsResponse = await connection.QueryAsync<GetDepartmentWithChildResponse>(
            $"""
             WITH roots AS  (SELECT * FROM departments d
             WHERE d.parent_id IS NULL
             ORDER BY d.created_at
             LIMIT {pageSize} OFFSET {offset})
             
             SELECT *, 
             (EXISTS (SELECT 1 FROM departments d WHERE d.parent_id = r.id OFFSET {prefetch} LIMIT 1))
             AS has_more_children
             FROM roots r
             
             UNION ALL
             
             SELECT c.*, 
             (EXISTS (SELECT 1 FROM departments d WHERE d.parent_id = c.id))
             AS has_more_children
             FROM roots r
                CROSS JOIN LATERAL (SELECT * FROM departments d
                                    WHERE d.parent_id = r.id
                                    ORDER BY d.created_at
                                    LIMIT {prefetch}) c
             """);
        
        return new GetDepartmentsWithChildrenResponse(departmentsResponse.ToList());
    }
}