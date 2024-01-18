using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    
    private void Start(){
        // increase animation speed by value
        var animator = GetComponent<Animator>();
        animator.speed = 5f;
    }

    public void PurchaseGood(){
        var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        float random = Random.Range(0f, 1f);
        if (random <= gameManager.preferenceForPrimaryGood){
            gameManager.NPCBuyGood("banana");
        }
        else{
            gameManager.NPCBuyGood("apple");
        }
    }

    void OnBecameInvisible(){
        Destroy(gameObject);
    }
}
