#nullable enable

using UnityEngine;

internal static class GameHelpers
{
    private static Character? _character;
    
    private static CharacterData? _characterData;
    
    private static CharacterMovement? _movementComponent;
    
    private static CharacterAfflictions? _afflictionsComponent;
    
    private static CharacterClimbing? _climbingComponent;
    
    private static CharacterVineClimbing? _vineClimbingComponent;
    
    private static CharacterRopeHandling? _ropeClimbingComponent;

    // ReSharper disable Unity.PerformanceAnalysis
    private static T? GetCharacterComponent<T>() where T : Component
    {
        return GetCharacter()?.GetComponent<T>();
    }

    public static Character? GetCharacter()
    {
        if (_character is null || !_character.isActiveAndEnabled)
        {
            _character = Character.localCharacter;
        }

        return _character;
    }

    public static CharacterData? GetCharacterData()
    {
        if (_characterData is null || !_characterData.isActiveAndEnabled)
        {
            _characterData = Object.FindFirstObjectByType<CharacterData>();
        }

        return _characterData;
    }

    public static CharacterMovement? GetMovementComponent()
    {
        if (_movementComponent is null || !_movementComponent.isActiveAndEnabled)
        {
            _movementComponent = GetCharacterComponent<CharacterMovement>();
        }

        return _movementComponent;
    }

    public static CharacterAfflictions? GetAfflictionsComponent()
    {
        if (_afflictionsComponent is null || !_afflictionsComponent.isActiveAndEnabled)
        {
            _afflictionsComponent = GetCharacterComponent<CharacterAfflictions>();
        }

        return _afflictionsComponent;
    }

    public static CharacterClimbing? GetClimbingComponent()
    {
        if (_climbingComponent is null || !_climbingComponent.isActiveAndEnabled)
        {
            _climbingComponent = GetCharacterComponent<CharacterClimbing>();
        }

        return _climbingComponent;
    }

    public static CharacterVineClimbing? GetVineClimbComponent()
    {
        if (_vineClimbingComponent is null || !_vineClimbingComponent.isActiveAndEnabled)
        {
            _vineClimbingComponent = GetCharacterComponent<CharacterVineClimbing>();
        }

        return _vineClimbingComponent;
    }

    public static CharacterRopeHandling? GetRopeClimbComponent()
    {
        if (_ropeClimbingComponent is null || !_ropeClimbingComponent.isActiveAndEnabled)
        {
            _ropeClimbingComponent = GetCharacterComponent<CharacterRopeHandling>();
        }

        return _ropeClimbingComponent;
    }

    public static void Refresh()
    {
        _character = null;
        _characterData = null;
        _movementComponent = null;
        _afflictionsComponent = null;
        _climbingComponent = null;
        _vineClimbingComponent = null;
        _ropeClimbingComponent = null;
    }
}