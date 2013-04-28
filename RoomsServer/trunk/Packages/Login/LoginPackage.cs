﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packages
{
	[Package(PackageType.Login)]
	public class LoginPackage : Package
	{
		[Data(0)]
		public String Name { get; set; }

		[Data(1)]
		public string PasswordMD5 { get; set; }
	}
}
