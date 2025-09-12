using HarmonyLib;
using Photon.Pun;
using System;
using UnityEngine;

[HarmonyPatch(typeof(PointPinger), "ReceivePoint_Rpc")]
public class PointPingPatch
{
    private static void Postfix(Vector3 point, Vector3 hitNormal, PointPinger __instance)
    {
        try
        {
            if (!ConfigManager.TeleportToPing.Value)
                return;

            var owner = __instance.character?.photonView?.Owner;
            if (owner is null || !Equals(owner, PhotonNetwork.LocalPlayer) || !Character.localCharacter || Character.localCharacter.data.dead)
                return;
            Vector3 safePoint = point + Vector3.up;
            Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, safePoint, true);

            ConfigManager.Logger.LogInfo("[Patch] Teleported to ping!");
        }
        catch (Exception ex)
        {
            ConfigManager.Logger.LogError($"[Patch] Exception: {ex}");
        }
    }
}

[HarmonyPatch(typeof(Character), "Update")]
public class FlyPatch
{
    private static Vector3 flyVelocity = Vector3.zero;
    
    public static bool IsFlying { get; private set; }

    public static void SetFlying(bool enable)
    {
        IsFlying = enable;
        flyVelocity = Vector3.zero;

        ConfigManager.Logger.LogInfo($"[FlyMod] Flight {(enable ? "enabled" : "disabled")}.");
    }
    
    private static void Postfix(Character __instance)
    {
        if (!__instance.IsLocal)
            return;

        if (!ConfigManager.FlyMod.Value)
        {
            if (!IsFlying)
                return;
            IsFlying = false;
            flyVelocity = Vector3.zero;
            ConfigManager.Logger.LogInfo("[FlyMod] Flight disabled.");
            return;
        }

        if (!IsFlying)
        {
            IsFlying = true;
            ConfigManager.Logger.LogInfo("[FlyMod] Flight enabled.");
        }

        __instance.data.isGrounded = true;
        __instance.data.sinceGrounded = 0f;
        __instance.data.sinceJump = 0f;

        Vector3 input = __instance.input.movementInput;
        Vector3 forward = __instance.data.lookDirection_Flat.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
        Vector3 moveVec = forward * input.y + right * input.x;

        if (__instance.input.jumpIsPressed)
        {
            moveVec += Vector3.up;
        }

        if (__instance.input.crouchIsPressed)
        {
            moveVec += Vector3.down;
        }

        float speed = ConfigManager.FlySpeed.Value;
        float accel = ConfigManager.FlyAcceleration.Value;

        flyVelocity = Vector3.Lerp(flyVelocity, moveVec.normalized * speed, Time.deltaTime * accel);

        foreach (Bodypart part in __instance.refs.ragdoll.partList)
        {
            if (part?.Rig is { } rigidbody)
            {
                rigidbody.linearVelocity = flyVelocity;
            }
        }
    }
}

[HarmonyPatch(typeof(CharacterAfflictions), "UpdateWeight")]
public class Patch_UpdateWeight
{
    private static void Postfix(CharacterAfflictions __instance)
    {
        if (ConfigManager.NoWeight.Value)
        {
            __instance.SetStatus(CharacterAfflictions.STATUSTYPE.Weight, 0f);
        }
    }
}