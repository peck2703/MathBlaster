﻿using UnityEngine;
using System;
using System.Collections;


public class AsteroidMovement : MonoBehaviour
{
    public GameObject explosion;    //Explosion prefab.
    GameObject asteroidManager; //Used to reference the AsteroidManager gameobject.
    public float speed; //The speed at which the asteroid will move.
    Rect windowSize;
    //bool that identifies if the asteroid is inside the screen "Rectangle"
    public bool insideRect;
    //Stores the int to re-add the equation to the original list to re-use if possible
    int equationIndex;
    int inQuadNum;

    GameObject player;      //Reference to the player.
    ObjectPooling objectPooling;    //Reference to the pooling script
    GameObject clone;   //Clone to set the explosion to.
    void Start()
    {
        asteroidManager = GameObject.Find("AsteroidManager");   //Reference the Asteroid Manager gameobject.
        objectPooling = asteroidManager.GetComponent<ObjectPooling>();
        player = GameObject.Find("Player");
        //Creates a rectangle that identifies the screen height / width
        windowSize = new Rect(0, 0, Screen.width, Screen.height);
        //Spawn points start outside the screen, so starts out as false
        insideRect = false;

    }
	void Update ()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed); //Moves the asteroid to the left.
        //Spawned asteroids collide with screen bounds (checks only once)
        if (!insideRect && windowSize.Contains(transform.position))
        {
            insideRect = true;
            //WriteEquation();
        }
        else if (insideRect && !windowSize.Contains(transform.position))
        {
            insideRect = false;
            switch(inQuadNum)   //Tells the asteroid manager what lane it is in and lets it know it is done using that lane.
            {
                case 1:
                    objectPooling.quad1InUse = false;
                    break;
                case 2:
                    objectPooling.quad2InUse = false;
                    break;
                case 3:
                    objectPooling.quad3InUse = false;
                    break;
                case 4:
                    objectPooling.quad4InUse = false;
                    break;
            }
            objectPooling.objectCount--;    //Reduces the count of active objects in the scene.
            gameObject.SetActive(false);    //Turns off the gameobject.
        }
    }
    void WriteEquation()
    {
        equationIndex = Camera.main.GetComponent<EquationWindow>().ListEquations();
        gameObject.GetComponent<GUIText>().text = Camera.main.GetComponent<EquationWindow>().WriteEquation(equationIndex);
    }
    public void QuadNum(int quadNum)
    {
        inQuadNum = quadNum;    //Defines the lane the object is in.
    }
    void OnTriggerEnter2D(Collider2D other) //If you collide with a meteor activate the explosion sprite and destoy the missile and meteor.
    {
        if (other.tag == "Rocket")
        {
            clone = Instantiate(explosion, GetComponent<RectTransform>().position, GetComponent<RectTransform>().rotation) as GameObject; //Spawn the explosion prefab at the meteor.
            clone.transform.SetParent(player.GetComponent<Shooting>().canvas.transform);    //Child the explosion to the canvas so that it gets rendered.
            Destroy(other.gameObject);
            gameObject.SetActive(false);
        }
    }
}
