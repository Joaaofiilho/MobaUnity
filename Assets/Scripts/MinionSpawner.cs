using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private Lanes spawnLane;
    [SerializeField] private Teams minionTeam;
    
    [SerializeField] private float waveCooldown = 20f;

    private float _waveCounter;

    [SerializeField] private float minionSpawnIntervalCooldown = 1f;
    
    private float _minionSpawnIntervalCounter;

    [SerializeField] private GameObject minionPrefab;

    [SerializeField] private int qttMinionPerWave = 6;

    private int _qttMinionPerWaveCounter;
    
    private void Start()
    {
        _waveCounter = waveCooldown;
        _minionSpawnIntervalCounter = minionSpawnIntervalCooldown;
    }

    private void Update()
    {
        if (_waveCounter >= waveCooldown)
        {
            bool hasMinionsToSpawn = _qttMinionPerWaveCounter < qttMinionPerWave;
            
            if (hasMinionsToSpawn)
            {
                bool shouldSpawnMinion = _minionSpawnIntervalCounter >= minionSpawnIntervalCooldown;
                
                if (shouldSpawnMinion)
                {
                    var minionPfb = Instantiate(minionPrefab, transform);
                    var minion = minionPfb.GetComponent<Minion>();
                    minion.team = minionTeam;
                    minion.destinations = GameUtils.GetMinionCustomDestinationsByLane(minionTeam, spawnLane);
                     
                    _minionSpawnIntervalCounter = 0;
                    _qttMinionPerWaveCounter++;
                }
                else
                {
                    _minionSpawnIntervalCounter += Time.deltaTime;
                }
            }
            else
            {
                _waveCounter = 0;
                _qttMinionPerWaveCounter = 0;
            }
        }
        else
        {
            _waveCounter += Time.deltaTime;
        }
    }
}
