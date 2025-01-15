using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    //Spawnear cada 2 segundos un zombie aleatorio entre distintos puntos de spawn
    [SerializeField] private Transform[] puntosSpawn;
    [SerializeField] private Enemigo enemigoPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawnear());
    }


    private IEnumerator Spawnear()
    {
        while (true)
        {
            Enemigo enemigoCopia = Instantiate(enemigoPrefab, puntosSpawn[Random.Range(0, puntosSpawn.Length)].position, Quaternion.identity);
            yield return new WaitForSeconds(1.2f);
        }
    }
}
