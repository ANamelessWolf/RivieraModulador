using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Ritsu
{
    public static class PointSorting
    {
        #region Point Compare
        /// <summary>
        /// Compare two points of Point2d
        /// </summary>
        /// <param name="pt">The point to be compared</param>
        /// <param name="comparePt">The point to compare with</param>
        /// <param name="rule">The point compare rule</param>
        /// <returns>
        /// A point indicating the relative values of this instance and value.Return
        /// Value Description Less than zero This instance is less than value.-or- This
        /// instance is not a number (System.Double.NaN) and value is a number. Zero
        /// This instance is equal to value.-or- Both this instance and value are not
        /// a number (System.Double.NaN), System.Double.PositiveInfinity, or System.Double.NegativeInfinity.
        /// Greater than zero This instance is greater than value.-or- This instance
        /// is a number and value is not a number (System.Double.NaN).
        /// </returns>
        public static int CompareTo(this Point2d pt, Point2d comparePt, PointCompareable rule)
        {
            Double x0 = pt.X, x1 = comparePt.X;
            Double y0 = pt.Y, y1 = comparePt.Y;
            if (rule == PointCompareable.XY)
                return x0 != x1 ? x0.CompareTo(x1) : y0.CompareTo(y1);
            else if (rule == PointCompareable.YX)
                return y0 != y1 ? y0.CompareTo(y1) : x0.CompareTo(x1);
            else
                throw new RomioException(Errors.InvalidPointComparation);
        }
        /// <summary>
        /// Compare two points of Point3d
        /// </summary>
        /// <param name="pt">The point to be compared</param>
        /// <param name="comparePt">The point to compare with</param>
        /// <param name="rule">The point compare rule</param>
        /// <returns>
        /// A point indicating the relative values of this instance and value.Return
        /// Value Description Less than zero This instance is less than value.-or- This
        /// instance is not a number (System.Double.NaN) and value is a number. Zero
        /// This instance is equal to value.-or- Both this instance and value are not
        /// a number (System.Double.NaN), System.Double.PositiveInfinity, or System.Double.NegativeInfinity.
        /// Greater than zero This instance is greater than value.-or- This instance
        /// is a number and value is not a number (System.Double.NaN).
        /// </returns>
        public static int CompareTo(this Point3d pt, Point3d comparePt, PointCompareable rule)
        {
            Double x0 = pt.X, x1 = comparePt.X;
            Double y0 = pt.Y, y1 = comparePt.Y;
            Double z0 = pt.Z, z1 = comparePt.Z;
            switch (rule)
            {
                case PointCompareable.XYZ:
                    return x0 != x1 ? x0.CompareTo(x1) : y0 != y1 ? y0.CompareTo(y1) : z0.CompareTo(z1);
                case PointCompareable.XZY:
                    return x0 != x1 ? x0.CompareTo(x1) : z0 != z1 ? z0.CompareTo(z1) : y0.CompareTo(y1);
                case PointCompareable.YXZ:
                    return y0 != y1 ? y0.CompareTo(y1) : x0 != x1 ? x0.CompareTo(x1) : z0.CompareTo(z1);
                case PointCompareable.YZX:
                    return y0 != y1 ? y0.CompareTo(y1) : z0 != z1 ? z0.CompareTo(z1) : x0.CompareTo(x1);
                case PointCompareable.ZXY:
                    return z0 != z1 ? z0.CompareTo(z1) : x0 != x1 ? x0.CompareTo(x1) : y0.CompareTo(y1);
                case PointCompareable.ZYX:
                    return z0 != z1 ? z0.CompareTo(z1) : y0 != y1 ? y0.CompareTo(y1) : x0.CompareTo(x1);
                default:
                    throw new RomioException(Errors.InvalidPointComparation);
            }
        }
        #endregion
        #region Sorting Methods
        /// <summary>
        /// Sort a group of 2D points in an ascendent or in descendant order,
        /// The List is ordered using the merge sort method.
        /// </summary>
        /// <param name="pts">The group of points to be ordered.</param>
        /// <param name="orderBy">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The List put in order.</returns>
        public static Point2dCollection Sort(this Point2dCollection pts, PointCompareable compareType, SortOrder orderBy)
        {
            return new Point2dCollection(MergeSort(pts.OfType<Point2d>().ToList(), compareType, orderBy).ToArray());
        }
        /// <summary>
        /// Sort a group of 2D points in an ascendent or in descendant order,
        /// The List is ordered using the merge sort method.
        /// </summary>
        /// <param name="pts">The group of points to be ordered.</param>
        /// <param name="orderBy">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The List put in order.</returns>
        public static Point2d[] Sort(this Point2d[] pts, PointCompareable compareType, SortOrder orderBy)
        {
            return MergeSort(pts.ToList(), compareType, orderBy).ToArray();
        }
        /// <summary>
        /// Sort a group of 2D points in an ascendent or in descendant order,
        /// The List is ordered using the merge sort method.
        /// </summary>
        /// <param name="pts">The group of points to be ordered.</param>
        /// <param name="orderBy">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The List put in order.</returns>
        public static List<Point2d> Sort(this List<Point2d> pts, PointCompareable compareType, SortOrder orderBy)
        {
            return MergeSort(pts, compareType, orderBy);
        }
        /// <summary>
        /// Sort a group of 3D points in an ascendent or in descendant order,
        /// The List is ordered using the merge sort method.
        /// </summary>
        /// <param name="pts">The group of points to be ordered.</param>
        /// <param name="orderBy">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The List put in order.</returns>
        public static Point3dCollection Sort(this Point3dCollection pts, PointCompareable compareType, SortOrder orderBy)
        {
            return new Point3dCollection(MergeSort(pts.OfType<Point3d>().ToList(), compareType, orderBy).ToArray());
        }
        /// <summary>
        /// Sort a group of 3D points in an ascendent or in descendant order,
        /// The List is ordered using the merge sort method.
        /// </summary>
        /// <param name="pts">The group of points to be ordered.</param>
        /// <param name="orderBy">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The List put in order.</returns>
        public static Point3d[] Sort(this Point3d[] pts, PointCompareable compareType, SortOrder orderBy)
        {
            return MergeSort(pts.ToList(), compareType, orderBy).ToArray();
        }
        /// <summary>
        /// Sort a group of 3D points in an ascendent or in descendant order,
        /// The List is ordered using the merge sort method.
        /// </summary>
        /// <param name="pts">The group of points to be ordered.</param>
        /// <param name="orderBy">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The List put in order.</returns>
        public static List<Point3d> Sort(this List<Point3d> pts, PointCompareable compareType, SortOrder orderBy)
        {
            return MergeSort(pts, compareType, orderBy);
        }
        #endregion
        #region Mergesorting  Point2D
        /// <summary>
        /// Sort a group of 2D points in an ascendent or in descendant order,
        /// The List is ordered using the merge sort method.
        /// </summary>
        /// <param name="pts">The group of points to be ordered.</param>
        /// <param name="sortOrder">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The List put in order.</returns>
        static List<Point2d> MergeSort(List<Point2d> pts, PointCompareable compareType, SortOrder sortOrder)
        {
            if (pts.Count <= 1) return pts.ToList();
            List<Point2d> leftList, rightList;
            SplitList(pts, out leftList, out rightList);
            leftList = MergeSort(leftList, compareType, sortOrder);
            rightList = MergeSort(rightList, compareType, sortOrder);
            pts = MergeSort(leftList, rightList, compareType, sortOrder);
            return pts.ToList();
        }
        /// <summary>
        /// Split the list in a left list and the right list.
        /// </summary>
        /// <param name="pts">The point colection to be splitted in to list.</param>
        /// <param name="leftList">The left list to be used in the merge sort.</param>
        /// <param name="rightList">The right list to be used in the merge sort.</param>
        static void SplitList(List<Point2d> pts, out List<Point2d> leftList, out List<Point2d> rightList)
        {
            int half = (int)Math.Round(pts.Count / 2d);
            leftList = new List<Point2d>(half);
            rightList = new List<Point2d>(pts.Count - half);
            for (int i = 0; i < pts.Count; i++)
            {
                if (i < half) leftList.Add(pts[i]);
                else rightList.Add(pts[i]);
            }
        }
        /// <summary>
        /// Merge the 2D point List in the selected order.
        /// </summary>
        /// <param name="leftList">The left list used in the merge sort</param>
        /// <param name="rightList">The right list used in the merge sort</param>
        /// <param name="sortOrder">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The Lists merge in order.</returns>
        static List<Point2d> MergeSort(List<Point2d> leftList, List<Point2d> rightList, PointCompareable compareType, SortOrder sortingOrder)
        {
            List<Point2d> result = new List<Point2d>(leftList.Count + rightList.Count);
            while (leftList.Count > 0 || rightList.Count > 0)
            {

                if (sortingOrder == SortOrder.Ascending && leftList.Count > 0 && rightList.Count > 0)
                {
                    #region Ascending Code
                    if (leftList[0].CompareTo(rightList[0], compareType) == -1)
                    {
                        result.Add(leftList[0]);
                        leftList.RemoveAt(0);
                    }
                    else
                    {
                        result.Add(rightList[0]);
                        rightList.RemoveAt(0);
                    }
                    #endregion
                }
                else if (sortingOrder == SortOrder.Descending && leftList.Count > 0 && rightList.Count > 0)
                {
                    #region Descending Code
                    if (leftList[0].CompareTo(rightList[0], compareType) == 1)
                    {
                        result.Add(leftList[0]);
                        leftList.RemoveAt(0);
                    }
                    else
                    {
                        result.Add(rightList[0]);
                        rightList.RemoveAt(0);
                    }
                    #endregion
                }
                else if (leftList.Count > 0)
                {
                    result.Add(leftList[0]);
                    leftList.RemoveAt(0);
                }
                else if (rightList.Count > 0)
                {
                    result.Add(rightList[0]);
                    rightList.RemoveAt(0);
                }
            }
            return result;
        }
        #endregion
        #region Mergesorting  Point3D
        /// <summary>
        /// Sort a group of 3D points in an ascendent or in descendant order,
        /// The List is ordered using the merge sort method.
        /// </summary>
        /// <param name="pts">The group of points to be ordered.</param>
        /// <param name="sortOrder">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The List put in order.</returns>
        static List<Point3d> MergeSort(List<Point3d> pts, PointCompareable compareType, SortOrder sortOrder)
        {
            if (pts.Count <= 1) return pts.ToList();
            List<Point3d> leftList, rightList;
            SplitList(pts, out leftList, out rightList);
            leftList = MergeSort(leftList, compareType, sortOrder);
            rightList = MergeSort(rightList, compareType, sortOrder);
            pts = MergeSort(leftList, rightList, compareType, sortOrder);
            return pts.ToList();
        }
        /// <summary>
        /// Split the list in a left list and the right list.
        /// </summary>
        /// <param name="pts">The point colection to be splitted in to list.</param>
        /// <param name="leftList">The left list to be used in the merge sort.</param>
        /// <param name="rightList">The right list to be used in the merge sort.</param>
        static void SplitList(List<Point3d> pts, out List<Point3d> leftList, out List<Point3d> rightList)
        {
            int half = (int)Math.Round(pts.Count / 2d);
            leftList = new List<Point3d>(half);
            rightList = new List<Point3d>(pts.Count - half);
            for (int i = 0; i < pts.Count; i++)
            {
                if (i < half) leftList.Add(pts[i]);
                else rightList.Add(pts[i]);
            }
        }
        /// <summary>
        /// Merge the 3D point List in the selected order.
        /// </summary>
        /// <param name="leftList">The left list used in the merge sort</param>
        /// <param name="rightList">The right list used in the merge sort</param>
        /// <param name="sortOrder">The sorting order</param>
        /// <param name="compareType">How the points are ordered</param>
        /// <returns>The Lists merge in order.</returns>
        static List<Point3d> MergeSort(List<Point3d> leftList, List<Point3d> rightList, PointCompareable compareType, SortOrder sortingOrder)
        {
            List<Point3d> result = new List<Point3d>(leftList.Count + rightList.Count);
            while (leftList.Count > 0 || rightList.Count > 0)
            {

                if (sortingOrder == SortOrder.Ascending && leftList.Count > 0 && rightList.Count > 0)
                {
                    #region Ascending Code
                    if (leftList[0].CompareTo(rightList[0], compareType) == -1)
                    {
                        result.Add(leftList[0]);
                        leftList.RemoveAt(0);
                    }
                    else
                    {
                        result.Add(rightList[0]);
                        rightList.RemoveAt(0);
                    }
                    #endregion
                }
                else if (sortingOrder == SortOrder.Descending && leftList.Count > 0 && rightList.Count > 0)
                {
                    #region Descending Code
                    if (leftList[0].CompareTo(rightList[0], compareType) == 1)
                    {
                        result.Add(leftList[0]);
                        leftList.RemoveAt(0);
                    }
                    else
                    {
                        result.Add(rightList[0]);
                        rightList.RemoveAt(0);
                    }
                    #endregion
                }
                else if (leftList.Count > 0)
                {
                    result.Add(leftList[0]);
                    leftList.RemoveAt(0);
                }
                else if (rightList.Count > 0)
                {
                    result.Add(rightList[0]);
                    rightList.RemoveAt(0);
                }
            }
            return result;
        }
        #endregion
    }
}
