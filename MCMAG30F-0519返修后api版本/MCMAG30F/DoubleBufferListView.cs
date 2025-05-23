using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MCMAG30F
{
    public class DoubleBufferListView : ListView
    {
        public DoubleBufferListView()
        {
            SetStyle(ControlStyles.DoubleBuffer |
               ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.AllPaintingInWmPaint, true);

            this.FullRowSelect = false;     // 禁用全行选择
            this.View = View.Details;
            this.MouseClick += DoubleBufferListView_MouseClick;
            this.OwnerDraw = true;
            this.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(DoubleBufferListView_DrawColumnHeader);
            this.DrawSubItem += new DrawListViewSubItemEventHandler(DoubleBufferListView_DrawSubItem);

            UpdateStyles();
        }

        /// <summary>
        /// 列头绘制事件的处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DoubleBufferListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // 自定义列头的绘制，包括颜色
            e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, e.Bounds, Color.Black, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        // 子项绘制事件的处理方法
        void DoubleBufferListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // 自定义子项的绘制，包括颜色
            e.Graphics.FillRectangle(new SolidBrush(e.SubItem.BackColor), e.Bounds);
            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.SubItem.Font, e.Bounds, e.SubItem.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        /// <summary>
        /// 子项鼠标点击事件的处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoubleBufferListView_MouseClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewHitTestInfo hitTestInfo = listView.HitTest(e.Location);
            if (hitTestInfo.Item != null && hitTestInfo.SubItem != null)
            {
                // 清除之前的选中状态
                foreach (ListViewItem item in listView.Items)
                {
                    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                    {
                        subItem.Font = new System.Drawing.Font(subItem.Font, System.Drawing.FontStyle.Regular);
                        subItem.BackColor = Color.White;
                    }
                }

                // 选中当前点击的单元格
                hitTestInfo.SubItem.Font = new System.Drawing.Font(hitTestInfo.SubItem.Font, System.Drawing.FontStyle.Bold);
                hitTestInfo.SubItem.BackColor = Color.FromArgb(100, 0, 0, 255);
            }
        }
    }
}
