using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipFX : MonoBehaviour
{
    private GameObject landingFxPrefab = null!;
    private GameObject liftoffFxPrefab = null!;
    private Vector3 spawnPosition;

    public void InitFX(GameObject _landingFxPrefab, GameObject _liftoffFxPrefab, Transform _liftoffPosition)
    {
        landingFxPrefab = _landingFxPrefab;
        liftoffFxPrefab = _liftoffFxPrefab;
        spawnPosition = _liftoffPosition.position + new Vector3(0, -1.42f, 0);

    }

    public void Landing()
    {
        GameObject effect = Instantiate<GameObject>(landingFxPrefab, spawnPosition, Quaternion.Euler(-90, 0, 0));
        Destroy(effect, 8f);
    }

    public void Liftoff()
    {
        GameObject effect = Instantiate<GameObject>(liftoffFxPrefab, spawnPosition, Quaternion.Euler(-90, 0, 0));
        Destroy(effect, 8f);
    }
}
