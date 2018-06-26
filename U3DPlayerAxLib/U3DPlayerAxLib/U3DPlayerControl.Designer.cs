namespace U3DPlayerAxLib
{
    partial class U3DPlayerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(U3DPlayerControl));
            this.u3dPlayer = new AxUnityWebPlayerAXLib.AxUnityWebPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.u3dPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // u3dPlayer
            // 
            this.u3dPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.u3dPlayer.Enabled = true;
            this.u3dPlayer.Location = new System.Drawing.Point(0, 0);
            this.u3dPlayer.Name = "u3dPlayer";
            this.u3dPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("u3dPlayer.OcxState")));
            this.u3dPlayer.Size = new System.Drawing.Size(352, 335);
            this.u3dPlayer.TabIndex = 0;
            // 
            // U3DPlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.u3dPlayer);
            this.Name = "U3DPlayerControl";
            this.Size = new System.Drawing.Size(352, 335);
            ((System.ComponentModel.ISupportInitialize)(this.u3dPlayer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxUnityWebPlayerAXLib.AxUnityWebPlayer u3dPlayer;
    }
}
