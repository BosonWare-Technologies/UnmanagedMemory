// ReSharper disable CheckNamespace

namespace JetBrains.Annotations;

/// <summary>
/// </summary>
[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)] [PublicAPI]
[AttributeUsage(AttributeTargets.All)]
internal sealed class PublicAPIAttribute : Attribute;
