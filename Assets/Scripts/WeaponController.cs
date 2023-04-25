using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public enum EnumLayerMask
{
    Player = 6, Monster = 3, Mineral = 8
};




public class WeaponController : MonoBehaviour
{
    [SerializeField] Sprite swordSprite;
     [SerializeField] Sprite axeSprite;
    public float attacCoefficient;
    
    private Player parentPlayerComponent;
    private IWeapon weaponController;
    [SerializeField] private LayerMask targetLayerMask;

    private IWeapon meleeWeaponController;
    private IWeapon pickaxWeaponController;

    private GameObject playerGameObject;
    private SpriteRenderer spriteRenderer;
    [SerializeField]private float disFromBody; 
    private void Awake() {
       
    }
    private void Start() {
        meleeWeaponController = new MeleeWeaponController();
        pickaxWeaponController = new PickaxWeaponController();

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        playerGameObject = transform.root.gameObject;

        weaponController = meleeWeaponController;
    }

    private void Update() {
          //Get the Screen positions of the object
         Vector3 positionOnScreen = playerGameObject.transform.position;
         
         //Get the Screen position of the mouse
         Vector3 mouseOnScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         
         //Get the angle between the points
         float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen) + 90.0f; //90도 보정
 
         //Rotate the object to face the mouse
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        Debug.Log(angle);

        transform.position = new Vector3(-disFromBody * Mathf.Sin(angle * Mathf.Deg2Rad), disFromBody* Mathf.Cos(angle* Mathf.Deg2Rad),0) + transform.root.position;

         float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
         return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        
    }
   

    private void OnTriggerEnter2D(Collider2D other) {

        if(other == null) return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable == null) return;
        
        if(IsInLayerMask(other.gameObject))
        {
            weaponController.Attack(damageable);
        }

    }
    

    //데미지 대상 레이어 마스크 함수 3개
    private bool IsInLayerMask(GameObject go)
    {
        return ((targetLayerMask.value & (1<<go.layer))>0);
    }

    private void AddLayerMask(int layerNum)
    {

    }
    private void RemoveLayerMask(int layerNum)
    {
        
    }

    public void ChangeWeapon(InputAction.CallbackContext context)
    {
       
        if(context.ReadValue<float>() > 0 && context.started)
        {
         Debug.Log("wheel up");
                spriteRenderer.sprite = axeSprite;
                spriteRenderer.sortingOrder = 3;
            
            
        }
        else if(context.ReadValue<float>() < 0 && context.started)
        {
              Debug.Log("wheel down");
                spriteRenderer.sprite = swordSprite;
                spriteRenderer.sortingOrder = 3;
                
        }
    }

}
