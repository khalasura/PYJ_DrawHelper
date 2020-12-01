namespace PYJ_DrawHelper
{
    partial class PYJ_DrawHelpDialog
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCommit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbRectNo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numPosX = new System.Windows.Forms.NumericUpDown();
            this.numPosY = new System.Windows.Forms.NumericUpDown();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.numLineWidth = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.chkText = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.pnlText = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.btnImageSize = new System.Windows.Forms.Button();
            this.btnImageClear = new System.Windows.Forms.Button();
            this.btnImage = new System.Windows.Forms.Button();
            this.btnFontStyle = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtFontStyle = new System.Windows.Forms.TextBox();
            this.cmbTextAlign = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkFill = new System.Windows.Forms.CheckBox();
            this.chkLine = new System.Windows.Forms.CheckBox();
            this.pnlFill = new System.Windows.Forms.Panel();
            this.pnlLine = new System.Windows.Forms.Panel();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.txtText = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.numAngle1 = new System.Windows.Forms.NumericUpDown();
            this.numAngle2 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numPosX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLineWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngle1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngle2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommit.Location = new System.Drawing.Point(170, 404);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(55, 23);
            this.btnCommit.TabIndex = 0;
            this.btnCommit.Text = "확인";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(229, 404);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(55, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbRectNo
            // 
            this.cmbRectNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRectNo.FormattingEnabled = true;
            this.cmbRectNo.Location = new System.Drawing.Point(89, 16);
            this.cmbRectNo.Name = "cmbRectNo";
            this.cmbRectNo.Size = new System.Drawing.Size(173, 20);
            this.cmbRectNo.TabIndex = 2;
            this.cmbRectNo.SelectedIndexChanged += new System.EventHandler(this.cmbRectNo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Object No";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Position";
            // 
            // numPosX
            // 
            this.numPosX.Location = new System.Drawing.Point(89, 112);
            this.numPosX.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
            this.numPosX.Minimum = new decimal(new int[] {
            32767,
            0,
            0,
            -2147483648});
            this.numPosX.Name = "numPosX";
            this.numPosX.Size = new System.Drawing.Size(84, 21);
            this.numPosX.TabIndex = 6;
            this.numPosX.ValueChanged += new System.EventHandler(this.numPosX_ValueChanged);
            // 
            // numPosY
            // 
            this.numPosY.Location = new System.Drawing.Point(179, 112);
            this.numPosY.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
            this.numPosY.Minimum = new decimal(new int[] {
            32767,
            0,
            0,
            -2147483648});
            this.numPosY.Name = "numPosY";
            this.numPosY.Size = new System.Drawing.Size(84, 21);
            this.numPosY.TabIndex = 7;
            this.numPosY.ValueChanged += new System.EventHandler(this.numPosY_ValueChanged);
            // 
            // numWidth
            // 
            this.numWidth.Location = new System.Drawing.Point(89, 139);
            this.numWidth.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(84, 21);
            this.numWidth.TabIndex = 8;
            this.numWidth.ValueChanged += new System.EventHandler(this.numWidth_ValueChanged);
            // 
            // numHeight
            // 
            this.numHeight.Location = new System.Drawing.Point(179, 139);
            this.numHeight.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(84, 21);
            this.numHeight.TabIndex = 9;
            this.numHeight.ValueChanged += new System.EventHandler(this.numHeight_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picImage);
            this.groupBox1.Controls.Add(this.numLineWidth);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.chkText);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.cmbType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbRectNo);
            this.groupBox1.Controls.Add(this.pnlText);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.btnImageSize);
            this.groupBox1.Controls.Add(this.btnImageClear);
            this.groupBox1.Controls.Add(this.btnImage);
            this.groupBox1.Controls.Add(this.btnFontStyle);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtFontStyle);
            this.groupBox1.Controls.Add(this.cmbTextAlign);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkFill);
            this.groupBox1.Controls.Add(this.chkLine);
            this.groupBox1.Controls.Add(this.pnlFill);
            this.groupBox1.Controls.Add(this.pnlLine);
            this.groupBox1.Controls.Add(this.txtKey);
            this.groupBox1.Controls.Add(this.txtText);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numHeight);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numAngle1);
            this.groupBox1.Controls.Add(this.numWidth);
            this.groupBox1.Controls.Add(this.numAngle2);
            this.groupBox1.Controls.Add(this.numPosX);
            this.groupBox1.Controls.Add(this.numPosY);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 385);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit";
            // 
            // picImage
            // 
            this.picImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picImage.Location = new System.Drawing.Point(89, 356);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(32, 21);
            this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picImage.TabIndex = 32;
            this.picImage.TabStop = false;
            // 
            // numLineWidth
            // 
            this.numLineWidth.Location = new System.Drawing.Point(216, 166);
            this.numLineWidth.Maximum = new decimal(new int[] {
            32768,
            0,
            0,
            0});
            this.numLineWidth.Name = "numLineWidth";
            this.numLineWidth.Size = new System.Drawing.Size(48, 21);
            this.numLineWidth.TabIndex = 31;
            this.numLineWidth.ValueChanged += new System.EventHandler(this.numLineWidth_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(178, 170);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 12);
            this.label11.TabIndex = 30;
            this.label11.Text = "Width";
            // 
            // chkText
            // 
            this.chkText.AutoSize = true;
            this.chkText.Location = new System.Drawing.Point(126, 303);
            this.chkText.Name = "chkText";
            this.chkText.Size = new System.Drawing.Size(48, 16);
            this.chkText.TabIndex = 29;
            this.chkText.Text = "없음";
            this.chkText.UseVisualStyleBackColor = true;
            this.chkText.CheckedChanged += new System.EventHandler(this.chkText_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 89);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(34, 12);
            this.label13.TabIndex = 3;
            this.label13.Text = "Type";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(89, 85);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(173, 20);
            this.cmbType.TabIndex = 2;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // pnlText
            // 
            this.pnlText.BackColor = System.Drawing.Color.White;
            this.pnlText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlText.Location = new System.Drawing.Point(88, 301);
            this.pnlText.Name = "pnlText";
            this.pnlText.Size = new System.Drawing.Size(32, 21);
            this.pnlText.TabIndex = 28;
            this.pnlText.Click += new System.EventHandler(this.pnlText_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 305);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 12);
            this.label10.TabIndex = 27;
            this.label10.Text = "Text Color";
            // 
            // btnImageSize
            // 
            this.btnImageSize.Location = new System.Drawing.Point(170, 355);
            this.btnImageSize.Name = "btnImageSize";
            this.btnImageSize.Size = new System.Drawing.Size(43, 23);
            this.btnImageSize.TabIndex = 14;
            this.btnImageSize.Text = "Size";
            this.btnImageSize.UseVisualStyleBackColor = true;
            this.btnImageSize.Click += new System.EventHandler(this.btnImageSize_Click);
            // 
            // btnImageClear
            // 
            this.btnImageClear.Location = new System.Drawing.Point(125, 355);
            this.btnImageClear.Name = "btnImageClear";
            this.btnImageClear.Size = new System.Drawing.Size(43, 23);
            this.btnImageClear.TabIndex = 14;
            this.btnImageClear.Text = "Clear";
            this.btnImageClear.UseVisualStyleBackColor = true;
            this.btnImageClear.Click += new System.EventHandler(this.btnImageClear_Click);
            // 
            // btnImage
            // 
            this.btnImage.Location = new System.Drawing.Point(214, 355);
            this.btnImage.Name = "btnImage";
            this.btnImage.Size = new System.Drawing.Size(50, 23);
            this.btnImage.TabIndex = 14;
            this.btnImage.Text = "Image";
            this.btnImage.UseVisualStyleBackColor = true;
            this.btnImage.Click += new System.EventHandler(this.btnImage_Click);
            // 
            // btnFontStyle
            // 
            this.btnFontStyle.Location = new System.Drawing.Point(215, 273);
            this.btnFontStyle.Name = "btnFontStyle";
            this.btnFontStyle.Size = new System.Drawing.Size(48, 23);
            this.btnFontStyle.TabIndex = 14;
            this.btnFontStyle.Text = "Font";
            this.btnFontStyle.UseVisualStyleBackColor = true;
            this.btnFontStyle.Click += new System.EventHandler(this.btnFontStyle_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 360);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(40, 12);
            this.label14.TabIndex = 26;
            this.label14.Text = "Image";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 278);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 12);
            this.label9.TabIndex = 26;
            this.label9.Text = "Text Style";
            // 
            // txtFontStyle
            // 
            this.txtFontStyle.Location = new System.Drawing.Point(88, 274);
            this.txtFontStyle.Name = "txtFontStyle";
            this.txtFontStyle.Size = new System.Drawing.Size(125, 21);
            this.txtFontStyle.TabIndex = 25;
            this.txtFontStyle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFontStyle_KeyDown);
            // 
            // cmbTextAlign
            // 
            this.cmbTextAlign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTextAlign.FormattingEnabled = true;
            this.cmbTextAlign.Items.AddRange(new object[] {
            "왼쪽 정렬",
            "가운데 정렬",
            "오른쪽 정렬"});
            this.cmbTextAlign.Location = new System.Drawing.Point(88, 247);
            this.cmbTextAlign.Name = "cmbTextAlign";
            this.cmbTextAlign.Size = new System.Drawing.Size(175, 20);
            this.cmbTextAlign.TabIndex = 24;
            this.cmbTextAlign.SelectedIndexChanged += new System.EventHandler(this.cmbTextAlign_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 251);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 12);
            this.label4.TabIndex = 23;
            this.label4.Text = "Text Align";
            // 
            // chkFill
            // 
            this.chkFill.AutoSize = true;
            this.chkFill.Location = new System.Drawing.Point(127, 195);
            this.chkFill.Name = "chkFill";
            this.chkFill.Size = new System.Drawing.Size(48, 16);
            this.chkFill.TabIndex = 20;
            this.chkFill.Text = "없음";
            this.chkFill.UseVisualStyleBackColor = true;
            this.chkFill.CheckedChanged += new System.EventHandler(this.chkFill_CheckedChanged);
            // 
            // chkLine
            // 
            this.chkLine.AutoSize = true;
            this.chkLine.Location = new System.Drawing.Point(127, 168);
            this.chkLine.Name = "chkLine";
            this.chkLine.Size = new System.Drawing.Size(48, 16);
            this.chkLine.TabIndex = 19;
            this.chkLine.Text = "없음";
            this.chkLine.UseVisualStyleBackColor = true;
            this.chkLine.CheckedChanged += new System.EventHandler(this.chkLine_CheckedChanged);
            // 
            // pnlFill
            // 
            this.pnlFill.BackColor = System.Drawing.Color.White;
            this.pnlFill.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFill.Location = new System.Drawing.Point(89, 193);
            this.pnlFill.Name = "pnlFill";
            this.pnlFill.Size = new System.Drawing.Size(32, 21);
            this.pnlFill.TabIndex = 18;
            this.pnlFill.Click += new System.EventHandler(this.pnlFill_Click);
            // 
            // pnlLine
            // 
            this.pnlLine.BackColor = System.Drawing.Color.White;
            this.pnlLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLine.Location = new System.Drawing.Point(89, 166);
            this.pnlLine.Name = "pnlLine";
            this.pnlLine.Size = new System.Drawing.Size(32, 21);
            this.pnlLine.TabIndex = 17;
            this.pnlLine.Click += new System.EventHandler(this.pnlLine_Click);
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(89, 58);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(174, 21);
            this.txtKey.TabIndex = 16;
            this.txtKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKey_KeyDown);
            // 
            // txtText
            // 
            this.txtText.Location = new System.Drawing.Point(89, 220);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(174, 21);
            this.txtText.TabIndex = 16;
            this.txtText.TextChanged += new System.EventHandler(this.txtText_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 197);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "Fill Color";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "Line Color";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "(Name)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 224);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "Text";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 332);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 12);
            this.label12.TabIndex = 5;
            this.label12.Text = "Arc Angle";
            // 
            // numAngle1
            // 
            this.numAngle1.Location = new System.Drawing.Point(89, 328);
            this.numAngle1.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numAngle1.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numAngle1.Name = "numAngle1";
            this.numAngle1.Size = new System.Drawing.Size(84, 21);
            this.numAngle1.TabIndex = 6;
            this.numAngle1.ValueChanged += new System.EventHandler(this.numAngle1_ValueChanged);
            // 
            // numAngle2
            // 
            this.numAngle2.Location = new System.Drawing.Point(179, 328);
            this.numAngle2.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numAngle2.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numAngle2.Name = "numAngle2";
            this.numAngle2.Size = new System.Drawing.Size(84, 21);
            this.numAngle2.TabIndex = 7;
            this.numAngle2.ValueChanged += new System.EventHandler(this.numAngle2_ValueChanged);
            // 
            // PYJ_DrawHelpDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 436);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCommit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PYJ_DrawHelpDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Property";
            this.Load += new System.EventHandler(this.frmTrackingDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numPosX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLineWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngle1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAngle2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbRectNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numPosX;
        private System.Windows.Forms.NumericUpDown numPosY;
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkLine;
        private System.Windows.Forms.Panel pnlFill;
        private System.Windows.Forms.Panel pnlLine;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkFill;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkText;
        private System.Windows.Forms.Panel pnlText;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnFontStyle;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtFontStyle;
        private System.Windows.Forms.ComboBox cmbTextAlign;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numLineWidth;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numAngle1;
        private System.Windows.Forms.NumericUpDown numAngle2;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnImage;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.PictureBox picImage;
        private System.Windows.Forms.Button btnImageClear;
        private System.Windows.Forms.Button btnImageSize;
    }
}