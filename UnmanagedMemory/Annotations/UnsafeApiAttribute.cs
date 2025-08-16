using JetBrains.Annotations;
using static System.AttributeTargets;

namespace UnmanagedMemory.Annotations;

/// <summary>
///     Specifies that the annotated member is unsafe.
/// </summary>
[AttributeUsage(Class | Struct | Constructor | Method | Property)]
public sealed class UnsafeApiAttribute : Attribute
{
    [UsedImplicitly]
    public string Comment { get; set; } = "";
}
