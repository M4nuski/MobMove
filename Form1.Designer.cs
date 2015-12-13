namespace MobMove
{
    partial class Form1
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
            if (disposing && (components != null))
            {
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
            this.components = new System.ComponentModel.Container();
            this.OutputLabel = new System.Windows.Forms.Label();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // OutputLabel
            // 
            this.OutputLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.OutputLabel.Location = new System.Drawing.Point(0, 0);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(840, 480);
            this.OutputLabel.TabIndex = 0;
            this.OutputLabel.Text = "Viewport";
            this.OutputLabel.Paint += new System.Windows.Forms.PaintEventHandler(this.OutputLabel_Paint);
            this.OutputLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OutputLabel_MouseDown);
            this.OutputLabel.MouseLeave += new System.EventHandler(this.OutputLabel_MouseLeave);
            this.OutputLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OutputLabel_MouseMove);
            this.OutputLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OutputLabel_MouseUp);
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 576);
            this.Controls.Add(this.OutputLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Game Tests : Mob Movement Test (WinForms)";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label OutputLabel;
        private System.Windows.Forms.Timer UpdateTimer;
    }
}

