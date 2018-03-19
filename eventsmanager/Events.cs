using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace datathegenius.eventsmanager
{
    //If ever remove command, change Guest group (which was default), back to default... The ID.
    
    public class Events : RocketPlugin<EventsConfiguration>
    {
        public static Events Instance;
        public static List<UnturnedPlayer> deadPlayerForTransport = new List<UnturnedPlayer>() { };
        public static string winners;

        public static List<UnturnedPlayer> playersToKillOnConnect = new List<UnturnedPlayer>() { };
        public UnturnedPlayer tempClassPlayer;
        //       var fs = File.Open("file.name", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        public static List<PlayerData> leftDuringEvent = new List<PlayerData>() { };

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    {"events_on", "Paintball system is firing up!" },
                    {"events_off", "Paintball system is winding down!" },
                    {"events_invalid_parameter", "Sorry, it looks like you didn't use the command correctly." },
                    {"events_joined_game", "You have queued into the event!" },
                    {"events_already_joined_game", "{0} has already joined the game!" },
                    {"events_error", "Something just went wrong!" },
                    {"events_not_active", "There are no active events!" },
                    {"events_already_active", "Events are already active!" },
                    {"events_already_notactive", "Events are already not running!" },
                    {"events_join_announcement", "Join paintball! Do '/paintball storage' to get lockers, and '/paintball join' to join." },
                    {"events_ended_announcement", "The events have ended! Do /paintball reward to claim your reward, if you won." },
                    {"events_already_not_in_queue", "You aren't in an event!"},
                    {"events_leave_game", "You have left the game!" },
                    {"events_position_set", "{0} set at {1}" },
                    {"events_reward_given", "You have been given your reward!" },
                    {"events_no_reward", "Sorry, you got no reward." },
                    {"events_started", "Let the games begin!" },
                    {"events_start_only_one", "Only one person is in the queue!" },
                    {"events_group_error", "One member in your group has already joined, please leave your group." },
                    {"events_player_list", "Joined Players ({0}): {1}" },
                    {"events_started_already", "The event has already been started." },
                    {"events_stopped_already", "The event has already been stopped. Try turning off events." },
                    {"events_stopped", "The event has been stopped. Standby." },
                    {"events_error_not_finished_reward", "Wait until the event is over to claim your reward." },
                    {"events_left_active_event", "Last time you logged off, it was during an active event you had joined." },
                    {"events_winner_announcement", "The winner was: {0}" },
                    {"events_one_player_only", "Only one player has joined the event! Need more than 1." },
                    {"events_storage_given", "You've been given 4 lockers." },
                    {"events_storage_used", "You've already used your storage request." },
                    {"events_no_permission", "Sorry, you don't have the required permissions to use this." }
                };
            }
        }
        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            try
            {
                

                var tempCharacterInfoDuplicate = CommandEvent.joinedPlayers.FirstOrDefault(item => item.getSteamID() == player.CSteamID.ToString());

                if (tempCharacterInfoDuplicate != null && CommandEvent.started)
                {
                    CommandEvent.activePlayers--;

                    //              player.Damage(255, new Vector3(0, 0, 0), EDeathCause.PUNCH, ELimb.SKULL, player.CSteamID);
                    //               player.Suicide();
                    leftDuringEvent.Add(tempCharacterInfoDuplicate);
                    Rocket.Core.R.Permissions.RemovePlayerFromGroup("EventGroup", player);
                    Rocket.Core.R.Permissions.AddPlayerToGroup("Guest", player);

                    CommandEvent.joinedPlayers.Remove(tempCharacterInfoDuplicate);

                    if (CommandEvent.eventActive)
                    {
                        dealWithWinEvent();
                    }
                    //   CommandEvent.joinedPlayers.Remove(player);
                    UnturnedChat.Say(player, Events.Instance.Translate("events_leave_game"), Events.Instance.Configuration.Instance.SuccessColor);
                    return;
                }
                }catch(NullReferenceException)
                {
                Logger.LogError("Player died while being null.");
                }            
        } 

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            player.GodMode = false;
            Rocket.Core.R.Permissions.RemovePlayerFromGroup("EventGroup", player);
            Rocket.Core.R.Permissions.AddPlayerToGroup("Guest", player);

            var tempCharacterLeftDuringEvent = leftDuringEvent.FirstOrDefault(item => item.getSteamID() == player.CSteamID.ToString());
            leftDuringEvent.Remove(tempCharacterLeftDuringEvent);

            if (tempCharacterLeftDuringEvent != null)
            {
                CommandEvent myEvent = new CommandEvent();
                myEvent.clearInventory(player);
                tempClassPlayer = player;
                playersToKillOnConnect.Add(player);
//                           player.Suicide();
              //  player.Teleport("Seattle");
       //         player.Damage(255, new Vector3(0, 0, 0), EDeathCause.PUNCH, ELimb.SKULL, player.CSteamID);
                UnturnedChat.Say(player, Events.Instance.Translate("events_left_active_event"), Events.Instance.Configuration.Instance.ErrorColor);
            }
        }

        //On player revive, check if active, event active, then move them. 
            
        public void OnPlayerRevive(UnturnedPlayer player, Vector3 position, byte angle)
        {
   //         Logger.Log(player.CharacterName + " has died.");
            if(CommandEvent.started)
            {
                var tempCharacterInfoDuplicate = CommandEvent.joinedPlayers.FirstOrDefault(item => item.getSteamID() == player.CSteamID.ToString());
                if (CommandEvent.started && tempCharacterInfoDuplicate != null)
                {
                    //            tempCharacterInfoDuplicate.setDead(true);
                    deadPlayerForTransport.Add(player);
                    tempCharacterInfoDuplicate.setRevived(true);
                    return;
                }
   //             Logger.Log(player.CharacterName + " has died with " + tempCharacterInfoDuplicate.getDead());
            }

    //        if(CommandEvent.started)
   //         {

      //      }

        }

        public int AliveCount()
        {
            int deathCount = 0;

            for (int x = 0; x < CommandEvent.joinedPlayers.Count; x++)
            {
                if (CommandEvent.joinedPlayers[x].getDead())
                    deathCount++;
            }

            return (CommandEvent.joinedPlayers.Count - deathCount);
        }

        public void OnPlayerDead(UnturnedPlayer player, Vector3 position)
        {

            if (CommandEvent.eventActive)
            {
                

                var tempCharacterInfoDuplicate = CommandEvent.joinedPlayers.FirstOrDefault(item => item.getSteamID() == player.CSteamID.ToString());
                if (CommandEvent.eventActive && tempCharacterInfoDuplicate != null)
                {
                    UnturnedPlayer playerDead = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(tempCharacterInfoDuplicate.getSteamID())));
                    CommandEvent myEvent = new CommandEvent();
                    myEvent.clearInventory(playerDead);
                    tempCharacterInfoDuplicate.setRevived(false);
                    tempCharacterInfoDuplicate.setDead(true);
                    dealWithWinEvent();
                    player.GodMode = true;
                }
                    //       int deadCount = 0;
                    //See if 1 player is left
                    //             for (int x = 0; x < CommandEvent.joinedPlayers.Count(); x++)
                    //               {
                    //       var playerThatDied = CommandEvent.joinedPlayers.FirstOrDefault(item => item.getSteamID() == player.CSteamID.ToString());
                    //      if (playerThatDied != null)
                    //         CommandEvent.activePlayers--;
                    //                       deadCount++;


            }
            //       Logger.Log("Player Joined Count: " + CommandEvent.joinedPlayers.Count());
            //       Logger.Log("Player Dead Count: " + deadCount);

            }

        //Keep Track of Time... Kill player
        DateTime lastCalled = DateTime.Now;
        private void delayKillPlayer()
        {
            if ((DateTime.Now - this.lastCalled).TotalSeconds > .5)
            {
                lastCalled = DateTime.Now;

                if (playersToKillOnConnect.Count == 1 && playersToKillOnConnect[0] != null)
                {
                    //               playersToKillOnConnect[0].GodMode = false;
                    //              playersToKillOnConnect[0].Suicide();
                    CommandEvent myEvent = new CommandEvent();
                    myEvent.clearInventory(playersToKillOnConnect[0]);
                    UnturnedPlayer playerMove = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(playersToKillOnConnect[0].CSteamID.ToString())));
                    playersToKillOnConnect[0].Teleport(new Vector3((float)-312.1, (float)72.4, (float)80.0), 0);
                    playersToKillOnConnect.Remove(playersToKillOnConnect[0]);
                }            

            }
        }

        public void dealWithWinEvent()
        {
            //Count and deal with alive people.
            int tempTotalAlive = AliveCount();
                         Logger.Log("Alive count: " + tempTotalAlive);

            if (tempTotalAlive == 1)
            {
                var winningPlayer = CommandEvent.joinedPlayers.FirstOrDefault(item => item.getDead() == false);
                winners += winningPlayer.getSteamID() + " ";
                winningPlayer.increaseWins();

                UnturnedPlayer playerWinner = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(winningPlayer.getSteamID())));
                CommandEvent myEvent = new CommandEvent();
                myEvent.clearInventory(playerWinner);
                playerWinner.Damage(255, new Vector3(0, 0, 0), EDeathCause.PUNCH, ELimb.SKULL, playerWinner.CSteamID);
                //                playerWinner.Teleport(CommandEvent.position2, 0);
                //                playerWinner.GodMode = true;
                //                winningPlayer.setDead(true);
                UnturnedChat.Say(Events.Instance.Translate("events_winner_announcement", winningPlayer.getName()), Events.Instance.Configuration.Instance.AnnouncementColor);
                //                    CommandEvent.eventActive = false;
            }
        }

        void FixedUpdate()
        {
            delayKillPlayer();

            announcementManager();
        //    if(CommandEvent.started)
        //           healAllDead();

            //           if (CommandEvent.eventActive)
            //          {


            //        }
            //            for(int x = 0; x < deadPlayerForTransport.Count(); x++)
            //          {
            if (CommandEvent.started && deadPlayerForTransport.Count == 1 && deadPlayerForTransport[0] != null)
            {
                deadPlayerForTransport[0].Teleport(CommandEvent.position2, 0);
   //             deadPlayerForTransport[0].GodMode = true;
                deadPlayerForTransport.Remove(deadPlayerForTransport[0]);
            }

   //             deadPlayerForTransport[x].GodMode = true;
   //         }
//            deadPlayerForTransport = new List<UnturnedPlayer>();
        }
        DateTime lastCalledTimer = DateTime.Now;

        private void announcementManager()
        {
            if (CommandEvent.started && ((DateTime.Now - this.lastCalledTimer).TotalSeconds > Events.Instance.Configuration.Instance.announcementSeconds))
            {
                lastCalledTimer = DateTime.Now;
                UnturnedChat.Say(Events.Instance.Translate("events_join_announcement"), Events.Instance.Configuration.Instance.AnnouncementColor);
            }
        }

        public void healAllDead()
        {
            for (int x = 0; x < CommandEvent.joinedPlayers.Count; x++)
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(CommandEvent.joinedPlayers[x].getSteamID())));
                if (CommandEvent.joinedPlayers[x].getDead())
                {
                    CommandEvent myEvent = new CommandEvent();
                    myEvent.healPlayer(player);
                }
            }
        }

        protected override void Load()
        {
   //         Logger.Log("Event Plugin Started!");
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            U.Events.OnPlayerConnected += OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerRevive += OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerDead += OnPlayerDead;
             
            CommandEvent.joinedPlayers = new List<PlayerData>();
            CommandEvent.eventActive = false;
            CommandEvent.started = false;

            Instance = this;

            foreach (SteamPlayer plr in Provider.Players)
            {
                //So let's convert each SteamPlayer into an UnturnedPlayer
                UnturnedPlayer unturnedPlayer = UnturnedPlayer.FromSteamPlayer(plr);

                unturnedPlayer.GodMode = false;

                //Reset permissions
                Rocket.Core.R.Permissions.RemovePlayerFromGroup("EventGroup", unturnedPlayer);
                Rocket.Core.R.Permissions.AddPlayerToGroup("Guest", unturnedPlayer);
            }
            //            joinedPlayers = new ArrayList();  
            //joinedPlayers = new PlayerData[Configuration.Instance.totalPlayersInServer];

        }

        protected override void Unload()
        {
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            UnturnedPlayerEvents.OnPlayerRevive -= OnPlayerRevive;
            UnturnedPlayerEvents.OnPlayerDead -= OnPlayerDead;

        }
    }
}
