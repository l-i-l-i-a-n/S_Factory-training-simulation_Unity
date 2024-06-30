using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController myController;
    public Transform myEyes;
    public Transform myRightHand;
    
    public float walkSpeed = 12f;
    public float mouseSensitivity = 100f;
    public float mouseWheelSensitivity = 10f;
    
    // Enum determinant quel composant est controlé par les mouvements de la souris
    private enum MouseControlState
    {
        EyeRotation, // La souris contrôle la rotation de la camera
        RightHandRotation // la souris contrôle l'orientation de la main droite
    }
    
    
    private MouseControlState mouseControlState = MouseControlState.EyeRotation; // variable spécifiant quel composant est controlé par la souris
    
    private float eyeVerticalRotation = 0f;
    private float yVelocity = 0f;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        myController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Curseur bloqué au centre de l'écran, permettant de faire tourner
                                                  // librement la caméra sans risquer de sortir le curseur
                                                  // de la fenêtre de jeu
    }
    
    void Start()
    {
        eyeVerticalRotation = myEyes.localRotation.x;
    }
    
    // Update is called once per frame
    void Update()
    {
        
        // GESTION DES INPUTS
        
        mouseControlState = Input.GetKey(KeyCode.LeftShift) ? MouseControlState.RightHandRotation : MouseControlState.EyeRotation;
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        // Les commandes "Horizontal" et "Vertical" sont attribuées par défaut aux flèches directionnelles et W A S D (clavier QWERTY).
        // Pour modifier la configuration des touches (par exemple, pour attribuer Z Q S D plutôt),
        // se reporter au menu : Edit > Project Settings > Input Manager
        
        // Rq: La gestion des Inputs utilise ici l'ancien gestionnaire d'input de Unity.
        // Pour vous renseigner sur le nouveau système d'Input, n'hésitez pas à consulter ce lien : 
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Installation.html


        // GESTION SIMPLE DE LA GRAVITE
        
        if (myController.isGrounded) // Le composant CharacterController comporte une propriété permettant de déterminer
                                     // si le personnage est au contact du sol ou non
        {
            yVelocity = 0;
        }

        yVelocity = Physics.gravity.y * Time.deltaTime;

        // DEPLACEMENT DU PERSONNAGE
        
        Vector3 move = transform.right * hInput + transform.forward * vInput;
        
        myController.Move(move * (walkSpeed * Time.deltaTime));
        myController.Move(Vector3.up * (yVelocity));
        
        myRightHand.Translate(new Vector3(0,0,mouseWheelSensitivity*Time.deltaTime*Input.GetAxis("Mouse ScrollWheel")),Space.Self);
        
        if (mouseControlState == MouseControlState.RightHandRotation)
        {
            myRightHand.Rotate(new Vector3(-1*mouseY, mouseX, 0f), Space.Self);
        }
        else if (mouseControlState == MouseControlState.EyeRotation)
        {
            eyeVerticalRotation -= mouseY;
            eyeVerticalRotation = Mathf.Clamp(eyeVerticalRotation, -90f, 90f);

            myController.transform.Rotate(new Vector3(0f, mouseX, 0f), Space.Self);
            myEyes.localEulerAngles = new Vector3(eyeVerticalRotation, 0f, 0f);
        }
    }
}
