using Godot;

public partial class Chest : Area2D
{
	[Export] public NodePath InventoryPath;
	[Export] public PackedScene ChestUIPrefab;

	private Inventory _inv;
	private ChestUI _openUi;

	public override void _Ready()
	{
		_inv = GetNode<Inventory>(InventoryPath);
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name != "Player") return;
		SetProcessInput(true);
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name != "Player") return;
		SetProcessInput(false);
		CloseUi();
	}

	public override void _Input(InputEvent e)
	{
		if (Input.IsActionJustPressed("open_inventory"))
			ToggleUi();
	}

	private void ToggleUi()
	{
		if (_openUi == null) OpenUi(); else CloseUi();
	}

	private void OpenUi()
	{
		_openUi = (ChestUI)ChestUIPrefab.Instantiate();
		// znajdź Inventory gracza:
		var playerInv = GetTree().CurrentScene.GetNode<Inventory>("Game/Player/Inventory");
		_openUi.Bind(playerInv, _inv);
		GetTree().CurrentScene.AddChild(_openUi);
	}

	private void CloseUi()
	{
		_openUi?.QueueFree();
		_openUi = null;
	}
}
