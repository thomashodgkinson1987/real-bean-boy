using Godot;
using Godot.Collections;

public partial class LevelSection : Node2D
{
	private TileMapLayer skyLayer;

	public override void _Ready()
	{
		skyLayer = GetNode<TileMapLayer>("Sky");
	}

	public Rect2I GetBounds()
	{
		Array<Vector2I> tiles = skyLayer.GetUsedCells();

		int minX = 0;
		int minY = 0;
		int maxX = 0;
		int maxY = 0;

		foreach (Vector2I v in tiles)
		{
			if (v.X * 8 < minX) minX = v.X;
			if (v.X * 8 > maxX) maxX = v.X;
			if (v.Y * 8 < minY) minY = v.Y;
			if (v.Y * 8 > maxY) maxY = v.Y;
		}

		minX *= 8;
		minY *= 8;
		maxX++; maxX *= 8;
		maxY++; maxY *= 8;


		return new(minX, minY, maxX, maxY);
	}
}
