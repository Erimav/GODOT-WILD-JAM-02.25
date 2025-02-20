using Godot;
using System;
using System.Collections.Generic;

public interface IItemUsage { }
public class LuxuryItemUsage : IItemUsage { }

public class BuffItemUsage : IItemUsage
{
    public Action<Mob> UseAction { get; init; }
}

public class FieldItemUsage : IItemUsage
{
    public Func<TilePosition, bool> UseAction { get; init; }
}

public record Item(string Name, string Description, string IconName, int Price, IItemUsage Usage)
{
    public static IReadOnlyList<Item> List { get; } = new List<Item>()
    {
        new("Magnifying glass", "Reveals a tile.", "magnifier.png", 50, new FieldItemUsage
            {
                UseAction = (tile) =>
                {
                    //Reveal code
                    return true;
                }
            }),
        new("Dynamite",         "Powerful enough to blow up a tower.", "dynamite.png", 100, new FieldItemUsage
            {
                UseAction = (tile) =>
                {
                    //Blow up code
                    return true;
                }
            }),
        new("Energy potion",    "An invigorating drink. One sip can make anyone run faster.", "potion.png", 75, new BuffItemUsage
        {
            UseAction = (mob) =>
            {
                // Increase mob speed
            }
        }),
        new("Golden apple",     "Healthy food makes people(and goblins) more durable.", "apple.png", 100, new BuffItemUsage
        {
            UseAction = (mob) =>
            {
                // Increase mob health
            }
        }),
        new("Shiny gem",        "No particular reason to buy it, but it's very beautiful.", "gem.png", 1000, new LuxuryItemUsage())
    };
}