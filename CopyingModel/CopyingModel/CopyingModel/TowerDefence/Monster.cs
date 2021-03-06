﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CopyingModel
{
	public class Monster : LandEntery
	{
		public enum Status { }

		public int Radius { get; set; }
		public int Life { get; set; }

		private TimeSpan lastCreation;
		private static TimeSpan creationTimeout = TimeSpan.FromMilliseconds(6000);

		public static int Loot { get { return 20; } }

		public Monster(Point pos)
			: base(pos)
		{
			Radius = 4;
		}

		public override void Update(GameTime time)
		{
			if ((lastCreation += time.ElapsedGameTime) < creationTimeout)
				return;
			lastCreation = TimeSpan.FromMilliseconds(0);

			FireStateChanged(LandEntery.State.MonsterCreated, this, Position);
		}
	}
}
