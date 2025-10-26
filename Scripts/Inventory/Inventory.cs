using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Node
{
	[Export] public int Size = 18;
	[Export] public int HotbarSize = 6;
	public ItemStack[] Slots;     // 0..Size-1 -> backpack
	public ItemStack[] Hotbar;    // 0..HotbarSize-1

public override void _Ready()
{
	Slots = new ItemStack[Size];
	Hotbar = new ItemStack[HotbarSize];
	for (int i = 0; i < Size; i++) Slots[i] = new ItemStack();
	for (int i = 0; i < HotbarSize; i++) Hotbar[i] = new ItemStack();

	// TEST: dodaj marchew do ekwipunku
	var carrot = GD.Load<ItemData>("res://Data/Items/carrot.tres");
	AddToAny(carrot, 5);
}


	public static bool CanMerge(ItemStack a, ItemStack b)
		=> a.Item != null && b.Item != null && a.Item == b.Item;

	public bool AddToAny(ItemData item, int count)
	{
		// 1) spróbuj do istniejących stosów (hotbar + backpack)
		foreach (var arr in new[]{Hotbar, Slots})
		{
			for (int i = 0; i < arr.Length && count > 0; i++)
				if (!arr[i].IsEmpty && arr[i].Item == item)
					count = arr[i].AddUpTo(count);
		}
		// 2) wolne miejsca
		foreach (var arr in new[]{Hotbar, Slots})
		{
			for (int i = 0; i < arr.Length && count > 0; i++)
			{
				if (arr[i].IsEmpty)
				{
					var take = Mathf.Min(item.MaxStack, count);
					arr[i].Item = item;
					arr[i].Count = take;
					count -= take;
				}
			}
		}
		return count == 0;
	}

	public void Swap(ItemStack a, ItemStack b)
	{ var tmpItem = a.Item; var tmpCnt = a.Count; a.Item = b.Item; a.Count = b.Count; b.Item = tmpItem; b.Count = tmpCnt; }

	public void MoveOrMerge(ItemStack from, ItemStack to)
	{
		if (from.IsEmpty) return;
		if (to.IsEmpty) { to.Item = from.Item; to.Count = from.Count; from.Item=null; from.Count=0; return; }
		if (CanMerge(from, to))
		{
			var rest = Mathf.Max(0, (to.Count + from.Count) - to.Item.MaxStack);
			to.Count = Mathf.Min(to.Item.MaxStack, to.Count + from.Count);
			if (rest == 0) { from.Item = null; from.Count = 0; }
			else { from.Count = rest; }
		}
		else Swap(from, to);
	}
}
