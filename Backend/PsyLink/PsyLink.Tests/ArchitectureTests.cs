using MediatR;
using System.Reflection;
using Xunit;

namespace PsyLink.Tests;

public sealed class ArchitectureTests
{
    [Fact]
    public void Handlers_AreSealed_AndImplementIRequestHandler()
    {
        var assembly = typeof(PsyLink.Application.DependencyInjection).Assembly;
        var handlers = assembly.GetTypes()
            .Where(type => type.Name.EndsWith("Handler", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(handlers);

        foreach (var handler in handlers)
        {
            Assert.True(handler.IsSealed, $"{handler.FullName} must be sealed.");
            Assert.True(
                handler.GetInterfaces().Any(type =>
                    type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)),
                $"{handler.FullName} must implement IRequestHandler.");
        }
    }

    [Fact]
    public void Application_DoesNotReferenceInfrastructureOrEntityFrameworkOrAspNet()
    {
        var assembly = typeof(PsyLink.Application.DependencyInjection).Assembly;
        var forbidden = assembly.GetReferencedAssemblies()
            .Select(assemblyName => assemblyName.Name ?? string.Empty)
            .ToArray();

        Assert.DoesNotContain(forbidden, name =>
            name.Contains("PsyLink.Infrastructure", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(forbidden, name =>
            name.Contains("EntityFramework", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(forbidden, name =>
            name.StartsWith("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void FeatureHandlers_DoNotReferenceOtherFeatures()
    {
        var assembly = typeof(PsyLink.Application.DependencyInjection).Assembly;

        foreach (var handler in assembly.GetTypes()
                     .Where(type => type.Name.EndsWith("Handler", StringComparison.Ordinal)))
        {
            var feature = GetFeature(handler.Namespace);
            if (feature is null)
            {
                continue;
            }

            var referencedTypes = handler.GetConstructors()
                .SelectMany(constructor => constructor.GetParameters())
                .Select(parameter => parameter.ParameterType);

            foreach (var referencedType in referencedTypes)
            {
                var referencedFeature = GetFeature(referencedType.Namespace);
                if (referencedFeature is not null)
                {
                    Assert.True(
                        referencedFeature == feature,
                        $"{handler.FullName} references feature {referencedFeature}.");
                }
            }
        }
    }

    private static string? GetFeature(string? namespaceName)
    {
        if (namespaceName is null)
        {
            return null;
        }

        const string marker = "PsyLink.Application.Features.";
        if (!namespaceName.StartsWith(marker, StringComparison.Ordinal))
        {
            return null;
        }

        var remainder = namespaceName[marker.Length..];
        var separator = remainder.IndexOf('.');
        return separator < 0 ? remainder : remainder[..separator];
    }
}
