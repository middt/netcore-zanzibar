using Zanzibar.Core.Models;

namespace Zanzibar.Core.Interfaces;

public interface IZanzibarService
{
    Task<bool> CheckPermissionAsync(RelationTuple tuple);
    Task AddRelationAsync(RelationTuple tuple);
    Task RemoveRelationAsync(RelationTuple tuple);
    Task<List<RelationTuple>> GetRelationsAsync(string @namespace, string @object, string relation);
    Task<bool> HasPermissionAsync(string @namespace, string @object, string relation, string subject);
} 