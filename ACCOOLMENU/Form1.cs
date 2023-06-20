using System.Runtime.InteropServices;
using System.Threading;

namespace ACCOOLMENU
{
    public partial class Form1 : Form
    {

        methods? m;
        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>();

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vkey);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            m = new methods();
            if (m != null)
            {
                Thread thread = new Thread(Main) { IsBackground = true };
                thread.Start();
            }
   
        }

        void Main()
        {
            while (true)
            {
                localPlayer = m.ReadLocalPlayer();
                entities = m.ReadEntites(localPlayer);

                entities = entities.OrderBy(o => o.mag).ToList();

                if (GetAsyncKeyState(Keys.XButton2) < 0)
                {
                    if (entities.Count > 0)
                    {
                        
                        foreach (var ent in entities)
                        {
                            // 아군 팀이 아니여야함
                            if (ent.team != localPlayer.team)
                            {
                                var angles = m.CalcAngles(localPlayer, ent);
                                m.Aim(localPlayer, angles.X, angles.Y);
                                break;
                            }
                        }
                    }
                }

                 
                Thread.Sleep(20);
            }
        }
    }
}