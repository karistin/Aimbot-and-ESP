using ezOverLay;
using System.Runtime.InteropServices;
using System.Threading;

namespace ACCOOLMENU
{
    public partial class Form1 : Form
    {
        // 그림 오버레이
        ez ez = new ez();
        // 필요 함수들 모음 
        methods? m;
        // 자신
        Entity localPlayer = new Entity();
        // 모든 플레이어 
        List<Entity> entities = new List<Entity>();

        // 키보드 입력 받기 
        // user32.dll
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
                // 그래픽 오버레이 설정 
                ez.SetInvi(this);
                ez.DoStuff("AssaultCube", this);

                // 쓰래드 백그라운드 
                Thread thread = new Thread(Main) { IsBackground = true };
                thread.Start();
            }

        }

        void Main()
        {
            // 매 프레임 마다 
            while (true)
            {
                // 자기 자신 및 EntityList 초기화 
                localPlayer = m.ReadLocalPlayer();
                entities = m.ReadEntites(localPlayer);

                // 위치순 정렬 
                entities = entities.OrderBy(o => o.mag).ToList();

                // 마우스 5번 클릭시 
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
                                // 각 계산 및 에이 조준 
                                break;
                            }
                        }
                    }
                }

                // 화면 refresh를 통해 버퍼 초기화 및 그리기(paint event)
                Form1 f = this;
                f.Refresh();

                Thread.Sleep(20);
            }
        }

        // paint 이벤트 설정 
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen red = new Pen(Color.Red, 3);
            // 적군 색
            Pen green = new Pen(Color.Green, 3);
            // 아군 색 
            foreach (var ent in entities.ToList())
            {
                // 다리와 머리의 데이터 가져오기 
                var wtsFeet = m.WordToScreen(m.ReadMatrix(), ent.feet, this.Width, this.Height);
                var wtsHead= m.WordToScreen(m.ReadMatrix(), ent.head, this.Width, this.Height);


                if (wtsFeet.X > 0 )
                {
                    if (localPlayer.team == ent.team)
                    {
                        // 아군시 초록색 발에 그리기 
                        g.DrawLine(green, new Point(Width / 2, Height), wtsFeet);
                        //g.DrawRectangle(green, m.CalcReact(wtsFeet, wtsHead));
                    }
                    else
                    {
                        // 적군시 적색 발에 그리기 
                        g.DrawLine(red, new Point(Width / 2, Height), wtsFeet);
                        //g.DrawRectangle(red, m.CalcReact(wtsFeet, wtsHead));
                    }
                }
            }
        }
    }
}