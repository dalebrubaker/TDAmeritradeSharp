using System.ComponentModel;

namespace TDAmeritradeSharpUI;

public static class ControlExtensions
{
    /// <summary>
    ///     Thanks to https://stackoverflow.com/questions/7181756/invoke-or-begininvoke-cannot-be-called-on-a-control-until-the-window-handle-has
    /// </summary>
    /// <param name="control"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool SafeInvoke(this Control control, MethodInvoker method)
    {
        try
        {
            // ReSharper disable once PossibleNullReferenceException
            if (control.IsDisposed || !control.IsHandleCreated || control.FindForm()?.IsHandleCreated == false)
            {
                //s_logger.ConditionalTrace($"Ignoring SafeInvoke on {control}.{method}");
                return false;
            }
            if (control.InvokeRequired)
            {
                control.BeginInvoke(method);
                control.Invoke(method);
            }
            else
            {
                method();
            }
        }
        catch (InvalidAsynchronousStateException)
        {
            return false;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
        catch (Exception)
        {
            if (!control.IsDisposed)
            {
                throw;
            }
        }
        return true;
    }

    /// <summary>
    ///     Like SafeInvoke but is asynchronous, using BeginInvoke instead of Invoke. Returns immediately
    /// </summary>
    /// <param name="control"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool SafeInvokeAsync(this Control control, MethodInvoker method)
    {
        try
        {
            // ReSharper disable once PossibleNullReferenceException
            if (control.IsDisposed || !control.IsHandleCreated || control.FindForm()?.IsHandleCreated == false)
            {
                //s_logger.ConditionalTrace($"Ignoring SafeInvoke on {control}.{method}");
                return false;
            }
            if (control.InvokeRequired)
            {
                control.BeginInvoke(method);
            }
            else
            {
                method();
            }
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
        catch (Exception)
        {
            if (!control.IsDisposed)
            {
                throw;
            }
        }
        return true;
    }
}