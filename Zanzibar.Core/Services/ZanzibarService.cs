using Zanzibar.Core.Interfaces;
using Zanzibar.Core.Models;
using Microsoft.Extensions.Logging;

namespace Zanzibar.Core.Services;

public class ZanzibarService : IZanzibarService
{
    private readonly ILogger<ZanzibarService> _logger;
    private readonly List<RelationTuple> _relations = new();

    public ZanzibarService(ILogger<ZanzibarService> logger)
    {
        _logger = logger;
        _logger.LogInformation("ZanzibarService initialized");
    }

    public async Task<bool> CheckPermissionAsync(RelationTuple tuple)
    {
        _logger.LogInformation("Checking permission for tuple: {Tuple}", tuple);
        _logger.LogInformation("Current relations count: {Count}", _relations.Count);
        return await HasPermissionAsync(tuple.Namespace, tuple.Object, tuple.Relation, tuple.Subject);
    }

    public async Task AddRelationAsync(RelationTuple tuple)
    {
        _logger.LogInformation("Adding relation: {Tuple}", tuple);
        _relations.Add(tuple);
        _logger.LogInformation("Relation added. Total relations: {Count}", _relations.Count);
        await Task.CompletedTask;
    }

    public async Task RemoveRelationAsync(RelationTuple tuple)
    {
        _logger.LogInformation("Removing relation: {Tuple}", tuple);
        var countBefore = _relations.Count;
        _relations.RemoveAll(r => 
            r.Namespace == tuple.Namespace &&
            r.Object == tuple.Object &&
            r.Relation == tuple.Relation &&
            r.Subject == tuple.Subject &&
            r.SubjectRelation == tuple.SubjectRelation);
        _logger.LogInformation("Relations removed: {RemovedCount}. Total relations: {Count}", 
            countBefore - _relations.Count, _relations.Count);
        await Task.CompletedTask;
    }

    public async Task<List<RelationTuple>> GetRelationsAsync(string @namespace, string @object, string relation)
    {
        _logger.LogInformation("Getting relations for {Namespace}:{Object}#{Relation}", @namespace, @object, relation);
        var relations = _relations
            .Where(r => r.Namespace == @namespace && r.Object == @object && r.Relation == relation)
            .ToList();
        _logger.LogInformation("Found {Count} relations", relations.Count);
        return await Task.FromResult(relations);
    }

    public async Task<bool> HasPermissionAsync(string @namespace, string @object, string relation, string subject)
    {
        _logger.LogInformation("=== Starting permission check ===");
        _logger.LogInformation("Checking if {Subject} has {Relation} on {Namespace}:{Object}", 
            subject, relation, @namespace, @object);

        // Direct check
        var directMatch = _relations.Any(r =>
            r.Namespace == @namespace &&
            r.Object == @object &&
            r.Relation == relation &&
            r.Subject == subject);

        if (directMatch)
        {
            _logger.LogInformation("✓ Direct permission found");
            return true;
        }
        _logger.LogInformation("✗ No direct permission found");

        // Check for group memberships
        var groupMemberships = _relations
            .Where(r => r.Namespace == "groups" && 
                       r.Relation == "member" && 
                       r.Subject == subject)
            .Select(r => r.Object)
            .ToList();

        _logger.LogInformation("Found {Count} group memberships for {Subject}", groupMemberships.Count, subject);

        foreach (var group in groupMemberships)
        {
            _logger.LogInformation("Checking group {Group} for permission", group);
            // Check if the group has the permission
            var groupPermission = _relations.Any(r =>
                r.Namespace == @namespace &&
                r.Object == @object &&
                r.Relation == relation &&
                r.Subject == $"group:{group}");

            if (groupPermission)
            {
                _logger.LogInformation("✓ Permission found through group membership in {Group}", group);
                return true;
            }
            _logger.LogInformation("✗ No permission found in group {Group}", group);
        }

        // Check for hierarchical permissions
        var parentRelations = _relations
            .Where(r => r.Namespace == @namespace &&
                       r.Object == @object &&
                       r.Relation == "parent")
            .ToList();

        _logger.LogInformation("Found {Count} parent relations for {Object}", parentRelations.Count, @object);

        foreach (var parentRelation in parentRelations)
        {
            var parentObject = parentRelation.Subject.Replace("doc:", "");
            _logger.LogInformation("Checking parent document {Parent} for permission", parentObject);
            
            // Check if the subject has permission on the parent
            var parentPermission = await HasPermissionAsync(
                @namespace,
                parentObject,
                relation,
                subject);

            if (parentPermission)
            {
                _logger.LogInformation("✓ Permission found through parent document {Parent}", parentObject);
                return true;
            }
            _logger.LogInformation("✗ No permission found in parent document {Parent}", parentObject);
        }

        _logger.LogInformation("=== No permission found in any check ===");
        return false;
    }
} 