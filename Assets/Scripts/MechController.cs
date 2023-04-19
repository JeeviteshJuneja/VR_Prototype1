using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechController : MonoBehaviour
{
    public InputActionReference boostReference = null;
    public InputActionReference turnReference = null;
    public InputActionReference quickTurnReference = null;
    public InputActionReference boostDirectionReference = null;
    public InputActionReference moveReference = null;

    public GameObject camera;

    private CharacterController characterController;

    private float boostSpeed = 25.0f;
    private float currentBoostSpeed = 0.0f;
    private float speed = 5.0f;
    private float gravity = 5.0f;
    private float turnSpeed = 60.0f;
    private Vector3 forwards;
    private bool boost = false;
    public bool quickTurn = false;
    private Quaternion startRot;
    private Quaternion targetRot;
    private float qSlerpParam = 0.0f;
    private float boostTime = 2.0f;
    private float timer = 0.0f;
    // Start is called before the first frame update

    private void Awake()
    {
        boostReference.action.performed += Boost;
        quickTurnReference.action.performed += QuickTurn;
    }

    private void OnDestroy()
    {
        boostReference.action.performed -= Boost;
        quickTurnReference.action.performed -= QuickTurn;
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        forwards = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (quickTurn)
        {
            qSlerpParam += Time.deltaTime;
            if (qSlerpParam > 1.0f) 
            { 
                quickTurn = false;
            }
            else
            {
                transform.rotation = Quaternion.Slerp(startRot, targetRot, qSlerpParam);
            }

        }
        else
        {
            float turn = turnReference.action.ReadValue<float>();
            transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * turn);
        }


        Vector3 velocity = characterController.velocity;
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        float verticalSpeed = characterController.velocity.y;
        Vector3 moveCharacter = new Vector3(0, 0, 0);

        Vector2 move = moveReference.action.ReadValue<Vector2>();

        moveCharacter = moveCharacter + transform.right * move.x * speed * Time.deltaTime;
        moveCharacter = moveCharacter + transform.forward * move.y * speed * Time.deltaTime;

        if (boost == true)
        {
            if (currentBoostSpeed < boostSpeed)
            {
                currentBoostSpeed += boostSpeed *2f * Time.deltaTime;
            }
            else
            {
                timer += Time.deltaTime;
            }
            moveCharacter = moveCharacter + forwards * Time.deltaTime * currentBoostSpeed;
        }
        else if (currentBoostSpeed > 0f)
        {
            moveCharacter = moveCharacter + forwards * Time.deltaTime * currentBoostSpeed;
            currentBoostSpeed -= boostSpeed * Time.deltaTime;
        }

        if (!characterController.isGrounded)
        {
            moveCharacter = moveCharacter + Vector3.down * gravity * Time.deltaTime;
        }

        if (timer > boostTime)
        {
            boost = false;
            timer = 0.0f;
        }

        characterController.Move(moveCharacter);
    }

    private void Boost(InputAction.CallbackContext context)
    {

        Vector2 boostDirection = boostDirectionReference.action.ReadValue<Vector2>();
        Vector3 horizontal = transform.right * 2f;
        Vector3 vertical = transform.up * 2f;
        forwards = (transform.forward + horizontal * boostDirection.x + vertical * boostDirection.y).normalized;
        timer = 0.0f;
        boost = true;
    }

    private void QuickTurn(InputAction.CallbackContext context)
    {
        startRot = transform.rotation;
        targetRot = Quaternion.AngleAxis(camera.transform.rotation.eulerAngles.y, Vector3.up);
        qSlerpParam = 0.0f;
        quickTurn = true;
    }
}
