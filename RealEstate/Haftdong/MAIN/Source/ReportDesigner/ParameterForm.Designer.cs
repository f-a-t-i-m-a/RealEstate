namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    partial class ParameterForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtParameterName = new System.Windows.Forms.TextBox();
            this.cmbParameterType = new System.Windows.Forms.ComboBox();
            this.txtDefaultValue = new System.Windows.Forms.TextBox();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.chbRequired = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDisplayText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(358, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "نام";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(358, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "نوع";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(358, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "مقدار اولیه";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(358, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "کمترین";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(173, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "بیشترین";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(358, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "اجباری";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(120, 200);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "ذخیره";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(39, 200);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "لغو";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtParameterName
            // 
            this.txtParameterName.Location = new System.Drawing.Point(39, 18);
            this.txtParameterName.Name = "txtParameterName";
            this.txtParameterName.Size = new System.Drawing.Size(313, 20);
            this.txtParameterName.TabIndex = 9;
            // 
            // cmbParameterType
            // 
            this.cmbParameterType.FormattingEnabled = true;
            this.cmbParameterType.Location = new System.Drawing.Point(39, 68);
            this.cmbParameterType.Name = "cmbParameterType";
            this.cmbParameterType.Size = new System.Drawing.Size(313, 21);
            this.cmbParameterType.TabIndex = 11;
            // 
            // txtDefaultValue
            // 
            this.txtDefaultValue.Location = new System.Drawing.Point(39, 95);
            this.txtDefaultValue.Name = "txtDefaultValue";
            this.txtDefaultValue.Size = new System.Drawing.Size(313, 20);
            this.txtDefaultValue.TabIndex = 12;
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(223, 121);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(129, 20);
            this.txtMinValue.TabIndex = 13;
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(39, 121);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(128, 20);
            this.txtMaxValue.TabIndex = 14;
            // 
            // chbRequired
            // 
            this.chbRequired.AutoSize = true;
            this.chbRequired.Location = new System.Drawing.Point(337, 147);
            this.chbRequired.Name = "chbRequired";
            this.chbRequired.Size = new System.Drawing.Size(15, 14);
            this.chbRequired.TabIndex = 15;
            this.chbRequired.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(358, 45);
            this.label7.Name = "label7";
            this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "نام قابل نمایش";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtDisplayText
            // 
            this.txtDisplayText.Location = new System.Drawing.Point(39, 42);
            this.txtDisplayText.Name = "txtDisplayText";
            this.txtDisplayText.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtDisplayText.Size = new System.Drawing.Size(313, 20);
            this.txtDisplayText.TabIndex = 10;
            // 
            // ParameterForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(442, 235);
            this.Controls.Add(this.txtDisplayText);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chbRequired);
            this.Controls.Add(this.txtMaxValue);
            this.Controls.Add(this.txtMinValue);
            this.Controls.Add(this.txtDefaultValue);
            this.Controls.Add(this.cmbParameterType);
            this.Controls.Add(this.txtParameterName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ParameterForm";
            this.Text = "اضافه کردن پارامتر";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtParameterName;
        private System.Windows.Forms.ComboBox cmbParameterType;
        private System.Windows.Forms.TextBox txtDefaultValue;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.CheckBox chbRequired;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDisplayText;
    }
}