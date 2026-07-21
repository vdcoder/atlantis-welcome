using Microsoft.AspNetCore.Mvc;
using Atlantis.Api.Models;
using Atlantis.Api.World;

namespace Atlantis.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorldController : ControllerBase
    {
        private readonly WorldRuntime _worldRuntime;

        public WorldController(WorldRuntime worldRuntime)
        {
            _worldRuntime = worldRuntime;
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
}
