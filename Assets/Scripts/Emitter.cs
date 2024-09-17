using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{

	[SerializeField] private Transform SpawnTransform;
	[SerializeField] private GameObject SpawnPrefub;
	[SerializeField] private float SpawnRute = 1.0f;
    [SerializeField] private float MaxParticles = 3;
    [SerializeField] private Vector2 SlizeRange = new Vector2(0.0f, 1.0f);

    private GameObject[] pool;
	private GameObject poolParent;
    // Start is called before the first frame update
    void Start()
    {
		InitializePool();
		Spawn();
	}

	private void InitializePool()
	{
		poolParent = new GameObject("poolParent");
		pool = new GameObject[(int)MaxParticles];
		for (var i = 0; i < MaxParticles; i++)
		{
			if (pool[i] == null)
			{
				pool[i] = Instantiate(SpawnPrefub, transform.position, Quaternion.identity);
				pool[i].SetActive(false);
				
				pool[i].name = pool[i].name + i;
				pool[i].transform.parent = poolParent.transform;
			}
		}

	}
	private void Spawn()
	{
		foreach (var partivle in pool)
		{
            if (!partivle.activeSelf)
            {
                partivle.transform.position = SpawnTransform.TransformPoint( Random.insideUnitSphere * 0.5f);
				partivle.transform.localScale = Random.Range(SlizeRange.x, SlizeRange.y) * Vector3.one;
				partivle.SetActive(true);

				//F‚ðƒ‰ƒ“ƒ_ƒ€‚ÉÝ’è‚·‚é
				var renderer = partivle.GetComponent<Renderer>();
				if (renderer != null)
				{
					renderer.material.color = new Color(Random.value, Random.value, Random.value);
				}
				break;
            }
        }
		Invoke("Spawn", SpawnRute);
	}
	// Update is called once per frame
	void Update()
    {
    }
}
