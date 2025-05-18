<h1>Wake on Lan (WOL) GUI</h1>
<p>A graphical interface to send a Wake-on-Lan packet made in Unity.</p>
<ul><li>Wake on Application Startup</li><li>Run as a startup app (Windows)</li><li>Ping's an IP address when there is response opens website(s)</li></ul>
<h2>Installation and How to use:</h2><p>To download checkout the&nbsp;[Release](https://github.com/RezaRafia/Speedy/releases) section of this repo.&nbsp;</p><p>To build from source download/clone the repo and open in Unity w/ version&nbsp;6000.0.48f1.</p><p></p>
<p>All settings are stored in in an JSON file named:</p>
<pre>
WOL_CONFIG_SETTINGS.json</pre>
<p>Modify the JSON to change settings/parameters.</p>
<h3>UI Buttons:</h3>
<ul><li>Wake button</li><li>Copy button - copies the path of the JSON file.</li><li>File button - opens the file in your default text editor program (creates new JSON if none found).</li><li>Delete button -&nbsp;deletes the existing JSON file instance.</li><li>Reset</li><li>Quit</li></ul>
<h3>Settings and Flags:</h3>
<ul><li><strong><em>StartFullscreen</em></strong>:<strong>&nbsp;</strong>Starts the application in fullscreen<strong>&nbsp;(default: false)</strong></li></ul>
<ul><li><strong><em>DefaultResolution</em></strong>: Set a custom windowed resolution.&nbsp;<strong><em>StartFullscreen</em>&nbsp;</strong>must be <strong>false</strong>. Set to default values of (0,0) to automatically scale window resolution to 50% of monitor resolution.&nbsp;<strong>(default: x=0, y=0)</strong></li></ul>
<ul><li><strong><em>ExecuteWakeOnStart</em></strong>: On application startup will automatically execute the wake-on-lan command after the set amount of&nbsp;<strong><em>WakeOnStartDelay</em></strong> along with any other overloads such as launching sites and quitting.&nbsp;<strong>(default: false)</strong></li></ul>
<ul><li><strong><em>WakeOnStartDelay</em></strong>: how much delay before it starts execution. This gives you time to enter the reset hotkey if access to GUI is needed with&nbsp;<strong><em>QuitApplicationAfterWake</em></strong> flag set&nbsp;&nbsp;<strong>(default: 3.0)</strong></li></ul>
<ul><li><strong><em>MAC</em></strong>: must be in the format of FF:FF:FF:FF:FF:FF</li></ul>
<ul><li><strong><em>BroadcastAddress</em></strong>:<strong>&nbsp;</strong>Recommended to not touch if you don't know what your are doing.&nbsp;<strong>(default: 255.255.255.255)</strong></li></ul>
<ul><li><strong><em>BroadcastPort</em></strong>: Recommended to not touch if you don't know what your are doing.&nbsp;<strong>(default: 9)</strong></li><li><strong><em>LaunchSitesAfterWake</em></strong>: enables loading of websites when ping from device/server returns<strong>(default: false)</strong></li><li><strong><em>QuitApplicationAfterWake</em></strong>: Quits application after wake or sites loaded if&nbsp;&nbsp;<strong><em>LaunchSitesAfterWake </em></strong>is true&nbsp;<strong>(default: false)</strong></li><li><strong><em>Ping_IP</em></strong>: The usually local IP address of the device/server you are trying to turn on. When it returns a successful ping sites will be loaded.</li><li><strong><em>SitesToLoad</em></strong>: Array of websites to load after the ping of the server address is successful.&nbsp;<strong><em>LaunchSitesAfterWake</em> </strong>must be <strong>true&nbsp;</strong>and <strong><em>Ping_IP&nbsp;</em></strong>must be set.</li></ul>
<p></p>
<p><br></p>
<h3>Advanced:</h3>
<p>Default path on Windows is :</p>
<pre>
C:/Users/&lt;USERNAME&gt;/AppData/LocalLow/TurnipTossProductions/WAKE (WOL-GUI)/WOL_CONFIG_SETTINGS.json</pre>
<p>Press&nbsp;<strong>[F2] </strong>or<strong>&nbsp;[DELETE]</strong>&nbsp;to reset&nbsp; <strong><em>ExecuteWakeOnStart,</em>&nbsp;</strong><strong><em>WakeOnStartDelay, LaunchSitesAfterWake, QuitApplicationAfterWake</em> </strong>to default values<strong style="text-decoration-line: line-through;"></strong><strong style="text-decoration-line: line-through;"></strong></p>
<p>This allows you to make changes to the JSON config file through GUI even if <strong><em>QuitApplicationAfterWake</em></strong> flag is enabled.</p>
