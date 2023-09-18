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

        static void Main(string[] args)
        {
            Model model = new Model();
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
