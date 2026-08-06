using Atlantis.Api.Citizens.Brain.CortexJobs.Context;

namespace Atlantis.Api.Citizens.Brain.CortexContext.DynamicContext.Generators;

public sealed class PrimedCortexTaskContextGenerator
    : IDynamicContextGenerator
{
    private readonly string _workerCitizenId;

    private readonly IPrimedCortexTaskContextLoader
        _loader;

    public PrimedCortexTaskContextGenerator(
        string workerCitizenId,
        IPrimedCortexTaskContextLoader loader)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            workerCitizenId);

        _workerCitizenId =
            workerCitizenId;

        _loader =
            loader ??
            throw new ArgumentNullException(
                nameof(loader));
    }

    public int PrefixStabilityHint =>
        800;

    public PrimedCortexTaskContext?
        GeneratedTask
    {
        get;
        private set;
    }

    public async ValueTask GenerateAsync(
        ContextWriter writer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            writer);

        GeneratedTask =
            await _loader.LoadAsync(
                _workerCitizenId,
                cancellationToken);

        if (GeneratedTask is null)
        {
            return;
        }

        writer.WriteLine(
            "<authorized_cortex_task>");

        writer.WriteLine(
            $"  <assignment_id>" +
            $"{GeneratedTask.AssignmentId}" +
            $"</assignment_id>");

        writer.WriteLine(
            $"  <task_id>" +
            $"{GeneratedTask.CortexTaskId}" +
            $"</task_id>");

        writer.WriteLine(
            $"  <task_type>" +
            $"{GeneratedTask.CortexTaskType}" +
            $"</task_type>");

        writer.WriteLine(
            "  <instructions_and_inputs>");

        writer.WriteRaw(
            GeneratedTask.InstructionsAndInputs);

        if (!GeneratedTask.InstructionsAndInputs
                .EndsWith(
                    Environment.NewLine,
                    StringComparison.Ordinal))
        {
            writer.WriteLine();
        }

        writer.WriteLine(
            "  </instructions_and_inputs>");

        writer.WriteLine(
            "</authorized_cortex_task>");

        writer.WriteLine();
    }
}