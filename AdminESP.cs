using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;

namespace AdminESP;

public sealed partial class AdminESP : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Admin ESP";
    public override string ModuleAuthor => "AquaVadis & GSM-RO";
    public override string ModuleVersion => "1.3.1"; 
    public override string ModuleDescription => "Plugin uses code borrowed from CS2Fixes / cs2kz-metamod / hl2sdk / unknown cheats and xstage from CS# discord";

    public bool[] toggleAdminESP = new bool[64]; 
    public bool[] wantESP = new bool[64]; 
    public bool togglePlayersGlowing = false;
    public Config Config { get; set; } = new();
    private static readonly ConVar? _forceCamera = ConVar.Find("mp_forcecamera");

    public override void Load(bool hotReload)
    {
        RegisterListeners();

        if (hotReload) {
      
            foreach (var player in Utilities.GetPlayers().Where(p => p is not null 
                                                                && p.IsValid is true
                                                                && p.Connected is PlayerConnectedState.PlayerConnected)) {

                if (cachedPlayers.Contains(player) is not true)
                    cachedPlayers.Add(player);

            }
        }

    }

    public override void Unload(bool hotReload)
    {

        DeregisterListeners();
    }

    [ConsoleCommand("css_esp", "Toggle Admin ESP (non-persistent)")]
    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnToggleAdminEsp(CCSPlayerController? player, CommandInfo command)
    {

        if (player is null || player.IsValid is not true) return;

        if (AdminManager.PlayerHasPermissions(player, Config.AdminFlag) is not true) {

            SendMessageToSpecificChat(player, msg: "Admin ESP can only be used from {GREEN}admins{DEFAULT}!", print: PrintTo.Chat);
            return;  
        }
        
        switch (player.PawnIsAlive) {
            
            case true:

                if (Config.AllowDeadAdminESP is true) {
                    SendMessageToSpecificChat(player, msg: "You should be {RED}dead {DEFAULT}to use Admin ESP!", print: PrintTo.Chat);
                    return;
                }
                SendMessageToSpecificChat(player, msg: "Admin ESP is only allowed while {RED}spectating{DEFAULT}!", print: PrintTo.Chat);

            break;
            case false:

                if (player.Team is CsTeam.Spectator) {
                    toggleAdminESP[player.Slot] = !toggleAdminESP[player.Slot];

                    if (toggleAdminESP[player.Slot] is true) {
                        
                        if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                            SetAllPlayersGlowing();
                        }
                        
                    }
                    else {

                        if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                            RemoveAllGlowingPlayers();
                        }
                        
                    }
                    

                    SendMessageToSpecificChat(player, msg: $"Admin ESP has been " + (toggleAdminESP[player.Slot] ? "{GREEN}enabled!" : "{RED}disabled!"), print: PrintTo.Chat); 
                    return;
                }
                else {
                    if (Config.AllowDeadAdminESP is true) {
                        toggleAdminESP[player.Slot] = !toggleAdminESP[player.Slot];

                        if (toggleAdminESP[player.Slot] is true) {

                            if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                                SetAllPlayersGlowing();
                            }
                            
                        }
                        else {

                            if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                                RemoveAllGlowingPlayers();
                            }
                            
                        }

                        SendMessageToSpecificChat(player, msg: $"Admin ESP has been " + (toggleAdminESP[player.Slot] ? "{GREEN}enabled!" : "{RED}disabled!"), print: PrintTo.Chat);
                        return;
                    
                    }
                }

                SendMessageToSpecificChat(player, msg: "Admin ESP is only allowed in {RED}spectate mode{DEFAULT}!", print: PrintTo.Chat);
            break;
        

        }
    }

    [ConsoleCommand("css_esp_p", "Toggle Persistent Admin ESP")]
    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnTogglePersistentAdminEsp(CCSPlayerController? player, CommandInfo command)
    {

        if (player is null || player.IsValid is not true) return;

        if (AdminManager.PlayerHasPermissions(player, Config.AdminFlag) is not true) {

            SendMessageToSpecificChat(player, msg: "Persistent Admin ESP can only be used from {GREEN}admins{DEFAULT}!", print: PrintTo.Chat);
            return;  
        }

        wantESP[player.Slot] = !wantESP[player.Slot];
        
        switch (player.PawnIsAlive) {
            
            case true:

                if (Config.AllowDeadAdminESP is true) {
                    SendMessageToSpecificChat(player, msg: "You should be {RED}dead {DEFAULT}to use Persistent Admin ESP!", print: PrintTo.Chat);
                    return;
                }
                SendMessageToSpecificChat(player, msg: "Persistent Admin ESP is only allowed while {RED}spectating{DEFAULT}!", print: PrintTo.Chat);

            break;
            case false:

                if (player.Team is CsTeam.Spectator) {
                    toggleAdminESP[player.Slot] = wantESP[player.Slot]; 

                    if (toggleAdminESP[player.Slot] is true) {
                        
                        if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                            SetAllPlayersGlowing();
                        }
                    }
                    else {

                        if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                            RemoveAllGlowingPlayers();
                        }
                         }
                    

                    SendMessageToSpecificChat(player, msg: $"Persistent Admin ESP has been " + (wantESP[player.Slot] ? "{GREEN}enabled!" : "{RED}disabled!"), print: PrintTo.Chat); 
                    return;
                }
                else {
                    if (Config.AllowDeadAdminESP is true) {
                        toggleAdminESP[player.Slot] = wantESP[player.Slot]; 

                        if (toggleAdminESP[player.Slot] is true) {

                            if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                                SetAllPlayersGlowing();
                            }
                        }
                        else {

                            if (togglePlayersGlowing is not true || AreThereEsperingAdmins() is not true) {
                                RemoveAllGlowingPlayers();
                            }
                        }

                        SendMessageToSpecificChat(player, msg: $"Persistent Admin ESP has been " + (wantESP[player.Slot] ? "{GREEN}enabled!" : "{RED}disabled!"), print: PrintTo.Chat);
                        return;
                    
                    }
                }

                SendMessageToSpecificChat(player, msg: "Persistent Admin ESP is only allowed in {RED}spectate mode{DEFAULT}!", print: PrintTo.Chat);
            break;
        

        }
    }

    public void OnConfigParsed(Config config)
    {
        Config = config;
    }

}