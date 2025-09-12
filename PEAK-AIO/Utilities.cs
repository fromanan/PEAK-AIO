using BepInEx.Logging;
using DearImGuiInjection.BepInEx;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Zorro.Core.Serizalization;
using Object = UnityEngine.Object;

public static class Utilities
{
    public static ManualLogSource Logger;

    public static bool HasInitializedLuggageList;

    private static bool NoPlayer => Globals.PlayerObject is null || !Globals.PlayerObject;

    private static ItemSlot[] ItemSlots => Globals.PlayerObject.itemSlots;

    private static bool HasSlot(int slot) => ItemSlots is { } itemSlots && itemSlots.Length > slot;

    public static void GetPlayer()
    {
        if (NoPlayer)
        {
            Globals.PlayerObject = Player.localPlayer;
        }
    }

    public static void UpdateItems()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            Globals.Items.Clear();
            Globals.ItemNames.Clear();

            for (int i = 0; i < 3; i++)
            {
                Globals.SelectedItems[i] = -1;
            }

            Object[] allItems = Resources.FindObjectsOfTypeAll(typeof(Item));

            foreach (Object obj in allItems)
            {
                if (obj is not Item item || item.gameObject.scene.handle != 0 || !string.IsNullOrEmpty(item.gameObject.scene.name))
                    continue;
                
                Globals.Items.Add(item);
                Globals.ItemNames.Add(item.GetName());
            }
        });
    }

    public static void AssignInventoryItem(int slot, int itemIndex)
    {
        GetPlayer();

        if (NoPlayer)
        {
            Logger.LogError("[PEAK AIO] Player is null during inventory operation");
            return;
        }

        if (NoPlayer || !HasSlot(slot) || itemIndex < 0 || itemIndex >= Globals.Items.Count)
            return;
        
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            ItemSlot slotData = ItemSlots[slot];
            slotData.prefab = Globals.Items[itemIndex];
            slotData.data = new ItemInstanceData(Guid.NewGuid());
            ItemInstanceDataHandler.AddInstanceData(slotData.data);

            byte[] syncData = IBinarySerializable.ToManagedArray(
                new InventorySyncData(
                    ItemSlots,
                    Globals.PlayerObject.backpackSlot,
                    Globals.PlayerObject.tempFullSlot
                )
            );

            Globals.PlayerObject.photonView.RPC("SyncInventoryRPC", RpcTarget.Others, syncData, true);
        });
        
        Logger.LogInfo($"[Inventory] Assigned {Globals.ItemNames[itemIndex]} to slot {slot}");
    }

    public static void RechargeInventorySlot(int slot, float rechargeValue)
    {
        GetPlayer();

        if (Globals.PlayerObject is null)
        {
            Logger.LogError("[PEAK AIO] Player is null during inventory operation");
            return;
        }

        if (NoPlayer || HasSlot(slot))
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (ItemSlots[slot]?.data?.data is not { } data)
                return;

            foreach (KeyValuePair<DataEntryKey, DataEntryValue> kvp in data)
            {
                switch (kvp.Key)
                {
                    case DataEntryKey.PetterItemUses:
                    {
                        if (kvp.Value is IntItemData intData)
                        {
                            intData.Value = (int)rechargeValue;
                        }

                        break;
                    }
                    case DataEntryKey.Fuel:
                    case DataEntryKey.UseRemainingPercentage:
                    {
                        if (kvp.Value is FloatItemData floatData)
                        {
                            floatData.Value = rechargeValue;
                        }

                        break;
                    }
                    case DataEntryKey.ItemUses:
                    {
                        if (kvp.Value is OptionableIntItemData intData)
                        {
                            intData.Value = (int)rechargeValue;
                        }

                        break;
                    }
                }
            }
        });
        
        Logger.LogInfo($"[Inventory] Recharged slot {slot} to {rechargeValue}");
    }

    public static void RefreshPlayerList()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Globals.AllPlayers.Clear();
                Globals.PlayerNames.Clear();
                Globals.SelectedPlayer = -1;

                foreach (Character character in Character.AllCharacters)
                {
                    Globals.AllPlayers.Add(character);
                    Globals.PlayerNames.Add(character.characterName);
                }

                Logger.LogInfo($"[PlayerList] Found {Globals.AllPlayers.Count} players.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        });
    }

    public static void ReviveAllPlayers()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            foreach (Character character in Character.AllCharacters)
            {
                Vector3 revivePos = character.Ghost ? character.Ghost.transform.position : character.Head;
                character.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, revivePos + new Vector3(0f, 4f, 0f), false);
            }

            Logger.LogInfo("[Lobby] Revive All triggered.");
        });
    }

    public static void KillAllPlayers()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            foreach (Character character in Character.AllCharacters)
            {
                if (Globals.ExcludeSelfFromAllActions && character.IsLocal)
                    continue;

                Vector3 pos = character.transform.position;
                character.photonView.RPC("RPCA_Die", RpcTarget.All, pos);
            }

            Logger.LogInfo($"[Lobby] Kill All triggered. ExcludeSelf: {Globals.ExcludeSelfFromAllActions}");
        });
    }

    public static void WarpAllPlayersToMe()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            Vector3 myPos = Character.localCharacter.Head + new Vector3(0f, 4f, 0f);
            foreach (Character character in Character.AllCharacters)
            {
                character.photonView.RPC("WarpPlayerRPC", RpcTarget.All, myPos, true);
            }

            Logger.LogInfo("[Lobby] Warp All To Me triggered.");
        });
    }

    private static bool NoPlayers => Globals.SelectedPlayer < 0 || Globals.SelectedPlayer >= Globals.AllPlayers.Count;

    private static Character SelectedPlayer => Globals.AllPlayers[Globals.SelectedPlayer];

    public static void ReviveSelectedPlayer()
    {
        if (NoPlayers)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Character target = SelectedPlayer;
                Vector3 revivePos = target.Ghost ? target.Ghost.transform.position : target.Head;
                target.photonView.RPC("RPCA_ReviveAtPosition", RpcTarget.All, revivePos + new Vector3(0f, 4f, 0f),
                    false);
                Logger.LogInfo($"[Lobby] Revive requested for player index {Globals.SelectedPlayer}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        });
    }

    public static void KillSelectedPlayer()
    {
        if (NoPlayers)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Character target = SelectedPlayer;
                Vector3 spawnPoint = target.transform.position; // or any desired location
                target.photonView.RPC("RPCA_Die", RpcTarget.All, spawnPoint);
                Logger.LogInfo($"[Lobby] Kill requested for player index {Globals.SelectedPlayer}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        });
    }

    public static void WarpToSelectedPlayer()
    {
        if (NoPlayers)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Character target = SelectedPlayer;
                Vector3 targetPos = target.Head + new Vector3(0f, 4f, 0f);
                Character.localCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, targetPos, true);
                Logger.LogInfo($"[Lobby] Warp to requested for player index {Globals.SelectedPlayer}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        });
    }

    public static void WarpSelectedPlayerToMe()
    {
        if (NoPlayers)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                Character target = SelectedPlayer;
                Vector3 myHead = Character.localCharacter.Head + new Vector3(0f, 4f, 0f);
                target.photonView.RPC("WarpPlayerRPC", RpcTarget.All, myHead, true);
                Logger.LogInfo($"[Lobby] Warp to me requested for player index {Globals.SelectedPlayer}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        });
    }

    public static void TeleportToCoords(float x, float y, float z)
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                if (Character.localCharacter is not { } localCharacter || localCharacter.data.dead)
                {
                    Logger.LogWarning("[Teleport] Local character is null or dead. Aborting teleport.");
                    return;
                }

                if (localCharacter.photonView is not { } photonView)
                    return;

                var target = new Vector3(x, y, z);
                photonView.RPC("WarpPlayerRPC", RpcTarget.All, target, true);

                Logger.LogInfo($"[Teleport] Teleported to {target}");
            }
            catch (Exception ex)
            {
                Logger.LogError("[Teleport] Exception: " + ex);
            }
        });
    }

    public static void EnsureLuggageListInitialized()
    {
        if (HasInitializedLuggageList)
            return;
        HasInitializedLuggageList = true;
        RefreshLuggageList();
    }

    public static void RefreshLuggageList()
    {
        Globals.LuggageLabels.Clear();
        Globals.LuggageObjects.Clear();
        Globals.SelectedLuggageIndex = -1;

        var allLuggage = new List<(Luggage lug, float distance)>();

        foreach (Luggage luggage in Luggage.ALL_LUGGAGE.Where(l => l is not null))
        {
            float distance = Vector3.Distance(Character.localCharacter.Head, luggage.Center());
            if (distance <= 300)
            {
                allLuggage.Add((luggage, distance));
            }
        }

        // Sort by distance (closest first)
        allLuggage.Sort((a, b) => a.distance.CompareTo(b.distance));

        foreach ((Luggage lug, float distance) in allLuggage)
        {
            string name = lug.displayName ?? "Unnamed";
            Globals.LuggageLabels.Add($"{name} [{distance:F1}m]");
            Globals.LuggageObjects.Add(lug);
        }

        Logger.LogInfo($"[Luggage] Refreshed. Found {Globals.LuggageLabels.Count} nearby.");
    }

    public static void OpenAllNearbyLuggage()
    {
        UnityMainThreadDispatcher.Enqueue(() =>
        {
            int opened = 0;

            foreach (Luggage luggage in Globals.LuggageObjects)
            {
                if (luggage?.GetComponent<PhotonView>() is not { } view)
                    continue;
                view.RPC("OpenLuggageRPC", RpcTarget.All, true);
                opened++;
            }

            Logger.LogInfo($"[Luggage] Requested open for {opened} nearby containers.");
        });
    }

    public static void OpenLuggage(int index)
    {
        if (index < 0 || index >= Globals.LuggageObjects.Count)
            return;

        if (Globals.LuggageObjects[index] is not { } luggage)
            return;

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            try
            {
                if (luggage.GetComponent<PhotonView>() is not { } view)
                    return;
                view.RPC("OpenLuggageRPC", RpcTarget.All, true);
                Logger.LogInfo($"[Luggage] Sent OpenLuggageRPC for: {luggage.displayName}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"[Luggage] Open failed: {ex}");
            }
        });
    }

    public static void SpawnScoutmasterForPlayer(int playerIndex)
    {
        UnityMainThreadDispatcher.Enqueue(async () =>
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Logger.LogWarning("[Scoutmaster] Only the MasterClient can spawn the Scoutmaster.");
                return;
            }

            if (playerIndex < 0 || playerIndex >= Character.AllCharacters.Count)
            {
                Logger.LogWarning("[Scoutmaster] Invalid player index.");
                return;
            }

            Character targetCharacter = Character.AllCharacters[playerIndex];
            Vector3 targetPos = targetCharacter.transform.position;
            Vector3 spawnOrigin = targetPos + new Vector3(UnityEngine.Random.Range(-10f, 10f), 25f,
                UnityEngine.Random.Range(-10f, 10f));
            Vector3 down = Vector3.down;

            if (Physics.Raycast(spawnOrigin, down, out RaycastHit hit, 100f, ~0))
            {
                Vector3 spawnPoint = hit.point + Vector3.up * 1f;
                Quaternion rotation = Quaternion.identity;

                GameObject scoutObj = PhotonNetwork.InstantiateRoomObject("Character_Scoutmaster", spawnPoint, rotation);
                if (scoutObj.GetComponent<Character>() is { } character)
                {
                    character.data.spawnPoint = character.transform;
                }

                await Task.Delay(100);

                if (scoutObj.GetComponent<Scoutmaster>() is not { } scoutmaster)
                    return;
                try
                {
                    var method = typeof(Scoutmaster).GetMethod("SetCurrentTarget",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    if (method is not null)
                    {
                        method.Invoke(scoutmaster, new object[] { targetCharacter, 15f });
                        Logger.LogInfo($"[Scoutmaster] Target set to {targetCharacter.characterName}");
                    }
                    else
                    {
                        Logger.LogWarning("[Scoutmaster] Reflection failed — method not found.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"[Scoutmaster] Reflection error: {ex}");
                }
            }
            else
            {
                Logger.LogWarning("[Scoutmaster] No valid ground to spawn.");
            }
        });
    }
}