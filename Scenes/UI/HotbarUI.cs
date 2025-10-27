using Godot;
using System.Collections.Generic;

public partial class HotbarUI : Control
{
	[Export] public NodePath PlayerInventoryPath;   // wskaż: ../Player/Inventory
	[Export] public NodePath HotbarGridPath;        // wskaż: Margin/Panel/HotbarGrid

	private Inventory _inv;
	private GridContainer _grid;
	private List<SlotUI> _slots = new();

	public override void _Ready()
	{
		AddToGroup("inventory_ui"); // żeby RefreshAll działało globalnie
		_inv = GetNode<Inventory>(PlayerInventoryPath);
		_grid = GetNode<GridContainer>(HotbarGridPath);

		Build();
		RefreshAll();  // pokaż od razu ikony
		Visible = true; // zawsze widoczny HUD
	}

	private void ClearChildren(Node parent)
	{
		foreach (Node n in parent.GetChildren()) n.QueueFree();
	}

	private void Build()
	{
		ClearChildren(_grid);
		_slots.Clear();

		var slotScene = (PackedScene)GD.Load("res://scenes/ui/Slot.tscn");
		for (int i = 0; i < _inv.Hotbar.Length; i++)
		{
			var slot = (SlotUI)slotScene.Instantiate();
			slot.FocusMode = FocusModeEnum.All; // pozwala podświetlać wybrany
			slot.Bind(_inv, true, i);
			_grid.AddChild(slot);
			_slots.Add(slot);
		}
	}

	public void RefreshAll()
	{
		foreach (var s in _slots) s.UpdateView();
	}

	public override void _UnhandledInput(InputEvent e)
	{
		// wybór aktywnego slotu 1..6
		for (int i = 1; i <= 6; i++)
		{
			if (Input.IsActionJustPressed($"hotbar_{i}"))
			{
				SetActive(i - 1);
			}
		}
	}

	private void SetActive(int index)
	{
		if (index < 0 || index >= _slots.Count) return;
		_slots[index].GrabFocus(); // pokaże styl Focus => podświetlenie
		// tu możesz też zapisać aktywny indeks do jakiegoś globala, jeśli chcesz
		// np. EventBus.ActiveHotbarIndex = index;
	}
}
