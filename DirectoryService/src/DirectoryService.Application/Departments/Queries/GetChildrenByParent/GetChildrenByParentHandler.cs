using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments.Response;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetChildrenByParent;

public class GetChildrenByParentHandler
    : IQueryHandler<Result<GetChildrenByParentResponse, Errors>, GetChildrenByParentQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetChildrenByParentQuery> _validator;

    public GetChildrenByParentHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetChildrenByParentQuery> validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<Result<GetChildrenByParentResponse, Errors>> Handle(
        GetChildrenByParentQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();
        
        var parameters = new DynamicParameters();
        
        parameters.Add("parentId", query.ParentId, DbType.Guid);
        
        parameters.Add("offset", (query.Request.Page - 1) * query.Request.PageSize, DbType.Int32);
        
        parameters.Add("page_size", query.Request.PageSize, DbType.Int32);
        
        using var connection = _connectionFactory.GetDbConnection();

        var departmentsResponse = await connection.QueryAsync<GetChildrenDepartmentByParentResponse>(
            """             
             WITH children AS  (SELECT * FROM departments d
             WHERE d.parent_id = @parentId
             ORDER BY d.created_at
             LIMIT @page_size OFFSET @offset)

             SELECT *, (EXISTS (SELECT 1 FROM departments WHERE parent_id = children.id)) AS has_more_children
             FROM children
             """,
            param: parameters);
        
        return new GetChildrenByParentResponse(departmentsResponse.ToList());
    }
}