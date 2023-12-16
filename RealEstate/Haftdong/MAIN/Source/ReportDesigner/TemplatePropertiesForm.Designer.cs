namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    partial class TemplatePropertiesForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtDataSourceType = new System.Windows.Forms.TextBox();
            this.txtApplicationImplementedDataSourceType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtOrder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "نوع منبع داده:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtDataSourceType
            // 
            this.txtDataSourceType.Location = new System.Drawing.Point(150, 31);
            this.txtDataSourceType.Name = "txtDataSourceType";
            this.txtDataSourceType.ReadOnly = true;
            this.txtDataSourceType.Size = new System.Drawing.Size(223, 20);
            this.txtDataSourceType.TabIndex = 1;
            // 
            // txtApplicationImplementedDataSourceType
            // 
            this.txtApplicationImplementedDataSourceType.Location = new System.Drawing.Point(150, 57);
            this.txtApplicationImplementedDataSourceType.Name = "txtApplicationImplementedDataSourceType";
            this.txtApplicationImplementedDataSourceType.ReadOnly = true;
            this.txtApplicationImplementedDataSourceType.Size = new System.Drawing.Size(223, 20);
            this.txtApplicationImplementedDataSourceType.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "نوع منبع داده سیستمی:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(150, 83);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(223, 20);
            this.txtName.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(121, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "نام:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(150, 109);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(223, 20);
            this.txtKey.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(115, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "کلید:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(150, 135);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(223, 86);
            this.txtDescription.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(92, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "توضیحات:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtOrder
            // 
            this.txtOrder.Location = new System.Drawing.Point(150, 227);
            this.txtOrder.Name = "txtOrder";
            this.txtOrder.Size = new System.Drawing.Size(223, 20);
            this.txtOrder.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(109, 230);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "ترتیب:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(298, 273);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "لغو";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(217, 273);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "تایید";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // TemplatePropertiesForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(412, 325);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtOrder);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtApplicationImplementedDataSourceType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDataSourceType);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplatePropertiesForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "خصوصیات قالب گزارش";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDataSourceType;
        private System.Windows.Forms.TextBox txtApplicationImplementedDataSourceType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOrder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}