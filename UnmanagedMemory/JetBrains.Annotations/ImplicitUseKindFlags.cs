namespace JetBrains.Annotations;

/// <summary>
///     Specifies the details of an implicitly used symbol when it is marked
///     with <see cref="T:JetBrains.Annotations.MeansImplicitUseAttribute" /> or
///     <see cref="T:JetBrains.Annotations.UsedImplicitlyAttribute" />.
/// </summary>
[Flags]
internal enum ImplicitUseKindFlags
{
    Default = 7,

    /// <summary>Only entity marked with attribute considered used.</summary>
    Access = 1,

    /// <summary>Indicates implicit assignment to a member.</summary>
    Assign = 2,

    /// <summary>
    ///     Indicates implicit instantiation of a type with fixed constructor signature.
    ///     That means any unused constructor parameters won't be reported as such.
    /// </summary>
    InstantiatedWithFixedConstructorSignature = 4,

    /// <summary>Indicates implicit instantiation of a type.</summary>
    InstantiatedNoFixedConstructorSignature = 8
}
