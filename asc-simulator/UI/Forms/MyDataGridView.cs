/*
 * MyDataGridView.cs
 * ブレークポイントなどに対応したDataGridViewクラス
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator.UI.Forms
{
    public class MyDataGridView : DataGridView
    {
        // ハイライト表示をするRowを指定
        // 表示の更新も同時に行う
        public int HighlightedIndex
        {
            get
            {
                return this._HighlightedIndex;
            }
            set
            {
                this._HighlightedIndex = value;

                this.Invalidate();
            }
        }

        public int SecondaryHighlightedIndex
        {
            get
            {
                return this._SecondaryHighlightedIndex;
            }
            set
            {
                this._SecondaryHighlightedIndex = value;

                this.Invalidate();
            }
        }


        public MyDataGridView()
            : base()
        {
            // ハイライトに使うブラシ
            this._HighlightBrush = new SolidBrush(Color.FromKnownColor(KnownColor.Salmon));
            this._SecondaryHighlightBrush = new SolidBrush(Color.FromKnownColor(KnownColor.Pink));

            // 行全体を選択するように
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // HeaderCellとしてMyDataGridViewRowHeaderCellを割り当てる
            this.RowTemplate.HeaderCell = new MyDataGridViewRowHeaderCell();
        }

        public void ScrollToIndexRow(int index)
        {
	        this.FirstDisplayedScrollingRowIndex = Math.Min(Math.Max(index, 0), 0xFFFF);
        }

        // ブレークポイントの表示をクリアする
        public void ClearChecks()
        {
            for (int i = 0; i < this.Rows.Count; i++)
            {
                ((MyDataGridViewRowHeaderCell)(this.Rows[i].HeaderCell)).IsChecked = false;
            }

            this.Invalidate();
        }

        // ブレークポイントのアイコンを表示するか否か
        public bool IsEnabledCheckIcon = false;

        #region Override
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Enterキーを押したとき、別のセルに選択が移動しないように
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                var currentCell = this.CurrentCell;
                this.EndEdit();
                this.CurrentCell = null;

                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override void SetSelectedRowCore(int rowIndex, bool selected)
        {
            // 行の選択をさせない
            base.SetSelectedRowCore(rowIndex, false);
        }

        protected override void OnRowHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            // ブレークポイントの表示をトグルする
            if (this.IsEnabledCheckIcon && !this.ReadOnly)
            {
                if (((MyDataGridViewRowHeaderCell)this.Rows[e.RowIndex].HeaderCell).IsChecked)
                {
                    ((MyDataGridViewRowHeaderCell)this.Rows[e.RowIndex].HeaderCell).IsChecked = false;
                    this.OnRowUnchecked(e);
                }
                else
                {
                    ((MyDataGridViewRowHeaderCell)this.Rows[e.RowIndex].HeaderCell).IsChecked = true;
                    this.OnRowChecked(e);
                }
            }

            base.OnRowHeaderMouseClick(e);
        }

        // ブレークポイントが設定・解除されたときに発生するイベント
        public event DataGridViewCellMouseEventHandler RowChecked;
        public event DataGridViewCellMouseEventHandler RowUnchecked;

        protected void OnRowChecked(DataGridViewCellMouseEventArgs e)
        {
            this.RowChecked(this, e);
            this.Invalidate();
        }
        protected void OnRowUnchecked(DataGridViewCellMouseEventArgs e)
        {
            this.RowUnchecked(this, e);
            this.Invalidate();
        }

        protected override void OnRowPrePaint(DataGridViewRowPrePaintEventArgs e)
        {
            base.OnRowPrePaint(e);

            // ハイライト行は背景の色を変える
            if (e.RowIndex == this._HighlightedIndex)
            {
                var offset = this.Columns[0].Width + this.RowHeadersWidth;
                e.Graphics.FillRectangle(Brushes.White, new Rectangle(e.RowBounds.X, e.RowBounds.Y, offset, e.RowBounds.Height));
                e.Graphics.FillRectangle(this._HighlightBrush, new Rectangle(e.RowBounds.X + offset, e.RowBounds.Y, e.RowBounds.Width - offset, e.RowBounds.Height));
                e.PaintParts &= ~DataGridViewPaintParts.Background;

                e.PaintHeader(true);
            }
            if (e.RowIndex == this._SecondaryHighlightedIndex)
            {
                var offset = this.Columns[0].Width + this.RowHeadersWidth;
                e.Graphics.FillRectangle(Brushes.White, new Rectangle(e.RowBounds.X, e.RowBounds.Y, offset, e.RowBounds.Height));
                e.Graphics.FillRectangle(this._SecondaryHighlightBrush, new Rectangle(e.RowBounds.X + offset, e.RowBounds.Y, e.RowBounds.Width - offset, e.RowBounds.Height));
                e.PaintParts &= ~DataGridViewPaintParts.Background;

                e.PaintHeader(true);
            }

        }
        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);

            if (e.RowIndex == this._HighlightedIndex)
            {
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnReadOnlyChanged(EventArgs e)
        {
            base.OnReadOnlyChanged(e);

            // ReadOnlyが変更されたら、セルの編集を解除する
            this.CancelEdit();
            this.CurrentCell = null;
        }

        #endregion

        private int _HighlightedIndex = -1;
        private int _SecondaryHighlightedIndex = -1;
        private Brush _HighlightBrush;
        private Brush _SecondaryHighlightBrush;
    }


    // ブレークポイントの表示に対応したHeaderCell
    public class MyDataGridViewRowHeaderCell : DataGridViewRowHeaderCell
    {
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            if (this.IsChecked)
            {
                int size = cellBounds.Height - 8;
                graphics.FillEllipse(Brushes.Red, cellBounds.Right -size-5, cellBounds.Top +2, size, size);
            }
        }

        public bool IsChecked = false;
    }

}
