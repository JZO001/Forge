/* *********************************************************************
 * Date: 28 Feb 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

namespace Forge
{

    /// <summary>
    /// Void delegate without parameter
    /// </summary>
    public delegate void Action();

    /// <summary>
    /// Void delegate with parameter
    /// </summary>
    /// <typeparam name="T">Type of the parameter</typeparam>
    /// <param name="p1">The p1.</param>
    public delegate void Action<T>(T p1);

    /// <summary>
    /// Void delegate with parameters
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    public delegate void Action<T1, T2>(T1 p1, T2 p2);

    /// <summary>
    /// Void delegate with parameters
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    public delegate void Action<T1, T2, T3>(T1 p1, T2 p2, T3 p3);

    /// <summary>
    /// Void delegate with parameters
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    public delegate void Action<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4);

    /// <summary>
    /// Void delegate with parameters
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="p5">The p5.</param>
    public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);

    /// <summary>
    /// Void delegate with parameters
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="p5">The p5.</param>
    /// <param name="p6">The p6.</param>
    public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);

    /// <summary>
    /// Void delegate with parameters
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <typeparam name="T7">The type of the 7.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="p5">The p5.</param>
    /// <param name="p6">The p6.</param>
    /// <param name="p7">The p7.</param>
    public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);

    /// <summary>
    /// Void delegate with parameters
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <typeparam name="T7">The type of the 7.</typeparam>
    /// <typeparam name="T8">The type of the 8.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="p5">The p5.</param>
    /// <param name="p6">The p6.</param>
    /// <param name="p7">The p7.</param>
    /// <param name="p8">The p8.</param>
    public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8);

    /// <summary>
    /// Function delegate without parameter
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <returns>TResult</returns>
    public delegate TResult Func<TResult>();

    /// <summary>
    /// Function delegate with parameter(s)
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <returns>TResult</returns>
    public delegate TResult Func<T1, TResult>(T1 p1);

    /// <summary>
    /// Function delegate with parameter(s)
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <returns>TResult</returns>
    public delegate TResult Func<T1, T2, TResult>(T1 p1, T2 p2);

    /// <summary>
    /// Function delegate with parameter(s)
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <returns>TResult</returns>
    public delegate TResult Func<T1, T2, T3, TResult>(T1 p1, T2 p2, T3 p3);

    /// <summary>
    /// Function delegate with parameter(s)
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <returns>TResult</returns>
    public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 p1, T2 p2, T3 p3, T4 p4);

    /// <summary>
    /// Function delegate with parameter(s)
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="p5">The p5.</param>
    /// <returns>TResult</returns>
    public delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);

    /// <summary>
    /// Function delegate with parameter(s)
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="p5">The p5.</param>
    /// <param name="p6">The p6.</param>
    /// <returns>TResult</returns>
    public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);

    /// <summary>
    /// Function delegate with parameter(s)
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <typeparam name="T7">The type of the 7.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="p5">The p5.</param>
    /// <param name="p6">The p6.</param>
    /// <param name="p7">The p7.</param>
    /// <returns>TResult</returns>
    public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);

    /// <summary>
    /// Function delegate with parameter(s)
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <typeparam name="T7">The type of the 7.</typeparam>
    /// <typeparam name="T8">The type of the 8.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="p5">The p5.</param>
    /// <param name="p6">The p6.</param>
    /// <param name="p7">The p7.</param>
    /// <param name="p8">The p8.</param>
    /// <returns>TResult</returns>
    public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8);

}
