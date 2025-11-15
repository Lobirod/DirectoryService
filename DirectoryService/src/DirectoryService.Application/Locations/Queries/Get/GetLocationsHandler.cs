using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Locations.Response;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Locations.Queries.Get;

public class GetLocationsHandler : IQueryHandler<Result<GetLocationsResponse, Errors>, GetLocationsQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetLocationsQuery> _validator;

    public GetLocationsHandler(
        IDbConnectionFactory connectionFactory,
        IValidator<GetLocationsQuery> validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<Result<GetLocationsResponse, Errors>> Handle(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.ToList();

        var parameters = new DynamicParameters();

        var conditions = new List<string>();

        if (query.Request.DepartmentIds != null)
        {
            parameters.Add("department_ids", query.Request.DepartmentIds);
            conditions.Add("""
                           EXISTS (
                               SELECT 1
                               FROM department_location dl
                               WHERE dl.location_id = l.id
                                 AND dl.department_id = ANY(@department_ids)
                           )
                           """);
        }

        if (!string.IsNullOrWhiteSpace(query.Request.Search))
        {
            parameters.Add("search", $"%{query.Request.Search}%");
            conditions.Add("l.name ILIKE @search");
        }

        if (query.Request.IsActive.HasValue)
        {
            parameters.Add("is_active", query.Request.IsActive);
            conditions.Add("l.is_active = @is_active");
        }

        parameters.Add("offset", (query.Request.Page - 1) * query.Request.PageSize, DbType.Int32);
        
        parameters.Add("page_size", query.Request.PageSize, DbType.Int32);

        string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;

        string direction = query.Request.SortDirection.ToLower() == "asc" ? "ASC" : "DESC";

        string orderByField = query.Request.SortBy.ToLower() switch
        {
            "name" => "l.name",
            _ => "l.created_at"
        };

        string orderByString = $"ORDER BY {orderByField} {direction}";

        long? totalCount = null;
        
        var connection = _connectionFactory.GetDbConnection();

        var locationsResponse = await connection.QueryAsync<
            GetLocationResponse,
            long,
            GetLocationResponse>(
            $"""
             SELECT l.*, COUNT(*) OVER () AS total_count FROM locations l
             {whereClause}
             {orderByString}
             LIMIT @page_size OFFSET @offset
             """,
            param: parameters,
            splitOn: "total_count",
            map: (location, count) =>
            {
                totalCount ??= count;

                return location;
            });

        return new GetLocationsResponse(locationsResponse.ToList(), totalCount ?? 0);
    }
}