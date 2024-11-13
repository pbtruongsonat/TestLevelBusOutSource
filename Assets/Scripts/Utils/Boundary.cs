using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    [SerializeField] private List<Transform> nodePathBoundary;

    public List<Transform> NodePathBoundary => nodePathBoundary;
}
