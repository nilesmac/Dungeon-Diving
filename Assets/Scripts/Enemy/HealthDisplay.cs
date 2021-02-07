using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    public Transform healthBorder;

    Vector3 localScale;
    private Enemy enemy;
    public bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        healthBorder.gameObject.SetActive(false);
        localScale = transform.localScale;
        enemy = GetComponentInParent<Enemy>();
        //healthBorder.x = enemy.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy == true)
            healthBorder.gameObject.SetActive(true);
        localScale.x = enemy.currentHealth;
        transform.localScale = localScale;
    }
}
