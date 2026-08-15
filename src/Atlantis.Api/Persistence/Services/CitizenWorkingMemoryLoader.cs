using Atlantis.Api.Citizens.Brain.Memory.WorkingMemory;
using Atlantis.Api.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Atlantis.Api.Persistence.Services;

public sealed class CitizenWorkingMemoryLoader
{
    private readonly AtlantisDbContext
        _dbContext;

    public CitizenWorkingMemoryLoader(
        AtlantisDbContext dbContext)
    {
        _dbContext =
            dbContext;
    }

    public async Task<CitizenWorkingMemory>
        LoadAsync(
            string citizenId,
            CancellationToken cancellationToken =
                default)
    {
        var lineRecords =
            await LoadCurrentLinesAsync(
                citizenId,
                cancellationToken);

        var streamRecords =
            await LoadCurrentStreamAsync(
                citizenId,
                cancellationToken);

        var lines =
            lineRecords
                .Select(
                    record =>
                        new WorkingMemoryLine(
                            Id:
                                record.Id,

                            LineNumber:
                                record.LineNumber,

                            Content:
                                record.Content,

                            ContentTokenCount:
                                record.ContentTokenCount,

                            CreatedAt:
                                record.CreatedAt))
                .ToList();

        var stream =
            streamRecords
                .Select(
                    record =>
                        new MemoryStreamEntry(
                            Id:
                                record.Id,

                            Content:
                                record.Content,

                            ContentTokenCount:
                                record.ContentTokenCount,

                            CreatedAt:
                                record.CreatedAt))
                .ToList();

        return new CitizenWorkingMemory(
            citizenId,
            lines,
            stream);
    }

    private async Task<
        IReadOnlyList<CitizenWorkingMemoryLineRecord>>
        LoadCurrentLinesAsync(
            string citizenId,
            CancellationToken cancellationToken)
    {
        return await _dbContext
            .CitizenWorkingMemoryLines
            .FromSqlInterpolated(
                $"""
                SELECT DISTINCT ON (line_number)
                    id,
                    citizen_id,
                    line_number,
                    content,
                    content_token_count,
                    created_at

                FROM citizen_working_memory_lines

                WHERE citizen_id =
                    {citizenId}

                ORDER BY
                    line_number,
                    created_at DESC,
                    id DESC
                """)
            .AsNoTracking()
            .ToListAsync(
                cancellationToken);
    }

    private async Task<
        IReadOnlyList<CitizenMemoryStreamEntryRecord>>
        LoadCurrentStreamAsync(
            string citizenId,
            CancellationToken cancellationToken)
    {
        return await _dbContext
            .CitizenMemoryStreamEntries
            .FromSqlInterpolated(
                $"""
                WITH newest_entries AS
                (
                    SELECT
                        id,
                        citizen_id,
                        content,
                        content_token_count,
                        created_at,

                        SUM(
                            content_token_count +
                            {WorkingMemoryLimits.StreamEntryHeaderTokens}
                        )
                        OVER
                        (
                            ORDER BY
                                created_at DESC,
                                id DESC
                        )
                        AS running_total

                    FROM citizen_memory_stream_entries

                    WHERE citizen_id =
                        {citizenId}
                )

                SELECT
                    id,
                    citizen_id,
                    content,
                    content_token_count,
                    created_at

                FROM newest_entries

                WHERE running_total <=
                    {WorkingMemoryLimits.MaxStreamTokens}

                ORDER BY
                    created_at,
                    id
                """)
            .AsNoTracking()
            .ToListAsync(
                cancellationToken);
    }
}