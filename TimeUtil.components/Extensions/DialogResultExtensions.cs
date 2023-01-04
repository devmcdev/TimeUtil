using MudBlazor;
using System.Diagnostics.CodeAnalysis;

namespace TimeUtil.Components.Extensions;
internal static class DialogResultExtensions
{
    public static bool TryGetSuccessfulResult<T>(this DialogResult dialogResult, [NotNullWhen(true)] out T? data)
    {
        if (!dialogResult.Canceled && dialogResult.Data is T value)
        {
            data = value;
            return true;
        }

        data = default;
        return false;
    }
}
