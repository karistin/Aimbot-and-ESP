using ezOverLay;
using System.Runtime.InteropServices;
using System.Threading;

namespace ACCOOLMENU
{
    public partial class Form1 : Form
    {
        // �׸� ��������
        ez ez = new ez();
        // �ʿ� �Լ��� ���� 
        methods? m;
        // �ڽ�
        Entity localPlayer = new Entity();
        // ��� �÷��̾� 
        List<Entity> entities = new List<Entity>();

        // Ű���� �Է� �ޱ� 
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
                // �׷��� �������� ���� 
                ez.SetInvi(this);
                ez.DoStuff("AssaultCube", this);

                // ������ ��׶��� 
                Thread thread = new Thread(Main) { IsBackground = true };
                thread.Start();
            }

        }

        void Main()
        {
            // �� ������ ���� 
            while (true)
            {
                // �ڱ� �ڽ� �� EntityList �ʱ�ȭ 
                localPlayer = m.ReadLocalPlayer();
                entities = m.ReadEntites(localPlayer);

                // ��ġ�� ���� 
                entities = entities.OrderBy(o => o.mag).ToList();

                // ���콺 5�� Ŭ���� 
                if (GetAsyncKeyState(Keys.XButton2) < 0)
                {
                    if (entities.Count > 0)
                    {

                        foreach (var ent in entities)
                        {
                            // �Ʊ� ���� �ƴϿ�����
                            if (ent.team != localPlayer.team)
                            {
                                var angles = m.CalcAngles(localPlayer, ent);
                                m.Aim(localPlayer, angles.X, angles.Y);
                                // �� ��� �� ���� ���� 
                                break;
                            }
                        }
                    }
                }

                // ȭ�� refresh�� ���� ���� �ʱ�ȭ �� �׸���(paint event)
                Form1 f = this;
                f.Refresh();

                Thread.Sleep(20);
            }
        }

        // paint �̺�Ʈ ���� 
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen red = new Pen(Color.Red, 3);
            // ���� ��
            Pen green = new Pen(Color.Green, 3);
            // �Ʊ� �� 
            foreach (var ent in entities.ToList())
            {
                // �ٸ��� �Ӹ��� ������ �������� 
                var wtsFeet = m.WordToScreen(m.ReadMatrix(), ent.feet, this.Width, this.Height);
                var wtsHead= m.WordToScreen(m.ReadMatrix(), ent.head, this.Width, this.Height);


                if (wtsFeet.X > 0 )
                {
                    if (localPlayer.team == ent.team)
                    {
                        // �Ʊ��� �ʷϻ� �߿� �׸��� 
                        g.DrawLine(green, new Point(Width / 2, Height), wtsFeet);
                        //g.DrawRectangle(green, m.CalcReact(wtsFeet, wtsHead));
                    }
                    else
                    {
                        // ������ ���� �߿� �׸��� 
                        g.DrawLine(red, new Point(Width / 2, Height), wtsFeet);
                        //g.DrawRectangle(red, m.CalcReact(wtsFeet, wtsHead));
                    }
                }
            }
        }
    }
}