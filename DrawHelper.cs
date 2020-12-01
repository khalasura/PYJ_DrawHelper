using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace PYJ_DrawHelper
{
    public class DrawHelper
    {
        #region 선언
        // 드로잉 오브젝트의 속성을 저장할 DataSet
        private DataSet ds = new DataSet();
        // XML파일 경로 
        private string _XmlFileName = Application.StartupPath + "\\Object.XML";
        // Manager Mode
        private bool _ManagerMode = false;
        // 캔퍼스가 되는 Control
        private Control ctl;
        // Start Point (Edit Mode시)
        private Point stPoint;
        // Edit Point
        private Rectangle[,] EditRect;
        // Edit Mode
        private bool bEditMode = false;
        // Edit Type
        private int EditType = -2;
        // Context Menu
        private ContextMenuStrip cMenu;
        private ContextMenuStrip cMenu2;
        // 다중선택 플래그
        private bool bMultiSelect = false;
        // Index 최소값 상수
        private const int CUR_MIN = -2147483648;
        // 복사된 객체        
        private Hashtable CopyedObjects = new Hashtable();
        // 에디트 시작 객체 정보
        private Hashtable htStartInfo = new Hashtable();
        // 가이드 사각형
        private Rectangle GuideRect = new Rectangle();
        private bool isDrag = false;
        // 오브젝트 속성이 수정되었는가?
        private bool isModify = false;
        // 실행취소, 다시실행 기능 관련
        private int doSize = 10;
        private ObjectsCollection[] arrDataSet;
        private int DoIndex;
        // 파일 다이얼로그
        static public OpenFileDialog gFileDlg = new OpenFileDialog();
        // 칼라 다이얼로그
        static public ColorDialog gColorDlg = new ColorDialog();
        // 타이머
        private Timer timer = new Timer();
        public int TimerTick = 100; // 실행모드일시 그리기 틱

        // 
        private DrawLinkage dLink = new DrawLinkage();
        private Rectangle[] LinkRect = new Rectangle[4];

        /// <summary>
        /// 현재 선택된 오브젝트의 그리기 우선순위 (숫자가 클수록 앞에 그려집니다.)
        /// </summary>
        public int CurrentID = CUR_MIN;
        /// <summary>
        /// 현재 선택된 오브젝트 키
        /// </summary>
        public string CurrentKey = string.Empty;
        /// <summary>
        /// 오브젝트 정보를 저장하는 XML파일 경로 (기본경로: Application.StartupPath + "\\Object.XML")
        /// </summary>
        public string XmlFileName
        {
            get { return _XmlFileName; }
            set { _XmlFileName = value; }
        }
        /// <summary>
        /// 관리자모드 (True로 설정시 런타임 중 오브젝트를 추가,수정,삭제 할 수 있습니다.)
        /// </summary>
        public bool ManagerMode
        {
            get { return _ManagerMode; }
            set
            {
                _ManagerMode = value;
                if (!_ManagerMode)
                {
                    // 매니저모드에서 실행모드로 변환시 초기화
                    CurrentID = CUR_MIN;
                    CurrentKey = string.Empty;
                    SelectedObjects.Clear();
                    htStartInfo.Clear();
                    EditType = -2;
                    // 저장
                    XmlFileUpdate();
                    // 실행취소버퍼 클리어
                    Array.Clear(arrDataSet, 0, arrDataSet.Length);
                }
                Refresh();
            }
        }
        /// <summary>
        /// 오브젝트를 관리하는 오브젝트 컬렉션 클래스
        /// </summary>
        public ObjectsCollection Objects = new ObjectsCollection();
        /// <summary>
        /// 선택된 오브젝트가 저장된 해쉬테이블
        /// </summary>
        public Hashtable SelectedObjects = new Hashtable();
        /// <summary>
        /// 다중선택시 기준이 되는 오브젝트 키
        /// </summary>
        public string StandardObjKey = string.Empty;
        #endregion

        #region Event
        #endregion

        #region 생성자
        /// <summary>
        /// PYJ 드로우헬퍼 생성자
        /// </summary>
        /// <param name="_ctl">Window Form, Picture Box 등등 Control Class를 상속 받는 모든 Class를 인자로 받는다.</param>
        public DrawHelper(Control _ctl)
        {
            // 실행 버퍼 사이즈
            arrDataSet = new ObjectsCollection[doSize];
            DoIndex = doSize - 1;

            // 폼객체 할당
            ctl = _ctl;

            // DataTable 로드
            //Load();

            // 이벤트 등록
            ctl = _ctl;
            ctl.MouseMove += new MouseEventHandler(ctl_MouseMove);
            ctl.MouseDown += new MouseEventHandler(ctl_MouseDown);
            ctl.MouseUp += new MouseEventHandler(ctl_MouseUp);  
            ctl.MouseDoubleClick += new MouseEventHandler(ctl_MouseDoubleClick);
            ctl.MouseClick += new MouseEventHandler(ctl_MouseClick);
            if (ctl.GetType().BaseType.Name != "Form")
            {
                Form parent = getParentFrom(ctl);
                if (parent != null)
                {
                    parent.KeyDown += new KeyEventHandler(ctl_KeyDown);
                    parent.KeyUp += new KeyEventHandler(ctl_KeyUp);
                }
            }
            else
            {
                ctl.KeyDown += new KeyEventHandler(ctl_KeyDown);
                ctl.KeyUp += new KeyEventHandler(ctl_KeyUp);
            }

            if (ctl.GetType().Name == "Panel")
            {
                typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null,
                                            ctl, new object[] { true }); 
            }
            Objects.AfterAdd += new ObjectsCollection.ObjectAdded(Objects_AfterAdd);
            Objects.AfterRemove += new ObjectsCollection.ObjectRemoved(Objects_AfterRemove);
            ctl.Paint += new PaintEventHandler(ctl_Paint);

            // Context 메뉴 등록 (오브젝트 선택시)
            cMenu = new ContextMenuStrip();
            cMenu.Items.Add(new ToolStripMenuItem("모두선택"));//0
            cMenu.Items.Add(new ToolStripSeparator());//1
            cMenu.Items.Add(new ToolStripMenuItem("삭제"));//2
            cMenu.Items.Add(new ToolStripMenuItem("복사"));//3
            cMenu.Items.Add(new ToolStripMenuItem("정렬"));//4
            cMenu.Items.Add(new ToolStripSeparator());//5
            cMenu.Items.Add(new ToolStripMenuItem("맨 앞으로 보내긔"));//6
            cMenu.Items.Add(new ToolStripMenuItem("맨 뒤로 보내긔"));//7
            cMenu.Items.Add(new ToolStripSeparator());//8
            cMenu.Items.Add(new ToolStripMenuItem("스타일 동일하게 적용"));//9
            cMenu.Items.Add(new ToolStripMenuItem("속성"));//10
            //cMenu.Items.Add(new ToolStripMenuItem("연결"));//11
            cMenu.Items[0].Click += new EventHandler(go_SelectAll);
            cMenu.Items[2].Click += new EventHandler(go_RemoveObject);
            cMenu.Items[3].Click += new EventHandler(go_CopyObject);
            cMenu.Items[4].Click += new EventHandler(go_AlignObject);
            cMenu.Items[6].Click += new EventHandler(go_forward);
            cMenu.Items[7].Click += new EventHandler(go_back);
            cMenu.Items[9].Click += new EventHandler(go_style);
            cMenu.Items[10].Click += new EventHandler(go_property);
            //cMenu.Items[11].Click += new EventHandler(go_Linkage);

            // Context 메뉴 등록 (오브젝트 미 선택시)
            cMenu2 = new ContextMenuStrip();
            cMenu2.Items.Add(new ToolStripMenuItem("모두선택"));//0
            cMenu2.Items.Add(new ToolStripSeparator());//1
            cMenu2.Items.Add(new ToolStripMenuItem("추가"));//2
            cMenu2.Items.Add(new ToolStripMenuItem("붙여넣기"));//3
            cMenu2.Items.Add(new ToolStripSeparator());//4
            cMenu2.Items.Add(new ToolStripMenuItem("실행취소"));//5
            cMenu2.Items.Add(new ToolStripMenuItem("다시실행"));//6
            cMenu2.Items[0].Click += new EventHandler(go_SelectAll);
            cMenu2.Items[2].Click += new EventHandler(go_AddObject);
            cMenu2.Items[3].Click += new EventHandler(go_PasteObject);
            cMenu2.Items[5].Click += new EventHandler(go_UnDo);
            cMenu2.Items[6].Click += new EventHandler(go_ReDo);
        }

        private Form getParentFrom(Control _ctl)
        {
            if (_ctl == null)
            {
                return null;
            }
            else if (_ctl.GetType().BaseType.Name == "Form")
            {
                return _ctl as Form; 
            }
            return getParentFrom(_ctl.Parent);
        }

        void go_Linkage(object sender, EventArgs e)
        {
            dLink.Mode = LinkMode.Ready;
            Refresh();
        }

        // 타이머
        void timer_Tick(object sender, EventArgs e)
        {
            if (ManagerMode) return;           
            ctl.Invalidate();                          
        }
        #endregion

        #region [Event] Context Menu 이벤트
        // 정렬
        private void go_AlignObject(object sender, EventArgs e)
        {
            ShowAlingDialog();
        }
        // 복사
        private void go_CopyObject(object sender, EventArgs e)
        {
            ObjectCopy();
            Refresh();
        }
        // 삭제
        private void go_RemoveObject(object sender, EventArgs e)
        {
            ObjectRemove();
            Refresh();
        }
        // 모두선택
        private void go_SelectAll(object sender, EventArgs e)
        {
            ObjectSelectAll();
            Refresh();
        }
        // 붙여넣기
        private void go_PasteObject(object sender, EventArgs e)
        {
            ObjectPaste();
            Refresh();
        }
        // 추가
        private void go_AddObject(object sender, EventArgs e)
        {
            ObjectAdd();
            Refresh();
        }
        // 맨앞으로
        private void go_forward(object sender, EventArgs e)
        {
            if (CurrentID != CUR_MIN)
            {
                Set_Forward(CurrentID);
            }
            Refresh();
        }
        // 맨뒤로
        private void go_back(object sender, EventArgs e)
        {
            if (CurrentID != CUR_MIN)
            {
                Set_Backward(CurrentID);
            }
            Refresh();
        }
        // 속성
        private void go_property(object sender, EventArgs e)
        {
            ShowProperty();
        }
        // 다시실행
        void go_ReDo(object sender, EventArgs e)
        {
            ReDo();
        }
        // 실행취소
        void go_UnDo(object sender, EventArgs e)
        {
            UnDo();
        }
        // 스타일 동일하게 적용
        void go_style(object sender, EventArgs e)
        {
            if (SelectedObjects.Count < 2) return;
            IDictionaryEnumerator iEnum = SelectedObjects.GetEnumerator();
            while (iEnum.MoveNext())
            {
                DrawObject obj = (DrawObject)iEnum.Value;
                obj.LineColor = Objects[StandardObjKey].LineColor;
                obj.LineWidth = Objects[StandardObjKey].LineWidth;
                obj.FillColor = Objects[StandardObjKey].FillColor;
                obj.FontColor = Objects[StandardObjKey].FontColor;
                obj.FontStyle = Objects[StandardObjKey].FontStyle;
            }
            XmlFileUpdate();

            // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
            DataSetShift(Objects);
            Refresh();
        }
        #endregion

        #region [Event] Control 이벤트
        // 컨트롤 Paint 이벤트
        private void ctl_Paint(object sender, PaintEventArgs e)
        {
            Display(e.Graphics);
        }

        private void Display(Graphics g)
        {
            // 드로우 스무스 모드
            g.SmoothingMode = SmoothingMode.HighQuality;            
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Id순으로 정렬
            Objects.CollectionSort();

            Pen pEdit = new Pen(Color.Green, 1);
            Pen pBox = new Pen(Color.Blue, 1);
            Pen pGuide = new Pen(Color.Black, 1);
            Brush fill = Brushes.Black;
            pEdit.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            pGuide.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            EditRect = new Rectangle[Objects.Count, 8];

            // 오브젝트 그리기
            for (int i = 0; i < Objects.Count; i++)
            {                
                //if (Objects[i].Rotate == RotationType.Right)
                //    Objects[i].RotationAngle = Objects[i].RotationAngle + Objects[i].RotationSpeed;
                //else if (Objects[i].Rotate == RotationType.Left)
                //    Objects[i].RotationAngle = Objects[i].RotationAngle - Objects[i].RotationSpeed;
                //else
                //    Objects[i].RotationAngle = Objects[i].Angle1;

                // 애니메이션
                if (Objects[i].AnimationType != AnimationType.None)
                {
                    timer.Interval = 10;
                    int frame = Objects[i].AnimationSetMsec > 0 ? Objects[i].AnimationSetMsec / 10 : 100;
                    float pad = (float)(Objects[i].AnimationToValue - Objects[i].AnimationFromValue) / (float)frame;
                    switch (Objects[i].AnimationType)
                    {
                        case AnimationType.Move_X:
                            if (Math.Abs(Objects[i].AnimationToValue -Objects[i].AnimationActValue) <= Math.Abs(pad))
                            {
                                Objects[i].X = Objects[i].AnimationToValue;
                                if (Objects[i].AnimationSetMsec == 0)
                                {
                                    Objects[i].X = Objects[i].AnimationFromValue;
                                    Objects[i].AnimationActValue = Objects[i].AnimationFromValue;
                                }
                                else
                                    Objects[i].AnimationClear();
                                timer.Interval = TimerTick;
                                break;
                            }
                            Objects[i].AnimationActValue += pad;
                            Objects[i].X = (int)Objects[i].AnimationActValue;
                            break;
                        case AnimationType.Move_Y:
                            if (Math.Abs(Objects[i].AnimationToValue - Objects[i].AnimationActValue) <= Math.Abs(pad))
                            {
                                Objects[i].Y = Objects[i].AnimationToValue;
                                if (Objects[i].AnimationSetMsec == 0)
                                {
                                    Objects[i].Y = Objects[i].AnimationFromValue;
                                    Objects[i].AnimationActValue = Objects[i].AnimationFromValue;
                                }
                                else
                                    Objects[i].AnimationClear();
                                timer.Interval = TimerTick;
                                break;
                            }
                            Objects[i].AnimationActValue += pad;
                            Objects[i].Y = (int)Objects[i].AnimationActValue;
                            break;
                        case AnimationType.Width:
                            if (Math.Abs(Objects[i].AnimationToValue - Objects[i].AnimationActValue) <= Math.Abs(pad))
                            {
                                Objects[i].Width = Objects[i].AnimationToValue;
                                if (Objects[i].AnimationSetMsec == 0)
                                {
                                    Objects[i].Width = Objects[i].AnimationFromValue;
                                    Objects[i].AnimationActValue = Objects[i].AnimationFromValue;
                                }
                                else
                                    Objects[i].AnimationClear();
                                timer.Interval = TimerTick;
                                break;
                            }
                            Objects[i].AnimationActValue += pad;
                            Objects[i].Width = (int)Objects[i].AnimationActValue;
                            break;
                        case AnimationType.Height:
                            if (Math.Abs(Objects[i].AnimationToValue - Objects[i].AnimationActValue) <= Math.Abs(pad))
                            {
                                Objects[i].Height = Objects[i].AnimationToValue;
                                if (Objects[i].AnimationSetMsec == 0)
                                {
                                    Objects[i].Height = Objects[i].AnimationFromValue;
                                    Objects[i].AnimationActValue = Objects[i].AnimationFromValue;
                                }
                                else
                                    Objects[i].AnimationClear();
                                timer.Interval = TimerTick;
                                break;
                            }
                            Objects[i].AnimationActValue += pad;
                            Objects[i].Height = (int)Objects[i].AnimationActValue;
                            break;
                        case AnimationType.Rotation:
                            if (Math.Abs(Objects[i].AnimationToValue - Objects[i].AnimationActValue) <= Math.Abs(pad))
                            {
                                Objects[i].RotationAngle = Objects[i].AnimationToValue;
                                if (Objects[i].AnimationSetMsec == 0)
                                {
                                    Objects[i].RotationAngle = Objects[i].AnimationFromValue;
                                    Objects[i].AnimationActValue = Objects[i].AnimationFromValue;
                                }
                                else
                                    Objects[i].AnimationClear();
                                timer.Interval = TimerTick;
                                break;
                            }
                            Objects[i].AnimationActValue += pad;
                            Objects[i].RotationAngle = (int)Objects[i].AnimationActValue;
                            break;                            
                        case AnimationType.Arc:
                            if (Math.Abs(Objects[i].AnimationToValue - Objects[i].AnimationActValue) <= Math.Abs(pad))
                            {
                                Objects[i].Angle2 = Objects[i].AnimationToValue;
                                if (Objects[i].AnimationSetMsec == 0)
                                {
                                    Objects[i].Angle2 = Objects[i].AnimationFromValue;
                                    Objects[i].AnimationActValue = Objects[i].AnimationFromValue;
                                }
                                else
                                    Objects[i].AnimationClear();
                                timer.Interval = TimerTick;
                                break;
                            }
                            Objects[i].AnimationActValue += pad;
                            Objects[i].Angle2 = (int)Objects[i].AnimationActValue;
                            break;
                    }
                }
                else
                    Objects[i].RotationAngle = Objects[i].Angle1;


                // 오브젝트 드로잉 메소드 호출
                Objects[i].Drawing(g, _ManagerMode);

                g.PixelOffsetMode = PixelOffsetMode.Default;
                switch (dLink.Mode)
                {
                    case LinkMode.None:
                        // Edit Box 그리기
                        if (SelectedObjects.Contains(Objects[i].Key))
                        {
                            if (Objects[i].Key == StandardObjKey)
                            {
                                pBox = new Pen(Color.Blue, 1);
                                fill = Brushes.White;
                            }
                            else
                            {
                                pBox = new Pen(Color.White, 1);
                                fill = Brushes.Black;
                            }

                            // Edit rect
                            g.DrawRectangle(pEdit, Objects[i].X - 2, Objects[i].Y - 2, Objects[i].Width + 4, Objects[i].Height + 4);

                            // Edit Box rect
                            // [0]----[1]----[2]
                            //  -             -
                            //  -             -
                            // [3]-----------[4]
                            //  -             -
                            //  -             -
                            // [5]----[6]----[7]
                            //
                            // Set Edit Rect 1~8
                            EditRect[i, 0] = new Rectangle(Objects[i].X - 4, Objects[i].Y - 4, 4, 4);
                            EditRect[i, 1] = new Rectangle(Objects[i].X - 4 + (Objects[i].Width / 2 + 2), Objects[i].Y - 4, 4, 4);
                            EditRect[i, 2] = new Rectangle(Objects[i].X + (Objects[i].Width), Objects[i].Y - 4, 4, 4);
                            EditRect[i, 3] = new Rectangle(Objects[i].X - 4, Objects[i].Y - 4 + (Objects[i].Height / 2 + 2), 4, 4);
                            EditRect[i, 4] = new Rectangle(Objects[i].X + (Objects[i].Width), Objects[i].Y - 4 + (Objects[i].Height / 2 + 2), 4, 4);
                            EditRect[i, 5] = new Rectangle(Objects[i].X - 4, Objects[i].Y + (Objects[i].Height), 4, 4);
                            EditRect[i, 6] = new Rectangle(Objects[i].X - 4 + (Objects[i].Width / 2 + 2), Objects[i].Y + (Objects[i].Height), 4, 4);
                            EditRect[i, 7] = new Rectangle(Objects[i].X + (Objects[i].Width), Objects[i].Y + (Objects[i].Height), 4, 4);

                            //// pLine과 다른 오브젝트들과의 EditBox를 다르게 설정한다.
                            //if (Objects[i].ObjectType == DrawObjectType.pLine)
                            //{
                            //    g.FillRectangle(fill, EditRect[i,3]);
                            //    g.FillRectangle(fill, EditRect[i,4]);
                            //    g.DrawRectangle(pBox, EditRect[i, 3]);
                            //    g.DrawRectangle(pBox, EditRect[i, 4]);
                            //}
                            //else
                            //{
                            for (int j = 0; j < 8; j++)
                            {
                                g.FillRectangle(fill, EditRect[i, j]);
                                g.DrawRectangle(pBox, EditRect[i, j]);
                            }
                            //}
                        }
                        break;
                    case LinkMode.Ready:
                        // Link Box rect
                        // [-]----[0]----[-]
                        //  -             -
                        //  -             -
                        // [1]-----------[2]
                        //  -             -
                        //  -             -
                        // [-]----[3]----[-]
                        //
                        LinkRect[0] = new Rectangle(Objects[CurrentKey].X - 4 + (Objects[CurrentKey].Width / 2 + 2), Objects[CurrentKey].Y - 3, 6, 6);
                        LinkRect[1] = new Rectangle(Objects[CurrentKey].X - 3, Objects[CurrentKey].Y - 4 + (Objects[CurrentKey].Height / 2 + 2), 6, 6);
                        LinkRect[2] = new Rectangle(Objects[CurrentKey].X + (Objects[CurrentKey].Width) - 3, Objects[CurrentKey].Y - 4 + (Objects[CurrentKey].Height / 2 + 2), 6, 6);
                        LinkRect[3] = new Rectangle(Objects[CurrentKey].X - 4 + (Objects[CurrentKey].Width / 2 + 2), Objects[CurrentKey].Y + (Objects[CurrentKey].Height) - 3, 6, 6);
                        for (int j = 0; j < 4; j++)
                        {
                            g.FillEllipse(Brushes.White, LinkRect[j]);
                            g.DrawEllipse(new Pen(Color.Red, 1), LinkRect[j]);
                        }
                        break;
                    case LinkMode.Linking:
                        break;
                }
            }

            // Guid 그리기
            if (!GuideRect.IsEmpty)
                g.DrawRectangle(pEdit, GuideRect);

            
        }

        private void DisplayEachLayer(Graphics g, int i)
        {
            Pen pEdit = new Pen(Color.Green, 1);
            Pen pBox = new Pen(Color.Blue, 1);
            Pen pGuide = new Pen(Color.Black, 1);
            Brush fill = Brushes.Black;
            pEdit.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            pGuide.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            EditRect = new Rectangle[Objects.Count, 8];

            // 오브젝트 드로잉 메소드 호출
            if (Objects[i].Rotate == RotationType.Right)
                Objects[i].RotationAngle = Objects[i].RotationAngle + Objects[i].RotationSpeed;
            else if (Objects[i].Rotate == RotationType.Left)
                Objects[i].RotationAngle = Objects[i].RotationAngle - Objects[i].RotationSpeed;
            else
                Objects[i].RotationAngle = 0;

            //if (Objects[i].bChanged)
                Objects[i].Drawing(g, _ManagerMode);

            switch (dLink.Mode)
            {
                case LinkMode.None:
                    // Edit Box 그리기
                    if (SelectedObjects.Contains(Objects[i].Key))
                    {
                        if (Objects[i].Key == StandardObjKey)
                        {
                            pBox = new Pen(Color.Blue, 1);
                            fill = Brushes.White;
                        }
                        else
                        {
                            pBox = new Pen(Color.White, 1);
                            fill = Brushes.Black;
                        }

                        // Edit rect
                        g.DrawRectangle(pEdit, Objects[i].X - 2, Objects[i].Y - 2, Objects[i].Width + 4, Objects[i].Height + 4);

                        // Edit Box rect
                        // [0]----[1]----[2]
                        //  -             -
                        //  -             -
                        // [3]-----------[4]
                        //  -             -
                        //  -             -
                        // [5]----[6]----[7]
                        //
                        // Set Edit Rect 1~8
                        EditRect[i, 0] = new Rectangle(Objects[i].X - 4, Objects[i].Y - 4, 4, 4);
                        EditRect[i, 1] = new Rectangle(Objects[i].X - 4 + (Objects[i].Width / 2 + 2), Objects[i].Y - 4, 4, 4);
                        EditRect[i, 2] = new Rectangle(Objects[i].X + (Objects[i].Width), Objects[i].Y - 4, 4, 4);
                        EditRect[i, 3] = new Rectangle(Objects[i].X - 4, Objects[i].Y - 4 + (Objects[i].Height / 2 + 2), 4, 4);
                        EditRect[i, 4] = new Rectangle(Objects[i].X + (Objects[i].Width), Objects[i].Y - 4 + (Objects[i].Height / 2 + 2), 4, 4);
                        EditRect[i, 5] = new Rectangle(Objects[i].X - 4, Objects[i].Y + (Objects[i].Height), 4, 4);
                        EditRect[i, 6] = new Rectangle(Objects[i].X - 4 + (Objects[i].Width / 2 + 2), Objects[i].Y + (Objects[i].Height), 4, 4);
                        EditRect[i, 7] = new Rectangle(Objects[i].X + (Objects[i].Width), Objects[i].Y + (Objects[i].Height), 4, 4);

                        //// pLine과 다른 오브젝트들과의 EditBox를 다르게 설정한다.
                        //if (Objects[i].ObjectType == DrawObjectType.pLine)
                        //{
                        //    g.FillRectangle(fill, EditRect[i,3]);
                        //    g.FillRectangle(fill, EditRect[i,4]);
                        //    g.DrawRectangle(pBox, EditRect[i, 3]);
                        //    g.DrawRectangle(pBox, EditRect[i, 4]);
                        //}
                        //else
                        //{
                        for (int j = 0; j < 8; j++)
                        {
                            g.FillRectangle(fill, EditRect[i, j]);
                            g.DrawRectangle(pBox, EditRect[i, j]);
                        }
                        //}
                    }
                    break;
                case LinkMode.Ready:
                    // Link Box rect
                    // [-]----[0]----[-]
                    //  -             -
                    //  -             -
                    // [1]-----------[2]
                    //  -             -
                    //  -             -
                    // [-]----[3]----[-]
                    //
                    LinkRect[0] = new Rectangle(Objects[CurrentKey].X - 4 + (Objects[CurrentKey].Width / 2 + 2), Objects[CurrentKey].Y - 3, 6, 6);
                    LinkRect[1] = new Rectangle(Objects[CurrentKey].X - 3, Objects[CurrentKey].Y - 4 + (Objects[CurrentKey].Height / 2 + 2), 6, 6);
                    LinkRect[2] = new Rectangle(Objects[CurrentKey].X + (Objects[CurrentKey].Width) - 3, Objects[CurrentKey].Y - 4 + (Objects[CurrentKey].Height / 2 + 2), 6, 6);
                    LinkRect[3] = new Rectangle(Objects[CurrentKey].X - 4 + (Objects[CurrentKey].Width / 2 + 2), Objects[CurrentKey].Y + (Objects[CurrentKey].Height) - 3, 6, 6);
                    for (int j = 0; j < 4; j++)
                    {
                        g.FillEllipse(Brushes.White, LinkRect[j]);
                        g.DrawEllipse(new Pen(Color.Red, 1), LinkRect[j]);
                    }
                    break;
                case LinkMode.Linking:
                    break;
            }


            // Guid 그리기
            if (!GuideRect.IsEmpty)
                g.DrawRectangle(pEdit, GuideRect);
        }

        // 컨트롤 KeyDown 이벤트
        private void ctl_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_ManagerMode) return;

            if (e.KeyCode == Keys.ShiftKey)
                bMultiSelect = true;
        }

        // 컨트롤 KeyUp 이벤트
        private void ctl_KeyUp(object sender, KeyEventArgs e)
        {
            if (!_ManagerMode) return; 

            // 객체가 선택되지 않았을때도 동작되는 기능
            //
            // 객체 추가
            if (e.KeyCode == Keys.Insert)
            {
                ObjectAdd();
            }
            // 붙여넣기
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                ObjectPaste();
            }
            // 정렬
            else if (e.KeyCode == Keys.T && e.Modifiers == Keys.Control)
            {
                ShowAlingDialog();
            }
            // 실행취소
            else if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
            {
                UnDo();
            }
            // 다시실행
            else if (e.KeyCode == Keys.Y && e.Modifiers == Keys.Control)
            {
                ReDo();
            }
            // 모두선택
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                ObjectSelectAll();
            }
            else if (e.KeyCode == Keys.ShiftKey)
                bMultiSelect = false;

            // 객체가 선택 되었을때만동작 되는 기능
            //
            if (SelectedObjects.Count > 0)
            {
                IDictionaryEnumerator iEnum = SelectedObjects.GetEnumerator();

                // 복사
                if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
                {
                    ObjectCopy();
                }

                // 크기조절
                else if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Shift)
                {
                    while (iEnum.MoveNext())
                    {
                        DrawObject obj = (DrawObject)iEnum.Value;
                        obj.Height = obj.Height - 1;
                        // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                        DataSetShift(Objects);
                    }
                }
                else if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Shift)
                {
                    while (iEnum.MoveNext())
                    {
                        DrawObject obj = (DrawObject)iEnum.Value;
                        obj.Height = obj.Height + 1;
                        // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                        DataSetShift(Objects);
                    }
                }
                else if (e.KeyCode == Keys.Left && e.Modifiers == Keys.Shift)
                {
                    while (iEnum.MoveNext())
                    {
                        DrawObject obj = (DrawObject)iEnum.Value;
                        obj.Width = obj.Width - 1;
                        // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                        DataSetShift(Objects);
                    }
                }
                else if (e.KeyCode == Keys.Right && e.Modifiers == Keys.Shift)
                {
                    while (iEnum.MoveNext())
                    {
                        DrawObject obj = (DrawObject)iEnum.Value;
                        obj.Width = obj.Width + 1;
                        // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                        DataSetShift(Objects);
                    }
                }
                else
                {
                    switch (e.KeyCode)
                    {
                        // 삭제
                        case Keys.Delete:
                            ObjectRemove();
                            break;
                        //이동
                        case Keys.Up:
                            while (iEnum.MoveNext())
                            {
                                DrawObject obj = (DrawObject)iEnum.Value;
                                obj.Y = obj.Y - 1;
                                // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                                DataSetShift(Objects);
                            }
                            break;
                        case Keys.Down:
                            while (iEnum.MoveNext())
                            {
                                DrawObject obj = (DrawObject)iEnum.Value;
                                obj.Y = obj.Y + 1;
                                // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                                DataSetShift(Objects);
                            }
                            break;
                        case Keys.Left:
                            while (iEnum.MoveNext())
                            {
                                DrawObject obj = (DrawObject)iEnum.Value;
                                obj.X = obj.X - 1;
                                // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                                DataSetShift(Objects);
                            }
                            break;
                        case Keys.Right:
                            while (iEnum.MoveNext())
                            {
                                DrawObject obj = (DrawObject)iEnum.Value;
                                obj.X = obj.X + 1;
                                // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                                DataSetShift(Objects);
                            }
                            break;
                    }
                }
                XmlFileUpdate();
            }

            Refresh();
        }

        // 컨트롤 MouseClick 이벤트
        private void ctl_MouseClick(object sender, MouseEventArgs e)
        {
            if (_ManagerMode) return;
            CurrentID = CUR_MIN;
            CurrentKey = string.Empty;

            Objects.CollectionReverseSort();
            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].ObjectBound.Contains(e.Location))
                {
                    CurrentID = Objects[i].Id;
                    CurrentKey = Objects[i].Key;
                    if (Objects[i].ObjectType == DrawObjectType.pButton)
                        Objects[i].LineColor = Color.Gray;
                    Objects[i].FireClickEvent();
                    break;
                }
            }
        }

        // 컨트롤 MouseDoubleClick 이벤트
        private void ctl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowProperty();
        }

        // 컨트롤 MouseDown 이벤트
        private void ctl_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_ManagerMode)
            {
                Objects.CollectionReverseSort();
                for (int i = 0; i < Objects.Count; i++)
                {
                    Objects[i].bFocus = false;
                }
                for (int i = 0; i < Objects.Count; i++)
                {                    
                    if (Objects[i].ObjectBound.Contains(e.Location))
                    {                        
                        if (Objects[i].ObjectType == DrawObjectType.pButton)
                        {
                            isDrag = true;
                            Objects[i].bFocus = true;
                            Objects[i].LineColor = Color.Blue;
                        }
                        break;
                    }
                }
                return;
            }

            // Guide Rect
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                GuideRect.Location = ((Control)sender).PointToScreen(new Point(e.X, e.Y));
            }

            // Edit Type
            EditType = -2;
            for (int i = 0; i < Objects.Count; i++)
            {
                if (EditRect[i, 0].Contains(e.Location))
                {
                    EditType = 0;
                    break;
                }
                else if (EditRect[i, 1].Contains(e.Location))
                {
                    EditType = 1;
                    break;
                }
                else if (EditRect[i, 2].Contains(e.Location))
                {
                    EditType = 2;
                    break;
                }
                else if (EditRect[i, 3].Contains(e.Location))
                {
                    EditType = 3;
                    break;
                }
                else if (EditRect[i, 4].Contains(e.Location))
                {
                    EditType = 4;
                    break;
                }
                else if (EditRect[i, 5].Contains(e.Location))
                {
                    EditType = 5;
                    break;
                }
                else if (EditRect[i, 6].Contains(e.Location))
                {
                    EditType = 6;
                    break;
                }
                else if (EditRect[i, 7].Contains(e.Location))
                {
                    EditType = 7;
                    break;
                }
            }


            if (CurrentID != CUR_MIN && EditType >= 0)
            {
                // 커서가 화살표이면 시작포인트 시작객체를 초기화한다.
                if (ctl.Cursor == Cursors.Arrow)
                {
                    CurrentID = CUR_MIN;
                    CurrentKey = string.Empty;
                    SelectedObjects.Clear();
                    htStartInfo.Clear();
                    stPoint = new Point();
                    //stRect = new Rectangle();
                }
            }
            else
            {
                // 커서가 화살표이거나 사이즈올이면 시작포인트 시작객체를 초기화한다.
                InitStartObject();

                // CurrentID, SelectedObjects, stRect 구하기 
                int cnt = 0;
                Objects.CollectionReverseSort(); // 역순으로 오브젝트 정렬
                for (int i = 0; i < Objects.Count; i++)
                {
                    if (Objects[i].ObjectBound.Contains(e.Location))
                    {
                        cnt++;
                        CurrentID = Objects[i].Id;
                        CurrentKey = Objects[i].Key;
                        EditType = -1;
                        
                        if (bMultiSelect)
                        {
                            if (SelectedObjects.Count == 0)
                            {
                                // 기준 오브젝트 산출
                                StandardObjKey = CurrentKey;
                                // 선택되지 않았다면 해쉬테이블에 추가
                                SelectedObjects.Add(CurrentKey, Objects[i]);
                                htStartInfo.Add(CurrentKey, new Rectangle(Objects[i].X, Objects[i].Y, Objects[i].Width, Objects[i].Height));
                                break;
                            }
                            else
                            {
                                // 다중선택 모드시 이미 선택되었다면 선택취소 (해쉬테이블에서 제거)
                                if (SelectedObjects.Contains(CurrentKey))
                                {
                                    SelectedObjects.Remove(CurrentKey);
                                    htStartInfo.Remove(CurrentKey);
                                    break;
                                }
                                else
                                {
                                    // 선택되지 않았다면 해쉬테이블에 추가
                                    SelectedObjects.Add(CurrentKey, Objects[i]);
                                    htStartInfo.Add(CurrentKey, new Rectangle(Objects[i].X, Objects[i].Y, Objects[i].Width, Objects[i].Height));
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // 이미 다중선택이 되어있다면 현재선택된 객체를 기준오브젝트로 산출
                            StandardObjKey = CurrentKey;

                            if (SelectedObjects.Count > 1)
                            {
                                // 다중선택된 오브젝트가 아닌 다른 오브젝트를 선택하면 SelectedObjects를 초기화하고 현재 선택된 객체만 추가
                                if (!SelectedObjects.Contains(CurrentKey))
                                {
                                    SelectedObjects.Clear();
                                    htStartInfo.Clear();
                                    SelectedObjects.Add(CurrentKey, Objects[i]);
                                    htStartInfo.Add(CurrentKey, new Rectangle(Objects[i].X, Objects[i].Y, Objects[i].Width, Objects[i].Height));
                                    break;
                                }
                                else
                                    break;
                            }
                            else
                            {
                                // 다중선택시가 아니면 SelectedObjects를 초기화하고 현재 선택된 객체만 추가
                                SelectedObjects.Clear();
                                htStartInfo.Clear();
                                SelectedObjects.Add(CurrentKey, Objects[i]);
                                htStartInfo.Add(CurrentKey, new Rectangle(Objects[i].X, Objects[i].Y, Objects[i].Width, Objects[i].Height));
                                break;
                            }
                        }                                                                       
                    }
                }

                if (cnt == 0)
                {
                    CurrentID = CUR_MIN;
                    CurrentKey = string.Empty;
                    SelectedObjects.Clear();
                    htStartInfo.Clear();
                    dLink.Mode = LinkMode.None;
                }

            }

            // 컨텍스트 메뉴 호출하기
            if (CurrentID == CUR_MIN)
                ctl.ContextMenuStrip = cMenu2;
            else
                if (!bMultiSelect) ctl.ContextMenuStrip = cMenu;

            // Refresh
            Refresh();

            // Edit Mode
            if (ctl.Cursor != Cursors.Arrow && e.Button == MouseButtons.Left)
            {
                bEditMode = true;
                stPoint = new Point(e.X, e.Y);
            }
        }

        // 컨트롤 MouseMove 이벤트
        private void ctl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_ManagerMode)
            {
                // 실행모드 일때~
                CurrentID = CUR_MIN;
                CurrentKey = string.Empty;

                Objects.CollectionReverseSort();
                // MouseLeave 이벤트 발생      
                for (int i = 0; i < Objects.Count; i++)
                {
                    if (Objects[i].hasMouseOver())
                    {
                        Objects[i].FireMouseLeaveEvent();
                    }

                    // 오브젝트가 버튼이라면 라인색을 회색으로 바꾼다.
                    if (Objects[i].ObjectType == DrawObjectType.pButton && !isDrag)
                    {
                        Objects[i].LineColor = Color.Gray;                        
                    }
                }

                // MouseOver 이벤트 발생
                for (int i = 0; i < Objects.Count; i++)
                {
                    if (Objects[i].ObjectBound.Contains(e.Location))
                    {
                        CurrentID = Objects[i].Id;
                        CurrentKey = Objects[i].Key;
                        Objects[i].FireMouseOverEvent(e); 

                        // 오브젝트가 버튼이라면 라인색을 오렌지색으로 바꾼다.
                        if (Objects[i].ObjectType == DrawObjectType.pButton && !isDrag)
                        {
                            Objects[i].LineColor = Color.Orange;
                        }
                        break;
                    }
                }
                return;
            }

            // Guide Rect
            if (isDrag && ctl.Cursor == Cursors.Arrow)
            {
                ControlPaint.DrawReversibleFrame(GuideRect, ctl.BackColor, FrameStyle.Dashed);
                Point endPoint = ((Control)sender).PointToScreen(new Point(e.X, e.Y));
                GuideRect.Width = endPoint.X - GuideRect.X;
                GuideRect.Height = endPoint.Y - GuideRect.Y;
                ControlPaint.DrawReversibleFrame(GuideRect, ctl.BackColor, FrameStyle.Dashed);        
            }


            // 마우스 커서 구하기
            switch (dLink.Mode)
            {
                case LinkMode.None:
                    if (GuideRect.Width + GuideRect.Height == 0)
                    {
                        switch (EditType)
                        {
                            case -1:
                                ctl.Cursor = Cursors.SizeAll;
                                break;
                            case 0:
                                ctl.Cursor = Cursors.SizeNWSE;
                                break;
                            case 1:
                                ctl.Cursor = Cursors.SizeNS;
                                break;
                            case 2:
                                ctl.Cursor = Cursors.SizeNESW;
                                break;
                            case 3:
                                ctl.Cursor = Cursors.SizeWE;
                                break;
                            case 4:
                                ctl.Cursor = Cursors.SizeWE;
                                break;
                            case 5:
                                ctl.Cursor = Cursors.SizeNESW;
                                break;
                            case 6:
                                ctl.Cursor = Cursors.SizeNS;
                                break;
                            case 7:
                                ctl.Cursor = Cursors.SizeNWSE;
                                break;
                            default:
                                ctl.Cursor = Cursors.Arrow;
                                for (int i = 0; i < Objects.Count; i++)
                                {
                                    if (Objects[i].ObjectBound.Contains(e.Location))
                                    {
                                        ctl.Cursor = Cursors.SizeAll;
                                        break;
                                    }
                                }

                                if (SelectedObjects.Count > 0)
                                {
                                    // Edit rect
                                    // [0]----[1]----[2]
                                    //  -             -
                                    //  -             -
                                    // [3]-----------[4]
                                    //  -             -
                                    //  -             -
                                    // [5]----[6]----[7]
                                    //
                                    try
                                    {
                                        for (int i = 0; i < Objects.Count; i++)
                                        {
                                            if (EditRect[i, 0].Contains(e.Location))
                                                ctl.Cursor = Cursors.SizeNWSE;
                                            else if (EditRect[i, 1].Contains(e.Location))
                                                ctl.Cursor = Cursors.SizeNS;
                                            else if (EditRect[i, 2].Contains(e.Location))
                                                ctl.Cursor = Cursors.SizeNESW;
                                            else if (EditRect[i, 3].Contains(e.Location))
                                                ctl.Cursor = Cursors.SizeWE;
                                            else if (EditRect[i, 4].Contains(e.Location))
                                                ctl.Cursor = Cursors.SizeWE;
                                            else if (EditRect[i, 5].Contains(e.Location))
                                                ctl.Cursor = Cursors.SizeNESW;
                                            else if (EditRect[i, 6].Contains(e.Location))
                                                ctl.Cursor = Cursors.SizeNS;
                                            else if (EditRect[i, 7].Contains(e.Location))
                                                ctl.Cursor = Cursors.SizeNWSE;
                                        }
                                    }
                                    catch (Exception) { }
                                }
                                break;
                        }
                    }
                    break;
                case LinkMode.Ready:
                    ctl.Cursor = Cursors.Arrow;
                    for (int i = 0; i < Objects.Count; i++)
                    {
                        if (Objects[i].ObjectBound.Contains(e.Location))
                        {
                            ctl.Cursor = Cursors.SizeAll;
                            break;
                        }
                    }

                    if (LinkRect[0].Contains(e.Location))
                        ctl.Cursor = Cursors.PanNorth;
                    else if (LinkRect[1].Contains(e.Location))
                        ctl.Cursor = Cursors.PanWest;
                    else if (LinkRect[2].Contains(e.Location))
                        ctl.Cursor = Cursors.PanEast;
                    else if (LinkRect[3].Contains(e.Location))
                        ctl.Cursor = Cursors.PanSouth;
                    break;
                case LinkMode.Linking:
                    break;
            }

            // Edit Mode 시
            if (bEditMode)
            {
                // GetEnumerator() 메소드로 iEnum을 할당
                IDictionaryEnumerator iEnum = SelectedObjects.GetEnumerator(); 
                int padX = e.X - stPoint.X;
                int padY = e.Y - stPoint.Y;
                // 시작 좌표에서 조금이라도 오브젝트가 이동하였다면 isModify를 true로 설정
                if (padX != 0 || padY != 0) isModify = true; 

                switch (EditType)
                {
                    case -1:// 이동
                        if (Math.Abs(padX) <= 2 && Math.Abs(padY) <= 2) // 클릭이나 더블클릭하다 이동하는걸 방지하기 위함
                            break;
                        while (iEnum.MoveNext())
                        {                            
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.X = ((Rectangle)htStartInfo[iEnum.Key]).X + padX;
                            obj.Y = ((Rectangle)htStartInfo[iEnum.Key]).Y + padY;
                        }


                        break;
                    case 0:// 대각선 좌상
                        while (iEnum.MoveNext())
                        {
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.X = ((Rectangle)htStartInfo[iEnum.Key]).X + padX;
                            obj.Y = ((Rectangle)htStartInfo[iEnum.Key]).Y + padY;
                            obj.Width = (padX > 0) ? (((Rectangle)htStartInfo[iEnum.Key]).Width - padX) : (((Rectangle)htStartInfo[iEnum.Key]).Width + Math.Abs(padX));
                            obj.Height = (padY > 0) ? (((Rectangle)htStartInfo[iEnum.Key]).Height - padY) : (((Rectangle)htStartInfo[iEnum.Key]).Height + Math.Abs(padY));
                        }
                        break;
                    case 1:// 상
                        while (iEnum.MoveNext())
                        {
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.Y = ((Rectangle)htStartInfo[iEnum.Key]).Y + padY;
                            obj.Height = (padY > 0) ? (((Rectangle)htStartInfo[iEnum.Key]).Height - padY) : (((Rectangle)htStartInfo[iEnum.Key]).Height + Math.Abs(padY));
                        }
                        break;
                    case 2:// 대각선 우상
                        while (iEnum.MoveNext())
                        {
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.Y = ((Rectangle)htStartInfo[iEnum.Key]).Y + padY;
                            obj.Width = ((Rectangle)htStartInfo[iEnum.Key]).Width + (e.X - stPoint.X);
                            obj.Height = (padY > 0) ? (((Rectangle)htStartInfo[iEnum.Key]).Height - padY) : (((Rectangle)htStartInfo[iEnum.Key]).Height + Math.Abs(padY));
                        }
                        break;
                    case 3:// 좌
                        while (iEnum.MoveNext())
                        {
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.X = ((Rectangle)htStartInfo[iEnum.Key]).X + padX;
                            obj.Width = (padX > 0) ? (((Rectangle)htStartInfo[iEnum.Key]).Width - padX) : (((Rectangle)htStartInfo[iEnum.Key]).Width + Math.Abs(padX));
                        }
                        break;
                    case 4:// 우
                        while (iEnum.MoveNext())
                        {
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.Width = ((Rectangle)htStartInfo[iEnum.Key]).Width + (e.X - stPoint.X);
                        }
                        break;
                    case 5:// 대각선 좌하
                        while (iEnum.MoveNext())
                        {
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.X = ((Rectangle)htStartInfo[iEnum.Key]).X + padX;
                            obj.Width = (padX > 0) ? (((Rectangle)htStartInfo[iEnum.Key]).Width - padX) : (((Rectangle)htStartInfo[iEnum.Key]).Width + Math.Abs(padX));
                            obj.Height = ((Rectangle)htStartInfo[iEnum.Key]).Height + (e.Y - stPoint.Y);
                        }
                        break;
                    case 6:// 하
                        while (iEnum.MoveNext())
                        {
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.Height = ((Rectangle)htStartInfo[iEnum.Key]).Height + (e.Y - stPoint.Y);
                        }
                        break;
                    case 7:// 대각선 우하
                        while (iEnum.MoveNext())
                        {
                            DrawObject obj = (DrawObject)iEnum.Value;
                            obj.Width = ((Rectangle)htStartInfo[iEnum.Key]).Width + (e.X - stPoint.X);
                            obj.Height = ((Rectangle)htStartInfo[iEnum.Key]).Height + (e.Y - stPoint.Y);
                        }
                        break;
                }

                // Refresh
                Refresh();
            }
        }

        // 컨트롤 MouseUp 이벤트
        private void ctl_MouseUp(object sender, MouseEventArgs e)
        {
            isDrag = false;
            if (!_ManagerMode)
            {
                return;
            }    

            // Guide Rect 절대값으로 바꾸기
            if (GuideRect.Width < 0)
            {
                GuideRect.X = GuideRect.X + GuideRect.Width;
                GuideRect.Width = Math.Abs(GuideRect.Width);
            }
            if (GuideRect.Height < 0)
            {
                GuideRect.Y = GuideRect.Y + GuideRect.Height;
                GuideRect.Height = Math.Abs(GuideRect.Height);
            }

            // Guide Rect 에 포함되어있는 오브젝트 산출
            for (int i = 0; i < Objects.Count; i++)
            {
                if (GuideRect.Contains(ctl.RectangleToScreen(Objects[i].ObjectBound)))
                {
                    CurrentID = Objects[i].Id;
                    CurrentKey = Objects[i].Key;
                    StandardObjKey = CurrentKey;
                    SelectedObjects.Add(CurrentKey, Objects[i]);
                    htStartInfo.Add(CurrentKey, new Rectangle(Objects[i].X, Objects[i].Y, Objects[i].Width, Objects[i].Height));
                }
            }

            // Guide Rect 초기화
            //isDrag = false;
            ControlPaint.DrawReversibleFrame(GuideRect, ctl.BackColor, FrameStyle.Dashed);
            GuideRect = new Rectangle(0, 0, 0, 0);
            Refresh();

            // 편집모드 초기화
            bEditMode = false;
            // 편집타입 초기화
            EditType = -2;

            // 선택된 객체 초기화
            if (SelectedObjects.Count == 0)
            {                
                CurrentID = CUR_MIN;
                CurrentKey = string.Empty;
                StandardObjKey = string.Empty;
            }

            // 마우스로 객체를 이동했을시 XML 파일 저장
            if (isModify)
            {
                IDictionaryEnumerator iEnum = SelectedObjects.GetEnumerator();
                while (iEnum.MoveNext())
                {
                    DrawObject obj = (DrawObject)iEnum.Value;
                    htStartInfo[iEnum.Key] = new Rectangle(obj.X, obj.Y, obj.Width, obj.Height);
                }
                XmlFileUpdate();

                // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                DataSetShift(Objects);
            }

            // 수정 플래그 초기화
            isModify = false;
            //ctl.Text = SelectedObjects.Count.ToString() +"  " + bMultiSelect.ToString();
        }
        #endregion

        #region [Event] 오브젝트 컬렉션 이벤트
        private void Objects_AfterRemove(object sender, RemoveEventArgs args)
        {
            XmlFileUpdate();
            Refresh();
        }

        private void Objects_AfterAdd(object sender, EventArgs args)
        {
            XmlFileUpdate();
            Refresh();
        }
        #endregion

        #region [Method] Private 메소드
        private void DataSetShift(ObjectsCollection oc)
        {
            if (DoIndex == doSize - 1)
                Array.Copy(arrDataSet, 1, arrDataSet, 0, DoIndex);
            else
                Array.Copy(arrDataSet, 0, arrDataSet, doSize - (DoIndex + 2), DoIndex + 1);
            arrDataSet[doSize - 1] = oc.Clone();
            DoIndex = doSize - 1;
        }

        private void Set_Forward(int id)
        {
            // 끝번호 구하기
            //int nMax = 0;
            //DataRow[] drs = ds.Tables[frm.Name].Select("", "Id");
            //if (drs.Length > 0)
            //    nMax = Convert.ToInt32(drs[drs.Length - 1]["id"]) + 1;

            //// 선택된 객체 id를 nMax로 대체
            //DataRow[] rows = ds.Tables[ctl.Name].Select(String.Format("Id={0}", id));
            //if (rows.Length > 0)
            //    rows[0]["Id"] = nMax;
            Objects[CurrentKey].Id = Objects.GetMaxId();

            // 저장
            XmlFileUpdate();
        }

        private void Set_Backward(int id)
        {
            //// 앞번호 구하기
            //int nMin = 0;
            //DataRow[] drs = ds.Tables[ctl.Name].Select("", "Id");
            //if (drs.Length > 0)
            //    nMin = Convert.ToInt32(drs[0]["id"]) - 1;

            //// 선택된 객체 id를 nMin로 대체
            //DataRow[] rows = ds.Tables[ctl.Name].Select(String.Format("Id={0}", id));
            //if (rows.Length > 0)
            //    rows[0]["Id"] = nMin;
            Objects[CurrentKey].Id = Objects.GetMinId();

            // 저장
            XmlFileUpdate();
        }

        private void ObjectAdd()
        {
            Objects.Add(new DrawObject(DrawObjectType.pRectangle));

            // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
            DataSetShift(Objects);
        }

        private void ObjectRemove()
        {
            IDictionaryEnumerator iEnum = SelectedObjects.GetEnumerator();
            while (iEnum.MoveNext())
            {
                DrawObject obj = (DrawObject)iEnum.Value;
                Objects.RemoveNotSaveXml(obj.Key);
            }
            XmlFileUpdate();

            // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
            DataSetShift(Objects);
        }

        private void ObjectCopy()
        {
            IDictionaryEnumerator iEnum = SelectedObjects.GetEnumerator();
            CopyedObjects.Clear();
            while (iEnum.MoveNext())
                CopyedObjects.Add(((DrawObject)iEnum.Value).Key, iEnum.Value);
        }

        private void ObjectSelectAll()
        {
            if (StandardObjKey == string.Empty)
            {
                if (Objects.Count > 0)
                    StandardObjKey = Objects[0].Key;
            }

            SelectedObjects.Clear();
            htStartInfo.Clear();
            for (int i = 0; i < Objects.Count; i++)
            {
                SelectedObjects.Add(Objects[i].Key, Objects[i]);
                htStartInfo.Add(Objects[i].Key, new Rectangle(Objects[i].X, Objects[i].Y, Objects[i].Width, Objects[i].Height));
            }
        }

        private void ObjectPaste()
        {
            // GetEnumerator() 메소드로 iEnum을 할당
            Hashtable htTemp = (Hashtable)CopyedObjects.Clone();
            CopyedObjects.Clear();
            SelectedObjects.Clear();
            htStartInfo.Clear();
            IDictionaryEnumerator copyiEnum = htTemp.GetEnumerator();
            while (copyiEnum.MoveNext())
            {
                DrawObject obj = Objects.CopyObject((DrawObject)copyiEnum.Value);
                obj.X = obj.X + 10;
                obj.Y = obj.Y + 10;
                Objects.AddNotSaveXml(obj);
                CurrentID = obj.Id;
                CurrentKey = obj.Key;
                StandardObjKey = CurrentKey;
                CopyedObjects.Add(obj.Key, obj);
                SelectedObjects.Add(obj.Key, obj);
                htStartInfo.Add(obj.Key, new Rectangle(obj.X, obj.Y, obj.Width, obj.Height));

            }
            XmlFileUpdate();

            // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
            DataSetShift(Objects);
        }

        private void UnDo()
        {
            // 바로 전 데이터셋 불러오기
            if (DoIndex <= 0) return;
            ObjectsCollection beforOC = arrDataSet[--DoIndex];

            if (beforOC != null)
            {
                // 오브젝트 컬렉션 초기화
                Objects.Clear();

                // 오브젝트 컬렉션 교체
                Objects = beforOC.Clone();
            }
            else
                DoIndex++;

            // 새로고침
            Refresh();

            // 저장
            XmlFileUpdate();
        }

        private void ReDo()
        {
            // 바로 앞 데이터셋 불러오기
            if (DoIndex >= doSize-1) return;
            ObjectsCollection AfterOC = arrDataSet[++DoIndex];

            if (AfterOC != null)
            {
                // 오브젝트 컬렉션 초기화
                Objects.Clear();

                // 오브젝트 컬렉션 교체
                Objects = AfterOC.Clone();
            }
            Refresh();

            // 저장
            XmlFileUpdate();
        }

        private void ShowAlingDialog()
        {
            if (SelectedObjects.Count > 1)
            {
                PYJ_DrawHelpAlign dlg = new PYJ_DrawHelpAlign(this);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                    DataSetShift(Objects);

                    XmlFileUpdate();
                    CurrentID = CUR_MIN;
                    CurrentKey = string.Empty;
                    SelectedObjects.Clear();
                    htStartInfo.Clear();
                }
                else
                {
                    Load();
                    Refresh();
                    CurrentID = CUR_MIN;
                    CurrentKey = string.Empty;
                    SelectedObjects.Clear();
                    htStartInfo.Clear();
                }
            }
        }

        private void InitStartObject()
        {
            // 커서가 화살표이거나 사이즈올이면 시작포인트 시작객체를 초기화한다.
            if (ctl.Cursor == Cursors.Arrow || ctl.Cursor == Cursors.SizeAll)
            {
                CurrentID = CUR_MIN;
                CurrentKey = string.Empty;
                //SelectedObjects.Clear();
                //htStartInfo.Clear();
                stPoint = new Point();
                //stRect = new Rectangle();
            }
        }

        private void XmlFileUpdate()
        {
            ds.Tables[ctl.Name].Clear();
            for(int i=0; i<Objects.Count; i++)
            {
                DataRow dr = ds.Tables[ctl.Name].NewRow();               
                dr["Type"] = Convert.ToInt32(Objects[i].ObjectType);
                dr["Key"] = Objects[i].Key;
                dr["Id"] = Objects[i].Id;
                dr["PosX"] = Objects[i].X;
                dr["PosY"] = Objects[i].Y;
                dr["Width"] = Objects[i].Width;
                dr["Height"] = Objects[i].Height;
                dr["LineColor"] = Objects[i].LineColor.ToArgb();
                dr["LineWidth"] = Objects[i].LineWidth;
                dr["FillColor"] = Objects[i].FillColor.ToArgb();
                dr["Text"] = Objects[i].Text;
                dr["TextAlign"] = StringAlignToInt(Objects[i].TextAlign);
                dr["FontStyle"] = FontStyleToString(Objects[i].FontStyle);
                dr["FontColor"] = Objects[i].FontColor.ToArgb();
                dr["Angle1"] = Objects[i].Angle1;
                dr["Angle2"] = Objects[i].Angle2;
                dr["Image"] = imageToByteArray(Objects[i].Img);
                ds.Tables[ctl.Name].Rows.Add(dr);
            }
            ds.WriteXml(XmlFileName, XmlWriteMode.WriteSchema);
        }

        private void ShowProperty()
        {
            if (!_ManagerMode) return;
            bMultiSelect = false;
            if (CurrentID == CUR_MIN) return;
            PYJ_DrawHelpDialog dlg = new PYJ_DrawHelpDialog(ds, ctl, Objects, CurrentKey, this);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
                DataSetShift(Objects);

                XmlFileUpdate();
            }
            else
            {
                Load();
                Refresh();
                CurrentID = CUR_MIN;
                CurrentKey = string.Empty;
                SelectedObjects.Clear();
                htStartInfo.Clear();

            }
        }

        private void ImageRotate(PaintEventArgs e, DrawObject obj)
        {
            Rectangle rect = obj.ObjectBound;

            System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix();
            // Rotates around the images top left + centre
            mat.RotateAt(obj.RotationAngle, new PointF(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2)));
            e.Graphics.Transform = mat;
            e.Graphics.DrawImage(obj.Img, rect);
            e.Graphics.ResetTransform();
        }
        #endregion

        #region [Method] Public 메소드
        /// <summary>
        /// 드로우헬퍼에서 지정한 XML파일의 데이터를 읽어 오브젝트 컬렉션에 반영합니다.
        /// 해당 XML파일이 없다면 새로운 테이블을 만들어 XML파일을 생성합니다.
        /// </summary>
        public void Load()
        {
            Objects.Clear();
            ds.Clear();

            // Create Table
            DataTable dt = new DataTable();
            dt.Columns.Add("Type", typeof(int));         // 타입
            dt.Columns.Add("Key", typeof(string));       // 오브젝트 네임
            dt.Columns.Add("Id", typeof(int));           // 객체 Index
            dt.Columns.Add("PosX", typeof(int));         // X좌표
            dt.Columns.Add("PosY", typeof(int));         // Y좌표
            dt.Columns.Add("Width", typeof(int));        // 너비
            dt.Columns.Add("Height", typeof(int));       // 높이
            dt.Columns.Add("Text", typeof(string));      // 텍스트
            dt.Columns.Add("LineColor", typeof(int));    // 라인색상
            dt.Columns.Add("FillColor", typeof(int));    // 채우기색상                
            dt.Columns.Add("LineWidth", typeof(int));    // 라인두께
            dt.Columns.Add("FontStyle", typeof(string)); // 폰트스타일("굴림체,10,0")
            dt.Columns.Add("FontColor", typeof(int));    // 폰트색상
            dt.Columns.Add("TextAlign", typeof(int));    // 텍스트정렬방식 (0:left, 1:center, 2:right)
            dt.Columns.Add("Angle1", typeof(float));     // Arc각도1
            dt.Columns.Add("Angle2", typeof(float));     // Arc각도2
            dt.Columns.Add("Image", typeof(byte[]));     // 이미지
            dt.TableName = ctl.Name;

            // 파일이 있는지 확인
            if (System.IO.File.Exists(_XmlFileName))
            {
                ds.ReadXml(XmlFileName, XmlReadMode.ReadSchema);
            }
            else
            {
                ds.Tables.Add(dt);
                ds.WriteXml(XmlFileName, XmlWriteMode.WriteSchema);
            }

            // 오브젝트 객체 리스트로 관리            
            if (ds.Tables[ctl.Name] != null)
            {
                DataRow[] rows = ds.Tables[ctl.Name].Select();
                foreach (DataRow dr in rows)
                    Objects.AddNotSaveXml(new DrawObject(dr));
            }
            else
            {
                ds.Tables.Add(dt);
                ds.WriteXml(XmlFileName, XmlWriteMode.WriteSchema);
            }

            // 실행취소 버퍼에 오브젝트 저장 후 쉬프트
            DataSetShift(Objects);

            // 타이머
            timer.Enabled = true;
            timer.Interval = TimerTick;
            timer.Tick += new EventHandler(timer_Tick); 
        }

        /// <summary>
        /// 컨트롤의 전체화면을 무효화 하고 모든 오브젝트를 다시 그립니다.
        /// </summary>
        public void Refresh()
        {
            ctl.Invalidate();
        }

        /// <summary>
        /// 해쉬테이블(SelectedObjects)에 저장된 오브젝트들을 기준 오브젝트(StandardObjKey)를 기준으로 정렬합니다.
        /// </summary>
        /// <param name="Align">정렬 형태</param>
        public void ObjectAlign(DrawAlign Align)
        {
            ArrayList arrSorter = new ArrayList();            
            IDictionaryEnumerator iEnum = SelectedObjects.GetEnumerator();
            int nX = Objects[StandardObjKey].X;
            int nY = Objects[StandardObjKey].Y;
            int nW = Objects[StandardObjKey].Width;
            int nH = Objects[StandardObjKey].Height;
            switch (Align)
            {
                case DrawAlign.Left:    // 왼쪽 맞춤
                    while (iEnum.MoveNext())
                        ((DrawObject)iEnum.Value).X = Objects[StandardObjKey].X;
                    break;
                case DrawAlign.Center:  // 가운데 맞춤
                    while (iEnum.MoveNext())
                    {
                        if (iEnum.Key.ToString() == StandardObjKey) continue;
                        ((DrawObject)iEnum.Value).X =
                            (nX + (nW / 2)) - (((DrawObject)iEnum.Value).Width / 2);
                    }
                    break;
                case DrawAlign.Right:   // 오른쪽 맞춤
                    while (iEnum.MoveNext())
                    {
                        if (iEnum.Key.ToString() == StandardObjKey) continue;
                        ((DrawObject)iEnum.Value).X =
                            (nX + nW) - (((DrawObject)iEnum.Value).Width);
                    }
                    break;
                case DrawAlign.Top:     // 위쪽 맞춤
                    while (iEnum.MoveNext())
                        ((DrawObject)iEnum.Value).Y = Objects[StandardObjKey].Y;
                    break;
                case DrawAlign.Middle:  // 중간 맞춤
                    while (iEnum.MoveNext())
                    {
                        if (iEnum.Key.ToString() == StandardObjKey) continue;
                        ((DrawObject)iEnum.Value).Y =
                            (nY + (nH / 2)) - (((DrawObject)iEnum.Value).Height / 2);
                    }
                    break;
                case DrawAlign.Bottom:  // 아래쪽 맞춤
                    while (iEnum.MoveNext())
                    {
                        if (iEnum.Key.ToString() == StandardObjKey) continue;
                        ((DrawObject)iEnum.Value).Y =
                            (nY + nH) - (((DrawObject)iEnum.Value).Height);
                    }
                    break;
                case DrawAlign.Width:   // 같은 너비로
                    while (iEnum.MoveNext())
                        ((DrawObject)iEnum.Value).Width = Objects[StandardObjKey].Width;
                    break;
                case DrawAlign.Height:  // 같은 높이로
                    while (iEnum.MoveNext())
                        ((DrawObject)iEnum.Value).Height = Objects[StandardObjKey].Height;
                    break;
                case DrawAlign.Size:    // 같은 크기로
                    while (iEnum.MoveNext())
                    {
                        ((DrawObject)iEnum.Value).Width = Objects[StandardObjKey].Width;
                        ((DrawObject)iEnum.Value).Height = Objects[StandardObjKey].Height;
                    }
                    break;
                case DrawAlign.ArrayH:  // 가로 간격 제거
                    IComparer cmpX = new SortForPosX();
                    arrSorter.Clear();
                    arrSorter.AddRange(SelectedObjects.Values);
                    arrSorter.Sort(cmpX);
                    int? beforeX=null, beforeW=null;
                    foreach (Object obj in arrSorter)
                    {
                        if (beforeX.HasValue && beforeW.HasValue)
                            ((DrawObject)obj).X = beforeX.Value + beforeW.Value;
                        beforeX = ((DrawObject)obj).X;
                        beforeW = ((DrawObject)obj).Width;

                    }
                    break;
                case DrawAlign.ArrayV:  // 세로 간격 제거
                    IComparer cmpY = new SortForPosY();
                    arrSorter.Clear();
                    arrSorter.AddRange(SelectedObjects.Values);
                    arrSorter.Sort(cmpY);
                    int? beforeY = null, beforeH = null;
                    foreach (Object obj in arrSorter)
                    {
                        if (beforeY.HasValue && beforeH.HasValue)
                            ((DrawObject)obj).Y = beforeY.Value + beforeH.Value;                            
                        beforeY = ((DrawObject)obj).Y;
                        beforeH = ((DrawObject)obj).Height;
                    }
                    break;
                case DrawAlign.SameH:   // 가로 간격 같게
                    cmpX = new SortForPosX();
                    arrSorter.Clear();
                    arrSorter.AddRange(SelectedObjects.Values);
                    arrSorter.Sort(cmpX);
                    beforeX = null;
                    beforeW = null;
                    DrawObject FirstObject = (DrawObject)arrSorter[0];
                    DrawObject LastObject = (DrawObject)arrSorter[arrSorter.Count -1];
                    int TotalWidth = (LastObject.X + LastObject.Width) - FirstObject.X;
                    int SumWidth = 0;
                    foreach (Object obj in arrSorter)
                    {
                        SumWidth += ((DrawObject)obj).Width;
                    }
                    int pad = (TotalWidth - SumWidth) / (arrSorter.Count - 1);
                    
                    foreach (Object obj in arrSorter)
                    {
                        if (beforeX.HasValue && beforeW.HasValue)
                            ((DrawObject)obj).X = beforeX.Value + beforeW.Value + pad;
                        beforeX = ((DrawObject)obj).X;
                        beforeW = ((DrawObject)obj).Width;

                    }
                    break;
                case DrawAlign.SameV:   // 세로 간격 같게
                    cmpY = new SortForPosY();
                    arrSorter.Clear();
                    arrSorter.AddRange(SelectedObjects.Values);
                    arrSorter.Sort(cmpY);
                    beforeY = null;
                    beforeH = null;
                    FirstObject = (DrawObject)arrSorter[0];
                    LastObject = (DrawObject)arrSorter[arrSorter.Count - 1];
                    int TotalH = (LastObject.Y + LastObject.Height) - FirstObject.Y;
                    int SumH = 0;
                    foreach (Object obj in arrSorter)
                    {
                        SumH += ((DrawObject)obj).Height;
                    }
                    pad = (TotalH - SumH) / (arrSorter.Count - 1);

                    foreach (Object obj in arrSorter)
                    {
                        if (beforeY.HasValue && beforeH.HasValue)
                            ((DrawObject)obj).Y = beforeY.Value + beforeH.Value + pad;
                        beforeY = ((DrawObject)obj).Y;
                        beforeH = ((DrawObject)obj).Height;
                    }
                    break;
                default:
                    break;
            }
            Refresh();
        }
        #endregion

        #region [Method] Static 메소드
        static public FontStyle GetFontStyle(string type)
        {
            if (type == "Bold")
                return FontStyle.Bold;
            else if (type == "Italic")
                return FontStyle.Italic;
            else if (type == "Underline")
                return FontStyle.Underline;
            else if (type == "Strikeout")
                return FontStyle.Strikeout;
            else if (type == "Bold, Strikeout")
                return FontStyle.Bold | FontStyle.Strikeout;
            else if (type == "Bold, Underline")
                return FontStyle.Bold | FontStyle.Underline;
            else if (type == "Italic, Strikeout")
                return FontStyle.Italic | FontStyle.Strikeout;
            else if (type == "Italic, Underline")
                return FontStyle.Italic | FontStyle.Underline;
            else if (type == "Underline, Strikeout")
                return FontStyle.Underline | FontStyle.Strikeout;
            else if (type == "Italic, Underline, Strikeout")
                return FontStyle.Italic | FontStyle.Underline | FontStyle.Strikeout;
            else if (type == "Bold, Underline, Strikeout")
                return FontStyle.Bold | FontStyle.Underline | FontStyle.Strikeout;
            else if (type == "Bold, Italic, Underline, Strikeout")
                return FontStyle.Bold | FontStyle.Italic | FontStyle.Underline | FontStyle.Strikeout;
            else if (type == "Bold, Italic")
                return FontStyle.Bold | FontStyle.Italic;
            else
                return FontStyle.Regular;
        }

        static public StringAlignment GetStringAlign(int type)
        {
            switch (type)
            {
                case 0:
                    return StringAlignment.Near;
                case 1:
                    return StringAlignment.Center;
                default:
                    return StringAlignment.Far;
            }
        }

        static public string FontStyleToString(Font font)
        {
            return String.Format("{0}:{1}:{2}", font.Name, Convert.ToSingle(font.Size), font.Style);
        }

        static public int StringAlignToInt(StringAlignment sa)
        {
            switch (sa)
            {
                case StringAlignment.Near:
                    return 0;
                case StringAlignment.Center:
                    return 1;
                case StringAlignment.Far:
                    return 2;
                default:
                    return 0;
            }
        }

        static public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            byte[] arrDummy = { 0 };
            if (imageIn == null) return arrDummy;
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        static public Image byteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn.Length == 1) return null;
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }      
        
        static public GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int nRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, nRadius * 2, nRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + nRadius, rect.Y, rect.Right - nRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - nRadius * 2, rect.Y, nRadius * 2, nRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + nRadius * 2, rect.Right, rect.Y + rect.Height - nRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - nRadius * 2, rect.Y + rect.Height - nRadius * 2, nRadius * 2, nRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - nRadius * 2, rect.Bottom, rect.X + nRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - nRadius * 2, nRadius * 2, nRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - nRadius * 2, rect.X, rect.Y + nRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect; 
        }

        static public GraphicsPath CreateDiamondPath(Rectangle rect)
        {
            GraphicsPath diamondRect = new GraphicsPath();
            diamondRect.AddLine(rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width, rect.Y + rect.Height / 2);
            diamondRect.AddLine(rect.X + rect.Width, rect.Y + rect.Height / 2, rect.X + rect.Width / 2, rect.Y + rect.Height);
            diamondRect.AddLine(rect.X + rect.Width / 2, rect.Y + rect.Height, rect.X, rect.Y + rect.Height/2);
            diamondRect.AddLine(rect.X, rect.Y + rect.Height / 2, rect.X + rect.Width / 2, rect.Y);
            diamondRect.CloseFigure();
            return diamondRect;
        }
        #endregion
    }
}
