using Godot;

[GlobalClass]
public partial class ItemData : Resource
{
	[Export] public string Id = "";
	[Export] public string DisplayName = "";
	[Export] public Texture2D Icon;
	[Export] public int MaxStack = 30;
	[Export] public string Type = "generic"; // np. seed, tool, material
}
