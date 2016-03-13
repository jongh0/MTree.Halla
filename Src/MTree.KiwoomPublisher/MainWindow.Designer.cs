namespace MTree.KiwoomPublisher
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.axKHOpenAPI = new AxKHOpenAPILib.AxKHOpenAPI();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI)).BeginInit();
            this.SuspendLayout();
            // 
            // axKHOpenAPI
            // 
            this.axKHOpenAPI.Enabled = true;
            this.axKHOpenAPI.Location = new System.Drawing.Point(13, 216);
            this.axKHOpenAPI.Name = "axKHOpenAPI";
            this.axKHOpenAPI.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axKHOpenAPI.OcxState")));
            this.axKHOpenAPI.Size = new System.Drawing.Size(70, 21);
            this.axKHOpenAPI.TabIndex = 0;
            this.axKHOpenAPI.Visible = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.axKHOpenAPI);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "KiwoomPublisher";
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        
        private AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI;
    }
}

