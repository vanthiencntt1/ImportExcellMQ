namespace ImportXML
{
    partial class Form2
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
            this.btnnoicapbhyt = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnnoicapbhyt
            // 
            this.btnnoicapbhyt.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnnoicapbhyt.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnnoicapbhyt.ForeColor = System.Drawing.Color.Red;
            this.btnnoicapbhyt.Location = new System.Drawing.Point(0, 0);
            this.btnnoicapbhyt.Name = "btnnoicapbhyt";
            this.btnnoicapbhyt.Size = new System.Drawing.Size(284, 30);
            this.btnnoicapbhyt.TabIndex = 22;
            this.btnnoicapbhyt.Text = "DM NƠI CẤP THẺ";
            this.btnnoicapbhyt.UseVisualStyleBackColor = true;
            this.btnnoicapbhyt.Click += new System.EventHandler(this.btnnoicapbhyt_Click);
            // 
            // button1
            // 
            //this.button1.Location = new System.Drawing.Point(24, 165);
            //this.button1.Name = "button1";
            //this.button1.Size = new System.Drawing.Size(219, 52);
            //this.button1.TabIndex = 23;
            //this.button1.Text = "button1";
            //this.button1.UseVisualStyleBackColor = true;
            //this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 265);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnnoicapbhyt);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnnoicapbhyt;
        private System.Windows.Forms.Button button1;
    }
}