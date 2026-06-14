using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Level : Node2D
{
	private Node2D sectionsNode;
	private List<LevelSection> sections;

	public override void _Ready()
	{
		sectionsNode = GetNode<Node2D>("Sections");

		sections = new List<LevelSection>();
		sections.AddRange(sectionsNode.GetChildren().Cast<LevelSection>());
	}

	public List<LevelSection> GetSections()
	{
		return sections;
	}
}
