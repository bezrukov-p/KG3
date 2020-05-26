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

            ConsolePanel.Visible = false;

            glControl.Location = new Point(0, 0);
            glControl.Size = new Size(this.Width, this.Height);

            glControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;

            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseWheel += GlControl_MouseWheel;

            glControl.Paint += GLControl_Paint;
            this.Controls.Add(glControl);           

            this.Load += MainForm_Load;

            glControl.KeyDown += MainForm_KeyDown;
            ConsoleText.KeyDown += ConsoleText_KeyDown;
        }

        private void ConsoleText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ConsoleView.Text += "\n" + ConsoleText.Text;
                ConsoleText.Text = "";                
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                ConsolePanel.Visible = !ConsolePanel.Visible;

                if(ConsolePanel.Visible)
                {
                    ConsoleText.Focus();
                }
            }
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            view.CurrentCamera.MoveBack(2.0f * e.Delta / 100.0f);
            glControl.Invalidate();
        }

        bool mousePressed = false;

        int lx;
        int ly;

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if(mousePressed && e.Button == MouseButtons.Middle)
            {
                view.CurrentCamera.MoveSide(2.0f * ( -e.X+ lx) / 100.0f);
                view.CurrentCamera.MoveUp(2.0f * (e.Y- ly) / 100.0f);

                lx = e.X;
                ly = e.Y; 

                glControl.Invalidate();
            }
            if (mousePressed && e.Button == MouseButtons.Left)
            {
                view.CurrentCamera.Rotate(new OpenTK.Vector3(-1.0f * (e.Y - ly) / 100, -1.0f * (e.X - lx) / 100, 0f));

                lx = e.X;
                ly = e.Y;


                glControl.Invalidate();
            }

        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left)
            {
                mousePressed = false;
            }
        }

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            ConsolePanel.Visible = false;
            if(e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left)
            {
                mousePressed = true;

                lx = e.X;
                ly = e.Y;
            }
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
