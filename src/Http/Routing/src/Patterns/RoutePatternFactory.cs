// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Microsoft.AspNetCore.Routing.Patterns;

/// <summary>
/// Contains factory methods for creating <see cref="RoutePattern"/> and related types.
/// Use <see cref="Parse(string)"/> to parse a route pattern in
/// string format.
/// </summary>
public static class RoutePatternFactory
{
    private static readonly IReadOnlyDictionary<string, object?> EmptyDictionary =
        new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>());

    private static readonly IReadOnlyDictionary<string, IReadOnlyList<RoutePatternParameterPolicyReference>> EmptyPoliciesDictionary =
        new ReadOnlyDictionary<string, IReadOnlyList<RoutePatternParameterPolicyReference>>(new Dictionary<string, IReadOnlyList<RoutePatternParameterPolicyReference>>());

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from its string representation.
    /// </summary>
    /// <param name="pattern">The route pattern string to parse.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Parse(string pattern)
    {
        if (pattern == null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        return RoutePatternParser.Parse(pattern);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from its string representation along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="pattern">The route pattern string to parse.</param>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the parsed route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the parsed route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    [RequiresUnreferencedCode(RouteValueDictionaryTrimmerWarning.Warning)]
    public static RoutePattern Parse(string pattern, object? defaults, object? parameterPolicies)
    {
        if (pattern == null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        var original = RoutePatternParser.Parse(pattern);
        return PatternCore(original.RawText, Wrap(defaults), Wrap(parameterPolicies), requiredValues: null, original.PathSegments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from its string representation along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="pattern">The route pattern string to parse.</param>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the parsed route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the parsed route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Parse(string pattern, RouteValueDictionary? defaults, RouteValueDictionary? parameterPolicies)
    {
        if (pattern == null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        var original = RoutePatternParser.Parse(pattern);
        return PatternCore(original.RawText, defaults, parameterPolicies, requiredValues: null, original.PathSegments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from its string representation along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="pattern">The route pattern string to parse.</param>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the parsed route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the parsed route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="requiredValues">
    /// Route values that can be substituted for parameters in the route pattern. See remarks on <see cref="RoutePattern.RequiredValues"/>.
    /// </param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    [RequiresUnreferencedCode(RouteValueDictionaryTrimmerWarning.Warning)]
    public static RoutePattern Parse(string pattern, object? defaults, object? parameterPolicies, object? requiredValues)
    {
        if (pattern == null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        var original = RoutePatternParser.Parse(pattern);
        return PatternCore(original.RawText, Wrap(defaults), Wrap(parameterPolicies), Wrap(requiredValues), original.PathSegments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from its string representation along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="pattern">The route pattern string to parse.</param>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the parsed route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the parsed route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="requiredValues">
    /// Route values that can be substituted for parameters in the route pattern. See remarks on <see cref="RoutePattern.RequiredValues"/>.
    /// </param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Parse(string pattern, RouteValueDictionary? defaults, RouteValueDictionary? parameterPolicies, RouteValueDictionary? requiredValues)
    {
        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        var original = RoutePatternParser.Parse(pattern);
        return PatternCore(original.RawText, defaults, parameterPolicies, requiredValues, original.PathSegments);
    }

    /// <summary>
    /// Creates a new instance of <see cref="RoutePattern"/> from a collection of segments.
    /// </summary>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Pattern(IEnumerable<RoutePatternPathSegment> segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(null, null, null, null, segments);
    }

    /// <summary>
    /// Creates a new instance of <see cref="RoutePattern"/> from a collection of segments.
    /// </summary>
    /// <param name="rawText">The raw text to associate with the route pattern. May be null.</param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Pattern(string? rawText, IEnumerable<RoutePatternPathSegment> segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(rawText, null, null, null, segments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from a collection of segments along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    [RequiresUnreferencedCode(RouteValueDictionaryTrimmerWarning.Warning)]
    public static RoutePattern Pattern(
        object? defaults,
        object? parameterPolicies,
        IEnumerable<RoutePatternPathSegment> segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(null, new RouteValueDictionary(defaults), new RouteValueDictionary(parameterPolicies), requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from a collection of segments along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Pattern(
        RouteValueDictionary? defaults,
        RouteValueDictionary? parameterPolicies,
        IEnumerable<RoutePatternPathSegment> segments)
    {
        if (segments is null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(null, defaults, parameterPolicies, requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from a collection of segments along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="rawText">The raw text to associate with the route pattern. May be null.</param>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    [RequiresUnreferencedCode(RouteValueDictionaryTrimmerWarning.Warning)]
    public static RoutePattern Pattern(
        string? rawText,
        object? defaults,
        object? parameterPolicies,
        IEnumerable<RoutePatternPathSegment> segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(rawText, new RouteValueDictionary(defaults), new RouteValueDictionary(parameterPolicies), requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from a collection of segments along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="rawText">The raw text to associate with the route pattern. May be null.</param>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Pattern(
        string? rawText,
        RouteValueDictionary? defaults,
        RouteValueDictionary? parameterPolicies,
        IEnumerable<RoutePatternPathSegment> segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(rawText, defaults, parameterPolicies, requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a new instance of <see cref="RoutePattern"/> from a collection of segments.
    /// </summary>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Pattern(params RoutePatternPathSegment[] segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(null, null, null, requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a new instance of <see cref="RoutePattern"/> from a collection of segments.
    /// </summary>
    /// <param name="rawText">The raw text to associate with the route pattern. May be null.</param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Pattern(string rawText, params RoutePatternPathSegment[] segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(rawText, null, null, requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from a collection of segments along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    [RequiresUnreferencedCode(RouteValueDictionaryTrimmerWarning.Warning)]
    public static RoutePattern Pattern(
        object? defaults,
        object? parameterPolicies,
        params RoutePatternPathSegment[] segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(null, new RouteValueDictionary(defaults), new RouteValueDictionary(parameterPolicies), requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from a collection of segments along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Pattern(
        RouteValueDictionary? defaults,
        RouteValueDictionary? parameterPolicies,
        params RoutePatternPathSegment[] segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(null, defaults, parameterPolicies, requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from a collection of segments along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="rawText">The raw text to associate with the route pattern.</param>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    [RequiresUnreferencedCode(RouteValueDictionaryTrimmerWarning.Warning)]
    public static RoutePattern Pattern(
        string? rawText,
        object? defaults,
        object? parameterPolicies,
        params RoutePatternPathSegment[] segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(rawText, new RouteValueDictionary(defaults), new RouteValueDictionary(parameterPolicies), requiredValues: null, segments);
    }

    /// <summary>
    /// Creates a <see cref="RoutePattern"/> from a collection of segments along
    /// with provided default values and parameter policies.
    /// </summary>
    /// <param name="rawText">The raw text to associate with the route pattern.</param>
    /// <param name="defaults">
    /// Additional default values to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// </param>
    /// <param name="parameterPolicies">
    /// Additional parameter policies to associated with the route pattern. May be null.
    /// The provided object will be converted to key-value pairs using <see cref="RouteValueDictionary"/>
    /// and then merged into the route pattern.
    /// Multiple policies can be specified for a key by providing a collection as the value.
    /// </param>
    /// <param name="segments">The collection of segments.</param>
    /// <returns>The <see cref="RoutePattern"/>.</returns>
    public static RoutePattern Pattern(
        string? rawText,
        RouteValueDictionary? defaults,
        RouteValueDictionary? parameterPolicies,
        params RoutePatternPathSegment[] segments)
    {
        if (segments == null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        return PatternCore(rawText, defaults, parameterPolicies, requiredValues: null, segments);
    }

    private static RoutePattern PatternCore(
        string? rawText,
        RouteValueDictionary? defaults,
        RouteValueDictionary? parameterPolicies,
        RouteValueDictionary? requiredValues,
        IEnumerable<RoutePatternPathSegment> segments)
    {
        // We want to merge the segment data with the 'out of line' defaults and parameter policies.
        //
        // This means that for parameters that have 'out of line' defaults we will modify
        // the parameter to contain the default (same story for parameter policies).
        //
        // We also maintain a collection of defaults and parameter policies that will also
        // contain the values that don't match a parameter.
        //
        // It's important that these two views of the data are consistent. We don't want
        // values specified out of line to have a different behavior.

        Dictionary<string, object?>? updatedDefaults = null;
        if (defaults != null && defaults.Count > 0)
        {
            updatedDefaults = new Dictionary<string, object?>(defaults.Count, StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in defaults)
            {
                updatedDefaults.Add(kvp.Key, kvp.Value);
            }
        }

        Dictionary<string, List<RoutePatternParameterPolicyReference>>? updatedParameterPolicies = null;
        if (parameterPolicies != null && parameterPolicies.Count > 0)
        {
            updatedParameterPolicies = new Dictionary<string, List<RoutePatternParameterPolicyReference>>(parameterPolicies.Count, StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in parameterPolicies)
            {
                var policyReferences = new List<RoutePatternParameterPolicyReference>();

                if (kvp.Value is IParameterPolicy parameterPolicy)
                {
                    policyReferences.Add(ParameterPolicy(parameterPolicy));
                }
                else if (kvp.Value is string)
                {
                    // Constraint will convert string values into regex constraints
                    policyReferences.Add(Constraint(kvp.Value));
                }
                else if (kvp.Value is IEnumerable multiplePolicies)
                {
                    foreach (var item in multiplePolicies)
                    {
                        // Constraint will convert string values into regex constraints
                        policyReferences.Add(item is IParameterPolicy p ? ParameterPolicy(p) : Constraint(item));
                    }
                }
                else
                {
                    throw new InvalidOperationException(Resources.FormatRoutePattern_InvalidConstraintReference(
                        kvp.Value ?? "null",
                        typeof(IRouteConstraint)));
                }

                updatedParameterPolicies.Add(kvp.Key, policyReferences);
            }
        }

        List<RoutePatternParameterPart>? parameters = null;
        var updatedSegments = segments.ToArray();
        for (var i = 0; i < updatedSegments.Length; i++)
        {
            var segment = VisitSegment(updatedSegments[i]);
            updatedSegments[i] = segment;

            for (var j = 0; j < segment.Parts.Count; j++)
            {
                if (segment.Parts[j] is RoutePatternParameterPart parameter)
                {
                    if (parameters == null)
                    {
                        parameters = new List<RoutePatternParameterPart>();
                    }

                    parameters.Add(parameter);
                }
            }
        }

        // Each Required Value either needs to either:
        // 1. be null-ish
        // 2. have a corresponding parameter
        // 3. have a corresponding default that matches both key and value
        if (requiredValues != null)
        {
            foreach (var kvp in requiredValues)
            {
                // 1.be null-ish
                var found = RouteValueEqualityComparer.Default.Equals(string.Empty, kvp.Value);

                // 2. have a corresponding parameter
                if (!found && parameters != null)
                {
                    for (var i = 0; i < parameters.Count; i++)
                    {
                        if (string.Equals(kvp.Key, parameters[i].Name, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                // 3. have a corresponding default that matches both key and value
                if (!found &&
                    updatedDefaults != null &&
                    updatedDefaults.TryGetValue(kvp.Key, out var defaultValue) &&
                    RouteValueEqualityComparer.Default.Equals(kvp.Value, defaultValue))
                {
                    found = true;
                }

                if (!found)
                {
                    throw new InvalidOperationException(
                        $"No corresponding parameter or default value could be found for the required value " +
                        $"'{kvp.Key}={kvp.Value}'. A non-null required value must correspond to a route parameter or the " +
                        $"route pattern must have a matching default value.");
                }
            }
        }

        return new RoutePattern(
            rawText,
            updatedDefaults ?? EmptyDictionary,
            updatedParameterPolicies != null
                ? updatedParameterPolicies.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<RoutePatternParameterPolicyReference>)kvp.Value.ToArray())
                : EmptyPoliciesDictionary,
            requiredValues ?? EmptyDictionary,
            (IReadOnlyList<RoutePatternParameterPart>?)parameters ?? Array.Empty<RoutePatternParameterPart>(),
            updatedSegments);

        RoutePatternPathSegment VisitSegment(RoutePatternPathSegment segment)
        {
            RoutePatternPart[]? updatedParts = null;
            for (var i = 0; i < segment.Parts.Count; i++)
            {
                var part = segment.Parts[i];
                var updatedPart = VisitPart(part);

                if (part != updatedPart)
                {
                    if (updatedParts == null)
                    {
                        updatedParts = segment.Parts.ToArray();
                    }

                    updatedParts[i] = updatedPart;
                }
            }

            if (updatedParts == null)
            {
                // Segment has not changed
                return segment;
            }

            return new RoutePatternPathSegment(updatedParts);
        }

        RoutePatternPart VisitPart(RoutePatternPart part)
        {
            if (!part.IsParameter)
            {
                return part;
            }

            var parameter = (RoutePatternParameterPart)part;
            var @default = parameter.Default;

            if (updatedDefaults != null && updatedDefaults.TryGetValue(parameter.Name, out var newDefault))
            {
                if (parameter.Default != null && !Equals(newDefault, parameter.Default))
                {
                    var message = Resources.FormatTemplateRoute_CannotHaveDefaultValueSpecifiedInlineAndExplicitly(parameter.Name);
                    throw new InvalidOperationException(message);
                }

                if (parameter.IsOptional)
                {
                    var message = Resources.TemplateRoute_OptionalCannotHaveDefaultValue;
                    throw new InvalidOperationException(message);
                }

                @default = newDefault;
            }

            if (parameter.Default != null)
            {
                if (updatedDefaults == null)
                {
                    updatedDefaults = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                }

                updatedDefaults[parameter.Name] = parameter.Default;
            }

            List<RoutePatternParameterPolicyReference>? parameterConstraints = null;
            if ((updatedParameterPolicies == null || !updatedParameterPolicies.TryGetValue(parameter.Name, out parameterConstraints)) &&
                parameter.ParameterPolicies.Count > 0)
            {
                if (updatedParameterPolicies == null)
                {
                    updatedParameterPolicies = new Dictionary<string, List<RoutePatternParameterPolicyReference>>(StringComparer.OrdinalIgnoreCase);
                }

                parameterConstraints = new List<RoutePatternParameterPolicyReference>(parameter.ParameterPolicies.Count);
                updatedParameterPolicies.Add(parameter.Name, parameterConstraints);
            }

            if (parameter.ParameterPolicies.Count > 0)
            {
                parameterConstraints!.AddRange(parameter.ParameterPolicies);
            }

            if (Equals(parameter.Default, @default)
                && parameter.ParameterPolicies.Count == 0
                && (parameterConstraints?.Count ?? 0) == 0)
            {
                // Part has not changed
                return part;
            }

            return ParameterPartCore(
                parameter.Name,
                @default,
                parameter.ParameterKind,
                parameterConstraints?.ToArray() ?? Array.Empty<RoutePatternParameterPolicyReference>(),
                parameter.EncodeSlashes);
        }
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternPathSegment"/> from the provided collection
    /// of parts.
    /// </summary>
    /// <param name="parts">The collection of parts.</param>
    /// <returns>The <see cref="RoutePatternPathSegment"/>.</returns>
    public static RoutePatternPathSegment Segment(IEnumerable<RoutePatternPart> parts)
    {
        if (parts == null)
        {
            throw new ArgumentNullException(nameof(parts));
        }

        return SegmentCore(parts.ToArray());
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternPathSegment"/> from the provided collection
    /// of parts.
    /// </summary>
    /// <param name="parts">The collection of parts.</param>
    /// <returns>The <see cref="RoutePatternPathSegment"/>.</returns>
    public static RoutePatternPathSegment Segment(params RoutePatternPart[] parts)
    {
        if (parts == null)
        {
            throw new ArgumentNullException(nameof(parts));
        }

        return SegmentCore((RoutePatternPart[])parts.Clone());
    }

    private static RoutePatternPathSegment SegmentCore(RoutePatternPart[] parts)
    {
        return new RoutePatternPathSegment(parts);
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternLiteralPart"/> from the provided text
    /// content.
    /// </summary>
    /// <param name="content">The text content.</param>
    /// <returns>The <see cref="RoutePatternLiteralPart"/>.</returns>
    public static RoutePatternLiteralPart LiteralPart(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(content));
        }

        if (content.Contains('?'))
        {
            throw new ArgumentException(Resources.FormatTemplateRoute_InvalidLiteral(content));
        }

        return LiteralPartCore(content);
    }

    private static RoutePatternLiteralPart LiteralPartCore(string content)
    {
        return new RoutePatternLiteralPart(content);
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternSeparatorPart"/> from the provided text
    /// content.
    /// </summary>
    /// <param name="content">The text content.</param>
    /// <returns>The <see cref="RoutePatternSeparatorPart"/>.</returns>
    public static RoutePatternSeparatorPart SeparatorPart(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(content));
        }

        return SeparatorPartCore(content);
    }

    private static RoutePatternSeparatorPart SeparatorPartCore(string content)
    {
        return new RoutePatternSeparatorPart(content);
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPart"/> from the provided parameter name.
    /// </summary>
    /// <param name="parameterName">The parameter name.</param>
    /// <returns>The <see cref="RoutePatternParameterPart"/>.</returns>
    public static RoutePatternParameterPart ParameterPart(string parameterName)
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(parameterName));
        }

        if (parameterName.IndexOfAny(RoutePatternParser.InvalidParameterNameChars) >= 0)
        {
            throw new ArgumentException(Resources.FormatTemplateRoute_InvalidParameterName(parameterName));
        }

        return ParameterPartCore(
            parameterName: parameterName,
            @default: null,
            parameterKind: RoutePatternParameterKind.Standard,
            parameterPolicies: Array.Empty<RoutePatternParameterPolicyReference>());
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPart"/> from the provided parameter name
    /// and default value.
    /// </summary>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="default">The parameter default value. May be <c>null</c>.</param>
    /// <returns>The <see cref="RoutePatternParameterPart"/>.</returns>
    public static RoutePatternParameterPart ParameterPart(string parameterName, object @default)
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(parameterName));
        }

        if (parameterName.IndexOfAny(RoutePatternParser.InvalidParameterNameChars) >= 0)
        {
            throw new ArgumentException(Resources.FormatTemplateRoute_InvalidParameterName(parameterName));
        }

        return ParameterPartCore(
            parameterName: parameterName,
            @default: @default,
            parameterKind: RoutePatternParameterKind.Standard,
            parameterPolicies: Array.Empty<RoutePatternParameterPolicyReference>());
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPart"/> from the provided parameter name
    /// and default value, and parameter kind.
    /// </summary>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="default">The parameter default value. May be <c>null</c>.</param>
    /// <param name="parameterKind">The parameter kind.</param>
    /// <returns>The <see cref="RoutePatternParameterPart"/>.</returns>
    public static RoutePatternParameterPart ParameterPart(
        string parameterName,
        object? @default,
        RoutePatternParameterKind parameterKind)
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(parameterName));
        }

        if (parameterName.IndexOfAny(RoutePatternParser.InvalidParameterNameChars) >= 0)
        {
            throw new ArgumentException(Resources.FormatTemplateRoute_InvalidParameterName(parameterName));
        }

        if (@default != null && parameterKind == RoutePatternParameterKind.Optional)
        {
            throw new ArgumentNullException(nameof(parameterKind), Resources.TemplateRoute_OptionalCannotHaveDefaultValue);
        }

        return ParameterPartCore(
            parameterName: parameterName,
            @default: @default,
            parameterKind: parameterKind,
            parameterPolicies: Array.Empty<RoutePatternParameterPolicyReference>());
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPart"/> from the provided parameter name
    /// and default value, parameter kind, and parameter policies.
    /// </summary>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="default">The parameter default value. May be <c>null</c>.</param>
    /// <param name="parameterKind">The parameter kind.</param>
    /// <param name="parameterPolicies">The parameter policies to associated with the parameter.</param>
    /// <returns>The <see cref="RoutePatternParameterPart"/>.</returns>
    public static RoutePatternParameterPart ParameterPart(
        string parameterName,
        object? @default,
        RoutePatternParameterKind parameterKind,
        IEnumerable<RoutePatternParameterPolicyReference> parameterPolicies)
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(parameterName));
        }

        if (parameterName.IndexOfAny(RoutePatternParser.InvalidParameterNameChars) >= 0)
        {
            throw new ArgumentException(Resources.FormatTemplateRoute_InvalidParameterName(parameterName));
        }

        if (@default != null && parameterKind == RoutePatternParameterKind.Optional)
        {
            throw new ArgumentNullException(nameof(parameterKind), Resources.TemplateRoute_OptionalCannotHaveDefaultValue);
        }

        if (parameterPolicies == null)
        {
            throw new ArgumentNullException(nameof(parameterPolicies));
        }

        return ParameterPartCore(
            parameterName: parameterName,
            @default: @default,
            parameterKind: parameterKind,
            parameterPolicies: parameterPolicies.ToArray());
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPart"/> from the provided parameter name
    /// and default value, parameter kind, and parameter policies.
    /// </summary>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="default">The parameter default value. May be <c>null</c>.</param>
    /// <param name="parameterKind">The parameter kind.</param>
    /// <param name="parameterPolicies">The parameter policies to associated with the parameter.</param>
    /// <returns>The <see cref="RoutePatternParameterPart"/>.</returns>
    public static RoutePatternParameterPart ParameterPart(
        string parameterName,
        object? @default,
        RoutePatternParameterKind parameterKind,
        params RoutePatternParameterPolicyReference[] parameterPolicies)
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(parameterName));
        }

        if (parameterName.IndexOfAny(RoutePatternParser.InvalidParameterNameChars) >= 0)
        {
            throw new ArgumentException(Resources.FormatTemplateRoute_InvalidParameterName(parameterName));
        }

        if (@default != null && parameterKind == RoutePatternParameterKind.Optional)
        {
            throw new ArgumentNullException(nameof(parameterKind), Resources.TemplateRoute_OptionalCannotHaveDefaultValue);
        }

        if (parameterPolicies == null)
        {
            throw new ArgumentNullException(nameof(parameterPolicies));
        }

        return ParameterPartCore(
            parameterName: parameterName,
            @default: @default,
            parameterKind: parameterKind,
            parameterPolicies: (RoutePatternParameterPolicyReference[])parameterPolicies.Clone());
    }

    private static RoutePatternParameterPart ParameterPartCore(
        string parameterName,
        object? @default,
        RoutePatternParameterKind parameterKind,
        RoutePatternParameterPolicyReference[] parameterPolicies)
    {
        return ParameterPartCore(parameterName, @default, parameterKind, parameterPolicies, encodeSlashes: true);
    }

    private static RoutePatternParameterPart ParameterPartCore(
        string parameterName,
        object? @default,
        RoutePatternParameterKind parameterKind,
        RoutePatternParameterPolicyReference[] parameterPolicies,
        bool encodeSlashes)
    {
        return new RoutePatternParameterPart(
            parameterName,
            @default,
            parameterKind,
            parameterPolicies,
            encodeSlashes);
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPolicyReference"/> from the provided contraint.
    /// </summary>
    /// <param name="constraint">
    /// The constraint object, which must be of type <see cref="IRouteConstraint"/>
    /// or <see cref="string"/>. If the constraint object is a <see cref="string"/>
    /// then it will be transformed into an instance of <see cref="RegexRouteConstraint"/>.
    /// </param>
    /// <returns>The <see cref="RoutePatternParameterPolicyReference"/>.</returns>
    public static RoutePatternParameterPolicyReference Constraint(object constraint)
    {
        // Similar to RouteConstraintBuilder
        if (constraint is IRouteConstraint policy)
        {
            return ParameterPolicyCore(policy);
        }
        else if (constraint is string content)
        {
            return ParameterPolicyCore(new RegexRouteConstraint("^(" + content + ")$"));
        }
        else
        {
            throw new InvalidOperationException(Resources.FormatRoutePattern_InvalidConstraintReference(
                constraint ?? "null",
                typeof(IRouteConstraint)));
        }
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPolicyReference"/> from the provided constraint.
    /// </summary>
    /// <param name="constraint">
    /// The constraint object.
    /// </param>
    /// <returns>The <see cref="RoutePatternParameterPolicyReference"/>.</returns>
    public static RoutePatternParameterPolicyReference Constraint(IRouteConstraint constraint)
    {
        if (constraint == null)
        {
            throw new ArgumentNullException(nameof(constraint));
        }

        return ParameterPolicyCore(constraint);
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPolicyReference"/> from the provided constraint.
    /// </summary>
    /// <param name="constraint">
    /// The constraint text, which will be resolved by <see cref="ParameterPolicyFactory"/>.
    /// </param>
    /// <returns>The <see cref="RoutePatternParameterPolicyReference"/>.</returns>
    public static RoutePatternParameterPolicyReference Constraint(string constraint)
    {
        if (string.IsNullOrEmpty(constraint))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(constraint));
        }

        return ParameterPolicyCore(constraint);
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPolicyReference"/> from the provided object.
    /// </summary>
    /// <param name="parameterPolicy">
    /// The parameter policy object.
    /// </param>
    /// <returns>The <see cref="RoutePatternParameterPolicyReference"/>.</returns>
    public static RoutePatternParameterPolicyReference ParameterPolicy(IParameterPolicy parameterPolicy)
    {
        if (parameterPolicy == null)
        {
            throw new ArgumentNullException(nameof(parameterPolicy));
        }

        return ParameterPolicyCore(parameterPolicy);
    }

    /// <summary>
    /// Creates a <see cref="RoutePatternParameterPolicyReference"/> from the provided object.
    /// </summary>
    /// <param name="parameterPolicy">
    /// The parameter policy text, which will be resolved by <see cref="ParameterPolicyFactory"/>.
    /// </param>
    /// <returns>The <see cref="RoutePatternParameterPolicyReference"/>.</returns>
    public static RoutePatternParameterPolicyReference ParameterPolicy(string parameterPolicy)
    {
        if (string.IsNullOrEmpty(parameterPolicy))
        {
            throw new ArgumentException(Resources.Argument_NullOrEmpty, nameof(parameterPolicy));
        }

        return ParameterPolicyCore(parameterPolicy);
    }

    internal static RoutePattern Combine(RoutePattern left, RoutePattern right)
    {
        static IReadOnlyList<T> CombineLists<T>(
            IReadOnlyList<T> leftList,
            IReadOnlyList<T> rightList,
            Func<int, string, Action<T>>? checkDuplicates = null,
            string? rawText = null)
        {
            if (leftList.Count is 0)
            {
                return rightList;
            }
            if (rightList.Count is 0)
            {
                return leftList;
            }

            var combinedCount = leftList.Count + rightList.Count;
            var combinedList = new List<T>(combinedCount);
            // If checkDuplicates is set, so is rawText so the right exception can be thrown from check.
            var check = checkDuplicates?.Invoke(combinedCount, rawText!);
            foreach (var item in leftList)
            {
                check?.Invoke(item);
                combinedList.Add(item);
            }
            foreach (var item in rightList)
            {
                check?.Invoke(item);
                combinedList.Add(item);
            }
            return combinedList;
        }

        // Technically, the ParameterPolicies could probably be merged because it's a list, but it makes little sense to add policy
        // for the same parameter in both the left and right part of the combined pattern. Defaults and Required values cannot be
        // merged because the `TValue` is `object?`, but over-setting a Default or RequiredValue (which may not be in the parameter list)
        // seems okay as long as the values are the same for a given key in both the left and right pattern. There's already similar logic
        // in PatternCore for when defaults come from both the `defaults` and `segments` param. `requiredValues` cannot be defined in
        // `segments` so there's no equivalent to merging these until now.
        static IReadOnlyDictionary<string, TValue> CombineDictionaries<TValue>(
            IReadOnlyDictionary<string, TValue> leftDictionary,
            IReadOnlyDictionary<string, TValue> rightDictionary,
            string rawText,
            string dictionaryName)
        {
            if (leftDictionary.Count is 0)
            {
                return rightDictionary;
            }
            if (rightDictionary.Count is 0)
            {
                return leftDictionary;
            }

            var combinedDictionary = new Dictionary<string, TValue>(leftDictionary.Count + rightDictionary.Count, StringComparer.OrdinalIgnoreCase);
            foreach (var (key, value) in leftDictionary)
            {
                combinedDictionary.Add(key, value);
            }
            foreach (var (key, value) in rightDictionary)
            {
                if (combinedDictionary.TryGetValue(key, out var leftValue))
                {
                    if (!Equals(leftValue, value))
                    {
                        throw new InvalidOperationException(Resources.FormatMapGroup_RepeatedDictionaryEntry(rawText, dictionaryName, key));
                    }
                }
                else
                {
                    combinedDictionary.Add(key, value);
                }
            }
            return combinedDictionary;
        }

        static Action<RoutePatternParameterPart> CheckDuplicateParameters(int parameterCount, string rawText)
        {
            var parameterNameSet = new HashSet<string>(parameterCount, StringComparer.OrdinalIgnoreCase);
            return parameterPart =>
            {
                if (!parameterNameSet.Add(parameterPart.Name))
                {
                    var errorText = Resources.FormatTemplateRoute_RepeatedParameter(parameterPart.Name);
                    throw new RoutePatternException(rawText, errorText);
                }
            };
        }

        var rawText = $"{left.RawText?.TrimEnd('/')}/{right.RawText?.TrimStart('/')}";

        var parameters = CombineLists(left.Parameters, right.Parameters, CheckDuplicateParameters, rawText);
        var pathSegments = CombineLists(left.PathSegments, right.PathSegments);

        var defaults = CombineDictionaries(left.Defaults, right.Defaults, rawText, nameof(RoutePattern.Defaults));
        var requiredValues = CombineDictionaries(left.RequiredValues, right.RequiredValues, rawText, nameof(RoutePattern.RequiredValues));
        var parameterPolicies = CombineDictionaries(left.ParameterPolicies, right.ParameterPolicies, rawText, nameof(RoutePattern.ParameterPolicies));

        return new RoutePattern(rawText, defaults, parameterPolicies, requiredValues, parameters, pathSegments);
    }

    private static RoutePatternParameterPolicyReference ParameterPolicyCore(string parameterPolicy)
    {
        return new RoutePatternParameterPolicyReference(parameterPolicy);
    }

    private static RoutePatternParameterPolicyReference ParameterPolicyCore(IParameterPolicy parameterPolicy)
    {
        return new RoutePatternParameterPolicyReference(parameterPolicy);
    }

    [RequiresUnreferencedCode(RouteValueDictionaryTrimmerWarning.Warning)]
    private static RouteValueDictionary? Wrap(object? values)
    {
        return values is null ? null : new RouteValueDictionary(values);
    }
}
