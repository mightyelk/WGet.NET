﻿//--------------------------------------------------//
// Created by basicx-StrgV                          //
// https://github.com/basicx-StrgV/                 //
//--------------------------------------------------//
using System;
using System.Text;
using System.Security;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using WGetNET.HelperClasses;
using System.Threading;

namespace WGetNET
{
    /// <summary>
    /// The <see cref="WGetNET.WinGetSourceManager"/> class offers methods to manage the sources used by winget.
    /// </summary>
    public class WinGetSourceManager : WinGetInfo
    {
        private const string _sourceListCmd = "source list";
        private const string _sourceAddCmd = "source add -n {0} -a {1} --accept-source-agreements";
        private const string _sourceAddWithTypeCmd = "source add -n {0} -a {1} -t {2} --accept-source-agreements";
        private const string _sourceUpdateCmd = "source update";
        private const string _sourceExportCmd = "source export";
        private const string _sourceResetCmd = "source reset --force";
        private const string _sourceRemoveCmd = "source remove -n {0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="WGetNET.WinGetSourceManager"/> class.
        /// </summary>
        public WinGetSourceManager()
        {
            //Provide empty constructor
        }

        //---List--------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a list of all sources that are installed in winget.
        /// </summary>
        /// <remarks>
        /// Because the list source output is limited it is recommanded to use 
        /// <see cref="WGetNET.WinGetSourceManager.ExportSourcesToObject()"/> instead.
        /// </remarks>
        /// <returns>
        /// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> instances.
        /// </returns>
        public List<WinGetSource> GetInstalledSources()
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_sourceListCmd);

                return ProcessOutputReader.ToSourceList(result.Output);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously gets a list of all sources that are installed in winget.
        /// </summary>
        /// <remarks>
        /// Because the list source output is limited it is recommanded to use 
        /// <see cref="WGetNET.WinGetSourceManager.ExportSourcesToObject()"/> instead.
        /// </remarks>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is a <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> instances.
        /// </returns>
        public async Task<List<WinGetSource>> GetInstalledSourcesAsync(CancellationToken cancellationToken)
        {
            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(_sourceListCmd, cancellationToken);

                return ProcessOutputReader.ToSourceList(result.Output);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }
        //---------------------------------------------------------------------------------------------

        //---Add---------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a new source to winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// The source type is optional but some sources like the "msstore" need it or adding it wil throw an error.
        /// </remarks>
        /// <param name="name">
        /// A <see cref="System.String"/> representing the name of the source to add.
        /// </param>
        /// <param name="arg">
        /// A <see cref="System.String"/> representing the source (eg. URL).
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was succesfull and <see langword="false"/> if it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public bool AddSource(string name, string arg)
        {
            if (!PrivilegeChecker.CheckAdministratorPrivileges())
            {
                throw new SecurityException("Administrator privileges are missing.");
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(arg))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(
                        string.Format(_sourceAddCmd, name, arg));

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }

        /// <summary>
        /// Adds a new source to winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// The source type is optional but some sources like the "msstore" need it or adding it wil throw an error.
        /// </remarks>
        /// <param name="name">
        /// A <see cref="System.String"/> representing the name of the source to add.
        /// </param>
        /// <param name="arg">
        /// A <see cref="System.String"/> representing the source (eg. URL).
        /// </param>
        /// <param name="type">
        /// A <see cref="System.String"/> representing the source type.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was succesfull and <see langword="false"/> if it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public bool AddSource(string name, string arg, string type)
        {
            if (!PrivilegeChecker.CheckAdministratorPrivileges())
            {
                throw new SecurityException("Administrator privileges are missing.");
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(arg) || string.IsNullOrWhiteSpace(type))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(
                        string.Format(_sourceAddWithTypeCmd, name, arg, type));

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }

        /// <summary>
        /// Adds a new source to winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// The source type is optional but some sources like the "msstore" need it or adding it wil throw an error.
        /// </remarks>
        /// <param name="source">
        /// The <see cref="WGetNET.WinGetSource"/> to add.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was succesfull and <see langword="false"/> if it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public bool AddSource(WinGetSource source)
        {
            if (source == null)
            {
                return false;
            }

            if (source.IsEmpty)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(source.SourceType))
            {
                return AddSource(source.SourceName, source.SourceUrl);
            }

            return AddSource(source.SourceName, source.SourceUrl, source.SourceType);
        }

        /// <summary>
        /// Asynchronously adds a new source to winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// The source type is optional but some sources like the "msstore" need it or adding it wil throw an error.
        /// </remarks>
        /// <param name="name">
        /// A <see cref="System.String"/> representing the name of the source to add.
        /// </param>
        /// <param name="arg">
        /// A <see cref="System.String"/> representing the source (eg. URL).
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the action was succesfull and <see langword="false"/> if it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public async Task<bool> AddSourceAsync(string name, string arg, CancellationToken cancellationToken)
        {
            if (!PrivilegeChecker.CheckAdministratorPrivileges())
            {
                throw new SecurityException("Administrator privileges are missing.");
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(arg))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(
                        string.Format(_sourceAddCmd, name, arg), cancellationToken);

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously adds a new source to winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// The source type is optional but some sources like the "msstore" need it or adding it wil throw an error.
        /// </remarks>
        /// <param name="name">
        /// A <see cref="System.String"/> representing the name of the source to add.
        /// </param>
        /// <param name="arg">
        /// A <see cref="System.String"/> representing the source (eg. URL).
        /// </param>
        /// <param name="type">
        /// A <see cref="System.String"/> representing the source type.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the action was succesfull and <see langword="false"/> if it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public async Task<bool> AddSourceAsync(string name, string arg, string type, CancellationToken cancellationToken)
        {
            if (!PrivilegeChecker.CheckAdministratorPrivileges())
            {
                throw new SecurityException("Administrator privileges are missing.");
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(arg) || string.IsNullOrWhiteSpace(type))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(
                        string.Format(_sourceAddWithTypeCmd, name, arg, type), cancellationToken);

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously adds a new source to winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// The source type is optional but some sources like the "msstore" need it or adding it wil throw an error.
        /// </remarks>
        /// <param name="source">
        /// The <see cref="WGetNET.WinGetSource"/> to add.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the action was succesfull and <see langword="false"/> if it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public async Task<bool> AddSourceAsync(WinGetSource source, CancellationToken cancellationToken)
        {
            if (source == null)
            {
                return false;
            }

            if (source.IsEmpty)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(source.SourceType))
            {
                return await AddSourceAsync(source.SourceName, source.SourceUrl, cancellationToken);
            }

            return await AddSourceAsync(source.SourceName, source.SourceUrl, source.SourceType, cancellationToken);
        }
        //---------------------------------------------------------------------------------------------

        //---Update------------------------------------------------------------------------------------
        /// <summary>
        /// Updates all sources that are installed in winget.
        /// </summary>
        /// <remarks>
        /// This may take a while depending on the sources.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if the update was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public bool UpdateSources()
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_sourceUpdateCmd);

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Updating sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously updates all sources that are installed in winget.
        /// </summary>
        /// <remarks>
        /// This may take a while depending on the sources.
        /// </remarks>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the update was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<bool> UpdateSourcesAsync(CancellationToken cancellationToken)
        {
            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(_sourceUpdateCmd, cancellationToken);

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Updating sources failed.", e);
            }
        }
        //---------------------------------------------------------------------------------------------

        //---Export------------------------------------------------------------------------------------
        /// <summary>
        /// Exports the winget sources as a json string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that contains the winget sorces in json format.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public string ExportSources()
        {
            try
            {
                ProcessResult result = 
                    _processManager.ExecuteWingetProcess(_sourceExportCmd);

                return ProcessOutputReader.ExportOutputToString(result);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Exports the winget sources as a json string.
        /// </summary>
        /// <param name="sourceName">The name of the source for the export.</param>
        /// <returns>
        /// A <see cref="System.String"/> that contains the winget sorces in json format.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public string ExportSources(string sourceName)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                return string.Empty;
            }

            try
            {
                //Set Arguments
                string cmd =
                    _sourceExportCmd +
                    " -n " +
                    sourceName;

                ProcessResult result =
                    _processManager.ExecuteWingetProcess(cmd);

                return ProcessOutputReader.ExportOutputToString(result);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Exports the winget sources as a json string.
        /// </summary>
        /// <param name="source">
        /// The <see cref="WGetNET.WinGetSource"/> for the export.
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/> that contains the winget sorces in json format.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public string ExportSources(WinGetSource source)
        {
            if (source == null)
            {
                return string.Empty;
            }

            if (source.IsEmpty)
            {
                return string.Empty;
            }

            return ExportSources(source.SourceName);
        }

        /// <summary>
        /// Asynchronously exports the winget sources as a json string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is a <see cref="System.String"/> that contains the winget sorces in json format.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<string> ExportSourcesAsync(CancellationToken cancellationToken)
        {
            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(_sourceExportCmd, cancellationToken);

                return ProcessOutputReader.ExportOutputToString(result);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously exports the winget sources as a json string.
        /// </summary>
        /// <param name="sourceName">The name of the source for the export.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is a <see cref="System.String"/> that contains the winget sorces in json format.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<string> ExportSourcesAsync(string sourceName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                return string.Empty;
            }

            try
            {
                //Set Arguments
                string cmd =
                    _sourceExportCmd +
                    " -n " +
                    sourceName;

                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(cmd, cancellationToken);

                return ProcessOutputReader.ExportOutputToString(result);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously exports the winget sources as a json string.
        /// </summary>
        /// <param name="source">
        /// The <see cref="WGetNET.WinGetSource"/> for the export.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is a <see cref="System.String"/> that contains the winget sorces in json format.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<string> ExportSourcesAsync(WinGetSource source, CancellationToken cancellationToken)
        {
            if (source == null)
            {
                return string.Empty;
            }

            if (source.IsEmpty)
            {
                return string.Empty;
            }

            return await ExportSourcesAsync(source.SourceName, cancellationToken);
        }

        /// <summary>
        /// Exports the winget sources to a <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public List<WinGetSource> ExportSourcesToObject()
        {
            return ExportStringToSources(ExportSources());
        }

        /// <summary>
        /// Exports the winget sources to a <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public List<WinGetSource> ExportSourcesToObject(string sourceName)
        {
            return ExportStringToSources(ExportSources(sourceName));
        }

        /// <summary>
        /// Asynchronously exports the winget sources to a <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is a <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<List<WinGetSource>> ExportSourcesToObjectAsync(CancellationToken cancellationToken)
        {
            return await ExportStringToSourcesAsync(await ExportSourcesAsync(cancellationToken), cancellationToken);
        }

        /// <summary>
        /// Asynchronously exports the winget sources to a <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is a <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<List<WinGetSource>> ExportSourcesToObjectAsync(string sourceName, CancellationToken cancellationToken)
        {
            return await ExportStringToSourcesAsync(await ExportSourcesAsync(sourceName, cancellationToken), cancellationToken);
        }

        /// <summary>
        /// Exports the winget sources in json format to a file.
        /// </summary>
        /// <param name="file">The file for the export.</param>
        /// <returns>
        /// <see langword="true"/> if the export was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public bool ExportSourcesToFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_sourceExportCmd);

                return FileHandler.ExportOutputToFile(result, file);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Exports the winget sources in json format to a file.
        /// </summary>
        /// <param name="file">The file for the export.</param>
        /// <param name="sourceName">The name of the source for the export.</param>
        /// <returns>
        /// <see langword="true"/> if the export was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public bool ExportSourcesToFile(string file, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(file) || string.IsNullOrWhiteSpace(sourceName))
            {
                return false;
            }

            try
            {
                //Set Arguments
                string cmd =
                    _sourceExportCmd +
                    " -n " +
                    sourceName;

                ProcessResult result =
                    _processManager.ExecuteWingetProcess(cmd);

                return FileHandler.ExportOutputToFile(result, file);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Exports the winget sources in json format to a file.
        /// </summary>
        /// <param name="file">
        /// The file for the export.
        /// </param>
        /// <param name="source">
        /// The <see cref="WGetNET.WinGetSource"/> for the export.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the export was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public bool ExportSourcesToFile(string file, WinGetSource source)
        {
            if (source == null)
            {
                return false;
            }

            if (source.IsEmpty || string.IsNullOrWhiteSpace(file))
            {
                return false;
            }

            return ExportSourcesToFile(file, source.SourceName);
        }

        /// <summary>
        /// Asynchronously exports the winget sources in json format to a file.
        /// </summary>
        /// <param name="file">The file for the export.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the export was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<bool> ExportSourcesToFileAsync(string file, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(_sourceExportCmd, cancellationToken);

                return await FileHandler.ExportOutputToFileAsync(result, file);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously exports the winget sources in json format to a file.
        /// </summary>
        /// <param name="file">The file for the export.</param>
        /// <param name="sourceName">The name of the source for the export.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the export was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<bool> ExportSourcesToFileAsync(string file, string sourceName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(file) || string.IsNullOrWhiteSpace(sourceName))
            {
                return false;
            }

            try
            {
                //Set Arguments
                string cmd =
                    _sourceExportCmd +
                    " -n " +
                    sourceName;

                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(cmd, cancellationToken);

                return await FileHandler.ExportOutputToFileAsync(result, file);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously exports the winget sources in json format to a file.
        /// </summary>
        /// <param name="file">
        /// The file for the export.
        /// </param>
        /// <param name="source">
        /// The <see cref="WGetNET.WinGetSource"/> for the export.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the export was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        public async Task<bool> ExportSourcesToFileAsync(string file, WinGetSource source, CancellationToken cancellationToken)
        {
            if (source == null)
            {
                return false;
            }

            if (source.IsEmpty || string.IsNullOrWhiteSpace(file))
            {
                return false;
            }

            return await ExportSourcesToFileAsync(file, source.SourceName, cancellationToken);
        }

        /// <summary>
        /// Convert the string output from winget source export to a 
        /// <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </summary>
        /// <param name="exportString">
        /// A <see cref="System.String"/> containing the winget source export content.
        /// </param>
        /// <returns>
        /// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </returns>
        private List<WinGetSource> ExportStringToSources(string exportString)
        {
            List<WinGetSource> sourceList = new List<WinGetSource>();

            string[] jsonStrings = exportString.Split("}{");
            StringBuilder jsonString = new StringBuilder();
            for (int i = 0; i < jsonStrings.Length; i++)
            {
                if (!jsonStrings[i].StartsWith('{'))
                {
                    jsonString.Append('{');
                }

                jsonString.Append(jsonStrings[i]);

                if (!jsonStrings[i].EndsWith('}'))
                {
                    jsonString.Append('}');
                }
                
                WinGetSource? source =
                    JsonHandler.StringToObject<WinGetSource>(jsonString.ToString());

                if (source == null)
                {
                    throw new WinGetActionFailedException("Exporting sources failed. Could not parse json.");
                }

                sourceList.Add(source);

                jsonString.Clear();
            }

            return sourceList;
        }

        /// <summary>
        /// Asynchronously convert the string output from winget source export to a 
        /// <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </summary>
        /// <param name="exportString">
        /// A <see cref="System.String"/> containing the winget source export content.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is a <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </returns>
        private async Task<List<WinGetSource>> ExportStringToSourcesAsync(string exportString, CancellationToken cancellationToken)
        {
            List<WinGetSource> sourceList = new List<WinGetSource>();

            string[] jsonStrings = exportString.Split("}{");
            StringBuilder jsonString = new StringBuilder();
            for (int i = 0; i < jsonStrings.Length; i++)
            {
                if (!jsonStrings[i].StartsWith('{'))
                {
                    jsonString.Append('{');
                }

                jsonString.Append(jsonStrings[i]);

                if (!jsonStrings[i].EndsWith('}'))
                {
                    jsonString.Append('}');
                }

                WinGetSource? source =
                    await JsonHandler.StringToObjectAsync<WinGetSource>(jsonString.ToString(), cancellationToken);

                if (source == null)
                {
                    throw new WinGetActionFailedException("Exporting sources failed. Could not parse json.");
                }

                sourceList.Add(source);

                jsonString.Clear();
            }

            return sourceList;
        }
        //---------------------------------------------------------------------------------------------

        //---Import------------------------------------------------------------------------------------
        /// <summary>
        /// Imports sources into winget.
        /// </summary>
        /// <param name="winGetSources">
        /// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was successfull and <see langword="false"/> if on or more sorces failed.
        /// </returns>
        public bool ImportSource(List<WinGetSource> winGetSources)
        {
            if (winGetSources == null || winGetSources.Count <= 0)
            {
                return false;
            }

            bool status = true;
            for (int i = 0; i < winGetSources.Count; i++)
            {
                if (!AddSource(winGetSources[i]))
                {
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// Imports a source into winget.
        /// </summary>
        /// <param name="winGetSource">
        /// A <see cref="WGetNET.WinGetSource"/> objects.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was successfull and <see langword="false"/> if it failed.
        /// </returns>
        public bool ImportSource(WinGetSource winGetSource)
        {
            return AddSource(winGetSource);
        }

        /// <summary>
        /// Imports a source into winget.
        /// </summary>
        /// <param name="jsonString">
        /// A <see cref="System.String"/> containing the json for ONE source.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was successfull and <see langword="false"/> if it failed.
        /// </returns>
        public bool ImportSource(string jsonString)
        {
            WinGetSource? source = JsonHandler.StringToObject<WinGetSource>(jsonString);

            if (source == null)
            {
                throw new WinGetActionFailedException("Importing source failed. Could not parse json.");
            }

            return AddSource(source);
        }

        /// <summary>
        /// Asynchronously imports sources into winget.
        /// </summary>
        /// <param name="winGetSources">
        /// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> objects.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the action was successfull and <see langword="false"/> if on or more sorces failed.
        /// </returns>
        public async Task<bool> ImportSourceAsync(List<WinGetSource> winGetSources, CancellationToken cancellationToken)
        {
            if (winGetSources == null || winGetSources.Count <= 0)
            {
                return false;
            }

            bool status = true;
            for (int i = 0; i < winGetSources.Count; i++)
            {
                if (!await AddSourceAsync(winGetSources[i], cancellationToken))
                {
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// Asynchronously imports a source into winget.
        /// </summary>
        /// <param name="winGetSource">
        /// A <see cref="WGetNET.WinGetSource"/> objects.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the action was successfull and <see langword="false"/> if it failed.
        /// </returns>
        public async Task<bool> ImportSourceAsync(WinGetSource winGetSource, CancellationToken cancellationToken)
        {
            return await AddSourceAsync(winGetSource, cancellationToken);
        }

        /// <summary>
        /// Asynchronously imports a source into winget.
        /// </summary>
        /// <param name="jsonString">
        /// A <see cref="System.String"/> containing the json for ONE source.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the action was successfull and <see langword="false"/> if it failed.
        /// </returns>
        public async Task<bool> ImportSourceAsync(string jsonString, CancellationToken cancellationToken)
        {
            WinGetSource? source = await JsonHandler.StringToObjectAsync<WinGetSource>(jsonString, cancellationToken);

            if (source == null)
            {
                throw new WinGetActionFailedException("Importing source failed. Could not parse json.");
            }

            return await AddSourceAsync(source, cancellationToken);
        }
        //---------------------------------------------------------------------------------------------

        //---Reset-------------------------------------------------------------------------------------
        /// <summary>
        /// Resets all sources that are installed in winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// This may take a while depending on the sources.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if the reset was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public bool ResetSources()
        {
            if (!PrivilegeChecker.CheckAdministratorPrivileges())
            {
                throw new SecurityException("Administrator privileges are missing.");
            }

            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_sourceResetCmd);

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Reset sources failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously resets all sources that are installed in winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// This may take a while depending on the sources.
        /// </remarks>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the reset was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public async Task<bool> ResetSourcesAsync(CancellationToken cancellationToken)
        {
            if (!PrivilegeChecker.CheckAdministratorPrivileges())
            {
                throw new SecurityException("Administrator privileges are missing.");
            }

            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(_sourceResetCmd, cancellationToken);

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Reset sources failed.", e);
            }
        }
        //---------------------------------------------------------------------------------------------

        //---Remove------------------------------------------------------------------------------------
        /// <summary>
        /// Removes a source from winget (Needs administrator rights).
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/> representing the name of the source.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the remove was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public bool RemoveSources(string name)
        {
            if (!PrivilegeChecker.CheckAdministratorPrivileges())
            {
                throw new SecurityException("Administrator privileges are missing.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(string.Format(_sourceRemoveCmd, name));

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Removing source failed.", e);
            }
        }

        /// <summary>
        /// Removes a source from winget (Needs administrator rights).
        /// </summary>
        /// <param name="source">
        /// The <see cref="WGetNET.WinGetSource"/> to remove.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the remove was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public bool RemoveSources(WinGetSource source)
        {
            if (source == null)
            {
                return false;
            }

            if (source.IsEmpty)
            {
                return false;
            }

            return RemoveSources(source.SourceName);
        }

        /// <summary>
        /// Asynchronously removes a source from winget (Needs administrator rights).
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/> representing the name of the source.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the remove was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public async Task<bool> RemoveSourcesAsync(string name, CancellationToken cancellationToken)
        {
            if (!PrivilegeChecker.CheckAdministratorPrivileges())
            {
                throw new SecurityException("Administrator privileges are missing.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            try
            {
                ProcessResult result =
                    await _processManager.ExecuteWingetProcessAsync(string.Format(_sourceRemoveCmd, name), cancellationToken);

                return result.Success;
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Removing source failed.", e);
            }
        }

        /// <summary>
        /// Asynchronously removes a source from winget (Needs administrator rights).
        /// </summary>
        /// <param name="source">
        /// The <see cref="WGetNET.WinGetSource"/> to remove.
        /// </param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task"/>, containing the result.
        /// The result is <see langword="true"/> if the remove was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        /// <exception cref="WGetNET.WinGetNotInstalledException">
        /// WinGet is not installed or not found on the system.
        /// </exception>
        /// <exception cref="WGetNET.WinGetActionFailedException">
        /// The current action failed for an unexpected reason.
        /// Please see inner exception.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The current user is missing administrator privileges for this call.
        /// </exception>
        public async Task<bool> RemoveSourcesAsync(WinGetSource source, CancellationToken cancellationToken)
        {
            if (source == null)
            {
                return false;
            }

            if (source.IsEmpty)
            {
                return false;
            }

            return await RemoveSourcesAsync(source.SourceName, cancellationToken);
        }
        //---------------------------------------------------------------------------------------------
    }
}
