using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace datathegenius.eventsmanager
{
    public class EventsConfiguration : IRocketPluginConfiguration
    {

        public bool Enabled = true;
        public Color ErrorColor = Color.red;
        public Color SuccessColor = Color.green;
        public Color AnnouncementColor = Color.cyan;
        public int totalPlayersInServer = 24;
        public ulong WalkDistanceDied = 100;
        public ulong WalkDistanceDiedAlive = 300;
        public uint expAmount = 1500;
        public int announcementSeconds = 30;

        public void LoadDefaults()
        {
            Enabled = true;
            ErrorColor = UnityEngine.Color.red;
            SuccessColor = Color.green;
            AnnouncementColor = Color.cyan;
            expAmount = 1500;
            totalPlayersInServer = 24;
            announcementSeconds = 30;
        }
    }
}
