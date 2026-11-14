#if TOOLS
using System.Linq;
using Godot;
namespace DeerEssentials;

public partial class ContextMenu : EditorContextMenuPlugin
{
	private string[] _lastPaths;
	
	public override void _PopupMenu(string[] paths)
	{
		base._PopupMenu(paths);
		
		if (paths.Length == 1)
		{
			_lastPaths = paths;
			
			AddContextMenuItem(
				"C# Copy Node Reference", 
				new Callable(this, MethodName.CopyNodeReference), 
				ResourceLoader.Load<Texture2D>(DeerEssentials.Icons.ActionCopy));
		}
		
	}
	
	public void CopyNodeReference(Godot.Collections.Array data)
	{
		if (_lastPaths is null) return;
		
		var node = data.First().As<Node>();
		string path;
		if (node.IsUniqueNameInOwner())
		{
			path = "%" + node.Name;
		}
		else
		{
			path = _lastPaths.First();
		}

		var result = $"GetNode<{node.GetClass()}>(\"{path}\");";
		DisplayServer.ClipboardSet(result);
	}
}
#endif