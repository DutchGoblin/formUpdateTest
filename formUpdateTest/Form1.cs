namespace formUpdateTest
{
    public partial class Form1 : Form
    {

        public static string CurrentVersion = "1.0.0";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = $"Version: {CurrentVersion}";

        }
    }
}
