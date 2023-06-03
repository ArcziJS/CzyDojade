﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CzyDojade
{
    public class Route
    {
        public string Geometry { get; set; }
        public double Distance { get;  set; }

        public double Duration { get; set; }
        public int RouteIndex { get; set; }
    }
}