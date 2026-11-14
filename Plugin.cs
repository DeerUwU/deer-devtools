#if TOOLS

using System;
using System.Text.RegularExpressions;
using Godot;
using Godot.Collections;

namespace DeerEssentials;

/// <summary>
/// Main Plugin Script
/// </summary>
[Tool]
public partial class Plugin : EditorPlugin
{
	#region Vars

	private EditorContextMenuPlugin _sceneDockContextMenu = new ContextMenu();
	private EditorExportPlugin ExportPlugin = new EditorExport();

	private BuildVersionSetting BuildVersionMode = BuildVersionSetting.Off;
	
	public enum BuildVersionSetting
	{
		Off = 0,
		OnBuild = 1, // bump version when building project
		OnExport = 2, // bump version when exporting project
	}

	#endregion
	
	// -----------------------------------------------------------------------------------------------------------------
	
	#region Setup

		public override void _EnablePlugin()
		{
			base._EnablePlugin();
			
			// version bump mode
			if (!ProjectSettings.HasSetting(SettingNames.VersionBumpMode))
			{
				ProjectSettings.SetSetting(SettingNames.VersionBumpMode, (int) BuildVersionSetting.Off);
				ProjectSettings.SetInitialValue(SettingNames.VersionBumpMode, (int) BuildVersionSetting.Off);
				ProjectSettings.SetAsBasic(SettingNames.BuildNumber, true);
				ProjectSettings.AddPropertyInfo(
					new Dictionary() {
						{"name",SettingNames.VersionBumpMode},
						{"type", (int)Variant.Type.Int},
						{"hint", (int)PropertyHint.Enum},
						{"hint_string", $"{nameof(BuildVersionSetting.Off)},{nameof(BuildVersionSetting.OnBuild)},{nameof(BuildVersionSetting.OnExport)}"},
						// {"description","Decides when the version number should be bumped"}
					});
			}
			// version formatting string
			if (!ProjectSettings.HasSetting(SettingNames.VersionFormatting))
			{
				ProjectSettings.SetSetting(SettingNames.VersionFormatting, SettingDefaults.VersionFormatting);
				ProjectSettings.SetAsBasic(SettingNames.VersionFormatting, true);
			}
			// internal version string
			if (!ProjectSettings.HasSetting(SettingNames.InternalVersionString))
			{
				ProjectSettings.SetSetting(SettingNames.InternalVersionString, SettingDefaults.InternalVersionString);
				ProjectSettings.SetInitialValue(SettingNames.InternalVersionString, SettingDefaults.InternalVersionString);
				ProjectSettings.SetAsInternal(SettingNames.InternalVersionString, true);
			}
			// version build number
			if (!ProjectSettings.HasSetting(SettingNames.BuildNumber))
			{
				ProjectSettings.SetSetting(SettingNames.BuildNumber, 0);
				ProjectSettings.SetInitialValue(SettingNames.BuildNumber, 0);
				ProjectSettings.SetAsBasic(SettingNames.BuildNumber, false);
			}
		}

		public override void _DisablePlugin()
		{
			base._DisablePlugin();
			// setting data probably shouldn't be cleared in case you wanna disable & re-enable the plugin
		}


		// register plugin
		public override void _EnterTree()
		{
			base._EnterTree();

			if (ProjectSettings.HasSetting(SettingNames.VersionBumpMode))
			{
				ProjectSettings.AddPropertyInfo(
					new Dictionary() {
						{"name",SettingNames.VersionBumpMode},
						{"type", (int)Variant.Type.Int},
						{"hint", (int)PropertyHint.Enum},
						{"hint_string", $"{nameof(BuildVersionSetting.Off)},{nameof(BuildVersionSetting.OnBuild)},{nameof(BuildVersionSetting.OnExport)}"},
						// {"description","Decides when the version number should be bumped"}
					});
			}
			
			if (ProjectSettings.HasSetting(SettingNames.InternalVersionString))
			{
				ProjectSettings.SetAsInternal(SettingNames.InternalVersionString, true);
			}
			
			AddContextMenuPlugin(0, _sceneDockContextMenu);
			AddExportPlugin(ExportPlugin);
		}

		// unregister plugin
		public override void _ExitTree()
		{
			base._ExitTree();
			
			RemoveContextMenuPlugin(_sceneDockContextMenu);
			RemoveExportPlugin(ExportPlugin);
		}
		
	#endregion
	
	/// <summary>
	/// Runs when the game is being built
	/// </summary>
	/// <returns></returns>
	public override bool _Build()
	{
		var bvm = (BuildVersionSetting) ProjectSettings.GetSetting(SettingNames.VersionBumpMode, 0).AsInt16();
		if (bvm != BuildVersionSetting.OnBuild) return true;
		BumpBuildNumber();
		return true;
	}

	/// <summary>
	/// Increase the build number and store a version string inside projectsettings.
	/// the format will match the versionformatting setting.
	/// valid value identifiers: %build, %yy, %yyyy, %M, %MM, %MMM/%mon, %MMMM/%month, %dd, %ddd %dddd/day, %hh, %h, %m, %mm/min, %ss/sec
	/// most identifiers are case insensitive, except %MM, %M, %mm, %m
	/// <example>"build %build - %yyyy/%MM/%dd (%hh\h %m\m)" becomes "build 153 - 2003/01/13 (15h 5m)"</example>
	/// </summary>
	public static void BumpBuildNumber()
	{
		var buildnumber = ProjectSettings.GetSetting(SettingNames.BuildNumber, 0).AsInt32();
		buildnumber++;
		var t = DateTime.Now;
		
		string info = ProjectSettings.GetSetting(SettingNames.VersionFormatting, SettingDefaults.VersionFormatting).AsString();
		// parse version formatting. (?i) means case insensitive match
		info = Regex.Replace(info, "(?i)%build", $"{buildnumber}");
		info = Regex.Replace(info, "(?i)%yyyy", $"{t.Year:D4}");
		info = Regex.Replace(info, "(?i)%yy", $"{t.Year}"[^2..]);
		info = Regex.Replace(info, "(?i)%mmmm|(?i)%month", t.ToString("MMMM"));
		info = Regex.Replace(info, "(?i)%mmm|(?i)%mon", t.ToString("MMMM")[..3]);
		info = Regex.Replace(info, "%MM", $"{t.Month:D2}");
		info = Regex.Replace(info, "%M", $"{t.Month}");
		info = Regex.Replace(info, "(?i)%dddd|(?i)%day", t.ToString("dddd"));
		info = Regex.Replace(info, "(?i)%ddd", t.ToString("dddd")[..3]);
		info = Regex.Replace(info, "(?i)%dd", $"{t.Day:D2}");
		info = Regex.Replace(info, "(?i)%d", $"{t.Day}");
		info = Regex.Replace(info, "(?i)%hh", $"{t.Hour:D2}");
		info = Regex.Replace(info, "(?i)%h", $"{t.Hour}");
		info = Regex.Replace(info, "%mm|(?i)%min", $"{t.Minute:D2}");
		info = Regex.Replace(info, "%m", $"{t.Minute}");
		info = Regex.Replace(info, "(?i)%ss|(?i)%sec", $"{t.Second:D2}");
		info = Regex.Replace(info, "(?i)%s", $"{t.Second}");
		// allow escaping characters
		info = info.Replace("\\", "");
		
		ProjectSettings.SetSetting(SettingNames.BuildNumber, buildnumber);
		ProjectSettings.SetSetting(SettingNames.InternalVersionString, info);
		ProjectSettings.SetSetting("application/config/version", buildnumber);
		ProjectSettings.Save();

		GD.Print("bumped version.");
		GD.Print(info);
	}
}

#endif