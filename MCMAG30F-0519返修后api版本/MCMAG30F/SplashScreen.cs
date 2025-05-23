using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCMAG30F
{
    /// <summary>
    /// 启动画面
    /// </summary>
    public partial class SplashScreen : Form
    {
        /// <summary> 
        /// 启动画面本身 
        /// </summary> 
        static SplashScreen instance;
        /// <summary> 
        /// 显示的图片 
        /// </summary> 
        Bitmap bitmap;

        /// <summary>
        /// 启动画面单例
        /// </summary>
        public static SplashScreen Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SplashScreen()
        {
            InitializeComponent();

            // 设置窗体的类型 
            //const string showInfo = "启动画面：我们正在努力的加载程序，请稍后...";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            bitmap = new Bitmap(Properties.Resources.Logo, Properties.Resources.Logo.Width/2, Properties.Resources.Logo.Height / 2);
            ClientSize = bitmap.Size;
            //using (Font font = new Font("Consoles", 10))
            //{
            //    using (Graphics g = Graphics.FromImage(bitmap))
            //    {
            //        g.DrawString(showInfo, font, Brushes.White, 130, 100);
            //    }
            //}
            BackgroundImage = bitmap;
        }

        /// <summary>
        /// 显示启动画面
        /// </summary>
        public static void ShowSplashScreen()
        {
            instance = new SplashScreen();
            instance.Show();
        }
    }
}
