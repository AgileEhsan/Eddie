﻿// <eddie_source_header>
// This file is part of Eddie/AirVPN software.
// Copyright (C)2014-2018 AirVPN (support@airvpn.org) / https://airvpn.org
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
//
// 20 June 2018 - author: promind - initial release. (a tribute to the 1859 Perugia uprising occurred on 20 June 1859 and in memory of those brave inhabitants who fought for the liberty of Perugia)

using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Android.Support.V4.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Net;
using System.Collections.Generic;
using Android.Runtime;

namespace Eddie.NativeAndroidApp
{
    [Activity (Label = "Eddie - OpenVPN GUI", Icon = "@drawable/icon", Theme="@style/AppTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity, INetworkStatusReceiverListener
	{
        private const int ACTIVITY_RESULT_FILE_CHOOSER = 1000;
        private const int ACTIVITY_RESULT_SETTINGS = 1001;

        private SupportTools supportTools = null;
        private SettingsManager settingsManager = new SettingsManager();

		private VPNManager vpnManager = null;
        private VPN.Status currentConnectionStatus = VPN.Status.NOT_CONNECTED;
        private Toolbar toolbar = null;
        private DrawerLayout drawer = null;
        private ActionBarDrawerToggle drawerToggle = null;
        private NavigationView navigationView = null;
        private Button btnSelectProfile = null, btnConnectProfile = null, btnDisconnectProfile = null;
        private TextView txtProfileFileName = null, txtServerName = null, txtServerPort = null, txtServerProtocol = null;
        private TextView txtVpnStatus = null, txtNetworkStatus = null, txtConnectionError = null;
        private LinearLayout llServerInfo = null, llConnectionError = null;
        private Uri profileUri = null;
        private string profileData = "";
        private Dictionary<string, string> profileInfo = null;
        private NetworkStatusReceiver networkStatusReceiver = null;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

            SetContentView(Resource.Layout.main_activity_layout);

            supportTools = new SupportTools(this);

            EddieLogger.Init(this);

            networkStatusReceiver = new NetworkStatusReceiver();
            networkStatusReceiver.AddListener(this);
            this.RegisterReceiver(networkStatusReceiver, new IntentFilter(Android.Net.ConnectivityManager.ConnectivityAction));

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerToggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += OnNavigationViewItemSelected;

            btnSelectProfile = FindViewById<Button>(Resource.Id.select_profile_btn);

            btnSelectProfile.Click += delegate
            {
                OnClickSelectProfileButton();
            };

            txtProfileFileName = FindViewById<TextView>(Resource.Id.profile_filename);
            txtProfileFileName.Text = Resources.GetString(Resource.String.conn_no_profile);

            llServerInfo = FindViewById<LinearLayout>(Resource.Id.server_info_layout);

            txtServerName = FindViewById<TextView>(Resource.Id.profile_server);
            txtServerName.Text = "";

            txtServerPort = FindViewById<TextView>(Resource.Id.profile_port);
            txtServerPort.Text = "";

            txtServerProtocol = FindViewById<TextView>(Resource.Id.profile_protocol);
            txtServerProtocol.Text = "";

            btnConnectProfile = FindViewById<Button>(Resource.Id.connect_profile_btn);

            btnConnectProfile.Click += delegate
            {
                OnStartConnection();
            };

            btnDisconnectProfile = FindViewById<Button>(Resource.Id.disconnect_profile_btn);

            btnDisconnectProfile.Click += delegate
            {
                OnStopConnection();
            };

            txtVpnStatus = FindViewById<TextView>(Resource.Id.vpn_connection_status);
            txtVpnStatus.Text = Resources.GetString(Resource.String.conn_status_disconnected);

            txtNetworkStatus = FindViewById<TextView>(Resource.Id.network_connection_status);
            txtNetworkStatus.Text = Resources.GetString(Resource.String.conn_status_disconnected);

            llConnectionError = FindViewById<LinearLayout>(Resource.Id.connection_error_layout);

            txtConnectionError = FindViewById<TextView>(Resource.Id.connection_error);
            txtConnectionError.Text = "";

            vpnManager = new VPNManager(this);

            vpnManager.StatusChanged += OnServiceStatusChanged;
            
            if(settingsManager.SystemRestoreLastProfile)
                RestoreLastProfile();
            else
            {
                txtProfileFileName.Text = Resources.GetString(Resource.String.conn_no_profile);

                llServerInfo.Visibility = ViewStates.Gone;
            }
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

            switch(requestCode)
            {
                case ACTIVITY_RESULT_FILE_CHOOSER:
                {
                    if(data != null)
                    {
                        profileUri = data.Data;

                        SelectOpenVPNProfile(profileUri);
                    }
                }
                break;

                case ACTIVITY_RESULT_SETTINGS:
                {
                    if(resultCode == Result.Ok)
                    {
                        if(currentConnectionStatus == VPN.Status.CONNECTED)
                            supportTools.InfoDialog(Resource.String.settings_changed);
                    }
                }
                break;

                case VPNManager.VPN_REQUEST_CODE:
                {
                    if(vpnManager != null)
                        vpnManager.HandleActivityResult(requestCode, resultCode, data);
                }
                break;
                
                default:
                {
                    EddieLogger.Warning("MainActivity::OnActivityResult: unhandled requestCode {0}, resultCode {1}", requestCode, resultCode, new string[0]);
                }
                break;
            }
		}

        private void RestoreLastProfile()
        {
            string value = "";
            
            profileData = settingsManager.SystemLastProfile;
            
            profileInfo = settingsManager.SystemLastProfileInfo;

            if(!profileData.Equals(""))
            {
                if(profileInfo.TryGetValue("name", out value))
                    txtProfileFileName.Text = value;
                else
                    txtProfileFileName.Text = Resources.GetString(Resource.String.conn_status_unknown);
                
                if(profileInfo.TryGetValue("server", out value))
                    txtServerName.Text = value;
                else
                    txtServerName.Text = Resources.GetString(Resource.String.conn_status_unknown);
                
                if(profileInfo.TryGetValue("port", out value))
                    txtServerPort.Text = value;
                else
                    txtServerPort.Text = Resources.GetString(Resource.String.conn_status_unknown);
    
                if(profileInfo.TryGetValue("protocol", out value))
                    txtServerProtocol.Text = value;
                else
                    txtServerProtocol.Text = Resources.GetString(Resource.String.conn_status_unknown);

                llServerInfo.Visibility = ViewStates.Visible;

                btnConnectProfile.Enabled = true;
                btnDisconnectProfile.Enabled = false;
            }
            else
            {
                txtProfileFileName.Text = Resources.GetString(Resource.String.conn_no_profile);

                llServerInfo.Visibility = ViewStates.Gone;
            }
        }

        private void SelectOpenVPNProfile(Uri profile)
        {
            if(profile == null)
                return;

            profileInfo = supportTools.GetOpenVPNProfile(profileUri);

            if(profileInfo != null)
            {
                if(profileInfo["status"].Equals("ok"))
                {
                    profileData = profileInfo["profile"];

                    if(profileInfo.ContainsKey("name") == true)
                        txtProfileFileName.Text = profileInfo["name"];
                    else
                        txtProfileFileName.Text = "???";
                    
                    if(profileInfo.ContainsKey("server") == true)
                        txtServerName.Text = profileInfo["server"];
                    else
                        txtServerName.Text = "???";

                    if(profileInfo.ContainsKey("port") == true)
                        txtServerPort.Text = profileInfo["port"];
                    else
                        txtServerPort.Text = "???";

                    if(profileInfo.ContainsKey("protocol") == true)
                        txtServerProtocol.Text = profileInfo["protocol"];
                    else
                        txtServerProtocol.Text = "???";

                    llServerInfo.Visibility = ViewStates.Visible;

                    btnConnectProfile.Enabled = true;
                    btnDisconnectProfile.Enabled = false;
                }
                else
                {
                    int errMsg = 0;
                    
                    if(profileInfo.ContainsKey("status") == true)
                    {
                        if(profileInfo["status"].Equals("not_found"))
                            errMsg = Resource.String.conn_profile_not_found;
                        else if(profileInfo["status"].Equals("invalid"))
                            errMsg = Resource.String.conn_profile_is_invalid;
                        else if(profileInfo["status"].Equals("no_permission"))
                            errMsg = Resource.String.conn_profile_no_permission;

                        supportTools.InfoDialog(errMsg);
                    }
                    
                    profileData = "";
                    
                    profileInfo["name"] = "";
                    profileInfo["server"] = "";
                    profileInfo["port"] = "";
                    profileInfo["protocol"] = "";
                    
                    txtProfileFileName.Text = Resources.GetString(Resource.String.conn_no_profile);

                    llServerInfo.Visibility = ViewStates.Gone;
                }

                SaveCurrentProfile();
            }
        }
        
		protected override void OnStart()
		{
			base.OnStart();

			if(vpnManager != null)
				vpnManager.HandleActivityStart();
		}
	
		protected override void OnStop()
		{
			base.OnStop();

			if(vpnManager != null)
				vpnManager.HandleActivityStop();
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
    
            networkStatusReceiver.RemoveListener(this);
    
            this.UnregisterReceiver(networkStatusReceiver);
            
            AndroidEnvironment.UnhandledExceptionRaiser -= supportTools.HandleUncaughtException;
        }

        protected void OnNavigationViewItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            drawer.CloseDrawers();

            switch(e.MenuItem.ItemId)
            {
                case Resource.Id.nav_select_profile:
                {
                    OnClickSelectProfileButton();
                }
                break;

                case Resource.Id.nav_log:
                {
                    Intent logActivityIntent = new Intent(ApplicationContext, typeof(LogActivity));
                    
                    logActivityIntent.PutExtra("ViewMode", (int)LogActivity.ViewMode.ListView);

                    StartActivity(logActivityIntent);
                }
                break;

                case Resource.Id.nav_settings:
                {
                    Intent settingsActivityIntent = new Intent(ApplicationContext, typeof(SettingsActivity));
                    
                    StartActivityForResult(settingsActivityIntent, ACTIVITY_RESULT_SETTINGS);
                }
                break;

                case Resource.Id.nav_about:
                {
                    Intent aboutActivityIntent = new Intent(ApplicationContext, typeof(AboutActivity));
                    
                    StartActivity(aboutActivityIntent);
                }
                break;

                case Resource.Id.nav_website:
                {
					// Clodo Hack

					Intent i = new Intent(Intent.ActionView);
					i.SetData(Android.Net.Uri.Parse(Resources.GetString(Resource.String.eddie_url)));
					StartActivity(i);
				}
                break;
            }
        }

        protected void OnClickSelectProfileButton()
        {
            if(currentConnectionStatus == VPN.Status.CONNECTED)
            {
                supportTools.InfoDialog(string.Format(Resources.GetString(Resource.String.conn_disconnect_first), profileInfo["server"]));

                return;
            }

            Intent fileChooserIntent = new Intent();

            fileChooserIntent.SetAction(Intent.ActionGetContent);
            fileChooserIntent.SetType("*/*");

            StartActivityForResult(Intent.CreateChooser(fileChooserIntent, Resources.GetString(Resource.String.conn_select_profile_cap)), ACTIVITY_RESULT_FILE_CHOOSER);
        }

        private void OnServiceStatusChanged(bool ready, VPN.Status status, string error)
        {
            UpdateConnectionStatus(ready, status, error);
        }

        private void StartConnection()
        {
            if(profileData.Equals(""))
            {
                supportTools.InfoDialog(Resource.String.conn_no_profile_selected);

                return;
            }

            vpnManager.ClearProfiles();

            vpnManager.SetProfile(profileData);

            string profileString = settingsManager.Ovpn3CustomDirectives.Trim();
            
            if(profileString.Length > 0)
                vpnManager.AddProfileString(profileString);

            vpnManager.Start();
        }

        private void OnStartConnection()
        {
            if(profileData.Equals(""))
            {
                supportTools.InfoDialog(Resource.String.conn_no_profile_selected);

                return;
            }

            try
            {
                StartConnection();
            }           
            catch(System.Exception e)
            {
                ShowErrorMessage(e.Message);
            }
        }

        private void OnStopConnection()
        {
            try
            {
                vpnManager.Stop();
            }
            catch(System.Exception e)
            {
                ShowErrorMessage(e.Message);
            }
        }

        private void UpdateConnectionStatus(bool ready, VPN.Status status, string error)
        {
            if(ready)
                txtVpnStatus.Text = Resources.GetString(VPN.DescriptionResource(status));
            else
                txtVpnStatus.Text = Resources.GetString(Resource.String.conn_status_initialize);

            btnConnectProfile.Enabled = ready && (status == VPN.Status.NOT_CONNECTED) && NetworkStatusReceiver.IsNetworkConnected();

            btnDisconnectProfile.Enabled = (status == VPN.Status.CONNECTING) || (status == VPN.Status.CONNECTED) || (status == VPN.Status.PAUSED) || (status == VPN.Status.LOCKED);

            if(currentConnectionStatus != status)
            {
                currentConnectionStatus = status;
                
                switch(status)
                {
                    case VPN.Status.CONNECTED:
                    {
                        if(profileInfo.ContainsKey("server") == true)
                            supportTools.InfoDialog(string.Format(Resources.GetString(Resource.String.connection_success), profileInfo["server"], NetworkStatusReceiver.GetNetworkDescription()));

                        settingsManager.SystemLastProfileIsConnected = true;
                    }
                    break;

                    case VPN.Status.NOT_CONNECTED:
                    {
                        if(profileInfo.ContainsKey("server") == true)
                            supportTools.InfoDialog(string.Format(Resources.GetString(Resource.String.connection_disconnected), profileInfo["server"]));

                        settingsManager.SystemLastProfileIsConnected = false;
                    }
                    break;

                    case VPN.Status.PAUSED:
                    {
                        supportTools.InfoDialog(Resources.GetString(Resource.String.connection_paused));

                        settingsManager.SystemLastProfileIsConnected = true;
                    }
                    break;

                    default:
                    {
                        settingsManager.SystemLastProfileIsConnected = false;
                    }
                    break;
                }
            }
            
            ShowErrorMessage(error);
        }
        
        private void SaveCurrentProfile()
        {
            Dictionary<string, string> pData = new Dictionary<string, string>();
            
            if(profileInfo == null || profileData.Equals(""))
                return;

            settingsManager.SystemLastProfile = profileData;

            if(profileInfo.ContainsKey("name") == true)
                pData.Add("name", profileInfo["name"]);
            else
                pData.Add("name", "???");

            if(profileInfo.ContainsKey("server") == true)
                pData.Add("server", profileInfo["server"]);
            else
                pData.Add("server", "???");

            if(profileInfo.ContainsKey("port") == true)
                pData.Add("port", profileInfo["port"]);
            else
                pData.Add("port", "???");

            if(profileInfo.ContainsKey("protocol") == true)
                pData.Add("protocol", profileInfo["protocol"]);
            else
                pData.Add("protocol", "???");

            settingsManager.SystemLastProfileInfo = pData;
        }

        private void ShowErrorMessage(string error)
        {
            if(error.Equals("") == false)
            {
                llConnectionError.Visibility = ViewStates.Visible;
                txtConnectionError.Text = error;
            }
            else
            {
                llConnectionError.Visibility = ViewStates.Gone;
                txtConnectionError.Text = "";
            }
        }
        
        // NetworkStatusReceiver

        public void OnNetworkStatusNotAvailable()
        {
            if(txtNetworkStatus != null)
                txtNetworkStatus.Text = Resources.GetString(Resource.String.conn_status_not_available);
                        
            if(btnConnectProfile != null)
                btnConnectProfile.Enabled = (currentConnectionStatus == VPN.Status.NOT_CONNECTED) && NetworkStatusReceiver.IsNetworkConnected();
        }

        public void OnNetworkStatusConnected()
        {
            if(txtNetworkStatus != null)
                txtNetworkStatus.Text = string.Format(Resources.GetString(Resource.String.conn_status_connected), NetworkStatusReceiver.GetNetworkDescription());
            
            if(btnConnectProfile != null)
                btnConnectProfile.Enabled = (currentConnectionStatus == VPN.Status.NOT_CONNECTED) && NetworkStatusReceiver.IsNetworkConnected();
        }
    
        public void OnNetworkStatusIsConnecting()
        {
            if(txtNetworkStatus != null)
                txtNetworkStatus.Text = Resources.GetString(Resource.String.conn_status_not_available);
            
            if(btnConnectProfile != null)
                btnConnectProfile.Enabled = (currentConnectionStatus == VPN.Status.NOT_CONNECTED) && NetworkStatusReceiver.IsNetworkConnected();
        }
    
        public void OnNetworkStatusIsDisonnecting()
        {
            if(txtNetworkStatus != null)
                txtNetworkStatus.Text = Resources.GetString(Resource.String.conn_status_not_available);
            
            if(btnConnectProfile != null)
                btnConnectProfile.Enabled = (currentConnectionStatus == VPN.Status.NOT_CONNECTED) && NetworkStatusReceiver.IsNetworkConnected();
        }
    
        public void OnNetworkStatusSuspended()
        {
            if(txtNetworkStatus != null)
                txtNetworkStatus.Text = Resources.GetString(Resource.String.conn_status_not_available);
            
            if(btnConnectProfile != null)
                btnConnectProfile.Enabled = (currentConnectionStatus == VPN.Status.NOT_CONNECTED) && NetworkStatusReceiver.IsNetworkConnected();
        }
    
        public void OnNetworkStatusNotConnected()
        {
            if(txtNetworkStatus != null)
                txtNetworkStatus.Text = Resources.GetString(Resource.String.conn_status_disconnected);
            
            if(btnConnectProfile != null)
                btnConnectProfile.Enabled = (currentConnectionStatus == VPN.Status.NOT_CONNECTED) && NetworkStatusReceiver.IsNetworkConnected();
        }
    
        public void OnNetworkTypeChanged()
        {
            if(txtNetworkStatus != null)
                txtNetworkStatus.Text = Resources.GetString(Resource.String.conn_status_disconnected);
            
            if(btnConnectProfile != null)
                btnConnectProfile.Enabled = (currentConnectionStatus == VPN.Status.NOT_CONNECTED) && NetworkStatusReceiver.IsNetworkConnected();
        }
    }
}
