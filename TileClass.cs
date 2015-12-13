using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobMove
{

    class SmallSpotClass
    {
        public IUnit unit;
    }
    class MediumSpotClass
    {
        public IUnit unit;
        public SmallSpotClass leftSpot, rightSpot; 
    }
    class LargeSpotClass
    {
        public IUnit unit;
        public MediumSpotClass topSpot, bottomSpot;
    }

    class TileClass
    {
        public int height;
        public LargeSpotClass Spots;

        public TileClass()
        {
            //
        }

        public bool AddUnit(IUnit unit)
        {
            return false;
        }

        public bool Accessible(IUnit unit)
        {
            return false;
        }
    }
}
