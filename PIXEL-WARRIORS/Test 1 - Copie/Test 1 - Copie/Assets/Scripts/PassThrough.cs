﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThrough : MonoBehaviour {

    private Collider2D platform;
    //private Rigidbody2D rb2d;


    void Start () {
  
        platform = gameObject.GetComponent<Collider2D>();
        //rb2d = gameObject.GetComponent<Rigidbody2D>();

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player 1" && Input.GetKey(col.gameObject.GetComponent<Player>().down))
        {
            platform.GetComponent<Collider2D>().enabled = false;
            //platform.GetComponent<Collider2D>().isTrigger = true;
            StopCoroutine("Wait");
            StartCoroutine("Wait");
        }

      
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player 1" && Input.GetKey(col.gameObject.GetComponent<Player>().down))
        {
            platform.GetComponent<Collider2D>().enabled = false;
            //platform.GetComponent<Collider2D>().isTrigger = true;
            StopCoroutine("Wait");
            StartCoroutine("Wait");
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(.5f);
        platform.GetComponent<Collider2D>().enabled = true;
        //platform.GetComponent<Collider2D>().isTrigger = false;
    }
}
