using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
  public partial class FormTemplate : Form
  {
    private Timer mFormsTimer;
    private BufferedGraphicsContext context_;
    private BufferedGraphics graphics_;
    private float map_width_ = 800;
    private float map_height_ = 600;

    public FormTemplate()
    {
      InitializeComponent();
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
      graphics_.Render();
    }

  }
}
