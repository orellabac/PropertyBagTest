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
            this.userControl11 = new PropertyBagTest.UserControl1();
            this.userControl12 = new PropertyBagTest.UserControl1();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // userControl11
            // 
            resources.ApplyResources(this.userControl11, "userControl11");
            this.userControl11.MyProperty = 20;
            this.userControl11.Name = "userControl11";
            // 
            // userControl12
            // 
            resources.ApplyResources(this.userControl12, "userControl12");
            this.userControl12.MyProperty = 0;
            this.userControl12.Name = "userControl12";
            // 
            // button1
            // 
            this.button1.Image = global::PropertyBag.Properties.Resources.feeds;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.userControl12);
            this.Controls.Add(this.userControl11);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControl1 userControl1121;
        private UserControl1 userControl11;
        private UserControl1 userControl12;
        private System.Windows.Forms.Button button1;
    }
}

