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
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System.Xml;
using Eddie.Common;

namespace Eddie.Core
{
	public class WebServer
	{
		private HttpListener m_listener = new HttpListener();
		//private Func<HttpListenerRequest, string> m_responderMethod;

		private List<Json> m_pullItems = new List<Json>();

		public static string GetPath()
		{
			string pathRoot = Platform.Instance.NormalizePath(Engine.Instance.LocateResource("webui"));
			if (Platform.Instance.DirectoryExists(pathRoot))
				return pathRoot;
			else
				return "";
		}

		public void Init(string prefix)
		{
			if (GetPath() == "")
				return;

			if (!HttpListener.IsSupported)
				throw new NotSupportedException(
					"Needs Windows XP SP2, Server 2003 or later.");

			// Engine.Instance.CommandEvent += OnCommandEvent;

			m_listener.Prefixes.Add(prefix);

			m_listener.Start();
		}

		public void Run()
		{
			ThreadPool.QueueUserWorkItem((o) =>
			{
				try
				{
					while (m_listener.IsListening)
					{
						ThreadPool.QueueUserWorkItem((c) =>
						{
							var ctx = c as HttpListenerContext;
							try
							{
								string urlPath = ctx.Request.Url.LocalPath;
								if (urlPath == "/")
									urlPath = "/index.html";
								string localPath = GetPath() + urlPath;
								if (Platform.Instance.FileExists(localPath))
								{
									WriteFile(ctx, localPath, false);
								}
								else
								{
									string rstr = SendResponse(ctx.Request);
									byte[] buf = Encoding.UTF8.GetBytes(rstr);
									ctx.Response.ContentLength64 = buf.Length;
									ctx.Response.OutputStream.Write(buf, 0, buf.Length);
								}
							}
							catch { } // suppress any exceptions
							finally
							{
								// always close the stream
								ctx.Response.OutputStream.Close();
							}
						}, m_listener.GetContext());
					}
				}
				catch { } // suppress any exceptions
			});
		}

		void WriteFile(HttpListenerContext ctx, string path, bool asDownload)
		{
			var response = ctx.Response;
			using (FileStream fs = File.OpenRead(path))
			{
				string filename = Path.GetFileName(path);
				//response is HttpListenerContext.Response...
				response.ContentLength64 = fs.Length;
				response.SendChunked = false;
				if (asDownload)
				{
					response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
					response.AddHeader("Content-disposition", "attachment; filename=" + filename);
				}
				else
				{
					if (path.EndsWith(".html"))
					{
						response.ContentType = "text/html";
						response.ContentEncoding = Encoding.UTF8;
					}
					else if (path.EndsWith(".css"))
					{
						response.ContentType = "text/css";
						response.ContentEncoding = Encoding.UTF8;
					}
					else if (path.EndsWith(".js"))
					{
						response.ContentType = "text/javascript";
						response.ContentEncoding = Encoding.UTF8;
					}
					else if (path.EndsWith(".png"))
					{
						response.ContentType = "image/png";
					}
					else if (path.EndsWith(".gif"))
					{
						response.ContentType = "image/gif";
					}
					else if (path.EndsWith(".ico"))
					{
						response.ContentType = "image/x-icon";
					}
				}

				byte[] buffer = new byte[64 * 1024];
				int read;
				using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
				{
					while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
					{
						bw.Write(buffer, 0, read);
						bw.Flush(); //seems to have no effect
					}

					bw.Close();
				}

				response.StatusCode = (int)HttpStatusCode.OK;
				response.StatusDescription = "OK";
				response.OutputStream.Close();
			}
		}

		public void Stop()
		{
			m_listener.Stop();
			m_listener.Close();
		}

		public void Start()
		{
			string listenUrl = "http://" + Engine.Instance.Storage.Get("webui.ip") + ":" + Engine.Instance.Storage.Get("webui.port") + "/";
			//Init(listenUrl, SendResponse);
			Init(listenUrl);
			Run();
		}

		public string SendResponse(HttpListenerRequest request)
		{
			// string physicalPath = GetPath() + request.RawUrl;

			if (request.Url.AbsolutePath == "/api/command/")
			{
				// Pull mode
				var data = new StreamReader(request.InputStream).ReadToEnd();
				Json ret = Receive(data);
				if (ret != null)
					return ret.ToJson();
				else
					return "";
			}
			else if (request.Url.AbsolutePath == "/pull/receive/")
			{
				lock (m_pullItems)
				{
					if (m_pullItems.Count == 0)
						return "";

					Json data = m_pullItems[0];
					m_pullItems.RemoveAt(0);
					return data.ToJson();
				}
			}

			return string.Format("<HTML><BODY>Unexpected. {0}</BODY></HTML>", DateTime.Now);
		}

		private void OnCommandEvent(Json data)
		{
			Send(data);
		}

		public static XmlElement CreateMessage()
		{
			XmlDocument doc = new XmlDocument();
			XmlElement nodeRoot = doc.CreateElement("message");
			doc.AppendChild(nodeRoot);
			return nodeRoot;
		}

		public Json Receive(string data)
		{
			return null;
			//return Engine.Instance.Command(data, true);
		}

		// Clodo, TOCLEAN; still used?
		public delegate void SendEventHandler(Json data);
		public event SendEventHandler SendEvent;

		public void Send(Json data)
		{
			if (SendEvent != null)
				SendEvent(data);

			lock (m_pullItems)
			{
				m_pullItems.Add(data); // Clodo TOFIX memory huge
			}
		}
	}
}