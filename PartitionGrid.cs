using Godot;
using System.Collections;
using System.Collections.Generic;



public partial class PartitionGrid : GodotObject
{
  private const int maxNum = 4;

  private const int maxIndex = maxNum - 1;
  public int m_width;
  public int m_height;
  public float m_worldWidth;
  public float m_worldHeight;
  public int m_currentParticles;
  public List<int> m_counts;
  public List<int> m_data;
  public List<Vector2> m_positions;
  public float m_separation;
  public float m_separationHalf;
  public float m_sqrdSeparation;
  public float m_separationInv;
  public int m_solverTimes;
  public float m_solverTimesInv;

  public PartitionGrid(int width, int height, int particles, float separation, int solverTimes)
  {
	m_width = width;
	m_height = height;
	m_worldWidth = m_width * separation;
	m_worldHeight = m_height * separation;
	m_currentParticles = 0;
	m_counts = new List<int>(new int[m_width * m_height]);
	m_data = new List<int>(new int[m_width * m_height * 4]);
	m_positions = new List<Vector2>(new Vector2[particles]);
  m_separation = separation;
	m_separationHalf = separation * .5f;
	m_sqrdSeparation = separation * separation;
	m_separationInv = 1f / separation;
	m_solverTimes = solverTimes;
	m_solverTimesInv = 1f / solverTimes;
  }


  public void AddAtom(Vector2 position)
  {
	m_positions[m_currentParticles] = position;
	m_currentParticles++;
  }

  public void checkForAtoms(int atom, int x, int y)
  {
	checkForAtomsInCell(atom, x, y);
	checkForAtomsInCell(atom, x + 1, y);
	checkForAtomsInCell(atom, x - 1, y);
	checkForAtomsInCell(atom, x, y + 1);
	checkForAtomsInCell(atom, x, y - 1);
	checkForAtomsInCell(atom, x + 1, y + 1);
	checkForAtomsInCell(atom, x + 1, y - 1);
	checkForAtomsInCell(atom, x - 1, y + 1);
	checkForAtomsInCell(atom, x - 1, y - 1);
  }

  public void checkForAtomsInCell(int atom, int x, int y)
  {
	if (checkCoords(x, y, out int index))
	{
	  for (int i = 0; i < m_counts[index]; ++i)//a
	  {
		checkCollicion(atom, m_data[index * 4 + i]);
	  }
	}
  }

  public void checkForAtomsAndWallInCell(int atom, int x, int y, int dirx, int diry)
  {
	if (checkCoords(x, y, out int index))
	{
	  for (int i = 0; i < m_counts[index]; ++i)//a
	  {
		checkCollicion(atom, m_data[index * 4 + i]);
	  }
	}
	else
	{
	  var m_currentPos = m_positions[atom];
	  if (dirx != 0)
	  {
		m_currentPos.X = (x + .5f) * m_separation;
	  }
	  if (diry != 0)
	  {
		m_currentPos.Y = (y + .5f) * m_separation;
	  }
	  m_positions[atom] = m_currentPos;
	}
  }

  void checkCollicion(int id1, int id2)
  {
	Vector2 delta = m_positions[id1] - m_positions[id2];
	float distSQrd = delta.LengthSquared();
	if (distSQrd < m_sqrdSeparation)
	{
	  float dist = Mathf.Sqrt(distSQrd);
	  var overlap = (m_separation - dist) * .5f;
	  var finalVector = delta / dist * overlap;
	  m_positions[id1] += finalVector * m_solverTimesInv;
	  m_positions[id2] -= finalVector * m_solverTimesInv;
	}
  }


  public bool checkCoords(int x, int y, out int index)
  {
	index = y * m_width + x;
	return x >= 0 && y >= 0 && x < m_width && y < m_height;
  }

  public void solveCollicions()
  {
	for (int i = 0; i < m_solverTimes; ++i)
	{
	  for (int atom = 0; atom < m_currentParticles; ++atom)
	  {
		var position = m_positions[atom];
		int x = Mathf.FloorToInt(position.X * m_separationInv);
		int y = Mathf.FloorToInt(position.Y * m_separationInv);
		checkForAtoms(atom, x, y);
		var index = y * m_width + x;
		var count = m_counts[index];
		if (count != maxNum)
		{
		  m_data[index * 4 + count] = atom;
		  m_counts[index]++;
		}
		if (m_positions[atom].X > m_worldWidth - m_separationHalf)
		{
		  m_positions[atom] = new Vector2(m_worldWidth - m_separationHalf, m_positions[atom].Y);
		}
		else if (m_positions[atom].X < m_separationHalf)
		{
		  m_positions[atom] = new Vector2(m_separationHalf, m_positions[atom].Y);
		}
		if (m_positions[atom].Y > m_worldHeight - m_separationHalf)
		{
		  m_positions[atom] = new Vector2(m_positions[atom].X, m_worldHeight - m_separationHalf);
		}
		else if (m_positions[atom].Y < m_separationHalf)
		{
		  m_positions[atom] = new Vector2(m_positions[atom].X, m_separationHalf);
		}

	  }
	  for (int j = 0; j < m_counts.Count; j++)
	  {
		m_counts[j] = 0;
	  }
	}

  }

  public void Clear()
  {
	for (int i = 0; i < m_counts.Count; i++)
	{
	  m_counts[i] = 0;
	}
	m_currentParticles = 0;
  }



//#if GODOT_EDITOR
//  public void print()
//  {
//	for(int i = 0; i< m_height; i++)
//	{
//	  for (int j = 0; j < m_width; j++)
//	  {
//			Debug.Log(i.ToString()+" "+j+" "+m_counts[i * m_width + j].ToString());
//	  }
//	}
//  }
//#endif
}
