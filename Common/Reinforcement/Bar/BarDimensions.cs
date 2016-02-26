using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using System.Diagnostics;
 
namespace Shopdrawing.Reinforcement
{
    public class Dimensions : INotifyPropertyChanged
    {
        private double _a;
        public double A 
        {
            get { return _a; }
            set {
                _a = value;
                OnPropertyChanged("A");
            }
        }
        private double _b;
        public double B 
        {
            get { return _b; }
            set {
                _b = value;
                OnPropertyChanged("B");
            }
        }
        private double _c;
        public double C
        {
            get { return _c; }
            set
            {
                _c = value;
                OnPropertyChanged("C");
            }
        }
        private double _d;
        public double D
        {
            get { return _d; }
            set
            {
                _d = value;
                OnPropertyChanged("D");
            }
        }
        private double _e;
        public double E
        {
            get { return _e; }
            set
            {
                _e = value;
                OnPropertyChanged("E");
            }
        }
        private double _f;
        public double F
        {
            get { return _f; }
            set
            {
                _f = value;
                OnPropertyChanged("F");
            }
        }
        private double _r;
        public double R
        {
            get { return _r; }
            set
            {
                _r = value;
                OnPropertyChanged("R");
            }
        }

        public override string ToString()
        {
            return "Dimensions";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
