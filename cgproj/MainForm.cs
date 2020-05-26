using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cgproj
{
    public partial class MainForm : Form
    {
        OpenTK.GLControl glControl;
        View view;
        public MainForm()
        {
            glControl = new OpenTK.GLControl();

            InitializeComponent();

            glControl.Location = new Point(0, 0);
            glControl.Size = new Size(this.Width, this.Height);

            glControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;

            glControl.Paint += GLControl_Paint;
            this.Controls.Add(glControl);           

            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            view = new View();           

            view.CurrentShaders = new View.Shaders()
            {
                VertexShader = @"../../../shaders/raytracing.vert",
                FragmentShader = @"../../../shaders/raytracing.frag"
            };

            glControl.Invalidate();
        }

        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            view.Draw(this.Width, this.Height);
            glControl.SwapBuffers();
        }
    }
}
