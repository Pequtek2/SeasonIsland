using Godot;

public partial class ItemStack : RefCounted
{
	public ItemData Item;
	public int Count;

	public ItemStack() {}
	public ItemStack(ItemData item, int count) { Item = item; Count = count; }
	public bool IsEmpty => Item == null || Count <= 0;
	public int FreeSpace => Item == null ? 0 : Mathf.Max(0, Item.MaxStack - Count);

	public int AddUpTo(int amount)
	{
		if (Item == null) return amount;
		var take = Mathf.Min(amount, FreeSpace);
		Count += take;
		return amount - take;
	}
	public ItemStack Clone() => new(Item, Count);
}
