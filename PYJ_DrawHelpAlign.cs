using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PYJ_DrawHelper
{
    /// <summary>
    /// 드로우헬퍼의 오브젝트 정렬 형태입니다. 
    /// </summary>
    public enum DrawAlign 
    { 
        /// <summary>
        /// 왼쪽 정렬
        /// </summary>
        Left,
        /// <summary>
        /// 가운데 정렬
        /// </summary>
        Center,
        /// <summary>
        /// 오른쪽 정렬
        /// </summary>
        Right, 
        /// <summary>
        /// 위쪽 맞춤
        /// </summary>
        Top,
        /// <summary>
        /// 중간 맞춤
        /// </summary>
        Middle,
        /// <summary>
        /// 아래쪽 맞춤
        /// </summary>
        Bottom,
        /// <summary>
        /// 같은 너비로
        /// </summary>
        Width,
        /// <summary>
        /// 같은 높이로
        /// </summary>
        Height,
        /// <summary>
        /// 같은 크기로
        /// </summary>
        Size,
        /// <summary>
        /// 가로 간격 제거
        /// </summary>
        ArrayH,
        /// <summary>
        /// 세로 간격 제거
        /// </summary>
        ArrayV,
        /// <summary>
        /// 가로 간격 같게
        /// </summary>
        SameH,
        /// <summary>
        /// 세로 간격 같게
        /// </summary>
        SameV
        
    }    


    public partial class PYJ_DrawHelpAlign : Form
    {
        private DrawHelper dh;

        public PYJ_DrawHelpAlign(DrawHelper dh)
        {
            InitializeComponent();
            this.dh = dh;
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Left);
        }

        private void btnCenter_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Center);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Right);
        }

        private void btnTop_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Top);
        }

        private void btnMiddle_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Middle);
        }

        private void btnBottom_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Bottom);
        }

        private void btnWidth_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Width);
        }

        private void btnHeight_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Height);
        }

        private void btnSize_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.Size);
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

        private void btnArrayH_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.ArrayH);
        }

        private void btnArrayV_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.ArrayV);
        }

        private void btnSameH_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.SameH);
        }

        private void btnSameV_Click(object sender, EventArgs e)
        {
            dh.ObjectAlign(DrawAlign.SameV);
        }
    }
}
