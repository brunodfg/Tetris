/*
 * Author: Bruno Gonçalves - brunodfg@gmail.com
 * Date: 18/02/2012
 * 
 * **/

namespace brunodfg.tetris.engine
{
    /// <summary>
    /// The state of execution of a tetris game
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// 
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// 
        /// </summary>
        Running = 1,

        /// <summary>
        /// 
        /// </summary>
        Paused = 2,

        /// <summary>
        /// 
        /// </summary>
        GameOver = 3,
    }
}
