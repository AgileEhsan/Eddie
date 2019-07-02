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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

namespace Eddie.Deploy
{
	public class Engine
	{
		private static string PathBase = "";
		private static string PathBaseTemp = "";
		private static string PathBaseCommon = "";
		private static string PathBaseDeploy = "";
		private static string PathBaseRepository = "";
		private static string PathBaseResources = "";
		private static string PathBaseTools = "";
		private static string PathBaseSigning = "";

		private static string SO = "";
		private static List<string> Arguments;

		private static int Errors = 0;

		public void Start(string[] args)
		{
			Log("Eddie deployment v1.5 - " + DateTime.UtcNow.ToLongDateString() + " - " + DateTime.UtcNow.ToLongTimeString());
			Log("Arguments allowed: 'verbose' (show more logs), 'official' (signing files)");

			Arguments = new List<string>();
			foreach (string arg in args)
			{
				string a = arg.ToLowerInvariant().Trim();
				Log("Argument detected: " + a);
				Arguments.Add(a);
			}

			if (IsVerbose())
			{
				Log("PlatformOS: " + Environment.OSVersion.Platform.ToString());
				Log("VersionString: " + Environment.OSVersion.VersionString.ToString());
			}

			/* -------------------------------
			   Detect Platform
			------------------------------- */

			if (Environment.OSVersion.VersionString.IndexOf("Windows") != -1)
				SO = "windows";
			else if ((Environment.OSVersion.Platform.ToString() == "Unix") && (Shell("uname") == "Darwin"))
			{
				SO = "macos";
			}
			else if (Environment.OSVersion.Platform.ToString() == "Unix")
			{
				SO = "linux";
			}
			else
			{
				Log("Unknown platform.");
				return;
			}
			Log("Platform: " + SO);

			//PathBase = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent.Parent.Parent.FullName;
			PathBase = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.Parent.FullName;

			Log("Path base: " + PathBase);

			PathBaseTemp = new DirectoryInfo(PathBase + "/tmp").FullName;
			PathBaseCommon = new DirectoryInfo(PathBase + "/common").FullName;
			PathBaseDeploy = new DirectoryInfo(PathBase + "/deploy").FullName;
			PathBaseRepository = new DirectoryInfo(PathBase + "/repository/files").FullName;
			PathBaseResources = new DirectoryInfo(PathBase + "/resources").FullName;
			PathBaseTools = new DirectoryInfo(PathBase + "/tools").FullName;
			PathBaseSigning = new DirectoryInfo(PathBase + "/repository/signing").FullName;

			string versionString3 = ExtractBetween(ReadTextFile(PathBase + "/src/Lib.Common/Constants.cs"), "public static string VersionDesc = \"", "\"");

			/* --------------------------------------------------------------
			   Checking environment, required
			-------------------------------------------------------------- */

			if (SO == "linux")
			{
				int uid = 9999;
				int.TryParse(Shell("id -u"), out uid);
				if (uid != 0)
				{
					Console.WriteLine("Sorry, must be run as root, because need to chown to root:root on some file for example Debian docs.");
					return;
				}
			}

			if (SO == "linux")
			{
				if (Shell("tar --version").IndexOf("GNU tar") == -1)
				{
					Console.WriteLine("tar required.");
					return;
				}

				if (File.Exists("/usr/include/zlib.h") == false)
				{
					Console.WriteLine("zlib1g-dev required.");
					return;
				}

				if (File.Exists("/usr/include/zlib.h") == false)
				{
					Console.WriteLine("zlib1g-dev required.");
					return;
				}

				if ((IsOfficial()) && (Shell("which dpkg-sig").Trim() == ""))
				{
					Console.WriteLine("dpkg-sig required.");
					return;
				}

				if (Shell("which lintian").Trim() == "")
				{
					Console.WriteLine("lintian required.");
					return;
				}
			}

			/* --------------------------------------------------------------
			   Checking environment, optional
			-------------------------------------------------------------- */

			bool AvailableMkBundle = false;
			string MonoVersion = "";
			bool AvailableDpkg = false;
			bool AvailableRPM = false;
			bool AvailableAUR = true;

			if (SO == "linux")
			{
				MonoVersion = Shell("mono --version");
				MonoVersion = MonoVersion.Replace("Mono JIT compiler version ", "");
				MonoVersion = MonoVersion.Substring(0, MonoVersion.IndexOf(" "));
				Console.WriteLine("Mono version:" + MonoVersion);

				if (Shell("mkbundle --help").IndexOf("Usage is: mkbundle") == -1)
				{
					Console.WriteLine("'mkbundle' not found. Package mono-complete required. Portable edition will be unavailable.");
				}
				else
					AvailableMkBundle = true;

				if (Shell("dpkg --version").IndexOf("package management program") == -1)
				{
					Console.WriteLine("'dpkg' required. Debian installer will be unavailable.");
					return;
				}
				else
					AvailableDpkg = true;

				if (Shell("rpmbuild --version").IndexOf("RPM version") == -1)
				{
					Console.WriteLine("'rpmbuild' required. RPM installer will be unavailable.");
					return;
				}
				else
					AvailableRPM = true;
			}

			/* -------------------------------
				Pause, to read logs.
			------------------------------- */

			Pause();

			if (IsSigning())
			{
				string[] dirs = Directory.GetDirectories(PathBaseDeploy, SO + "*");
				foreach (string dir in dirs)
				{
					SignPath(SO, "", dir);
				}
			}
			else
			{
				/* -------------------------------
				   Build packages list
				------------------------------- */

				List<Package> ListPackages = new List<Package>();

				if (SO == "windows")
				{
					foreach (string arch in new string[] { "x64", "x86" })
					{
						foreach (string ui in new string[] { "ui", "cli" })
						{							
							foreach (string format in new string[] { "portable", "installer" })
							{								
								foreach (string os in new string[] { "windows-10", "windows-7", "windows-vista", "windows-xp" })
								{
									string netFramework = "4.0";									
									/* 2.17.1
									if ((os == "windows-7") && (IsEddie3() == false))
										netFramework = "3.5";
									if ((os == "windows-vista") && (IsEddie3() == false))
										netFramework = "4.0";
									if ((os == "windows-xp") && (IsEddie3() == false))
										netFramework = "4.0";
									*/
									
									ListPackages.Add(new Package(os, arch, ui, true, netFramework, format));
								}
							}
						}
					}
				}

				if (SO == "linux")
				{
					string arch = Shell("uname -m");

					if (arch == "x86_64")
						arch = "x64";
					else if (arch == "armv7l")
						arch = "armhf";
					else
						arch = "x86";

					ListPackages.Add(new Package("linux", arch, "cli", true, "4.0", "mono"));
					ListPackages.Add(new Package("linux", arch, "ui", true, "4.0", "mono"));
					ListPackages.Add(new Package("linux", arch, "cli", true, "4.0", "portable"));
					ListPackages.Add(new Package("linux", arch, "ui", true, "4.0", "portable"));
					ListPackages.Add(new Package("linux", arch, "ui", false, "4.0", "debian"));
					ListPackages.Add(new Package("linux", arch, "ui", false, "4.0", "opensuse"));
					ListPackages.Add(new Package("linux", arch, "ui", false, "4.0", "fedora"));
					//ListPackages.Add(new Package("linux", arch, "ui", false, "4.0", "aur"));
				}

				if (SO == "macos")
				{
					ListPackages.Add(new Package("macos", "x64", "cli", true, "4.0", "mono"));
					ListPackages.Add(new Package("macos", "x64", "cli", true, "4.0", "portable"));
					ListPackages.Add(new Package("macos", "x64", "ui", true, "4.0", "portable"));
					ListPackages.Add(new Package("macos", "x64", "ui", true, "4.0", "installer"));
					ListPackages.Add(new Package("macos", "x64", "ui", true, "4.0", "disk"));
				}

				if (SO == "linux")
					PathBaseTemp = "/tmp/eddie_deploy";

				foreach (Package package in ListPackages)
				{
					string platform = package.Platform;
					string arch = package.Architecture;
					string archCompile = package.ArchitectureCompile;
					string ui = package.UI;
					string requiredNetFramework = package.NetFramework;
					string format = package.Format;

					string archiveName = "eddie-" + ui + "_" + versionString3 + "_" + platform + "_" + arch + "_" + format;
					string fileName = archiveName;
					string pathCommon = PathBaseCommon;
					string pathDeploy = PathBaseDeploy + "/" + platform + "_" + arch;
					string pathTemp = PathBaseTemp + "/" + archiveName;
					//string pathRelease = PathBaseRelease + "/" + archCompile + "/Release/";

					// Exceptions
					if (platform == "windows-10") // Windows_10 use the same common files of Windows
						pathDeploy = pathDeploy.Replace("windows-10", "windows");
					if (platform == "windows-7") // Windows_7 use the same common files of Windows
						pathDeploy = pathDeploy.Replace("windows-7", "windows");

					// Start
					Log("------------------------------");
					Log("Building '" + archiveName + "'");

					bool skipCompile = false;
					if (SO == "macos")
						skipCompile = true;

					if (skipCompile)
					{
						Log("Expected already compiled binaries for this platform.");
					}
					else
					{
						if (Compile(SO, archCompile, requiredNetFramework) == false)
						{
							continue;
						}
					}

					Log("Packaging files");

					CreateDirectory(pathTemp);

					CreateDirectory(PathBaseRepository, false);

					if (platform.StartsWith("windows"))
					{
						if (ui == "ui")
						{
							CopyAll(pathDeploy, pathTemp);
							CopyAll(pathCommon, pathTemp + "/res");

							string pathRelease = new DirectoryInfo(PathBase + "/src/App.Forms.Windows/bin/" + archCompile + "/Release").FullName;

							CopyFile(pathRelease, "Lib.Core.dll", pathTemp);
							CopyFile(pathRelease, "Lib.Common.dll", pathTemp);
							CopyFile(pathRelease, "Lib.Platform.Windows.dll", pathTemp);
							CopyFile(pathRelease, "Lib.Forms.dll", pathTemp);
							if (format == "portable")
								CopyFile(pathRelease, "App.Forms.Windows.exe", pathTemp, "Eddie-UI.exe");
							else
								CopyFile(pathRelease, "App.Forms.Windows.exe", pathTemp, "Eddie-UI.exe"); // TODO Eddie3: "Eddie-UI.exe"

							pathRelease = new DirectoryInfo(PathBase + "/src/App.CLI.Windows/bin/" + archCompile + "/Release").FullName;
							CopyFile(pathRelease, "App.CLI.Windows.exe", pathTemp, "Eddie-CLI.exe");
						}
						else if (ui == "cli")
						{
							CopyAll(pathDeploy, pathTemp);
							CopyAll(pathCommon, pathTemp + "/res");

							string pathRelease = new DirectoryInfo(PathBase + "/src/App.CLI.Windows/bin/" + archCompile + "/Release").FullName;

							CopyFile(pathRelease, "Lib.Core.dll", pathTemp);
							CopyFile(pathRelease, "Lib.Common.dll", pathTemp);
							CopyFile(pathRelease, "Lib.Platform.Windows.dll", pathTemp);
							CopyFile(pathRelease, "App.CLI.Windows.exe", pathTemp, "Eddie-CLI.exe");
						}

						SignPath(platform, format, pathTemp);

						if (format == "portable")
						{
							string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".zip");

							if (File.Exists(pathFinal))
								File.Delete(pathFinal);

							// ZIP
							string command = PathBaseTools + "/windows/7za.exe a -mx9 -tzip";
							command += " \"" + pathFinal + "\" \"" + pathTemp;
							Shell(command);
						}
						else if (format == "installer")
						{
							string nsis = "";

							if (ui == "ui")
							{
								nsis = ReadTextFile(PathBaseResources + "/nsis/Eddie-UI.nsi");
							}
							else if (ui == "cli")
							{

							}

							if (nsis != "")
							{
								string pathExe = NormalizePath(PathBaseRepository + "/" + fileName + ".exe");

								nsis = nsis.Replace("{@resources}", NormalizePath(PathBaseResources + "/nsis"));
								nsis = nsis.Replace("{@temp}", NormalizePath(pathTemp));
								nsis = nsis.Replace("{@out}", pathExe);

								List<string> filesList = GetFilesRecursive(pathTemp);

								string filesAdd = "";
								string filesDelete = "";
								string filesAddLastPath = "";
								List<string> pathsToDelete = new List<string>();
								foreach (string filePath in filesList)
								{
									string name = filePath.Substring(pathTemp.Length + 1);

									FileInfo fi = new FileInfo(filePath);

									if (fi.Directory.FullName != filesAddLastPath)
									{
										filesAddLastPath = fi.Directory.FullName;
										string pathName = "$INSTDIR" + filesAddLastPath.Substring(pathTemp.Length);
										filesAdd += "SetOutPath \"" + pathName + "\"\r\n";
										if (pathName != "$INSTDIR")
											pathsToDelete.Add(pathName);
									}

									filesAdd += "File \"" + name + "\"\r\n";
									filesDelete += "Delete \"$INSTDIR\\" + name + "\"\r\n";
								}

								foreach (string pathToDelete in pathsToDelete)
								{
									filesDelete += "RMDIR \"" + pathToDelete + "\"\r\n";
								}

								nsis = nsis.Replace("{@files_add}", filesAdd);
								nsis = nsis.Replace("{@files_delete}", filesDelete);

								if (arch == "x64")
									nsis = nsis.Replace("$PROGRAMFILES", "$PROGRAMFILES64");

								WriteTextFile(pathTemp + "/Eddie.nsi", nsis);

								//Shell("c:\\Program Files (x86)\\NSIS\\makensisw.exe", "\"" + NormalizePath(pathTemp + "/Eddie.nsi") + "\"");
								Shell("c:\\Program Files (x86)\\NSIS\\makensis.exe", "\"" + NormalizePath(pathTemp + "/Eddie.nsi") + "\"");

								SignFile(platform, format, pathExe);
							}
						}
					}
					else if (platform == "linux")
					{
						if (format == "mono")
						{
							if (ui == "cli")
							{
								CopyAll(pathDeploy, pathTemp);
								CopyAll(pathCommon, pathTemp + "/res");

								RemoveFile(pathTemp + "/eddie-tray");
								RemoveFile(pathTemp + "/libgdiplus.so.0");
								RemoveFile(pathTemp + "/libappindicator.so.1");

								string pathRelease = new DirectoryInfo(PathBase + "/src/App.CLI.Linux/bin/" + archCompile + "/Release").FullName;

								CopyFile(pathRelease, "Lib.Core.dll", pathTemp);
								CopyFile(pathRelease, "Lib.Common.dll", pathTemp);
								CopyFile(pathRelease, "Lib.Platform.Linux.dll", pathTemp);
								CopyFile(pathRelease, "App.CLI.Linux.exe", pathTemp, "Eddie-CLI.exe");

								CopyFile(PathBaseResources + "/linux_portable/", "eddie-cli.sh", pathTemp, "eddie-cli");
								Shell("chmod 755 \"" + pathTemp + "/eddie-cli\"");
							}
							else if (ui == "ui")
							{
								CopyAll(pathDeploy, pathTemp);
								CopyAll(pathCommon, pathTemp + "/res");

								string pathRelease = new DirectoryInfo(PathBase + "/src/App.Forms.Linux/bin/" + archCompile + "/Release").FullName;

								CopyFile(pathRelease, "Lib.Core.dll", pathTemp);
								CopyFile(pathRelease, "Lib.Common.dll", pathTemp);
								CopyFile(pathRelease, "Lib.Platform.Linux.dll", pathTemp);
								CopyFile(pathRelease, "Lib.Forms.dll", pathTemp);
								CopyFile(pathRelease, "App.Forms.Linux.exe", pathTemp, "Eddie-UI.exe");

								CopyFile(new DirectoryInfo(PathBase + "/src/App.CLI.Linux/bin/" + archCompile + "/Release").FullName, "App.CLI.Linux.exe", pathTemp, "Eddie-CLI.exe");

								CopyFile(PathBaseResources + "/linux_portable/", "eddie-ui.sh", pathTemp, "eddie-ui");
								Shell("chmod 755 \"" + pathTemp + "/eddie-ui\"");
							}

							string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".tar.gz");

							if (File.Exists(pathFinal))
								File.Delete(pathFinal);

							Shell("chmod 755 \"" + pathTemp + "/openvpn\"");
							Shell("chmod 755 \"" + pathTemp + "/stunnel\"");
							Shell("chmod 755 \"" + pathTemp + "/eddie-tray\"");
							Shell("chmod 644 \"" + pathTemp + "/libLib.Platform.Linux.Native.so\"");

							RemoveFile(pathTemp + "/libgdiplus.so.0");
							RemoveFile(pathTemp + "/libMonoPosixHelper.so");
							RemoveFile(pathTemp + "/libappindicator.so.1");

							CreateDirectory(pathTemp + "/" + fileName);
							MoveAll(pathTemp, pathTemp + "/" + fileName);

							Shell("chown -R root:root " + pathTemp);

							// TAR.GZ
							string command2 = "cd \"" + pathTemp + "\" && tar cvfz \"" + pathFinal + "\" " + "*";
							Shell(command2);
						}
						else if ((format == "portable") && (AvailableMkBundle))
						{
							CopyAll(pathDeploy, pathTemp);
							CopyAll(pathCommon, pathTemp + "/res");

							Shell("chmod 755 \"" + pathTemp + "/stunnel\"");
							if (ui == "cli")
							{
								RemoveFile(pathTemp + "/eddie-tray");
								RemoveFile(pathTemp + "/libgdiplus.so.0");
								RemoveFile(pathTemp + "/libappindicator.so.1");
							}

							//CopyFile(PathBaseResources + "/linux_portable/eddie.config", pathTemp + "/eddie.config");
							//CopyFile(pathBaseResources + "/linux_portable/eddie.machine.config", pathTemp + "/eddie.machine.config");

							// mkbundle


							// 4.0 version
							/*
							string command = "mkbundle ";
							command += " --deps";
							command += " --keeptemp";
							command += " --static";
							command += " --config \"" + PathBaseResources + "/linux_portable/eddie.config\"";
							command += " --machine-config /etc/mono/4.0/machine.config";
							command += " -z";
							*/

							// 4.5 version
							string command = "";

							if (ui == "cli")
							{
								string pathRelease = new DirectoryInfo(PathBase + "/src/App.CLI.Linux/bin/" + archCompile + "/Release").FullName;

								command = "cd \"" + pathRelease + "\" && mkbundle";

								command += " \"" + pathRelease + "/App.CLI.Linux.exe\"";
								command += " \"" + pathRelease + "/Lib.Core.dll\"";
								command += " \"" + pathRelease + "/Lib.Common.dll\"";
								command += " \"" + pathRelease + "/Lib.Platform.Linux.dll\"";
							}
							else if (ui == "ui")
							{
								string pathRelease = new DirectoryInfo(PathBase + "/src/App.Forms.Linux/bin/" + archCompile + "/Release").FullName;

								command = "cd \"" + pathRelease + "\" && mkbundle";

								command += " \"" + pathRelease + "/App.Forms.Linux.exe\"";
								command += " \"" + pathRelease + "/Lib.Forms.dll\"";
								command += " \"" + pathRelease + "/Lib.Core.dll\"";
								command += " \"" + pathRelease + "/Lib.Common.dll\"";
								command += " \"" + pathRelease + "/Lib.Platform.Linux.dll\"";
							}

							if (MonoVersion.StartsWith("5."))
							{
								// Debian 8/9 with Mono 5.x.
								// Don't work for pending bug in mkbundle: https://bugzilla.xamarin.com/show_bug.cgi?id=51650
								command += " --i18n all";
								command += " -L /usr/lib/mono/4.5";
								command += " --keeptemp";
								command += " --static";
								command += " --config \"" + PathBaseResources + "/linux_portable/eddie.config\"";
								command += " --machine-config /etc/mono/4.5/machine.config";
								command += " -z";
							}
							else
							{
								command += " --deps";
								command += " --keeptemp";
								command += " --static";
								command += " --config \"" + PathBaseResources + "/linux_portable/eddie.config\"";
								command += " --machine-config /etc/mono/4.0/machine.config";
								command += " -z";
							}


							// TOOPTIMIZE: This can be avoided, but mkbundle don't support specific exclude, we need to list manually all depencencies and avoid --deps
							// Otherwise, we need to have two different WinForms project (Windows AND Linux)
							//command += " \"" + pathRelease + "/Windows.dll\"";
							//command += " \"" + pathRelease + "/Microsoft.Win32.TaskScheduler.dll\"";



							if (ui == "cli")
							{
								command += " -o \"" + pathTemp + "/eddie-cli\"";
							}
							else if (ui == "ui")
							{
								command += " -o \"" + pathTemp + "/eddie-ui\"";
							}

							Shell(command);

							//RemoveFile(pathTemp + "/eddie.config");

							string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".tar.gz");

							if (File.Exists(pathFinal))
								File.Delete(pathFinal);

							if (ui == "cli")
							{
								Shell("chmod 755 \"" + pathTemp + "/eddie-cli\"");
							}
							else
							{
								Shell("chmod 755 \"" + pathTemp + "/eddie-ui\"");
							}
							Shell("chmod 755 \"" + pathTemp + "/openvpn\"");
							Shell("chmod 755 \"" + pathTemp + "/stunnel\"");
							if (ui == "ui")
								Shell("chmod 755 \"" + pathTemp + "/eddie-tray\"");
							Shell("chmod 644 \"" + pathTemp + "/libLib.Platform.Linux.Native.so\"");

							CreateDirectory(pathTemp + "/" + fileName);
							MoveAll(pathTemp, pathTemp + "/" + fileName);

							Shell("chown -R root:root " + pathTemp);

							// TAR.GZ
							string command2 = "cd \"" + pathTemp + "\" && tar cvfz \"" + pathFinal + "\" " + "*";
							Shell(command2);
						}
						else if ((format == "debian") && (AvailableDpkg))
						{
							string pathRelease = new DirectoryInfo(PathBase + "/src/App.Forms.Linux/bin/" + archCompile + "/Release").FullName;

							string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".deb");

							CreateDirectory(pathTemp + "/usr/lib/eddie-ui");

							CopyFile(pathRelease, "Lib.Core.dll", pathTemp + "/usr/lib/eddie-ui");
							CopyFile(pathRelease, "Lib.Common.dll", pathTemp + "/usr/lib/eddie-ui");
							CopyFile(pathRelease, "Lib.Forms.dll", pathTemp + "/usr/lib/eddie-ui");
							CopyFile(pathRelease, "Lib.Platform.Linux.dll", pathTemp + "/usr/lib/eddie-ui");
							CopyFile(pathRelease, "App.Forms.Linux.exe", pathTemp + "/usr/lib/eddie-ui", "Eddie-UI.exe");
							//CopyFile(new DirectoryInfo(PathBase + "/src/App.CLI.Linux/bin/" + archCompile + "/Release").FullName, "App.CLI.Linux.exe", pathTemp + "/usr/lib/eddie", "Eddie-CLI.exe");

							CopyAll(pathDeploy, pathTemp + "/usr/lib/eddie-ui");
							CopyDirectory(PathBaseResources + "/" + format, pathTemp);

							ReplaceInFile(pathTemp + "/DEBIAN/control", "{@version}", versionString3);
							string debianArchitecture = "unknown";
							if (arch == "x86")
								debianArchitecture = "i386"; // any-i386? not accepted by reprepro
							else if (arch == "x64")
								debianArchitecture = "amd64"; // any-amd64?
							else if (arch == "armhf")
								debianArchitecture = "armhf"; // any-armhf
							ReplaceInFile(pathTemp + "/DEBIAN/control", "{@architecture}", debianArchitecture);

							RemoveFile(pathTemp + "/usr/lib/eddie-ui/openvpn");
							RemoveFile(pathTemp + "/usr/lib/eddie-ui/stunnel");
							RemoveFile(pathTemp + "/usr/lib/eddie-ui/libgdiplus.so.0");
							RemoveFile(pathTemp + "/usr/lib/eddie-ui/libMonoPosixHelper.so");
							RemoveFile(pathTemp + "/usr/lib/eddie-ui/libappindicator.so.1");

							Shell("chmod 755 -R \"" + pathTemp + "\"");

							CreateDirectory(pathTemp + "/usr/share/eddie-ui");
							CopyAll(pathCommon, pathTemp + "/usr/share/eddie-ui");

							WriteTextFile(pathTemp + "/usr/share/doc/eddie-ui/changelog.Debian", FetchUrl(Constants.ChangeLogUrl));
							Shell("gzip -n -9 \"" + pathTemp + "/usr/share/doc/eddie-ui/changelog.Debian\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/share/doc/eddie-ui/changelog.Debian.gz\"");

							WriteTextFile(pathTemp + "/usr/share/man/man8/eddie-ui.8", Shell("mono \"" + pathTemp + "/usr/lib/eddie-ui/Eddie-UI.exe\" -cli -help -help.format=man"));
							Shell("gzip -n -9 \"" + pathTemp + "/usr/share/man/man8/eddie-ui.8\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/share/man/man8/eddie-ui.8.gz\"");

							Shell("chmod 644 \"" + pathTemp + "/usr/lib/eddie-ui/Lib.Core.dll\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/lib/eddie-ui/Lib.Common.dll\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/lib/eddie-ui/Lib.Forms.dll\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/lib/eddie-ui/Lib.Platform.Linux.dll\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/lib/eddie-ui/libLib.Platform.Linux.Native.so\"");
							Shell("chmod 755 \"" + pathTemp + "/usr/lib/eddie-ui/eddie-tray\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/share/pixmaps/eddie-ui.png\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/share/applications/eddie-ui.desktop\"");
							Shell("chmod 644 \"" + pathTemp + "/usr/share/polkit-1/actions/org.airvpn.eddie.ui.policy\"");

							Shell("chmod 644 \"" + pathTemp + "/usr/share/doc/eddie-ui/copyright\"");
							Shell("chmod 644 " + pathTemp + "/usr/share/eddie-ui/*"); // Note: wildchar don't works if quoted

							Shell("chown -R root:root " + pathTemp);

							string command = "dpkg -b \"" + pathTemp + "\" \"" + pathFinal + "\"";
							Log(command);
							Shell(command);

							Log("Lintian report:");
							Log(Shell("lintian \"" + pathFinal + "\""));

							SignFile(platform, format, pathFinal);
						}
						else if (AvailableRPM)
						{
							if ((format == "opensuse") || (format == "fedora"))
							{
								string pathRelease = new DirectoryInfo(PathBase + "/src/App.Forms.Linux/bin/" + archCompile + "/Release").FullName;

								string libSubPath = "lib";
								if (arch == "x64")
									libSubPath = "lib64";

								string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".rpm");

								CreateDirectory(pathTemp + "/usr/" + libSubPath + "/eddie-ui");

								CopyFile(pathRelease, "Lib.Core.dll", pathTemp + "/usr/" + libSubPath + "/eddie-ui");
								CopyFile(pathRelease, "Lib.Common.dll", pathTemp + "/usr/" + libSubPath + "/eddie-ui");
								CopyFile(pathRelease, "Lib.Forms.dll", pathTemp + "/usr/" + libSubPath + "/eddie-ui");
								CopyFile(pathRelease, "Lib.Platform.Linux.dll", pathTemp + "/usr/" + libSubPath + "/eddie-ui");
								CopyFile(pathRelease, "App.Forms.Linux.exe", pathTemp + "/usr/" + libSubPath + "/eddie-ui", "Eddie-UI.exe");
								//CopyFile(new DirectoryInfo(PathBase + "/src/App.CLI.Linux/bin/" + archCompile + "/Release").FullName, "App.CLI.Linux.exe", pathTemp + "/usr/" + libSubPath + "/AirVPN", "Eddie-CLI.exe");

								CopyAll(pathDeploy, pathTemp + "/usr/" + libSubPath + "/eddie-ui");
								CopyDirectory(PathBaseResources + "/" + format, pathTemp);

								ReplaceInFile(pathTemp + "/eddie-ui.spec", "{@version}", versionString3);
								ReplaceInFile(pathTemp + "/eddie-ui.spec", "{@lib}", libSubPath);

								ReplaceInFile(pathTemp + "/usr/bin/eddie-ui", "{@lib}", libSubPath);

								RemoveFile(pathTemp + "/usr/" + libSubPath + "/eddie-ui/openvpn");
								RemoveFile(pathTemp + "/usr/" + libSubPath + "/eddie-ui/stunnel");
								RemoveFile(pathTemp + "/usr/" + libSubPath + "/eddie-ui/libgdiplus.so.0");
								RemoveFile(pathTemp + "/usr/" + libSubPath + "/eddie-ui/libMonoPosixHelper.so");
								RemoveFile(pathTemp + "/usr/" + libSubPath + "/eddie-ui/libappindicator.so.1");

								CreateDirectory(pathTemp + "/usr/share/eddie-ui");
								CopyAll(pathCommon, pathTemp + "/usr/share/eddie-ui");

								WriteTextFile(pathTemp + "/usr/share/man/man8/eddie-ui.8", Shell("mono \"" + pathTemp + "/usr/" + libSubPath + "/eddie-ui/Eddie-UI.exe\" -cli -help -help.format=man"));
								Shell("gzip -n -9 \"" + pathTemp + "/usr/share/man/man8/eddie-ui.8\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/share/man/man8/eddie-ui.8.gz\"");

								Shell("chmod 755 -R \"" + pathTemp + "\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/" + libSubPath + "/eddie-ui/Lib.Core.dll\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/" + libSubPath + "/eddie-ui/Lib.Common.dll\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/" + libSubPath + "/eddie-ui/Lib.Forms.dll\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/" + libSubPath + "/eddie-ui/Lib.Platform.Linux.dll\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/" + libSubPath + "/eddie-ui/libLib.Platform.Linux.Native.so\"");
								Shell("chmod 755 \"" + pathTemp + "/usr/" + libSubPath + "/eddie-ui/eddie-tray\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/share/pixmaps/eddie-ui.png\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/share/applications/eddie-ui.desktop\"");
								Shell("chmod 644 \"" + pathTemp + "/usr/share/polkit-1/actions/org.airvpn.eddie.ui.policy\"");
								Shell("chmod 644 " + pathTemp + "/usr/share/eddie-ui/*"); // Note: wildchar don't works if quoted

								Shell("chown -R root:root " + pathTemp);

								string command = "rpmbuild";
								if (IsOfficial())
								{
									string pathPassphrase = NormalizePath(PathBaseSigning + "/gpg.passphrase");
									if (File.Exists(pathPassphrase))
									{
										command += " -sign";

										// I don't yet find a working method to automate it.
										//string passphrase = File.ReadAllText(pathPassphrase);
										//command = "echo " + passphrase + " | setsid " + command;

										Log("Enter AirVPN Staff signing password for RPM build");
									}
									else
									{
										LogError("Missing passphrase file for automatic build. (" + pathPassphrase + ")");
									}
								}
								command += " -bb \"" + pathTemp + "/eddie-ui.spec\" --buildroot \"" + pathTemp + "\"";

								Log("RPM Build");
								string output = Shell(command);
								if (IsOfficial())
								{
									if (output.Contains("signing failed"))
									{
										LogError("RPM fail: " + output);
									}
									else
										Log(output);
								}

								Shell("mv ../*.rpm " + pathFinal);
							}
						}
						else if ((format == "aur") && (AvailableAUR))
						{

						}
					}
					else if (platform == "macos")
					{
						if (format == "portable")
						{
							if (ui == "cli")
							{
								CopyAll(pathDeploy, pathTemp);
								CopyAll(pathCommon, pathTemp + "/res");

								string pathRelease = new DirectoryInfo(PathBase + "/src/App.CLI.MacOS/bin/" + archCompile + "/Release").FullName;
								pathRelease = pathRelease.Replace("/x64/Release", "/Release");

								//CopyFile(pathRelease, "eddie-cli", pathTemp, "eddie-cli");

								string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".tar.gz");

								if (File.Exists(pathFinal))
									File.Delete(pathFinal);

								{
									// Tested with Xamarin Studio 6.1.2 build 44, Mono 4.6.2, macOS Sierra 10.12.1

									string cmd = "";

									// Ensure it can find pkg-config:
									cmd += "export PKG_CONFIG_PATH=$PKG_CONFIG_PATH:/usr/lib/pkgconfig:/Library/Frameworks/Mono.framework/Versions/Current/lib/pkgconfig;";

									// Force 32bit build and manually set some clang linker properties:
									cmd += "export AS=\"as -arch i386\";";
									cmd += "export CC=\"cc -arch i386 -lobjc -liconv -framework CoreFoundation -I /Library/Frameworks/Mono.framework/Versions/Current/include/mono-2.0/\";";

									// Force 64bit build and manually set some clang linker properties:
									// export AS="as -arch x86_64"
									// export CC="cc -arch x86_64 -lobjc -liconv -framework CoreFoundation -I /Library/Frameworks/Mono.framework/Versions/Current/include/mono-2.0/"

									// Other ensure it can find pig-config
									cmd += "export PATH=/Library/Frameworks/Mono.framework/Versions/Current/bin/:$PATH;";

									// WARNING: Currently 2017-03-10 , cannot be signed for this bug: https://bugzilla.xamarin.com/show_bug.cgi?id=52443
									cmd += "mkbundle";
									cmd += " --sdk /Library/Frameworks/Mono.framework/Versions/Current";
									cmd += " \"" + pathRelease + "/App.CLI.macOS.exe\"";
									cmd += " \"" + pathRelease + "/Lib.Common.dll\"";
									cmd += " \"" + pathRelease + "/Lib.Core.dll\"";
									cmd += " \"" + pathRelease + "/Lib.Platform.macOS.dll\"";
									cmd += " \"" + pathRelease + "/Xamarin.Mac.dll\"";
									cmd += " -z";
									cmd += " --static";
									cmd += " --deps";
									cmd += " -o \"" + pathTemp + "/eddie-cli\"";

									Shell(cmd);
								}

								CopyFile(pathRelease, "libxammac.dylib", pathTemp);

								Shell("chmod 755 \"" + pathTemp + "/eddie-cli\"");
								Shell("chmod 755 \"" + pathTemp + "/openvpn\"");
								Shell("chmod 755 \"" + pathTemp + "/stunnel\"");
								Shell("chmod 644 \"" + pathTemp + "/libLib.Platform.macOS.Native.dylib\"");
								Shell("chmod 755 \"" + pathTemp + "/libxammac.dylib\"");

								SignFile(platform, format, pathTemp + "/eddie-cli"); // WARNING: Currently 2017-03-10 , signing don't work for this bug: https://bugzilla.xamarin.com/show_bug.cgi?id=52443
								SignFile(platform, format, pathTemp + "/openvpn");
								SignFile(platform, format, pathTemp + "/stunnel");
								SignFile(platform, format, pathTemp + "/libLib.Platform.macOS.Native.dylib");
								SignFile(platform, format, pathTemp + "/libxammac.dylib");

								CreateDirectory(pathTemp + "/" + fileName);
								MoveAll(pathTemp, pathTemp + "/" + fileName);

								// TAR.GZ
								string command2 = "cd \"" + pathTemp + "\" && tar cvfz \"" + pathFinal + "\" " + "*";
								Shell(command2);
							}
							else if (ui == "ui")
							{
								string pathRelease = new DirectoryInfo(PathBase + "/src/App.Cocoa.MacOS/bin/" + archCompile + "/Release").FullName;
								pathRelease = pathRelease.Replace("/x64/Release", "/Release");

								CreateDirectory(pathTemp + "/Eddie.app");
								CopyDirectory(pathRelease + "/Eddie.app", pathTemp + "/Eddie.app");

								// TAR.GZ
								string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".tar.gz");

								if (File.Exists(pathFinal))
									File.Delete(pathFinal);

								CopyFile(PathBaseResources + "/macos/Info.plist", pathTemp + "/Eddie.app/Contents/Info.plist");
								ReplaceInFile(pathTemp + "/Eddie.app/Contents/Info.plist", "{@version}", versionString3);

								CopyAll(pathDeploy, pathTemp + "/Eddie.app/Contents/MacOS");
								CopyAll(pathCommon, pathTemp + "/Eddie.app/Contents/Resources");

								SignFile(platform, format, pathTemp + "/Eddie.app/Contents/MacOS/Eddie");
								SignFile(platform, format, pathTemp + "/Eddie.app/Contents/MacOS/openvpn");
								SignFile(platform, format, pathTemp + "/Eddie.app/Contents/MacOS/stunnel");
								SignFile(platform, format, pathTemp + "/Eddie.app/Contents/MonoBundle/libLib.Platform.macOS.Native.dylib");
								SignFile(platform, format, pathTemp + "/Eddie.app");

								string command2 = "cd \"" + pathTemp + "\" && tar cvfz \"" + pathFinal + "\" " + " Eddie.app";
								Shell(command2);
							}

						}
						else if (format == "installer")
						{
							if (ui == "ui")
							{
								string pathRelease = new DirectoryInfo(PathBase + "/src/App.Cocoa.MacOS/bin/" + archCompile + "/Release").FullName;
								pathRelease = pathRelease.Replace("/x64/Release", "/Release");

								CreateDirectory(pathTemp + "/Applications/Eddie.app");
								CopyDirectory(pathRelease + "/Eddie.app", pathTemp + "/Applications/Eddie.app");

								// TAR.GZ
								string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".pkg");

								if (File.Exists(pathFinal))
									File.Delete(pathFinal);

								CopyFile(PathBaseResources + "/macos/Info.plist", pathTemp + "/Applications/Eddie.app/Contents/Info.plist");
								ReplaceInFile(pathTemp + "/Applications/Eddie.app/Contents/Info.plist", "{@version}", versionString3);

								CopyAll(pathDeploy, pathTemp + "/Applications/Eddie.app/Contents/MacOS");
								CopyAll(pathCommon, pathTemp + "/Applications/Eddie.app/Contents/Resources");

								SignFile(platform, format, pathTemp + "/Applications/Eddie.app/Contents/MacOS/Eddie");
								SignFile(platform, format, pathTemp + "/Applications/Eddie.app/Contents/MacOS/openvpn");
								SignFile(platform, format, pathTemp + "/Applications/Eddie.app/Contents/MacOS/stunnel");
								SignFile(platform, format, pathTemp + "/Applications/Eddie.app/Contents/MonoBundle/libLib.Platform.macOS.Native.dylib");
								SignFile(platform, format, pathTemp + "/Applications/Eddie.app");

								string command2 = "pkgbuild";
								command2 += " --identifier org.airvpn.eddie.ui";
								command2 += " --version " + versionString3;
								command2 += " --root \"" + pathTemp + "\"";
								//command2 += " --component \"" + pathRelease + "Eddie.app\"";

								string pathSignString = NormalizePath(PathBaseSigning + "/apple-dev-id.txt");
								if (File.Exists(pathSignString))
								{
									string appleSign = ReadTextFile(pathSignString).Trim();
									command2 += " --sign \"" + appleSign + "\"";
									command2 += " --timestamp";
								}
								command2 += " \"" + pathFinal + "\"";
								Log("pkgbuild command: " + command2);
								Log(Shell(command2));

								//SignFile(platform, format, pathFinal);
							}
						}
						else if (format == "disk")
						{
							if (ui == "ui")
							{
								string pathRelease = new DirectoryInfo(PathBase + "/src/App.Cocoa.MacOS/bin/" + archCompile + "/Release").FullName;
								pathRelease = pathRelease.Replace("/x64/Release", "/Release");

								Log("Extract DMG base");
								Shell("tar -jxvf " + "\"" + PathBaseResources + "/macos/diskbase.dmg.tar.bz2\" -C \"" + pathTemp + "\"");
								Log("Resize DMG");
								Shell("hdiutil resize -size 200m '" + pathTemp + "/diskbase.dmg" + "'");
								Log("Attach DMG");
								Shell("hdiutil attach '" + pathTemp + "/diskbase.dmg" + "' -mountpoint '" + pathTemp + "/DiskBuild'");
								Log("Fill DMG");

								CreateDirectory(pathTemp + "/DiskBuild/Eddie.app");
								CopyDirectory(pathRelease + "/Eddie.app", pathTemp + "/DiskBuild/Eddie.app");

								// TAR.GZ
								string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".dmg");

								if (File.Exists(pathFinal))
									File.Delete(pathFinal);

								CopyFile(PathBaseResources + "/macos/Info.plist", pathTemp + "/DiskBuild/Eddie.app/Contents/Info.plist");
								ReplaceInFile(pathTemp + "/DiskBuild/Eddie.app/Contents/Info.plist", "{@version}", versionString3);

								CopyAll(pathDeploy, pathTemp + "/DiskBuild/Eddie.app/Contents/MacOS");
								CopyAll(pathCommon, pathTemp + "/DiskBuild/Eddie.app/Contents/Resources");

								SignFile(platform, format, pathTemp + "/DiskBuild/Eddie.app/Contents/MacOS/Eddie");
								SignFile(platform, format, pathTemp + "/DiskBuild/Eddie.app/Contents/MacOS/openvpn");
								SignFile(platform, format, pathTemp + "/DiskBuild/Eddie.app/Contents/MacOS/stunnel");
								SignFile(platform, format, pathTemp + "/DiskBuild/Eddie.app/Contents/MonoBundle/libLib.Platform.macOS.Native.dylib");
								SignFile(platform, format, pathTemp + "/DiskBuild/Eddie.app");

								Log("Detach DMG");
								Shell("hdiutil detach \"" + pathTemp + "/DiskBuild" + "\"");
								Log("Compress DMG");
								Shell("hdiutil convert \"" + pathTemp + "/diskbase.dmg" + "\" -format UDCO -imagekey zlib-level=9 -o \"" + pathFinal + "\"");

							}
						}
						else if (format == "mono")
						{
							if (ui == "cli")
							{
								CopyAll(pathDeploy, pathTemp);
								CopyAll(pathCommon, pathTemp + "/res");

								string pathRelease = new DirectoryInfo(PathBase + "/src/App.CLI.MacOS/bin/" + archCompile + "/Release").FullName;
								pathRelease = pathRelease.Replace("/x64/Release", "/Release");

								CopyFile(pathRelease, "Lib.Core.dll", pathTemp);
								CopyFile(pathRelease, "Lib.Common.dll", pathTemp);
								CopyFile(pathRelease, "Lib.Platform.macOS.dll", pathTemp);
								//CopyFile(pathRelease, "Newtonsoft.Json.dll", pathTemp);
								CopyFile(pathRelease, "Xamarin.Mac.dll", pathTemp);
								CopyFile(pathRelease, "libxammac.dylib", pathTemp);
								CopyFile(pathRelease, "App.CLI.macOS.exe", pathTemp, "Eddie-CLI.exe");

								string pathFinal = NormalizePath(PathBaseRepository + "/" + fileName + ".tar.gz");

								if (File.Exists(pathFinal))
									File.Delete(pathFinal);

								Shell("chmod 755 \"" + pathTemp + "/openvpn\"");
								Shell("chmod 755 \"" + pathTemp + "/stunnel\"");
								Shell("chmod 644 \"" + pathTemp + "/libLib.Platform.macOS.Native.dylib\"");
								Shell("chmod 644 \"" + pathTemp + "/libxammac.dylib\"");

								SignFile(platform, format, pathTemp + "/openvpn");
								SignFile(platform, format, pathTemp + "/stunnel");
								SignFile(platform, format, pathTemp + "/libLib.Platform.macOS.Native.dylib");
								SignFile(platform, format, pathTemp + "/libxammac.dylib");

								RemoveFile(pathTemp + "/libgdiplus.so.0");
								RemoveFile(pathTemp + "/libMonoPosixHelper.so");

								CreateDirectory(pathTemp + "/" + fileName);
								MoveAll(pathTemp, pathTemp + "/" + fileName);

								// TAR.GZ
								string command2 = "cd \"" + pathTemp + "\" && tar cvfz \"" + pathFinal + "\" " + "*";
								Shell(command2);
							}
						}
					}

				}

				/* -------------------------------
					Generate Man pages
				------------------------------- */

				if (SO == "windows")
				{
					Log("Generating manual files");
					string pathExe = new FileInfo(PathBase + "/src/bin/x64/Release/App.CLI.Windows.exe").FullName;
					WriteTextFile(PathBaseRepository + "/manual.html", Shell(pathExe + " -help -help.format=html"));
					WriteTextFile(PathBaseRepository + "/manual.bb", Shell(pathExe + " -help -help.format=bbc"));
					WriteTextFile(PathBaseRepository + "/manual.txt", Shell(pathExe + " -help -help.format=text"));
					WriteTextFile(PathBaseRepository + "/manual.man", Shell(pathExe + " -help -help.format=man"));
				}
			}

			Log("------------------------------");
			if (Errors == 0)
				Log("Done");
			else
				Log("WARNING: Done with " + Errors.ToString() + " errors.");

			if (SO == "linux")
			{
				if (IsOfficial())
				{
					Console.WriteLine("If running from a developing VM, maybe need:");
					Console.WriteLine("cp files/eddie* /media/sf_eddie-dev/repository/files/");
				}
			}

			if (SO == "windows")
				Pause();
		}

		static bool IsEddie3()
		{
			return Engine.Arguments.Contains("eddie3");
		}

		static bool IsVerbose()
		{
			return Engine.Arguments.Contains("verbose");
		}

		static bool IsSigning()
		{
			return Engine.Arguments.Contains("signing");
		}

		static bool IsOfficial()
		{
			return ((Engine.Arguments.Contains("official")) || (IsSigning()));
		}

		static bool Compile(string so, string architecture, string netFramework)
		{
			Log("Compilation, Architecture: " + architecture + ", NetFramework: " + netFramework);

			string pathCompiler = "";
			if (Environment.OSVersion.VersionString.IndexOf("Windows") != -1)
			{
				pathCompiler = "c:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\msbuild.exe";
			}
			else
			{
				pathCompiler = "/usr/bin/xbuild";
			}

			if (File.Exists(pathCompiler) == false)
			{
				LogError("Compiler expected in " + pathCompiler + " but not found, build skipped.");
				return false;
			}

			string pathProject = "";
			if (so == "windows")
				pathProject = PathBase + "/src/eddie2.windows.sln";
			else if (so == "linux")
				pathProject = PathBase + "/src/eddie2.linux.sln";

			string arguments = " /property:CodeAnalysisRuleSet=\"" + PathBase + "/tools/ruleset/norules.ruleset\"  /p:Configuration=Release /p:Platform=" + architecture + " /p:TargetFrameworkVersion=\"v" + netFramework + "\" /t:Rebuild \"" + pathProject + "\"";

			if (Environment.OSVersion.VersionString.IndexOf("Windows") != -1)
			{
				if (netFramework.StartsWith("4"))
				{
					arguments += " /p:DefineConstants=\"EDDIENET4\"";
					arguments += " /p:DefineConstants=\"EDDIENET3\"";
					arguments += " /p:DefineConstants=\"EDDIENET2\"";
				}

				if (netFramework.StartsWith("3"))
				{
					arguments += " /p:DefineConstants=\"EDDIENET3\"";
					arguments += " /p:DefineConstants=\"EDDIENET2\"";
				}

				if (netFramework.StartsWith("2"))
					arguments += " /p:DefineConstants=\"EDDIENET2\"";
			}

			string o = Shell(pathCompiler, arguments);

			if (o.IndexOf(" 0 Error(s)", StringComparison.InvariantCulture) != -1)
			{
				return true;
			}
			else
			{
				LogError("Compilation errors, build skipped. Dump compilation report.");
				Log(o);
				return false;
			}
		}

		static string NormalizePath(string path)
		{
			if (SO == "windows")
			{
				return path.Replace("/", "\\");
			}
			else
				return path.Replace("\\", "/");
		}

		static string FetchUrl(string url)
		{
			// Note: Actually used only under Linux
			return Shell("curl \"" + url + "\"").Trim();

			// This version works under Windows, but not under Linux/Mono due RC4 cipher deprecated on airvpn.org
			//WebClient w = new WebClient();
			//w.Proxy = null;
			//return w.DownloadString(url);
		}

		static void CreateDirectory(string path)
		{
			CreateDirectory(path, true);
		}

		static void CreateDirectory(string path, bool cleanIfExists)
		{
			if (Directory.Exists(path))
			{
				if (cleanIfExists)
				{
					Directory.Delete(NormalizePath(path), true);
					Directory.CreateDirectory(NormalizePath(path));
				}
			}
			else
				Directory.CreateDirectory(NormalizePath(path));
		}

		static void CreateSymlink(string path, string origin)
		{
			Shell("ln -s \"" + origin + "\" \"" + path + "\"");
		}

		static void Fatal(string message)
		{
			Log("Fatal error: " + message);
			throw new Exception(message);
		}

		static void NotImplemented()
		{
			Log("Not yet implemented.");
		}

		static string ShellPlatformIndipendent(string FileName, string Arguments, string WorkingDirectory, bool WaitEnd, bool ShowWindow)
		{
			try
			{
				Process p = new Process();

				p.StartInfo.Arguments = Arguments;

				if (WorkingDirectory != "")
					p.StartInfo.WorkingDirectory = WorkingDirectory;

				p.StartInfo.FileName = FileName;

				if (ShowWindow == false)
				{
					//#do not show DOS window
					p.StartInfo.CreateNoWindow = true;
					p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				}

				if (WaitEnd)
				{
					p.StartInfo.UseShellExecute = false;
					p.StartInfo.RedirectStandardOutput = true;
					p.StartInfo.RedirectStandardError = true;
				}

				p.Start();

				if (WaitEnd)
				{
					string Output = p.StandardOutput.ReadToEnd() + "\n" + p.StandardError.ReadToEnd();
					p.WaitForExit();
					return Output.Trim();
				}
				else
				{
					return "";
				}
			}
			catch (Exception E)
			{
				return E.Message;
			}


		}

		/*
		static string Shell(string command)
		{
			return Shell(command, true);
		}
		*/

		static string Shell(string command)
		{
			if (IsVerbose())
				Console.WriteLine("Shell: " + command);

			if (SO == "windows")
				return Shell("cmd.exe", String.Format("/c {0}", command));
			else
				return Shell("sh", String.Format("-c '{0}'", command));
		}

		static string Shell(string filename, string arguments)
		{
			if (IsVerbose())
				Console.WriteLine("Shell, filename: " + filename + ", arguments: " + arguments);
			string output = ShellPlatformIndipendent(filename, arguments, "", true, false);
			if ((IsVerbose()) && (output.Trim() != ""))
				Console.WriteLine("Output: " + output);
			return output;
		}

		static void SignPath(string platform, string format, string path)
		{
			if (IsOfficial() == false)
				return;

			Log("Signing path: " + path);

			string[] files = Directory.GetFiles(path);
			foreach (string file in files)
			{
				bool skip = false;

				if (file.EndsWith("tap-windows.exe", StringComparison.InvariantCulture)) // Already signed by OpenVPN Technologies
					skip = true;

				if (skip == false)
					SignFile(platform, format, file);
			}
		}

		static void SignFile(string platform, string format, string path)
		{
			if (IsOfficial() == false)
				return;

			if (platform == "macos")
			{
				string pathSignString = NormalizePath(PathBaseSigning + "/apple-dev-id.txt");
				if (File.Exists(pathSignString))
				{
					string appleSign = File.ReadAllText(pathSignString).Trim();
					string cmd = "codesign -d --deep -v --force --sign \"" + appleSign + "\" \"" + path + "\"";

					string output = Shell(cmd);
					Log("macOS Signing file: " + output);
				}
				else
					Log("Missing Apple Developer ID for macOS signatures. (" + pathSignString + ")");
			}
			else if (platform.StartsWith("windows", StringComparison.InvariantCulture))
			{
				string pathPfx = NormalizePath(PathBaseSigning + "/eddie.pfx");
				string pathPfxPwd = NormalizePath(PathBaseSigning + "/eddie.pfx.pwd");

				string title = "Eddie - OpenVPN UI";

				if ((File.Exists(pathPfx)) && (File.Exists(pathPfxPwd)))
				{
					for (int t = 0; ; t++)
					{
						{
							string cmd = "";
							cmd += PathBaseTools + "/windows/signtool.exe";
							cmd += " verify";
							cmd += " /pa";
							cmd += " \"" + path + "\""; // File
							string output = Shell(cmd);

							bool valid = (output.IndexOf("Successfully verified: " + path) != -1);
							if (valid)
								break;
						}

						{
							string cmd = "";
							cmd += PathBaseTools + "/windows/signtool.exe";
							cmd += " sign";
							cmd += " /p " + File.ReadAllText(pathPfxPwd); // Password
							cmd += " /f " + pathPfx; // Pfx
							cmd += " /t " + Constants.WindowsSigningTimestampUrl; // Timestamp
							cmd += " /d \"" + title + "\""; // Title
							cmd += " \"" + path + "\""; // File
							string output = Shell(cmd);

							Log("Windows Signing file: \"" + path + "\": " + output);

							if (output.Contains("Error"))
							{
								Log("Failed to sign file with windows, try " + t.ToString() + ". Retry.");
							}
							else
								break;
						}
					}
				}
				else
				{
					LogError("Missing PFX or password for Windows signatures. (" + pathPfx + " , " + pathPfxPwd + ")");
				}
			}
			else if (platform == "linux")
			{
				if (format == "debian")
				{
					string pathPassphrase = NormalizePath(PathBaseSigning + "/gpg.passphrase");
					if (File.Exists(pathPassphrase))
					{
						string passphrase = File.ReadAllText(pathPassphrase);
						Log("Signing .deb file (keys need to be already configured)");
						string cmd = "dpkg-sig -g \"--no-tty --passphrase " + passphrase + "\" --sign builder " + path;
						string output = Shell(cmd);
						if (output.Contains("Signed deb ") == false)
						{
							LogError("Signing .deb failed: " + output);
						}
						else
							Log(output);
					}
					else
					{
						LogError("Missing passphrase file for automatic build. (" + pathPassphrase + ")");
					}
				}
			}
		}

		static void MoveFile(string fromFilePath, string toFilePath)
		{
			fromFilePath = NormalizePath(fromFilePath);
			toFilePath = NormalizePath(toFilePath);
			if (IsVerbose())
				Log("Move file from '" + fromFilePath + "' to '" + toFilePath + "'");
			if (File.Exists(fromFilePath) == false)
				throw new Exception("MoveFile failed, source don't exists: " + fromFilePath);
			if (File.Exists(toFilePath))
				File.Delete(toFilePath);
			File.Move(fromFilePath, toFilePath);
		}

		static void CopyFile(string fromFilePath, string toFilePath)
		{
			fromFilePath = NormalizePath(fromFilePath);
			toFilePath = NormalizePath(toFilePath);
			if (IsVerbose())
				Log("Copy file from '" + fromFilePath + "' to '" + toFilePath + "'");
			if (File.Exists(toFilePath))
				File.Delete(toFilePath);

			CreateDirectory(new FileInfo(toFilePath).Directory.FullName, false);

			File.Copy(fromFilePath, toFilePath, false);
		}

		static void CopyFile(string fromPath, string fromFile, string toPath)
		{
			CopyFile(fromPath + "/" + fromFile, toPath + "/" + fromFile);
		}

		static void CopyFile(string fromPath, string fromFile, string toPath, string toFile)
		{
			CopyFile(fromPath + "/" + fromFile, toPath + "/" + toFile);
		}

		static void RemoveFile(string path)
		{
			if (IsVerbose())
				Log("Remove file '" + path + "'");
			File.Delete(path);
		}

		static void CopyAll(string from, string to)
		{
			string[] files = Directory.GetFiles(from);

			foreach (string file in files)
			{
				FileInfo fi = new FileInfo(file);

				CopyFile(fi.FullName, to + "/" + fi.Name);
			}

			string[] dirs = Directory.GetDirectories(from);

			foreach (string dir in dirs)
			{
				DirectoryInfo di = new DirectoryInfo(dir);

				// Hack Eddie2
				if (di.Name == "providers")
					continue;
				if (di.Name == "ui")
					continue;
				if (di.Name == "webui")
					continue;

				Directory.CreateDirectory(to + "/" + di.Name);

				CopyAll(di.FullName, to + "/" + di.Name);
			}
		}

		static void MoveAll(string from, string to)
		{
			string[] files = Directory.GetFiles(from);

			foreach (string file in files)
			{
				FileInfo fi = new FileInfo(file);

				MoveFile(fi.FullName, to + "/" + fi.Name);
			}

			string[] dirs = Directory.GetDirectories(from);

			foreach (string dir in dirs)
			{
				DirectoryInfo di = new DirectoryInfo(dir);

				if (di.FullName != to)
					Directory.Move(di.FullName, to + "/" + di.Name);
			}
		}

		static string ReadTextFile(string path)
		{
			if (IsVerbose())
				Log("Read text in '" + path + "'");
			return File.ReadAllText(path);
		}

		static void WriteTextFile(string path, string contents)
		{
			if (IsVerbose())
				Log("Write text in '" + path + "'");
			string dir = Path.GetDirectoryName(path);
			if (Directory.Exists(dir) == false)
				Directory.CreateDirectory(dir);
			File.WriteAllText(path, contents);
		}

		static void ReplaceInFile(string path, string from, string to)
		{
			if (IsVerbose())
				Log("Replace text in '" + path + "'");
			File.WriteAllText(path, File.ReadAllText(path).Replace(from, to));
		}

		static void CopyDirectory(string fromPath, string toPath)
		{
			if (IsVerbose())
				Log("Copy directory from '" + fromPath + "' to '" + toPath + "'");

			//Now Create all of the directories
			foreach (string dirPath in Directory.GetDirectories(fromPath, "*", SearchOption.AllDirectories))
				Directory.CreateDirectory(dirPath.Replace(fromPath, toPath));

			//Copy all the files & Replaces any files with the same name
			foreach (string newPath in Directory.GetFiles(fromPath, "*.*", SearchOption.AllDirectories))
			{
				string fileFromPath = newPath;
				string fileToPath = newPath.Replace(fromPath, toPath);
				File.Copy(fileFromPath, fileToPath, true);
			}
		}

		public static List<string> GetFilesRecursive(string path)
		{
			List<string> result = new List<string>();

			foreach (string filePath in Directory.GetFiles(path))
			{
				result.Add(filePath);
			}

			foreach (string dirPath in Directory.GetDirectories(path))
			{
				result.AddRange(GetFilesRecursive(dirPath));
			}

			return result;
		}

		public static string ExtractBetween(string str, string from, string to)
		{
			int iPos1 = str.IndexOf(from);
			if (iPos1 != -1)
			{
				int iPos2 = str.IndexOf(to, iPos1 + from.Length);
				if (iPos2 != -1)
				{
					return str.Substring(iPos1 + from.Length, iPos2 - iPos1 - from.Length);
				}
			}

			return "";
		}

		public static string StringRemoveLineWith(string str, string find)
		{
			string final = "";
			foreach (string line in str.Split('\n'))
			{
				if (line.IndexOf(find) == -1)
					final += line + "\n";
			}
			return final;
		}

		static void LogError(string message)
		{
			Log("Error: " + message);
			Errors++;
		}

		static void Log(string message)
		{
			Console.WriteLine(message);
			File.AppendAllText("build.log", message + "\r\n");
		}

		static void Pause()
		{
			Pause("Press any key to continue.");
		}

		static void Pause(string message)
		{
			Log(message);
			Console.ReadKey();
		}
	}
}
