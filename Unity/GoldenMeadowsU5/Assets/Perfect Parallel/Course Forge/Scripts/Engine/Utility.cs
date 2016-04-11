using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JsonFx.Json;
using JsonFx.Bson;
using JsonFx.Json.Resolvers;
using JsonFx.Serialization;

namespace PerfectParallel
{
    using CourseForge;

    public enum Units
    {
        Metric = 0,
        Imperial,
    }

    /// <summary>
    /// Engine utility, extension methods, enum methods
    /// convertion between types, constants
    /// </summary>
    public static class Utility
    {
        #region Constants
        public const float squareHalf = 0.2f;
        public const float holeRadius = 0.052f;//0.053975f-0.001975f
        public const float holeTransition = 0.0014f;
        public const float holeLip = 0.02f;
        public const float ballRadius = 0.021335f;
        public const float markerOffset = 2.7f;
        public const float metersToYards = 1.0936133f;
        public const float metersToFeet = 3.28084f;
        public const float metersToMiles = 0.000621371f;
        #endregion

        #region Serialization Methods
        /// <summary>
        /// Is string packed?
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsPacked(string text)
        {
            return text.Contains("__SEPARATOR__");
        }
        /// <summary>
        /// Write values to the string
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string StringWrite(params object[] list)
        {
            string text = "";

            for (int i = 0; i < list.Length; ++i)
                text += list[i].ToString() + "__SEPARATOR__";

            return text;
        }
        /// <summary>
        /// Read values from the string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] StringRead(string text)
        {
            return text.Split(new string[] { "__SEPARATOR__" }, StringSplitOptions.RemoveEmptyEntries);
        }
        /// <summary>
        /// Write object to string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public static string JsonWrite<T>(T t)
        {
            JsonWriter writer = new JsonWriter(new DataWriterSettings(new JsonResolverStrategy()));
            return writer.Write(t);
        }
        /// <summary>
        /// Read object from the string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public static T BsonRead<T>(byte[] bytes)
        {
            var bsonTokenizer = new BsonReader.BsonTokenizer();
            var tokens2 = bsonTokenizer.GetTokens(bytes);
            var jsonFormatter = new JsonWriter.JsonFormatter(new DataWriterSettings { PrettyPrint = false });
            string actualText = jsonFormatter.Format(tokens2);
            JsonReader reader = new JsonReader(new DataReaderSettings(new JsonResolverStrategy()));
            return reader.Read<T>(actualText);
        }
        /// <summary>
        /// Read object from the string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public static T JsonRead<T>(string text)
        {
            JsonReader reader = new JsonReader(new DataReaderSettings(new JsonResolverStrategy()));
            return reader.Read<T>(text);
        }
        #endregion

        #region Transform Methods
        /// <summary>
        /// Find child that starts with ""
        /// </summary>
        /// <param name="t"></param>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public static Transform FindChild(Transform t, string startsWith)
        {
            if (t.name.StartsWith(startsWith)) return t;

            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform ch = FindChild(t.GetChild(i), startsWith);
                if (ch != null) return ch;
            }
            return null;
        }

        /// <summary>
        /// Enable the behaviour
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="enabled"></param>
        public static void EnableBehaviour(this Component source, string name, bool enabled = true)
        {
            if (source)
            {
                Component component = source.GetComponent(name);
                if (component)
                {
                    try
                    {
                        Behaviour behaviour = (Behaviour)component;
                        if (behaviour) behaviour.enabled = enabled;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
        /// <summary>
        /// Enable behaviour
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="other"></param>
        public static void EnableBehaviour(this Component source, string name, Component other)
        {
            if (source)
            {
                Component component = source.GetComponent(name);
                if (component)
                {
                    try
                    {
                        Behaviour behaviour = (Behaviour)component;
                        Behaviour behaviourOther = (Behaviour)other;
                        if (behaviour && behaviourOther) behaviour.enabled = behaviourOther.enabled;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
        /// <summary>
        /// Disable the behaviour
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void DisableBehaviour(this Component value, string name)
        {
            EnableBehaviour(value, name, false);
        }
        #endregion

        #region Enum Methods
        /// <summary>
        /// Recieve names for T enum
        /// </summary>
        /// <typeparam name="T">should be type of enum</typeparam>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public static string[] EnumNames<T>()
        {
            string[] names = Enum.GetNames(typeof(T));

            for (int i = 0; i < names.Length; ++i)
                names[i] = GetName(names[i]);

            return names;
        }
        /// <summary>
        /// Recieve names for T enum matching Y or excluding y 
        /// </summary>
        /// <typeparam name="T">should be type of enum</typeparam>
        /// <typeparam string Y> The string to include or exclude</string>
        /// <typeparam name=" bool"> True to match false to exclude</typeparam>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public static string[] EnumNames<T>(string Y, bool match)
        {
            string[] names = Enum.GetNames(typeof(T));
            string[] matchnames;
            if (match == true)
            {
                matchnames = new string[1];
                foreach (string name in names)
                {
                    if (name == Y)
                    {
                        matchnames[0] = GetName(name);
                        break;
                    }
                }
            }
            else
            {
                matchnames = new string[names.Length - 1];
                int i = 0;
                foreach (string name in names)
                {
                    if (name != Y)
                    {
                        matchnames[i] = GetName(name);
                        i++;
                    }
                }
            }
            return matchnames;
        }
        /// <summary>
        /// Get enum value from enum string value and type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public static T FromName<T>(string name) where T : struct
        {
            return (T)Enum.Parse(typeof(T), Utility.RestoreName(name), true);
        }
        /// <summary>
        /// Get name from enum name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="spaceCharacter"></param>
        /// <returns></returns>
        public static string GetName(object name)
        {
            return name.ToString().Replace("_", " ");
        }
        /// <summary>
        /// Restore enum name from string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RestoreName(string name)
        {
            return name.Replace(" ", "_");
        }
        #endregion

        #region General Methods
        /// <summary>
        /// Get "TH" word based on the number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetTh(int number)
        {
            switch (number)
            {
                case 1:
                    return "ST";

                case 2:
                    return "ND";

                case 3:
                    return "RD";

                default:
                    return "TH";
            }
        }
        /// <summary>
        /// Convert from rgb components in range [0..255]
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Color FromRGB256(float r, float g, float b)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        }

        /// <summary>
        /// Unix time to DateTime
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime UnixToDateTime(double unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks);
        }

        /// <summary>
        /// Distance and height special text
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="height"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string DistanceHeight(float distance, float height, Units units)
        {
            return Distance(distance, units) + " " + Height(height, units);
        }
        /// <summary>
        /// Distance special text
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string Distance(float distance, Units units)
        {
            if (units == Units.Imperial)
            {
                int d = distance.ToIntUI(units);
                if (d == 0) return "0" + distance.ToIntShortNameUI(units);
                if (d < 1) return "<1" + distance.ToIntShortNameUI(units);
                return d + distance.ToIntShortNameUI(units);
            }
            else
            {
                float f = distance.ToFloatUI(units);
                if (f == 0) return "0" + distance.ToIntShortNameUI(units);
                if (f < 1) return "<1" + distance.ToIntShortNameUI(units);
                return f + distance.ToIntShortNameUI(units);
            }
        }
        /// <summary>
        /// Height special text
        /// </summary>
        /// <param name="height"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string Height(float height, Units units)
        {
            string arrows = (height >= 0 ? "▲" : "▼");
            int d = Math.Abs(height.ToIntUI(units));
            if (d == 0) return "0" + height.ToIntShortNameUI(units);
            if (d < 1) return arrows + "<1" + height.ToIntShortNameUI(units);
            return arrows + d + height.ToIntShortNameUI(units);
        }

        /// <summary>
        /// Raycast scene with the layerMask
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static RaycastHit? Raycast(Ray ray, int mask)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, mask)) return hit;
            return null;
        }
        /// <summary>
        /// Raycast scene with layerMask
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static RaycastHit? Raycast(Vector3 position, Vector3 direction, int mask)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, direction, out hit, Camera.main.farClipPlane, mask)) return hit;
            return null;
        }
        #endregion

        #region Extension Methods
        /// <summary>
        /// Bitwise AND
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public static bool AND<T>(this Enum type, T value)
        {
            return ((int)(object)type).AND((int)(object)value);
        }
        /// <summary>
        /// Bitwise AND
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obfuscation(Exclude = true)]
        public static bool AND(this int type, int value)
        {
            return (type & value) == value;
        }

        /// <summary>
        /// Convert to CamelCase
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string text)
        {
            text = Regex.Replace(text, @"(-[a-z])", m => "" + m.ToString().ToUpper());
            text = text.Replace("-", "");
            return text;
        }
        /// <summary>
        /// Convert to Snake_Case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSnake_Case(this string text)
        {
            text = Regex.Replace(text, @"(?<=[a-z])([A-Z])", m => "-" + m.ToString().ToLower());
            return text;
        }

        /// <summary>
        /// Convert to Color array
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Color[] ToColor(this ColorObject[] line)
        {
            Color[] array = new Color[line.Length];
            for (int i = 0; i < line.Length; ++i)
                array[i] = line[i];
            return array;
        }
        /// <summary>
        /// Convert to Color array
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Color[] ToColor(this List<ColorObject> line)
        {
            Color[] array = new Color[line.Count];
            for (int i = 0; i < line.Count; ++i)
                array[i] = line[i];
            return array;
        }

        /// <summary>
        /// Convert float to int (rounding to int)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this float value)
        {
            return Mathf.RoundToInt(value);
        }
        /// <summary>
        /// Convert float to int based on units (rounding to int)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static int ToInt(this float value, Units units)
        {
            if (units == Units.Imperial) return ToInt(value * Utility.metersToYards);
            return ToInt(value);
        }
        /// <summary>
        /// Converts float to units value, if value is less then minimum for this
        /// units, then it will convert to lesser units (i.e. 20 yards = 60 feet)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static int ToIntUI(this float value, Units units)
        {
            if (units == Units.Imperial)
            {
                float yards = value * Utility.metersToYards;
                if (Mathf.Abs(yards) < 30.0f)
                {
                    float feets = value * Utility.metersToYards * 3;
                    if (Mathf.Abs(feets) < 3.0f)
                    {
                        float inches = value * Utility.metersToYards * 3 * 12;
                        return ToInt(inches);
                    }
                    else
                    {
                        return ToInt(feets);
                    }
                }
                else
                {
                    return ToInt(yards);
                }
            }
            //if (units == ppMeasureInfo.Units.meters) 
            {
                float meters = value;
                if (Mathf.Abs(meters) < 1.0f)
                {
                    float centimeters = value * 100;
                    if (Mathf.Abs(centimeters) < 1.0f)
                    {
                        float millimeters = value * 100 * 10;
                        return ToInt(millimeters);
                    }
                    else
                    {
                        return ToInt(centimeters);
                    }
                }
                else
                {
                    return ToInt(meters);
                }
            }
        }
        /// <summary>
        /// Converts float to units value, if value is less then minimum for this
        /// units, then it will convert to lesser units (i.e. 20 yards = 60 feet)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static float ToFloatUI(this float value, Units units)
        {
            if (units == Units.Imperial)
            {
                float yards = value * Utility.metersToYards;
                if (Mathf.Abs(yards) < 30.0f)
                {
                    float feets = value * Utility.metersToYards * 3;
                    if (Mathf.Abs(feets) < 3.0f)
                    {
                        float inches = value * Utility.metersToYards * 3 * 12;
                        return ToInt(inches);
                    }
                    else
                    {
                        return ToInt(feets);
                    }
                }
                else
                {
                    return ToInt(yards);
                }
            }
            //if (units == ppMeasureInfo.Units.meters) 
            {
                float meters = value;
                if (Mathf.Abs(meters) < 1.0f)
                {
                    float centimeters = value * 100;
                    if (Mathf.Abs(centimeters) < 1.0f)
                    {
                        float millimeters = value * 100 * 10;
                        return ToInt(millimeters);
                    }
                    else
                    {
                        return ToInt(centimeters);
                    }
                }
                else
                {
                    return (float)Math.Round(meters, 1);
                }
            }
        }

        /// <summary>
        /// Converts float to units string, if value is less then minimum for this
        /// units, then it will convert to lesser units (i.e. 20 yards = 60 feet)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string ToIntNameUI(this float value, Units units)
        {
            if (units == Units.Imperial)
            {
                float yards = value * Utility.metersToYards;
                if (Mathf.Abs(yards) < 30.0f)
                {
                    float feets = value * Utility.metersToYards * 3;
                    if (Mathf.Abs(feets) < 3.0f)
                    {
                        float inches = value * Utility.metersToYards * 3 * 12;
                        if (ToInt(inches) == 1) return "Inch";
                        return "Inches";
                    }
                    else
                    {
                        if (ToInt(feets) == 1) return "Foot";
                        return "Feet";
                    }
                }
                else
                {
                    if (ToInt(yards) == 1) return "Yard";
                    return "Yards";
                }
            }
            //if (units == ppMeasureInfo.Units.meters) 
            {
                float meters = value;
                if (Mathf.Abs(meters) < 1.0f)
                {
                    float centimeters = value * 100;
                    if (Mathf.Abs(centimeters) < 1.0f)
                    {
                        float millimeters = value * 100 * 10;
                        if (ToInt(millimeters) == 1) return "mm";
                        return "mm";
                    }
                    else
                    {
                        if (ToInt(centimeters) == 1) return "cm";
                        return "cm";
                    }
                }
                else
                {
                    if (ToInt(meters) == 1) return "Meter";
                    return "Meters";
                }
            }
        }
        /// <summary>
        /// Converts float to units string one character, if value is less then minimum for this
        /// units, then it will convert to lesser units (i.e. 20 yards = 60 feet)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static string ToIntShortNameUI(this float value, Units units)
        {
            if (units == Units.Imperial)
            {
                float yards = value * Utility.metersToYards;
                if (Mathf.Abs(yards) < 30.0f)
                {
                    float feets = value * Utility.metersToYards * 3;
                    if (Mathf.Abs(feets) < 3.0f)
                    {
                        float inches = value * Utility.metersToYards * 3 * 12;
                        if (ToInt(inches) == 1) return "in";
                        return "in";
                    }
                    else
                    {
                        if (ToInt(feets) == 1) return "ft";
                        return "ft";
                    }
                }
                else
                {
                    if (ToInt(yards) == 1) return "y";
                    return "y";
                }
            }
            //if (units == ppMeasureInfo.Units.meters) 
            {
                float meters = value;
                if (Mathf.Abs(meters) < 1.0f)
                {
                    float centimeters = value * 100;
                    if (Mathf.Abs(centimeters) < 1.0f)
                    {
                        float millimeters = value * 100 * 10;
                        if (ToInt(millimeters) == 1) return "mm";
                        return "mm";
                    }
                    else
                    {
                        if (ToInt(centimeters) == 1) return "cm";
                        return "cm";
                    }
                }
                else
                {
                    if (ToInt(meters) == 1) return "m";
                    return "m";
                }
            }
        }
        /// <summary>
        /// Converts to string with 2 characters (i.e. 1 = "01")
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToStringTwoChar(this int number)
        {
            if (number < 10) return "0" + number;
            return number.ToString();
        }
        /// <summary>
        /// Converts to string with 3 characters (i.e. 20 = "020")
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToStringThreeChar(this int number)
        {
            string text = "";
            if (number < 100) text += "0";
            if (number < 10) text += "0";
            return text + number;
        }
        /// <summary>
        /// To Par string, based on score and method of play
        /// </summary>
        /// <param name="score"></param>
        /// <param name="strokePlay"></param>
        /// <param name="match"></param>
        /// <param name="skins"></param>
        /// <returns></returns>
        public static string ToParString(this int score, bool strokePlay, bool match, bool skins)
        {
            if (strokePlay || skins)
            {
                if (score == 0) return "E";
                if (score > 0) return "+" + score.ToString();
                else return score.ToString();
            }
            if (match)
            {
                if (score == 0) return "All SQ";
                if (score > 0) return score.ToString() + " UP";
                else return Math.Abs(score).ToString() + " DN";
            }
            //			if (skins)
            //		{
            //		return "$" + score.ToString();
            //}

            return "";
        }
        /// <summary>
        /// To par name based on strokes
        /// </summary>
        /// <param name="strokes"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        public static string ToParName(this int strokes, int par)
        {
            int diffToPar = strokes - par;
            string scoreName = "+" + diffToPar;
            switch (diffToPar)
            {
                case -3:
                    scoreName = (par == 4) ? "HOLE IN ONE!" : "ALBATROSS!";
                    break;
                case -2:
                    scoreName = (par == 3) ? "HOLE IN ONE!" : "EAGLE";
                    break;
                case -1:
                    scoreName = "Birdie";
                    break;
                case 0:
                    scoreName = "Par";
                    break;
                case 1:
                    scoreName = "Bogey";
                    break;
                case 2:
                    scoreName = "Double Bogey";
                    break;
                case 3:
                    scoreName = "Triple Bogey";
                    break;
                case 4:
                    scoreName = "Quadruple Bogey";
                    break;
                case 18:
                    scoreName = "Alex's Melancholy";
                    break;
            }
            return scoreName;
        }
        #endregion
    }
}