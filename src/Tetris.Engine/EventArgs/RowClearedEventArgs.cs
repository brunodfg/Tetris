/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace brunodfg.tetris
{
    public class RowClearedEventArgs : EventArgs
    {
        /// <summary>
        /// The number of the cleared row
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowNumber"></param>
        public RowClearedEventArgs(int rowNumber)
        {
            this.RowNumber = rowNumber;
        }
    }
}
