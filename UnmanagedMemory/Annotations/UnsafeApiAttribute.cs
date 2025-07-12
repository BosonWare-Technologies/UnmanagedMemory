using static System.AttributeTargets;

namespace UnmanagedMemory.Annotations;

[AttributeUsage(Class | Struct | Constructor | Method | Property)]
public sealed class UnsafeApiAttribute : Attribute;