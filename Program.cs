//#define EXPERIMENT
//#undef EXPERIMENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossLogicSolver
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Clear();

            if (args.Length > 0)
            {
                if (args[0].ToLower().Equals("-a"))
                {
                    Console.WriteLine(string.Format("CrossLogic Solver - (c) {0}, by Taiyo Kato", DateTime.Today.Year));
                    Console.Read();
                    System.Environment.Exit(0); //exit
                }
                if (!Reader.ReadFromFile(args[0])) return;
#if (EXPERIMENT)
                new Experimental(true);
#else
                new Solver(true);
#endif

            }
            return;
        }
        

    
    }
    public struct Point
    {
        private int? _x;
        private int? _y;
        public int x
        {
            get { return (_x.HasValue) ? _x.Value : -1; }
            set { _x = value; }
        }
        public int y
        {
            get { return (_y.HasValue) ? _y.Value : -1; }
            set { _y = value; }
        }

        public Point(int X, int Y) : this()
        {
            x = X;
            y = Y;
        }
        public Point(Point p)
            : this()
        {
            x = p.x;
            y = p.y;
        }
    }
}
