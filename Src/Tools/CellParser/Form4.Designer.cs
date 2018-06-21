namespace WindowsFormsApplication1
{
  partial class Form4
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.SuspendLayout();
      // 
      // Form4
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(901, 579);
      this.Name = "Form4";
      this.Text = "Form4";
      this.Load += new System.EventHandler(this.Form4_Load);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form4_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form4_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form4_MouseUp);
      this.ResumeLayout(false);

    }

    #endregion
  }
}