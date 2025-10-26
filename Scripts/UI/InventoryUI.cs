using Godot;

public partial class InventoryUI : Control
{
	[Export] public NodePath PlayerInventoryPath;
	[Export] public NodePath HotbarGridPath;
	[Export] public NodePath BackpackGridPath;

	private Inventory _inv;
	private GridContainer _hotbar;
	private GridContainer _backpack;

	public override void _Ready()
	{
		AddToGroup("inventory_ui");
		_inv = GetNode<Inventory>(PlayerInventoryPath);
		_hotbar = GetNode<GridContainer>(HotbarGridPath);
		_backpack = GetNode<GridContainer>(BackpackGridPath);

		Build(_hotbar, _inv.Hotbar.Length, true);
		Build(_backpack, _inv.Slots.Length, false);

		Visible = false; // E otwiera
	}

	private void ClearChildren(Node parent)
	{
		foreach (Node n in parent.GetChildren())
			n.QueueFree();
	}

	private void Build(GridContainer grid, int count, bool hotbar)
	{
		ClearChildren(grid); // <<< zamiast QueueFreeChildren()

		for (int i = 0; i < count; i++)
		{
			var slotScene = (PackedScene)GD.Load("res://scenes/ui/Slot.tscn");
			var slot = (SlotUI)slotScene.Instantiate();
			slot.Bind(_inv, hotbar, i);
			grid.AddChild(slot);
		}
	}

	public void RefreshAll()
	{
		foreach (var node in _hotbar.GetChildren())
			(node as SlotUI)?.UpdateView();
		foreach (var node in _backpack.GetChildren())
			(node as SlotUI)?.UpdateView();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsActionJustPressed("open_inventory"))
		{
			Visible = !Visible;
			if (Visible) RefreshAll();
			// GetViewport().SetInputAsHandled(); // można pominąć
		}

		for (int i = 1; i <= 6; i++)
			if (Input.IsActionJustPressed($"hotbar_{i}"))
				GD.Print($"ACTIVE HOTBAR: {i - 1}");
	}
}
