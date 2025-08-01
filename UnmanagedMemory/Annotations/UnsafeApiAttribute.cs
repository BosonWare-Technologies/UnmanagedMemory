using static System.AttributeTargets;

namespace UnmanagedMemory.Annotations;

/// <summary>
/// Annotates the annotated member as unsafe.
/// </summary>
[AttributeUsage(Class | Struct | Constructor | Method | Property)]
public sealed class UnsafeApiAttribute : Attribute;
