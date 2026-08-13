using Atlantis.Api.Common;
using Atlantis.Api.World;
using Atlantis.Api.World.Actions;
using Microsoft.AspNetCore.Mvc;

namespace Atlantis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorldController : ControllerBase
    {
        private readonly WorldRuntime _worldRuntime;

        private readonly WorldActionProcessor
            _worldActionProcessor;

        public WorldController(
            WorldRuntime worldRuntime,
            WorldActionProcessor worldActionProcessor)
        {
            _worldRuntime = worldRuntime;
            _worldActionProcessor = worldActionProcessor;
        }

        [HttpGet(Name = "GetWorld")]
        public ActionResult<WorldSnapshot> Get()
        {
            return Ok(_worldRuntime.GetSnapshot());
        }

        [HttpPost("entities/{entityId}/move")]
        public async Task<ActionResult<MoveEntityResult>> MoveEntity(string entityId, [FromBody] MoveEntityRequestDto request)
        {
            try
            {
                var actorId = request.ActorId ?? "system";
                var result = await _worldRuntime.MoveEntityAsync(
                    actorId,
                    entityId,
                    new Position(request.Destination.X, request.Destination.Y, request.Destination.Z));
                return Ok(result);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("entities/{entityId}/say")]
        public async Task<IActionResult> SayAsync(
            string entityId,
            [FromBody] SayEntityRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var transitions =
                    await _worldActionProcessor.ProcessAsync(
                        new SayRequest(
                            ActorId:
                                request.ActorId,

                            EntityId:
                                entityId,

                            Text:
                                request.Text));

                return Ok(
                    new
                    {
                        transitionCount =
                            transitions.Count
                    });
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost(
            "entities/{entityId}/report-position-observation")]
        public async Task<IActionResult>
            ReportPositionObservationAsync(
                string entityId,
                [FromBody]
                ReportPositionObservationDto request)
        {
            try
            {
                var transitions =
                    await _worldActionProcessor.ProcessAsync(
                        new ReportPositionObservationRequest(
                            ActorId:
                                request.ActorId,

                            ObservedEntityId:
                                entityId,

                            Position:
                                new Position(
                                    request.Position.X,
                                    request.Position.Y,
                                    request.Position.Z)));

                return Ok(
                    new
                    {
                        transitionCount =
                            transitions.Count
                    });
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }

    public sealed class MoveEntityRequestDto
    {
        public string? ActorId { get; set; }
        public string? EntityId { get; set; }
        public PositionDto Destination { get; set; } = new PositionDto();
    }

    public sealed class PositionDto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public sealed class SayEntityRequestDto
    {
        public string? ActorId
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        } =
            string.Empty;
    }

    public sealed class ReportPositionObservationDto
    {
        public string ActorId
        {
            get;
            set;
        } =
            string.Empty;

        public PositionDto Position
        {
            get;
            set;
        } =
            new();
    }
}
