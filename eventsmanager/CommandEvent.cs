using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Collections;
using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Unturned.Chat;
using System.Collections;
using UnityEngine;
using Rocket.Unturned.Player;
using Rocket.Core.Logging;
using Steamworks;
using SDG.Unturned;
using Rocket.Unturned.Items;


namespace datathegenius.eventsmanager
{
    public class CommandEvent : IRocketCommand
    {
        //If player leaves, then remove them.. If game is active, let them join and go to waiting area.
        //If /home, clear them or kill
        //Groups, then can't join.
        //If 
        public static Boolean started = false;
        public static int activePlayers = 0;
        public static Boolean eventActive = false;
        public string gameMode = "paintball";
        public static Vector3 position1;
        public static Vector3 position2;

        public static List<string> usedStorage = new List<string>();

        //        ArrayList joinedPlayers;
        //        PlayerData[] joinedPlayers;
        public static List<PlayerData> joinedPlayers = new List<PlayerData>() { };

        public List<PlayerData> getJoinedPlayerList()
        {
            return joinedPlayers;
        }

        public Boolean getStarted()
        {
            return started;
        }

        public Vector3 getPosition1()
        {
            return position1;
        }

        public Vector3 getPosition2()
        {
            return position2;
        }

        public List<string> Aliases
        {
            get
            {
                return new List<string>();
            }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return Rocket.API.AllowedCaller.Player; ;
            }
        }

        public string Help
        {
            get
            {
                return "Manages player events for paintball.";

            }
        }

        public string Name
        {
            get
            {
                return "paintball";
            }
        }

        public bool RunFromConsole
        {
            get { return false; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "eventsmanager.event" };
            }
        }

        public string Syntax
        {
            get
            {
                return "<paintball>";
            }
        }

        //       [RocketCommand("event", "Main command to start and manage events.", "<what to do>", AllowedCaller.Player)]
        public void Execute(IRocketPlayer caller, string[] parameters)
        {
            //           Logger.Log("Inside Execute command.");
            string[] command = new string[1];
            UnturnedPlayer pCaller = (UnturnedPlayer)caller;

            var tempCharacterInfoDuplicate = joinedPlayers.FirstOrDefault(item => item.getSteamID() == pCaller.CSteamID.ToString());
            var tempCharacterInfoGroupDuplicate = joinedPlayers.FirstOrDefault(item => item.getGroupID() == pCaller.SteamGroupID.ToString());

            if (parameters.Count() == 1)
            {
                command[0] = parameters[0].ToLower();
            }
            else
            {
                UnturnedChat.Say(caller, Events.Instance.Translate("events_invalid_parameter"), Events.Instance.Configuration.Instance.ErrorColor);
            }

            //Claim Reward
            if (command[0] == "reward")
            {
                if (!started && !eventActive)
                {
                    int playerWins = getWins(pCaller);
                    //      Logger.Log("Wins: " + playerWins);

                    if (playerWins == 0)
                    {
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_no_reward"), Events.Instance.Configuration.Instance.ErrorColor);
                        return;
                    }
                    else
                    {
                        if (playerWins >= 1)
                        {
                            pCaller.GiveItem(253, 1);
                        }
                        if (playerWins >= 2)
                        {
                            pCaller.GiveItem(307, 1);
                            pCaller.GiveItem(308, 1);
                            pCaller.GiveItem(309, 1);
                            pCaller.GiveItem(310, 1);
                        }
                        if (playerWins >= 3)
                        {
                            pCaller.Inventory.tryAddItem(UnturnedItems.AssembleItem(1337, 250, new Attachment(1004, 100), new Attachment(151, 100), new Attachment(8, 100), new Attachment(1338, 1), new Attachment(1340, 100), EFiremode.AUTO), true);
                        }
                        if (playerWins >= 4)
                        {
                            pCaller.GiveItem(363, 1);
                            pCaller.GiveItem(6, 1);
                        }
                        if (playerWins >= 5)
                        {
                            pCaller.GiveItem(116, 1);
                            pCaller.GiveItem(6, 1);
                        }
                        if (playerWins >= 6)
                        {
                            pCaller.GiveItem(18, 1);
                            pCaller.GiveItem(20, 1);
                        }
                        if (playerWins >= 7)
                        {
                            pCaller.GiveItem(132, 1);
                            pCaller.GiveItem(133, 1);
                        }
                        if (playerWins >= 8)
                        {
                            pCaller.GiveItem(519, 1);
                            pCaller.GiveItem(520, 1);
                        }

                        UnturnedChat.Say(caller, Events.Instance.Translate("events_reward_given"), Events.Instance.Configuration.Instance.SuccessColor);
                    }
                    //Go through players and count wins and give prize based on that.
                    //Remove name from list once claimed.
                    //     pCaller.Inventory.tryAddItem(UnturnedItems.AssembleItem(363, 30, new Attachment(1004, 100), new Attachment(151, 100), new Attachment(8, 100), new Attachment(7, 100), new Attachment(6, 100), EFiremode.AUTO), true);
                    //   pCaller.GiveItem(394, 100);
                    return;
                }
                else
                {
                    UnturnedChat.Say(caller, Events.Instance.Translate("events_error_not_finished_reward"), Events.Instance.Configuration.Instance.ErrorColor);
                    return;
                }
            }

            #region on
            //Begin commands
            //On event command
            if (command[0] == "on")
            {
                if (caller.IsAdmin)
                {
                    if (!started)
                    {
                        joinedPlayers = new List<PlayerData>() { };
                        usedStorage = new List<string>();
                        Events.winners = "";
                        started = true;
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_on"), Events.Instance.Configuration.Instance.SuccessColor);
       //                 UnturnedChat.Say(Events.Instance.Translate("events_join_announcement", gameMode), Events.Instance.Configuration.Instance.AnnouncementColor);
                        return;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_already_active"), Events.Instance.Configuration.Instance.ErrorColor);
                        return;
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, Events.Instance.Translate("events_no_permission"), Events.Instance.Configuration.Instance.ErrorColor);
                }
                return;
            }
            #endregion

            #region off
            //Off event command
            if (command[0] == "off")
            {
                if (caller.IsAdmin)
                {
                    if (started)
                    {
                        started = false;
                        eventActive = false;

                        Logger.Log("Total PLayer Count on Off: " + joinedPlayers.Count);
                        int playerCountBeforeDump = joinedPlayers.Count;
                        for (int x = 0; x < playerCountBeforeDump; x++)
                        {
                            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(joinedPlayers[0].getSteamID())));
                            Logger.Log("Inside Off Loop" + x);
                            //          player.GodMode = false;
                            //              Logger.Log("Ran list");
                            leaveEvent(player);
                        }
    //                    leaveEvent(pCaller);

                        UnturnedChat.Say(caller, Events.Instance.Translate("events_off"), Events.Instance.Configuration.Instance.SuccessColor);
                        UnturnedChat.Say(Events.Instance.Translate("events_ended_announcement"), Events.Instance.Configuration.Instance.AnnouncementColor);
                        return;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_already_notactive"), Events.Instance.Configuration.Instance.ErrorColor);
                        return;
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, Events.Instance.Translate("events_no_permission"), Events.Instance.Configuration.Instance.ErrorColor);
                }
                return;
            }
            #endregion

            //Permission Reload command
            if (command[0] == "permreload")
            {
                if (caller.IsAdmin)
                {
                    foreach (SteamPlayer plr in Provider.Players)
                    {
                        //So let's convert each SteamPlayer into an UnturnedPlayer
                        UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(plr);

                        unturnedPlayer.GodMode = false;

                        //Reset permissions
                        Rocket.Core.R.Permissions.RemovePlayerFromGroup("EventGroup", unturnedPlayer);
                        Rocket.Core.R.Permissions.AddPlayerToGroup("Guest", unturnedPlayer);
                    }
                    UnturnedChat.Say(caller, "Done.", Events.Instance.Configuration.Instance.SuccessColor);
                    return;
                }
                else
                {
                    UnturnedChat.Say(caller, Events.Instance.Translate("events_no_permission"), Events.Instance.Configuration.Instance.ErrorColor);
                    return;
                }
            }

            
    
            if (started)
            {
                //Give lockers to store.
                if (command[0] == "storage")
                {
                    var tempCharacterInfoDuplicateLockers = usedStorage.FirstOrDefault(item => item == pCaller.CSteamID.ToString());

                    if (tempCharacterInfoDuplicateLockers == null)
                    {
                        pCaller.GiveItem(328, 4);
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_storage_given"), Events.Instance.Configuration.Instance.SuccessColor);
                        usedStorage.Add(pCaller.CSteamID.ToString());
                        return;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_storage_used"), Events.Instance.Configuration.Instance.ErrorColor);
                        return;
                    }
                }

                #region join
                //Join Event
                if (command[0] == "join")
                {

           //         Logger.Log("Join Command");
                    
                    //                joinedPlayers += pCaller.CSteamID;
                    //If player is not already added, add them
                    if (tempCharacterInfoDuplicate == null)
                    {
         //               Logger.Log("Group: ." + pCaller.SteamGroupID + ".");
                        if(tempCharacterInfoGroupDuplicate == null || pCaller.SteamGroupID.ToString() == "0")
                        {
                            PlayerData thisPlayer = new PlayerData(pCaller.CharacterName, pCaller.CSteamID.ToString(), pCaller.SteamGroupID.ToString(), pCaller.Position);
                            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(thisPlayer.getSteamID())));

                            joinedPlayers.Add(thisPlayer);
                            Rocket.Core.R.Permissions.RemovePlayerFromGroup("Guest", player);
                            Rocket.Core.R.Permissions.AddPlayerToGroup("EventGroup", player);

                            //During active event, send to position2.
                            if (eventActive)
                            {
                                //          pCaller.Suicide();
                                player.Teleport(position2, 0);
                                clearInventory(player);
                                player.GodMode = true;
                                thisPlayer.setDead(true);

                            }
                            UnturnedChat.Say(caller, Events.Instance.Translate("events_joined_game"), Events.Instance.Configuration.Instance.SuccessColor);
                            return;
                        }
                        else
                        {
                            UnturnedChat.Say(caller, Events.Instance.Translate("events_group_error"), Events.Instance.Configuration.Instance.ErrorColor);
                            return;
                        }

                    }
                    else if (tempCharacterInfoDuplicate != null)
                    {
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_already_joined_game", tempCharacterInfoDuplicate.getName()), Events.Instance.Configuration.Instance.ErrorColor);
                        return;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_error"), Events.Instance.Configuration.Instance.ErrorColor);
                        return;
                    }

                }
                #endregion

                #region leave
                //Leave Event
                if (command[0] == "leave")
                {
       //             Logger.Log("Leave Command");
                    leaveEvent(pCaller);
                    return;
                    //                joinedPlayers += pCaller.CSteamID;
                    //                PlayerData thisPlayer = new PlayerData(pCaller.CharacterName, pCaller.CSteamID, pCaller.Position);
                    //               joinedPlayers.Add(thisPlayer);
                    //                UnturnedChat.Say(caller, Translate("events_joined_game"));
                }
                #endregion

                #region list
                //List All Joined Players
                if (command[0] == "list")
                {
   //                 Logger.Log("List Command");

                    if (caller.IsAdmin)
                    {
     //                   Logger.Log("List Command - admin");
                        string allJoinedPlayers = "";
                        for (int x = 0; x < joinedPlayers.Count; x++)
                        {
   //                         Logger.Log("Ran list");
                            allJoinedPlayers += joinedPlayers[x].getName() + " ";
                        }
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_player_list", joinedPlayers.Count, allJoinedPlayers));
                        //                    Logger.Log(string.Join(Environment.NewLine, joinedPlayers.getName()));

     //                   Logger.Log(allJoinedPlayers);
                        return;
                    }
                    else
                    {
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_no_permission"), Events.Instance.Configuration.Instance.ErrorColor);
                        return;
                    }
                }
                #endregion

                //Paintball Game Mode
                if(gameMode == "paintball")
                {
                    if(caller.IsAdmin)
                    { 
                        if (command[0] == "position1")
                        {
                            position1 = pCaller.Position;
                            UnturnedChat.Say(caller, Events.Instance.Translate("events_position_set", "position1", position1), Events.Instance.Configuration.Instance.SuccessColor);
                        }

                        if(command[0] == "position2")
                        {
                            position2 = pCaller.Position;
                            UnturnedChat.Say(caller, Events.Instance.Translate("events_position_set", "position2", position2), Events.Instance.Configuration.Instance.SuccessColor);
                        }

                        //Command start event
                        if (command[0] == "start")
                        {
                            if (!eventActive)
                            {
                                if (joinedPlayers.Count > 1)
                                {
                                    //                            Logger.Log("Event Start");
                                    eventActive = true;
                                    activePlayers = joinedPlayers.Count;


                                    for (int x = 0; x < joinedPlayers.Count; x++)
                                    {
                                        UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(joinedPlayers[x].getSteamID())));

                                        player.GodMode = false;

                                        if(!joinedPlayers[x].getRevived())
                                        {
                                            leaveEvent(player);
                                        }
                                        else if(joinedPlayers[x].getRevived())
                                        {
                                            //                                  player.GodMode = false;
                                            joinedPlayers[x].setDead(false);
                                            //                                  Logger.Log("Event Start 2");

                                            clearInventory(player);
                                            healPlayer(player);
                                            maxSkills(player);

                                            //                                  Logger.Log("Event Start 3");
                                            //Paintball gun, 3 hoppers, 5 bandages, 
                                            player.Inventory.tryAddItem(UnturnedItems.AssembleItem(1337, 250, new Attachment(1004, 100), new Attachment(151, 100), new Attachment(8, 100), new Attachment(1338, 1), new Attachment(1340, 100), EFiremode.SEMI), true);
                                            player.GiveItem(1048, 1);
                                            player.GiveItem(394, 4);
                                            player.GiveItem(1133, 1);
                                            player.GiveItem(431, 1);
                                            player.GiveItem(177, 1);
                                            player.GiveItem(548, 1);
                                            player.GiveItem(1340, 5);

                                            player.GodMode = false;
                                            player.Teleport(position1, 0);

                                            UnturnedChat.Say(player, Events.Instance.Translate("events_started", "position1", position1), Events.Instance.Configuration.Instance.SuccessColor);
                                        }
                                        else
                                        {
                                            UnturnedChat.Say(player, "Error. #3 Tell an admin the error code.", Events.Instance.Configuration.Instance.ErrorColor);
                                        }

                                    }
                                    //Winning method here
                                    Events myEvent = new Events();
                                    myEvent.dealWithWinEvent();
                                }
                                else
                                {
                                    UnturnedChat.Say(caller, Events.Instance.Translate("events_start_only_one"), Events.Instance.Configuration.Instance.ErrorColor);
                                }
                            }
                            else
                            {
                                UnturnedChat.Say(caller, Events.Instance.Translate("events_started_already"), Events.Instance.Configuration.Instance.ErrorColor);
                            }
                        }

                        //Command stop event
                        //Removes people and sends them to position2. 
                        if (command[0] == "stop")
                        {
                            if (eventActive)
                            {
    //                            Logger.Log("Event Stop");
                                eventActive = false;
                                for (int x = 0; x < joinedPlayers.Count; x++)
                                {
                                    UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(joinedPlayers[x].getSteamID())));
                                    player.GodMode = true;
                                    joinedPlayers[x].setDead(true);
    //                                Logger.Log("Event Start 2");

                                    clearInventory(player);
                                    healPlayer(player);
                                    

    //                                Logger.Log("Event Start 3");
           //                         player.GodMode = true;
                                    clearInventory(player);
                                    player.Teleport(position2, 0);

                                    UnturnedChat.Say(player, Events.Instance.Translate("events_stopped"), Events.Instance.Configuration.Instance.SuccessColor);
                                }
                            }
                            else
                            {
                                UnturnedChat.Say(caller, Events.Instance.Translate("events_stopped_already"), Events.Instance.Configuration.Instance.ErrorColor);
                            }
                        }

      //                  Logger.Log("Command Start Finished");
                    }
                    else 
                    {
                        UnturnedChat.Say(caller, Events.Instance.Translate("events_no_permission"), Events.Instance.Configuration.Instance.ErrorColor);
                        return;
                    }


                }
            }
            else
            {
                UnturnedChat.Say(caller, Events.Instance.Translate("events_not_active"), Events.Instance.Configuration.Instance.ErrorColor);
                return;
            }
        }

        public void maxSkills(UnturnedPlayer tempPlayer)
        {
            tempPlayer.Experience += Events.Instance.Configuration.Instance.expAmount;

            UnturnedChat.Say(tempPlayer, "You received " + Events.Instance.Configuration.Instance.expAmount + " experience to upgrade your skills.", Color.cyan);
        }

        public void removeFromList(UnturnedPlayer player)
        {
            var itemToRemove = joinedPlayers.Single(r => r.getSteamID() == player.CSteamID.ToString());
            joinedPlayers.Remove(itemToRemove);
        }

        public void clearInventory(UnturnedPlayer tempPlayer)
        {
            var playerInventory = tempPlayer.Inventory;

            // "Remove "models" of items from player "body""
            tempPlayer.Player.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, (byte)0, (byte)0, new byte[0]);
            tempPlayer.Player.channel.send("tellSlot", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, (byte)1, (byte)0, new byte[0]);

            // Remove items
            for (byte page = 0; page < 8; page++)
            {
                var count = playerInventory.getItemCount(page);

                for (byte index = 0; index < count; index++)
                {
                    playerInventory.removeItem(page, 0);
                }
            }

            // Remove clothes

            // Remove unequipped cloths
            System.Action removeUnequipped = () =>
            {
                for (byte i = 0; i < playerInventory.getItemCount(2); i++)
                {
                    playerInventory.removeItem(2, 0);
                }
            };

            // Unequip & remove from inventory
            tempPlayer.Player.clothing.askWearBackpack(0, 0, new byte[0], true);
            removeUnequipped();

            tempPlayer.Player.clothing.askWearGlasses(0, 0, new byte[0], true);
            removeUnequipped();

            tempPlayer.Player.clothing.askWearHat(0, 0, new byte[0], true);
            removeUnequipped();

            tempPlayer.Player.clothing.askWearPants(0, 0, new byte[0], true);
            removeUnequipped();

            tempPlayer.Player.clothing.askWearMask(0, 0, new byte[0], true);
            removeUnequipped();

            tempPlayer.Player.clothing.askWearShirt(0, 0, new byte[0], true);
            removeUnequipped();

            tempPlayer.Player.clothing.askWearVest(0, 0, new byte[0], true);
            removeUnequipped();
        }

        public void healPlayer(UnturnedPlayer tempPlayer)
        {
            tempPlayer.Heal(100, true, true);
            tempPlayer.Infection = 0;
            tempPlayer.Thirst = 0;
            tempPlayer.Hunger = 0;

        }

        public int getWins(UnturnedPlayer tempPlayer)
        {
            int count = 0;
            int i = 0;
            string identifier = tempPlayer.CSteamID + " ";
            //Count
            while ((i = Events.winners.IndexOf(identifier, i)) != -1)
            {
                i += identifier.Length;
                count++;
            }

            //Remove them
            new List<string> { identifier }.ForEach(m => Events.winners = Events.winners.Replace(m, ""));

            return count;
        }
           
        public void leaveEvent(UnturnedPlayer tempPlayer)
        {
            var tempCharacterInfoDuplicate = joinedPlayers.FirstOrDefault(item => item.getSteamID() == tempPlayer.CSteamID.ToString());

            if (tempCharacterInfoDuplicate != null)
            {
                var itemToRemove = joinedPlayers.Single(r => r.getSteamID() == tempPlayer.CSteamID.ToString());
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(itemToRemove.getSteamID())));
                Rocket.Core.R.Permissions.RemovePlayerFromGroup("EventGroup", player);
                Rocket.Core.R.Permissions.AddPlayerToGroup("Guest", player);

      //          tempPlayer.GodMode = false;

        //        if (started)
         //       {
                    //Removed to create delay
                    //   player.Damage(255, new Vector3(0, 0, 0), EDeathCause.PUNCH, ELimb.SKULL, player.CSteamID);
                    //       Events.playersToKillOnConnect.Add(player);
                    clearInventory(player);
                    player.Teleport(new Vector3((float)-312.1, (float)72.4, (float)80.0), 0);
                    player.GodMode = false;
     //           }

                //            Logger.Log("Leave Command 1");

                //           Logger.Log("Leave Command 2");

                //removeFromList(player);
                //             Logger.Log("Leave Command 3");
                //             tempPlayer.GodMode = false;
                player.GodMode = false;
                joinedPlayers.Remove(itemToRemove);
                UnturnedChat.Say(tempPlayer, Events.Instance.Translate("events_leave_game"), Events.Instance.Configuration.Instance.SuccessColor);
               
            }
            else
            {
    //            Logger.Log("Leave Command 4");
                UnturnedChat.Say(tempPlayer, Events.Instance.Translate("events_already_not_in_queue"), Events.Instance.Configuration.Instance.ErrorColor);
                
            }
        }
    }
    
}
