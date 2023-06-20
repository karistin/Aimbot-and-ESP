﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Swed32;

namespace ACCOOLMENU
{
    public class methods
    {
        public Swed mem;
        public IntPtr moudleBase;
        public Entity ReadLocalPlayer()
        {
            var localPlayer = ReadEntity(mem.ReadPointer(moudleBase, Offsets.iLocalPlayer));
            localPlayer.viewAngles.X = mem.ReadFloat(localPlayer.baseAddress, Offsets.vAngles);
            localPlayer.viewAngles.Y = mem.ReadFloat(localPlayer.baseAddress, Offsets.vAngles + 0x4);

            return localPlayer;
        }

        public Entity ReadEntity(IntPtr entBase)
        {
            var ent = new Entity();
            ent.baseAddress = entBase;

            ent.currentAmmo = mem.ReadInt(ent.baseAddress, Offsets.iCurrentAmmo);
            ent.health = mem.ReadInt(ent.baseAddress, Offsets.iHealth);
            ent.team = mem.ReadInt(ent.baseAddress, Offsets.iTeam);

            ent.feet = mem.ReadVec(entBase, Offsets.vFeet);
            ent.head = mem.ReadVec(entBase, Offsets.vHead);

            ent.name = Encoding.UTF8.GetString(mem.ReadBytes(ent.baseAddress, Offsets.sName, 11));
            
            return ent;
        }

        public List<Entity> ReadEntites(Entity localPlayer)
        {
            var entities = new List<Entity>();
            var entityList = mem.ReadPointer(moudleBase, Offsets.iEntityList);
            // 4 bot 
            for(int i = 0; i < 4; i ++)
            {
                var currentEntBase = mem.ReadPointer(entityList, i * 0x4);
                var ent = ReadEntity(currentEntBase);
                ent.mag = CalcMag(localPlayer, ent);

                // health ? 0
                if (ent.health > 0 && ent.health < 101)
                    entities.Add(ent);
            }

            return entities;
        }
        // 각 계산
        public Vector2 CalcAngles(Entity localPlayer, Entity destEnt)
        {
            float x, y;
            var deletaX = destEnt.head.X - localPlayer.head.X;
            var deletaY = destEnt.head.Y - localPlayer.head.Y; 

            // 역탄젠트     
            x = (float) (Math.Atan2(deletaY, deletaX) * 180 / Math.PI) + 90;

            float deletaZ = destEnt.head.Z - localPlayer.head.Z;
            float dist = CalcDist(localPlayer, destEnt);

            y = (float)(Math.Atan2(deletaZ, dist) * 180 / Math.PI);

            return new Vector2(x, y);
        }

        public void Aim(Entity ent, float x, float y)
        {
            mem.WriteFloat(ent.baseAddress, Offsets.vAngles, x);
            mem.WriteFloat(ent.baseAddress, Offsets.vAngles + 0x4, y);
        }

        public static float CalcDist(Entity localPlayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.feet.X - localPlayer.feet.X, 2)
                + Math.Pow(destEnt.feet.Y - localPlayer.feet.Y, 2));
        }
        public static float CalcMag(Entity localPlayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.feet.X - localPlayer.feet.X, 2)
                + Math.Pow(destEnt.feet.Y - localPlayer.feet.Y, 2)
                + Math.Pow(destEnt.feet.Z - localPlayer.feet.Z, 2));
        }

        public methods()
        {
            mem = new Swed("ac_client");
            moudleBase = mem.GetModuleBase(".exe");
        }
    }


}
