﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Gameplay
{
	public enum Direction
	{
		Up = 0,
		Left = 1,
		Down = 2,
		Right = 3,
	}
	public class FiledCursor : ICloneable
	{
		public int X { get; set; }
		public int Y { get; set; }
		public Direction Direction { get; set; }
		public Point Position
		{
			get { return new Point(X, Y); }
		}
		public FiledCursor Mirror
		{
			get
			{
				int free = (int)Direction + 1;
				Direction newDirection = (Direction)((free + 1) % 4);
				int newX = X + Math.Abs(2 - free) - 1;
				int newY = Y - Math.Abs(3 - free) + 1;

				FiledCursor rv = new FiledCursor();
				rv.X = newX;
				rv.Y = newY;
				rv.Direction = newDirection;
				return rv;
			}
		}

		public object Clone()
		{
			var rv = new FiledCursor();
			rv.X = X;
			rv.Y = Y;
			rv.Direction = Direction;

			return rv;
		}
	}
}
