using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SampleMvcApp.Extensions;

namespace SampleMvcApp.Authentication;

internal static class Utils
{
    public static void AddRange<T>(this ICollection<T> collection, ICollection<T> rangeToAdd)
    {
        foreach (var obj in rangeToAdd)
            collection.AddSafe<T>(obj);
    }

    public static void AddSafe<T>(this ICollection<T> collection, T item)
    {
        if (collection.Contains(item))
            return;
        collection.Add(item);
    }

    public static string CreateAgentString()
    {
        var version = typeof (AuthenticationBuilderExtensions).GetTypeInfo().Assembly.GetName().Version;
        var interpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 3);
        interpolatedStringHandler.AppendLiteral("{\"name\":\"aspnetcore-authentication\",\"version\":\"");
        interpolatedStringHandler.AppendFormatted(version?.Major);
        interpolatedStringHandler.AppendLiteral(".");
        interpolatedStringHandler.AppendFormatted(version?.Minor);
        interpolatedStringHandler.AppendLiteral(".");
        interpolatedStringHandler.AppendFormatted(version?.Revision);
        interpolatedStringHandler.AppendLiteral("\"}");
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(interpolatedStringHandler.ToStringAndClear()));
    }

    public static Func<T, Task> ProxyEvent<T>(
        Func<T, Task> newHandler,
        Func<T, Task> originalHandler)
    {
        return (Func<T, Task>) (async context =>
        {
            if (newHandler != null)
                await newHandler(context);
            if (originalHandler == null)
                return;
            await originalHandler(context);
        });
    }
}