#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using Jitter.Collision;
using Jitter;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Util;
#endregion

namespace JitterOpenGLDemo
{
    public class Program : DrawStuffOtk, IDisposable
    {
        private World world;
        private bool initFrame = true;

        private const string title = "Jitter OpenGL - Press 'Space' to shoot a sphere, 'R' to Reset";

        public Program() : base(800,600)
        {
            world = new World(new CollisionSystemSAP());
            world.Gravity = new Util.MyMath.Vector3(0, 0, -10);

            dsSetSphereQuality(2);

            this.VSync = OpenTK.VSyncMode.Off;
            this.Title = title;

            Keyboard.KeyDown += new EventHandler<OpenTK.Input.KeyboardKeyEventArgs>(Keyboard_KeyDown);

            BuildScene();
        }

        void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.Space) ShootSphere();
            if (e.Key == OpenTK.Input.Key.R) BuildScene();
            if (e.Key == OpenTK.Input.Key.W) MyMove(Util.MyMath.Vector3.forward);
            if (e.Key == OpenTK.Input.Key.S) MyMove(Util.MyMath.Vector3.back);
            if (e.Key == OpenTK.Input.Key.A) MyMove(Util.MyMath.Vector3.left);
            if (e.Key == OpenTK.Input.Key.D) MyMove(Util.MyMath.Vector3.right);
        }

        private void MyMove(Util.MyMath.Vector3 vec)
        {
            m_LastSphere.LinearVelocity = vec * 50;
        }

        private void BuildScene()
        {
            world.Clear();

            RigidBody body = AddBox(new Util.MyMath.Vector3(0, 0, -0.5f), Util.MyMath.Vector3.zero,
                new Util.MyMath.Vector3(300, 300, 1));

            body.IsStatic = true;
            body.Tag = false;

            for (int i = 0; i < 20; i++)
            {
                for (int e = i; e < 20; e++)
                {
                    var box = AddBox(new Util.MyMath.Vector3(0.0f, (e - i * 0.5f) * 1.01f, 0.5f + i * 1.0f),
                        Util.MyMath.Vector3.zero, Util.MyMath.Vector3.one);
                    box.IsStatic = true;
                }
            }
        }

        private RigidBody m_LastSphere = null;
        private void ShootSphere()
        {
            Util.MyMath.Vector3 pos, ang;
            dsGetViewPoint(out pos, out ang);

            RigidBody body = new RigidBody(new SphereShape(1.0f));
            body.Position = pos;

            Util.MyMath.Vector3 unit;
            unit.x = (float)Math.Cos(ang.x / 180.0f * Util.MyMath.Mathf.PI);
            unit.y = (float)Math.Sin(ang.x / 180.0f * Util.MyMath.Mathf.PI);
            unit.z = (float)Math.Sin(ang.y / 180.0f * Util.MyMath.Mathf.PI);

            body.LinearVelocity = unit * 50.0f;

            world.AddBody(body);

            m_LastSphere = body;
        }

        private RigidBody AddBox(Util.MyMath.Vector3 position, Util.MyMath.Vector3 velocity, Util.MyMath.Vector3 size)
        {
            BoxShape shape = new BoxShape(size);
            RigidBody body = new RigidBody(shape);
            world.AddBody(body);
            body.Position = position;
            body.Material.Restitution = 0.0f;
            body.LinearVelocity = velocity;
            body.IsActive = false;
            return body;
        }

        protected override void OnBeginRender(double elapsedTime)
        {
            if (initFrame)
            {
                dsSetViewpoint(new float[] { 18, 10, 8 }, new float[] { 190, -10, 0 });
                initFrame = false;
            }

            RenderAll();

            base.OnBeginRender(elapsedTime);
        }

        private void RenderAll()
        {
            dsSetTexture(DS_TEXTURE_NUMBER.DS_WOOD);

            foreach (RigidBody body in world.RigidBodies)
            {
                if (body.Tag is bool) continue;

                if (body.Shape is BoxShape)
                {
                    BoxShape shape = body.Shape as BoxShape;

                    if (body.IsActive) dsSetColor(1, 1, 1);
                    else dsSetColor(0.5f, 0.5f, 1);

                    dsDrawBox(body.Position, body.Orientation, shape.Size);
                }
                else if (body.Shape is SphereShape)
                {
                    SphereShape shape = body.Shape as SphereShape;

                    if (body.IsActive) dsSetColor(1,1,0);
                    else dsSetColor(0.5f, 0.5f, 1);

                    dsDrawSphere(body.Position, body.Orientation, shape.Radius-0.1f);
                }
            }
        }

        float accTime = 0.0f;

        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {
            accTime += 1.0f / (float)RenderFrequency;

            if (accTime > 1.0f)
            {
                this.Title = title + " " + RenderFrequency.ToString("##.#") + " fps";
                accTime = 0.0f;
            }

            float step = 1.0f / (float)RenderFrequency;
            if (step > 1.0f / 100.0f) step = 1.0f / 100.0f;
            world.Step(step, true);

            base.OnUpdateFrame(e);
        }

        static void Main(string[] args)
        {
            using (Program p = new Program())
            {
                p.Run();
            }
        }
    }
}

