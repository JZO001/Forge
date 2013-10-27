/* *********************************************************************
 * Date: 28 Feb 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge.Threading.Tasking
{

    /// <summary>
    /// Callback for end methods
    /// </summary>
    /// <param name="taskResult">Nongeneric TaskResult</param>
    public delegate void ReturnCallback(TaskResult taskResult);

    /// <summary>
    /// Callback for end methods
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="taskResult">Generic TaskResult</param>
    public delegate void ReturnCallback<TResult>(TaskResult<TResult> taskResult);

}
