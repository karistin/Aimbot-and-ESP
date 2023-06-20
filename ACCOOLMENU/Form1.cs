using System.Threading;

namespace ACCOOLMENU
{
    public partial class Form1 : Form
    {

        methods? m;
        Entity localPlayer = new Entity();
        List<Entity> entities = new List<Entity>(); 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            m = new methods();

            localPlayer = m.ReadLocalPlayer();
            entities = m.ReadEntites(localPlayer);


            int i = 0;
        }
    }
}