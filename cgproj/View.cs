using OpenTK.Graphics.OpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace cgproj
{
    public class View
    {

        public int BasicProgramId { get; private set; }

        public class ShaderBox
        {
            public ShaderBox(ShaderType st)
            {
                Type = st;
            }

            public string Name { get; set; } = null;
            public ShaderType Type { get; }
            public int Address { get; set; }
        }

        public class Shaders
        {
            ShaderBox vertexShader = new ShaderBox(ShaderType.VertexShader);
            public string VertexShader
            {
                get => vertexShader.Name;
                set => vertexShader.Name = value;
            }

            ShaderBox tessellationControlShader = new ShaderBox(ShaderType.TessControlShader);
            public string TessellationControlShader
            {
                get => tessellationControlShader.Name;
                set => tessellationControlShader.Name = value;
            }

            ShaderBox tessellationEvalutionShader = new ShaderBox(ShaderType.TessEvaluationShader);
            public string TessellationEvalutionShader
            {
                get => tessellationEvalutionShader.Name;
                set => tessellationEvalutionShader.Name = value;
            }

            ShaderBox geometryShader = new ShaderBox(ShaderType.GeometryShader);
            public string GeometryShader
            {
                get => geometryShader.Name;
                set => geometryShader.Name = value;
            }

            ShaderBox fragmentShader = new ShaderBox(ShaderType.FragmentShader);
            public string FragmentShader
            {
                get => fragmentShader.Name;
                set => fragmentShader.Name = value;
            }

            public IEnumerable<ShaderBox> GetShaders()
            {
                if(vertexShader.Name != null)
                    yield return vertexShader;

                if (tessellationControlShader.Name != null)
                    yield return tessellationControlShader;

                if (tessellationEvalutionShader.Name != null)
                    yield return tessellationEvalutionShader;

                if (geometryShader.Name != null)
                    yield return geometryShader;

                if (fragmentShader.Name != null)
                    yield return fragmentShader;
            }
        }

        public View()
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.DepthTest);
        }

        public Shaders CurrentShaders
        {
            set
            {
                Shaders sh = value;

                BasicProgramId = GL.CreateProgram();

                foreach(var s in sh.GetShaders())
                {
                    s.Address = GL.CreateShader(s.Type);
                    using (StreamReader sr = new StreamReader(s.Name))
                    {
                        GL.ShaderSource(s.Address, sr.ReadToEnd());
                    }

                    GL.CompileShader(s.Address);
                    GL.AttachShader(BasicProgramId,s.Address);
                }

                GL.LinkProgram(BasicProgramId);
                GL.GetProgram(BasicProgramId, GetProgramParameterName.LinkStatus, out int status);
                Console.WriteLine(status);
                Console.WriteLine(GL.GetProgramInfoLog(BasicProgramId));
            }
        }

        public void Draw(int w, int h)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1);
            GL.Viewport(0, 0, w, h);

            Vector3[] v = new Vector3[] {
                new Vector3(-1, -1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0)
            };

            GL.GenBuffers(1, out int vboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vector3.SizeInBytes * v.Length), v, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(BasicProgramId);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, v.Length);

        }
    }
}
