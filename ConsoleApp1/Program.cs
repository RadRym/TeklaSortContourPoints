using TSM = Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using Tekla.Structures.Model;
using System;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace ConsoleApp1
{
    internal class Program
    {
        public static Model model = new Model();
        public static void Main(string[] args)
        {
            addBlacha();
        }
        public static void addBlacha()
        {
            TSMUI.ModelObjectSelector selector = new TSMUI.ModelObjectSelector();
            Picker picker = new Picker();
            Beam beam = (Beam)picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT);
            if (beam == null)
                return;
            Solid solid = beam.GetSolid();
            Point pointXYZ = new Point( solid.MaximumPoint.X, solid.MaximumPoint.Y, solid.MaximumPoint.Z);
            Point pointXyZ = new Point( solid.MaximumPoint.X, solid.MinimumPoint.Y, solid.MaximumPoint.Z);
            Point pointxyZ = new Point( solid.MinimumPoint.X, solid.MinimumPoint.Y, solid.MaximumPoint.Z);
            Point pointxYZ = new Point( solid.MinimumPoint.X, solid.MaximumPoint.Y, solid.MaximumPoint.Z);
            Point pointXYz = new Point(solid.MaximumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z);
            Point pointXyz = new Point(solid.MaximumPoint.X, solid.MinimumPoint.Y, solid.MinimumPoint.Z);
            Point pointxyz = new Point(solid.MinimumPoint.X, solid.MinimumPoint.Y, solid.MinimumPoint.Z);
            Point pointxYz = new Point(solid.MinimumPoint.X, solid.MaximumPoint.Y, solid.MinimumPoint.Z);

            List<Point> points = new List<Point>
            {
                pointXYZ,
                pointXyZ,
                pointxyZ,
                pointxYZ,
                pointXYz,
                pointXyz,
                pointxyz,
                pointxYz
            };

            GraphicsDrawer graphicsDrawer = new GraphicsDrawer();
            for(int i = 0; i < points.Count; i++)
            {
                graphicsDrawer.DrawText(points[i], i.ToString(), new Color(0, 0, 0));
            }
            List<List<Point>> list = new List<List<Point>> 
            { 
                new List<Point>{ points[0], points[1], points[5], points[4] },
                new List<Point>{ points[1], points[2], points[6], points[5] },
                new List<Point>{ points[2], points[3], points[8], points[7] },
                new List<Point>{ points[3], points[0], points[4], points[8] },
            };

            int face = 1;
            bool bottom = true;
            float bottomOffset = 200;
            float horizontalOffset = 0;

            List<Point> FacePoints = list[face];
            CoordinateSystem coordinateSystem = new CoordinateSystem(FacePoints[2], GetVector(FacePoints[2], FacePoints[3]), GetVector(FacePoints[2], FacePoints[1]));
            TransformationPlane WorkPlane = new TransformationPlane(coordinateSystem);
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(WorkPlane);
            model.CommitChanges();

            if (bottom)
            {
                float width = 
                ContourPlate contourPlateBottom = new ContourPlate();
                ContourPoint contourPoint1 = new ContourPoint(new Point(FacePoints[2].X, FacePoints[2].Y, FacePoints[2].Z), null);
                contourPlateBottom.AddContourPoint(FacePoints[2] as ContourPoint);
            }
            

        }
        public static Vector GetVector(Point startPoint, Point endPoint)
        {
            startPoint = new Point(0.0, 0.0, 0.0); // Punkt początkowy
            endPoint = new Point(10.0, 5.0, 3.0);  // Punkt końcowy

            // Oblicz wektor różnicy między punktem końcowym a początkowym
            Vector vector = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);
            return vector;
        }
        public static void rotateContourPlate()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Zaznacz element...");

                    var selectedModelObject = new Picker().PickObject(Picker.PickObjectEnum.PICK_ONE_PART);

                    if (selectedModelObject is ContourPlate selectedContourPlate)
                    {
                        var contourPoints = selectedContourPlate.Contour.ContourPoints;
                        Point centerPoint = CalculateCenterPoint(contourPoints);
                        ArrayList arrayList = ConvertListToArrayList(SortPointsByAngle(contourPoints, centerPoint));
                        selectedContourPlate.Contour.ContourPoints = arrayList;
                        selectedModelObject.Modify();
                        model.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static List<Point> SortPointsByAngle(ArrayList points, Point centralPoint)
        {
            List<Point> list = ConvertArrayListToPoints(points);
            return list.OrderBy(point => Math.Atan2(point.Y - centralPoint.Y, point.X - centralPoint.X)).ToList();
        }
        static Point CalculateCenterPoint(ArrayList points)
        {
            List<Point> list = ConvertArrayListToPoints(points);
            if (points.Count == 0)
            {
                throw new ArgumentException("Lista punktów jest pusta.");
            }

            double sumX = 0;
            double sumY = 0;

            foreach (var point in list)
            {
                sumX += point.X;
                sumY += point.Y;
            }

            double centerX = sumX / list.Count;
            double centerY = sumY / list.Count;

            return new Point(centerX, centerY);
        }
        static List<Point> ConvertArrayListToPoints(ArrayList arrayList)
        {
            List<Point> pointList = new List<Point>();

            foreach (var item in arrayList)
            {
                if (item is Point point)
                {
                    pointList.Add(point);
                }
            }

            return pointList;
        }
        static ArrayList ConvertListToArrayList(List<Point> pointList)
        {
            ArrayList arrayList = new ArrayList();

            foreach (var point in pointList)
            {
                arrayList.Add(point);
            }

            return arrayList;
        }
    }
}
