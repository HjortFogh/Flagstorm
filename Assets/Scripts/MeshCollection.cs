using System.Collections.Generic;
using UnityEngine;

public class MeshCollection : MonoBehaviour
{
    [System.Serializable]
    private struct MeshNamePair
    {
        public string identifier;
        public Mesh mesh;
    }

    [SerializeField]
    private List<MeshNamePair> m_Pairs;

    public static MeshCollection Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public static Mesh RequestMesh(string identifier)
    {
        foreach (MeshNamePair pair in Instance.m_Pairs)
        {
            if (pair.identifier == identifier)
                return pair.mesh;
        }

        return null;
    }
}
