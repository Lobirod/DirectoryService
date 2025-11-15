using System.Data;

namespace DirectoryService.Application.Abstractions;

public interface IDbConnectionFactory
{
    IDbConnection GetDbConnection();
}