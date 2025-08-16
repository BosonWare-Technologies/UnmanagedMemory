// ReSharper disable CheckNamespace

namespace JetBrains.Annotations;

/// <summary>
///     Can be applied to attributes, type parameters, and parameters of a type assignable from
///     <see cref="T:System.Type" /> .
///     When applied to an attribute, the decorated attribute behaves the same as
///     <see cref="T:JetBrains.Annotations.UsedImplicitlyAttribute" />.
///     When applied to a type parameter or to a parameter of type <see cref="T:System.Type" />,
///     indicates that the corresponding type is used implicitly.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
internal sealed class MeansImplicitUseAttribute : Attribute
{
    public MeansImplicitUseAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
        : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    public MeansImplicitUseAttribute(
        ImplicitUseKindFlags useKindFlags,
        ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    [UsedImplicitly]
    public ImplicitUseKindFlags UseKindFlags { get; }

    [UsedImplicitly]
    public ImplicitUseTargetFlags TargetFlags { get; }
}
