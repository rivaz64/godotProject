using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CollicionSystem : Node
{
  IEnumerable<Node3D> nodes;
  PartitionGrid grid = Manager.m_instace.m_grid;
  public override void _Ready()
  {
    nodes = GetChildren()
        .Where(child => child is Node3D) // We only want nodes that we know are Weapon nodes
        .Select(child => child)          // Once we've got the weapons select them all
        .Cast<Node3D>();
  }
  public override void _PhysicsProcess(double delta)
  {
    foreach (var node in nodes)
    {
      grid.AddAtom(new Vector2(node.Position.X, node.Position.Z));
    }
    grid.solveCollicions();
    int i = 0;
    foreach (var node in nodes)
    {
      node.Position = new Vector3(grid.m_positions[i].X,1, grid.m_positions[i].Y);
        ++i;
    }
    grid.Clear();
  }
}
