using System.Collections.Generic;
using System.Reflection;

public static class Globals
{
    // Boolean
    
    public static bool AnyAfflictionEnabled;

    // Objects
    
    public static Character Character;
    
    public static CharacterData CharacterData;

    public static FieldInfo StaminaField;
    
    public static PropertyInfo InfiniteStamProp;

    public static FieldInfo SinceFallSlideField;
    
    public static FieldInfo SinceGroundedField;

    public static object MovementComp;
    
    public static FieldInfo MovementModifierField;
    
    public static FieldInfo JumpGravityField;
    
    public static FieldInfo FallDamageTimeField;

    public static object CharacterClimb;
    
    public static FieldInfo ClimbSpeedModifierField;

    public static object CharacterVineClimb;
    
    public static FieldInfo VineClimbSpeedModifierField;

    public static object CharacterRopeHandling;
    
    public static FieldInfo RopeClimbSpeedModifierField;

    public static object AfflictionsObject;
    
    public static MethodInfo SetStatusMethod;
    
    public static object WeightEnumValue;
    
    public static object PoisonEnumValue;
    
    public static object HotEnumValue;
    
    public static object ColdEnumValue;
    
    public static object CurseEnumValue;
    
    public static object InjuryEnumValue;
    
    public static object DrowsyEnumValue;
    
    public static object HungerEnumValue;

    // Inventory
    
    public static readonly List<Item> Items = new();
    
    public static readonly List<string> ItemNames = new();
    
    public static readonly int[] SelectedItems = { -1, -1, -1 };
    
    public static string[] ItemDisplayNames = { None, None, None };
    
    public static string[] ItemSearchBuffers = new string[3];

    // Player
    
    public static Player PlayerObject;

    // Lobby
    
    public static readonly List<Character> AllPlayers = new();
    
    public static readonly List<string> PlayerNames = new();
    
    public static int SelectedPlayer = -1;
    
    public static bool ExcludeSelfFromAllActions = true;

    // Teleport
    
    public static bool TeleportToPingEnabled = false;
    
    public static float TeleportX = 0f;
    
    public static float TeleportY = 0f;
    
    public static float TeleportZ = 0f;

    // World
    
    public static int SelectedLuggageIndex = -1;
    
    public static readonly List<string> LuggageLabels = new();
    
    public static readonly List<Luggage> LuggageObjects = new();
    
    public static List<Luggage> AllOpenedLuggage = new();
    
    // Constants

    public const string None = "None";
}