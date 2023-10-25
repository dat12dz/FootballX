
using System;
using System.Collections.Generic;


public enum TeamEnum
{
    Red,
    Blue
}
[Serializable]
public class TeamClass
{
    public static TeamEnum GetTeamFromSlot(int slot)
    {
        if (slot >= 0 && slot < 5)
            return TeamEnum.Red;
        if (slot >= 5 && slot < 10)          
         return TeamEnum.Blue;

        return 0;
    }
    public TeamClass(Room room,TeamEnum team_ = 0)
    {
        GameRoom = room;
        team = team_;
    }
    public TeamClass(Room room, int Slot = 0)
    {
        GameRoom = room;
        GetTeamFromSlot(Slot);
    }
   public Room GameRoom;
   public TeamEnum team;
   public TeamClass GetOpponentTeam()
    {
        if (team == TeamEnum.Red)
        {
            return new TeamClass(GameRoom,TeamEnum.Blue);

        }
        else
        {
          return  new TeamClass(GameRoom,TeamEnum.Red);
        }
    }
    public List<Player> GetAllPlayerInThisTeam()
    {
        List<Player> playerList = new List<Player>();
        if (team == TeamEnum.Red)
        {
            for (byte i = 0;i < 5;i++)
            {
                PlayerRoomManager TempPlayerRoom = null;
                try
                {
                    TempPlayerRoom = GameRoom.playerDict[i]; 
                }
                catch
                {

                }             
                if (TempPlayerRoom != null)
                    playerList.Add(TempPlayerRoom.thisPlayer);
            }
        }
        else
        {
            for (byte i = 5; i < 10; i++)
            {
                PlayerRoomManager TempPlayerRoom = null;
                try
                {
                    TempPlayerRoom = GameRoom.playerDict[i];
                }
                catch
                {

                }
                if (TempPlayerRoom != null)
                    playerList.Add(TempPlayerRoom.thisPlayer);
            }
        }
        return playerList;
    }
    public Player GetRandomPlayer()
    {
        var PlayerList  = GetAllPlayerInThisTeam();
        if (PlayerList.Count > 0)
        return PlayerList[UnityEngine.Random.Range(0, PlayerList.Count)];
        else return null;
    }
}
