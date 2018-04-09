using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Fratal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitForm();
            InitTimer();
            InitTriangle();
            InitFratalTree();
        }

        //初始化屏保窗口
        void InitForm()
        {
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = Screen.PrimaryScreen.Bounds.Size;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
        }

        //UI定时器
        void InitTimer()
        {
            this.UITimer.Interval = 1000;//刷新频率
            this.UITimer.Enabled = true;
            this.UITimer.Tick += OnTimerTick;
            this.UITimer.Start();
        }

        //逻辑
        void OnTimerTick(object sender, EventArgs e)
        {
            //选择一个分形算法运算
            //重绘
            this.Invalidate();
        }

        //绘图
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            //绘制特定图形

            // DrawSierpinskiCarpet(g, 50, 50, ClientSize.Width - 100, ClientSize.Height - 100);
            //计算屏幕中心三角形坐标
            //DrawSierpinskiTriangle(g, A, B, C);
            DrawFratalTree(g);
        }


        Brush fillBrush = new SolidBrush(Color.WhiteSmoke);
        //绘制 谢尔宾斯基地毯
        void DrawSierpinskiCarpet(Graphics g, float x, float y, float width, float height)
        {
            if (width < 10 || height < 10) return;

            float subwidth = width / 3;
            float subheight = height / 3;
            float subx = x + subwidth;
            float suby = y + subheight;

            g.FillRectangle(fillBrush, subx, suby, subwidth, subheight);

            for(int i=0;i<3;i++)
            {
                for(int j=0;j<3;j++)
                {
                    if (i == 1 && j == 1) continue;
                    DrawSierpinskiCarpet(g, x+i*subwidth, y+j*subheight, subwidth, subheight);
                }
            }
        }

        PointF A ;
        PointF B ;
        PointF C ;
        //绘制 谢尔宾斯三角形
        void InitTriangle()
        {
            float triHeight = ClientSize.Height - 100;
            float siderLen = (float)(triHeight / Math.Sin(Math.PI/3));
            A = new PointF(ClientSize.Width / 2, 50);
            B = new PointF(A.X - siderLen / 2, A.Y + triHeight);
            C = new PointF(B.X + siderLen, B.Y);
        }
        void DrawSierpinskiTriangle(Graphics g, PointF A, PointF B, PointF C)
        {
            if (A.X - B.X < 10) return;

            PointF a = new PointF((A.X + B.X) / 2, (A.Y + B.Y) / 2);
            PointF b = new PointF((A.X + C.X) / 2, (A.Y + C.Y) / 2);
            PointF c = new PointF((B.X + C.X) / 2, (B.Y + C.Y) / 2);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(a, b);
            path.AddLine(b, c);
            path.AddLine(c, a);
            path.CloseAllFigures();
            g.FillRegion(fillBrush, new Region(path));

            DrawSierpinskiTriangle(g, A, a, b);
            DrawSierpinskiTriangle(g, a, B, c);
            DrawSierpinskiTriangle(g, b, c, C);
        }


        /// <summary>  
        /// 以中心点旋转Angle角度  
        /// </summary>  
        /// <param name="center">中心点</param>  
        /// <param name="p1">待旋转的点</param>  
        /// <param name="angle">旋转角度（弧度）</param>  
        private PointF PointRotate(PointF center,  PointF p1, double angle)
        {
            double x1 = (p1.X - center.X) * Math.Cos(angle) + (p1.Y - center.Y) * Math.Sin(angle) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angle) + (p1.Y - center.Y) * Math.Cos(angle) + center.Y;
            return new PointF((float)x1, (float)y1);
        }

        //求解一元二次方程组的两个根
        private double[] Root2(double a, double b, double c)
        {
            double[] Roots = new double[2];
            double Delt = b * b - 4 * a * c;
            if (Delt >= 0)
            {
                Roots[0] = (-b + Math.Sqrt(Delt)) / 2 * a;
                Roots[1] = (-b - Math.Sqrt(Delt)) / 2 * a;
                return Roots;
            }
            else
            {
                Roots = null;
                return Roots;
            }
        }

        //y=kx+b
        private void CalcLineBy2Point(PointF A, PointF B, out float k, out float b)
        {
            k = (A.Y - B.Y) / (A.X - B.X);
            b = (A.Y - k * A.X);
        }
        //求点C，位于AB延长线方向
        private PointF CalcPointOnlineByOffset(PointF A, PointF B, float len)
        {
            float Cx, Cy;
//             if(A.X == B.X)
//             {
//                 Cx = A.X;
//                 Cy = (A.Y < B.Y) ? B.Y + len : B.Y - len;
//             }
//             else if(false)
//             {
//                 float k; float b;
//                 CalcLineBy2Point(A, B, out k, out b);
//                 //y=kx+b
//                 //(cx-ax)^2+(cy-ay)^2=len^2
//                 float aa = k * k + 1;
//                 float bb = 2 * (b - B.Y * k - B.X);
//                 float cc = B.X * B.X + B.Y * B.Y + b * b - 2 * B.Y * b - len * len;
//                 double[] cx = Root2(aa, bb, cc);
//                 Cx = (float)cx[1];
//                 if (B.X > A.X && cx[0] > B.X || B.X < A.X && cx[0] < B.X)
//                 {
//                     Cx = (float)cx[0];
//                 }
//                 Cy = b + k * Cx;
// 
//             }else
            {
                //相似法
                Cx = B.X + (B.X - A.X) * 0.618f;
                Cy = B.Y + (B.Y - A.Y) * 0.618f;
            }

            return new PointF(Cx, Cy);
        }

        //最简单的分形树
        float pecent = 0.618f;//树干和树枝的长度比例
        float firstTall = 500;//第一个树干的长度
        double rotate = Math.PI / 3;//旋转60度
        void InitFratalTree()
        {
            firstTall = (ClientSize.Height - 100) * 0.4f;
        }
        Pen linePen = new Pen(Color.YellowGreen, 2);
        void DrawFratalTree(Graphics g)
        {
            PointF a = new PointF(ClientSize.Width*0.5f, ClientSize.Height-50);
            PointF b = new PointF(a.X, a.Y - firstTall);
            g.DrawLine(linePen, a, b);
            RealDrawFratalTree(g, a, b, firstTall);
        }

        //树干AB长度baselen
        void RealDrawFratalTree(Graphics g, PointF A, PointF B, float baselen)
        {
            if (baselen < 1) return;
            float sublen = baselen * pecent;
            PointF C= CalcPointOnlineByOffset(A, B, sublen);
            PointF p1 = PointRotate(B, C, rotate);
            PointF p2 = PointRotate(B, C, -rotate);

            g.DrawLine(linePen, B, p1);
            g.DrawLine(linePen, B, p2);

            RealDrawFratalTree(g, B, p1, sublen);
            RealDrawFratalTree(g, B, p2, sublen);
        }

    }
}
