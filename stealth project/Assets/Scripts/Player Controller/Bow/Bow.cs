using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;
    public float maxShootSpeed = 30f;
    public float shootSpeed = 10f;
    public Transform shotPoint;

    public float maxChargeTime = 0.5f;
    public float currentChargeTime = 0;

    private bool buttonDown = false;
    private Collider2D hitCollider;


    // Prediction path
    [Header("Prediction Path")]
    public GameObject point;
    public GameObject pathIndicator;
    GameObject[] points;
    public int numberOfPoints;
    public float spaceBetweenPoints;
    Vector2 direction;

    void Start()
    {
        points = new GameObject[numberOfPoints];
        for(int i = 0; i < numberOfPoints; i++)
        {
            points[i] = Instantiate(point, shotPoint.position, Quaternion.identity, pathIndicator.transform);
        }
    }

    void Update()
    {
        Vector2 bowPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = (mousePosition - bowPosition).normalized;
        transform.right = direction;

        if (Input.GetMouseButtonDown(0))
        {
            buttonDown = true;
            //pathIndicator.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (currentChargeTime >= maxChargeTime)
                Shoot();
            pathIndicator.SetActive(false);
            currentChargeTime = 0;
            shootSpeed = 0;
            buttonDown = false;
        }

        if (buttonDown)
        {
            if (currentChargeTime < maxChargeTime)
                currentChargeTime += Time.deltaTime;
            else if (currentChargeTime > maxChargeTime)
                currentChargeTime = maxChargeTime;

            shootSpeed = maxShootSpeed * (currentChargeTime / maxChargeTime);
        }


        if (pathIndicator.active)
        {
            bool wallHit = false;
            int wallHitIndex = 0;
            ShotIndicatorCollision collider;
            for (int i = 0; i < numberOfPoints; i++)
            {
                if (!wallHit)
                {
                    collider = points[i].GetComponent<ShotIndicatorCollision>();

                    if (collider.isColliding)
                    {
                        Debug.Log("hit");

                        wallHit = true;
                        wallHitIndex = i;
                    }
                    if(!wallHit)
                        points[i].transform.position = PointPosition(i * spaceBetweenPoints);
                }

                else if(wallHit)
                    points[i].transform.position = PointPosition(wallHitIndex * spaceBetweenPoints);
            }
        }
        

    }

    private void Shoot()
    {
        GameObject newArrow = Instantiate(arrow, shotPoint.position, shotPoint.rotation);
        newArrow.GetComponent<Rigidbody2D>().velocity = transform.right * shootSpeed;
    }


    Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)shotPoint.position 
                                    + (direction * shootSpeed * t) 
                                    + (0.5f * Physics2D.gravity) 
                                    * (t * t);
        return position;
    }
}
