using System.Reflection;

internal static class ConstantFields
{
    private static PropertyInfo _infiniteStaminaProp;

    private static PropertyInfo _statusLockProp;

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

    public static PropertyInfo GetInfiniteStaminaProperty()
    {
        return _infiniteStaminaProp ??= typeof(Character).GetProperty(PropertyNames.InfiniteStamina,
            InstanceBindingFlags);
    }

    public static PropertyInfo GetStatusLockProperty()
    {
        return _statusLockProp ??= typeof(Character).GetProperty(PropertyNames.StatusesLocked,
            InstanceBindingFlags);
    }

    public static FieldInfo GetFallDamageTimeField()
    {
        return _fallDamageTimeField ??= typeof(CharacterMovement).GetField(
            FieldNames.FallDamageTime,
            InstanceBindingFlags);
    }

    public static FieldInfo GetStaminaField()
    {
        return _staminaField ??= typeof(CharacterData).GetField(FieldNames.Stamina,
            InstanceBindingFlags);
    }

    public static FieldInfo GetMovementModifierField()
    {
        return _movementModifierField ??= typeof(CharacterMovement).GetField(
            FieldNames.MovementModifier,
            InstanceBindingFlags);
    }

    public static FieldInfo GetJumpGravityField()
    {
        return _jumpGravityField ??= typeof(CharacterMovement).GetField(FieldNames.JumpGravity,
            InstanceBindingFlags);
    }

    public static FieldInfo GetClimbSpeedModField()
    {
        return _climbSpeedModField ??= typeof(CharacterClimbing).GetField(
            FieldNames.ClimbingSpeedModifier,
            InstanceBindingFlags);
    }

    public static FieldInfo GetVineClimbSpeedModField()
    {
        return _vineClimbSpeedModField ??= typeof(CharacterVineClimbing).GetField(
            FieldNames.ClimbingSpeedModifier,
            InstanceBindingFlags);
    }

    public static FieldInfo GetRopeClimbSpeedModField()
    {
        return _ropeClimbSpeedModField ??= typeof(CharacterRopeHandling).GetField(
            FieldNames.ClimbingSpeedModifier,
            InstanceBindingFlags);
    }

    public static MethodInfo GetSetStatusMethod()
    {
        return _setStatusMethod ??= typeof(CharacterAfflictions).GetMethod(MethodNames.SetStatus,
            InstanceBindingFlags);
    }

    public static System.Array GetStatusEnumValues()
    {
        return _statusEnumValues ??= System.Enum.GetValues(typeof(CharacterAfflictions.STATUSTYPE));
    }

    public static void RefreshAll()
    {
        _infiniteStaminaProp = null;
        _statusLockProp = null;
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