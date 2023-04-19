using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechControllerRB : MonoBehaviour
{
    
    public InputActionReference turnReference = null;
    public InputActionReference moveReference = null;
    public InputActionReference boostReference = null;
    public InputActionReference jumpReference = null;
    public InputActionReference quickTurnReference = null;
    public InputActionReference boostDirectionReference = null;
    

    public GameObject camera;

    public bool quickTurn = false;

    private Rigidbody mechRb;

    private bool jump = false;
    private bool crouch = false;
    [SerializeField] private bool isOnGround = true;
    private bool boost = false;
    private Quaternion targetRot;

    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float boostSpeed;
    [SerializeField] private Vector3 upperBounds;
    [SerializeField] private Vector3 lowerBounds;

    private void Awake()
    {
        boostReference.action.performed += Boost;
        jumpReference.action.performed += Jump;
        quickTurnReference.action.performed += QuickTurn;
    }

    private void OnDestroy()
    {
        boostReference.action.performed -= Boost;
        jumpReference.action.performed += Jump;
        quickTurnReference.action.performed -= QuickTurn;
    }
    // Start is called before the first frame update
    void Start()
    {
        mechRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        RotateMech();

        MoveMech();

        if (boost)
        {
            MechBoost();
        }

        if (jump)
        {
            MechJump();
        }

        if (crouch)
        {
            MechCrouch();
        }

        

        BoundMech();

        //output player velocity for debuging
        Debug.Log(mechRb.velocity);
    }

    // Update is called once per frame
    void Update()
    {
        


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

    private void QuickTurn(InputAction.CallbackContext context)
    {
        targetRot = Quaternion.AngleAxis(camera.transform.rotation.eulerAngles.y, Vector3.up);
        quickTurn = true;
    }

    private void Boost(InputAction.CallbackContext context)
    {
        boost = true;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isOnGround)
        {
            jump = true;
        }
    }

    private void RotateMech()
    {
        if (quickTurn)
        {
            mechRb.MoveRotation(targetRot);
            quickTurn = false;
        }
        else
        {
            float turnDirection = turnReference.action.ReadValue<float>();
            Quaternion detaRot = Quaternion.Euler(new Vector3(0, turnSpeed, 0) * turnDirection * Time.fixedDeltaTime);
            mechRb.MoveRotation(mechRb.rotation * detaRot);
        }
    }

    private void MoveMech()
    {
        Vector2 move = moveReference.action.ReadValue<Vector2>();

        mechRb.AddForce(transform.right * speed * move.x, ForceMode.Force);
        mechRb.AddForce(transform.forward * speed * move.y, ForceMode.Force);
    }

    private void MechJump()
    {
        mechRb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        jump = false;
        isOnGround = false;
    }

    private void MechCrouch()
    {
        mechRb.AddForce(Vector3.down * jumpSpeed, ForceMode.Impulse);
        crouch = false;
    }

    private void MechBoost()
    {
        Vector2 boostDirection = boostDirectionReference.action.ReadValue<Vector2>();

        mechRb.AddForce(mechRb.transform.right * boostSpeed * boostDirection.x, ForceMode.Impulse);
        mechRb.AddForce(mechRb.transform.forward * boostSpeed * boostDirection.y, ForceMode.Impulse);

        boost = false;
    }

    private void BoundMech()
    {
        float x = Mathf.Clamp(mechRb.transform.position.x, lowerBounds.x, upperBounds.x);
        float y = Mathf.Clamp(mechRb.transform.position.y, lowerBounds.y, upperBounds.y);
        float z = Mathf.Clamp(mechRb.transform.position.z, lowerBounds.z, upperBounds.z);
        mechRb.transform.position = new Vector3(x, y, z);
    }

}