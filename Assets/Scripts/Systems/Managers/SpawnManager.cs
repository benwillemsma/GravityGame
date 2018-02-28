using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public enum SpawnType
    {
        RoundRobin,
        Random,
        Index
    }

    public SpawnType m_type;
    public int m_nextIndex;
    public Transform[] m_spawnLocations;

    private void Awake()
    {
        if (GameManager.Instance)
            GameManager.levelSpawn = this;
    }

    public Transform GetNextSpawn()
    {
        Transform nextSpawn = null;
        if (m_spawnLocations.Length > 0)
        {
            switch (m_type)
            {
                case SpawnType.RoundRobin:
                    nextSpawn = m_spawnLocations[m_nextIndex];
                    m_nextIndex++;
                    break;
                case SpawnType.Random:
                    nextSpawn = m_spawnLocations[Random.Range(0, m_spawnLocations.Length - 1)];
                    break;
                case SpawnType.Index:
                    nextSpawn = m_spawnLocations[m_nextIndex];
                    break;
                default:
                    nextSpawn = transform;
                    break;
            }
        }
        else nextSpawn = transform;
        return nextSpawn;
    }
}
