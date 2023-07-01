namespace BZModding.Nautilus;

internal class Ingredient
{
    public Ingredient()
    {
    }

    public Ingredient(string name, int amount)
    {
        Name = name;
        Amount = amount;
    }

    public string Name { get; set; }
    public int Amount { get; set; }
}
