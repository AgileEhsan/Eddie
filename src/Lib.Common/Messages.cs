﻿// <eddie_source_header>
// This file is part of Eddie/AirVPN software.
// Copyright (C)2014-2016 AirVPN (support@airvpn.org) / https://airvpn.org
//
// Eddie is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Eddie is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Eddie. If not, see <http://www.gnu.org/licenses/>.
// </eddie_source_header>

using System;
using System.Collections.Generic;
using System.Text;

namespace Eddie.Common
{
	// Here messages used only by Core
	public static class Messages
	{
		// Start Messages
		public static string AppStarting = "Starting";
		public static string AppShutdownStart = "Shutdown in progress";
		public static string AppShutdownComplete = "Shutdown complete";
		public static string ReceivedOsSignal = "Received signal {1}";
		public static string NotImplemented = "Not yet implemented. Contact our support staff.";
		public static string UnhandledException = "Unexpected error. Please contact our support staff.";
		public static string Available = "Available";
		public static string NotAvailable = "Not available";
		public static string NotFound = "Not found";
		public static string Automatic = "Automatic";
		public static string Ready = "Ready";
		public static string Unknown = "Unknown";
		public static string Yes = "Yes";
		public static string No = "No";
		public static string Ok = "Ok";
		public static string Failed = "Failed";
		public static string PleaseWait = "Please wait.";
		public static string StatsNotConnected = "Not connected.";
		public static string DoubleClickToAction = "(double-click to {1})";
		public static string StatusConnectedSpeed = "\u2193 {1} \u2191 {2}";
		public static string ExitConfirm = "Do you really want to exit?";
		public static string WarningLocalTimeInPast = "We have detected a local date/time set to the past.\nThis may cause issues with security verifications.\n\nContinue at your own risk.\n\nIf you're a time traveller, you have been warned.";
		public static string CheckingRequired = "Unavailable (option 'Checking Tunnel' required)";
		public static string ServerByNameNotFound = "Requested server '{1}' not found.";
		public static string KeyNotFound = "Key '{1}' not found.";
		public static string FileNotFound = "File '{1}' not found.";
		public static string FileErrorRead = "Error in file '{1}': {2}.";
		public static string DeprecatedRemoveDefaultGateway = "The option 'Remove default gateway' will be deprecated soon.";
		public static string WaitingLatencyTestsTitle = "Waiting for latency tests";
		public static string WaitingLatencyTestsStep = "({1} to go)";
		public static string ExchangeTryFailed = "{1}, {2}° try failed ({3})";
		public static string AuthorizeLogin = "Checking login ...";
		public static string AuthorizeLoginFailed = "Cannot login. ({1})";
		public static string AuthorizeLoginDone = "Logged in.";
		public static string AuthorizeLogout = "Logout ...";
		public static string AuthorizeLogoutDone = "Logged out.";
		public static string AuthorizeConnect = "Checking authorization ...";
		public static string AuthorizeConnectFailed = "Authorization check failed, continue anyway ({1})";
		public static string AppExiting = "Exiting";
		public static string AppEvent = "Running event {1}";
		public static string AutoPortSwitch = "Auto retry with another port.";
		public static string AuthFailed = "Authorization failed.";
		public static string AirVpnAuthFailed = "Authorization failed. Look at the client area to discover the reason.";
		public static string ConsoleKeyboardHelp = "Press 'X' to Cancel, 'N' to connect/reconnect to the best available server.";
		public static string ConsoleKeyboardHelpNoConnect = "Use -connect to start connection when application starts";
		public static string ConsoleKeyBreak = "Break signal received. Shutdown. Hit again to force break.";
		public static string ConsoleKeyCancel = "Cancel requested from keyboard.";
		public static string ConsoleKeySwitch = "Server switch requested from keyboard.";
		public static string OsInstanceAlreadyRunning = "An instance of Eddie is already running.";
		public static string OsDriverInstall = "Installing tunnel driver";
		public static string OsDriverInstallerNotAvailable = "Driver installer not available.";
		public static string OsDriverFailed = "Driver installation failed.";
		public static string OsDriverNoAdapterFound = "Cannot find tunnel adapter.";
		public static string OsDriverNoVersionFound = "Cannot determine tunnel version.";
		public static string OsDriverCannotInstall = "Tunnel direct not found and can't be installed.";
		public static string OsDriverUninstallDone = "Tunnel driver uninstalled.";
		public static string OsDriverNeedUpgrade = "Tunnel driver will be upgraded, from {1} to {2}.";
		public static string OsDriverNotAvailable = "Not available - Will be installed at next connection.";
		public static string OsDriverNoPath = "Unable to find driver path '{1}'";
		public static string OsDriverNoVersion = "Unable to find driver version of path '{1}'";
		public static string OsDriverNoRegPath = "Unable to find driver registry path";
		public static string BundleExecutableError = "Unexpected bundle executable error {1} ({2}). Dumping report.";
		public static string ManifestUpdate = "Updating systems & servers data ...";
		public static string ManifestDone = "Systems & servers data update completed";
		public static string ManifestFailed = "Cannot retrieve systems & servers data. ({1})";
		public static string ManifestFailedContinue = "Unable to retrieve systems & servers data. Continue anyway with the old data.";
		public static string ManifestFailedUnexpected302 = "Unexpected redirect. Open manually '{1}' in your browser for more info.";
		public static string CommandLineUnknownOption = "Unknown option in command-line: {1}";
		public static string OptionsRead = "Reading options from {1}";
		public static string OptionsNotFound = "Profile options not found, using defaults.";
		public static string OptionsUnknown = "Skipped unknown option '{1}'";
		public static string OptionsReverted = "Sorry, an error occurred during loading options. Options reverted to default. (Reason: {1})";
		public static string OptionsWriteFailed = "An error occurred during writing options in path '{1}': {2}";

		public static string AdminRequiredStop = "You need root access for this program (to alter routing table)";
		public static string AdminRequiredRestart = "Restarting with admin privileges (method:{1})";
		public static string AdminRequiredRestartFailed = "You need root access for this program (to alter routing table) and a program to obtain administrative privileges was NOT found.\nTry to install packages like 'gksu' or 'kdesudo' or 'xdg-su' or 'beesu' (Fedora)";
		public static string AdminRequiredPasswordPrompt = "Eddie needs administrative privileges. Please enter your password.";

		public static string GeneratedFileHeader = "Generated by Eddie v{1} | https://eddie.website";

		public static string ReportStepCollectEnvironmentInfo = "Collecting environment information...";
		public static string ReportStepTests = "Performing tests..."; 
		public static string ReportStepLogs = "Collecting logs...";
		public static string ReportStepNetwork = "Collecting Network information..."; 
		public static string ReportStepPlatform = "Collecting platform-specific information..."; 
		public static string ReportStepDone = "Report completed"; 
		public static string ReportLogs = "Logs"; 
		public static string ReportOptions = "Important options not at defaults"; 

		public static string AlreadyRunningOpenVPN = "OpenVPN is already running ({1}).";
		public static string AlreadyRunningSTunnel = "STunnel is already running ({1}).";
		public static string AlreadyRunningSsh = "SSH tunnel is already running ({1}).";

		public static string SocksProxyError = "Socks proxy connection error.";
		public static string ProxyDetectDeprecated = "Proxy mode 'Detect' is deprecated, please specify explicitly.";

		public static string LogsLineRepetitionSummary = "Above log line repeated {1} times more";

		public static string FatalUnexpected = "Fatal error occured, please contact Eddie support: {1}";

		public static string NetworkInterfaceNotAvailable = "Network interface no more available."; 

		public static string UnacceptableCharacters = "{1} contains not yet supported characters ('`\"%!$), sorry.";

		public static string PingerStatsNormal = "Invalid: {1}, Older check: {2}, Latest check: {3}";
		public static string PingerStatsPending = "Disabled during VPN connection. Latest check: {1}";

		public static string ConnectionWarningModeUnsupported = "connection mode not available";
		public static string ConnectionWarningNoEntryIPv4 = "no IPv4 entry available";
		public static string ConnectionWarningNoEntryIPv6 = "no IPv6 entry available";
		public static string ConnectionWarningNoExitIPv4 = "no IPv4 exit available";
		public static string ConnectionWarningNoExitIPv6 = "no IPv6 exit available";
		public static string ConnectionWarningOlderOpenVpnTlsCrypt = "tls-crypt not available on older openvpn";
		public static string ConnectionWarningLoginRequired = "login required";

		public static string CheckingEnvironment = "Checking environment";
		public static string CheckingProtocolUnknown = "Unknown protocol.";
		public static string CheckingProxyHostMissing = "Specify a host in the proxy settings.";
		public static string CheckingProxyPortWrong = "Invalid port in the proxy settings.";
		public static string CheckingProxyNoUdp = "UDP is not allowed with a proxy.";
		public static string CheckingIPv4BlockNotYetImplemented = "IPv4 block not yet implemented.";
		public static string CustomRouteInvalid = "Invalid or unresolved custom route: {1}";
		public static string RetrievingManifest = "Retrieving manifest";
		public static string SessionStart = "Session starting.";
		public static string SessionStop = "Session terminated.";
		public static string SessionCancel = "Cancel requested.";
		public static string SessionFailed = "Failed to start.";
		public static string KillWithManagement = "Sending management termination signal";
		public static string KillWithSoft = "Sending soft termination signal";
		public static string KillWithHard = "Sending hard termination signal";
		public static string ConnectionStop = "Connection terminated.";
		public static string RenewingTls = "Renewing TLS key";
		public static string DirectiveError = "Unrecognized directive or missing parameter: {1}";
		public static string RouteAddFailed = "Routes, add {1} for gateway {2} failed: {3}"; 
		public static string RouteDelFailed = "Routes, remove {1} for gateway {2} failed: {3}"; 
		public static string RouteAddRemoved = "Routes, restored a previously deleted route, {1} for gateway {2}"; 
		public static string RouteAddNew = "Routes, added a new route, {1} for gateway {2}"; 
		public static string RouteDelAdded = "Routes, removed a route previously added, {1} for gateway {2}"; 
		public static string RouteDelExist = "Routes, removed an existing route, {1} for gateway {2}";


		public static string ConnectionStartManagement = "Connection to OpenVPN Management Interface";
		public static string ConnectionCheckingRouteIPv4 = "Checking route IPv4";
		public static string ConnectionCheckingRouteIPv6 = "Checking route IPv6";
		public static string ConnectionCheckingTryRoute = "Checking route ({1}° try)";
		public static string ConnectionCheckingTryRouteFail = "Checking route failed: {1}";
		public static string ConnectionCheckingRoute2 = "Checking info";
		public static string ConnectionCheckingRouteNotAvailable = "Checking route not available on this server.";
		public static string ConnectionCheckingRouteIPv4Failed = "Checking route IPv4 failed.";
		public static string ConnectionCheckingRouteIPv6Failed = "Checking route IPv6 failed.";
		public static string ConnectionCheckingDNS = "Checking DNS";
		public static string ConnectionCheckingTryDNS = "Checking DNS ({1}° try)";
		public static string ConnectionCheckingTryDNSFail = "Checking DNS failed: {1}";
		public static string ConnectionCheckingDNSFailed = "Checking DNS failed.";
		public static string ConnectionFlushDNS = "Flushing DNS";
		public static string ConnectionDetectExit = "Detecting Exit IPs";
		public static string ConnectionConnected = "Connected.";
		public static string ConnectionCredentials = "Checking credentials";
		public static string ConnectionConnecting = "Connecting to {1}";
		public static string ConnectionDisconnecting = "Disconnecting";

		public static string NetworkLockActivation = "Activation of Network Lock";
		public static string NetworkLockDeactivation = "Deactivation of Network Lock";
		//public static string NetworkLockButtonActive = "Network Lock Active. Click to deactivate"; 
		//public static string NetworkLockButtonDeactive = "Network Lock Inactive. Click to activate";
		public static string NetworkLockButtonActive = "Deactivate Network Lock";
		public static string NetworkLockButtonDeactive = "Activate Network Lock";
		public static string NetworkLockStatusActive = "Network Lock enabled"; // macOS only
		public static string NetworkLockStatusDeactive = "Network Lock disabled"; // macOS only

		public static string NetworkLockNoMode = "There is no available or enabled Network Lock mode, sorry.";
		//public static string NetworkLockAllowedIpDuplicated = "Allowed IP '{1}' in custom lock duplicated"; 
		//public static string NetworkLockAllowedIpInvalid = "Allowed IP '{1}' in custom lock invalid"; 
		public static string NetworkLockRecoveryWhenActive = "Unexpected: Recovery Load with Network Lock active";
		public static string NetworkLockRecoveryUnknownMode = "Unknown mode in Network Lock Recovery";

		public static string NetworkLockRouteRemoved = "Route removed: {1}";
		public static string NetworkLockRouteRestored = "Route restored: {1}";
		public static string NetworkLockWarning = "Network Lock Mode\n\nIn this state, all network connections outside Eddie service & tunnel are unavailable, whether this system is connected to the VPN or not.\n\nWarning: Any active connection will be dropped.\n\nAre you sure you want to activate this mode?";
		public static string NetworkLockNotAvailableWithRouteOut = "You set the default routing outside the tunnel,\nincompatible with Network Lock feature";

		public static string NetworkLockUnexpectedAlreadyActive = "Unexpected already active network lock";
		public static string NetworkLockWindowsFirewallUnableToStartService = "Unable to start Windows Firewall service. Try to switch it from 'Disabled' to 'Manual'.";
		public static string NetworkLockMacOSUnableToStart = "Unable to start PF Firewall.";
		public static string NetworkLockMacOSUnableToStartReason = "Unable to start PF Firewall ({1}).";
		public static string NetworkLockLinuxUnexpectedAlreadyActive = "Unexpected already active iptables network lock";
		public static string NetworkLockLinuxIPv6NotAvailable = "Network lock not enabled on IPv6 layer. IPv6 seems disabled at system level."; 

		public static string NetworkLockWindowsFirewallBackupFailed = "Backup of current rules failed.";

		public static string TorControlAuth = "Tor Control authentication method: {1}";
		public static string TorControlGuardIps = "Tor Control Guard IP detected: {1}";
		public static string TorControlMeekUnsupported = "Tor, meek bridge found, unsupported yet. Please remove it from the config";
		public static string TorControlNoPath = "Unable to find your Tor path.";
		public static string TorControlNoIps = "Unable to find IP address of Tor first node of an established circuit.";
		public static string TorControlException = "Unable to communicate with Tor ({1}). Is Tor up and running?";
		public static string TorControlTest = "Successful test. Tor Version: ";
		public static string TorControlNEWNYM = "Sending request for a new circuit to Tor";

		public static string TcpServerNoBindAddress = "Unable to start TCP server: Address {1} mismatch";
		public static string IPv6NotSupportedByOS = "The server supports IPv6, but IPv6 is disabled at OS level. You need to re-enable it manually (reboot is required) or disable this warning by setting Preferences > Networking > Layer IPv6: Block.";
		public static string IPv4NotSupportedByNetworkAdapter = "The server supports IPv4, but IPv4 is disabled at Network Adapter level. You need to re-enable it manually or disable this warning by setting Preferences > Networking > Layer IPv4: Block.";
		public static string IPv4NotSupportedByNetworkAdapterAutoSwitch = "'Preferences > Networking > Layer IPv4' automatically switched to 'Block'.";
		public static string IPv6NotSupportedByNetworkAdapter = "The server supports IPv6, but IPv6 is disabled at Network Adapter level. You need to re-enable it manually or disable this warning by setting Preferences > Networking > Layer IPv6: Block.";
		public static string IPv6NotSupportedByNetworkAdapterAutoSwitch = "'Preferences > Networking > Layer IPv6' automatically switched to 'Block'.";

		public static string RecoveryDetected = "Recovery. Unexpected crash?";

		public static string ChartRange = "Range";
		public static string ChartGrid = "Grid";
		public static string ChartStep = "Step";
		public static string ChartClickToChangeResolution = "Click to change resolution";
		public static string ChartDownload = "Download";
		public static string ChartUpload = "Upload";

		public static string AreasName = "Name";
		public static string AreasServers = "Servers";
		public static string AreasLoad = "Load";
		public static string AreasUsers = "Users";

		public static string ServersName = "Name";
		public static string ServersScore = "Score";
		public static string ServersLocation = "Location";
		public static string ServersLatency = "Latency";
		public static string ServersLoad = "Load";
		public static string ServersUsers = "Users";

		public static string ProvidersInvalid = "Invalid provider";
		public static string ProvidersOpenVpnPathNotFound = "Path '{1}' not found for provider '{2}'";
		public static string ProvidersOpenVpnErrorProfile = "Cannot read configuration file '{1}' for provider '{2}': {3}";


		public static string WindowsAboutTitle = "About";
		public static string WindowsAboutVersion = "Version";
		public static string WindowsAboutThanks = "Special thanks to {1} and all the people of the AirVPN Community!";
		public static string WindowsTosTitle = "Terms of Service";
		public static string WindowsTosCheck1 = "I have read and I accept the Terms of Service ";
		public static string WindowsTosCheck2 = "I HEREBY EXPLICITLY ACCEPT POINTS 8, 10, 11";
		public static string WindowsTosAccept = "Accept";
		public static string WindowsTosReject = "Reject";
		public static string WindowsReportTitle = "System Report";
		public static string WindowsLoginTitle = "Login";
		public static string WindowsManTitle = "Man"; 
		public static string WindowsCredentialsRememberNo = "No";
		public static string WindowsCredentialsRememberRun = "Until closing app";
		public static string WindowsCredentialsRememberPermanent = "Permanent";
		public static string WindowsFrontMessageTitle = "Important Message";
		public static string WindowsFrontMessageAccept = "Ok";
		public static string WindowsFrontMessageMore = "Look at https://airvpn.org for more informations";
		public static string WindowsSettingsTitle = "Preferences";
		public static string WindowsSettingsUiUnit0 = "Bits for speed, bytes for volume";
		public static string WindowsSettingsUiUnit1 = "Always bytes";
		public static string WindowsSettingsUiUnit2 = "Always bits";
		public static string WindowsSettingsProxyWhenAlways = "Always";
		public static string WindowsSettingsProxyWhenWeb = "Only for web requests";
		public static string WindowsSettingsProxyWhenOpenVPN = "Only for OpenVPN";
		public static string WindowsSettingsProxyWhenNone = "Never";
		public static string WindowsSettingsSomeProtocolsUnavailable = "{1} protocol modes not available due to your OpenVPN version.";
		public static string WindowsSettingsRouteTitle = "Preferences - Route";
		public static string WindowsSettingsEventTitle = "Preferences - Event";
		public static string WindowsSettingsLoggingHelp = "Use %d, %m, %y or %w for day, month, year or day of week. Useful for log rotation.\nRelative to data path. For multiple logs with different paths, separate them with a semicolon ;\n\nAdvanced example:\nlogs/single.log;logs/months/eddie_%d.log;logs/week/eddie_%w.log";
		public static string WindowsSettingsRouteLockHelp = "IP in 'Outside Tunnel' are also unlocked\nwhen Network Lock feature is active.";
		public static string WindowsSettingsRouteEditIp = "Specify an hostname or a single IP address (ex. 1.2.3.4)\nor CIDR range (ex. 1.2.3.4/24)\nor subnet range (ex. 1.2.3.4/255.255.255.128)";
		public static string WindowsSettingsRouteInvalid = "Invalid IP address or range.";
		public static string WindowsSettingsRouteOutEmptyList = "WARNING: not specified routes go outside the tunnel, but you did not specify any route. Continue?"; // TOCLEAN, Deprecated
		public static string WindowsSettingsRouteWithHostname = "WARNING: you used hostname in routes, but your DNS is not whitelist in  Network Lock. We reccomend to use only IP address or enable 'Network Lock > allow DNS'. Continue anyway?";
		public static string WindowsSettingsDnsCheck = "Check if the tunnel uses AirVPN DNS";
		public static string WindowsSettingsDnsServers = "Leave empty to use DNS servers recommended by the VPN";
		public static string WindowsSettingsIpTitle = "Preferences - IP Address";
		public static string WindowsSettingsOpenVpnRcvBuf = "TCP/UDP receive buffer size";
		public static string WindowsSettingsOpenVpnSndBuf = "TCP/UDP send buffer size";
		public static string WindowsSettingsOpenVpnDefault = "OpenVPN Default";
		public static string WindowsSettingsOpenVpnDirectivesDefaultSkip1 = "Append Custom directives";
		public static string WindowsSettingsOpenVpnDirectivesDefaultSkip2 = "Use only Custom directives";
		public static string WindowsSettingsLogsCannotOpenDirectory = "Cannot open logs directory {1}";
		public static string WindowsSettingsNetworkIpModeInAlways = "Inside tunnel (must be supported)";
		public static string WindowsSettingsNetworkIpModeInOrOut = "Inside tunnel if supported, otherwise outside";
		public static string WindowsSettingsNetworkIpModeInOrBlock = "Inside tunnel if supported, otherwise blocked";
		public static string WindowsSettingsNetworkIpModeOut = "Outside tunnel";
		public static string WindowsSettingsNetworkIpModeBlock = "Block";
		public static string WindowsCommandTitle = "Console command";
		public static string WindowsOpenVpnManagementCommandTitle = "OpenVPN Management Command";
		public static string WindowsPortForwardingTitle = "Tools - Port Forwarding Tester";
		public static string WindowsMainSpeedResolution1 = "Range: 1 minute, Grid: 10 seconds, Step: 1 second";
		public static string WindowsMainSpeedResolution2 = "Range: 10 minutes, Grid: 1 minute, Step: 1 second";
		public static string WindowsMainSpeedResolution3 = "Range: 1 hour, Grid: 10 minutes, Step: 1 second";
		public static string WindowsMainSpeedResolution4 = "Range: 1 day, Grid: 1 hour, Step: 30 seconds";
		public static string WindowsMainSpeedResolution5 = "Range: 30 days, Grid: 1 day, Step: 10 minutes";
		public static string WindowsMainHide = "Hide Main Window";
		public static string WindowsMainShow = "Show Main Window";
		public static string WindowsProviderAddTitle = "Add provider";
		public static string WindowsProviderEditManifestTitle = "Edit provider";
		public static string WindowsProviderEditOpenVPNTitle = "Edit OpenVPN provider";
		public static string WindowsProviderEditOpenVPNPathBrowse = "Select a path that contain *.ovpn files";
		public static string WindowsProviderEditOpenVPNSupportIPv6 = " (unfortunately, it can't be detected a priori)";
		public static string WindowsConnectionTitle = "View connection";
		public static string WindowsConnectionRenameTitle = "Rename connection";
		public static string WindowsProviderNoBootstrapTitle = "Bootstrap failed";
		public static string WindowsProviderNoBootstrapBody = "Unable to contact provider {1} to obtain bootstrap information.\nPlease check your Internet connection.\n\nIf you are behind a restrictive firewall, please try to open a ticket for help.\n\nIf the support team provides custom bootstrap URLs, please enter them below.";
		public static string WindowsProfilesTitle = "Profiles";
		public static string WindowsProfileTitle = "Profile";
		public static string LogsCopyClipboardDone = "Copy to clipboard done.";
		public static string LogsSaveToFileDone = "Save to file done.";
		public static string TooltipServersScoreType = "Choose whether you prefer highest speed (ex. file-sharing) or low-latency speed (ex. gaming)";
		public static string TooltipServersLockCurrent = "Never leave the current server. \r\nFor example if you don\'t want to change your IP for port forwarding reasons.";
		public static string TooltipServersConnect = "Connect to the selected server now";
		public static string TooltipServersShowAll = "Show all servers if checked, or show only selected servers based on whitelist & blacklist";
		public static string TooltipServersUndefined = "Clear the selected servers from whitelist and blacklist";
		public static string TooltipServersBlackList = "Add the selected servers to blacklist. \r\nThe system will never connect to blacklisted servers.";
		public static string TooltipServersWhiteList = "Add the selected server to whitelist.\r\nThe system will only connect to whitelisted servers.";
		public static string TooltipServersRename = "Rename an OpenVPN connection";
		public static string TooltipServersMore = "Show more information about the selected connection";
		public static string TooltipServersRefresh = "Refresh connections list, ping results, discovered information";
		public static string TooltipAreasUndefined = "Clear the selected areas from whitelist and blacklist";
		public static string TooltipAreasBlackList = "Add the selected areas to blacklist. \r\nThe system will never connect to servers in blacklisted areas.";
		public static string TooltipAreasWhiteList = "Add the selected area to whitelist.\r\nThe system will only connect to servers in whitelisted areas.";
		public static string TooltipLogsCommand = "Run a direct command to Eddie or OpenVPN Management";
		public static string TooltipLogsClean = "Clear logs";
		public static string TooltipLogsCopy = "Copy to clipboard";
		public static string TooltipLogsSave = "Save to file";
		public static string TooltipLogsSupport = "Log system information and copy to clipboard. Useful for support requests";
		public static string CommandLoginButton = "Login";
		public static string CommandLogout = "Logout";
		public static string CommandConnect = "Connect to a recommended server";
		public static string CommandConnectSubtitle = "or choose a specific server in 'Servers' tab.";
		public static string CommandDisconnect = "Disconnect";
		public static string CommandCancel = "Cancel";
		public static string ManifestUpdateForce = "Updating now...";
		public static string ResetSettingsConfirm = "WARNING: all custom settings will be lost. Continue?";
		public static string ResetSettingsDone = "Settings reverted to default.";
		public static string OpenVpnManagementCommandFail = "Unable to send OpenVPN management command.";

		public static string FilterAllFiles = "All files (*.*)|*.*";
		public static string FilterTextFiles = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

		public static string ScoreUnknown = "NC";

		public static string ConsoleHelp = "Run the program with login & password of your AirVPN account. For example:\neddie-ui -cli -login=mynick -password=mypassword\n\nSee https://eddie.website/ for more information, or run with -help for inline manual.";

		public static string ToolsCurlRequired = "curl is required.";
		public static string ToolsCurlVersionNotSupported = "curl version {1} installed in your system is too old. Version {2} or higher required. Please upgrade.";


		public static string TimeJustNow = "Just now";
		public static string TimeAgo = "ago";
		public static string TimeRemain = "remain";

		public static string StatsServerName = "Server Name";
		public static string StatsServerLatency = "Server Latency";
		public static string StatsServerLocation = "Server Location";
		public static string StatsServerLoad = "Server Load";
		public static string StatsServerUsers = "Server Users";
		public static string StatsLogin = "Login";
		public static string StatsKey = "Device";
		public static string StatsVpnStart = "VPN Start";
		public static string StatsVpnSpeedDownload = "VPN Speed Download";
		public static string StatsVpnSpeedUpload = "VPN Speed Upload";		
		public static string StatsSessionStart = "Session Start";
		public static string StatsSessionTotalDownload = "Session Total Download";
		public static string StatsSessionTotalUpload = "Session Total Upload";
		public static string StatsVpnTotalDownload = "VPN Total Download";
		public static string StatsVpnTotalUpload = "VPN Total Upload";
		public static string StatsVpnEntryIP = "IP Entry";
		public static string StatsVpnExitIPv4 = "IPv4 Exit";
		public static string StatsVpnExitIPv6 = "IPv6 Exit";
		public static string StatsVpnProtocol = "VPN Protocol";
		public static string StatsVpnPort = "VPN Port";
		public static string StatsVpnRealIp = "Real IP detected";
		public static string StatsVpnIp = "VPN IP";
		public static string StatsVpnDns = "VPN DNS";
		public static string StatsVpnInterface = "VPN Interface";
		public static string StatsVpnGateway = "VPN Gateway";
		public static string StatsVpnCipher = "VPN Cipher";
		public static string StatsVpnControlChannel = "VPN Control Channel";
		public static string StatsVpnGeneratedOVPN = "VPN OpenVPN Config";
		public static string StatsVpnGeneratedOVPNPush = "VPN OpenVPN Config with pushes";
		public static string StatsManifestLastUpdate = "Latest Manifest Update";
		public static string StatsDiscovery = "Discovery stats";
		public static string StatsPinger = "Pinger stats";
		public static string StatsSystemTimeServerDifference = "Server Time Difference";
		public static string StatsSystemPathProfile = "Profile path";
		public static string StatsSystemPathData = "Data path";
		public static string StatsSystemPathApp = "Application path";
		public static string StatsSystemReport = "System Report";



		public static string ManName = "eddie-ui -- Eddie - OpenVPN UI, console edition";
		public static string ManHeaderComment = "Generated with 'eddie-ui -cli -help -help.format=man'";
		public static string ManSynopsis = "eddie-ui -cli [OPTIONS...]";
		public static string ManDescription = "Eddie is an OpenVPN wrapper providing advanced features such as network lock, connections over SSL/TLS and SSH tunnels, programmable events, customizable routes, extensive control on OpenVPN directives and a comfortable, optional GUI. For more information: [link]https://eddie.website[/link]";
		public static string ManCopyright = "Copyright (C) AirVPN - Released under GNU General Public License - [link]http://www.gnu.org/licenses/gpl.html[/link]";
		public static string ManHeaderOption1 = "Options can be written as [i]--option=\"value\"[/i]\nDouble-quotes and minus are optional, [i]option=value[/i] is valid.\nEvery option have only one value.\nOptions without a value like [i]--option[/i] are treated as [i]--option=\"True\"[/i]";
		public static string ManHeaderOption2 = "Default options are read from the profile. Options in command-line override them but are not saved in the profile at exit.\nYou can run the UI, choose options in the UI, and then launch the command-line edition with the profile options, to avoid to specify all the options in the command-line.";
		public static string ManHeaderOption3 = "The options [i]--login[/i] and [i]--password[/i] are mandatory if they are not already in the profile.\nBy default, the client doesn't connect to any server automatically. Specify [i]--connect[/i] to connect automatically.\nSpecify [i]--netlock[/i] to start with network lock activation.";
		public static string ManHeaderOption4 = "By default the application works interactively: Press [i]n[/i] to connect to a server, [i]x[/i] or [i]ctrl+c[/i] to exit.\nSpecify [i]--batch[/i] for non-interactive mode.";

		public static string ManOptionCli = "Don't show the graphics user interface. Connect directly.";
		public static string ManOptionHelp = "Show help manual.";
		public static string ManOptionHelpFormat = "Format of the help manual. Can be [i]man[/i], [i]text[/i], [i]html[/i] or [i]bbcode[/i]";
		public static string ManOptionLogin = "Login of your AirVPN account.";
		public static string ManOptionPassword = "Password of your AirVPN account.";
		public static string ManOptionRemember = "[i]True[/i] if username and password persist after exit.";
		public static string ManOptionKey = "Key name.";
		public static string ManOptionServer = "Server to connect to. Leave empty to pick recommended server.";
		public static string ManOptionConnect = "Connect automatically at startup. Only for GUI, command-line always starts directly.";
		public static string ManOptionNetLock = "Network lock automatically at startup.";
		public static string ManOptionProfile = "Profile name. Use it to store different set of options";
		public static string ManOptionPath = "Data path. May be a full path or special value [i]program[/i] or [i]home[/i]";

		public static string ManOptionServersWhiteList = "List of servers available for connection. Leave empty for all servers. Separate values with comma. Use ID or display name. Example: 'Canopus,Syrma,Taygeta'";
		public static string ManOptionServersBlackList = "List of servers to avoid in connection. Same syntax of whitelist.";
		public static string ManOptionServersStartLast = "[i]True[/i] if you want to connect to the last used server. [i]False[/i] to choose it automatically.";
		public static string ManOptionServersLockLast = "[i]True[/i] if you never leave the selected server, not even in case of disconnection.";
		public static string ManOptionServersScoreType = "May be [i]Speed[/i] or [i]Latency[/i]. Affects scoring of servers, indicates if you prefer a better speed or a better latency";
		public static string ManOptionAreasWhiteList = "List of areas available for connection. Same as server whitelist syntax. Example: 'nl,de'";
		public static string ManOptionAreasBlackList = "List of areas to avoid in connection. Same as whitelist syntax.";

		public static string ManOptionModeProtocol = "Protocol for connection. [i]UDP[/i], [i]TCP[/i] for direct openvpn connection. [i]SSH[/i], [i]SSL[/i] for additional tunneling";
		public static string ManOptionModePort = "Port for connection.";
		public static string ManOptionModeAlt = "0 to use the default entry IP, 1 or more for additional entry IP";

		public static string ManOptionProxyMode = "Proxy mode: [i]none[/i], [i]http[/i] or [i]socks[/i]. [i]protocol[/i] option must be [i]TCP[/i].";
		public static string ManOptionProxyHost = "Proxy host";
		public static string ManOptionProxyPort = "Proxy port";
		public static string ManOptionProxyAuth = "Proxy authentication method: [i]None[/i], [i]Basic[/i] or [i]NTLM[/i]";
		public static string ManOptionProxyLogin = "Proxy login, for authentication";
		public static string ManOptionProxyPassword = "Proxy password, for authentication";
		public static string ManOptionProxyTorControlPort = "Tor Control port";
		public static string ManOptionProxyTorControlAuth = "Tor Control needs authentication. Normally the Tor Browser Bundle requires authentication and accepts a file cookie password";
		public static string ManOptionProxyTorControlPassword = "Tor Control password. If empty, the software tries to detect the file cookie password";

		public static string ManOptionRoutesCustom = "Custom routes. Format: '{ip or range},{in/out},{note (optional)};...'. Separate multiple routes with ;. Example: '1.2.3.4,in;2.3.4.5/23,out,'test'";
		public static string ManOptionRoutesRemoveDefault = "Remove the default gateway route. Can not work as expected if DHCP renew occurs.";
		public static string ManOptionRoutesDefault = "Specify whether routes that don't match the custom route must be inside ([i]in[/i]) or outside ([i]out[/i]) the tunnel.";

		public static string ManOptionDnsMode = "[i]None[/i] to disable DNS switch. [i]Auto[/i] to let the software choose the best method. Otherwise choose a method. Refer to [link]https://airvpn.org/faq/software_advanced/[/link] for more information about each method on each platform.";
		public static string ManOptionDnsServers = "List of DNS servers. If not empty, override the DNS suggested by VPN server. Separate multiple values with comma.";
		public static string ManOptionDnsCheck = "When the connection is established, if [i]True[/i] it try to resolve domain names that are resolved only by AirDNS server, to ensure that system is correctly using our DNS server.\nIt's not mandatory to use our DNS server, but it's recommended to enjoy our Geolocation Routing service and avoid DNS blocks by your provider.";

		public static string ManOptionNetLockMode = "Network Lock mode. Can be [i]None[/i], [i]Auto[/i] or a method name.";
		public static string ManOptionNetLockAllowPrivate = "Allow private network in Network Lock mode.";
		public static string ManOptionNetLockAllowPing = "Allow ping (ICMP) in Network Lock mode.";
		public static string ManOptionNetLockAllowDNS = "Allow detected DNS servers in Network Lock mode.";
		public static string ManOptionNetLockAllowedsIps = "List (comma-separated) of IP or range allowed in Network Lock mode.";

		public static string ManOptionIPv6 = "IPv6 mode. Can be [i]disabled[/i] to disable IPv6, or [i]none[/i]";

		public static string ManOptionToolsOpenVpnPath = "Allows you to specify a path to OpenVPN executable, to skip the executable bundled with Eddie.";
		public static string ManOptionToolsSshPath = "Path to a custom SSH tunnel executable";
		public static string ManOptionToolsSslPath = "Path to a custom SSL tunnel executable";
		public static string ManOptionToolsCurlPath = "Path to a custom curl executable, used only with socks proxy";

		public static string ManOptionOpenVpnCustom = "Allows you to specify custom OpenVPN directives.";
		public static string ManOptionOpenVpnDevNode = "OpenVPN dev-node directive.";
		public static string ManOptionOpenVpnSndBuf = "TCP/UDP send buffer size. Special values: [i]-2[/i]: Automatic, [i]-1[/i]: OpenVPN default.";
		public static string ManOptionOpenVpnRcvBuf = "TCP/UDP receive buffer size. Special values: [i]-2[/i]: Automatic, [i]-1[/i]: OpenVPN default.";
		public static string ManOptionOpenVpnDirectives = "Base OpenVPN directives.";
		public static string ManOptionOpenVpnSkipDefaults = "If [i]False[/i] the custom directives are appended to the default directive.";
		public static string ManOptionOpenVpnManagementPort = "Default port of OpenVPN Management Interface.";
		public static string ManOptionSshPort = "Default port of SSH Tunnel. If [i]0[/i], a random port is used.";
		public static string ManOptionSslPort = "Default port of SSL Tunnel. If [i]0[/i], a random port is used.";

		public static string ManOptionOsSingleInstance = "If [i]False[/i], multiple instances of the software can be opened.";
		public static string ManOptionWebUiEnabled = "Web-interface enabled or not";
		public static string ManOptionWebUiAddress = "Web-interface bind address. Leave empty for all interfaces, localhost or specify an IP address.";
		public static string ManOptionWebUiPort = "Web-interface port.";

		public static string ManOptionAdvancedExpert = "Activate some expert information and features; allows sending commands to OpenVPN Management Interface via Logs window; show verbose logs message in main windows";
		public static string ManOptionAdvancedCheckRoute = "If [i]True[/i] send a request to the server, that check it come from within the tunnel, and reply with an acknowledgement.";
		public static string ManOptionAdvancedPingerEnabled = "If [i]True[/i] the software pings servers to determine latency score. Pings are not performed during VPN connection.";
		public static string ManOptionAdvancedPingerDelay = "Ping each server every [i]X[/i] seconds. If [i]0[/i], the recommended values are used.";
		public static string ManOptionAdvancedPingerRetry = "Ping every server that doesn't have ping results every X seconds. If [i]0[/i], the recommended values are used.";
		public static string ManOptionAdvancedPingerJobs = "Maximum parallel jobs/thread for pinging purpose.";
		public static string ManOptionAdvancedPingerValid = "Global pinger results valid if all ping reply are maximum X seconds ago. If [i]0[/i], the recommended values are used.";

		public static string ManOptionEventFileName = "Filename of the script/executable to launch on event.";
		public static string ManOptionEventArguments = "Arguments of the script/executable.";
		public static string ManOptionEventWaitEnd = "Use [i]True[/i] if the software needs to wait the end (synchronous) or [i]False[/i] to be asynchronous.";

		public static string ManOptionWindowsAdapterService = "OpenVPN TUN/TAP adapter identifier.";
		public static string ManOptionWindowsDisableDriverUpgrade = "Don't try to upgrade the TUN/TAP driver.";
		public static string ManOptionWindowsTapUp = "Force the TAP interface to come UP.";
		public static string ManOptionWindowsDhcpDisable = "If a DHCP adapter is renewed during connection, Windows may reset the original DNS settings.\nIf this option is active, when the connection starts the client changes any DHCP adapter to Static.\nWhen the connection is closed, the client resets every adapter to the original state.\nNo action is performed if there isn't any adapter in DHCP.\n\nThis option is equivalent to one feature of the https://www.dnsleaktest.com/ scripts.";
		public static string ManOptionWindowsWfp = "Use Windows Filtering Platform.";
		public static string ManOptionWindowsWfpDynamic = "If [i]True[/i], Windows Filtering Platform rules don't survive if the process crash.";
		public static string ManOptionWindowsIPv6DisableAtOs = "If IPv6 needs to be disabled, try to disable it also at OS level (registry).";
		public static string ManOptionWindowsDnsForceAllInterfaces = "If DNS leak protection is active, change DNS settings of all network interfaces.";
		public static string ManOptionWindowsDnsLock = "DNS leak protection.";

		public static string ManOptionUiUnit = "Unit of measurement of data volume and speed. Can be [i]bytes[/i], [i]bits[/i] or empty. If empty, bytes are used for volume, bits for speed.";
		public static string ManOptionUiIEC = "Use IEC standard and not metric standard. For example kilobyte are replaced by kibibyte";

		// Providers - Eddie v3        
		public static string ManOptionServicesOpenVpnAutoSync = "If [i]True[/i], profiles are automatically in-sync with the path. If false, profiles are imported and can be edited.";
		public static string ManOptionServicesOpenVpnPath = "Path where OpenVPN configuration files are fetched from";
		public static string ManOptionServicesDnsCheck = "When the connection is established, try to resolve domain names that are resolved only by AirDNS server, to ensure that system is correctly using our DNS server.\nIt's not mandatory to use our DNS server, but it's recommended to enjoy our Geolocation Routing service and avoid DNS blocks by your provider.";
		public static string ManOptionServicesTunnelCheck = "Send a request to the server, that verifies it comes from within the tunnel, and reply with an acknowledgement.";

		// Platform Windows		
		public static string NetworkAdapterDhcpDone = "Network adapter DHCP switched to static ({1})";
		public static string NetworkAdapterDhcpRestored = "DHCP of a network adapter restored to original settings ({1})";
		public static string NetworkAdapterMetricSwitch = "Interface {1} metric changed from {2} to {3}, layer {4}";
		public static string NetworkAdapterMetricRestore = "Interface {1} metric restored from {2} to {3}, layer {4}";
		public static string IPv6DisabledOs = "IPv6 disabled with registry changes.";
		public static string IPv6RestoredOs = "IPv6 restored with registry changes.";
		public static string IPv6DisabledWpf = "IPv6 disabled with packet filtering.";
		public static string IPv6RestoredWpf = "IPv6 restored with packet filtering.";
		public static string DnsLockActivatedWpf = "DNS leak protection with packet filtering enabled.";
		public static string DnsLockDeactivatedWpf = "DNS leak protection with packet filtering disabled.";
		public static string HackInterfaceUpDone = "Eddie Windows Interface Hack executed ({1})";
		public static string WfpStartFail = "Windows WFP, Start failed: {1}";
		public static string WfpRuleAddFail = "Windows WFP, Add rule failed: {1}:{2}";
		public static string WfpRuleAddSuccess = "Windows WFP, Add rule success: {1}";
		public static string WfpRuleRemoveFail = "Windows WFP, Add rule failed: {1}";
		public static string WfpRecovery = "Windows WFP, recovery of pending rules.";

		// Platform Windows
		public static string OsWindowsNetworkAdapterDnsDone = "DNS {1} of a network adapter forced ({2}, from {3} to {4})";
		public static string OsWindowsNetworkAdapterDnsRestored = "DNS {1} of a network adapter restored to original settings ({2}, to {3})";

		// Platform macOS
		public static string OsMacNetworkAdapterDnsDone = "DNS of a network adapter forced ({1}, from {2} to {3})";
		public static string OsMacNetworkAdapterDnsRestored = "DNS of a network adapter restored to original settings ({1}, to {2})";
		public static string OsMacNetworkAdapterIPv6Disabled = "IPv6 disabled on network adapter ({1})";
		public static string OsMacNetworkAdapterIPv6Restored = "IPv6 restored on network adapter ({1})";

		// Platform Linux
		public static string OsLinuxDnsResolvConfScript = "DNS of the system will be updated to VPN DNS (ResolvConf method)";
		public static string OsLinuxDnsRenameBackup = "/etc/resolv.conf moved to /etc/resolv.conf.eddie as backup";
		public static string OsLinuxDnsRenameDone = "DNS of the system updated to VPN DNS (Rename method: /etc/resolv.conf generated)";
		public static string OsLinuxDnsRenameRestored = "DNS of the system restored to original settings (Rename method)";
		public static string OsLinuxIPv6Warning = "IPv6 detected.\n\nThis can cause data leak ONLY if your ISP provides IPv6 support.\nCurrently our software can't disable and restore safely IPv6 on Linux.\nIf you continue, IPv6 detection will be disabled. You can re-enable it in Preferences -> Advanced -> IPv6.\n\nContinue?";
		public static string OsLinuxIPv6WarningUnableToDetect = "Unable to understand if IPv6 is active.";

		// End Messages
	}
}
