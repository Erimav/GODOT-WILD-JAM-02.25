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
    public Action UseAction { get; init; }

    public enum Action { Reveal, BlowUp }
}

public record Item(string Name, string Description, string IconName, int Price, IItemUsage Usage)
{
    public static IReadOnlyList<Item> List { get; } = new List<Item>()
    {
        new("Magnifying glass", "Reveals a tile.", "magnifier.png", 50, new FieldItemUsage
            {
                UseAction = FieldItemUsage.Action.Reveal,
            }),
        new("Dynamite",         "Powerful enough to blow up a tower.", "dynamite.png", 100, new FieldItemUsage
            {
                UseAction = FieldItemUsage.Action.BlowUp,
            }),
        new("Energy potion",    "An invigorating drink. One sip can make anyone run faster.", "potion.png", 75, new BuffItemUsage
        {
            UseAction = (mob) =>
            {
                mob.CurrentHP *= 2;
            }
        }),
        new("Golden apple",     "Healthy food makes people(and goblins) more durable.", "apple.png", 100, new BuffItemUsage
        {
            UseAction = (mob) =>
            {
                mob.MoveSpeed *= 1.5f;
            }
        }),
        new("Shiny gem",        "No particular reason to buy it, but it's very beautiful.", "gem.png", 1000, new LuxuryItemUsage())
    };

    private Texture2D mIconCache;

    public Texture2D Icon => mIconCache ??= GD.Load<Texture2D>($"res://Assets/Sprites/Items/{IconName}");
}