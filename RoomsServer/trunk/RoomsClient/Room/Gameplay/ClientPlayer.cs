﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RoomsClient
{
	public class ClientPlayer
	{
		public Image Image { get; private set; }
		public string Name { get; private set; }
		public ClientPlayer(string name, Image image)
		{
			Image = image;
			Name = name;
		}
	}
}
