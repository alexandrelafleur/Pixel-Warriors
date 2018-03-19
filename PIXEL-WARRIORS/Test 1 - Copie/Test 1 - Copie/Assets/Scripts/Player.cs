﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed = 2.5f;
    public float speed = 15f;
    public float jumpPower = 175f;
    public float maxJump = 2f;
    public float percentage = 0f;
    public int lives = 3;

    public bool grounded;
    public bool attack_1;
    public bool charge;
    public bool goingDown;
    public bool dead;
	private bool shootCharge;

	//controls
	public KeyCode up = KeyCode.W;
    public KeyCode left = KeyCode.A;
    public KeyCode down = KeyCode.S;
    public KeyCode right = KeyCode.D;
    public KeyCode attack1 = KeyCode.R;
    public KeyCode attack2 = KeyCode.F;
    public KeyCode attack3 = KeyCode.C;

    public bool aiON = true;
    public float distance;
    public int x = 0;


    public Vector3 initialPosition = new Vector3(-2, 1.6f, 0);

    private bool isRight;
    private bool isDead;
    private float stun = 0f;

    private Rigidbody2D rb2d;
    private Animator anim;
    private Player player;

    private Vector2 pos;
    private Vector2 knockback;

    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        player = gameObject.GetComponentInParent<Player>();

        //Solution temporaire. À changer selon la direction de l'attaque de l'autre joueur
        knockback.Set(-2, 0);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Hit by an ennemy projectile
        if ((player.tag == "Player 1" && col.gameObject.tag == "Ball2") || (player.tag == "Player 2" && col.gameObject.tag == "Ball1"))
        {
            Destroy(col.gameObject);
            this.ReceiveDamage(10, 0.75f);
            //rb2d.AddForce(col.rigidbody.velocity * percentage, ForceMode2D.Impulse);
            //percentage += 0.75f;
            //stun = 10 + (.5f * percentage);
            //Debug.Log("pourcentage P1: " + percentage);
        }

        //Hit by melee
        if (col.gameObject.tag == "Melee2")
        {
            //player.transform.position = pos;
            Destroy(col.gameObject);
            this.ReceiveDamage(10, 0.75f);

        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        player.grounded = true;
        maxJump = 2;
        maxSpeed = 2.5f;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        player.grounded = true;
        maxJump = 2;
        maxSpeed = 2.5f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player.grounded = false;
    }

    void Update()
    {
        anim.SetBool("Grounded", grounded);
        anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));
        anim.SetBool("Attack", attack_1);
        anim.SetBool("Charge", charge);
        anim.SetBool("GoingDown", goingDown);
        anim.SetBool("Dead", dead);

        //Flip character L/R
        if (isRight == false)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (isRight == true)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        //Double jump
        if (Input.GetKeyDown(up))
        {
            MoveUp();
        }

        //Attack 1
        if (Input.GetKeyDown(attack1))
        {
            Attack1();
        }
        else { attack_1 = false; }

        //Attack 2
        if (Input.GetKeyDown(attack2)) { Attack2(true); }
        else if(Input.GetKeyUp(attack2)){ Attack2(false); }

        //Gauche/Droite
        if ((Input.GetKey(left)) && rb2d.velocity.x > -maxSpeed) { MoveLeft(); }
        else if ((Input.GetKey(right)) && rb2d.velocity.x < maxSpeed) { MoveRight(); }
        else { x = 0; }


    }


    private void FixedUpdate()
    {

        if (aiON) AIUpdate();

        float h = Input.GetAxisRaw("Horizontal");
        float decay = 0.8f;

        pos = transform.position;

        //Out of map
        if (rb2d.transform.position.y < -1 || rb2d.transform.position.y > 3.2 || rb2d.transform.position.x > 2.7 || rb2d.transform.position.x < -4.5)
        {
            player.isDead = true;
            lives--;
        }
        if (player.isDead == true)
        {
            if (lives == 0)
            {
                Destroy(player);
            }
            else
            {
                this.Reset();
            }
        }

        //Going down
        if (rb2d.velocity.y < 0) { player.goingDown = true; }
        else { player.goingDown = false; }

        //Move player
        if (stun < 1)
        { rb2d.AddForce(Vector2.right * x * 10 * speed, ForceMode2D.Force); }
        else { stun--; }
        rb2d.velocity = new Vector2(rb2d.velocity.x * decay, rb2d.velocity.y);

    }


    //MOVES

    private void MoveUp()
    {
        if (maxJump > 0)
        {
            maxSpeed = 2f; //ENLEVER?
            Vector2 temp = rb2d.velocity;
            temp.y = 0;
            rb2d.velocity = new Vector2(temp.x, temp.y);
            rb2d.AddForce(new Vector2(0, jumpPower));
            maxJump--;

        }

    }
    private void MoveLeft()
    {
        x = -1; isRight = false;
    }
    private void MoveRight()
    {
        x = 1; isRight = true;
    }
    private void MoveDown()
    {

    }

    private void Attack1()
    {
		attack_1 = true;
	}
    private void Attack2(bool state)
    {
		if (state)
		{
			shootCharge = true;
			player.GetComponent<Shoot>().animate();
		}
		else if(shootCharge)
		{
			shootCharge = false;
			player.GetComponent<Shoot>().shoot();
		}
	}
    private void Attack3()
    {

    }
    private void ReceiveDamage(int stunReceived, float damage)
    {
        int dir = 0;
        if (isRight) dir = 1;
        else dir = -1;
        rb2d.AddForce(knockback * dir * percentage, ForceMode2D.Impulse);
        percentage += damage;
        stun = stunReceived + (percentage);
        Debug.Log("pourcentage P1: " + percentage);
    }

    private void Reset()
    {
        player.dead = true;
        percentage = 0;
        player.transform.position = initialPosition;
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        if (Input.GetKey(up) || Input.GetKey(down))
        {
            rb2d.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            player.isDead = false;
            player.dead = false;
        }
    }





    public void buttonJumpPointerDown()
    {
        rb2d.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        player.isDead = false;
        player.dead = false;

        this.MoveUp();


    }


    //Turcotte AI
    private void AIUpdate()
    {

        if (player.tag == "Player 2")
        {
           // Debug.Log("2");

            //Debug.Log("YOUHHOUH");
            List<Transform> trous = new List<Transform>();

            Transform autre = GameObject.FindGameObjectWithTag("Player 1").transform;
            distance = autre.position.x - this.transform.position.x;
            //isRight = distance > 0;

            trous.Add(GameObject.FindGameObjectWithTag("Hole 1").transform);
            //trous.Add(GameObject.FindGameObjectWithTag("Hole2").transform);
            List<float> distanceTrousX = new List<float>();
            List<float> distanceTrousY = new List<float>();

            foreach (Transform t in trous)
            {
                distanceTrousX.Add(t.position.x - this.transform.position.x);
                distanceTrousY.Add(t.position.y - this.transform.position.y);

            }

            //Debug.Log("distance Joueur" + distance);
            //Debug.Log("x " + x);
            //Debug.Log("distanceTrou " + distanceTrousX);
            //Debug.Log("test " + (distanceTrousX * x)); 
            bool saute = false;
            foreach (float d in distanceTrousX)
            {
                //Debug.Log("distanceTrou " + d);
                saute = (d * x > 0 && d * x < 0.7);
                if (saute)
                {
                    this.MoveUp();
                    break;
                }

            }
            
            bool avance = false;
            int trou = 0;
            for (int i = 0; i < distanceTrousX.Count; i++)
            {
                avance = Mathf.Abs(distanceTrousX[i]) < 0.8;
                if (avance)
                {
                    trou = i;
                    break;
                }
            }
            if (avance)
            {
                //Over Edge
                //Debug.Log("y vel = " + rb2d.velocity.y );

                //si il est plus bas que le trou
                if (rb2d.velocity.y < -4.5)
                {
                    player.MoveUp();
                }

                if (x == 0)
                {
                    Debug.Log("Avance");
                    //if (isRight) { this.MoveLeft(); } //////////////////////////////////À REMETTRE
                    //else { this.MoveRight(); }
                }

            }
            else if (Mathf.Abs(distance) > 0.5)
            {
                Debug.Log("Avance vers joueur");
                //if (isRight) { this.MoveLeft(); }
                // else { this.MoveRight(); }

                //Debug.Log("distance élevée");
            }
            else if (Mathf.Abs(distance) > 0.1)
            {
                x = 0;

                //Debug.Log("distance faible: " + distance);
            }

        }
    }



    //Boutons Will
    public void buttonAttackAPointerDown()
    {
		this.Attack1();
    }

    public void buttonAttackBPointerDown()
    {
		this.Attack2(true);
    }

    public void buttonAttackBPointerUp()
    {
		Attack2(false);
    }

    public void buttonLeftPointerDown()
    {
		this.MoveLeft();
    }

    public void buttonRightPointerDown()
    {
		this.MoveRight();
    }

    public void buttonDownPointerDown()
    {
		this.MoveDown();
    }

    public void buttonUpPointerDown()
    {
        this.MoveUp();
    }

}
