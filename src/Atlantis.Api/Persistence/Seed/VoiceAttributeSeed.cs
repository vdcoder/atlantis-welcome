namespace Atlantis.Api.Persistence.Seed;

using Atlantis.Api.Persistence.Records;
using Atlantis.Api.World.VoiceAttributes;
using Microsoft.EntityFrameworkCore;

public static class VoiceAttributeSeed
{
    public static async Task EnsureSeededAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            dbContext);

        await EnsureDefinitionsAsync(
            dbContext,
            cancellationToken);

        await EnsureValuesAsync(
            dbContext,
            cancellationToken);

        await EnsureEntityAssignmentsAsync(
            dbContext,
            cancellationToken);
    }

    private static async Task EnsureDefinitionsAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var definitions =
            new[]
            {
                new VoiceAttributeDefinitionRecord
                {
                    Id =
                        KnownVoiceAttributeDefinitions.Pitch,

                    Name =
                        "pitch"
                },

                new VoiceAttributeDefinitionRecord
                {
                    Id =
                        KnownVoiceAttributeDefinitions.Timbre,

                    Name =
                        "timbre"
                },

                new VoiceAttributeDefinitionRecord
                {
                    Id =
                        KnownVoiceAttributeDefinitions.Pace,

                    Name =
                        "pace"
                }
            };

        foreach (var definition in definitions)
        {
            var exists =
                await dbContext
                    .VoiceAttributeDefinitions
                    .AnyAsync(
                        record =>
                            record.Id ==
                            definition.Id,
                        cancellationToken);

            if (!exists)
            {
                dbContext
                    .VoiceAttributeDefinitions
                    .Add(
                        definition);
            }
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private static async Task EnsureValuesAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var values =
            new[]
            {
                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.PitchLow,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Pitch,

                    Value =
                        "low",

                    DisplayText =
                        "low pitch"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.PitchMedium,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Pitch,

                    Value =
                        "medium",

                    DisplayText =
                        "medium pitch"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.PitchHigh,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Pitch,

                    Value =
                        "high",

                    DisplayText =
                        "high pitch"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.TimbreWarm,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Timbre,

                    Value =
                        "warm",

                    DisplayText =
                        "warm timbre"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.TimbreBright,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Timbre,

                    Value =
                        "bright",

                    DisplayText =
                        "bright timbre"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.TimbreRough,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Timbre,

                    Value =
                        "rough",

                    DisplayText =
                        "rough timbre"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.TimbreSoft,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Timbre,

                    Value =
                        "soft",

                    DisplayText =
                        "soft timbre"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.PaceSlow,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Pace,

                    Value =
                        "slow",

                    DisplayText =
                        "slow pace"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.PaceModerate,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Pace,

                    Value =
                        "moderate",

                    DisplayText =
                        "moderate pace"
                },

                new VoiceAttributeValueRecord
                {
                    Id =
                        KnownVoiceAttributeValues.PaceQuick,

                    AttributeDefinitionId =
                        KnownVoiceAttributeDefinitions.Pace,

                    Value =
                        "quick",

                    DisplayText =
                        "quick pace"
                }
            };

        foreach (var value in values)
        {
            var exists =
                await dbContext
                    .VoiceAttributeValues
                    .AnyAsync(
                        record =>
                            record.Id ==
                            value.Id,
                        cancellationToken);

            if (!exists)
            {
                dbContext
                    .VoiceAttributeValues
                    .Add(
                        value);
            }
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private static async Task EnsureEntityAssignmentsAsync(
        AtlantisDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var assignments =
            new[]
            {
                new EntityVoiceAttributeRecord
                {
                    EntityId =
                        "visitor-default",

                    AttributeValueId =
                        KnownVoiceAttributeValues.PitchMedium,

                    Sequence =
                        1
                },

                new EntityVoiceAttributeRecord
                {
                    EntityId =
                        "visitor-default",

                    AttributeValueId =
                        KnownVoiceAttributeValues.TimbreWarm,

                    Sequence =
                        2
                },

                new EntityVoiceAttributeRecord
                {
                    EntityId =
                        "visitor-default",

                    AttributeValueId =
                        KnownVoiceAttributeValues.PaceModerate,

                    Sequence =
                        3
                },

                new EntityVoiceAttributeRecord
                {
                    EntityId =
                        "orestes",

                    AttributeValueId =
                        KnownVoiceAttributeValues.PitchLow,

                    Sequence =
                        1
                },

                new EntityVoiceAttributeRecord
                {
                    EntityId =
                        "orestes",

                    AttributeValueId =
                        KnownVoiceAttributeValues.TimbreWarm,

                    Sequence =
                        2
                },

                new EntityVoiceAttributeRecord
                {
                    EntityId =
                        "orestes",

                    AttributeValueId =
                        KnownVoiceAttributeValues.PaceModerate,

                    Sequence =
                        3
                }
            };

        foreach (var assignment in assignments)
        {
            var exists =
                await dbContext
                    .EntityVoiceAttributes
                    .AnyAsync(
                        record =>
                            record.EntityId ==
                                assignment.EntityId &&
                            record.AttributeValueId ==
                                assignment.AttributeValueId,
                        cancellationToken);

            if (!exists)
            {
                dbContext
                    .EntityVoiceAttributes
                    .Add(
                        assignment);
            }
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}