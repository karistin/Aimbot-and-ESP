using System;
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
            for (int i = 0; i < 4; i++)
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
            x = (float)(Math.Atan2(deletaY, deletaX) * 180 / Math.PI) + 90;

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

        public Point WordToScreen(viewMatrix mtx, Vector3 pos, int width, int height)
        {
            var twoD = new Point();

            float screenW = (mtx.m14 * pos.X) + (mtx.m24 * pos.Y) + (mtx.m34 * pos.Z) + mtx.m44;

            if (screenW > 0.001f)
            {
                float screenX = (mtx.m11 * pos.X) + (mtx.m21 * pos.Y) + (mtx.m31 * pos.Z) + mtx.m41;
                float screenY = (mtx.m12 * pos.X) + (mtx.m22 * pos.Y) + (mtx.m32 * pos.Z) + mtx.m42;

                float camX = width / 2f;
                float camY = height / 2f;

                float X = camX + (camX*screenX / screenW);
                
                float Y = camY + (camY*screenY / screenW);

                twoD.X = (int) X;
                twoD.Y = (int) Y;

                return twoD;

            }
            else
            {
                return new Point(-99, -99);
            }
        }
        public Rectangle CalcReact(Point feet, Point head)
        {
            var rect = new Rectangle();
            rect.X = head.X - 100;
            rect.Y = head.Y;

            rect.Width = (100);
            rect.Height = (head.Y - feet.Y); 

            return rect;
        }
        public viewMatrix ReadMatrix()
        {
            var viewMatrix = new viewMatrix();
            var mtx = mem.ReadMatrix(moudleBase + Offsets.iViewMatrix);

            viewMatrix.m11 = mtx[0];
            viewMatrix.m12 = mtx[1];
            viewMatrix.m13 = mtx[2];
            viewMatrix.m14 = mtx[3];

            viewMatrix.m21 = mtx[4];
            viewMatrix.m22 = mtx[5];
            viewMatrix.m23 = mtx[6];
            viewMatrix.m24 = mtx[7];

            viewMatrix.m31 = mtx[8];
            viewMatrix.m32 = mtx[9];
            viewMatrix.m33 = mtx[10];
            viewMatrix.m34 = mtx[11];

            viewMatrix.m41 = mtx[12];
            viewMatrix.m42 = mtx[13];
            viewMatrix.m43 = mtx[14];
            viewMatrix.m44 = mtx[15];

            return viewMatrix;
        }   
        public methods()
        {
            mem = new Swed("ac_client");
            moudleBase = mem.GetModuleBase(".exe");
        }
    }


}
