using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public GameObject[] BlocksPrefab;

    private List<GameObject> Blocks;

    // Start is called before the first frame update
    void Start()
    {
        Load_Blocks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Load_Blocks()
    {
        for (int i = 0; i < 50; i++)
        {
            GameObject instance = Instantiate(BlocksPrefab[0], transform);
            instance.transform.position = Vector3.forward * 50 * i;
        }
    }
}
