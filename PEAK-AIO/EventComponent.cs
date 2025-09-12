using UnityEngine;

public class EventComponent : MonoBehaviour
{
    private CharacterMovement _movement;

    private CharacterMovement Movement
    {
        get
        {
            _movement ??= GameHelpers.GetMovementComponent();
            return _movement;
        }
    }

    private Character _character;
    
    private Character Character
    {
        get
        {
            _character ??= GameHelpers.GetCharacter();
            return _character;
        }
    }

    private CharacterClimbing _climb;
    
    private CharacterClimbing Climb
    {
        get
        {
            _climb ??= GameHelpers.GetClimbingComponent();
            return _climb;
        }
    }

    private CharacterVineClimbing _vine;
    
    private CharacterVineClimbing Vine
    {
        get
        {
            _vine ??= GameHelpers.GetVineClimbComponent();
            return _vine;
        }
    }

    private CharacterRopeHandling _rope;
    
    private CharacterRopeHandling Rope
    {
        get
        {
            _rope ??= GameHelpers.GetRopeClimbComponent();
            return _rope;
        }
    }
    
    private void Update()
    {
        if (Movement is not null)
        {
            if (ConfigManager.SpeedMod.Value)
            {
                ConstantFields.GetMovementModifierField()?.SetValue(Movement, ConfigManager.SpeedAmount.Value);
            }

            if (ConfigManager.JumpMod.Value)
            {
                ConstantFields.GetJumpGravityField()?.SetValue(Movement, ConfigManager.JumpAmount.Value);

                if (ConfigManager.NoFallDmg.Value)
                {
                    ConstantFields.GetFallDamageTimeField()?.SetValue(Movement, 999f);
                }
            }
        }

        if (Character is not null)
        {
            if (ConfigManager.InfiniteStamina.Value)
            {
                ConstantFields.GetInfiniteStaminaProperty()?.SetValue(Character, true);
            }

            if (ConfigManager.LockStatus.Value)
            {
                ConstantFields.GetStatusLockProperty()?.SetValue(Character, true);
            }
        }

        if (Climb is not null && ConfigManager.ClimbMod.Value)
        {
            ConstantFields.GetClimbSpeedModField()?.SetValue(Climb, ConfigManager.ClimbAmount.Value);
        }

        if (Vine is not null && ConfigManager.VineClimbMod.Value)
        {
            ConstantFields.GetVineClimbSpeedModField()?.SetValue(Vine, ConfigManager.VineClimbAmount.Value);
        }

        if (Rope is not null && ConfigManager.RopeClimbMod.Value)
        {
            ConstantFields.GetRopeClimbSpeedModField()?.SetValue(Rope, ConfigManager.RopeClimbAmount.Value);
        }
    }
}