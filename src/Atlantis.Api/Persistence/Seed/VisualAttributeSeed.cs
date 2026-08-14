namespace Atlantis.Api.Persistence.Seed;

using Atlantis.Api.Persistence.Records;
using Atlantis.Api.World.VisualAttributes;
using Microsoft.EntityFrameworkCore;

public static class VisualAttributeSeed
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
            new VisualAttributeDefinitionRecord
            {
                Id =
                    KnownVisualAttributeDefinitions.HairLength,

                Name =
                    "hair_length"
            },

            new VisualAttributeDefinitionRecord
            {
                Id =
                    KnownVisualAttributeDefinitions.HairColor,

                Name =
                    "hair_color"
            },

            new VisualAttributeDefinitionRecord
            {
                Id =
                    KnownVisualAttributeDefinitions.SkinTone,

                Name =
                    "skin_tone"
            },

            new VisualAttributeDefinitionRecord
            {
                Id =
                    KnownVisualAttributeDefinitions.Build,

                Name =
                    "build"
            },

            new VisualAttributeDefinitionRecord
            {
                Id =
                    KnownVisualAttributeDefinitions.ShirtColor,

                Name =
                    "shirt_color"
            }
            };

        foreach (var definition in definitions)
        {
            var exists =
                await dbContext
                    .VisualAttributeDefinitions
                    .AnyAsync(
                        value =>
                            value.Id ==
                            definition.Id,
                        cancellationToken);

            if (!exists)
            {
                dbContext
                    .VisualAttributeDefinitions
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
            new VisualAttributeValueRecord
            {
                Id =
                    KnownVisualAttributeValues.HairLengthLong,

                AttributeDefinitionId =
                    KnownVisualAttributeDefinitions.HairLength,

                Value =
                    "long",

                DisplayText =
                    "long hair"
            },

            new VisualAttributeValueRecord
            {
                Id =
                    KnownVisualAttributeValues.HairLengthShort,

                AttributeDefinitionId =
                    KnownVisualAttributeDefinitions.HairLength,

                Value =
                    "short",

                DisplayText =
                    "short hair"
            },

            new VisualAttributeValueRecord
            {
                Id =
                    KnownVisualAttributeValues.HairColorRed,

                AttributeDefinitionId =
                    KnownVisualAttributeDefinitions.HairColor,

                Value =
                    "red",

                DisplayText =
                    "red hair"
            },

            new VisualAttributeValueRecord
            {
                Id =
                    KnownVisualAttributeValues.HairColorBlonde,

                AttributeDefinitionId =
                    KnownVisualAttributeDefinitions.HairColor,

                Value =
                    "blonde",

                DisplayText =
                    "blonde hair"
            },

            new VisualAttributeValueRecord
            {
                Id =
                    KnownVisualAttributeValues.HairColorBlack,

                AttributeDefinitionId =
                    KnownVisualAttributeDefinitions.HairColor,

                Value =
                    "black",

                DisplayText =
                    "black hair"
            },

            new VisualAttributeValueRecord
            {
                Id =
                    KnownVisualAttributeValues.ShirtColorBlue,

                AttributeDefinitionId =
                    KnownVisualAttributeDefinitions.ShirtColor,

                Value =
                    "blue",

                DisplayText =
                    "blue shirt"
            },

            new VisualAttributeValueRecord
            {
                Id =
                    KnownVisualAttributeValues.ShirtColorBlack,

                AttributeDefinitionId =
                    KnownVisualAttributeDefinitions.ShirtColor,

                Value =
                    "black",

                DisplayText =
                    "black shirt"
            }
            };

        foreach (var value in values)
        {
            var exists =
                await dbContext
                    .VisualAttributeValues
                    .AnyAsync(
                        record =>
                            record.Id ==
                            value.Id,
                        cancellationToken);

            if (!exists)
            {
                dbContext
                    .VisualAttributeValues
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
        const string entityId =
            "visitor-default";

        var assignments =
            new[]
            {
            new EntityVisualAttributeRecord
            {
                EntityId =
                    entityId,

                AttributeValueId =
                    KnownVisualAttributeValues.HairLengthLong,

                Sequence =
                    1
            },

            new EntityVisualAttributeRecord
            {
                EntityId =
                    entityId,

                AttributeValueId =
                    KnownVisualAttributeValues.HairColorRed,

                Sequence =
                    2
            },

            new EntityVisualAttributeRecord
            {
                EntityId =
                    entityId,

                AttributeValueId =
                    KnownVisualAttributeValues.ShirtColorBlue,

                Sequence =
                    3
            }
            };

        foreach (var assignment in assignments)
        {
            var exists =
                await dbContext
                    .EntityVisualAttributes
                    .AnyAsync(
                        value =>
                            value.EntityId ==
                                assignment.EntityId &&
                            value.AttributeValueId ==
                                assignment.AttributeValueId,
                        cancellationToken);

            if (!exists)
            {
                dbContext
                    .EntityVisualAttributes
                    .Add(
                        assignment);
            }
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }
}