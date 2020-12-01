using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PYJ_DrawHelper
{
    partial class PYJ_DrawHelpDialog : Form
    {
        private DataSet ds = null;        
        private Control ctl = null;
        //private DataRow CurrentRow = null;
        private ObjectsCollection Objects = null;
        private string key = string.Empty;
        private DrawHelper dh;
        OpenFileDialog dlg = DrawHelper.gFileDlg;
        ColorDialog colorDlg = DrawHelper.gColorDlg;

        public PYJ_DrawHelpDialog(DataSet _ds, Control _ctl, ObjectsCollection _objects, string _key, DrawHelper _dh)
        {
            InitializeComponent();
            ds = _ds;
            ctl = _ctl;
            //CurrentRow = _curRow;
            Objects = _objects;
            key = _key;
            dh = _dh;

            dlg.Title = "이미지 추가";
            dlg.InitialDirectory = Application.StartupPath;
            dlg.Filter = "All Image Files|*.jpg;*.gif;*.png;*.bmp|JPEG Images|*.jpg|GIF Images|*.gif|PNG Images|*.png";
            dlg.RestoreDirectory = true;
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmTrackingDialog_Load(object sender, EventArgs e)
        {
            cmbType.Items.Add("pRectangle");
            cmbType.Items.Add("pEllipse");
            cmbType.Items.Add("pArc");
            cmbType.Items.Add("pLine");
            cmbType.Items.Add("pLineNE");
            cmbType.Items.Add("pLineNW");
            cmbType.Items.Add("pImage");
            cmbType.Items.Add("pButton");
            cmbType.Items.Add("pDiamond");
            cmbType.Items.Add("pLineV");
            cmbType.Items.Add("pRoundRect");

            for (int i = 0; i < Objects.Count; i++)
                cmbRectNo.Items.Add(Objects[i].Key);

            SetProperty(Objects[key].Key);
        }

        private void SetProperty(string key)
        {
            cmbType.SelectedIndex = (int)Objects[key].ObjectType;
            cmbRectNo.Text = Objects[key].Key;
            txtKey.Text = key;
            numPosX.Value = Convert.ToDecimal(Objects[key].X);
            numPosY.Value = Convert.ToDecimal(Objects[key].Y);
            numWidth.Value = Convert.ToDecimal(Objects[key].Width);
            numHeight.Value = Convert.ToDecimal(Objects[key].Height);
            pnlLine.BackColor = Objects[key].LineColor;
            chkLine.Checked = (Objects[key].LineColor.ToArgb() == 0) ? true : false;
            numLineWidth.Value = Objects[key].LineWidth;

            txtText.Text = Objects[key].Text;
            pnlFill.BackColor = Objects[key].FillColor;
            chkFill.Checked = (Objects[key].FillColor.ToArgb() == 0) ? true : false;
            txtFontStyle.Text = DrawHelper.FontStyleToString(Objects[key].FontStyle);
            pnlText.BackColor = Objects[key].FontColor;
            chkText.Checked = (Objects[key].FontColor.ToArgb() == 0) ? true : false;
            cmbTextAlign.SelectedIndex = DrawHelper.StringAlignToInt(Objects[key].TextAlign);

            numAngle1.Value = (decimal)Objects[key].Angle1;
            numAngle2.Value = (decimal)Objects[key].Angle2;

            picImage.Image = Objects[key].Img; 
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(cmbType.SelectedIndex)
            {
                case 1:
                    Objects[key].ObjectType = DrawObjectType.pEllipse;
                    break;
                case 2:
                    Objects[key].ObjectType = DrawObjectType.pArc;
                    break;
                case 3:
                    Objects[key].ObjectType = DrawObjectType.pLine;
                    break;
                case 4:
                    Objects[key].ObjectType = DrawObjectType.pLineNE;
                    break;
                case 5:
                    Objects[key].ObjectType = DrawObjectType.pLineNW;
                    break;
                case 6:
                    Objects[key].ObjectType = DrawObjectType.pImage;
                    break;
                case 7:
                    Objects[key].ObjectType = DrawObjectType.pButton;
                    break;
                case 8:
                    Objects[key].ObjectType = DrawObjectType.pDiamond;
                    break;
                case 9:
                    Objects[key].ObjectType = DrawObjectType.pLineV;
                    break;
                case 10:
                    Objects[key].ObjectType = DrawObjectType.pRoundRect;
                    break;
                default:
                    Objects[key].ObjectType = DrawObjectType.pRectangle;
                    break;
            }
            ctl.Invalidate();
        }

        private void cmbRectNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(cmbRectNo.Text)) return;
            key = cmbRectNo.Text;
            dh.CurrentKey = key;
            dh.CurrentID = Objects[key].Id;
            dh.SelectedObjects.Clear();
            dh.SelectedObjects.Add(key, Objects[key]);
            dh.StandardObjKey = key;
            SetProperty(dh.CurrentKey);

            ctl.Invalidate();
        }

        private void numPosX_ValueChanged(object sender, EventArgs e)
        {
            Objects[key].X = (int)numPosX.Value;
            ctl.Invalidate();
        }

        private void numPosY_ValueChanged(object sender, EventArgs e)
        {
            Objects[key].Y = (int)numPosY.Value;
            ctl.Invalidate();
        }

        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            Objects[key].Width = (int)numWidth.Value;
            ctl.Invalidate();
        }

        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            Objects[key].Height = (int)numHeight.Value;
            ctl.Invalidate();
        }

        private void numLineWidth_ValueChanged(object sender, EventArgs e)
        {
            Objects[key].LineWidth = (int)numLineWidth.Value;
            ctl.Invalidate();
        }

        private void pnlLine_Click(object sender, EventArgs e)
        {          
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                Objects[key].LineColor = colorDlg.Color;
                pnlLine.BackColor = colorDlg.Color;
                if (chkLine.Checked)
                    chkLine.Checked = false;
                ctl.Invalidate();
            }
        }

        private void pnlFill_Click(object sender, EventArgs e)
        {
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                Objects[key].FillColor = colorDlg.Color;
                pnlFill.BackColor = colorDlg.Color;
                if (chkFill.Checked)
                    chkFill.Checked = false;
                ctl.Invalidate();
            }
        }

        private void pnlText_Click(object sender, EventArgs e)
        {
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                Objects[key].FontColor = colorDlg.Color;
                pnlText.BackColor = colorDlg.Color;
                if (chkText.Checked)
                    chkText.Checked = false;
                ctl.Invalidate();
            }
        }

        private void chkText_CheckedChanged(object sender, EventArgs e)
        {
            if (chkText.Checked)
            {
                Objects[key].FontColor = Color.Empty;
                pnlText.BackColor = Color.Empty;
                ctl.Invalidate();
            }
        }

        private void chkLine_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLine.Checked)
            {
                Objects[key].LineColor = Color.Empty;
                pnlLine.BackColor = Color.Empty;
                ctl.Invalidate();
            }
        }

        private void chkFill_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFill.Checked)
            {
                Objects[key].FillColor = Color.Empty;
                pnlFill.BackColor = Color.Empty;
                ctl.Invalidate();
            }
        }


        private void btnFontStyle_Click(object sender, EventArgs e)
        {
            try
            {
                FontDialog dlg = new FontDialog();
                string[] tmp = txtFontStyle.Text.Trim().Split(':');
                if (tmp.Length == 3)
                {
                    dlg.Font = new Font(tmp[0], float.Parse(tmp[1]), DrawHelper.GetFontStyle(tmp[2]));
                }
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtFontStyle.Text = DrawHelper.FontStyleToString(dlg.Font);
                    Objects[key].FontStyle = dlg.Font;
                    ctl.Invalidate();
                }
            }
            catch 
            {
                Objects[key].FontStyle = new Font("굴림", 9, FontStyle.Regular);
            }
        }

        private void txtFontStyle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string[] tmp = txtFontStyle.Text.Trim().Split(':');
                    if (tmp.Length == 3)
                    {
                        Objects[key].FontStyle = new Font(tmp[0], float.Parse(tmp[1]), DrawHelper.GetFontStyle(tmp[2]));
                        ctl.Invalidate();
                    }
                    else
                    {
                        MessageBox.Show("폰트 형식이 잘못되었습니다.\r\n(올바른 예= \"폰트패밀리:사이즈:스타일\")");
                    }
                }
                catch
                {
                    Objects[key].FontStyle = new Font("굴림", 9, FontStyle.Regular);
                }
            }
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            Objects[key].Text = txtText.Text;
            ctl.Invalidate();
        }

        private void numAngle1_ValueChanged(object sender, EventArgs e)
        {
            Objects[key].Angle1 = (int)numAngle1.Value;
            ctl.Invalidate();
        }

        private void numAngle2_ValueChanged(object sender, EventArgs e)
        {
            Objects[key].Angle2 = (int)numAngle2.Value;
            ctl.Invalidate();
        }

        private void txtKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // 중복검사
                string newKey = txtKey.Text.Trim();
                if (key.Equals(newKey)) return;                
                if (Objects.Contain(newKey))
                {
                    MessageBox.Show(String.Format("이미 [{0}]는 오브젝트 그룹에 포함되어 있습니다.", newKey));
                    txtKey.Focus();
                    txtKey.SelectAll();
                    return;
                }

                // 기존 오브젝트를 지우고, 새로운 키값을 가진 오브젝트를 추가한다.
                // 순번(Id)는 기존 오브젝트의 순번을 그대로 가져간다.
                DrawObject obj = Objects[key];
                Objects.Remove(key);
                obj.Key = newKey;
                Objects.Add(obj, obj.Id);

                // 콤보박스 재할당
                cmbRectNo.Items.Clear();
                for (int i = 0; i < Objects.Count; i++)
                    cmbRectNo.Items.Add(Objects[i].Key);
                cmbRectNo.Text = txtKey.Text.Trim();
            }
        }

        private void cmbTextAlign_SelectedIndexChanged(object sender, EventArgs e)
        {
            Objects[key].TextAlign = DrawHelper.GetStringAlign(cmbTextAlign.SelectedIndex);
            ctl.Invalidate();
        }

        private void btnImage_Click(object sender, EventArgs e)
        {            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(dlg.FileName);
                picImage.Image = img;
                Objects[key].Img = img;
                ctl.Invalidate();
            }
        }

        private void btnImageClear_Click(object sender, EventArgs e)
        {
            picImage.Image = null;
            Objects[key].Img = null;
            ctl.Invalidate();
        }

        private void btnImageSize_Click(object sender, EventArgs e)
        {
            if (Objects[key].Img != null)
            {
                Objects[key].Width = Objects[key].Img.Width;
                Objects[key].Height = Objects[key].Img.Height;
                numWidth.Value = (decimal)Objects[key].Width;
                numHeight.Value = (decimal)Objects[key].Height;
            }
        }






    }
}