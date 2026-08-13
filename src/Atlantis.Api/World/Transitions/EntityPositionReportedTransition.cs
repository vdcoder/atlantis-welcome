using Atlantis.Api.Common;

namespace Atlantis.Api.World.Transitions;

public sealed record EntityPositionReportedTransition(
    string EntityId,
    string SourceEntityId,
    Position ReportedPosition,
    DateTimeOffset ObservedAt)
    : WorldTransition;