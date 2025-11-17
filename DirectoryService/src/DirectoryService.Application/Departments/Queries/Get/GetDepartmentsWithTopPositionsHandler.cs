using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Contracts.Departments.Response;
using Shared;

namespace DirectoryService.Application.Departments.Queries.Get;

public class GetDepartmentsWithTopPositionsHandler : IQueryHandler<Result<GetDepartmentsWithTopPositionsResponse, Errors>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetDepartmentsWithTopPositionsHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<GetDepartmentsWithTopPositionsResponse, Errors>> Handle(CancellationToken cancellationToken)
    {
        var connection = _connectionFactory.GetDbConnection();

        var departmentsResponse = await connection.QueryAsync<GetDepartmentResponse>(
            """
                SELECT DISTINCT d.*,
                dp.positions_count AS positionsCount
                FROM departments d
                JOIN (SELECT department_id, COUNT(*) AS positions_count
                    FROM department_position
                    GROUP BY department_id) dp ON d.id = dp.department_id
                ORDER BY dp.positions_count DESC
                LIMIT 5
            """);
        return new GetDepartmentsWithTopPositionsResponse(departmentsResponse.ToList());
    }
}