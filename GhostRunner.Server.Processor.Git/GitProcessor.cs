using GhostRunner.Server.Models;
using GhostRunner.Server.Processor.Interface;
using GhostRunner.Server.Utils;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GhostRunner.Server.Processor.Git
{
    public class GitProcessor : IProcessor
    {
        private int _gitMinuteTimeout = 10;

        private String _processingLocation;

        private TaskScript _taskScript;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GitProcessor(String processingLocation, TaskScript taskScript)
        {
            _log.Debug("Setting up Git processing object");

            _log.Debug("Setting processing location to " + processingLocation);

            _processingLocation = processingLocation;

            _taskScript = taskScript;
        }

        public String Process()
        {
            _log.Debug("Starting to process Git task");

            Dictionary<String, String> scriptOptions = JsonConvert.DeserializeObject<Dictionary<String, String>>(_taskScript.Content);

            String location = String.Empty;
            if (scriptOptions.ContainsKey("Location"))
            {
                location = scriptOptions["Location"];

                _log.Debug("Processing location for Git task set to " + location);
            }

            String username = String.Empty;
            if (scriptOptions.ContainsKey("Username"))
            {
                username = scriptOptions["Username"];

                _log.Debug("Processing username for Git task set to " + username);
            }

            String password = String.Empty;
            if (scriptOptions.ContainsKey("Password"))
            {
                password = scriptOptions["Password"];

                _log.Debug("Processing password for Git task set");
            }

            if (!String.IsNullOrEmpty(location))
            {
                String gitURL = GetGitURL(location, username, password);

                _log.Debug("Git url supplied " + gitURL);

                if (!Directory.Exists(_processingLocation)) Directory.CreateDirectory(_processingLocation);

                String[] processingLocationParts = _processingLocation.TrimEnd(new char[] { '\\' }).Split(new char[] { '\\' });

                String formattedProcessingLocation = _processingLocation.TrimEnd(new char[] { '\\' }).Replace("\\" + processingLocationParts.Last(), String.Empty);

                if (!String.IsNullOrEmpty(gitURL)) return CommandWindowHelper.ProcessCommand(formattedProcessingLocation, _gitMinuteTimeout, "git clone " + gitURL + " " + processingLocationParts.Last());
                else return String.Empty;
            }
            else
            {
                _log.Info("Processing location for Git task not found for task " + _taskScript.ID);

                return String.Empty;
            }
        }

        private String GetGitURL(String location, String username, String password)
        {
            _log.Debug("Building git URL");

            if ((!String.IsNullOrEmpty(username)) || (!String.IsNullOrEmpty(password)))
            {
                _log.Debug("username and password defined");

                String formattedURL = Regex.Replace(location, "(http://|https://)", String.Empty, RegexOptions.IgnoreCase);
                String[] urlParts = formattedURL.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (urlParts.Length > 0)
                {
                    _log.Debug("Trimming http off url " + location);

                    String returnUrl = String.Empty;

                    if (location.Trim().ToLower().StartsWith("https")) returnUrl = "https://";
                    else returnUrl = "http://";

                    _log.Debug("Trimmed url " + returnUrl);

                    String hostname = urlParts[0];

                    if (hostname.Contains('@'))
                    {
                        _log.Debug("Hostname contains login details");

                        String[] hostParts = hostname.Split(new char[] { '@' });

                        String suppliedUsername = String.Empty;
                        String suppliedPassword = String.Empty;

                        if (!String.IsNullOrEmpty(hostParts[0]))
                        {
                            if (hostParts[0].Contains(":"))
                            {
                                String[] loginDetails = hostParts[0].Split(new char[] { ':' });

                                suppliedUsername = loginDetails[0];
                                suppliedPassword = loginDetails[1];
                            }
                            else suppliedUsername = hostParts[0];
                        }

                        if (!String.IsNullOrEmpty(username)) returnUrl += username;
                        else returnUrl += suppliedUsername;

                        returnUrl += ":";

                        if (!String.IsNullOrEmpty(password)) returnUrl += password;
                        else returnUrl += suppliedPassword;

                        returnUrl += "@" + hostParts[1];
                    }
                    else
                    {
                        _log.Debug("Hostname doesnt contain login details");

                        if (!String.IsNullOrEmpty(username)) returnUrl += username;
                        if (!String.IsNullOrEmpty(password)) returnUrl += ":" + password;

                        returnUrl += "@" + hostname;
                    }

                    for (int i = 1; i < urlParts.Length; i++)
                    {
                        returnUrl += "/" + urlParts[i];
                    }

                    return returnUrl;
                }
                else return location;
            }
            else
            {
                _log.Debug("Username and password not defined, returning original location");

                return location;
            }
        }
    }
}
