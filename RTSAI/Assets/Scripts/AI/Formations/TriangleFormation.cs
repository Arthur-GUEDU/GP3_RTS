using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class TriangleFormation : Formation
{
    [SerializeField] private float m_spacing = 4f;

    public override List<Vector3> GetFormationPositions(int _unitCount, Quaternion _orientation)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < _unitCount; ++i)
        {
            float row = Mathf.Floor((Mathf.Sqrt(8 * i + 1) - 1) / 2);
            float col = i - (row * (row + 1)) / 2;
            float xOffset = col - row / 2f;
            float zOffset = -row;
            Vector3 localPosition = new Vector3(xOffset, 0, zOffset) * m_spacing;
            Vector3 worldPosition = _orientation * localPosition;
            positions.Add(worldPosition);
        }
        return positions;
    }
}