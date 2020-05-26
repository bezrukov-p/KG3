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
                if (vertexShader.Name != null)
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

        public class Camera
        {
            public Vector3 Position { get; set; }
            public Vector3 View { get; set; }
            public Vector3 Side { get; set; }
            public Vector3 Up { get; set; }

            public void Move(Vector3 v)
            {
                Position += v;
            }

            public void MoveSide(float l)
            {
                Vector3 v = new Vector3();
                v.X = Side.X * l;
                v.Y = Side.Y * l;
                v.Z = Side.Z * l;

                Position += v;
            }

            public void MoveUp(float l)
            {
                Vector3 v = new Vector3();
                v.X = Up.X * l;
                v.Y = Up.Y * l;
                v.Z = Up.Z * l;

                Position += v;
            }

            public void MoveBack(float l)
            {
                Vector3 v = new Vector3();
                v.X = View.X * l;
                v.Y = View.Y * l;
                v.Z = View.Z * l;

                Position += v;
            }

            public void Rotate(Vector3 r)
            {
                View = RotateX(View, r.X);
                Up = RotateX(Up, r.X);
                Side = RotateX(Side, r.X);

                View = RotateY(View, r.Y);
                Up = RotateY(Up, r.Y);
                Side = RotateY(Side, r.Y);

                View = RotateZ(View, r.Z);
                Up = RotateZ(Up, r.Z);
                Side = RotateZ(Side, r.Z);
            }

            Vector3 RotateX(Vector3 orig,float a)
            {
                Vector3 res = new Vector3();
                res.X = orig.X;
                res.Y = (float)(Math.Cos(a) * orig.Y - Math.Sin(a) * orig.Z);
                res.Z = (float)(Math.Sin(a) * orig.Y + Math.Cos(a) * orig.Z);

                return res;
            }

            Vector3 RotateY(Vector3 orig, float a)
            {
                Vector3 res = new Vector3();
                res.X = (float)(Math.Cos(a)* orig.X + Math.Sin(a)*orig.Z);
                res.Y = orig.Y;
                res.Z = (float)(-Math.Sin(a) * orig.X + Math.Cos(a) * orig.Z);

                return res;
            }


            Vector3 RotateZ(Vector3 orig, float a)
            {
                Vector3 res = new Vector3();
                res.X = (float)(Math.Cos(a) * orig.X - Math.Sin(a) * orig.Y);
                res.Y = (float)(Math.Sin(a) * orig.X + Math.Cos(a) * orig.Y);
                res.Z = orig.Z;

                return res;
            }

            float Delta(float a1, float a2)
            {
                return (float)(a1 * a2 - Math.Sqrt(1 - a1 * a1) * Math.Sqrt(1 - a2 * a2) - a1);
            }
        }

        public Camera CurrentCamera { get; private set; }
        GLSetter Parameters;

        public View()
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.DepthTest);

            CurrentCamera = new Camera()
            {
                Position = new Vector3(0f, 0f, -8.0f),
                View = new Vector3(0f,0f,1f),
                Up = new Vector3(0f,1f,0f),
                Side = new Vector3(1f,0f,0f)
            };
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


                Parameters = new GLSetter(BasicProgramId);
                GL.LinkProgram(BasicProgramId);
                GL.GetProgram(BasicProgramId, GetProgramParameterName.LinkStatus, out int status);
                Console.WriteLine(status);
                Console.WriteLine(GL.GetProgramInfoLog(BasicProgramId));
            }
        }

        public int W;
        public int H;
        public void Draw(int w, int h)
        {
            W = w;
            H = h;

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
            FeedParameters();
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, v.Length);

        }

        void FeedParameters()
        {
            Parameters["cameraPos"] = CurrentCamera.Position;
            Parameters["cameraView"] = CurrentCamera.View;
            Parameters["cameraUp"] = CurrentCamera.Up;
            Parameters["cameraSide"] = CurrentCamera.Side;

            Parameters["cameraScale"] = new Vector2(1*(float)((double)W / H), 1f);
        }

        class GLSetter
        {
            int BasicProgramId;
            public GLSetter(int id) { BasicProgramId = id; }

            public object this[string str]
            {
                set
                {
                    var obj = value;
                    if(obj is float)
                    {
                        int param;
                        param = GL.GetUniformLocation(BasicProgramId, str);
                        GL.Uniform1(param, (double)obj);
                        return;
                    }
                    if(obj is Vector2)
                    {
                        int param;
                        param = GL.GetUniformLocation(BasicProgramId, str);
                        GL.Uniform2(param, (Vector2)obj);
                        return;
                    }
                    if (obj is Vector3)
                    {
                        int param;
                        param = GL.GetUniformLocation(BasicProgramId, str);
                        GL.Uniform3(param, (Vector3)obj);
                        return;
                    }

                    throw new Exception("What is it?");
                }
            }
        }
    }
}
