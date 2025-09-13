using System.Reflection;
using UnityEngine;

internal static class ConstantFields
{
    private static PropertyInfo _infiniteStaminaProperty;

    private static PropertyInfo _statusLockProperty;

    private static FieldInfo _fallDamageTimeField;

    private static FieldInfo _staminaField;

    private static FieldInfo _movementModifierField;

    private static FieldInfo _jumpGravityField;

    private static FieldInfo _climbSpeedModField;

    private static FieldInfo _vineClimbSpeedModField;

    private static FieldInfo _ropeClimbSpeedModField;

    private static MethodInfo _setStatusMethod;

    private static System.Array _statusEnumValues;

    private const BindingFlags InstanceBindingFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private static class PropertyNames
    {
        public const string InfiniteStamina = "infiniteStam";
        public const string StatusesLocked = "statusesLocked";
    }

    private static class FieldNames
    {
        public const string FallDamageTime = "fallDamageTime";
        public const string Stamina = "_stam";
        public const string MovementModifier = "movementModifier";
        public const string JumpGravity = "jumpGravity";
        public const string ClimbingSpeedModifier = "climbSpeedMod";
    }

    private static class MethodNames
    {
        public const string SetStatus = "SetStatus";
    }

    private static PropertyInfo GetProperty<T>(string name, BindingFlags bindingFlags = InstanceBindingFlags)
        where T : MonoBehaviour
    {
        return typeof(T).GetProperty(name, bindingFlags);
    }

    private static FieldInfo GetField<T>(string name, BindingFlags bindingFlags = InstanceBindingFlags)
        where T : MonoBehaviour
    {
        return typeof(T).GetField(name, bindingFlags);
    }

    private static MethodInfo GetMethod<T>(string name, BindingFlags bindingFlags = InstanceBindingFlags)
        where T : MonoBehaviour
    {
        return typeof(T).GetMethod(name, bindingFlags);
    }

    public static PropertyInfo InfiniteStaminaProperty =>
        _infiniteStaminaProperty ??= GetProperty<Character>(PropertyNames.InfiniteStamina);

    public static PropertyInfo StatusLockProperty =>
        _statusLockProperty ??= GetProperty<Character>(PropertyNames.StatusesLocked);

    public static FieldInfo FallDamageTimeField =>
        _fallDamageTimeField ??= GetField<CharacterMovement>(FieldNames.FallDamageTime);

    public static FieldInfo StaminaField => _staminaField ??= GetField<CharacterData>(FieldNames.Stamina);

    public static FieldInfo MovementModifierField =>
        _movementModifierField ??= GetField<CharacterMovement>(FieldNames.MovementModifier);

    public static FieldInfo JumpGravityField =>
        _jumpGravityField ??= GetField<CharacterMovement>(FieldNames.JumpGravity);

    public static FieldInfo ClimbSpeedModField =>
        _climbSpeedModField ??= GetField<CharacterClimbing>(FieldNames.ClimbingSpeedModifier);

    public static FieldInfo VineClimbSpeedModField => _vineClimbSpeedModField ??=
        GetField<CharacterVineClimbing>(FieldNames.ClimbingSpeedModifier);

    public static FieldInfo RopeClimbSpeedModField => _ropeClimbSpeedModField ??=
        GetField<CharacterRopeHandling>(FieldNames.ClimbingSpeedModifier);

    public static MethodInfo SetStatusMethod =>
        _setStatusMethod ??= GetMethod<CharacterAfflictions>(MethodNames.SetStatus);

    public static System.Array StatusEnumValues =>
        _statusEnumValues ??= System.Enum.GetValues(typeof(CharacterAfflictions.STATUSTYPE));

    public static void RefreshAll()
    {
        _infiniteStaminaProperty = null;
        _statusLockProperty = null;
        _fallDamageTimeField = null;
        _staminaField = null;
        _movementModifierField = null;
        _jumpGravityField = null;
        _climbSpeedModField = null;
        _vineClimbSpeedModField = null;
        _ropeClimbSpeedModField = null;
        _setStatusMethod = null;
        _statusEnumValues = null;
    }
}