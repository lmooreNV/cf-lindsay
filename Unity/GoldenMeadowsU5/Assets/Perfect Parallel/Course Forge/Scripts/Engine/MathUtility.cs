using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using ClipperLib;

namespace PerfectParallel
{
    /// <summary>
    /// Math utility class
    /// </summary>
    public static class MathUtility
    {
        #region Intersection Methods
        /// <summary>
        /// Checks only segment-intersection (not line-line)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns>true if intersects</returns>
        public static bool IsSegmentSegment(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            Vector2 a = p2 - p1;
            Vector2 b = p3 - p4;
            Vector2 c = p1 - p3;

            float alphaNumerator = b.y * c.x - b.x * c.y;
            float alphaDenominator = a.y * b.x - a.x * b.y;
            float betaNumerator = a.x * c.y - a.y * c.x;
            float betaDenominator = a.y * b.x - a.x * b.y;

            bool intersected = true;
            if (alphaDenominator == 0 || betaDenominator == 0)
            {
                intersected = false;
            }
            else
            {
                if (alphaDenominator > 0)
                {
                    if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
                    {
                        intersected = false;
                    }
                }
                else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
                {
                    intersected = false;
                }
                if (intersected && betaDenominator > 0)
                {
                    if (betaNumerator < 0 || betaNumerator > betaDenominator)
                    {
                        intersected = false;
                    }
                }
                else if (betaNumerator > 0 || betaNumerator < betaDenominator)
                {
                    intersected = false;
                }
            }
            return intersected;
        }
        /// <summary>
        /// Line line intersection
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public static Vector2? LineLine(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            Vector2 intersection = Vector2.zero;
            float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
            float x1lo, x1hi, y1lo, y1hi;

            Ax = p2.x - p1.x;
            Bx = p3.x - p4.x;

            if (Ax < 0)
            {
                x1lo = p2.x; x1hi = p1.x;
            }
            else
            {
                x1hi = p2.x; x1lo = p1.x;
            }

            if (Bx > 0)
            {
                if (x1hi < p4.x || p3.x < x1lo) return null;
            }
            else
            {
                if (x1hi < p3.x || p4.x < x1lo) return null;
            }

            Ay = p2.y - p1.y;
            By = p3.y - p4.y;

            if (Ay < 0)
            {
                y1lo = p2.y; y1hi = p1.y;
            }
            else
            {
                y1hi = p2.y; y1lo = p1.y;
            }

            if (By > 0)
            {
                if (y1hi < p4.y || p3.y < y1lo) return null;
            }
            else
            {
                if (y1hi < p3.y || p4.y < y1lo) return null;
            }

            Cx = p1.x - p3.x;
            Cy = p1.y - p3.y;
            d = By * Cx - Bx * Cy;
            f = Ay * Bx - Ax * By;

            if (f > 0)
            {
                if (d < 0 || d > f) return null;
            }
            else
            {
                if (d > 0 || d < f) return null;
            }

            e = Ax * Cy - Ay * Cx;

            if (f > 0)
            {
                if (e < 0 || e > f) return null;
            }
            else
            {
                if (e > 0 || e < f) return null;
            }

            if (f == 0) return null;
            num = d * Ax;
            intersection.x = p1.x + num / f;
            num = d * Ay;
            intersection.y = p1.y + num / f;
            return intersection;

        }
        #endregion

        #region Extension Methods
        /// <summary>
        /// Distance between two points
        /// </summary>
        /// <param name="point"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static float Distance(this Vector3 point, Vector3 other)
        {
            return Vector3.Distance(point, other);
        }

        /// <summary>
        /// Convert to Vector2 line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static List<Vector2> ToVector2(this List<Vector3> line)
        {
            List<Vector2> array = new List<Vector2>();
            for (int i = 0; i < line.Count; ++i)
                array.Add(new Vector2(line[i].x, line[i].z));
            return array;
        }
        /// <summary>
        /// Convert to IntPoint line
        /// </summary>
        /// <param name="points"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static List<IntPoint> ToIntPoint(this List<Vector2> points, float scale = 1)
        {
            List<IntPoint> array = new List<IntPoint>();
            for (int i = 0; i < points.Count; ++i)
                array.Add(new IntPoint((double)points[i].x * scale, (double)points[i].y * scale));
            return array;
        }
        /// <summary>
        /// Convert to Vector2 line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static List<Vector2> ToVector2(this List<IntPoint> line, float scale = 1)
        {
            List<Vector2> array = new List<Vector2>();
            for (int i = 0; i < line.Count; ++i)
                array.Add(new Vector2((float)line[i].X * scale, (float)line[i].Y * scale));
            return array;
        }
        /// <summary>
        /// Convert to Vector3Object line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static List<Vector3Object> ToVector3Object(this List<Vector3> line)
        {
            List<Vector3Object> array = new List<Vector3Object>();
            for (int i = 0; i < line.Count; ++i)
                array.Add(new Vector3Object(line[i]));
            return array;
        }
        /// <summary>
        /// Convert to Vector3 line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Vector3[] ToVector3(this Vector3Object[] line)
        {
            Vector3[] array = new Vector3[line.Length];
            for (int i = 0; i < line.Length; ++i)
                array[i].Set(line[i].x, line[i].y, line[i].z);
            return array;
        }

        /// <summary>
        /// Converts vector3 to vector2
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this Vector3 point)
        {
            return new Vector2(point.x, point.z);
        }

        /// <summary>
        /// Calculates the length of the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static float LineLength(this Vector3[] array)
        {
            float length = 0;
            for (int i = 1; i < array.Length; ++i) length += (array[i] - array[i - 1]).magnitude;
            return length;
        }
        /// <summary>
        /// Calculates the length of the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static float LineLength(this List<Vector2> line)
        {
            float length = 0;
            for (int i = 1; i < line.Count; ++i) length += (line[i] - line[i - 1]).magnitude;
            return length;
        }
        /// <summary>
        /// Calculates the length of the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static float LineLength(this List<Vector3> line)
        {
            float length = 0;
            for (int i = 1; i < line.Count; ++i) length += (line[i] - line[i - 1]).magnitude;
            return length;
        }

        /// <summary>
        /// Calculates, if the point is inside line polygon
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsInside(this Vector3 point, Vector3[] line)
        {
            int counter = 0;
            int i;
            double xinters;
            Vector3 p1 = Vector3.zero, p2 = Vector3.zero;
            int N = line.Length;

            if (line.Length != 0) p1 = line[0];
            for (i = 1; i <= N; i++)
            {
                p2 = line[i % N];
                if (point.z > FastMin(p1.z, p2.z))
                {
                    if (point.z <= FastMax(p1.z, p2.z))
                    {
                        if (point.x <= FastMax(p1.x, p2.x))
                        {
                            if (p1.z != p2.z)
                            {
                                xinters = (point.z - p1.z) * (p2.x - p1.x) / (p2.z - p1.z) + p1.x;
                                if (p1.x == p2.x || point.x <= xinters)
                                    counter++;
                            }
                        }
                    }
                }

                p1 = p2;
            }

            if (counter % 2 == 0) return false;
            else return true;
        }
        /// <summary>
        /// Calculates, if the point is inside line polygon
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsInside(this Vector3 point, List<Vector3> line)
        {
            int counter = 0;
            int i;
            double xinters;
            Vector3 p1 = Vector3.zero, p2 = Vector3.zero;
            int N = line.Count;

            if (line.Count != 0) p1 = line[0];
            for (i = 1; i <= N; i++)
            {
                p2 = line[i % N];
                if (point.z > FastMin(p1.z, p2.z))
                {
                    if (point.z <= FastMax(p1.z, p2.z))
                    {
                        if (point.x <= FastMax(p1.x, p2.x))
                        {
                            if (p1.z != p2.z)
                            {
                                xinters = (point.z - p1.z) * (p2.x - p1.x) / (p2.z - p1.z) + p1.x;
                                if (p1.x == p2.x || point.x <= xinters)
                                    counter++;
                            }
                        }
                    }
                }

                p1 = p2;
            }

            if (counter % 2 == 0) return false;
            else return true;
        }
        /// <summary>
        /// Calculates, if the point is inside line polygon
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsInside(this Vector2 point, Vector3[] line)
        {
            int counter = 0;
            int i;
            double xinters;
            Vector3 p1 = Vector3.zero, p2 = Vector3.zero;
            int N = line.Length;

            if (line.Length != 0) p1 = line[0];
            for (i = 1; i <= N; i++)
            {
                p2 = line[i % N];
                if (point.y > Mathf.Min(p1.z, p2.z))
                {
                    if (point.y <= Mathf.Max(p1.z, p2.z))
                    {
                        if (point.x <= Mathf.Max(p1.x, p2.x))
                        {
                            if (p1.z != p2.z)
                            {
                                xinters = (point.y - p1.z) * (p2.x - p1.x) / (p2.z - p1.z) + p1.x;
                                if (p1.x == p2.x || point.x <= xinters)
                                    counter++;
                            }
                        }
                    }
                }

                p1 = p2;
            }

            if (counter % 2 == 0) return false;
            else return true;
        }
        /// <summary>
        /// Calculates, if the point is inside line polygon
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsInside(this Vector2 point, List<Vector2> line)
        {
            int counter = 0;
            int i;
            double xinters;
            Vector2 p1 = Vector3.zero, p2 = Vector2.zero;
            int N = line.Count;

            if (line.Count != 0) p1 = line[0];
            for (i = 1; i <= N; i++)
            {
                p2 = line[i % N];
                if (point.y > FastMin(p1.y, p2.y))
                {
                    if (point.y <= FastMax(p1.y, p2.y))
                    {
                        if (point.x <= FastMax(p1.x, p2.x))
                        {
                            if (p1.y != p2.y)
                            {
                                xinters = (point.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                                if (p1.x == p2.x || point.x <= xinters)
                                    counter++;
                            }
                        }
                    }
                }
                p1 = p2;
            }

            if (counter % 2 == 0) return false;
            else return true;
        }

        /// <summary>
        /// Project point to [start;end] line
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Vector3 Project(this Vector3 point, Vector3 start, Vector3 end)
        {
            Vector3 pointToStart = point - start;
            Vector3 endToStart = end - start;
            float distance = endToStart.magnitude;

            Vector3 temp = endToStart;

            if (distance > float.Epsilon)
            {
                temp /= distance;
            }

            float projectedDistance = Mathf.Clamp(Vector3.Dot(temp, pointToStart), 0.0f, distance);
            return start + temp * projectedDistance;
        }
        /// <summary>
        /// Distance to [start;end] line
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float Distance(this Vector3 point, Vector3 start, Vector3 end)
        {
            return Vector3.Magnitude(Project(point, start, end) - point);
        }
        /// <summary>
        /// Minimum distance to line
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static float MinDistance(this Vector3 point, List<Vector3> line)
        {
            point.y = 0;
            float min = float.MaxValue;
            for (int i = 0; i < line.Count; ++i)
            {
                Vector3 p1 = line[(i + 0 >= line.Count ? i + 0 - line.Count : i + 0)];
                Vector3 p2 = line[(i + 1 >= line.Count ? i + 1 - line.Count : i + 1)];
                p1.y = 0;
                p2.y = 0;

                float distance = Distance(point, p1, p2);
                if (distance < min) min = distance;
            }

            return min;
        }
        /// <summary>
        /// Rotate point around the center by degree value
        /// </summary>
        /// <param name="point"></param>
        /// <param name="center"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector2 RotateAround(this Vector2 point, Vector2 center, float degrees)
        {
            Vector2 newPoint;

            newPoint.x = Mathf.Cos(degrees) * (point.x - center.x) - Mathf.Sin(degrees) * (point.y - center.y) + center.x;
            newPoint.y = Mathf.Sin(degrees) * (point.x - center.x) + Mathf.Cos(degrees) * (point.y - center.y) + center.y;

            return newPoint;
        }
        /// <summary>
        /// Create shell over the line
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="length"></param>
        /// <param name="scale"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static List<List<Vector2>> MakeShell(this List<List<Vector2>> polygons, float length, float scale = 1000, float threshold = 0.2f)
        {
            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            ClipperOffset co = new ClipperOffset();
            for (int i = 0; i < polygons.Count; ++i) co.AddPath(ToIntPoint(polygons[i], scale), JoinType.jtRound, EndType.etClosedPolygon);
            co.Execute(ref solution, length * scale);

            if (solution.Count == 0) return new List<List<Vector2>>();
            solution = Clipper.SimplifyPolygons(solution);
            if (solution.Count == 0) return new List<List<Vector2>>();
            for (int i = 0; i < solution.Count; ++i) solution[i] = Clipper.CleanPolygon(solution[i], -1);
            if (solution.Count == 0) return new List<List<Vector2>>();

            List<List<Vector2>> current = new List<List<Vector2>>();
            for (int s = 0; s < solution.Count; ++s)
            {
                List<Vector2> level = ToVector2(solution[s], 1.0f / scale);

                for (int i = 1; i < level.Count; ++i)
                    if ((level[i] - level[i - 1]).magnitude < Mathf.Abs(length) * (1 - threshold))
                    {
                        level.RemoveAt(i);
                        i = 0;
                        continue;
                    }
                if (level.Count <= 1) return new List<List<Vector2>>();
                if ((level[0] - level[level.Count - 1]).magnitude < Mathf.Abs(length) * (1 - threshold))
                    level.RemoveAt(level.Count - 1);
                if (level.Count <= 1) return new List<List<Vector2>>();
                for (int i = 1; i < level.Count; ++i)
                    if ((level[i] - level[i - 1]).magnitude > Mathf.Abs(length) * (1 + threshold))
                    {
                        level.Insert(i, level[i - 1] + (level[i] - level[i - 1]) / 2 + Vector2.one * 0.01f);
                        i = 0;
                        continue;
                    }
                if ((level[0] - level[level.Count - 1]).magnitude > Mathf.Abs(length) * (1 + threshold))
                    level.Insert(0, level[level.Count - 1] + (level[0] - level[level.Count - 1]) / 2 + Vector2.one * 0.01f);

                current.Add(level);
            }
            return current;
        }

        /// <summary>
        /// Angle between two zero-centered vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static float Angle(Vector3 a, Vector3 b, float step = 5)
        {
            a.Normalize();
            b.Normalize();
            float angle = Vector3.Angle(a, b);
            Vector3 c = Quaternion.Euler(0, -angle, 0) * a;
            if (Vector3.Angle(b, c) > step) angle = 360 - angle;
            return angle;
        }

        /// <summary>
        /// Get element at index
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static T GetAt<T>(this T[] array, int index, bool loop = true)
        {
            if (loop)
            {
                if (index < 0) return array[array.Length + index];
                if (index > array.Length - 1) return array[index - array.Length];
            }
            else
            {
                if (index < 0) return array[0];
                if (index > array.Length - 1) return array[array.Length - 1];
            }
            return array[index];
        }
        /// <summary>
        /// Get element at index
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static T GetAt<T>(this List<T> array, int index, bool loop = true)
        {
            if (loop)
            {
                if (index < 0) return array[array.Count + index];
                if (index > array.Count - 1) return array[index - array.Count];
            }
            else
            {
                if (index < 0) return array[0];
                if (index > array.Count - 1) return array[array.Count - 1];
            }
            return array[index];
        }
        /// <summary>
        /// Sweep distance in points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="index"></param>
        /// <param name="traveled"></param>
        /// <param name="loop"></param>
        /// <param name="forward"></param>
        /// <returns></returns>
        public static Vector3 SweepPoint(this Vector3[] points, int index, float traveled, bool loop = true, bool forward = true)
        {
            int length = points.Length;

            float distance = 0;
            if (forward)
            {
                for (int i = index + 1; i < length; ++i)
                {
                    Vector3 x0 = points.GetAt(i, loop);
                    Vector3 x1 = points.GetAt(i - 1, loop);

                    float local = (x1 - x0).magnitude;
                    if (distance + local < traveled) distance += local;
                    else return x0 + (x1 - x0).normalized * (traveled - distance);
                }

                if (loop)
                {
                    for (int i = 0; i < index + 1; ++i)
                    {
                        Vector3 x0 = points.GetAt(i, loop);
                        Vector3 x1 = points.GetAt(i - 1, loop);

                        float local = (x1 - x0).magnitude;
                        if (distance + local < traveled) distance += local;
                        else return x0 + (x1 - x0).normalized * (traveled - distance);
                    }
                }
                return points[length - 1];
            }
            else
            {
                for (int i = index; i > 0; --i)
                {
                    Vector3 x0 = points.GetAt(i, loop);
                    Vector3 x1 = points.GetAt(i - 1, loop);

                    float local = (x1 - x0).magnitude;
                    if (distance + local < traveled) distance += local;
                    else return x0 + (x1 - x0).normalized * (traveled - distance);
                }

                if (loop)
                {
                    for (int i = length; i > index; --i)
                    {
                        Vector3 x0 = points.GetAt(i, loop);
                        Vector3 x1 = points.GetAt(i - 1, loop);

                        float local = (x1 - x0).magnitude;
                        if (distance + local < traveled) distance += local;
                        else return x0 + (x1 - x0).normalized * (traveled - distance);
                    }
                }
                return points[0];
            }
        }

        /// <summary>
        /// Direction to the target
        /// </summary>
        /// <param name="point"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 Direction(this Vector3 point, Vector3 target)
        {
            return (target - point).normalized;
        }
        /// <summary>
        /// Direction to the target along XZ
        /// </summary>
        /// <param name="point"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 Direction2D(this Vector3 point, Vector3 target)
        {
            Vector3 temp = point;
            temp.y = 0;
            target.y = 0;
            return (target - temp).normalized;
        }
        
        /// Rearranges the points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="metersPerPoint">The meters per point.</param>
        /// <returns></returns>
        public static List<Vector3> RearrangePoints(this List<Vector3> points, float metersPerPoint)
        {
            float traveled = 0;
            for (int i = 1; i < points.Count; ++i)
            {
                float distance = (points[i] - points[i - 1]).magnitude;
                if (distance < metersPerPoint)
                {
                    points.RemoveAt(i);
                    i = 0;
                    continue;
                }
            }

            points.Add(points[0]);
            List<Vector3> line = new List<Vector3>();

            traveled = 0;
            for (int i = 1; i < points.Count; ++i)
            {
                float distance = (points[i] - points[i - 1]).magnitude;
                if (traveled + distance < metersPerPoint)
                {
                    traveled += distance;
                    continue;
                }
                else if (traveled + distance >= metersPerPoint)
                {
                    for (float x = metersPerPoint - traveled; x < distance; x += metersPerPoint)
                    {
                        traveled = distance - x;

                        Vector3 point = points[i - 1] + (points[i] - points[i - 1]).normalized * x;
                        //if (line.Count == 0) points.Add(point);
                        line.Add(point);
                    }
                }
            }

            return line;
        }

        /// <summary>
        /// Simple catmull-rom over the points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="slices"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static List<Vector3> CatmullRom(this Vector3[] points, int slices, bool loop)
        {
            IEnumerable<Vector3> array = (Interpolate.NewCatmullRom(points, slices, loop) as IEnumerable<Vector3>);
            List<Vector3> lines = new List<Vector3>();
            foreach (Vector3 v in array)
            {
                lines.Add(v);
                if (v == points[0] && lines.Count != 1) break;
            }
            return lines;
        }
        /// <summary>
        /// Simple catmull-rom over the points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="slices"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static List<Vector2> CatmullRom(this Vector2[] points, int slices, bool loop)
        {
            Vector3[] points3 = new Vector3[points.Length];
            for (int i = 0; i < points.Length; ++i) points3[i] = new Vector3(points[i].x, 0, points[i].y);

            IEnumerable<Vector3> array = (Interpolate.NewCatmullRom(points3, slices, loop) as IEnumerable<Vector3>);
            List<Vector2> lines = new List<Vector2>();
            foreach (Vector3 v in array)
            {
                lines.Add(new Vector2(v.x, v.z));
                if (v == points3[0] && lines.Count != 1) break;
            }
            return lines;
        }
        /// <summary>
        /// Smooth line
        /// </summary>
        /// <param name="points"></param>
        /// <param name="metersPerNode"></param>
        /// <param name="loop"></param>
        /// <param name="absorbAngle"></param>
        /// <returns></returns>
        public static Vector3[] SmoothLine(this Vector3[] points, float metersPerNode, bool loop = true, float absorbAngle = 30)
        {
            if (loop)
            {
                Vector3[] temp = new Vector3[points.Length + 1];
                points.CopyTo(temp, 0);
                temp[temp.Length - 1] = points[0];

                points = temp;
            }

            List<Vector3> newPoints = new List<Vector3>();

            float sum = 0;
            int i = 1;
            while (i != points.Length)
            {
                float distance = (points[i] - points[i - 1]).magnitude;
                for (float x = 0; x <= distance; x += 0.01f)
                {
                    sum += 0.01f;
                    if (sum > metersPerNode)
                    {
                        newPoints.Add(points[i - 1] + (points[i] - points[i - 1]).normalized * x);
                        sum -= metersPerNode;
                    }
                }
                i++;
            }

            for (i = 1; i < newPoints.Count - 1; ++i)
            {
                Vector3 p1 = newPoints[i - 1] - newPoints[i];
                Vector3 p2 = newPoints[i + 1] - newPoints[i];
                if (p1.magnitude < metersPerNode && p2.magnitude < metersPerNode)
                {
                    float angle = Vector3.Angle(p1, p2);
                    if (angle > 180 - absorbAngle)
                    {
                        newPoints.RemoveAt(i);
                        i = 0;
                        continue;
                    }
                }
            }

            return newPoints.ToArray();
        }
        #endregion

        #region Support Methods
        static float FastMin(float x, float y)
        {
            return (x < y ? x : y);
        }
        static float FastMax(float x, float y)
        {
            return (x > y ? x : y);
        }
        #endregion
    }
}