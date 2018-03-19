using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace datathegenius.eventsmanager
{

    public class PlayerData
    {
        int wins = 0;
        Boolean revived = true;
        Boolean dead = false;
        string name;
        string steamID;
        string groupID;
        Vector3 location;

        //         PlayerData()
        //      {
        //        name = "";
        //      steamID.Set(0,0,0);
        //    location.Set(0, 0, 0);
        // }   

        public PlayerData(string playerName, string playerID, string playerGroup, Vector3 playerLocation)
        {
            name = playerName;
            steamID = playerID;
            groupID = playerGroup;
            location = playerLocation;
            dead = false;
            wins = 0;
        }

        public Boolean getRevived()
        {
            return revived;
        }

        public void setRevived(Boolean status)
        {
            revived = status;
        }

        public void increaseWins()
        {
            wins++;
        }

        public void setWins(int tempWins)
        {
            wins = tempWins;
        }

        public int getWins()
        {
            return wins;
        }

        public string getName()
        {
            return name;
        }

        public string getSteamID()
        {
            return steamID;
        }

        public string getGroupID()
        {
            return groupID;
        }

        public Vector3 getLocation()
        {
            return location;
        }

        public Boolean getDead()
        {
            return dead;
        }

        public void setDead(Boolean died)
        {
            dead = died;
        }
    }
}
