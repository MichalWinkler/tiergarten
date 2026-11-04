using UnityEngine;
using UnityEngine.InputSystem;

public class pet : MonoBehaviour
{
    Vector3 movement;
    [SerializeField]
    int hunger;
    [SerializeField]
    int thirst;
    [SerializeField]
    GameObject food;
    [SerializeField]
    GameObject water;
    [SerializeField]
    GameObject carrot;
    [SerializeField]
    GameObject water_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carrot.SetActive(false);
        water_.SetActive(false);
        movement = new Vector3(0.1f, 0f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        //this.gameObject.transform.position += movement;
        hunger -= 1;
        thirst -= 1;    
       if(hunger <= 150)
         {
            carrot.SetActive(true);
            var heading = food.transform.position - this.gameObject.transform.position;
             var distance = heading.magnitude;
             if(distance < 2f)
             {
                 hunger += 1000;
                 carrot.SetActive(false);
             }
             /*else
             {
                 var direction = heading / distance;
                 this.gameObject.transform.position += Vector3.Scale(movement, direction);
             }*/
         }

        if (thirst <= 150)
        {
            water_.SetActive(true);
            var heading = water.transform.position - this.gameObject.transform.position;
            var distance = heading.magnitude;
            if (distance < 1.5f)
            {
                thirst += 1500;
                water_.SetActive(false);
            }
            /*else
            {
                var direction = heading / distance;
                this.gameObject.transform.position += Vector3.Scale(movement, direction);
            }*/
        }
        this.gameObject.transform.position += Vector3.Scale(movement, GetMoveInput());
    }

    Vector3 GetMoveInput()
    {
        Vector3 input = Vector3.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.x -= 1f;
            if (Keyboard.current.sKey.isPressed) input.x += 1f;
            if (Keyboard.current.dKey.isPressed) input.z += 1f;
            if (Keyboard.current.aKey.isPressed) input.z -= 1f;
        }

        // Clamp the result to avoid values higher than 1f for diagonals
        return Vector3.ClampMagnitude(input, 1f);
    }
}
