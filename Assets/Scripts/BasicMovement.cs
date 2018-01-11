using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;

public class BasicMovement : MonoBehaviour {
    public float maxSpeed = 5.0f;
    public float rotationSpeed = 0.3f;
    public float jumpSpeed = 5;
    public LayerMask groundLayer;
    public float gravity = 9;
    public bool grounded;
    private Vector3 curSpeed;
    private CharacterController cController;
    public Vector3 moveDirection;


    // Use this for initialization
    void Start()
    {
        cController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = cController.isGrounded;
        //transform.position += input * speed * Time.deltaTime;
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float tmp = 2 - Input.GetAxis("Sprint");

        transform.Rotate(Vector3.up, 10.0f * mouseInput.x * rotationSpeed);

        if (cController.isGrounded){
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= (maxSpeed / tmp);
			moveDirection.y = 0;
            if (Input.GetAxisRaw("Jump") == 1)
                moveDirection.y = jumpSpeed;

        }
        else
            moveDirection.y -= gravity * Time.deltaTime;
        cController.Move(moveDirection * Time.deltaTime);

    }
   
}
