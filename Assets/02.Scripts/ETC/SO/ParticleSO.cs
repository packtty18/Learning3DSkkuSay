using UnityEngine;

[CreateAssetMenu(fileName = "ParticleSO", menuName = "ScriptableObjects/ParticleSO")]
public class ParticleSO : ScriptableObject
{
    public EParticleType type;
    public GameObject particlePrefab;
}
