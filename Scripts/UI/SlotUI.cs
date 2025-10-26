using Godot;

public partial class SlotUI : Button
{
	[Export] public bool IsHotbar = false;
	public Inventory Inventory;
	public int Index;

	private TextureRect _icon;
	private Label _count;

	public override void _Ready()
	{
		_icon = GetNode<TextureRect>("Icon");
		_count = GetNode<Label>("Count");
	}

	public void Bind(Inventory inv, bool hotbar, int index)
	{ Inventory = inv; IsHotbar = hotbar; Index = index; UpdateView(); }

	ItemStack Stack => (IsHotbar ? Inventory.Hotbar[Index] : Inventory.Slots[Index]);

public void UpdateView()
{
	if (Inventory == null || _icon == null || _count == null)
		return; // nic nie rób, jeśli slot nie jest jeszcze zainicjalizowany

	var stack = (IsHotbar ? Inventory.Hotbar[Index] : Inventory.Slots[Index]);
	if (stack == null || stack.Item == null || stack.Count <= 0)
	{
		_icon.Texture = null;
		_count.Text = "";
		return;
	}

	_icon.Texture = stack.Item.Icon;
	_count.Text = stack.Count > 1 ? stack.Count.ToString() : "";
}


	// DRAG
	public override Variant _GetDragData(Vector2 atPosition)
{
	if (Inventory == null || Stack.IsEmpty)
		return new Variant(); // <- tu było brak returna

	var data = new Godot.Collections.Dictionary
	{
		{ "inv", Inventory },
		{ "hotbar", IsHotbar },
		{ "index", Index }
	};

	var preview = _icon.Duplicate() as Control;
	SetDragPreview(preview);

	return Variant.From<Godot.Collections.Dictionary>(data);
}


	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		if (data.VariantType != Variant.Type.Dictionary) return false;
		var dict = data.AsGodotDictionary();
		return dict.ContainsKey("inv");
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		var d = data.AsGodotDictionary();
		var fromInv   = d["inv"].As<Inventory>();
		bool fromHot  = (bool)d["hotbar"];
		int fromIdx   = (int)d["index"];

		var fromStack = (fromHot ? fromInv.Hotbar[fromIdx] : fromInv.Slots[fromIdx]);
		var toStack   = Stack;

		if (fromInv == Inventory && fromHot == IsHotbar && fromIdx == Index) return;

		Inventory.MoveOrMerge(fromStack, toStack);

		// odśwież wszystkie panele w grupie inventory_ui
		GetTree().CallGroup("inventory_ui", "RefreshAll");
	}
}
