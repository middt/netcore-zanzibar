namespace Zanzibar.Core.Models;

public class RelationTuple
{
    public string Namespace { get; set; } = string.Empty;
    public string Object { get; set; } = string.Empty;
    public string Relation { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string SubjectRelation { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Namespace}:{Object}#{Relation}@{Subject}:{SubjectRelation}";
    }
} 