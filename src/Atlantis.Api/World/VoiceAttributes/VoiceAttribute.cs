namespace Atlantis.Api.World.VoiceAttributes;

public sealed record VoiceAttribute(
    int DefinitionId,
    int ValueId,
    string Value,
    string DisplayText,
    int Sequence);