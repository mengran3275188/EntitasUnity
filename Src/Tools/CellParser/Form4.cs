using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScriptRuntime;
using DashFireSpatial;

namespace WindowsFormsApplication1
{
  public partial class Form4 : Form
  {
    private Timer mFormsTimer;
    private BufferedGraphicsContext context_;
    private BufferedGraphics graphics_;
    private float map_width_ = 800;
    private float map_height_ = 600;

    private Pen circle_pen_ = new Pen(Color.Black);
    private Pen line_pen_ = new Pen(Color.Green);
    private Pen hit_point_pen_ = new Pen(Color.Red);
    private Pen gun_pen_ = new Pen(Color.Red);
    private Vector3 circle_center_ = new Vector3();
    private float radius_ = 30;
    private Vector3 ray_start_pos_ = new Vector3();
    private Vector3 ray_end_pos_ = new Vector3();

    private Vector3 gun_end_pos_ = new Vector3(-20, 0, 60);
    private Vector3 gun_start_pos_ = new Vector3(-20, 0, 0);

    private float control_radius_ = 40;
    private TestSpaceObject control_obj_ = new TestSpaceObject(1);
    private bool is_rotate_ = false;

    public Form4()
    {
      InitializeComponent();
      Circle circle = new Circle(new Vector3(), control_radius_);
      control_obj_.SetCollideShape(circle);
      control_obj_.SetPosition(new Vector3());
    }

    private void Form4_Load(object sender, EventArgs e)
    {
      InitGraphics();

    }

    private void InitGraphics()
    {
      mFormsTimer = new System.Windows.Forms.Timer();
      mFormsTimer.Interval = 50;
      mFormsTimer.Tick += new System.EventHandler(this.TimerEventProcessor);
      mFormsTimer.Start();

      context_ = BufferedGraphicsManager.Current;
      context_.MaximumBuffer = new Size((int)map_width_, (int)map_height_);
      graphics_ = context_.Allocate(this.CreateGraphics(), new Rectangle(0, 0, (int)map_width_, (int)map_height_));
    }

    private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
    {
      Console.WriteLine("in timer tick");
      graphics_.Graphics.Clear(Color.White);
      Render();
      graphics_.Render();
    }

    private void Render()
    {
      DrawObj(circle_pen_,control_obj_);
      DrawLine(line_pen_, ray_start_pos_, ray_end_pos_);
      if (Math.Abs(ray_start_pos_.X - ray_end_pos_.X) > 0.001 && Math.Abs(ray_start_pos_.Z - ray_end_pos_.Z) > 0.001) {
        Vector3 ray_dir = new Vector3(ray_end_pos_.X - ray_start_pos_.X, 0, ray_end_pos_.Z - ray_start_pos_.Z);
        Vector3 hit_point;
        DashFire.Geometry.GetRayWithCircleFirstHitPoint(ray_start_pos_, ray_dir, circle_center_, radius_, out hit_point);
        if (Math.Abs(hit_point.X) > 0.001 || Math.Abs(hit_point.Z) > 0.001) {
          DrawLine(hit_point_pen_, hit_point, new Vector3(hit_point.X + 2, 0, hit_point.Z + 2));
        }
      }
      
    }

    private void DrawCircel(Pen pen, DashFireSpatial.Circle ci)
    {
      Vector3 center = ci.world_center_pos();
      graphics_.Graphics.DrawLine(pen, center.X, center.Z, center.X + 1, center.Z + 1);
      graphics_.Graphics.DrawEllipse(pen, (float)(center.X - ci.radius()),
          (float)(center.Z - ci.radius()), (float)ci.radius() * 2, (float)ci.radius() * 2);
    }

    private void DrawFace(Pen pen, Circle ci, double dir)
    {
      Vector3 center = ci.world_center_pos();
      Vector3 face_point;
      DashFire.Geometry.GetRayWithCircleFirstHitPoint(center,
                    DashFire.Geometry.GetVectorFromDir(dir), 
                    center, (float)ci.radius(), out face_point);
      graphics_.Graphics.DrawLine(pen, center.X, center.Z, face_point.X, face_point.Z);
    }

    private void DrawGun(Pen pen, TestSpaceObject obj, Vector3 gun_start, Vector3 gun_end)
    {
      Vector3 gun_word_start = Shape.TransformToWorldPos(obj.Position, gun_start, obj.CosDir, obj.SinDir);
      Vector3 gun_word_end = Shape.TransformToWorldPos(obj.Position, gun_end, obj.CosDir, obj.SinDir);
      graphics_.Graphics.DrawLine(gun_pen_, gun_word_start.X, gun_word_start.Z, gun_word_end.X, gun_word_end.Z);
    }

    private void DrawObj(Pen pen, TestSpaceObject obj)
    {
      obj.GetCollideShape().Transform(obj.GetPosition(), obj.CosDir, obj.SinDir);
      Circle c = obj.GetCollideShape() as Circle;
      if (c != null) {
        DrawCircel(pen, c);
        DrawFace(pen, c, obj.GetFaceDirection());
        DrawGun(pen, obj, gun_start_pos_, gun_end_pos_);
      }
    }

    private void DrawLine(Pen pen, Vector3 start, Vector3 end)
    {
      graphics_.Graphics.DrawLine(pen, start.X, start.Z, end.X, end.Z);
    }

    private void DrawCircle(Pen pen, Vector3 center, float radius)
    {
      graphics_.Graphics.DrawEllipse(pen, center.X - radius, center.Z - radius, 2 * radius, 2 * radius);
    }

    private double GetRotateDegree(TestSpaceObject obj, Vector3 gun_word_end, float mouse_x, float mouse_y)
    {
      Vector3 center = obj.GetPosition();
      Vector3 mouse_center_vect = new Vector3(mouse_x - center.X, 0, mouse_y - center.Z);

      Vector3 gun_word_start = GetGunStartPos(center, gun_word_end, obj.GetFaceDirection());
      float rotate_circle_radius = DashFire.Geometry.GetMod(new Vector3(mouse_x - center.X, 0, mouse_y - center.Z));
      Vector3 hit_point;
      if (!DashFire.Geometry.GetRayWithCircleFirstHitPoint(gun_word_start, 
                                         DashFire.Geometry.GetVectorFromDir(obj.GetFaceDirection()), 
                                         center, rotate_circle_radius, 
                                         out hit_point)) {
        return obj.GetFaceDirection();
      }
      Vector3 hit_vect = new Vector3(hit_point.X - center.X, 0, hit_point.Z - center.Z);
      float rotate_degree = DashFire.Geometry.GetVectorAngle(mouse_center_vect, hit_vect);

      double judge_gun_dir = Math.Atan2(mouse_x - gun_word_end.X, mouse_y - gun_word_end.Z);
      double degree = (judge_gun_dir - obj.GetFaceDirection()) % (2*Math.PI);
      if (Math.Abs(degree) > Math.PI) {
        if (degree > 0) {
          return obj.GetFaceDirection() - rotate_degree;
        } else {
          return obj.GetFaceDirection() + rotate_degree;
        }
      } else {
        if (degree > 0) {
          return obj.GetFaceDirection() + rotate_degree;
        } else {
          return obj.GetFaceDirection() - rotate_degree;
        }
      }
    }

    private Vector3 GetGunStartPos(Vector3 obj_pos, Vector3 gun_end_pos, double direction)
    {
      Vector3 obj_to_gun_endv = new Vector3(gun_end_pos.X - obj_pos.X, gun_end_pos.Y, gun_end_pos.Z - obj_pos.Z);
      Vector3 face_n = DashFire.Geometry.GetVectorFromDir(direction);
      Vector3 vt = DashFire.Geometry.GetVtProjection(obj_to_gun_endv, face_n);
      Vector3 start_pos = new Vector3();
      start_pos.X = obj_pos.X + vt.X;
      start_pos.Z = obj_pos.Z + vt.Z;
      return start_pos;
    }

    private void Form4_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left) {
        control_obj_.SetPosition(new Vector3(e.X, 0, e.Y));
      }
      if (e.Button == MouseButtons.Right) {
        is_rotate_ = true;
      }
    }

    private void Form4_MouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left) {
      }
      if (e.Button == MouseButtons.Right) {
        is_rotate_ = false;
      }
    }

    private void Form4_MouseMove(object sender, MouseEventArgs e)
    {
      if (is_rotate_) {
        Vector3 cent = control_obj_.GetPosition();
        Vector3 gun_word_end = Shape.TransformToWorldPos(cent, gun_end_pos_, control_obj_.CosDir, control_obj_.SinDir);
        double direction = GetRotateDegree(control_obj_, gun_word_end, e.X, e.Y);
        control_obj_.SetFaceDirection(direction);
      }
    }
  }
}
