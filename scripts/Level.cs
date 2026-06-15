using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Level : Node2D
{
	private Marker2D spawnPoint;
	private Node2D sectionsNode;
	private List<LevelSection> sections;

	public override void _Ready()
	{
		spawnPoint = GetNode<Marker2D>("SpawnPoint");
		sectionsNode = GetNode<Node2D>("Sections");

		sections = new List<LevelSection>();
		sections.AddRange(sectionsNode.GetChildren().Cast<LevelSection>());
	}

	public Marker2D GetSpawnPoint() => spawnPoint;

	public List<LevelSection> GetSections() => sections;
}
