#if TOOLS

using Godot;

namespace DeerEssentials;

/// <summary>
/// Execute during the project export process
/// </summary>
public partial class EditorExport : EditorExportPlugin
{
	public override string _GetName()
	{
		return "Deer Export Plugin";
	}

	public override void _ExportBegin(string[] features, bool isDebug, string path, uint flags)
	{
		base._ExportBegin(features, isDebug, path, flags);
		
		// check if version should be bumped
		var generate = (Plugin.BuildVersionSetting) 
			ProjectSettings.GetSetting(SettingNames.VersionBumpMode, 0).AsInt16();
		
		if (generate != Plugin.BuildVersionSetting.OnExport) return;

		Plugin.BumpBuildNumber();
	}
}
#endif
