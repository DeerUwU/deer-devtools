using Godot;
using System;
using DeerEssentials;

namespace DeerEssentials;

/// <summary>
/// General code that can be accessed within the project
/// </summary>
public class Essentials
{
	public static string GetVersionString()
	{
		return ProjectSettings.GetSetting(SettingNames.InternalVersionString, SettingDefaults.InternalVersionString).AsString();
	}
}
