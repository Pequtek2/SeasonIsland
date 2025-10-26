using Godot;

public partial class ChestUI : Control
{
	[Export] public NodePath ChestGridPath;
	[Export] public NodePath PlayerGridPath;

	private GridContainer _chestGrid, _playerGrid;
	private Inventory _playerInv, _chestInv;

	public override void _Ready()
	{
		AddToGroup("inventory_ui");
	}

	public void Bind(Inventory player, Inventory chest)
	{
		_playerInv = player; _chestInv = chest;
		_chestGrid  = GetNode<GridContainer>(ChestGridPath);
		_playerGrid = GetNode<GridContainer>(PlayerGridPath);

		Build(_chestGrid, _chestInv.Slots.Length, _chestInv, false);
		Build(_playerGrid, _playerInv.Slots.Length, _playerInv, false);
		Visible = true;
		RefreshAll();
	}

	private void ClearChildren(Node parent)
	{
		foreach (Node n in parent.GetChildren())
			n.QueueFree();
	}

	private void Build(GridContainer grid, int count, Inventory inv, bool hotbar)
	{
		ClearChildren(grid);

		for (int i = 0; i < count; i++)
		{
			var slot = (SlotUI)((PackedScene)GD.Load("res://scenes/ui/Slot.tscn")).Instantiate();
			slot.Bind(inv, hotbar, i);
			grid.AddChild(slot);
		}
	}

	public void RefreshAll()
	{
		foreach (var node in _chestGrid.GetChildren()) (node as SlotUI)?.UpdateView();
		foreach (var node in _playerGrid.GetChildren()) (node as SlotUI)?.UpdateView();
	}

	public override void _UnhandledInput(InputEvent e)
	{
		if (Input.IsActionJustPressed("open_inventory"))
			QueueFree(); // zamknij okno skrzyni
	}
}
