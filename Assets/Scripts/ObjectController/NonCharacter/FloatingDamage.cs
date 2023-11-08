using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingDamage : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    private float destroyTime;
    private TextMeshPro text;
    private Color alpha;

    private void Awake()
    {
        moveSpeed = 1.5f;
        alphaSpeed = 5f;
        destroyTime = 1f;

        text = GetComponent<TextMeshPro>();
        alpha = text.color;
    }

    void Start()
    {
        Invoke("DestroyObject", destroyTime);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    public void SetText(float dmg)
    {
        text.text = ((int)dmg).ToString();
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}