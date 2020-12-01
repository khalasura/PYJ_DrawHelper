using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PYJ_DrawHelper;


namespace Demo
{
    public partial class Form1 : Form
    {
        private DrawHelper dh;
        private DrawObject selectedObject;

        public Form1()
        {
            InitializeComponent();

            // 드로우헬퍼 생성
            dh = new DrawHelper(panel1);
            dh.Load();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dh.Objects["btnStart"].Click += new DrawObject.ObjectClick(AnimationTest);
            dh.Objects["btnStop"].Click += new DrawObject.ObjectClick(AnimationTest);

            dh.Objects["Object3"].Click += new DrawObject.ObjectClick(RotationTest);

        }

        // 회전 테스트
        private void RotationTest(object sender, EventArgs args)
        {
            DrawObject obj = (DrawObject)sender;
            if (dh.Objects["Object3"].Angle1 >= 360) 
                dh.Objects["Object3"].Angle1 = 0;
            dh.Objects["Object3"].Angle1 += 45;
        }

        // 애니메이션 테스트
        void AnimationTest(object sender, EventArgs args)
        {
            DrawObject obj = (DrawObject)sender;

            if (obj.Key == "btnStart")
            {
                // msec가 0이면 무한반복
                dh.Objects["Object1"].Animation(AnimationType.Rotation, 360, 0);
                dh.Objects["Object2"].Animation(AnimationType.Width, 500, 0);
            }
            else
            {
                // 동작 정지
                dh.Objects["Object1"].AnimationClear();
                dh.Objects["Object2"].AnimationClear();
                dh.Objects["Object2"].Width = 100;
            }
        }

        // 드로우헬퍼 실행/ 편집모드
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (btnModify.Text == "편집")
            {
                btnModify.Text = "실행";
                dh.ManagerMode = true;
            }
            else
            {
                btnModify.Text = "편집";
                dh.ManagerMode = false;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (dh.ManagerMode) return;
            var find = dh.Objects.ObjectList.ToArray().FirstOrDefault(g => ((DrawObject)g).ObjectBound.Contains(e.Location));
            if (find != null)
                selectedObject = find as DrawObject;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedObject == null) return;
            if (selectedObject.Key != "imgSunjin") return;
            selectedObject.X = e.X;
            selectedObject.Y = e.Y;
            panel1.Invalidate();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            selectedObject = null;
        }
    }
}
