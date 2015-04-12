﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceMe
{
    public abstract class BaseObject
    {
        public Color Color { get; set; }
        public double Reflection { get; set; }

        public abstract Hit Intersections(Lay lay);
    }
}
