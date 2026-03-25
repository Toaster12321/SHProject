using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRevolver : MonoBehaviour
{
    public float damage;
    public float bulletRange; //how far bullet travels
    private Transform playerCamera;
    //[SerializeField] GameObject crosshair;

    private void Start()
    {

        playerCamera = Camera.main.transform; //get camera's current spot
    }


    public void Shoot()
    {
        Ray gunRay = new Ray( playerCamera.position, playerCamera.forward ); //raycast pointing from camera forwards
        if (Physics.Raycast( gunRay, out RaycastHit hitinfo, bulletRange ) ) //if the raycast collides
        {
            if ( hitinfo.collider.gameObject.TryGetComponent( out EnemyStateMachine enemy ) ) //if we hit an object with the enemy AI script
            {
                //enemy.GetSetHealth -= damage; //apply damage
                //StartCoroutine(CrosshairFlash());
            }
        }

    }

    //IEnumerator CrosshairFlash() //coroutine to show the crosshair hit effect for 0.1s on enemy hit
    //{
    //    crosshair.SetActive( true );
    //    if (crosshair.activeSelf) //if crosshair is active 
    //    {
    //        yield return new WaitForSeconds( 0.1f );
    //        crosshair.SetActive( false );
    //    }
    //}
        
                
}
