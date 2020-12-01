using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace PYJ_DrawHelper
{
    /// <summary>
    /// 오브젝트 타입
    /// </summary>
    public enum DrawObjectType
    {
        /// <summary>
        /// 사각형
        /// </summary>
        pRectangle,
        /// <summary>
        /// 원형
        /// </summary>
        pEllipse,
        /// <summary>
        /// Arc형
        /// </summary>
        pArc,
        /// <summary>
        /// 가로선
        /// </summary>
        pLine,
        /// <summary>
        /// 대각선(↗)
        /// </summary>
        pLineNE,
        /// <summary>
        /// 대각선(↖)
        /// </summary>
        pLineNW,
        /// <summary>
        /// 이미지
        /// </summary>
        pImage,
        /// <summary>
        /// 버튼
        /// </summary>
        pButton,
        /// <summary>
        /// 마름모
        /// </summary>
        pDiamond,
        /// <summary>
        /// 세로선
        /// </summary>
        pLineV,
        /// <summary>
        /// 라운드사각형
        /// </summary>
        pRoundRect
    }

    /// <summary>
    /// 오브젝트 컬렌션의 오브젝트를 삭제하기 위한 타입
    /// </summary>
    public enum RemoveType
    {
        /// <summary>
        /// 선택된 오브젝트를 삭제한다.
        /// </summary>
        RemoveAt,
        /// <summary>
        /// 모든 오브젝트를 삭제한다.
        /// </summary>
        Clear
    }

    /// <summary>
    /// 오브젝트 돌리기(Rotation) 타입
    /// </summary>
    public enum RotationType
    {
        /// <summary>
        /// 오브젝트를 돌리지 않는다.
        /// </summary>
        None,
        /// <summary>
        /// 오브젝트를 왼쪽으로 돌린다.
        /// </summary>
        Left,
        /// <summary>
        /// 오브젝트를 오른쪽으로 돌린다.
        /// </summary>
        Right
    }

    /// <summary>
    /// 오브젝트컬렉션 Remove 이벤트 인자
    /// </summary>
    public class RemoveEventArgs
    {
        /// <summary>
        /// 삭제 타입
        /// </summary>
        public RemoveType RemoveType;
        /// <summary>
        /// 오브젝트 키
        /// </summary>
        public string key;
    }

    /// <summary>
    ///  오브젝트컬렉션 ID에 따른 정렬 클래스
    /// </summary>
    public class SortForID : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((DrawObject)x).Id, ((DrawObject)y).Id));
        }

    }

    /// <summary>
    /// 오브젝트컬렉션 ID에 따른 역순 정렬 클래스
    /// </summary>
    public class ReverseSortForID : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((DrawObject)y).Id, ((DrawObject)x).Id));
        }

    }

    /// <summary>
    /// 오브젝트컬렉션 X값에 따른 정렬 클래스
    /// </summary>
    public class SortForPosX : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((DrawObject)x).X, ((DrawObject)y).X));
        }
    }

    /// <summary>
    /// 오브젝트컬렉션 Y값에 따른 정렬 클래스
    /// </summary>
    public class SortForPosY : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((DrawObject)x).Y, ((DrawObject)y).Y));
        }
    }

    /// <summary>
    /// 오브젝트 컬렉션 클래스
    /// : 오브젝트를 추가, 삭제, 불러 올 수 있다.
    /// </summary>
    public class ObjectsCollection
    {
        #region 선언
        // 오브젝트를 담을 ArrayList (인덱스로 산출하기 위한)
        private ArrayList arrObjects = new ArrayList();
        // 오브젝트를 담을 Hashtable (Key:오브젝트키, Value: DrawObject)
        private Hashtable htObjects = new Hashtable();
        #endregion

        #region 이벤트 정의
        // After Add 이벤트
        public delegate void ObjectAdded(object sender, EventArgs args);
        public event ObjectAdded AfterAdd;
        private void FireAddEvent()
        {
            if (AfterAdd != null)
            {
                EventArgs args = new EventArgs();
                AfterAdd(this, args);
            }
        }

        // After Remove 이벤트
        public delegate void ObjectRemoved(object sender, RemoveEventArgs args);
        public event ObjectRemoved AfterRemove;
        private void FireRemoveEvent(RemoveEventArgs args)
        {
            if (AfterRemove != null)
            {
                AfterRemove(this, args);
            }
        }
        #endregion

        #region 인덱서
        /// <summary>
        /// 오브젝트 키로 DrawObject를 가져옵니다.
        /// </summary>
        /// <param name="key">오브젝트 키</param>
        /// <returns></returns>
        public DrawObject this[string key]
        {
            get
            {
                try
                {
                    return (DrawObject)htObjects[key];
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 인덱스로 DrawObject를 가져옵니다.
        /// </summary>
        /// <param name="idx">오브젝트 컬렉션에 등록된 순서</param>
        /// <returns></returns>
        public DrawObject this[int idx]
        {
            get
            {
                try
                {
                    return (DrawObject)arrObjects[idx];
                }
                catch
                {
                    return null;
                }
            }
        }

        public ArrayList ObjectList 
        { 
            get
            {
                return arrObjects;
            }
        }
        #endregion

        #region 생성자
        /// <summary>
        /// 오브젝트컬렉션 생성자
        /// </summary>
        public ObjectsCollection()
        {
        }
        #endregion


        #region [Method] Add
        /// <summary>
        /// 오브젝트를 추가하고 XML파일에 저장합니다.
        /// </summary>
        /// <param name="o">DrawObject</param>
        public void Add(DrawObject o)
        {
            Add(o, GetMaxId());
        }

        /// <summary>
        /// 오브젝트를 추가하고 XML파일에 저장합니다.
        /// </summary>
        /// <param name="o">DrawObject</param>
        /// <param name="Id">그리기 순서</param>
        public void Add(DrawObject o, int Id)
        {
            // 인덱스 산출
            o.Id = Id;

            // 중복체크 후 키값 변경
            int cnt = 0;
            while (htObjects.Contains(o.Key))
                o.Key = String.Format("Object{0}", ++cnt);

            // 컬렉션에 등록
            try
            {
                htObjects.Add(o.Key, o);
                arrObjects.Add(o);
            }
            catch (Exception)
            {
                o.Key = String.Format("{0}_{1}", o.Key, 1);
                htObjects.Add(o.Key, o);
                arrObjects.Add(o);
            }

            // 이벤트 발생
            FireAddEvent();
        }

        /// <summary>
        /// 오브젝트를 추가하지만 XML파일에 저장하지 않습니다.
        /// </summary>
        /// <param name="o"></param>
        public void AddNotSaveXml(DrawObject o)
        {
            // 인덱스 산출
            o.Id = GetMaxId();

            // 중복체크 후 키값 변경
            int cnt = 0;
            while (htObjects.Contains(o.Key))
                o.Key = String.Format("Object{0}", ++cnt);

            // 컬렉션에 등록
            htObjects.Add(o.Key, o);
            arrObjects.Add(o);
        }
        #endregion

        #region [Method] Remove
        /// <summary>
        /// 오브젝트를 삭제하고 XML파일에 저장합니다.
        /// </summary>
        /// <param name="key">오브젝트 키</param>
        public void Remove(string key)
        {
            arrObjects.Remove((DrawObject)htObjects[key]);
            htObjects.Remove(key);

            RemoveEventArgs args = new RemoveEventArgs();
            args.RemoveType = RemoveType.RemoveAt;
            args.key = key;
            //이벤트 발생
            FireRemoveEvent(args);
        }
        /// <summary>
        /// 오브젝트를 삭제하고 XML파일에 저장합니다.
        /// </summary>
        /// <param name="id">오브젝트 인덱스</param>
        public void RemoveAt(int id)
        {
            if (id < arrObjects.Count)
            {
                string key = ((DrawObject)arrObjects[id]).Key;
                htObjects.Remove(key);
                arrObjects.RemoveAt(id);

                RemoveEventArgs args = new RemoveEventArgs();
                args.RemoveType = RemoveType.RemoveAt;
                args.key = key;
                FireRemoveEvent(args);
            }
        }
        /// <summary>
        /// 오브젝트를 모두 삭제하고 XML파일에 저장합니다.
        /// </summary>
        public void RemoveAll()
        {
            htObjects.Clear();
            arrObjects.Clear();

            RemoveEventArgs args = new RemoveEventArgs();
            args.RemoveType = RemoveType.Clear;
            FireRemoveEvent(args);
        }
        /// <summary>
        /// 오브젝트를 삭제하지만 XML파일에 저장하지 않습니다.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveNotSaveXml(string key)
        {
            arrObjects.Remove((DrawObject)htObjects[key]);
            htObjects.Remove(key);
        }
        /// <summary>
        /// 오브젝트를 모두 삭제하지만 XML파일에 저장하지 않습니다.
        /// </summary>
        public void Clear()
        {
            // Remove 이벤트를 발생시키지 않는~
            htObjects.Clear();
            arrObjects.Clear();
        }
        #endregion

        /// <summary>
        /// 오브젝트 컬렉션에 저장된 오브젝트 수
        /// </summary>
        public int Count
        {
            get { return arrObjects.Count; }
        }

        /// <summary>
        /// 오브젝트 컬렉션에서 그리기 순번의 MAX값을 가져옵니다.
        /// </summary>
        /// <returns>그리기 순번</returns>
        public int GetMaxId()
        {
            int max = 0;
            foreach (object o in arrObjects)
            {
                if (((DrawObject)o).Id > max)
                    max = ((DrawObject)o).Id;
            }
            return max+1;
        }

        /// <summary>
        /// 오브젝트 컬렉션에서 그리기 순번의 MIN값을 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public int GetMinId()
        {
            int min = 0;
            foreach (object o in arrObjects)
            {
                if (((DrawObject)o).Id < min)
                    min = ((DrawObject)o).Id;
            }
            return min - 1;
        }

        /// <summary>
        /// 오브젝트의 선택 유무를 리턴합니다.
        /// </summary>
        /// <param name="key">오브젝트 키</param>
        /// <returns>(True:선택됨, False:선택안됨)</returns>
        public bool Contain(string key)
        {
            return htObjects.Contains(key);
        }

        /// <summary>
        /// 오브젝트들을 그리기 순번으로 정렬합니다.
        /// </summary>
        public void CollectionSort()
        {
            IComparer myComparer = new SortForID();
            arrObjects.Sort(myComparer);
        }

        /// <summary>
        /// 오브젝트들을 그리기 순번으로 역순 정렬합니다.
        /// </summary>
        public void CollectionReverseSort()
        {
            IComparer myComparer = new ReverseSortForID();
            arrObjects.Sort(myComparer);
        }

        /// <summary>
        /// 오브젝트를 복사합니다.
        /// </summary>
        /// <param name="src">복사 할 원본 DrawObject</param>
        /// <returns>복사된 DrawObject</returns>
        public DrawObject CopyObject(DrawObject src)
        {
            DrawObject dest = new DrawObject(DrawObjectType.pRectangle);
            dest.ObjectType = src.ObjectType;
            dest.Key = src.Key;
            dest.Id = src.Id;
            dest.X = src.X;
            dest.Y = src.Y;
            dest.Width = src.Width;
            dest.Height = src.Height;
            dest.LineColor = src.LineColor;
            dest.LineWidth = src.LineWidth;
            dest.Text = src.Text;
            dest.FontColor = src.FontColor;
            dest.FontStyle = src.FontStyle;
            dest.TextAlign = src.TextAlign;
            dest.FillColor = src.FillColor;
            dest.Angle1 = src.Angle1;
            dest.Angle2 = src.Angle2;
            dest.Img = src.Img;
            return dest;
        }

        /// <summary>
        /// 오브젝트컬렉션의 모든 오브젝트들을 복사합니다.
        /// </summary>
        /// <returns>복사된 ObjectsCollection</returns>
        public ObjectsCollection Clone()
        {
            ObjectsCollection rtnObj = new ObjectsCollection();

            ArrayList arrNew = new ArrayList();
            Hashtable htNew = new Hashtable();
            foreach (object o in arrObjects)
            {
                DrawObject obj = CopyObject((DrawObject)o);
                arrNew.Add(obj);
                htNew.Add(obj.Key, obj);
            }
            rtnObj.arrObjects = arrNew;
            rtnObj.htObjects = htNew;
            return rtnObj;
        }
    }

    /// <summary>
    /// PYJ DrawObject
    /// </summary>
    public class DrawObject
    {
        #region 선언
        private DrawObjectType _type;
        private string _Key;
        private int _Id;
        private int _X;
        private int _Y;
        private int _Width;
        private int _Height;
        private Color _LineColor;
        private int _LineWidth;
        private string _Text;
        private Font _FontStyle;
        private Color _FontColor;
        private StringAlignment _TextAlign;
        private Color _FillColor;
        private float _Angle1;
        private float _Angle2;
        private bool _Visible = true;
        private Image _Image;
        private RotationType _Rotate = RotationType.None;
        private object _tag;
        private bool bMouseOver = false;
        private float _speed = 10.0f;
        private float _RotationAngle = 0.0f;
        internal bool bFocus = false;
        internal bool bChanged = true;
         #endregion

        #region 프로퍼티
        /// <summary>
        /// 오브젝트 그리기 타입을 가져오거나 설정합니다.
        /// </summary>
        public DrawObjectType ObjectType { get { return _type; } 
            set 
            {
                if (_type != value)
                {
                    _type = value;
                    bChanged = true;
                }
                if (_type == DrawObjectType.pButton)
                {
                    this._LineColor = Color.Gray;
                }
            } }
        /// <summary>
        /// 오브젝트 키를 가져오거나 설정합니다.
        /// </summary>
        public string Key { get { return _Key; } 
            set 
            {
                if (_Key != value)
                {
                    _Key = value;
                    bChanged = true;
                }
            } }
        /// <summary>
        /// 오브젝트 그리기 순서를 가져오거나 설정합니다.
        /// </summary>
        public int Id { get { return _Id; } 
            set 
            {
                if (_Id != value)
                {
                    _Id = value;
                    bChanged = true;
                }
            } }
        /// <summary>
        /// 오브젝트 X좌표를 가져오거나 설정합니다.
        /// </summary>
        public int X
        {
            get { return _X; }
            set
            {
                if (_X != value)
                {
                    _X = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트 Y좌표를 가져오거나 설정합니다.
        /// </summary>
        public int Y
        {
            get { return _Y; }
            set
            {
                if (_Y != value)
                {
                    _Y = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트 너비를 가져오거나 설정합니다.
        /// </summary>
        public int Width { get { return _Width; } set {
            if (_Width != value)
            {
                _Width = value;
                bChanged = true;
            }
            if (_Width < 4)
            {
                _Width = 4;
            }
        } }
        /// <summary>
        /// 오브젝트 높이를 가져오거나 설정합니다.
        /// </summary>
        public int Height { get { return _Height; } set {
            if (_Height != value)
            {
                _Height = value;
                bChanged = true;
            }
            if (_Height < 4)
            {
                _Height = 4;
            }
        } }
        /// <summary>
        /// 오브젝트 테두리 색상을 가져오거나 설정합니다.
        /// </summary>
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                if (_LineColor != value)
                {
                    _LineColor = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트 테두리 두께를 가져오거나 설정합니다.
        /// </summary>
        public int LineWidth
        {
            get { return _LineWidth; }
            set
            {
                if (_LineWidth != value)
                {
                    _LineWidth = value;
                    bChanged = true;
                }
            }
        }      
        /// <summary>
        /// 오브젝트 텍스트를 가져오거나 설정합니다. (pArc, pLine, pLineNE, pLineNW 타입에는 표현되지 않습니다.)
        /// </summary>
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트 텍스트의 글꼴을 가져오거나 설정합니다.
        /// </summary>
        public Font FontStyle
        {
            get { return _FontStyle; }
            set
            {
                if (_FontStyle != value)
                {
                    _FontStyle = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트 텍스트의 색상을 가져오거나 설정합니다.
        /// </summary>
        public Color FontColor
        {
            get { return _FontColor; }
            set
            {
                if (_FontColor != value)
                {
                    _FontColor = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트 텍스트의 정렬타입을 가져오거나 설정합니다.
        /// </summary>
        public StringAlignment TextAlign
        {
            get { return _TextAlign; }
            set
            {
                if (_TextAlign != value)
                {
                    _TextAlign = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트의 채우기 생상을 가져오거나 설정합니다.
        /// </summary>
        public Color FillColor
        {
            get { return _FillColor; }
            set
            {
                if (_FillColor != value)
                {
                    _FillColor = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// pArc 오브젝트의 시작 각도를 가져오거나 설정합니다.
        /// </summary>
        public float Angle1
        {
            get { return _Angle1; }
            set
            {
                if (_Angle1 != value)
                {
                    _Angle1 = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// pArc 오브젝트의 호 각도를 가져오거나 설정합니다.
        /// </summary>
        public float Angle2
        {
            get { return _Angle2; }
            set
            {
                if (_Angle2 != value)
                {
                    _Angle2 = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트를 나타내는 영역의 사각형을 가져옵니다.
        /// </summary>
        public Rectangle ObjectBound { get { return new Rectangle(_X, _Y, _Width, _Height); } }
        /// <summary>
        /// 오브젝트의 활성화 상태를 가져오거나 설정합니다.
        /// </summary>
        public bool Visible { get { return _Visible; } 
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    bChanged = true;
                }
            }}
        /// <summary>
        /// pImage 오브젝트의 사용되는 이미지를 가져오거나 설정합니다.
        /// </summary>
        public Image Img
        {
            get { return _Image; }
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트의 돌리기(Rotation) 타입을 가져오거나 설정합니다.
        /// </summary>
        public RotationType Rotate
        {
            get { return _Rotate; }
            set
            {
                if (_Rotate != value)
                {
                    _Rotate = value;
                    bChanged = true;
                }
            }
        }
        /// <summary>
        /// 오브젝트의 돌리기(Rotation) 속도를 가져오거나 설정합니다. (Min:0, Max:100)
        /// </summary>
        public float RotationSpeed { get { return _speed; } 
            set 
            {
                if (_speed != value)
                {
                    _speed = value;
                    bChanged = true;
                }
                if (_speed > 100.0f)
                    _speed = 100.0f;
                if (_speed < 0.0f)
                    _speed = 0.0f; 
            }}
        /// <summary>
        /// 오브젝트의 돌리기(Rotation) 각도를 가져오거나 설정합니다.
        ///  : Rotate 타입을 Left 혹은 Right시 자동으로 값이 변경됩니다.
        /// </summary>      
        public float RotationAngle { get { return _RotationAngle; } 
            set 
            {
                if (_RotationAngle != value)
                {
                    _RotationAngle = value;
                    bChanged = true;
                }
                if (_RotationAngle > 359.9f)
                    _RotationAngle = 0.0f;
                if (_RotationAngle < 0.0f)
                    _RotationAngle = 359.9f;                
            } }
        /// <summary>
        /// 오브젝트의 Tag를 가져오거나 설정합니다.
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set
            {
                if (_tag != value)
                {
                    _tag = value;
                    bChanged = true;
                };
            }
        }

        // test animation
        public AnimationType AnimationType = AnimationType.None;
        public int AnimationSetMsec = 0;
        public int AnimationToValue = 0;
        public int AnimationFromValue = 0;
        public float AnimationActValue = 0;

        #endregion

        #region 이벤트 정의
        // 클릭 이벤트
        public delegate void ObjectClick(object sender, EventArgs args);
        public event ObjectClick Click;
        public void FireClickEvent()
        {
            if (Click != null)
            {
                EventArgs args = new EventArgs();
                Click(this, args);
            }
        }
        // 마우스오버 이벤트
        public delegate void ObjectMouseOver(object sender, MouseEventArgs args);
        public event ObjectMouseOver MouseOver;
        public void FireMouseOverEvent(MouseEventArgs _args)
        {
            if (MouseOver != null)
            {
                bMouseOver = true;
                MouseOver(this, _args);
            }
        }
        public bool hasMouseOver()
        {
            return bMouseOver;
        }
        // 마우스리버 이벤트
        public delegate void ObjectMouseLeave(object sender, EventArgs args);
        public event ObjectMouseLeave MouseLeave;
        public void FireMouseLeaveEvent()
        {
            if (MouseLeave != null)
            {
                bMouseOver = false;
                EventArgs args = new EventArgs();
                MouseLeave(this, args);
            }
        }
        #endregion

        #region 그리기 도구
            // 사각형
            Rectangle DrawRect = new Rectangle();

            // 버튼 바깥쪽 라인을 그리기 위한 라운드 사각형
            GraphicsPath rRect;
            // 버튼 안쪽 라인을 그리기 위한 임시 사각형
            Rectangle tempRect;

            // 펜
            Pen pLine = new Pen(Color.Blue);
            Pen pButtonGuide = new Pen(Color.FromArgb(55, 55, 55), 1);            
            Pen pButtonOutLine = new Pen(Color.Black, 1);
            Pen pButtonInnerLine;
 
            // 텍스트 정렬
            StringFormat format = new StringFormat();
        #endregion

        #region 생성자
        /// <summary>
        /// DrawObject 생성자
        /// </summary>
        /// <param name="type">오브젝트 타입</param>
        public DrawObject(DrawObjectType type)
        {
            Init("Object", type);
        }
        /// <summary>
        /// DrawObject 생성자
        /// </summary>
        /// <param name="key">오브젝트 키</param>
        /// <param name="type">오브젝트 타입</param>
        public DrawObject(string key, DrawObjectType type)
        {
            Init(key, type);
        }
        /// <summary>
        /// DrawObject 생성자
        /// </summary>
        /// <param name="dr">XML에서 불러온 테이블의 DataRow</param>
        public DrawObject(DataRow dr)
        {
            dr.BeginEdit();
            ObjectType = (DrawObjectType)Convert.ToInt32(dr["Type"]);
            Key = dr["Key"].ToString();
            Id = Convert.ToInt32(dr["Id"]);
            X = Convert.ToInt32(dr["PosX"]);
            Y = Convert.ToInt32(dr["PosY"]);
            Width = Convert.ToInt32(dr["Width"]);
            Height = Convert.ToInt32(dr["Height"]);            
            LineColor = Color.FromArgb(Convert.ToInt32(dr["LineColor"]));            
            LineWidth = Convert.ToInt32(dr["LineWidth"]);
            FillColor = Color.FromArgb(Convert.ToInt32(dr["FillColor"]));
            Text = dr["Text"].ToString();
            TextAlign = DrawHelper.GetStringAlign(Convert.ToInt32(dr["TextAlign"]));
            string[] tmp = dr["FontStyle"].ToString().Split(':');
            FontStyle = new Font(tmp[0], Convert.ToSingle(tmp[1]), DrawHelper.GetFontStyle(tmp[2]));
            FontColor = Color.FromArgb(Convert.ToInt32(dr["FontColor"]));
            Angle1 = Convert.ToSingle(dr["Angle1"]);
            Angle2 = Convert.ToSingle(dr["Angle2"]);
            Img = DrawHelper.byteArrayToImage((byte[])dr["Image"]);
            dr.EndEdit();
            dr.AcceptChanges();            
        }
        // 초기화
        private void Init(string key, DrawObjectType type)
        {
            ObjectType = type;
            Key = key;
            Id = 0;
            X = 0;
            Y = 0;
            Width = 100;
            Height = 100;
            LineColor = Color.Black;
            LineWidth = 1;
            FillColor = Color.Empty;
            Text = string.Empty;
            TextAlign = DrawHelper.GetStringAlign(1);
            FontStyle = new Font("굴림체", 10);
            FontColor = Color.Black;
            Angle1 = 0;
            Angle2 = 180;
            Img = null;
        }

        private void InitDrawTools()
        {
            // 그리기 도구
            DrawRect.X = X;
            DrawRect.Y = Y;
            DrawRect.Width = Width;
            DrawRect.Height = Height;
            rRect = DrawHelper.CreateRoundedRectanglePath(DrawRect, 4);
            tempRect = new Rectangle(DrawRect.X + 1, DrawRect.Y + 1, DrawRect.Width - 2, DrawRect.Height - 2);
            pLine.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            pLine.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            pButtonGuide.DashStyle = DashStyle.Dash;            
            format.Alignment = TextAlign;
            format.LineAlignment = StringAlignment.Center;
        }
        #endregion

        #region [Method] Drawing
        /// <summary>
        /// 오브젝트를 컨트롤에 그립니다.
        /// </summary>
        /// <param name="g">그래픽 객체</param>
        /// <param name="flag">개발자모드 플래그값(True:편집모드, False:런타임모드)</param>
        public void Drawing(Graphics g, bool flag)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix();
            mat.RotateAt(RotationAngle, new PointF(X + (Width / 2), Y + (Height / 2)));
            //if (!flag)
                g.Transform = mat;

            InitDrawTools();

            if (!Visible && !flag) return;

            // 타입에 따른 그리기
            switch (_type)
            {
                // **************************************************************************
                // 1. pRectangle 사각형
                // **************************************************************************
                case DrawObjectType.pRectangle:
                    if (_FillColor.ToArgb() != 0)
                        g.FillRectangle(new SolidBrush(_FillColor), DrawRect);
                    if (_LineColor.ToArgb() != 0)
                        g.DrawRectangle(new Pen(new SolidBrush(_LineColor), LineWidth), DrawRect);
                    else
                    {
                        if (flag)
                            g.DrawRectangle(pLine, DrawRect);
                    }
                    g.DrawString(Text.Replace("\\n", "\n"), FontStyle, new SolidBrush(_FontColor), DrawRect, format);
                    break;

                // **************************************************************************
                // 2. pEllipse 원형
                // **************************************************************************
                case DrawObjectType.pEllipse:
                    if (_FillColor.ToArgb() != 0)
                        g.FillEllipse(new SolidBrush(_FillColor), DrawRect);
                    if (_LineColor.ToArgb() != 0)
                        g.DrawEllipse(new Pen(new SolidBrush(_LineColor), LineWidth), DrawRect);
                    else
                    {
                        if (flag)
                            g.DrawEllipse(pLine, DrawRect);
                    }
                    g.DrawString(Text.Replace("\\n", "\n"), FontStyle, new SolidBrush(_FontColor), DrawRect, format);
                    break;

                // **************************************************************************
                // 3. pArc 호
                // **************************************************************************
                case DrawObjectType.pArc:
                    if (_LineColor.ToArgb() != 0)
                        g.DrawArc(new Pen(_LineColor, LineWidth), DrawRect, Angle1, Angle2);
                    else
                    {
                        if (flag)
                            g.DrawArc(pLine, DrawRect, Angle1, Angle2);
                    }
                    break;

                // **************************************************************************
                // 4. pLine 라인
                // **************************************************************************
                case DrawObjectType.pLine:
                    if (_LineColor.ToArgb() != 0)
                        g.DrawLine(new Pen(_LineColor, LineWidth), new Point(X, Y), new Point(X + Width, Y));
                    else
                    {
                        if (flag)
                            g.DrawLine(pLine, new Point(X, Y), new Point(X + Width, Y));
                    }
                    break;

                // **************************************************************************
                // 5. pLineNE 대각선 Line(↘)
                // **************************************************************************
                case DrawObjectType.pLineNE:
                    if (_LineColor.ToArgb() != 0)
                        g.DrawLine(new Pen(_LineColor, LineWidth), new Point(X, Y), new Point(X + Width, Y + Height));
                    else
                    {
                        if (flag)
                            g.DrawLine(pLine, new Point(X, Y), new Point(X + Width, Y + Height));
                    }
                    break;

                // **************************************************************************
                // 6. pLineNW 대각선 Line(↙)
                // **************************************************************************
                case DrawObjectType.pLineNW:
                    if (_LineColor.ToArgb() != 0)
                        g.DrawLine(new Pen(_LineColor, LineWidth), new Point(X + Width, Y), new Point(X, Y + Height));
                    else
                    {
                        if (flag)
                            g.DrawLine(pLine, new Point(X + Width, Y), new Point(X, Y + Height));
                    }
                    break;

                // **************************************************************************
                // 7. pImage 이미지
                // **************************************************************************
                case DrawObjectType.pImage:
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    if (Img != null)
                        g.DrawImage(Img, DrawRect);
                    g.PixelOffsetMode = PixelOffsetMode.Default;
                    if (_LineColor.ToArgb() != 0)
                        g.DrawRectangle(new Pen(new SolidBrush(_LineColor), LineWidth), DrawRect);
                    else
                    {
                        if (flag)
                            g.DrawRectangle(pLine, DrawRect);
                    }
                    g.DrawString(Text.Replace("\\n", "\n"), FontStyle, new SolidBrush(_FontColor), DrawRect, format);
                    break;

                // **************************************************************************
                // 8. pButton 버튼
                // **************************************************************************
                case DrawObjectType.pButton:
                    pButtonInnerLine = new Pen(new LinearGradientBrush(DrawRect, Color.FromArgb(50, _LineColor), Color.FromArgb(150, _LineColor), LinearGradientMode.ForwardDiagonal), 2.0f);
                    if (_FillColor.ToArgb() != 0)
                    {
                        g.FillPath(new SolidBrush(_FillColor), rRect); // 라운드 사각형 그리기
                        if (_LineColor != Color.Blue)
                        {
                            // 버튼다운 상태가 아니라면 그라데이션을 넣어서 입체적인 느낌을 낸다.
                            g.FillRectangle(new LinearGradientBrush(tempRect, Color.FromArgb(140, Color.White), Color.FromArgb(20, Color.White), 90), tempRect.X, tempRect.Y, tempRect.Width, tempRect.Height / 2);
                        }
                        else
                        {
                            // 버튼다운 상태면 안쪽에 음영사각형을 넣어서 버튼이 들어간 느낌을 낸다.
                            g.DrawRectangle(new Pen(new SolidBrush(Color.FromArgb(60, Color.Black)), 3), tempRect.X + 2, tempRect.Y + 2, tempRect.Width - 4, tempRect.Height -4);                            
                        }
                    }
                    if (_Visible)
                    {

                        // 버튼 테두리는 검정에 두께1 고정
                        g.DrawPath(pButtonOutLine, rRect);
                        // 버튼 안 테두리는 라인색에 두께2        
                        g.DrawPath(pButtonInnerLine, DrawHelper.CreateRoundedRectanglePath(tempRect, 4));
                        // 선택된 버튼은 점선 사각형으로 포커스를 활성화 한다.
                        if (bFocus)
                        {
                             //g.DrawRectangle(pButtonGuide, tempRect.X + 1, tempRect.Y + 1, tempRect.Width - 2, tempRect.Height - 2);
                            g.DrawRectangle(pLine, tempRect.X + 1, tempRect.Y + 1, tempRect.Width - 2, tempRect.Height - 2);
                        }
                    }
                    else
                    {
                        if (flag)
                            g.DrawRectangle(pLine, DrawRect);
                    }
                    // 텍스트 그리기
                    g.DrawString(Text.Replace("\\n", "\n"), FontStyle, new SolidBrush(_FontColor), DrawRect, format);
                    break;

                // **************************************************************************
                // 9. pDiamond 마름모
                // **************************************************************************
                case DrawObjectType.pDiamond:
                    GraphicsPath dRect = DrawHelper.CreateDiamondPath(DrawRect);
                    if (_FillColor.ToArgb() != 0)
                        g.FillPath(new SolidBrush(_FillColor), dRect);
                    if (_LineColor.ToArgb() != 0)
                        g.DrawPath(new Pen(new SolidBrush(_LineColor), LineWidth), dRect);
                    else
                    {
                        if (flag)
                            g.DrawRectangle(pLine, DrawRect);
                    }
                    g.DrawString(Text, FontStyle, new SolidBrush(_FontColor), DrawRect, format);
                    break;

                // **************************************************************************
                // 10. pLine 라인 (세로선)
                // **************************************************************************
                case DrawObjectType.pLineV:
                    if (_LineColor.ToArgb() != 0)
                        g.DrawLine(new Pen(_LineColor, LineWidth), new Point(X, Y), new Point(X, Y + Height));
                    else
                    {
                        if (flag)
                            g.DrawLine(pLine, new Point(X, Y), new Point(X, Y + Height));
                    }
                    break;

                // **************************************************************************
                // 11. pRoundRect 라운드사각형
                // **************************************************************************
                case DrawObjectType.pRoundRect:

                    GraphicsPath path = new GraphicsPath();
                    // 상단선
                    path.AddLine(new Point(DrawRect.X + (DrawRect.Height / 2), DrawRect.Y), new Point(DrawRect.X + DrawRect.Width - (DrawRect.Height / 2), DrawRect.Y));
                    // 우측 Arc
                    path.AddArc(new Rectangle(DrawRect.X + DrawRect.Width - (DrawRect.Height), DrawRect.Y, DrawRect.Height, DrawRect.Height), -90, 180);
                    // 하단선
                    path.AddLine(new Point(DrawRect.X + (DrawRect.Height / 2), DrawRect.Y + DrawRect.Height), new Point(DrawRect.X + DrawRect.Width - (DrawRect.Height / 2), DrawRect.Y + DrawRect.Height));
                    // 좌측 Arc
                    path.AddArc(new Rectangle(DrawRect.X, DrawRect.Y, DrawRect.Height, DrawRect.Height), 90, 180);


                    if (_FillColor.ToArgb() != 0)
                    {
                        //g.FillRectangle(new SolidBrush(_FillColor), DrawRect);
                        g.FillPath(new SolidBrush(_FillColor), path);
                    }
                    if (_LineColor.ToArgb() != 0)
                    {
                        //g.DrawPath(new Pen(new SolidBrush(_LineColor), LineWidth), DrawHelper.CreateRoundedRectanglePath(DrawRect, 45));
                        g.DrawPath(new Pen(new SolidBrush(_LineColor), LineWidth), path);
                    }
                    else
                    {
                        if (flag)
                            g.DrawPath(pLine, path);
                    }
                    g.DrawString(Text.Replace("\\n", "\n"), FontStyle, new SolidBrush(_FontColor), DrawRect, format);                    
                    break;

            }
            g.ResetTransform();
            bChanged = false;
        }
        #endregion

        /// <summary>
        /// 애니메이션을 나타냅니다.
        /// </summary>
        /// <param name="type">애니메이션 타입</param>
        /// <param name="TargetValue">애니메이션 값(이동x축,y축 등등)</param>
        /// <param name="msec">타겟값까지 도달하는 시간(msec)</param>
        public void Animation(AnimationType type, int TargetValue, int msec)
        {
            this.AnimationType = type;
            this.AnimationToValue = TargetValue;
            this.AnimationSetMsec = (int)(msec*0.6f);

            switch (type)
            {
                case AnimationType.Move_X:
                    AnimationFromValue = this.X;
                    break;
                case AnimationType.Move_Y:
                    AnimationFromValue = this.Y;
                    break;
                case AnimationType.Width:
                    AnimationFromValue = this.Width;
                    break;
                case AnimationType.Height:
                    AnimationFromValue = this.Height;
                    break;
                case AnimationType.Rotation:
                    AnimationFromValue = (int)this.Angle1;
                    break;
                case AnimationType.Arc:
                    AnimationFromValue = (int)this.Angle2;
                    break;
            }
            this.AnimationActValue = (float)AnimationFromValue;
        }

        /// <summary>
        /// 애니메이션을 정지합니다.
        /// </summary>
        public void AnimationClear()
        {
            this.AnimationType = AnimationType.None;
            this.AnimationFromValue = 0;
            this.AnimationToValue = 0;
            this.AnimationSetMsec = 0;
            this.AnimationActValue = 0;
        }
    }

    public enum LinkMode
    {
        None,
        Ready,
        Linking
    }

    public enum AnimationType
    {
        None,
        Move_X,
        Move_Y,
        Width,
        Height,
        Arc,
        Rotation
    }

    public class DrawLinkage
    {
        public LinkMode Mode = LinkMode.None;
        public DrawLinkage()
        {
        }
    }
}
