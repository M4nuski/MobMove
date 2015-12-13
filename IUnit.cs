using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MobMove
{
    enum UnitSizes
    {
        Small, Medium, Large
    }

    interface IUnit
    {
        Point LastLocation { get; } // for collision detection
        Point Location { get; set; }

        Point TargetDestination { get; set; }
        Point IntervalDestination { get; set; } //for path finding
        bool Moving { get; }
        bool Selected { get; set; }
        UnitSizes Size { get; }
        //todo add radius that update only on Size change to optimize hit-testing
        int Speed { get; }
        int MaxStep { get; } //for height limit
        //todo add height for flyers
        void MoveTo(Point Destination);
        void UpdateMove(long interval);
        void CheckSelection(int x, int y, int w, int h);
        void ClearSelection();

        //todo add context for collision detection of other units and map
    }

    class baseUnit : IUnit
    {
        public Point LastLocation { get; set; }
        public Point Location { get; set; }

        public Point TargetDestination { get; set; }
        public Point IntervalDestination { get; set; }
        public bool Moving { get; set; }
        public bool Selected { get; set; }
        public UnitSizes Size { get; set; }
        public int Speed { get; set; }
        public int MaxStep { get; set; }

        public void MoveTo(Point Destination)
        {
            TargetDestination = Destination;
            Moving = true;
        }

        public void UpdateMove(long interval)
        {
            LastLocation = Location;
            if (Moving)
            {
                var maxdelta = Speed * interval / 1000;
                var deltaX = TargetDestination.X - Location.X;
                var deltaY = TargetDestination.Y - Location.Y;
                var norm = Math.Sqrt(sqr(deltaX) + sqr(deltaY));
                var nextLocation = TargetDestination;

                if (norm > 0)
                {
                    if (norm <= maxdelta) nextLocation = new Point(TargetDestination.X, TargetDestination.Y);
                    else
                    {

                        deltaX = (int)(deltaX * maxdelta / norm);
                        deltaY = (int)(deltaY * maxdelta / norm);
                        nextLocation = new Point(Location.X + deltaX, Location.Y + deltaY);
                    }

                }
                else Moving = false;


                
                Location = nextLocation;
            }

            if (getDistance(Location, TargetDestination) == 0)
            {
                Moving = false;
            }

            
        }

        public void CheckSelection(int x, int y, int w, int h)
        {
            Selected = (Location.X >= x) && (Location.Y >= y) && (Location.X <= x + w) && (Location.Y <= y + h);
        }

        public void ClearSelection()
        {
            Selected = false;
        }

        private static int sqr(int a)
        {
            return a * a;
        }

        private static int getDistance(Point A, Point B)
        {
            return (int)Math.Sqrt(sqr(A.X - B.X) + sqr(A.Y - B.Y));
        }



    }


}
