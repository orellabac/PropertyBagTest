namespace PropertyBagTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.userControl13 = new PropertyBagTest.UserControl1();
            this.userControl14 = new PropertyBagTest.UserControl1();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.propertyGrid2 = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // userControl13
            // 
            this.userControl13.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.userControl13, "userControl13");
            this.userControl13.MyProperty = 0;
            this.userControl13.Name = "userControl13";
            // 
            // userControl14
            // 
            this.userControl14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.userControl14, "userControl14");
            this.userControl14.MyProperty = 0;
            this.userControl14.Name = "userControl14";
            // 
            // propertyGrid1
            // 
            resources.ApplyResources(this.propertyGrid1, "propertyGrid1");
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedObject = this.userControl13;
            // 
            // propertyGrid2
            // 
            resources.ApplyResources(this.propertyGrid2, "propertyGrid2");
            this.propertyGrid2.Name = "propertyGrid2";
            this.propertyGrid2.SelectedObject = this.userControl14;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyGrid2);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.userControl14);
            this.Controls.Add(this.userControl13);
            this.Name = "Form1";
            this.userControl13.ReadPropertiesFromResources();
            this.userControl14.ReadPropertiesFromResources();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControl1 userControl1121;
        private UserControl1 userControl11;
        private UserControl1 userControl12;
        private System.Windows.Forms.Button button1;
        private UserControl1 userControl13;
        private UserControl1 userControl14;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.PropertyGrid propertyGrid2;
    }
}

