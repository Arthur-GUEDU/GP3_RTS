using System.Collections.Generic;
using UnityEngine;

public class Formation
{
    public virtual List<Vector3> GetFormationPositions(int _unitCount, Quaternion _orientation)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < _unitCount; ++i)
            positions.Add(Vector3.zero);
        return positions;
    }
}