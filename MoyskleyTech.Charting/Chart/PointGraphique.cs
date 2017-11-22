﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting
{
    public class PointGraphique
    {
        public object AdditionnalData
        {
            get; set;
        }
        public PointGraphique(double x, double y)
        {
            Absisce = x;
            Ordonee = y;
        }
        public double Absisce
        {
            get;
            set;
        }
        public double Ordonee
        {
            get;
            set;
        }
    }
}
