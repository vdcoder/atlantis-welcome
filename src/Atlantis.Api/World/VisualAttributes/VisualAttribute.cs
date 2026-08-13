namespace Atlantis.Api.World.VisualAttributes;

public sealed record VisualAttribute(
    int DefinitionId,
    int ValueId,
    string Value,
    string DisplayText,
    int Sequence);