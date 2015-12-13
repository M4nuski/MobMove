using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobMove
{
    public partial class Form1 : Form
    {
        private Stopwatch timestuff = new Stopwatch();

        private bool selecting;
        private Point selectStartPoint, selectStopPoint;

        private Dictionary<UnitSizes, int> unitSizes = new Dictionary<UnitSizes, int> { { UnitSizes.Small, 30 }, { UnitSizes.Medium, 60 }, { UnitSizes.Large, 120 } };
        private List<IUnit> units = new List<IUnit>();

        private Pen unitPen = new Pen(Color.Gray, 2.5f);
        private Pen fromPen = new Pen(Color.CadetBlue, 2.0f);
        private Pen gotoPen = new Pen(Color.Chartreuse, 0.5f);
        private Pen unitSelectedPen = new Pen(Color.Blue, 2.5f);

        private const int tilesize = 120;
        private const int numXtiles = 7;
        private const int numYtiles = 4;
        private bool[,] tilemap;

        private Brush oddtileBrush = new SolidBrush(Color.BurlyWood);
        private Brush oddtilemapBrush = new HatchBrush(HatchStyle.HorizontalBrick, Color.BurlyWood);
        private Brush eventileBrush = new SolidBrush(Color.LemonChiffon);
        private Brush eventilemapBrush = new HatchBrush(HatchStyle.HorizontalBrick, Color.LemonChiffon);

        private Brush selectionBrush = new SolidBrush(Color.FromArgb(25, 0, 0, 255));

        private static bool tile(int x, int y)
        {
            return ((x % 2 == 0) ^ (y % 2 == 0));
        }

        private Brush tilebrushselector(int x, int y)
        {
            return tilemap[x, y]
                ? tile(x, y) ? eventilemapBrush : oddtilemapBrush
                : tile(x, y) ? eventileBrush : oddtileBrush;
        }

        public Form1()
        {
            InitializeComponent();

            tilemap = new bool[numXtiles, numYtiles];//todo replace with verctor / line defs
            tilemap[1, 0] = true;
            tilemap[1, 1] = true;
            tilemap[4, 1] = true;
            tilemap[4, 2] = true;
            tilemap[4, 3] = true;
            tilemap[5, 1] = true;

            units.Add(new baseUnit { Location = new Point(60, 60), Size = UnitSizes.Medium, Speed = 120 });

            units.Add(new baseUnit { Location = new Point(60, 180), Size = UnitSizes.Large, Speed = 50 });

            units.Add(new baseUnit { Location = new Point(180, 60), Size = UnitSizes.Small, Speed = 240 });

            units.Add(new baseUnit { Location = new Point(180, 180), Size = UnitSizes.Small, Speed = 140 });

            units.Add(new baseUnit { Location = new Point(360, 60), Size = UnitSizes.Medium, Speed = 100 });

            timestuff.Start();
        }

        private void OutputLabel_Paint(object sender, PaintEventArgs e)
        {
            var timedelta = timestuff.ElapsedMilliseconds;
            //redraw map
            for (var i = 0; i < numXtiles; i++)
            {
                for (var j = 0; j < numYtiles; j++)
                {
                    e.Graphics.FillRectangle(tilebrushselector(i, j), i * tilesize, j * tilesize, tilesize, tilesize);
                }
            }

            //update units
            Parallel.ForEach(units, unit => unit.UpdateMove(timedelta));

            ResolveUnitToMapCollisions();
            ResolveUnitToUnitCollisions();

            //redraw units
            foreach (var unit in units)
            {
                var hw = unitSizes[unit.Size];
                e.Graphics.DrawLine(fromPen, unit.Location, unit.LastLocation);
                e.Graphics.DrawEllipse(unit.Selected ? unitSelectedPen : unitPen, unit.Location.X - (hw / 2), unit.Location.Y - (hw / 2), hw, hw);
                e.Graphics.DrawLine(gotoPen, unit.Location, unit.TargetDestination);
            }


            if (selecting)
            {
                var x = Math.Min(selectStartPoint.X, selectStopPoint.X);
                var y = Math.Min(selectStartPoint.Y, selectStopPoint.Y);
                var w = Math.Abs(selectStartPoint.X - selectStopPoint.X);
                var h = Math.Abs(selectStartPoint.Y - selectStopPoint.Y);
                e.Graphics.FillRectangle(selectionBrush, x, y, w, h);
            }

            timestuff.Reset();
            timestuff.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            OutputLabel.Refresh();
        }

        private void OutputLabel_MouseDown(object sender, MouseEventArgs e)
        {
            selectStartPoint = e.Location;

        }

        private void OutputLabel_MouseLeave(object sender, EventArgs e)
        {
            selecting = false;
        }

        private void OutputLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectStopPoint = e.Location;
                selecting = true;
            }
        }

        private void OutputLabel_MouseUp(object sender, MouseEventArgs e)
        {

            if (selecting)
            {
                performselection();
            }
            else if (e.Button == MouseButtons.Right)
            {
                foreach (var unit in units)
                {
                    if (unit.Selected) unit.MoveTo(e.Location);
                }
            }
            else
                foreach (var unit in units)
                {
                    unit.ClearSelection();
                }

            selecting = false;
        }

        private void performselection()
        {
            var x = Math.Min(selectStartPoint.X, selectStopPoint.X);
            var y = Math.Min(selectStartPoint.Y, selectStopPoint.Y);
            var w = Math.Abs(selectStartPoint.X - selectStopPoint.X);
            var h = Math.Abs(selectStartPoint.Y - selectStopPoint.Y);

            foreach (var unit in units)
            {
                unit.CheckSelection(x, y, w, h);
            }
        }


        private void ResolveUnitToMapCollisions()
        {
            //todo
        }

        private void ResolveUnitToUnitCollisions()
        {
            //todo culling optimization
            for (var unitA = 0; unitA < units.Count; unitA++)
            {
                for (var unitB = 0; unitB < units.Count; unitB++)
                {
                    if (unitA != unitB)
                    {
                        //todo add to IUnits definition
                        var UA_Radius = unitSizes[units[unitA].Size]/2;
                        var UB_Radius = unitSizes[units[unitB].Size]/2;

                        var AB_Dist2 = D2(units[unitA].Location, units[unitB].Location);
                        var AB_radsum2 = (UA_Radius + UB_Radius) * (UA_Radius + UB_Radius);

                        if (AB_Dist2 < AB_radsum2)
                        {
                            //collision at x*slopedelta + datum = 0
                            var UA_Datum = units[unitA].LastLocation;
                            var UB_Datum = units[unitB].LastLocation;
                            var UA_Slope = SubPoint(units[unitA].Location, UA_Datum);
                            var UB_Slope = SubPoint(units[unitB].Location, UB_Datum);

                            var slope = SubPoint(UA_Slope, UB_Slope);
                            var datum = SubPoint(UA_Datum, UB_Datum);

                            //get quadratic factors
                            var c = sqr(datum.X) + sqr(datum.Y) - AB_radsum2;
                            var b = 2 * ((datum.X * slope.X) + (datum.Y * slope.Y));
                            var a2 = 2 * (sqr(slope.X) + sqr(slope.Y));

                            if (Math.Abs(a2) > float.Epsilon)
                            {
                                //resolve quadratic equation
                                var x1 = (-b + Math.Sqrt(sqr(b) - 2 * a2 * c)) / a2;
                                var x2 = (-b - Math.Sqrt(sqr(b) - 2 * a2 * c)) / a2;

                                if ((x1 > 0) || (x2 > 0))
                                {
                                    //clamp to datum
                                    if (x1 < 0) x1 = 0;
                                    if (x2 < 0) x2 = 0;

                                    //select nearest to datum
                                    var x = Math.Min(x1, x2);

                                    var UA_newpos = AddPoint(UA_Datum, ScalePoint(UA_Slope, x));

                                    //select best unit newpos
                                    var AB_Dist2a = D2(UA_newpos, units[unitB].Location);

                                    units[unitA].Location = UA_newpos;
                                }
                            }
                        }
                    }
                }
            }
        }



        private static int L2(Point p)
        {
            return (p.X * p.X) + (p.Y * p.Y);
        }
        private static int L2(int x, int y)
        {
            return (x * x) + (y * y);
        }
        private static int D2(Point p1, Point p2)
        {
            return L2(p2.X - p1.X, p2.Y - p1.Y);
        }

        private static float L(Point p)
        {
            return (float)Math.Sqrt((p.X * p.X) + (p.Y * p.Y));
        }
        private static float L(int x, int y)
        {
            return (float)Math.Sqrt((x * x) + (y * y));
        }
        private float D(Point p1, Point p2)
        {
            return L(p2.X - p1.X, p2.Y - p1.Y);
        }

        public static Point SubPoint(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
        public static Point ScalePoint(Point a, double scale)
        {
            return new Point((int)(a.X * scale), (int)(a.Y * scale));
        }
        public static Point AddPoint(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public float sqr(float a)
        {
            return a * a;
        }
    }

}        

/*
    NewPosX := Player.Pos.X + DeltaX;
    NewPosZ := Player.Pos.Z + DeltaZ;

    for l1 := 1 to NumChildObjects do begin
      CurrentObj := ChildObjects[l1];
      if CurrentObj.HasGColInf then begin

        RotObjCenter.X := CurrentObj.GrdCOLINF[1];
        RotObjCenter.Z := CurrentObj.GrdCOLINF[3];
        Rotate3DY(RotObjCenter,CurrentObj.Ang.Y*DegToRadFactor);
        RotObjCenter.X := RotObjCenter.X + CurrentObj.Pos.X;
        RotObjCenter.Z := RotObjCenter.Z + CurrentObj.Pos.Z;

        Dist := sqr(NewPosX - RotObjCenter.X) + sqr(NewPosZ - RotObjCenter.Z);
        if Dist <= sqr(Ray + CurrentObj.GrdCOLINF[4]) then begin

          HorHit:= true;
          if (PosY+Height) > CurrentObj.Pos.Y then begin
          if (PosY+MaxStep) < (CurrentObj.GrdCOLINF[2]+CurrentObj.Pos.Y) then begin
            //Change DeltaX and DeltaZ to strafe around
            ColAng := 180-(RadToDegFactor * ArcTan2(RotObjCenter.X-NewPosX,RotObjCenter.Z-NewPosZ));
            Colang := Colang - HDG;
            while colang < 0 do colang := colang + 360;
            HDG := HDG - 90;
            if colang > 180 then HDG := HDG + 180;
            RadHeading := HDG*DegToRadFactor;
            Spd := sqrt(sqr(DeltaX)+sqr(DeltaZ)) * abs(sin(DegToRadFactor*ColAng));
            DeltaX := Spd * sin(RadHeading);
            DeltaZ := -Spd * cos(RadHeading);
          end else if (NewGround < (CurrentObj.GrdCOLINF[2]+CurrentObj.Pos.Y)) then begin
            Newground := (CurrentObj.GrdCOLINF[2]+CurrentObj.Pos.Y);
            GroundChanged := true;
          end; // end VerHit
          end else begin
            if Roof > CurrentObj.Pos.Y then
            Roof := CurrentObj.Pos.Y;
            GroundChanged := true;
            Newground := HMG;
          end;
        end; // end Collide
      end; // end obj has coll info
    end; // end Child loop

    //Move
    Player.Pos.X := Player.Pos.X + DeltaX;
    Player.Pos.Z := Player.Pos.Z + DeltaZ;
*/
    

