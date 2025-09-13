using UnityEngine;
using static ConfigManager;
using static ConstantFields;

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
            if (SpeedMod.Value)
            {
                MovementModifierField?.SetValue(Movement, SpeedAmount.Value);
            }

            if (JumpMod.Value)
            {
                JumpGravityField?.SetValue(Movement, JumpAmount.Value);

                if (NoFallDmg.Value)
                {
                    FallDamageTimeField?.SetValue(Movement, 999f);
                }
            }
        }

        if (Character is not null)
        {
            if (InfiniteStamina.Value)
            {
                InfiniteStaminaProperty?.SetValue(Character, true);
            }

            if (LockStatus.Value)
            {
                StatusLockProperty?.SetValue(Character, true);
            }
        }

        if (Climb is not null && ClimbMod.Value)
        {
            ClimbSpeedModField?.SetValue(Climb, ClimbAmount.Value);
        }

        if (Vine is not null && VineClimbMod.Value)
        {
            VineClimbSpeedModField?.SetValue(Vine, VineClimbAmount.Value);
        }

        if (Rope is not null && RopeClimbMod.Value)
        {
            RopeClimbSpeedModField?.SetValue(Rope, RopeClimbAmount.Value);
        }
    }
}