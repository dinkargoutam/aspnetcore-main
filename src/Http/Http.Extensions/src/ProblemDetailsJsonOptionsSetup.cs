// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Http;

internal sealed class ProblemDetailsJsonOptionsSetup : IPostConfigureOptions<JsonOptions>
{
    public void PostConfigure(string? name, JsonOptions options)
    {
        switch (options.SerializerOptions.TypeInfoResolver)
        {
            case DefaultJsonTypeInfoResolver:
                // In this case, the current configuration is using a reflection-based resolver
                // and we are prepending our internal problem details context to be evaluated
                // first.
                options.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(ProblemDetailsJsonContext.Default, options.SerializerOptions.TypeInfoResolver);
                break;
            case not null:
                // Combine the current resolver with our internal problem details context (adding last)
                options.SerializerOptions.AddContext<ProblemDetailsJsonContext>();
                break;
            default:
                // Not adding our source gen context when TypeInfoResolver == null
                // since adding it will skip the reflection-based resolver and potentially
                // cause unexpected serialization problems
                break;
        }
    }
}
