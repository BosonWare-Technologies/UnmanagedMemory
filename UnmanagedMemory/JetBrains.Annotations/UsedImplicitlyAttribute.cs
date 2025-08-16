// ReSharper disable CheckNamespace

namespace JetBrains.Annotations;

/// <summary>
///     Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
///     so this symbol will be ignored by usage-checking inspections. <br />
///     You can use <see cref="T:JetBrains.Annotations.ImplicitUseKindFlags" /> and
///     <see cref="T:JetBrains.Annotations.ImplicitUseTargetFlags" />
///     to configure how this attribute is applied.
/// </summary>
/// <example>
///     <code>
/// [UsedImplicitly]
/// public class TypeConverter {}
/// 
/// public class SummaryData
/// {
///   [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
///   public SummaryData() {}
/// }
/// 
/// [UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.Default)]
/// public interface IService {}
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.All)]
internal sealed class UsedImplicitlyAttribute : Attribute
{
    public UsedImplicitlyAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
        : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    public UsedImplicitlyAttribute(
        ImplicitUseKindFlags useKindFlags,
        ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    public ImplicitUseKindFlags UseKindFlags { get; }

    public ImplicitUseTargetFlags TargetFlags { get; }
}
