using Godot;
using System;

public partial class Manager : Node
{
  static private Manager _instace = null;
  static public Manager m_instace
  {
    get
    {
      if (_instace == null)
      {
        _instace = new Manager();
      }
      return _instace;
    }
  }

  public PartitionGrid m_grid = new PartitionGrid(36, 36, 19, 1, 1);

  //public Player player;

}
