using Godot;
using Godot.Collections;

public partial class LevelSection : Node2D
{
	private TileMapLayer skyLayer;

	public override void _Ready()
	{
		skyLayer = GetNode<TileMapLayer>("Bounds");
	}

	public Rect2I GetBounds()
	{
		Array<Vector2I> tiles = skyLayer.GetUsedCells();

		int minX = int.MaxValue;
		int minY = int.MaxValue;
		int maxX = int.MinValue;
		int maxY = int.MinValue;

		foreach (Vector2I v in tiles)
		{
			if (v.X * 8 < minX) minX = v.X;
			if (v.X * 8 > maxX) maxX = v.X;
			if (v.Y * 8 < minY) minY = v.Y;
			if (v.Y * 8 > maxY) maxY = v.Y;
		}

		minX++; minX *= 8;
		minY++; minY *= 8;
		maxX *= 8;
		maxY *= 8;


		return new(minX, minY, maxX, maxY);
	}
}
