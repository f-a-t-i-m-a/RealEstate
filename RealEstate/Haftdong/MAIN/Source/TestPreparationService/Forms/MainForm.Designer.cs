namespace JahanJooy.RealEstateAgency.TestPreparationService.Forms
{
    partial class MainForm
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
            this.lstCollection = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstEntity = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEntity = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.btnClean = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstCollection
            // 
            this.lstCollection.FormattingEnabled = true;
            this.lstCollection.ItemHeight = 16;
            this.lstCollection.Location = new System.Drawing.Point(585, 31);
            this.lstCollection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstCollection.Name = "lstCollection";
            this.lstCollection.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lstCollection.Size = new System.Drawing.Size(233, 324);
            this.lstCollection.TabIndex = 0;
            this.lstCollection.SelectedIndexChanged += new System.EventHandler(this.lstCollection_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(737, 11);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "نوع موجودیت";
            // 
            // lstEntity
            // 
            this.lstEntity.FormattingEnabled = true;
            this.lstEntity.ItemHeight = 16;
            this.lstEntity.Location = new System.Drawing.Point(346, 31);
            this.lstEntity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstEntity.Name = "lstEntity";
            this.lstEntity.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lstEntity.Size = new System.Drawing.Size(233, 324);
            this.lstEntity.TabIndex = 2;
            this.lstEntity.SelectedIndexChanged += new System.EventHandler(this.lstEntity_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(504, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "موجودیت ها";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(294, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "جزئیات";
            // 
            // txtEntity
            // 
            this.txtEntity.Location = new System.Drawing.Point(14, 31);
            this.txtEntity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtEntity.Multiline = true;
            this.txtEntity.Name = "txtEntity";
            this.txtEntity.ReadOnly = true;
            this.txtEntity.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEntity.Size = new System.Drawing.Size(326, 324);
            this.txtEntity.TabIndex = 6;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(712, 375);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(105, 43);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "حذف";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRebuild
            // 
            this.btnRebuild.Location = new System.Drawing.Point(601, 375);
            this.btnRebuild.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(105, 43);
            this.btnRebuild.TabIndex = 9;
            this.btnRebuild.Text = "بازسازی";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // btnClean
            // 
            this.btnClean.Location = new System.Drawing.Point(472, 375);
            this.btnClean.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(126, 43);
            this.btnClean.TabIndex = 10;
            this.btnClean.Text = "پاک سازی دیتابیس";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(361, 375);
            this.btnExit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(105, 43);
            this.btnExit.TabIndex = 11;
            this.btnExit.Text = "خروج";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 431);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnClean);
            this.Controls.Add(this.btnRebuild);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.txtEntity);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstEntity);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstCollection);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstCollection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstEntity;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEntity;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.Button btnExit;
    }
}