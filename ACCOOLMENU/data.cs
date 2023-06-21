using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ACCOOLMENU
{
    // playerObject 저장 객체
    public class Entity
    {
        public IntPtr baseAddress;
        public Vector3 feet, head;
        // 카메라 
        public Vector2 viewAngles;
        public float mag, viewOffset;
        public int health, team, currentAmmo, dead;
        public string name;
    }
    // ViewMatrix

    public class viewMatrix
    {
        public float m11, m12, m13, m14;
        public float m21, m22, m23, m24;
        public float m31, m32, m33, m34;
        public float m41, m42, m43, m44;
    }
}
