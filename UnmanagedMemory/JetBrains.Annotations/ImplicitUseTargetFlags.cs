namespace JetBrains.Annotations;

/// <summary>
///     Specifies what is considered to be used implicitly when marked
///     with <see cref="T:JetBrains.Annotations.MeansImplicitUseAttribute" /> or
///     <see cref="T:JetBrains.Annotations.UsedImplicitlyAttribute" />.
/// </summary>
[Flags]
internal enum ImplicitUseTargetFlags
{
    Default = 1,
    Itself = Default, // 0x00000001

    /// <summary>Members of the type marked with the attribute are considered used.</summary>
    Members = 2,

    /// <summary> Inherited entities are considered used. </summary>
    WithInheritors = 4,

    /// <summary>Entity marked with the attribute and all its members considered used.</summary>
    WithMembers = Members | Itself // 0x00000003
}
