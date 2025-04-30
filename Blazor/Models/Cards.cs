namespace Blazor.models;
public class Card(string id, string name, int cost, int defense, int strength, string image)
{
    public string Id { get; init; } = id;
    public string Name { get; init; } = name;
    public int Defense { get; init; } = defense;
    public int Cost { get; init; } = cost;
    public int Strength { get; init; } = strength;
    public string Image { get; init; } = image;


    public override string ToString()
    {
        return $"Name: {Name}\n - Cost: {Cost}\n - Strength: {Strength}\n - Defense: {Defense}\nImage: {Image}";
    }
}