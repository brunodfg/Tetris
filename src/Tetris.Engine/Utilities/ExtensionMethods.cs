/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace brunodfg.tetris.engine.utilities
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts a hex string to a color. Example: #FFCAFF00
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static Color ToColor(this string hexColor)
        {
            if (!string.IsNullOrWhiteSpace(hexColor))
            {
                try
                {
                    return Color.FromArgb(
                        Convert.ToByte(hexColor.Substring(1, 2), 16),
                        Convert.ToByte(hexColor.Substring(3, 2), 16),
                        Convert.ToByte(hexColor.Substring(5, 2), 16),
                        Convert.ToByte(hexColor.Substring(7, 2), 16));
                }
                catch (Exception)
                {
                    return Colors.Transparent;
                }
            }
            else
            {
                return Colors.Transparent;
            }
        }
    }
}
