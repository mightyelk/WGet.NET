﻿//--------------------------------------------------//
// Created by basicx-StrgV                          //
// https://github.com/basicx-StrgV/                 //
//--------------------------------------------------//
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using WGetNET.HelperClasses;
using System.Threading;

namespace WGetNET
{
    /// <summary>
    /// The <see langword="internal"/> class <see cref="WGetNET.ProcessManager"/> 
    /// provides the winget process execution.
    /// </summary>
    internal class ProcessManager
    {
        private readonly ProcessStartInfo _winGetStartInfoTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="WGetNET.ProcessManager"/> class.
        /// </summary>
        /// <param name="processName">
        /// The name of the process to execute.
        /// </param>
        public ProcessManager(string processName)
        {
            _winGetStartInfoTemplate = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = processName,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.Default
            };
        }
    
        /// <summary>
        /// Executes a winget process with the given command and returns the result.
        /// </summary>
        /// <param name="cmd">
        /// A <see cref="System.String"/> representing the command that winget should be executed with.
        /// </param>
        /// <returns>
        /// A <see cref="WGetNET.ProcessResult"/> object, 
        /// containing the output an exit id of the process.
        /// </returns>
        public ProcessResult ExecuteWingetProcess(string cmd)
        {
            return RunProcess(GetStartInfo(cmd));
        }

        /// <summary>
        /// Asynchronous executes a winget process with the given command and returns the result.
        /// </summary>
        /// <param name="cmd">
        /// A <see cref="System.String"/> representing the command that winget should be executed with.
        /// </param>
        /// <returns>
        /// A <see cref="WGetNET.ProcessResult"/> object, 
        /// containing the output an exit id of the process.
        /// </returns>
        public async Task<ProcessResult> ExecuteWingetProcessAsync(string cmd, CancellationToken cancellationToken)
        {
            return await RunProcessAsync(GetStartInfo(cmd), cancellationToken);
        }

        /// <summary>
        /// Gets the start info for a process.
        /// </summary>
        /// <param name="cmd">
        /// String containig the arguments for the action.
        /// </param>
        /// <returns>
        /// A <see cref="System.Diagnostics.ProcessStartInfo"/> object, for the process.
        /// </returns>
        private ProcessStartInfo GetStartInfo(string cmd)
        {
            return new ProcessStartInfo()
            {
                CreateNoWindow = _winGetStartInfoTemplate.CreateNoWindow,
                FileName = _winGetStartInfoTemplate.FileName,
                RedirectStandardOutput = _winGetStartInfoTemplate.RedirectStandardOutput,
                StandardOutputEncoding = _winGetStartInfoTemplate.StandardOutputEncoding,
                UseShellExecute = _winGetStartInfoTemplate.UseShellExecute,
                WindowStyle = _winGetStartInfoTemplate.WindowStyle,
                Arguments = cmd,
            };
        }

        /// <summary>
        /// Runs a process with the current start informations.
        /// </summary>
        /// <returns>
        /// A <see cref="WGetNET.ProcessResult"/> object, 
        /// containing the output an exit id of the process.
        /// </returns>
        private ProcessResult RunProcess(ProcessStartInfo processStartInfo)
        {
            ProcessResult result = new ProcessResult();

            //Create and run process
            using (Process proc = new Process { StartInfo = processStartInfo })
            {
                proc.Start();

                result.Output = ReadSreamOutput(proc.StandardOutput);

                //Wait till end and get exit code
                proc.WaitForExit();
                result.ExitCode = proc.ExitCode;
            }

            return result;
        }

        /// <summary>
        /// Asynchronous runs a process with the current start informations.
        /// </summary>
        /// <returns>
        /// A <see cref="WGetNET.ProcessResult"/> object, 
        /// containing the output an exit id of the process.
        /// </returns>
        private async Task<ProcessResult> RunProcessAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
        {
            ProcessResult result = new ProcessResult();

            //Create and run process
            using (Process proc = new Process { StartInfo = processStartInfo })
            {
                proc.Start();
                
                result.Output = await ReadSreamOutputAsync(proc.StandardOutput, cancellationToken);

                //Wait till end and get exit code
                proc.WaitForExit();
                result.ExitCode = proc.ExitCode;
            }

            return result;
        }

        /// <summary>
        /// Reads the data from the process output to a string array.
        /// </summary>
        /// <param name="output">
        /// The <see cref="System.IO.StreamReader"/> with the process output.
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/> array 
        /// containing the process output stream content by lines.
        /// </returns>
        private string[] ReadSreamOutput(StreamReader output)
        {
            string[] outputArray = new string[0];

            //Read output to list
            while (!output.EndOfStream)
            {
                string? outputLine = output.ReadLine();
                if (outputLine is null)
                {
                    continue;
                }

                outputArray = ArrayManager.Add(outputArray, outputLine);
            }

            return outputArray;
        }

        /// <summary>
        /// Asynchronous reads the data from the process output to a string array.
        /// </summary>
        /// <param name="output">
        /// The <see cref="System.IO.StreamReader"/> with the process output.
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/> array 
        /// containing the process output stream content by lines.
        /// </returns>
        private async Task<string[]> ReadSreamOutputAsync(StreamReader output, CancellationToken cancellationToken)
        {
            string[] outputArray = new string[0];

            //Read output to list
            while (!output.EndOfStream)
            {
                string? outputLine = await output.ReadLineAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
                if (outputLine is null)
                {
                    continue;
                }

                outputArray = ArrayManager.Add(outputArray, outputLine);
            }

            return outputArray;
        }
    }
}
