using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCollider : MonoBehaviour
{
    private InventoryManager inventoryManager;
    [SerializeField] private Item ironIngot;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
        gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        Invoke("EnableCollider", 0.5f);
    }

    public void EnableCollider()
    {
        gameObject.GetComponent<PolygonCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inventoryManager.AddItem(ironIngot);
            Destroy(gameObject);
        }
    }
}
