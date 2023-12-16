namespace JahanJooy.RealEstateAgency.ReportDesigner
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
            this.btnEditTemplate = new System.Windows.Forms.Button();
            this.btnNewTemplate = new System.Windows.Forms.Button();
            this.lstDataSourceType = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstReportTemplate = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnViewReport = new System.Windows.Forms.Button();
            this.btnDeleteTemplate = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImpot = new System.Windows.Forms.Button();
            this.lstDataSource = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnEditProperties = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnAddParameter = new System.Windows.Forms.Button();
            this.grpParams = new System.Windows.Forms.GroupBox();
            this.btnRemoveParameter = new System.Windows.Forms.Button();
            this.btnEditParameter = new System.Windows.Forms.Button();
            this.lsbParameters = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.btnLogout = new System.Windows.Forms.Button();
            this.grpParams.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEditTemplate
            // 
            this.btnEditTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditTemplate.Location = new System.Drawing.Point(93, 348);
            this.btnEditTemplate.Name = "btnEditTemplate";
            this.btnEditTemplate.Size = new System.Drawing.Size(75, 23);
            this.btnEditTemplate.TabIndex = 3;
            this.btnEditTemplate.Text = "ویرایش";
            this.btnEditTemplate.UseVisualStyleBackColor = true;
            this.btnEditTemplate.Click += new System.EventHandler(this.btnEditTemplate_Click);
            // 
            // btnNewTemplate
            // 
            this.btnNewTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewTemplate.Enabled = false;
            this.btnNewTemplate.Location = new System.Drawing.Point(12, 348);
            this.btnNewTemplate.Name = "btnNewTemplate";
            this.btnNewTemplate.Size = new System.Drawing.Size(75, 23);
            this.btnNewTemplate.TabIndex = 2;
            this.btnNewTemplate.Text = "جدید";
            this.btnNewTemplate.UseVisualStyleBackColor = true;
            this.btnNewTemplate.Click += new System.EventHandler(this.btnNewTemplate_Click);
            // 
            // lstDataSourceType
            // 
            this.lstDataSourceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstDataSourceType.DisplayMember = "DisplayText";
            this.lstDataSourceType.FormattingEnabled = true;
            this.lstDataSourceType.IntegralHeight = false;
            this.lstDataSourceType.Location = new System.Drawing.Point(15, 32);
            this.lstDataSourceType.Name = "lstDataSourceType";
            this.lstDataSourceType.Size = new System.Drawing.Size(153, 300);
            this.lstDataSourceType.TabIndex = 1;
            this.lstDataSourceType.ValueMember = "Value";
            this.lstDataSourceType.SelectedIndexChanged += new System.EventHandler(this.lstDataSourceType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "نوع منبع";
            // 
            // lstReportTemplate
            // 
            this.lstReportTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstReportTemplate.DisplayMember = "DisplayText";
            this.lstReportTemplate.FormattingEnabled = true;
            this.lstReportTemplate.IntegralHeight = false;
            this.lstReportTemplate.Location = new System.Drawing.Point(333, 32);
            this.lstReportTemplate.Name = "lstReportTemplate";
            this.lstReportTemplate.Size = new System.Drawing.Size(240, 300);
            this.lstReportTemplate.TabIndex = 3;
            this.lstReportTemplate.ValueMember = "Value";
            this.lstReportTemplate.SelectedIndexChanged += new System.EventHandler(this.lstReportTemplate_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(330, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "قالب";
            // 
            // btnViewReport
            // 
            this.btnViewReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewReport.Location = new System.Drawing.Point(255, 348);
            this.btnViewReport.Name = "btnViewReport";
            this.btnViewReport.Size = new System.Drawing.Size(75, 23);
            this.btnViewReport.TabIndex = 5;
            this.btnViewReport.Text = "نمایش";
            this.btnViewReport.UseVisualStyleBackColor = true;
            this.btnViewReport.Click += new System.EventHandler(this.btnViewReport_Click);
            // 
            // btnDeleteTemplate
            // 
            this.btnDeleteTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteTemplate.Location = new System.Drawing.Point(336, 348);
            this.btnDeleteTemplate.Name = "btnDeleteTemplate";
            this.btnDeleteTemplate.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteTemplate.TabIndex = 6;
            this.btnDeleteTemplate.Text = "حذف";
            this.btnDeleteTemplate.UseVisualStyleBackColor = true;
            this.btnDeleteTemplate.Click += new System.EventHandler(this.btnDeleteTemplate_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.Location = new System.Drawing.Point(579, 348);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 7;
            this.btnExport.Text = "خروج فایل...";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImpot
            // 
            this.btnImpot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImpot.Location = new System.Drawing.Point(498, 348);
            this.btnImpot.Name = "btnImpot";
            this.btnImpot.Size = new System.Drawing.Size(75, 23);
            this.btnImpot.TabIndex = 8;
            this.btnImpot.Text = "ورود فایل...";
            this.btnImpot.UseVisualStyleBackColor = true;
            this.btnImpot.Click += new System.EventHandler(this.btnImpot_Click);
            // 
            // lstDataSource
            // 
            this.lstDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstDataSource.DisplayMember = "DisplayText";
            this.lstDataSource.FormattingEnabled = true;
            this.lstDataSource.IntegralHeight = false;
            this.lstDataSource.Location = new System.Drawing.Point(174, 32);
            this.lstDataSource.Name = "lstDataSource";
            this.lstDataSource.Size = new System.Drawing.Size(153, 300);
            this.lstDataSource.TabIndex = 9;
            this.lstDataSource.ValueMember = "Value";
            this.lstDataSource.SelectedIndexChanged += new System.EventHandler(this.lstDataSource_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(171, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "منبع";
            // 
            // btnEditProperties
            // 
            this.btnEditProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditProperties.Location = new System.Drawing.Point(174, 348);
            this.btnEditProperties.Name = "btnEditProperties";
            this.btnEditProperties.Size = new System.Drawing.Size(75, 23);
            this.btnEditProperties.TabIndex = 12;
            this.btnEditProperties.Text = "مشخصات";
            this.btnEditProperties.UseVisualStyleBackColor = true;
            this.btnEditProperties.Click += new System.EventHandler(this.btnEditProperties_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Location = new System.Drawing.Point(417, 348);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 13;
            this.btnCopy.Text = "کپی...";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnAddParameter
            // 
            this.btnAddParameter.Location = new System.Drawing.Point(169, 198);
            this.btnAddParameter.Name = "btnAddParameter";
            this.btnAddParameter.Size = new System.Drawing.Size(75, 23);
            this.btnAddParameter.TabIndex = 14;
            this.btnAddParameter.Text = "ایجاد";
            this.btnAddParameter.UseVisualStyleBackColor = true;
            this.btnAddParameter.Click += new System.EventHandler(this.btnAddParameter_Click);
            // 
            // grpParams
            // 
            this.grpParams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpParams.Controls.Add(this.btnRemoveParameter);
            this.grpParams.Controls.Add(this.btnEditParameter);
            this.grpParams.Controls.Add(this.lsbParameters);
            this.grpParams.Controls.Add(this.btnAddParameter);
            this.grpParams.Location = new System.Drawing.Point(579, 32);
            this.grpParams.Name = "grpParams";
            this.grpParams.Size = new System.Drawing.Size(253, 230);
            this.grpParams.TabIndex = 15;
            this.grpParams.TabStop = false;
            this.grpParams.Text = "پارامترها";
            // 
            // btnRemoveParameter
            // 
            this.btnRemoveParameter.Location = new System.Drawing.Point(6, 198);
            this.btnRemoveParameter.Name = "btnRemoveParameter";
            this.btnRemoveParameter.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveParameter.TabIndex = 16;
            this.btnRemoveParameter.Text = "حذف";
            this.btnRemoveParameter.UseVisualStyleBackColor = true;
            this.btnRemoveParameter.Click += new System.EventHandler(this.btnRemoveParameter_Click);
            // 
            // btnEditParameter
            // 
            this.btnEditParameter.Location = new System.Drawing.Point(88, 198);
            this.btnEditParameter.Name = "btnEditParameter";
            this.btnEditParameter.Size = new System.Drawing.Size(75, 23);
            this.btnEditParameter.TabIndex = 15;
            this.btnEditParameter.Text = "ویرایش";
            this.btnEditParameter.UseVisualStyleBackColor = true;
            this.btnEditParameter.Click += new System.EventHandler(this.btnEditParameter_Click);
            // 
            // lsbParameters
            // 
            this.lsbParameters.FormattingEnabled = true;
            this.lsbParameters.Location = new System.Drawing.Point(6, 19);
            this.lsbParameters.Name = "lsbParameters";
            this.lsbParameters.Size = new System.Drawing.Size(238, 173);
            this.lsbParameters.TabIndex = 4;
            this.lsbParameters.SelectedIndexChanged += new System.EventHandler(this.lsbParameters_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtId);
            this.panel1.Location = new System.Drawing.Point(579, 268);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 64);
            this.panel1.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(212, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ID:";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(6, 25);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(200, 20);
            this.txtId.TabIndex = 0;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(660, 348);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 17;
            this.btnLogout.Text = "خروج";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 383);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grpParams);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnEditProperties);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstDataSource);
            this.Controls.Add(this.btnImpot);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnDeleteTemplate);
            this.Controls.Add(this.btnViewReport);
            this.Controls.Add(this.btnEditTemplate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnNewTemplate);
            this.Controls.Add(this.lstReportTemplate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstDataSourceType);
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "هفت دنگ - طراحی گزارش";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.grpParams.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEditTemplate;
        private System.Windows.Forms.Button btnNewTemplate;
        private System.Windows.Forms.ListBox lstDataSourceType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstReportTemplate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnViewReport;
        private System.Windows.Forms.Button btnDeleteTemplate;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImpot;
        private System.Windows.Forms.ListBox lstDataSource;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnEditProperties;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnAddParameter;
        private System.Windows.Forms.GroupBox grpParams;
        private System.Windows.Forms.ListBox lsbParameters;
        private System.Windows.Forms.Button btnEditParameter;
        private System.Windows.Forms.Button btnRemoveParameter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Button btnLogout;
    }
}

