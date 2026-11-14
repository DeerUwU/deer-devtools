namespace DeerEssentials;

public static class SettingNames
{
	public const string BuildNumber = "Deer's Essentials/versioning/BuildNumber";
	public const string VersionBumpMode = "Deer's Essentials/versioning/Version Bump";
	public const string VersionFormatting = "Deer's Essentials/versioning/Version Formatting";
	public const string InternalVersionString = "Deer's Essentials/versioning/InternalVersion";
}

public static class SettingDefaults
{
	public const string VersionFormatting = "Build %build - %yy/%MM/%dd %hh:%mm.%ss";
	public const string InternalVersionString = "No version has been generated yet.";
}

public static class Icons
{
	public const string ActionCopy = "res://addons/DeerEssentials/assets/icons/ActionCopy.svg";
}