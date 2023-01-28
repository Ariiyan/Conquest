using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondPickup : MonoBehaviour
{

    [SerializeField] int diamondValue = 100;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FindObjectOfType<GameSession>().AddToScore(diamondValue);
        Destroy(gameObject);
    }
}
