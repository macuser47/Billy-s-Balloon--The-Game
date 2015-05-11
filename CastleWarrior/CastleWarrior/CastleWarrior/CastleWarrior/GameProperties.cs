using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CastleWarrior
{
    public class GameProperties
    {
        //This file constins all static game properties sepcific to this game to be used by the tiling framework

        //set this value to the width and height of the tiles
        public static int TILESIZE = 24;

        //This string sets the game type. Set it equal to "RPG" or "platformer/sidescroller"
        public static string GAMETYPE = "platformer/sidescroller";

        //set how fast the player moves in pixels per second
        public static int PLAYERMOVEVELOCITY = 315;

        //set whether the player gradually speeds up and slows down or just stop and starts moving upon key press and release
        public static bool GRADUALMOVE = false;

    }
}
