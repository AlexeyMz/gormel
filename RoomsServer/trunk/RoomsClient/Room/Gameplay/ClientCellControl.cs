﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gameplay;

namespace RoomsClient
{
	public class ClientCellControl : UserControl
	{
		public Cell<ClientPlayer> Cell { get; set; }
		public bool Marked { get; set; }
		public ClientCellControl(Cell<ClientPlayer> cell)
		{
			Cell = cell;
			DoubleBuffered = true;
			Marked = false;
			BackColor = Color.Transparent;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Cell.Symbol != null)
				e.Graphics.DrawImage(Cell.Symbol.Image, new Rectangle(1, 1, Width - 2, Height - 2));
			if (Marked)
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Red)), ClientRectangle);
			if (Cell.IsWall)
				e.Graphics.FillRectangle(new SolidBrush(Color.DimGray), ClientRectangle);
			if (Cell.Right)
				e.Graphics.DrawLine(new Pen(Color.Black), Width - 1, 0, Width - 1, Height - 1);
			if (Cell.Down)
				e.Graphics.DrawLine(new Pen(Color.Black), 0, Height - 1, Width - 1, Height - 1);
		}
	}
}
