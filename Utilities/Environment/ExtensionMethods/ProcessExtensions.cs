﻿/*
Copyright (c) 2012 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

#region Usings
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Company.Utilities.DataTypes.ExtensionMethods;
using Company.Utilities.Reflection.ExtensionMethods;
#endregion

namespace Company.Utilities.Environment.ExtensionMethods
{
    /// <summary>
    /// Process extensions
    /// </summary>
    public static class ProcessExtensions
    {
        #region Functions

        #region KillProcessAsync

        /// <summary>
        /// Kills a process
        /// </summary>
        /// <param name="Process">Process that should be killed</param>
        /// <param name="TimeToKill">Amount of time (in ms) until the process is killed.</param>
        public static void KillProcessAsync(this Process Process, int TimeToKill = 0)
        {
            if (Process == null)
                throw new ArgumentNullException("Process");
            ThreadPool.QueueUserWorkItem(delegate { KillProcessAsyncHelper(Process, TimeToKill); });
        }

        /// <summary>
        /// Kills a list of processes
        /// </summary>
        /// <param name="Processes">Processes that should be killed</param>
        /// <param name="TimeToKill">Amount of time (in ms) until the processes are killed.</param>
        public static void KillProcessAsync(this IEnumerable<Process> Processes, int TimeToKill = 0)
        {
            if (Processes == null)
                throw new ArgumentNullException("Processes");
            Processes.ForEach(x => ThreadPool.QueueUserWorkItem(delegate { KillProcessAsyncHelper(x, TimeToKill); }));
        }

        #endregion

        #region GetInformation

        /// <summary>
        /// Gets information about all processes and returns it in an HTML formatted string
        /// </summary>
        /// <param name="Process">Process to get information about</param>
        /// <param name="HTMLFormat">Should this be HTML formatted?</param>
        /// <returns>An HTML formatted string</returns>
        public static string GetInformation(this Process Process, bool HTMLFormat = true)
        {
            StringBuilder Builder = new StringBuilder();
            return Builder.Append(HTMLFormat ? "<strong>" : "")
                   .Append(Process.ProcessName)
                   .Append(" Information")
                   .Append(HTMLFormat ? "</strong><br />" : "\n")
                   .Append(Process.DumpProperties(HTMLFormat))
                   .Append(HTMLFormat ? "<br />" : "\n")
                   .ToString();
        }

        /// <summary>
        /// Gets information about all processes and returns it in an HTML formatted string
        /// </summary>
        /// <param name="Processes">Processes to get information about</param>
        /// <param name="HTMLFormat">Should this be HTML formatted?</param>
        /// <returns>An HTML formatted string</returns>
        public static string GetInformation(this IEnumerable<Process> Processes, bool HTMLFormat = true)
        {
            StringBuilder Builder = new StringBuilder();
            Processes.ForEach(x => Builder.Append(x.GetInformation(HTMLFormat)));
            return Builder.ToString();
        }

        #endregion

        #endregion

        #region Private Static Functions

        /// <summary>
        /// Kills a process asyncronously
        /// </summary>
        /// <param name="Process">Process to kill</param>
        /// <param name="TimeToKill">Amount of time until the process is killed</param>
        private static void KillProcessAsyncHelper(Process Process, int TimeToKill)
        {
            if (TimeToKill > 0)
                Thread.Sleep(TimeToKill);
            Process.Kill();
        }

        #endregion
    }
}