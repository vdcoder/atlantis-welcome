using Atlantis.Api.Citizens.Brain.CortexJobs.Tasks;

namespace Atlantis.Api.Citizens.Brain.CortexJobs.Simulation
{
    public sealed record SimulateCitizenPassTaskCreation
    {
        public required CortexTask CortexTask { get; init; }

        public required SimulateCitizenPassTask SimulationTask
        {
            get;
            init;
        }
    }
}
